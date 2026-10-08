using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.RecurringTransactions;

public sealed class CreateRecurringTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringTransactionDto>> ExecuteAsync(
        CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var built = await BuildAsync(userId, command, cancellationToken);
        if (built.Error is not null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(built.Error);
        }

        await repository.AddAsync(built.Plan!, cancellationToken);
        return ApplicationResult<RecurringTransactionDto>.Success(ToDto(built.Plan!));
    }

    internal sealed record BuiltPlan(RecurringTransaction? Plan, ApplicationError? Error);

    /// <summary>
    /// İsteği doğrular ve planı kurar, <b>yazmaz</b>. Tek plan ve toplu vergi
    /// tanımlama aynı kuralla kurulur; ikincisi hepsini tek seferde yazar.
    /// </summary>
    internal async Task<BuiltPlan> BuildAsync(
        Guid userId,
        CreateRecurringTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var isTax = command.TaxKind is not null;
        var source = await RecurringSources.LoadAsync(
            userId,
            command.SourceType,
            command.AccountId,
            command.CreditCardId,
            command.Kind,
            isTax,
            requireActive: true,
            accountRepository,
            cardRepository,
            cancellationToken);
        if (source.Error is not null)
        {
            return new BuiltPlan(null, source.Error);
        }

        if (command.Amount is null && !isTax)
        {
            return new BuiltPlan(null, RecurringErrors.Validation("Amount is required."));
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            return new BuiltPlan(null, RecurringErrors.CategoryUnavailable);
        }

        if (isTax && !IsTaxCategory(category))
        {
            return new BuiltPlan(null, RecurringErrors.CategoryNotTax);
        }

        var resolution = isTax
            ? ScopeResolution.Resolved(TransactionScopeResolution.ResolveTax(
                command.Scope,
                await HasBusinessAsync(profileRepository, userId, cancellationToken)))
            : await TransactionScopeResolution.ResolveAsync(
                command.Scope,
                category.DefaultScope,
                source.Card?.DefaultScope ?? source.Account?.DefaultScope,
                profileRepository,
                userId,
                cancellationToken);
        if (resolution.Scope is not TransactionScope resolvedScope)
        {
            return new BuiltPlan(null, resolution.ToError(
                RecurringErrors.ScopeUnresolved, RecurringErrors.ScopeConflict));
        }

        try
        {
            var amount = command.Amount is decimal value ? new Money(value, command.Currency) : null;
            var id = Guid.NewGuid();
            var recurring = source switch
            {
                { Card: CreditCard card } => new RecurringTransaction(
                    id, userId, card, category, amount, command.Kind, resolvedScope,
                    command.Frequency, command.StartDate, command.EndDate, command.MonthEndBehavior,
                    command.Description, command.OccurrenceLimit, command.TaxKind,
                    command.DayOfMonth, command.SelectedMonths),
                { Account: Account account } => new RecurringTransaction(
                    id, userId, account, category, amount, command.Kind, resolvedScope,
                    command.Frequency, command.StartDate, command.EndDate, command.MonthEndBehavior,
                    command.Description, command.OccurrenceLimit, command.TaxKind,
                    command.DayOfMonth, command.SelectedMonths),
                _ => new RecurringTransaction(
                    id, userId, category, amount, command.TaxKind!.Value, resolvedScope,
                    command.Frequency, command.StartDate, command.EndDate, command.MonthEndBehavior,
                    command.Description, command.DayOfMonth, command.SelectedMonths)
            };
            return new BuiltPlan(recurring, null);
        }
        catch (ArgumentException exception)
        {
            return new BuiltPlan(null, RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return new BuiltPlan(null, RecurringErrors.Validation(exception.Message));
        }
    }

    /// <summary>
    /// Vergi planının kategorisi vergi işaretli bir gider kategorisidir: vergi
    /// ekranının "Ödenenler" listesi o kategorilerden okunur (ADR 0018 T6).
    /// </summary>
    internal static bool IsTaxCategory(Category category) =>
        category.IsTax && category.Type == CategoryType.Expense;

    /// <summary>Profili olmayan kullanıcı "işletmesi yok" sayılır.</summary>
    internal static async Task<bool> HasBusinessAsync(
        IUserProfileRepository profileRepository,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.FindAsync(userId, track: false, cancellationToken);
        return profile?.HasBusiness ?? false;
    }

    internal static RecurringTransactionDto ToDto(RecurringTransaction recurring) => new(
        recurring.Id,
        recurring.SourceType,
        recurring.AccountId,
        recurring.CreditCardId,
        recurring.CategoryId,
        recurring.AmountValue,
        recurring.Currency,
        recurring.Kind,
        recurring.Scope,
        recurring.Frequency,
        recurring.StartDate,
        recurring.EndDate,
        recurring.OccurrenceLimit,
        recurring.GeneratedOccurrenceCount,
        recurring.NextOccurrenceDate,
        recurring.MonthEndBehavior,
        recurring.Description,
        recurring.IsActive,
        recurring.TaxKind,
        recurring.DayOfMonth,
        recurring.SelectedMonths);
}

/// <summary>
/// Plan kaynağının istekten okunup sahiplik kapsamında yüklenmesi; oluşturma
/// ve düzenleme aynı kuralı kullanır.
/// </summary>
internal static class RecurringSources
{
    internal sealed record LoadedSource(Account? Account, CreditCard? Card, ApplicationError? Error);

    internal static async Task<LoadedSource> LoadAsync(
        Guid userId,
        RecurringSourceType? sourceType,
        Guid? accountId,
        Guid? creditCardId,
        RecurringTransactionKind kind,
        bool isTax,
        bool requireActive,
        IAccountRepository accountRepository,
        ICreditCardRepository cardRepository,
        CancellationToken cancellationToken)
    {
        switch (sourceType)
        {
            case null:
                // Kaynaksız plan yalnız vergidir (ADR 0018 T4); sıradan planın
                // kaynağı tam olarak biridir (ADR 0005).
                return isTax && accountId is null && creditCardId is null
                    ? new LoadedSource(null, null, null)
                    : new LoadedSource(null, null, RecurringErrors.InvalidSource);
            case RecurringSourceType.CreditCard:
                if (creditCardId is null || accountId is not null)
                {
                    return new LoadedSource(null, null, RecurringErrors.InvalidSource);
                }

                if (kind == RecurringTransactionKind.Income)
                {
                    return new LoadedSource(null, null, RecurringErrors.IncomeCardSourceNotSupported);
                }

                var card = await cardRepository.FindOwnedByIdAsync(
                    creditCardId.Value, userId, false, cancellationToken);
                return card is null || (requireActive && !card.IsActive)
                    ? new LoadedSource(null, null, RecurringErrors.CardUnavailable)
                    : new LoadedSource(null, card, null);
            default:
                if (accountId is null || creditCardId is not null)
                {
                    return new LoadedSource(null, null, RecurringErrors.InvalidSource);
                }

                var account = await accountRepository.FindOwnedByIdAsync(
                    accountId.Value, userId, cancellationToken);
                return account is null || (requireActive && !account.IsActive)
                    ? new LoadedSource(null, null, RecurringErrors.AccountUnavailable)
                    : new LoadedSource(account, null, null);
        }
    }
}

/// <summary>
/// Bir planın tam güncel hâlini yazar (ADR 0018 T2: vergi düzenlenir).
/// </summary>
/// <remarks>
/// <para>
/// <b>Ritim değişmezse</b> bekleyen kalemler planın yeni hâline gelir: ad,
/// kaynak, kategori, kapsam; tutar ise yalnız kalem plandan gelmişse —
/// kullanıcının o dönem için ayrıca yazdığı tutar ("tutar belli oldu") korunur.
/// </para>
/// <para>
/// <b>Ritim değişirse</b> bekleyen kalemler tahmindir ve yeni ritimle yeniden
/// kurulur; ödenmiş ve kapatılmış kalemler geçmiştir ve yerinde kalır. Yeni
/// ritim son ödenen kalemden sonra başlar: aynı güne ikinci bir kalem düşmesi
/// bir kalemin tek sonuç taşıması kuralını bozardı (kullanıcı kararı,
/// 30 Eylül 2026).
/// </para>
/// </remarks>
public sealed class UpdateRecurringTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    IUserProfileRepository profileRepository,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringTransactionDto>> ExecuteAsync(
        UpdateRecurringTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var recurring = await repository.FindOwnedByIdAsync(
            command.RecurringTransactionId, userId, track: true, cancellationToken);
        if (recurring is null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.NotFound(command.RecurringTransactionId));
        }

        // Pasif kaynak, planın zaten bağlı olduğu kaynaksa kabul edilir; yeni
        // bir pasif kaynağa bağlamayı domain reddeder.
        var source = await RecurringSources.LoadAsync(
            userId,
            command.SourceType,
            command.AccountId,
            command.CreditCardId,
            recurring.Kind,
            recurring.IsTax,
            requireActive: false,
            accountRepository,
            cardRepository,
            cancellationToken);
        if (source.Error is not null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(source.Error);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId, userId, cancellationToken);
        if (category is null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.CategoryUnavailable);
        }

        if (recurring.IsTax && !CreateRecurringTransactionUseCase.IsTaxCategory(category))
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.CategoryNotTax);
        }

        var resolution = recurring.IsTax
            ? ScopeResolution.Resolved(TransactionScopeResolution.ResolveTax(
                command.Scope,
                await CreateRecurringTransactionUseCase.HasBusinessAsync(
                    profileRepository, userId, cancellationToken)))
            : await TransactionScopeResolution.ResolveAsync(
                command.Scope,
                category.DefaultScope,
                source.Card?.DefaultScope ?? source.Account?.DefaultScope,
                profileRepository,
                userId,
                cancellationToken);
        if (resolution.Scope is not TransactionScope resolvedScope)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(resolution.ToError(
                RecurringErrors.ScopeUnresolved, RecurringErrors.ScopeConflict));
        }

        var occurrences = await repository.ListPlanOccurrencesAsync(
            recurring.Id, userId, cancellationToken);
        var rhythmChanged =
            recurring.Frequency != command.Frequency ||
            recurring.StartDate != command.StartDate ||
            recurring.EndDate != command.EndDate ||
            recurring.MonthEndBehavior != command.MonthEndBehavior ||
            recurring.DayOfMonth != command.DayOfMonth ||
            recurring.SelectedMonths != command.SelectedMonths;

        try
        {
            var previousAmount = recurring.AmountValue;
            recurring.Update(
                category,
                command.Amount is decimal value ? new Money(value, recurring.Currency) : null,
                resolvedScope,
                command.Description,
                source.Account,
                source.Card);

            var removed = Array.Empty<RecurringTransactionOccurrence>();
            if (rhythmChanged)
            {
                var settled = occurrences
                    .Where(occurrence => occurrence.Status != RecurringOccurrenceStatus.Planned)
                    .ToArray();
                DateOnly? lastSettled = settled.Length == 0 ? null : settled.Max(item => item.ScheduledDate);
                if (lastSettled is DateOnly last && command.StartDate <= last)
                {
                    return ApplicationResult<RecurringTransactionDto>.Failure(
                        RecurringErrors.RescheduleBeforeHistory);
                }

                removed = occurrences
                    .Where(occurrence => occurrence.Status == RecurringOccurrenceStatus.Planned)
                    .ToArray();
                recurring.Reschedule(
                    command.Frequency,
                    command.StartDate,
                    command.EndDate,
                    command.MonthEndBehavior,
                    command.DayOfMonth,
                    command.SelectedMonths,
                    lastSettled,
                    settled.Length);
            }
            else
            {
                foreach (var occurrence in occurrences)
                {
                    occurrence.FollowPlan(recurring, previousAmount);
                }
            }

            await repository.SaveEditAsync(recurring, removed, cancellationToken);
            return ApplicationResult<RecurringTransactionDto>.Success(
                CreateRecurringTransactionUseCase.ToDto(recurring));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }
}

public sealed class ListRecurringTransactionsUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<RecurringTransactionDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var recurring = await repository.ListAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecurringTransactionDto>>.Success(
            recurring.Select(CreateRecurringTransactionUseCase.ToDto).ToArray());
    }
}

public sealed class SetRecurringActiveUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringTransactionDto>> ExecuteAsync(
        SetRecurringActiveCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var recurring = await repository.FindOwnedByIdAsync(
            command.RecurringTransactionId, userId, true, cancellationToken);
        if (recurring is null)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.NotFound(command.RecurringTransactionId));
        }

        try
        {
            if (command.IsActive) recurring.Activate();
            else recurring.Deactivate();
            await repository.UpdateAsync(recurring, cancellationToken);
            return ApplicationResult<RecurringTransactionDto>.Success(
                CreateRecurringTransactionUseCase.ToDto(recurring));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RecurringTransactionDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }
}

public sealed class GenerateRecurringOccurrencesUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public const int MaximumBatchSize = 1000;

    public async Task<ApplicationResult<GenerateRecurringOccurrencesResult>> ExecuteAsync(
        GenerateRecurringOccurrencesCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        if (command.ThroughDate == default)
        {
            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.Validation("Through date is required."));
        }

        return await GenerateOnceAsync(userId, command.ThroughDate, true, cancellationToken);
    }

    private async Task<ApplicationResult<GenerateRecurringOccurrencesResult>> GenerateOnceAsync(
        Guid userId,
        DateOnly throughDate,
        bool allowRetry,
        CancellationToken cancellationToken)
    {
        var schedules = await repository.ListDueAsync(userId, throughDate, cancellationToken);
        var generated = new List<RecurringTransactionOccurrence>();
        var hasMoreDue = false;

        foreach (var schedule in schedules)
        {
            while (schedule.IsActive &&
                   schedule.NextOccurrenceDate is DateOnly nextDate &&
                   nextDate <= throughDate)
            {
                if (generated.Count == MaximumBatchSize)
                {
                    hasMoreDue = true;
                    break;
                }

                generated.Add(RecurringTransactionOccurrence.Create(
                    Guid.NewGuid(), schedule, nextDate));
                schedule.AdvanceAfter(nextDate);
            }

            if (hasMoreDue) break;
        }

        if (generated.Count > 0 && !await repository.TrySaveGeneratedAsync(generated, cancellationToken))
        {
            if (allowRetry)
            {
                return await GenerateOnceAsync(userId, throughDate, false, cancellationToken);
            }

            return ApplicationResult<GenerateRecurringOccurrencesResult>.Failure(
                RecurringErrors.Validation("Occurrence generation conflicted with another request."));
        }

        return ApplicationResult<GenerateRecurringOccurrencesResult>.Success(new(
            generated.Select(ToDto).ToArray(),
            hasMoreDue));
    }

    internal static RecurringOccurrenceDto ToDto(RecurringTransactionOccurrence occurrence) => new(
        occurrence.Id,
        occurrence.RecurringTransactionId,
        occurrence.OccurrenceKey,
        occurrence.SourceType,
        occurrence.AccountId,
        occurrence.CreditCardId,
        occurrence.CategoryId,
        occurrence.AmountValue,
        occurrence.Currency,
        occurrence.Kind,
        occurrence.Scope,
        occurrence.ScheduledDate,
        occurrence.Description,
        occurrence.Status,
        occurrence.BudgetTransactionId,
        occurrence.CreditCardChargeId,
        occurrence.RealizedAtUtc,
        occurrence.ClosedByTransactionId,
        occurrence.ClosedByChargeId,
        occurrence.ClosedAtUtc);
}

public sealed class ListRecurringOccurrencesUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var occurrences = await repository.ListOccurrencesAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<RecurringOccurrenceDto>>.Success(
            occurrences.Select(GenerateRecurringOccurrencesUseCase.ToDto).ToArray());
    }
}

public sealed class RealizeRecurringOccurrenceUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository,
    IAccountRepository accountRepository,
    ICreditCardRepository cardRepository,
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    ICardChargeRepository chargeRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> ExecuteAsync(
        RealizeRecurringOccurrenceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var occurrence = await repository.FindOccurrenceOwnedByIdAsync(
            command.OccurrenceId, userId, true, cancellationToken);
        if (occurrence is null)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.OccurrenceNotFound(command.OccurrenceId));
        }

        // Kapatılmış kalemin sonucu kendi kaydı değil, onu kapatan ödemedir;
        // ikinci bir sonuç aynı vergiyi iki kez gider yazardı.
        if (occurrence.Status == RecurringOccurrenceStatus.Closed)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AlreadySettled);
        }

        // Checked before either branch, but after the already-realized shortcut
        // inside them stays reachable: switching a plan off must not rewrite the
        // history it already produced.
        if (occurrence.Status == RecurringOccurrenceStatus.Planned &&
            !await repository.IsScheduleActiveAsync(
                occurrence.RecurringTransactionId, userId, cancellationToken))
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.ScheduleInactive);
        }

        var payment = command.Payment;
        if (occurrence.Status == RecurringOccurrenceStatus.Planned)
        {
            var prepared = PrepareForPayment(occurrence, command.Amount, payment);
            if (prepared is not null)
            {
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(prepared);
            }
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            occurrence.CategoryId, userId, cancellationToken);

        // "Ödedim" kaydı ödeme gününe yazar, vade gününe değil (ADR 0018 T4);
        // gün verilmezse eski davranış: vade günü.
        var recordDate = payment?.PaidOn ?? occurrence.ScheduledDate;
        return occurrence.SourceType == RecurringSourceType.CreditCard
            ? await RealizeCardAsync(occurrence, category, recordDate, userId, cancellationToken)
            : await RealizeAccountAsync(occurrence, category, recordDate, userId, cancellationToken);
    }

    /// <summary>
    /// Bekleyen kalemi ödemeye hazırlar: tutar ve kaynak. Hata yoksa
    /// <see langword="null"/>.
    /// </summary>
    private ApplicationError? PrepareForPayment(
        RecurringTransactionOccurrence occurrence,
        decimal? amount,
        RecurringPaymentDetails? payment)
    {
        if (payment?.PaidOn is DateOnly paidOn && paidOn > LatestAllowedPaymentDate())
        {
            return RecurringErrors.PaidOnInFuture;
        }

        try
        {
            // Tutar düzeltmesi yalnız bekleyen kaleme uygulanır: gerçekleşmiş
            // olan geçmiştir ve kısa devre aşağıda aynı sonucu geri verir.
            if (amount is decimal correctedAmount)
            {
                occurrence.CorrectAmount(new Money(correctedAmount, occurrence.Currency));
            }
            else if (occurrence.AmountValue is null)
            {
                return RecurringErrors.AmountRequired;
            }

            if (payment?.AccountId is not null && payment.CreditCardId is not null)
            {
                return RecurringErrors.InvalidSource;
            }

            if (payment?.AccountId is Guid accountId) occurrence.UseAccount(accountId);
            if (payment?.CreditCardId is Guid cardId) occurrence.UseCreditCard(cardId);
        }
        catch (ArgumentException exception)
        {
            return RecurringErrors.Validation(exception.Message);
        }

        return occurrence.SourceType is null ? RecurringErrors.SourceRequired : null;
    }

    /// <summary>
    /// Bugünden sonraki bir ödeme günü reddedilir. Sunucu günü UTC'dir;
    /// Türkiye'de gece yarısından sonraki ilk saatlerde kullanıcının "bugün"ü
    /// UTC'de yarındır, bu yüzden bir gün pay bırakılır.
    /// </summary>
    private DateOnly LatestAllowedPaymentDate() =>
        DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddDays(1);

    private async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> RealizeAccountAsync(
        RecurringTransactionOccurrence occurrence,
        Category? category,
        DateOnly recordDate,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Already realized: hand back the existing result so a retry stays a no-op.
        if (occurrence.BudgetTransactionId is Guid existingTransactionId)
        {
            var existing = await transactionRepository.FindOwnedByIdAsync(
                existingTransactionId, userId, false, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Realized occurrence transaction was not found.");
            }

            return Success(CreateTransactionUseCase.ToDto(existing));
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            occurrence.AccountId!.Value, userId, cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AccountUnavailable);
        }

        if (category is null || !category.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CategoryUnavailable);
        }

        try
        {
            var transaction = new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                occurrence.Amount!,
                occurrence.GetTransactionType(),
                occurrence.Scope,
                recordDate,
                occurrence.Description);
            occurrence.RealizeWithTransaction(transaction.Id, timeProvider.GetUtcNow());
            var persisted = await repository.RealizeAsync(occurrence, transaction, cancellationToken);
            return Success(CreateTransactionUseCase.ToDto(persisted));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }

    private async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> RealizeCardAsync(
        RecurringTransactionOccurrence occurrence,
        Category? category,
        DateOnly recordDate,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (occurrence.CreditCardChargeId is Guid existingChargeId)
        {
            var existing = await chargeRepository.FindOwnedByIdAsync(
                existingChargeId, userId, false, cancellationToken);
            if (existing is null)
            {
                throw new InvalidOperationException("Realized occurrence charge was not found.");
            }

            return Success(CreateCardChargeUseCase.ToDto(existing));
        }

        var card = await cardRepository.FindOwnedByIdAsync(
            occurrence.CreditCardId!.Value, userId, false, cancellationToken);
        if (card is null || !card.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CardUnavailable);
        }

        if (category is null || !category.IsActive)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CategoryUnavailable);
        }

        // Short limit produces no charge and no occurrence state change. The
        // occurrence stays planned and stays retryable once the limit recovers,
        // which is why there is no persisted failure state to clean up later.
        var currentDebt = await cardRepository.CalculateCurrentDebtAsync(
            card.Id, userId, cancellationToken);
        if (occurrence.Amount!.Amount > card.Limit.Amount - currentDebt)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.CardLimitInsufficient);
        }

        try
        {
            var charge = new CreditCardCharge(
                Guid.NewGuid(),
                userId,
                card,
                category,
                occurrence.Amount,
                occurrence.Scope,
                recordDate,
                occurrence.Description);
            occurrence.RealizeWithCharge(charge.Id, timeProvider.GetUtcNow());
            var persisted = await repository.RealizeWithChargeAsync(
                occurrence, charge, cancellationToken);
            return Success(CreateCardChargeUseCase.ToDto(persisted));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.Validation(exception.Message));
        }
    }

    private static ApplicationResult<RealizeRecurringOccurrenceResult> Success(
        TransactionDto transaction)
    {
        return ApplicationResult<RealizeRecurringOccurrenceResult>.Success(
            new RealizeRecurringOccurrenceResult(
                RecurringSourceType.Account, transaction, null));
    }

    private static ApplicationResult<RealizeRecurringOccurrenceResult> Success(
        CardChargeDto charge)
    {
        return ApplicationResult<RealizeRecurringOccurrenceResult>.Success(
            new RealizeRecurringOccurrenceResult(
                RecurringSourceType.CreditCard, null, charge));
    }
}

public sealed class DeleteRecurringTransactionUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(
        DeleteRecurringTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<Guid>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var result = await repository.DeleteOwnedIfUnrealizedAsync(
            command.RecurringTransactionId, userId, cancellationToken);

        return result switch
        {
            RecurringDeletionResult.Deleted =>
                ApplicationResult<Guid>.Success(command.RecurringTransactionId),
            RecurringDeletionResult.NotFound =>
                ApplicationResult<Guid>.Failure(
                    RecurringErrors.NotFound(command.RecurringTransactionId)),
            RecurringDeletionResult.HasRealizedHistory =>
                ApplicationResult<Guid>.Failure(RecurringErrors.HasRealizedHistory),
            _ => throw new InvalidOperationException("Unknown recurring deletion result.")
        };
    }
}

/// <summary>
/// Realizes the recurring item that falls on one date, generating its occurrence
/// row first when nobody has generated it yet.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this exists.</b> The planned view lists two kinds of recurring row:
/// occurrences that exist, and dates projected from the schedule that were never
/// generated. Only the first kind could be realized, and generating was a button
/// on another screen — so the projected rows carried a permanently disabled
/// action and the screen never said why. Generating is bookkeeping the system
/// needs for idempotency, not a decision the user should have to make; the
/// decision is to realize.
/// </para>
/// <para>
/// <b>Why it composes rather than duplicates.</b> Generation advances the
/// schedule and keys occurrences by owner and period; realization branches on the
/// source and writes either a movement or a card charge. Re-implementing either
/// here would be a second copy of a financial rule, and the two copies would
/// drift. The cost is two save boundaries instead of one: if realization fails
/// after generation succeeded, what is left is a pending occurrence — exactly the
/// state the old "generate" button produced, and the next attempt picks it up.
/// </para>
/// <para>
/// <b>Early payment.</b> Without a payment day the record is written on the due
/// day, so a future row cannot be realized — that would put money in a month it
/// did not leave. With a payment day (the tax screen's "Ödedim") the record goes
/// to that day, which is never in the future, so paying a tax before its due
/// date is allowed.
/// </para>
/// </remarks>
public sealed class RealizeDueRecurringUseCase(
    ICurrentUser currentUser,
    RecurringOccurrenceMaterializer materializer,
    RealizeRecurringOccurrenceUseCase realize,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RealizeRecurringOccurrenceResult>> ExecuteAsync(
        RealizeDueRecurringCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.AuthenticationRequired);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (command.Payment?.PaidOn is null && command.ScheduledDate > today)
        {
            return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                RecurringErrors.NotDueYet);
        }

        var lookup = await materializer.EnsureAsync(
            userId, command.RecurringTransactionId, command.ScheduledDate, cancellationToken);
        switch (lookup.Status)
        {
            case OccurrenceLookupStatus.PlanNotFound:
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                    RecurringErrors.NotFound(command.RecurringTransactionId));

            // An inactive plan must not produce money, and generating first would
            // leave rows behind for a plan the user switched off.
            case OccurrenceLookupStatus.PlanInactive:
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                    RecurringErrors.ScheduleInactive);

            // The date the client projected is not one this schedule actually falls
            // on. Reporting it as missing beats realizing the nearest one, which
            // would move money to a day the user never chose.
            case OccurrenceLookupStatus.NotOnSchedule:
                return ApplicationResult<RealizeRecurringOccurrenceResult>.Failure(
                    RecurringErrors.OccurrenceNotFound(command.RecurringTransactionId));
        }

        return await realize.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(lookup.Occurrence!.Id, command.Amount, command.Payment),
            cancellationToken);
    }
}

/// <summary>
/// Bekleyen bir kaleme bu dönemin tutarını yazar ("tutar belli oldu", ADR 0018
/// T3); plan değişmez.
/// </summary>
/// <remarks>
/// Kalem gün ve planla adreslenir, çünkü planlanan görünüm henüz üretilmemiş
/// günleri de gösterir. Tutar ileri bir tarih için de yazılabilir; asıl amacı
/// budur: muhasebeci tutarı söylediğinde vade gelmemiştir.
/// </remarks>
public sealed class SetOccurrenceAmountUseCase(
    ICurrentUser currentUser,
    RecurringOccurrenceMaterializer materializer,
    IRecurringTransactionRepository repository)
{
    public async Task<ApplicationResult<RecurringOccurrenceDto>> ExecuteAsync(
        SetOccurrenceAmountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var lookup = await materializer.EnsureAsync(
            userId, command.RecurringTransactionId, command.ScheduledDate, cancellationToken);
        switch (lookup.Status)
        {
            case OccurrenceLookupStatus.PlanNotFound:
                return ApplicationResult<RecurringOccurrenceDto>.Failure(
                    RecurringErrors.NotFound(command.RecurringTransactionId));
            case OccurrenceLookupStatus.PlanInactive:
                return ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.ScheduleInactive);
            case OccurrenceLookupStatus.NotOnSchedule:
                return ApplicationResult<RecurringOccurrenceDto>.Failure(
                    RecurringErrors.OccurrenceNotFound(command.RecurringTransactionId));
        }

        var occurrence = lookup.Occurrence!;
        if (occurrence.Status != RecurringOccurrenceStatus.Planned)
        {
            return ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.AlreadySettled);
        }

        try
        {
            occurrence.CorrectAmount(new Money(command.Amount, occurrence.Currency));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<RecurringOccurrenceDto>.Failure(
                RecurringErrors.Validation(exception.Message));
        }

        return await repository.TrySaveOccurrenceChangeAsync(cancellationToken)
            ? ApplicationResult<RecurringOccurrenceDto>.Success(
                GenerateRecurringOccurrencesUseCase.ToDto(occurrence))
            : ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.ConcurrentChange);
    }
}

/// <summary>
/// Gerçekleşmiş bir kalemin ödemesini geri alır (ADR 0018 T4, İ7).
/// </summary>
/// <remarks>
/// <para>
/// Ürettiği kayıt silinmez, iptal edilir; kalem bekleyene döner ve yeniden
/// ödenebilir. İkisi tek <c>SaveChanges</c> ile yazılır: iptal edilmiş bir
/// kayda bağlı "ödendi" kalem ya da bağsız bir iptal kalamaz.
/// </para>
/// <para>
/// Birleşik akıştaki köken kilidi (<c>*.cancel_origin_locked</c>) yerinde kalır:
/// planın ürettiği kayıt İşlemler'den iptal edilemez, geri alma kaynağın kendi
/// ekranından yapılır. Kalemin kaydettiği tutar korunur; yeniden ödenirken
/// değiştirilebilir.
/// </para>
/// <para>
/// Toplu ödemeyle kapatılmış kalem buradan geri alınmaz: kalem kendi sonucunu
/// taşımaz, geri alınacak şey onu kapatan ödemedir.
/// </para>
/// </remarks>
public sealed class UndoRecurringOccurrenceUseCase(
    ICurrentUser currentUser,
    IRecurringTransactionRepository repository,
    ITransactionRepository transactionRepository,
    ICardChargeRepository chargeRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RecurringOccurrenceDto>> ExecuteAsync(
        UndoRecurringOccurrenceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.AuthenticationRequired);
        }

        var occurrence = await repository.FindOccurrenceOwnedByIdAsync(
            command.OccurrenceId, userId, true, cancellationToken);
        if (occurrence is null)
        {
            return ApplicationResult<RecurringOccurrenceDto>.Failure(
                RecurringErrors.OccurrenceNotFound(command.OccurrenceId));
        }

        switch (occurrence.Status)
        {
            // Zaten bekliyor: tekrar gönderilen geri alma etkisizdir.
            case RecurringOccurrenceStatus.Planned:
                return ApplicationResult<RecurringOccurrenceDto>.Success(
                    GenerateRecurringOccurrencesUseCase.ToDto(occurrence));
            case RecurringOccurrenceStatus.Closed:
                return ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.ClosedByPayment);
        }

        var now = timeProvider.GetUtcNow();
        if (occurrence.BudgetTransactionId is Guid transactionId)
        {
            var transaction = await transactionRepository.FindOwnedByIdAsync(
                transactionId, userId, track: true, cancellationToken)
                ?? throw new InvalidOperationException("Realized occurrence transaction was not found.");
            transaction.Cancel(now);
        }
        else if (occurrence.CreditCardChargeId is Guid chargeId)
        {
            var charge = await chargeRepository.FindOwnedByIdAsync(
                chargeId, userId, true, cancellationToken)
                ?? throw new InvalidOperationException("Realized occurrence charge was not found.");
            charge.Cancel(now);
        }

        occurrence.Reopen();
        return await repository.TrySaveOccurrenceChangeAsync(cancellationToken)
            ? ApplicationResult<RecurringOccurrenceDto>.Success(
                GenerateRecurringOccurrencesUseCase.ToDto(occurrence))
            : ApplicationResult<RecurringOccurrenceDto>.Failure(RecurringErrors.ConcurrentChange);
    }
}
