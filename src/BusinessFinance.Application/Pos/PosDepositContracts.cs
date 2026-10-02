using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

/// <summary>
/// "Hesaba geçenleri işaretle": seçilen yoldaki tahsilatlar tek yatışla kapanır.
/// </summary>
/// <remarks>
/// <see cref="ClientRequestId"/> yatışın kimliğini belirler: aynı istek ikinci
/// kez gelirse ilk yatış döner, ikinci bir yatış ya da kesinti gideri yazılmaz.
///
/// <see cref="DeductionCategoryId"/> yalnız yatan tutar beklenenden azsa
/// kullanılır; boşsa seçilen tahsilatların POS'undaki komisyon kategorisi
/// uygulanır. <see cref="Scope"/> boşsa kesintinin kapsamı tahsilatlardan gelir.
/// </remarks>
public sealed record CreatePosDepositCommand(
    Guid ClientRequestId,
    IReadOnlyList<Guid> SettlementIds,
    decimal DepositedAmount,
    DateOnly DepositDate,
    Guid? DeductionCategoryId,
    TransactionScope? Scope);

/// <summary>
/// Yatış formunun önizlemesi için seçim. Tutar boşsa beklenen tutar yatmış
/// sayılır: formun ilk açılışındaki hâl budur.
/// </summary>
public sealed record PreviewPosDepositQuery(
    IReadOnlyList<Guid> SettlementIds,
    decimal? DepositedAmount);

/// <summary>
/// Yatışın önizlemesi. Hiçbir şey yazmaz; istemci beklenen toplamı ve
/// kesintiyi kendisi hesaplamaz.
/// </summary>
public sealed record PosDepositPreviewDto(
    Guid AccountId,
    string AccountName,
    int SettlementCount,
    decimal ExpectedAmount,
    decimal DepositedAmount,
    decimal DeductionAmount,
    // Yatan tutar beklenenden fazla: kayıt reddedilir, form bunu alanın
    // yanında söyler. Bu durumda kesinti sıfır döner.
    bool ExceedsExpected,
    Guid? DeductionCategoryId,
    string? DeductionCategoryName,
    CurrencyCode Currency,
    // Yatış bu günden önce olamaz: seçilen tahsilatların en geç olanı.
    DateOnly EarliestDepositDate);

public sealed record PosDepositDto(
    Guid Id,
    Guid AccountId,
    string AccountName,
    DateOnly DepositDate,
    decimal ExpectedAmount,
    decimal DepositedAmount,
    decimal DeductionAmount,
    Guid? DeductionTransactionId,
    Guid? DeductionCategoryId,
    string? DeductionCategoryName,
    CurrencyCode Currency,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    // Geri alınmış yatışta boştur: tahsilatlar yola dönmüştür.
    IReadOnlyList<PosSettlementDto> Settlements,
    // Yatıştan hemen sonra hesabın bakiyesi; yalnız yatış okunurken dolar.
    // Geri alınmış yatışta boştur.
    decimal? BalanceAfter = null,
    // Kapattığı tahsilatların brüt satış ve komisyon toplamı: yatan tutarın
    // nereden geldiğini açıklar (brüt − komisyon = beklenen). Komisyon satış
    // günü gider yazılmıştır; burada yeniden yazılmaz, yalnız gösterilir.
    // Geri alınmış yatışta tahsilat kalmadığı için boştur.
    decimal? GrossAmount = null,
    decimal? CommissionAmount = null);

public interface IPosDepositRepository
{
    /// <summary>
    /// Sahip olunan tahsilatları izlenen hâlde döner; bulunamayan kimlik
    /// sonuçta yer almaz.
    /// </summary>
    Task<IReadOnlyList<PosSettlement>> FindOwnedSettlementsAsync(
        IReadOnlyCollection<Guid> settlementIds,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>Yatışın kapattığı tahsilatlar, izlenen hâlde.</summary>
    Task<IReadOnlyList<PosSettlement>> FindSettlementsOfDepositAsync(
        Guid depositId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<PosDeposit?> FindOwnedByIdAsync(
        Guid depositId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>Yatışın okuma hâli: adlar ve kapattığı tahsilatlarla.</summary>
    Task<PosDepositDto?> GetAsync(Guid depositId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Seçilen tahsilatlar için kesinti kategorisi adayları: tahsilatın
    /// POS'undaki komisyon kategorisi, POS'u yoksa tahsilatın kendi komisyon
    /// kategorisi.
    /// </summary>
    Task<IReadOnlyList<Guid>> ListDeductionCategoryCandidatesAsync(
        IReadOnlyCollection<Guid> settlementIds,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Yatışı, varsa kesinti giderini ve tahsilatların bağını tek
    /// <c>SaveChanges</c> ile yazar. Eşzamanlı bir yazma kazandıysa
    /// <c>false</c> döner ve hiçbir şey yazılmaz.
    /// </summary>
    Task<bool> TryAddAsync(
        PosDeposit deposit,
        BudgetTransaction? deductionTransaction,
        CancellationToken cancellationToken);

    Task<bool> TrySaveAsync(CancellationToken cancellationToken);
}

public static class PosDepositErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError SettlementsRequired = new(
        "pos_deposits.settlements_required",
        "Select at least one distinct settlement.",
        ApplicationErrorType.Validation);

    public static ApplicationError TooManySettlements(int maximum) => new(
        "pos_deposits.too_many_settlements",
        $"A deposit closes at most {maximum} settlements.",
        ApplicationErrorType.Validation);

    /// <summary>Başka kullanıcının ya da var olmayan tahsilatın cevabı aynıdır.</summary>
    public static readonly ApplicationError SettlementNotFound = new(
        "pos_deposits.settlement_not_found",
        "One of the selected settlements was not found.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError SettlementNotInTransit = new(
        "pos_deposits.settlement_not_in_transit",
        "Only settlements that are still in transit can be deposited.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError MixedAccounts = new(
        "pos_deposits.mixed_accounts",
        "A deposit closes settlements of a single account.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError AccountUnavailable = new(
        "pos_deposits.account_unavailable",
        "The account the settlements arrive in must be active.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Fazla yatan gelir değildir, fazla yazılmış komisyondur; kullanıcı
    /// tahsilatı doğru komisyonla yeniden girer.
    /// </summary>
    public static readonly ApplicationError AmountExceedsExpected = new(
        "pos_deposits.amount_exceeds_expected",
        "The deposited amount cannot exceed the expected net amount.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidAmount = new(
        "pos_deposits.invalid_amount",
        "The deposited amount must be greater than zero.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidDepositDate = new(
        "pos_deposits.invalid_deposit_date",
        "The deposit date cannot be in the future or before a settlement it closes.",
        ApplicationErrorType.Validation);

    /// <summary>Kesinti var ama yazılacağı kategori bilinmiyor; sunucu kategori uydurmaz.</summary>
    public static readonly ApplicationError DeductionCategoryRequired = new(
        "pos_deposits.deduction_category_required",
        "A deduction needs an expense category.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError DeductionCategoryUnavailable = new(
        "pos_deposits.deduction_category_unavailable",
        "An active owned expense category is required for the deduction.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ScopeUnresolved = new(
        "pos_deposits.scope_unresolved",
        "The deduction scope could not be resolved from the request, the settlements, the account or the category.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ConcurrentChange = new(
        "pos_deposits.concurrent_change",
        "The settlements changed while the deposit was being written; read them again.",
        ApplicationErrorType.Conflict);

    public static ApplicationError NotFound(Guid id) => new(
        "pos_deposits.not_found",
        $"Pos deposit '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Validation(string message) => new(
        "pos_deposits.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError Conflict(string message) => new(
        "pos_deposits.conflict",
        message,
        ApplicationErrorType.Conflict);
}
