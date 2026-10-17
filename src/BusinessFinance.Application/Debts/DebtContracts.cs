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
    string? CounterpartyName,
    DebtDirection Direction,

    // Kullanıcının açık seçimi. Boşsa açılış hesabının, yoksa kategorinin
    // varsayılanı kullanılır; üçü de boşsa istek reddedilir.
    TransactionScope? Scope,
    decimal Principal,
    decimal? TotalRepayment,
    decimal? AnnualInterestRate,
    CurrencyCode Currency,
    DebtSourceType SourceType,
    Guid? OpeningAccountId,
    Guid? CategoryId,
    DateOnly StartDate,
    DateOnly? FirstDueDate,
    int InstallmentCount,
    string? Description,

    // Dalga 2 (ADR 0021): plan bir ayardır; kişi kimlikle de seçilebilir ve
    // isteğe bağlıdır; uygulamadan önceki borç açılış yazmaz; peşin ödenen
    // tutar borcun ilk ödemesidir. Varsayılanlar eski taksitli borç isteğidir.
    DebtPlanKind PlanKind = DebtPlanKind.Installments,
    Guid? CounterpartyId = null,
    bool IsCarriedOver = false,
    DebtUpfrontPayment? UpfrontPayment = null,
    Guid? ClientRequestId = null);

/// <summary>Borç açılırken peşin ödenen ya da tahsil edilen tutar.</summary>
public sealed record DebtUpfrontPayment(decimal Amount, Guid? AccountId, Guid? CreditCardId);

/// <summary>
/// Borca ödeme ya da tahsilat. Yer tam olarak biridir: hesap, kredi kartı
/// (yalnız borçta) ya da POS (yalnız alacakta, <see cref="Card"/>).
/// </summary>
public sealed record RecordDebtPaymentCommand(
    Guid DebtId,
    Guid? ClientRequestId,
    decimal Amount,
    DateOnly PaymentDate,
    Guid? AccountId,
    Guid? CreditCardId,
    Pos.CardCollectionInput? Card);

/// <summary>İndirim: bedel düşer, para hareket etmez.</summary>
public sealed record ApplyDebtDiscountCommand(
    Guid DebtId,
    Guid? ClientRequestId,
    decimal Amount,
    DateOnly DiscountDate);

/// <summary>Planı değiştirir; o anki kalan yeni takvime bölünür.</summary>
public sealed record ChangeDebtPlanCommand(
    Guid DebtId,
    DebtPlanKind PlanKind,
    DateOnly? FirstDueDate,
    int InstallmentCount);

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
    DateOnly? DueDate,
    string Status,
    Guid? PaymentAccountId,
    DateOnly? PaymentDate,
    DateTimeOffset? PaidAtUtc,
    decimal PrincipalPortion,
    decimal InterestPortion,
    decimal PaidAmount = 0m,
    decimal OpenAmount = 0m);

/// <summary>Borcun bir kapanışı: ödeme ya da indirim; iptal edilmişse geri alınmıştır.</summary>
public sealed record DebtClosingDto(
    Guid Id,
    DebtPaymentKind Kind,
    decimal Amount,
    DateOnly Date,
    DateTimeOffset RecordedAtUtc,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid? PosSettlementId,
    decimal InterestAmount,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

public sealed record DebtDto(
    Guid Id,
    Guid? CounterpartyId,
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
    DateOnly? FirstDueDate,
    int InstallmentCount,
    string? Description,
    bool IsClosed,
    IReadOnlyList<DebtInstallmentDto> Installments,
    DebtPlanKind PlanKind = DebtPlanKind.Installments,
    bool IsCarriedOver = false,
    bool IsCancelled = false,
    IReadOnlyList<DebtClosingDto>? Closings = null);

public interface IDebtRepository
{
    Task AddAsync(Debt debt, CancellationToken cancellationToken);

    /// <summary>Taksitli borçlar; tek ödemeli borçlar yükümlülük uçlarındadır (dalga 1).</summary>
    Task<IReadOnlyList<Debt>> ListAsync(Guid userId, CancellationToken cancellationToken);

    Task<Debt?> FindOwnedByIdAsync(Guid id, Guid userId, bool track, CancellationToken cancellationToken);
    Task SavePaymentAsync(DebtPayment payment, CancellationToken cancellationToken);
    Task SaveOpeningAsync(Debt debt, CancellationToken cancellationToken);

    /// <summary>Her planda borç (tek ödeme, taksitli, planı olmayan).</summary>
    Task<Debt?> FindAnyOwnedAsync(Guid id, Guid userId, bool track, CancellationToken cancellationToken);

    /// <summary>Kartla tahsilin yoldaki POS kaydı, izlenen hâlde.</summary>
    Task<PosSettlement?> FindCardSettlementAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Yeni borcu, peşin ödemesini ve varsa POS kaydını tek <c>SaveChanges</c> ile yazar.
    /// </summary>
    Task AddWithClosingAsync(
        Debt debt,
        DebtPayment? closing,
        PosSettlement? cardSettlement,
        CancellationToken cancellationToken);

    /// <summary>
    /// Borcun değişikliğini (yeni kapanış, geri alma, plan, iptal) ve varsa
    /// POS kaydını tek <c>SaveChanges</c> ile yazar. Borç bu sırada başka bir
    /// istekle değiştiyse <see cref="DebtConcurrencyException"/>.
    /// </summary>
    Task SaveChangesAsync(
        DebtPayment? newClosing,
        PosSettlement? newCardSettlement,
        CancellationToken cancellationToken);
}

public sealed class DebtConcurrencyException : Exception
{
    public DebtConcurrencyException() : base("Debt installment was changed by another request.") { }
}
