using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
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
}

public sealed class CreateDebtUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository)
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

        if (TransactionScopeResolution.Resolve(
                command.Scope,
                openingAccount?.DefaultScope,
                category?.DefaultScope) is not TransactionScope scope)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.ScopeUnresolved);
        }

        // Karşı taraf en sonda kuruluyor: bu noktadan sonra yalnız aggregate
        // doğrulaması kalıyor ve kaydetme borcunkiyle aynı sınırda oluyor.
        Counterparty counterparty;
        try
        {
            counterparty = await counterpartyRepository.FindOrCreateByNameAsync(
                userId, command.CounterpartyName, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<DebtDto>.Failure(DebtErrors.Validation(exception.Message));
        }

        try
        {
            var debt = new DebtAgreement(
                Guid.NewGuid(), userId, counterparty, command.Direction,
                scope,
                new Money(command.Principal, command.Currency),
                new Money(totalRepayment.Value, command.Currency),
                command.SourceType, openingAccount, category,
                command.StartDate, command.FirstDueDate,
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

    internal static DebtDto ToDto(DebtAgreement debt, string counterpartyName, DateOnly asOfDate)
    {
        var splits = debt.InstallmentSplits;
        var installments = debt.Installments.OrderBy(x => x.Sequence).ToArray();
        return new DebtDto(
            debt.Id, debt.CounterpartyId, counterpartyName, debt.Direction, debt.Scope,
            debt.Principal.Amount,
            debt.TotalRepayment.Amount, debt.RemainingAmount, debt.Principal.Currency,
            debt.AnnualInterestRate, debt.TotalInterest,
            debt.SourceType, debt.OpeningAccountId, debt.CategoryId,
            debt.StartDate, debt.FirstDueDate, debt.InstallmentCount,
            debt.Description, debt.IsClosed,
            [.. installments.Select((x, index) => new DebtInstallmentDto(
                x.Id, x.Sequence, x.Amount.Amount, x.Amount.Currency, x.DueDate,
                x.IsPaid ? "paid" : x.DueDate < asOfDate ? "overdue" :
                    x.DueDate == asOfDate ? "due-today" : "upcoming",
                x.PaymentAccountId, x.PaymentDate, x.PaidAtUtc,
                splits[index].Principal, splits[index].Interest))]);
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
                debt.CounterpartyId, userId, cancellationToken);
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
            [.. debts.Select(item => item.CounterpartyId).Distinct()],
            cancellationToken);

        return ApplicationResult<IReadOnlyList<DebtDto>>.Success(
            debts
                .Select(item => CreateDebtUseCase.ToDto(
                    item,
                    names.TryGetValue(item.CounterpartyId, out var name) ? name : string.Empty,
                    asOfDate))
                .ToArray());
    }
}

public sealed class PayDebtInstallmentUseCase(
    ICurrentUser currentUser,
    IDebtRepository repository,
    IAccountRepository accountRepository,
    ICounterpartyRepository counterpartyRepository,
    TimeProvider timeProvider)
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
        var account = await accountRepository.FindOwnedByIdAsync(command.AccountId, userId, cancellationToken);
        if (account is null || !account.IsActive)
            return ApplicationResult<DebtDto>.Failure(DebtErrors.AccountUnavailable);
        try
        {
            debt.GetInstallment(command.Sequence).MarkPaid(account, command.PaymentDate, timeProvider.GetUtcNow());
            await repository.SavePaymentAsync(debt.GetInstallment(command.Sequence), cancellationToken);
            var counterparty = await counterpartyRepository.FindOwnedByIdAsync(
                debt.CounterpartyId, userId, cancellationToken);
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
