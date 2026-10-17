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
    string? CounterpartyName,
    string Direction,
    // İsteğe bağlı: boş bırakılırsa sunucu kapsamı türetir, türetemezse
    // isteği reddeder ve bir değer uydurmaz.
    string? Scope,
    string Principal,
    string? TotalRepayment,
    string? AnnualInterestRate,
    string Currency,
    string SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    string StartDate,
    string? FirstDueDate,
    int InstallmentCount,
    string? Description,
    string AsOfDate,

    // Dalga 2 (ADR 0021). Hepsi isteğe bağlı; boş bırakılan istek bugünkü
    // taksitli borçtur. `planKind`: `installments` · `single` · `none`
    // (`none`da `firstDueDate` isteğe bağlıdır). Kişi adla ya da kimlikle
    // seçilir ya da hiç seçilmez (kişisiz borç açıklama ister).
    string? PlanKind = null,
    Guid? CounterpartyId = null,
    bool IsCarriedOver = false,
    DebtUpfrontPaymentRequest? UpfrontPayment = null,
    Guid? ClientRequestId = null);

/// <summary>Açılışta peşin ödenen tutar; yeri hesap ya da kredi kartıdır.</summary>
public sealed record DebtUpfrontPaymentRequest(string Amount, Guid? AccountId = null, Guid? CreditCardId = null);

/// <summary>
/// Borca ödeme ya da tahsilat. Yer tam olarak biridir: <c>accountId</c>,
/// <c>creditCardId</c> (yalnız borçta) ya da <c>card</c> (POS; yalnız alacakta).
/// Aynı <c>clientRequestId</c> aynı ödemeyi döner.
/// </summary>
public sealed record RecordDebtPaymentRequest(
    string Amount,
    string PaymentDate,
    string AsOfDate,
    Guid? AccountId = null,
    Guid? CreditCardId = null,
    Pos.CardCollectionRequest? Card = null,
    Guid? ClientRequestId = null);

/// <summary>İndirim: bedel düşer, para hareket etmez.</summary>
public sealed record ApplyDebtDiscountRequest(
    string Amount,
    string DiscountDate,
    string AsOfDate,
    Guid? ClientRequestId = null);

/// <summary>Plan değişikliği: kalan yeni takvime bölünür.</summary>
public sealed record ChangeDebtPlanRequest(
    string PlanKind,
    string? FirstDueDate,
    int InstallmentCount,
    string AsOfDate);

/// <summary>Açılışı kaydedilmemiş bir borcun kaynağını sonradan tamamlar.</summary>
public sealed record RecordDebtOpeningRequest(
    string SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    string AsOfDate);

public sealed record PayDebtInstallmentRequest(Guid AccountId, string PaymentDate, string AsOfDate);

public sealed record DebtInstallmentResponse(Guid Id, int Sequence, string Amount, string Currency,
    string? DueDate, string Status, Guid? PaymentAccountId, string? PaymentDate, DateTimeOffset? PaidAtUtc,
    string PrincipalPortion, string InterestPortion, string PaidAmount, string OpenAmount);

/// <summary>
/// Borcun bir kapanışı. <c>kind</c>: <c>payment</c> (hesap, kart ya da POS) ya
/// da <c>discount</c> (para yok). İptal edilmişse geri alınmıştır.
/// </summary>
public sealed record DebtClosingResponse(Guid Id, string Kind, string Amount, string Date,
    DateTimeOffset RecordedAtUtc, Guid? AccountId, Guid? CreditCardId, Guid? PosSettlementId,
    string InterestAmount, bool IsCancelled, DateTimeOffset? CancelledAtUtc);

public sealed record DebtResponse(Guid Id, Guid? CounterpartyId, string CounterpartyName, string Direction, string Scope, string Principal,
    string TotalRepayment, string RemainingAmount, string Currency, string AnnualInterestRate,
    string TotalInterest, string SourceType, Guid? OpeningAccountId, Guid? CategoryId,
    string StartDate, string? FirstDueDate, int InstallmentCount, string? Description,
    bool IsClosed, IReadOnlyList<DebtInstallmentResponse> Installments,
    string PlanKind, bool IsCarriedOver, bool IsCancelled, IReadOnlyList<DebtClosingResponse> Closings);

public sealed record DebtListResponse(IReadOnlyList<DebtResponse> Items);

/// <summary>
/// "Borç ve alacaklar" listesinin satırı. <c>kind</c>: <c>debt</c> (borcun
/// kimliği) ya da <c>ledger</c> (cari hesap; kimlik kişinindir). <c>section</c>:
/// <c>overdue</c> · <c>upcoming</c> · <c>undated</c>.
/// </summary>
public sealed record DebtBookItemResponse(
    string Kind, Guid Id, string Direction, string Section, string Title,
    Guid? CounterpartyId, string? CounterpartyName, string? PlanKind,
    string Remaining, string Overdue, string? NextDueDate, string NextDueAmount,
    bool HasClosings, bool IsCarriedOver, string? StartDate);

public sealed record DebtBookPersonResponse(
    Guid CounterpartyId, string Name, string OwedToYou, string OwedByYou,
    string LedgerOwedToYou, string LedgerOwedByYou, int OpenDebtCount);

public sealed record DebtBookResponse(
    string PayableTotal, string ReceivableTotal,
    IReadOnlyList<DebtBookItemResponse> Items,
    IReadOnlyList<DebtBookPersonResponse> People,
    int ClosedPayableCount, int ClosedReceivableCount);

public sealed record DebtBookClosedItemResponse(
    Guid DebtId, string Direction, string Title, Guid? CounterpartyId, string? CounterpartyName,
    string PlanKind, string Amount, string ClosedOn);

public sealed record DebtBookClosedResponse(IReadOnlyList<DebtBookClosedItemResponse> Items);
