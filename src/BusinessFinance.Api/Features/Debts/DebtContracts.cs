namespace BusinessFinance.Api.Features.Debts;

/// <summary>
/// Borç açma isteği.
/// </summary>
/// <remarks>
/// <see cref="TotalRepayment"/> ile <see cref="AnnualInterestRate"/>'ten en az
/// biri dolu gelmelidir; hangisi gelirse diğeri hesaplanır. İkisi birden gelip
/// çelişirse istek <c>debt.repayment_conflict</c> ile reddedilir.
///
/// <see cref="SourceType"/> borcu neyin doğurduğunu söyler: <c>cash</c> ise
/// <see cref="OpeningAccountId"/>, <c>expense</c> ise <see cref="CategoryId"/>
/// zorunludur. Alacak yalnız <c>cash</c> olabilir.
/// </remarks>
public sealed record CreateDebtRequest(
    string CounterpartyName,
    string Direction,
    string Scope,
    string Principal,
    string? TotalRepayment,
    string? AnnualInterestRate,
    string Currency,
    string SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    string StartDate,
    string FirstDueDate,
    int InstallmentCount,
    string? Description,
    string AsOfDate);

/// <summary>Açılışı kaydedilmemiş bir borcun kaynağını sonradan tamamlar.</summary>
public sealed record RecordDebtOpeningRequest(
    string SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    string AsOfDate);

public sealed record PayDebtInstallmentRequest(Guid AccountId, string PaymentDate, string AsOfDate);

public sealed record DebtInstallmentResponse(Guid Id, int Sequence, string Amount, string Currency,
    string DueDate, string Status, Guid? PaymentAccountId, string? PaymentDate, DateTimeOffset? PaidAtUtc,
    string PrincipalPortion, string InterestPortion);

public sealed record DebtResponse(Guid Id, string CounterpartyName, string Direction, string Scope, string Principal,
    string TotalRepayment, string RemainingAmount, string Currency, string AnnualInterestRate,
    string TotalInterest, string SourceType, Guid? OpeningAccountId, Guid? CategoryId,
    string StartDate, string FirstDueDate, int InstallmentCount, string? Description,
    bool IsClosed, IReadOnlyList<DebtInstallmentResponse> Installments);

public sealed record DebtListResponse(IReadOnlyList<DebtResponse> Items);
