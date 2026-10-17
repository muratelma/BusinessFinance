using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Debts;

public static class DebtErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "auth.authentication_required", "Authentication is required.", ApplicationErrorType.Unauthorized);
    public static ApplicationError NotFound(Guid id) => new(
        "debt.not_found", $"Debt '{id}' was not found.", ApplicationErrorType.NotFound);
    /// <summary>
    /// Kapsam ne istekten, ne açılış hesabından, ne kategoriden çözülebildi.
    /// </summary>
    public static readonly ApplicationError ScopeUnresolved = new(
        "debt.scope_unresolved",
        "The scope could not be resolved from the request, the account or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "debt.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError AccountUnavailable = new(
        "debt.account_unavailable", "An active owned account is required.", ApplicationErrorType.Validation);
    public static readonly ApplicationError CategoryUnavailable = new(
        "debt.category_unavailable",
        "An active owned category of the matching type is required.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError RepaymentRequired = new(
        "debt.repayment_required",
        "Either a total repayment or an annual interest rate is required.",
        ApplicationErrorType.Validation);
    public static readonly ApplicationError RepaymentConflict = new(
        "debt.repayment_conflict",
        "The total repayment and the annual interest rate do not agree.",
        ApplicationErrorType.Validation);
    /// <summary>
    /// Borcun başladığı gün ya da taksidin ödendiği gün ileride. Vade ileride
    /// olabilir; olmuş olan olamaz.
    /// </summary>
    public static readonly ApplicationError DateInFuture = new(
        "debt.date_in_future",
        "The start date and the payment date cannot be in the future.",
        ApplicationErrorType.Validation);
    public static ApplicationError Validation(string message) => new(
        "debt.validation", message, ApplicationErrorType.Validation);
    public static ApplicationError Conflict(string message) => new(
        "debt.conflict", message, ApplicationErrorType.Conflict);

    /// <summary>
    /// Borcun karşı tarafı okunamadı. Sözleşme adını karşı taraftan aldığı
    /// için ad olmadan cevap üretilemez.
    /// </summary>
    public static readonly ApplicationError CounterpartyUnavailable = new(
        "debt.counterparty_unavailable",
        "An owned counterparty is required.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Kaynağın hangi tip kategoriyi istediği.
    /// </summary>
    /// <remarks>
    /// Borç tüketir (gider), alacak satar (gelir). Ters eşleşme parayı yanlış
    /// tarafa yazardı; Domain de aynı kuralı bağımsız olarak uyguluyor, buradaki
    /// kontrol yalnız kullanıcıya net bir hata kodu döndürmek için.
    /// </remarks>
    internal static CategoryType RequiredCategoryType(DebtSourceType sourceType) =>
        sourceType == DebtSourceType.Income ? CategoryType.Income : CategoryType.Expense;

    /// <summary>Kapanış kalanı aşıyor; ekran yolunu yönüne göre söyler (ADR 0021 T2).</summary>
    public static ApplicationError AmountExceedsRemaining(decimal remaining) => new(
        "debt.amount_exceeds_remaining",
        $"The amount exceeds the remaining {remaining.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture)}.",
        ApplicationErrorType.Validation);

    /// <summary>Faizli borç yalnız tam taksitle ödenir; planı değişmez.</summary>
    public static readonly ApplicationError InterestBearing = new(
        "debt.interest_bearing",
        "An interest-bearing debt is paid in whole installments and keeps its plan.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CardUnavailable = new(
        "debt.card_unavailable", "An active owned credit card is required.", ApplicationErrorType.Validation);

    /// <summary>Ödemenin yeri tam olarak biridir: hesap, kredi kartı ya da POS.</summary>
    public static readonly ApplicationError PaymentSourceInvalid = new(
        "debt.payment_source_invalid",
        "A payment is made from exactly one account, credit card or POS; a card pays a debt and a POS collects a receivable.",
        ApplicationErrorType.Validation);

    public static ApplicationError PaymentNotFound(Guid id) => new(
        "debt.payment_not_found", $"Debt payment '{id}' was not found.", ApplicationErrorType.NotFound);

    /// <summary>Kapanış bir gün sonunda sayıldı; önce gün sonu geri alınır.</summary>
    public static readonly ApplicationError DayCloseCounted = new(
        "debt.day_close_counted",
        "The record was counted by a day close; undo the day close first.",
        ApplicationErrorType.Conflict);

    /// <summary>Kartla tahsilin parası bir yatışla hesaba geçti; önce yatış geri alınır.</summary>
    public static readonly ApplicationError DepositLocked = new(
        "debt.deposit_locked",
        "The card collection is held by a deposit; undo the deposit first.",
        ApplicationErrorType.Conflict);

    /// <summary>Borç bu sırada başka bir istekle değişti.</summary>
    public static readonly ApplicationError Changed = new(
        "debt.changed", "The debt changed while it was being written.", ApplicationErrorType.Conflict);

    /// <summary>İndirim yalnız kayıtlı mal ya da hizmet borcundadır.</summary>
    public static readonly ApplicationError DiscountUnavailable = new(
        "debt.discount_unavailable",
        "Only a recorded purchase or sale can be discounted.",
        ApplicationErrorType.Validation);

    /// <summary>Kişisiz borç açıklama taşır; aksi hâlde listede ayırt edilemez.</summary>
    public static readonly ApplicationError DescriptionRequired = new(
        "debt.description_required",
        "A debt without a person needs a description.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError DueDateRequired = new(
        "debt.due_date_required", "This plan needs a due date.", ApplicationErrorType.Validation);
}

public sealed class CreateDebtUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository,
    IUserProfileRepository profileRepository,
    TimeProvider timeProvider,
    ICreditCardRepository creditCardRepository)
{
    // Toplam ile oran birlikte gelirse ne kadar sapma hoş görülür. İstemci
    // oranı toplamdan çözüp geri gönderdiğinde oran dört ondalığa yuvarlanır
    // ve toplam kuruşun altında kayar; bu gerçek bir çelişki değildir. Bir
    // kuruşun üstü ise kullanıcının iki farklı sayı yazdığı anlamına gelir.
    private const decimal RepaymentAgreementTolerance = 0.01m;

    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        CreateDebtCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);

        // Borç alındığı ya da verildiği gün yazılır: açılış o gün hesabı
        // değiştirir ya da gideri tanır. İlk vade ileride olabilir.
        if (command.StartDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DateInFuture);

        var legacy = command.PlanKind == DebtPlanKind.Installments && !command.IsCarriedOver &&
                     command.UpfrontPayment is null && command.ClientRequestId is null &&
                     command.CounterpartyId is null && !string.IsNullOrWhiteSpace(command.CounterpartyName);
        if (!legacy)
        {
            return await CreateAsync(userId, command, asOfDate, cancellationToken);
        }

        if (command.FirstDueDate is null)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DueDateRequired);

        var totalRepayment = ResolveTotalRepayment(command);
        if (totalRepayment.Error is not null)
            return ApplicationResult<DebtDto>.Failure(totalRepayment.Error);

        Account? openingAccount = null;
        if (command.OpeningAccountId is Guid accountId)
        {
            openingAccount = await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken);
            if (openingAccount is null || !openingAccount.IsActive)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
        }

        Category? category = null;
        if (command.CategoryId is Guid categoryId)
        {
            category = await categoryRepository.FindOwnedByIdAsync(categoryId, userId, cancellationToken);
            if (category is null || !category.IsActive ||
                category.Type != DebtErrors.RequiredCategoryType(command.SourceType))
                return ApplicationResult<DebtDto>.Failure(DebtErrors.CategoryUnavailable);
        }

        // Kategorisi olmayan borç (nakit alma ve verme) iki tarafa açık
        // kategori gibi davranır.
        var resolution = await TransactionScopeResolution.ResolveAsync(
            command.Scope,
            category?.DefaultScope,
            openingAccount?.DefaultScope,
            profileRepository,
            userId,
            cancellationToken);
        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<DebtDto>.Failure(resolution.ToError(
                DebtErrors.ScopeUnresolved, DebtErrors.ScopeConflict));
        }

        // Karşı taraf en sonda kuruluyor: bu noktadan sonra yalnız aggregate
        // doğrulaması kalıyor ve kaydetme borcunkiyle aynı sınırda oluyor.
        Counterparty counterparty;
        try
        {
            counterparty = await counterpartyRepository.FindOrCreateByNameAsync(
                userId, command.CounterpartyName!, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }

        try
        {
            var debt = Debt.CreateInstallments(
                Guid.NewGuid(), userId, counterparty, command.Direction,
                scope,
                new Money(command.Principal, command.Currency),
                new Money(totalRepayment.Value, command.Currency),
                command.SourceType, openingAccount, category,
                command.StartDate, command.FirstDueDate.Value,
                command.InstallmentCount, command.Description);
            await repository.AddAsync(debt, cancellationToken);
            return ApplicationResult<DebtDto>.Success(
                ToDto(debt, counterparty.Name, asOfDate));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
    }

    /// <summary>
    /// Dalga 2 yolu (ADR 0021 T1, T5, T7): her plan, isteğe bağlı kişi,
    /// uygulamadan önceki borç, peşin ödeme ve istek kimliği. Aynı istek kimliği
    /// aynı borcu döner; ikinci bir borç açmaz.
    /// </summary>
    private async Task<ApplicationResult<DebtDto>> CreateAsync(
        Guid userId,
        CreateDebtCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var id = command.ClientRequestId is Guid requestId
            ? RequestScopedId.Create("debt", userId, requestId)
            : Guid.NewGuid();
        if (command.ClientRequestId is not null &&
            await repository.FindAnyOwnedAsync(id, userId, false, cancellationToken) is Debt existing)
        {
            return ApplicationResult<DebtDto>.Success(
                ToDto(existing, await NameOfAsync(existing, userId, cancellationToken), asOfDate));
        }

        if (command.PlanKind != DebtPlanKind.None && command.FirstDueDate is null)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DueDateRequired);
        var count = command.PlanKind == DebtPlanKind.Installments ? command.InstallmentCount : 1;
        var totalRepayment = ResolveTotalRepayment(command with { InstallmentCount = count });
        if (totalRepayment.Error is not null)
            return ApplicationResult<DebtDto>.Failure(totalRepayment.Error);
        if (command.PlanKind == DebtPlanKind.None && totalRepayment.Value > command.Principal)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.InterestBearing);

        Account? openingAccount = null;
        Category? category = null;
        if (command.IsCarriedOver)
        {
            // Devirde para ve gelir/gider yazılmaz: hesap ve kategori alınmaz.
            if (command.OpeningAccountId is not null || command.CategoryId is not null)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(
                    "A carried-over debt has no opening account or category."));
        }
        else
        {
            if (command.OpeningAccountId is Guid accountId)
            {
                openingAccount = await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken);
                if (openingAccount is null || !openingAccount.IsActive)
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
            }

            if (command.CategoryId is Guid categoryId)
            {
                category = await categoryRepository.FindOwnedByIdAsync(categoryId, userId, cancellationToken);
                if (category is null || !category.IsActive ||
                    category.Type != DebtErrors.RequiredCategoryType(command.SourceType))
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.CategoryUnavailable);
            }
        }

        Counterparty? counterparty = null;
        if (command.CounterpartyId is Guid counterpartyId)
        {
            counterparty = await counterpartyRepository.FindOwnedByIdAsync(counterpartyId, userId, cancellationToken);
            if (counterparty is null)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.CounterpartyUnavailable);
        }
        else if (string.IsNullOrWhiteSpace(command.Description) &&
                 string.IsNullOrWhiteSpace(command.CounterpartyName))
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DescriptionRequired);
        }

        var resolution = await TransactionScopeResolution.ResolveAsync(
            command.Scope,
            category?.DefaultScope,
            openingAccount?.DefaultScope,
            profileRepository,
            userId,
            cancellationToken);
        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<DebtDto>.Failure(resolution.ToError(
                DebtErrors.ScopeUnresolved, DebtErrors.ScopeConflict));
        }

        Account? upfrontAccount = null;
        CreditCard? upfrontCard = null;
        if (command.UpfrontPayment is DebtUpfrontPayment upfront)
        {
            if ((upfront.AccountId is null) == (upfront.CreditCardId is null))
                return ApplicationResult<DebtDto>.Failure(DebtErrors.PaymentSourceInvalid);
            if (upfront.AccountId is Guid upfrontAccountId)
            {
                upfrontAccount = await accountRepository.FindOwnedByIdAsync(upfrontAccountId, userId, cancellationToken);
                if (upfrontAccount is null || !upfrontAccount.IsActive)
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
            }
            else
            {
                upfrontCard = await creditCardRepository.FindOwnedByIdAsync(
                    upfront.CreditCardId!.Value, userId, false, cancellationToken);
                if (upfrontCard is null || !upfrontCard.IsActive)
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.CardUnavailable);
            }
        }

        try
        {
            if (counterparty is null && !string.IsNullOrWhiteSpace(command.CounterpartyName))
            {
                counterparty = await counterpartyRepository.FindOrCreateByNameAsync(
                    userId, command.CounterpartyName, cancellationToken);
            }

            var now = timeProvider.GetUtcNow().ToUniversalTime();
            var principal = new Money(command.Principal, command.Currency);
            var total = new Money(totalRepayment.Value, command.Currency);
            var debt = command.IsCarriedOver
                ? Debt.CarriedOver(
                    id, userId, counterparty, command.Direction, scope, command.SourceType,
                    principal, total, command.StartDate, command.PlanKind, command.FirstDueDate,
                    count, command.Description, now)
                : Debt.Create(
                    id, userId, counterparty, command.Direction, scope, principal, total,
                    command.SourceType, openingAccount, category, command.StartDate,
                    command.PlanKind, command.FirstDueDate, count, command.Description, now);

            DebtPayment? closing = null;
            if (command.UpfrontPayment is DebtUpfrontPayment paid)
            {
                if (debt.BearsInterest)
                    await categoryRepository.FindOrOpenAsync(userId,
                        command.Direction == DebtDirection.Receivable
                            ? CategoryRole.InterestIncome : CategoryRole.InterestExpense,
                        cancellationToken);
                closing = debt.Pay(
                    RequestScopedId.Create("debt-upfront", userId, id), paid.Amount,
                    command.StartDate, now, upfrontAccount, upfrontCard);
            }

            await repository.AddWithClosingAsync(debt, closing, null, cancellationToken);
            return ApplicationResult<DebtDto>.Success(
                ToDto(debt, counterparty?.Name ?? string.Empty, asOfDate));
        }
        catch (DebtAmountExceedsRemainingException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AmountExceedsRemaining(exception.Remaining));
        }
        catch (DebtConcurrencyException)
        {
            // Aynı istek kimliği bu sırada yazıldı: ilkini döner.
            var winner = await repository.FindAnyOwnedAsync(id, userId, false, cancellationToken);
            return winner is null
                ? ApplicationResult<DebtDto>.Failure(DebtErrors.Changed)
                : ApplicationResult<DebtDto>.Success(
                    ToDto(winner, await NameOfAsync(winner, userId, cancellationToken), asOfDate));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
    }

    private async Task<string> NameOfAsync(Debt debt, Guid userId, CancellationToken cancellationToken) =>
        debt.CounterpartyId is Guid counterpartyId &&
        await counterpartyRepository.FindOwnedByIdAsync(counterpartyId, userId, cancellationToken) is Counterparty person
            ? person.Name
            : string.Empty;

    /// <summary>
    /// Toplam geri ödemeyi belirler: gelen değer, orandan hesaplanan değer ya
    /// da ikisinin uyuşmadığını söyleyen hata.
    /// </summary>
    private static (decimal Value, ApplicationError? Error) ResolveTotalRepayment(CreateDebtCommand command)
    {
        try
        {
            if (command.TotalRepayment is decimal total && command.AnnualInterestRate is decimal rate)
            {
                var implied = AmortizationSchedule.TotalRepaymentFor(
                    command.Principal, rate, command.InstallmentCount);
                return Math.Abs(implied - total) <= RepaymentAgreementTolerance
                    ? (total, null)
                    : (0m, DebtErrors.RepaymentConflict);
            }

            if (command.TotalRepayment is decimal onlyTotal)
            {
                return (onlyTotal, null);
            }

            if (command.AnnualInterestRate is decimal onlyRate)
            {
                return (
                    AmortizationSchedule.TotalRepaymentFor(
                        command.Principal, onlyRate, command.InstallmentCount),
                    null);
            }

            return (0m, DebtErrors.RepaymentRequired);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            // Anapara, taksit sayısı veya oran hesabın sınırları dışında.
            // Aggregate kurulmadan önce yakalanıyor; aynı mesaj oradan da
            // gelirdi ama o zaman hesap hiç yapılamazdı.
            return (0m, DebtErrors.Validation(exception.Message));
        }
    }

    internal static DebtDto ToDto(Debt debt, string counterpartyName, DateOnly asOfDate)
    {
        var splits = debt.InstallmentSplits;
        var installments = debt.CurrentItems;
        var items = debt.Items.ToDictionary(item => item.Id);
        return new DebtDto(
            debt.Id, debt.CounterpartyId, counterpartyName, debt.Direction, debt.Scope,
            debt.Principal.Amount,
            debt.TotalRepayment.Amount, debt.RemainingAmount, debt.Principal.Currency,
            debt.AnnualInterestRate, debt.TotalInterest,
            debt.SourceType, debt.OpeningAccountId, debt.CategoryId,
            debt.StartDate, debt.FirstDueDate, debt.InstallmentCount,
            debt.Description, debt.IsClosed,
            [.. installments.Select((x, index) =>
            {
                var payment = debt.PaymentOf(x);
                return new DebtInstallmentDto(
                    x.Id, x.Sequence, x.Amount.Amount, x.Amount.Currency, x.DueDate,
                    debt.IsPaid(x) ? "paid" :
                        x.DueDate is not DateOnly due ? "open" :
                        due < asOfDate ? "overdue" :
                        due == asOfDate ? "due-today" : "upcoming",
                    payment?.AccountId, payment?.PaymentDate, payment?.PaidAtUtc,
                    splits[index].Principal, splits[index].Interest,
                    debt.PaidAmount(x), debt.OpenAmount(x));
            })],
            debt.PlanKind,
            debt.IsCarriedOver,
            debt.IsCancelled,
            [.. debt.Payments.OrderBy(closing => closing.PaymentDate).ThenBy(closing => closing.PaidAtUtc)
                .Select(closing => new DebtClosingDto(
                    closing.Id, closing.Kind, closing.Amount.Amount, closing.PaymentDate, closing.PaidAtUtc,
                    closing.AccountId, closing.CreditCardId, closing.PosSettlementId,
                    closing.Kind != DebtPaymentKind.Payment
                        ? 0m
                        : closing.Allocations.Sum(allocation =>
                            items[allocation.ScheduleItemId].InterestPortion is decimal interest
                                ? allocation.Amount == items[allocation.ScheduleItemId].Amount.Amount
                                    ? interest
                                    : interest * allocation.Amount / items[allocation.ScheduleItemId].Amount.Amount
                                : 0m),
                    closing.IsCancelled, closing.CancelledAtUtc))]);
    }
}

/// <summary>
/// Açılışı kaydedilmemiş bir borcun kaynağını tamamlar.
/// </summary>
/// <remarks>
/// Bu ayrımdan önce açılmış borçların açılışında ne olduğu kayıtlı değil ve
/// hiçbir sayıdan türetilemiyor — yalnız kullanıcı biliyor. Bu use case o
/// boşluğu kapatır; tamamlandığı anda borç açılış hareketini ya da giderini
/// üretmeye başlar.
/// </remarks>
public sealed class RecordDebtOpeningUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        RecordDebtOpeningCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);

        var debt = await repository.FindOwnedByIdAsync(command.DebtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(command.DebtId));

        Account? openingAccount = null;
        if (command.OpeningAccountId is Guid accountId)
        {
            openingAccount = await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken);
            if (openingAccount is null || !openingAccount.IsActive)
                return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
        }

        Category? category = null;
        if (command.CategoryId is Guid categoryId)
        {
            category = await categoryRepository.FindOwnedByIdAsync(categoryId, userId, cancellationToken);
            if (category is null || !category.IsActive ||
                category.Type != DebtErrors.RequiredCategoryType(command.SourceType))
                return ApplicationResult<DebtDto>.Failure(DebtErrors.CategoryUnavailable);
        }

        try
        {
            debt.RecordOpening(command.SourceType, openingAccount, category);
            await repository.SaveOpeningAsync(debt, cancellationToken);
            var name = await counterpartyRepository.FindOwnedByIdAsync(
                debt.CounterpartyId!.Value, userId, cancellationToken);
            return name is null
                ? ApplicationResult<DebtDto>.Failure(DebtErrors.CounterpartyUnavailable)
                : ApplicationResult<DebtDto>.Success(
                    CreateDebtUseCase.ToDto(debt, name.Name, asOfDate));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            // Zaten kaydedilmiş bir açılışı değiştirmek geçmişi geriye dönük
            // siler; bu bir doğrulama hatası değil, çakışmadır.
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
        }
    }
}

public sealed class ListDebtsUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    ICounterpartyRepository counterpartyRepository)
{
    public async Task<ApplicationResult<IReadOnlyList<DebtDto>>> ExecuteAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<IReadOnlyList<DebtDto>>.Failure(DebtErrors.AuthenticationRequired);
        var debts = await repository.ListAsync(userId, cancellationToken);

        // Adlar tek sorguda: sözleşme başına bir okuma, uzun bir borç
        // listesinde liste kadar sorgu demek olurdu.
        var names = await counterpartyRepository.ListNamesAsync(
            userId,
            [.. debts.Select(item => item.CounterpartyId!.Value).Distinct()],
            cancellationToken);

        return ApplicationResult<IReadOnlyList<DebtDto>>.Success(
            debts
                .Select(item => CreateDebtUseCase.ToDto(
                    item,
                    names.TryGetValue(item.CounterpartyId!.Value, out var name) ? name : string.Empty,
                    asOfDate))
                .ToArray());
    }
}

public sealed class PayDebtInstallmentUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICounterpartyRepository counterpartyRepository,
    TimeProvider timeProvider,
    ICategoryRepository categoryRepository)
{
    public async Task<ApplicationResult<DebtDto>> ExecuteAsync(
        PayDebtInstallmentCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AuthenticationRequired);
        var debt = await repository.FindOwnedByIdAsync(command.DebtId, userId, true, cancellationToken);
        if (debt is null) return ApplicationResult<DebtDto>.Failure(DebtErrors.NotFound(command.DebtId));

        // Vadesi gelmemiş taksit erken ödenebilir; ödeme günü ileride olamaz.
        if (command.PaymentDate > LocalDay.LatestAllowed(timeProvider.GetUtcNow()))
            return ApplicationResult<DebtDto>.Failure(DebtErrors.DateInFuture);
        var account = await accountRepository.FindOwnedByIdAsync(command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
        try
        {
            if (debt.GetInstallment(command.Sequence).InterestPortion > 0m)
            {
                var category = await categoryRepository.FindOrOpenAsync(userId,
                    debt.Direction == DebtDirection.Receivable
                        ? CategoryRole.InterestIncome : CategoryRole.InterestExpense,
                    cancellationToken);
                if (!category.IsActive)
                    return ApplicationResult<DebtDto>.Failure(DebtErrors.CategoryUnavailable);
            }
            var payment = debt.PayInstallment(
                command.Sequence, account, command.PaymentDate, timeProvider.GetUtcNow());
            await repository.SavePaymentAsync(payment, cancellationToken);
            var counterparty = await counterpartyRepository.FindOwnedByIdAsync(
                debt.CounterpartyId!.Value, userId, cancellationToken);
            return counterparty is null
                ? ApplicationResult<DebtDto>.Failure(DebtErrors.CounterpartyUnavailable)
                : ApplicationResult<DebtDto>.Success(
                    CreateDebtUseCase.ToDto(debt, counterparty.Name, asOfDate));
        }
        catch (DebtConcurrencyException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Conflict(exception.Message));
        }
    }
}
