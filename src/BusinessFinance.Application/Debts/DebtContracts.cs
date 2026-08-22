using BusinessFinance.Domain;

namespace BusinessFinance.Application.Debts;

/// <summary>
/// Borç açma isteği.
/// </summary>
/// <remarks>
/// <see cref="TotalRepayment"/> ile <see cref="AnnualInterestRate"/>'ten
/// <b>en az biri</b> gelmelidir; hangisi gelirse diğeri hesaplanır. İkisi
/// birden gelip çelişirse istek reddedilir — sessizce birini seçmek
/// kullanıcının yazdığından başka bir borç kaydetmek olurdu.
/// </remarks>
public sealed record CreateDebtCommand(
    string CounterpartyName,
    DebtDirection Direction,
    TransactionScope Scope,
    decimal Principal,
    decimal? TotalRepayment,
    decimal? AnnualInterestRate,
    CurrencyCode Currency,
    DebtSourceType SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    DateOnly StartDate,
    DateOnly FirstDueDate,
    int InstallmentCount,
    string? Description);

/// <summary>
/// Açılışı kaydedilmemiş bir borcun kaynağını sonradan tamamlar.
/// </summary>
public sealed record RecordDebtOpeningCommand(
    Guid DebtId,
    DebtSourceType SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId);

public sealed record PayDebtInstallmentCommand(
    Guid DebtId,
    int Sequence,
    Guid AccountId,
    DateOnly PaymentDate);

public sealed record DebtInstallmentDto(
    Guid Id,
    int Sequence,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly DueDate,
    string Status,
    Guid? PaymentAccountId,
    DateOnly? PaymentDate,
    DateTimeOffset? PaidAtUtc,
    decimal PrincipalPortion,
    decimal InterestPortion);

public sealed record DebtDto(
    Guid Id,
    string CounterpartyName,
    DebtDirection Direction,
    TransactionScope Scope,
    decimal Principal,
    decimal TotalRepayment,
    decimal RemainingAmount,
    CurrencyCode Currency,
    decimal AnnualInterestRate,
    decimal TotalInterest,
    DebtSourceType SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    DateOnly StartDate,
    DateOnly FirstDueDate,
    int InstallmentCount,
    string? Description,
    bool IsClosed,
    IReadOnlyList<DebtInstallmentDto> Installments);

public interface IDebtRepository
{
    Task AddAsync(DebtAgreement debt, CancellationToken cancellationToken);
    Task<IReadOnlyList<DebtAgreement>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<DebtAgreement?> FindOwnedByIdAsync(Guid id, Guid userId, bool track, CancellationToken cancellationToken);
    Task SavePaymentAsync(DebtInstallment installment, CancellationToken cancellationToken);
    Task SaveOpeningAsync(DebtAgreement debt, CancellationToken cancellationToken);
}

public sealed class DebtConcurrencyException : Exception
{
    public DebtConcurrencyException() : base("Debt installment was changed by another request.") { }
}
