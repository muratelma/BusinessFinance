using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

/// <summary>
/// "Vergi ödemesi ekle": hiçbir vergi tanımlamadan, tek tutarla ödenmiş vergi
/// (ADR 0018 İ4, T5).
/// </summary>
/// <remarks>
/// <para>
/// Ödeme yeni bir kayıt türü değildir (İ6): hesaptan ödendiyse bir gider,
/// kartla ödendiyse bir kart harcamasıdır ve vergi işaretli bir kategoriye
/// yazılır. Kayıt ödeme gününe yazılır (İ3).
/// </para>
/// <para>
/// Seçilen tanımlı kalemler <b>kapatılır</b>: kendi sonuçları olmaz, bu ödemenin
/// kimliğini taşırlar. Tutar kalemlere dağıtılmaz ve eşleştirilmez; çoğunun
/// tutarı zaten yoktur ve gecikme zammı ödenen tutarın içindedir. Kapatma ve
/// ödeme tek <c>SaveChanges</c> ile yazılır.
/// </para>
/// <para>
/// İstek kimliği ödeme kimliğini belirler (<see cref="RequestScopedId"/>):
/// aynı istek ikinci kez gelirse ilk ödeme döner, ikinci gider yazılmaz.
/// </para>
/// </remarks>
public sealed class CreateTaxPaymentUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository,
    IRecurringTransactionRepository recurringRepository,
    RecurringOccurrenceMaterializer materializer,
    ITaxPaymentRepository repository,
    TimeProvider timeProvider)
{
    public const int MaximumClosedItems = 50;
    internal const string IdPurpose = "tax-payment";

    public async Task<ApplicationResult<TaxPaymentDto>> ExecuteAsync(
        CreateTaxPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.AuthenticationRequired);
        }

        if (command.ClientRequestId == Guid.Empty)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.Validation("Client request id is required."));
        }

        var paymentId = RequestScopedId.Create(IdPurpose, userId, command.ClientRequestId);
        if (await repository.FindAsync(userId, paymentId, cancellationToken) is TaxPaymentDto existing)
        {
            return ApplicationResult<TaxPaymentDto>.Success(existing);
        }

        if (command.PaidOn == default ||
            command.PaidOn > DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddDays(1))
        {
            // Bir gün pay: Türkiye'de gece yarısından sonra kullanıcının
            // "bugün"ü UTC'de henüz yarındır.
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.PaidOnInFuture);
        }

        if ((command.AccountId is null) == (command.CreditCardId is null))
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.InvalidSource);
        }

        var closes = command.Closes ?? [];
        if (closes.Count > MaximumClosedItems || closes.Distinct().Count() != closes.Count)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(
                TaxErrors.Validation($"Close at most {MaximumClosedItems} distinct items."));
        }

        Money amount;
        try
        {
            amount = new Money(command.Amount, CurrencyCode.TRY);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.Validation(exception.Message));
        }

        Account? account = null;
        CreditCard? card = null;
        if (command.AccountId is Guid accountId)
        {
            account = await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken);
            if (account is null || !account.IsActive)
            {
                return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.AccountUnavailable);
            }
        }
        else
        {
            card = await cardRepository.FindOwnedByIdAsync(
                command.CreditCardId!.Value, userId, false, cancellationToken);
            if (card is null || !card.IsActive)
            {
                return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.CardUnavailable);
            }
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive || !CreateRecurringTransactionUseCase.IsTaxCategory(category))
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.CategoryNotTax);
        }

        var scope = TransactionScopeResolution.ResolveTax(
            command.Scope,
            await CreateRecurringTransactionUseCase.HasBusinessAsync(profileRepository, userId, cancellationToken));

        var occurrences = await LoadClosedItemsAsync(userId, closes, cancellationToken);
        if (occurrences.Error is not null)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(occurrences.Error);
        }

        if (card is not null)
        {
            var currentDebt = await cardRepository.CalculateCurrentDebtAsync(card.Id, userId, cancellationToken);
            if (amount.Amount > card.Limit.Amount - currentDebt)
            {
                return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.CardLimitInsufficient);
            }
        }

        BudgetTransaction? transaction = null;
        CreditCardCharge? charge = null;
        var now = timeProvider.GetUtcNow();
        try
        {
            if (account is not null)
            {
                transaction = new BudgetTransaction(
                    paymentId, userId, account, category, amount, TransactionType.Expense, scope,
                    command.PaidOn, command.Note);
                foreach (var occurrence in occurrences.Items) occurrence.CloseWithTransaction(paymentId, now);
            }
            else
            {
                charge = new CreditCardCharge(
                    paymentId, userId, card!, category, amount, scope, command.PaidOn, command.Note);
                foreach (var occurrence in occurrences.Items) occurrence.CloseWithCharge(paymentId, now);
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.Validation(exception.Message));
        }

        var saved = await repository.AddAsync(transaction, charge, cancellationToken);
        if (saved == TaxPaymentSaveResult.Conflict)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.ConcurrentChange);
        }

        var persisted = await repository.FindAsync(userId, paymentId, cancellationToken)
            ?? throw new InvalidOperationException("Saved tax payment could not be read back.");
        return ApplicationResult<TaxPaymentDto>.Success(persisted);
    }

    private sealed record ClosedItems(
        IReadOnlyList<RecurringTransactionOccurrence> Items,
        ApplicationError? Error);

    /// <summary>
    /// Kapatılacak kalemleri, gerekirse üreterek, izlenen hâlde yükler.
    /// </summary>
    /// <remarks>
    /// Üretim bir çakışmada izlenen her şeyi bırakır; bu yüzden önce hepsi
    /// üretilir, sonra hepsi yeniden okunur — bir çakışma daha önce okunmuş bir
    /// kalemi izlenmez bırakırsa kapatması sessizce yazılmazdı.
    /// </remarks>
    private async Task<ClosedItems> LoadClosedItemsAsync(
        Guid userId,
        IReadOnlyList<TaxItemReference> closes,
        CancellationToken cancellationToken)
    {
        foreach (var reference in closes)
        {
            var plan = await recurringRepository.FindOwnedByIdAsync(
                reference.RecurringTransactionId, userId, false, cancellationToken);
            if (plan is null)
            {
                return new ClosedItems([], TaxErrors.ItemNotFound(reference.RecurringTransactionId));
            }

            if (!plan.IsTax)
            {
                return new ClosedItems([], TaxErrors.ItemNotTax);
            }

            var lookup = await materializer.EnsureAsync(
                userId, reference.RecurringTransactionId, reference.ScheduledDate, cancellationToken);
            switch (lookup.Status)
            {
                case OccurrenceLookupStatus.PlanNotFound:
                case OccurrenceLookupStatus.NotOnSchedule:
                    return new ClosedItems([], TaxErrors.ItemNotFound(reference.RecurringTransactionId));
                case OccurrenceLookupStatus.PlanInactive:
                    return new ClosedItems([], TaxErrors.ItemInactive);
            }
        }

        var items = new List<RecurringTransactionOccurrence>();
        foreach (var reference in closes)
        {
            var occurrence = await recurringRepository.FindOccurrenceOwnedByDateAsync(
                reference.RecurringTransactionId, reference.ScheduledDate, userId, cancellationToken, track: true)
                ?? throw new InvalidOperationException("Materialized occurrence disappeared.");
            if (occurrence.Status != RecurringOccurrenceStatus.Planned)
            {
                return new ClosedItems([], TaxErrors.ItemNotPending);
            }

            items.Add(occurrence);
        }

        return new ClosedItems(items, null);
    }
}

/// <summary>
/// Vergi ekranından "Ödemeyi geri al" (ADR 0018 T4, İ7).
/// </summary>
/// <remarks>
/// <para>
/// Ödeme bir kalemin "Ödedim" sonucuysa kalemin geri alması uygulanır: kayıt
/// iptal, kalem bekleyene. Değilse ödeme iptal edilir ve kapattığı kalemler
/// bekleyene döner; ikisi tek <c>SaveChanges</c> ile yazılır. Kayıt silinmez.
/// </para>
/// <para>
/// Aynı sonuç İşlemler'den iptalle de alınır: iptal, kapattığı kalemleri
/// kendiliğinden açar. Bu uç, vergi ekranının tek kapıdan hangisinin
/// geçerli olduğunu bilmek zorunda kalmaması içindir.
/// </para>
/// </remarks>
public sealed class UndoTaxPaymentUseCase(
    ICurrentUser currentUser,
    ITransactionRepository transactionRepository,
    ICardChargeRepository chargeRepository,
    IRecurringTransactionRepository recurringRepository,
    IActivityOriginReader originReader,
    UndoRecurringOccurrenceUseCase undoOccurrence,
    ITaxPaymentRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<TaxPaymentDto>> ExecuteAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.AuthenticationRequired);
        }

        var transaction = await transactionRepository.FindOwnedByIdAsync(
            paymentId, userId, track: true, cancellationToken);
        var charge = transaction is null
            ? await chargeRepository.FindOwnedByIdAsync(paymentId, userId, true, cancellationToken)
            : null;
        // Vergi ödemesi bir giderdir; gelir kaydı bu kapıdan geri alınmaz.
        if ((transaction is null && charge is null) ||
            transaction is { Type: not TransactionType.Expense })
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.PaymentNotFound(paymentId));
        }

        var payment = await repository.FindAsync(userId, paymentId, cancellationToken);
        var origin = transaction is not null
            ? await originReader.GetTransactionOriginAsync(userId, paymentId, cancellationToken)
            : await originReader.GetCardChargeOriginAsync(userId, paymentId, cancellationToken);
        // Taksit ve POS yatışı kesintisi kendi kaynağından geri alınır; buradan
        // iptal etmek kaynağı iptal edilmiş bir kayda bağlı bırakırdı.
        if (origin is FinancialActivityOrigin.Installment
            or FinancialActivityOrigin.PosDeposit
            or FinancialActivityOrigin.DayClose)
        {
            return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.UndoOriginLocked);
        }

        if (payment?.RealizedItem is TaxSettledItemDto realized)
        {
            var undone = await undoOccurrence.ExecuteAsync(
                new UndoRecurringOccurrenceCommand(realized.OccurrenceId), cancellationToken);
            if (!undone.IsSuccess) return ApplicationResult<TaxPaymentDto>.Failure(undone.Error);
        }
        else
        {
            var closed = await recurringRepository.ListClosedByAsync(
                userId,
                transaction?.Id,
                charge?.Id,
                cancellationToken);
            var now = timeProvider.GetUtcNow();
            transaction?.Cancel(now);
            charge?.Cancel(now);
            foreach (var occurrence in closed) occurrence.Reopen();
            if (!await repository.TrySaveAsync(cancellationToken))
            {
                return ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.ConcurrentChange);
            }
        }

        var after = await repository.FindAsync(userId, paymentId, cancellationToken);
        return after is null
            ? ApplicationResult<TaxPaymentDto>.Failure(TaxErrors.PaymentNotFound(paymentId))
            : ApplicationResult<TaxPaymentDto>.Success(after);
    }
}

/// <summary>
/// Vergi ekranının okuması: tanımlı vergiler, bekleyenler, son ödenenler.
/// </summary>
public sealed class GetTaxOverviewUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository recurringRepository,
    IPlannedActivityRepository plannedRepository,
    ITaxPaymentRepository repository)
{
    public const int RecentPaymentCount = 5;

    public async Task<ApplicationResult<TaxOverviewDto>> ExecuteAsync(
        DateOnly asOfDate,
        int daysAhead,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TaxOverviewDto>.Failure(TaxErrors.AuthenticationRequired);
        }

        if (asOfDate == default || asOfDate.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear)
        {
            return ApplicationResult<TaxOverviewDto>.Failure(TaxErrors.InvalidAsOfDate);
        }

        if (!ListPlannedActivitiesUseCase.AllowedDaysAhead.Contains(daysAhead))
        {
            return ApplicationResult<TaxOverviewDto>.Failure(TaxErrors.InvalidDaysAhead);
        }

        var plans = (await recurringRepository.ListAsync(userId, cancellationToken))
            .Where(plan => plan.IsTax)
            .Select(CreateRecurringTransactionUseCase.ToDto)
            .ToArray();

        // Bekleyenler planlanan projection'ın daraltılmış görünümüdür (İ6);
        // vergi için ikinci bir sorgu yoktur. Kapsam filtresi yok: ödenecek
        // vergi tek havuzdan çıkar.
        var pending = (await plannedRepository.ListAsync(
                userId, asOfDate, asOfDate.AddDays(daysAhead), scope: null, cancellationToken))
            .Where(item => item.TaxKind is not null)
            .OrderByDescending(item => item.Timing == PlannedActivityTiming.Overdue)
            .ThenBy(item => item.DueDate)
            .ThenBy(item => item.Title, StringComparer.Ordinal)
            .ThenBy(item => item.PlannedActivityId)
            .ToArray();

        var payments = await repository.ListAsync(userId, 0, RecentPaymentCount, cancellationToken);
        return ApplicationResult<TaxOverviewDto>.Success(new TaxOverviewDto(
            asOfDate,
            daysAhead,
            plans,
            pending,
            pending.Sum(item => item.Amount ?? 0m),
            pending.Count(item => item.Amount is null),
            payments.Items,
            payments.HasMore));
    }
}

/// <summary>"Ödenenler › Tümü": vergi ödemelerinin sayfalı listesi.</summary>
public sealed class ListTaxPaymentsUseCase(
    ICurrentUser currentUser,
    ITaxPaymentRepository repository)
{
    public const int MaximumTake = 100;

    public async Task<ApplicationResult<TaxPaymentPage>> ExecuteAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TaxPaymentPage>.Failure(TaxErrors.AuthenticationRequired);
        }

        if (skip < 0 || take is < 1 or > MaximumTake)
        {
            return ApplicationResult<TaxPaymentPage>.Failure(TaxErrors.InvalidPage);
        }

        return ApplicationResult<TaxPaymentPage>.Success(
            await repository.ListAsync(userId, skip, take, cancellationToken));
    }
}

/// <summary>
/// Bir vergi tanımının ayrıntısı: sıradaki kalemler ve geçmiş ödemeler.
/// </summary>
public sealed class GetTaxPlanDetailUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository recurringRepository,
    IPlannedActivityRepository plannedRepository,
    ITaxPaymentRepository repository)
{
    /// <summary>Gösterilen sıradaki (vadesi gelmemiş) kalem sayısı.</summary>
    public const int UpcomingCount = 3;

    /// <summary>
    /// Sıradaki kalemler için bakılan en uzak ufuk: yılda bir ödenen bir
    /// verginin üç kalemi bile bu ufka sığar.
    /// </summary>
    public const int UpcomingHorizonYears = 3;

    public async Task<ApplicationResult<TaxPlanDetailDto>> ExecuteAsync(
        Guid recurringTransactionId,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TaxPlanDetailDto>.Failure(TaxErrors.AuthenticationRequired);
        }

        if (asOfDate == default || asOfDate.Year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear)
        {
            return ApplicationResult<TaxPlanDetailDto>.Failure(TaxErrors.InvalidAsOfDate);
        }

        var plan = await recurringRepository.FindOwnedByIdAsync(
            recurringTransactionId, userId, false, cancellationToken);
        if (plan is null || !plan.IsTax)
        {
            return ApplicationResult<TaxPlanDetailDto>.Failure(TaxErrors.PlanNotFound(recurringTransactionId));
        }

        // Gecikmiş kalemlerin hepsi, ardından vadesi gelmemiş ilk üçü. Pencere
        // yerine sayı: üç ayda ya da yılda bir ödenen vergide sabit bir pencere
        // "Sıradaki"yi çoğu zaman boş bırakırdı.
        var items = (await plannedRepository.ListForRecurringPlanAsync(
                userId,
                recurringTransactionId,
                asOfDate,
                asOfDate.AddYears(UpcomingHorizonYears),
                cancellationToken))
            .OrderBy(item => item.DueDate)
            .ToArray();
        var upcoming = items
            .Where(item => item.Timing == PlannedActivityTiming.Overdue)
            .Concat(items
                .Where(item => item.Timing != PlannedActivityTiming.Overdue)
                .Take(UpcomingCount))
            .ToArray();
        var history = await repository.ListPlanHistoryAsync(userId, recurringTransactionId, cancellationToken);
        return ApplicationResult<TaxPlanDetailDto>.Success(new TaxPlanDetailDto(
            CreateRecurringTransactionUseCase.ToDto(plan), upcoming, history));
    }
}

/// <summary>
/// "Vergilerimi tanımla": ilk kurulumda seçilen vergilerin hepsi birden
/// (ADR 0018 T2). Ya hepsi yazılır ya hiçbiri: yarısı kurulmuş bir liste
/// kullanıcıya hangisinin eksik kaldığını aratırdı.
/// </summary>
/// <remarks>
/// Her plan tek plan oluşturmayla aynı kuralla kurulur; ayrı bir doğrulama yolu
/// yoktur. Kapsam verilmemişse profilin tarafıdır (İ9).
/// </remarks>
public sealed class CreateTaxPlansUseCase(
    ICurrentUser currentUser,
    CreateRecurringTransactionUseCase createPlan,
    IRecurringTransactionRepository repository)
{
    public const int MaximumPlans = 20;

    public async Task<ApplicationResult<IReadOnlyList<RecurringTransactionDto>>> ExecuteAsync(
        IReadOnlyList<CreateRecurringTransactionCommand> commands,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(commands);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(TaxErrors.AuthenticationRequired);
        }

        if (commands.Count == 0)
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(TaxErrors.PlansEmpty);
        }

        if (commands.Count > MaximumPlans)
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(
                TaxErrors.TooManyPlans(MaximumPlans));
        }

        if (commands.Any(command => command.TaxKind is null))
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(TaxErrors.PlanNotTax);
        }

        var plans = new List<RecurringTransaction>(commands.Count);
        foreach (var command in commands)
        {
            var built = await createPlan.BuildAsync(userId, command, cancellationToken);
            if (built.Error is not null)
            {
                return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(built.Error);
            }

            plans.Add(built.Plan!);
        }

        await repository.AddRangeAsync(plans, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Success(
            [.. plans.Select(CreateRecurringTransactionUseCase.ToDto)]);
    }
}
