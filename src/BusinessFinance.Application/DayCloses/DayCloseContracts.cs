using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.DayCloses;

/// <summary>Gün sonu panelinde bir POS satırına yazılan kartlı satış tutarı.</summary>
public sealed record DayClosePosAmount(Guid PosDefinitionId, decimal Amount);

/// <summary>Kaydın gün sonu tutarının hangi tarafından düşüleceği.</summary>
public enum DayCloseSide
{
    Cash = 1,
    Card = 2
}

/// <summary>
/// Kullanıcının bir "zaten girilmiş" kayıt için varsayılandan farklı seçimi.
/// </summary>
/// <remarks>
/// İstek yalnız <b>değişenleri</b> taşır; adı geçmeyen kayıt varsayılanıyla
/// işlenir. İşaretlilerin tam listesi istenseydi, panel açıkken girilen ve
/// istemcinin hiç görmediği bir satış "işaretsiz" sayılır ve gün sonuyla
/// birlikte iki kez gelir yazılırdı (ADR 0019 İ2).
/// </remarks>
public sealed record DayCloseRecordOverride(DayCloseRecordKind Kind, Guid Id, bool Included);

/// <summary>
/// Gün sonunun girdisi; önizleme ve kayıt aynı girdiyi alır ve aynı hesaptan
/// geçer.
/// </summary>
/// <remarks>
/// Nakit, kart (POS satırları) ve toplamdan <b>ikisi yeter</b>; üçüncüsü
/// hesaplanır. Yalnız nakit ya da yalnız kart verilirse öbür tarafa
/// dokunulmaz. Üçü de verilip tutmuyorsa kayıt engellenmez: toplam kayıt
/// üretmez, yalnız farkı açıklar (ADR 0019 T3).
/// </remarks>
public sealed record DayCloseInput(
    DateOnly Date,
    decimal? CashAmount,
    IReadOnlyList<DayClosePosAmount> PosAmounts,
    decimal? TotalAmount,
    Guid? CashAccountId,
    Guid? CashCategoryId,
    IReadOnlyList<DayCloseRecordOverride> RecordOverrides,
    bool IsAdditional,
    DateOnly? RangeStart,
    int? ZNumber);

public sealed record CreateDayCloseCommand(Guid ClientRequestId, DayCloseInput Input);

/// <summary>O gün zaten girilmiş ve gün sonu tutarının içinde olabilecek bir kayıt.</summary>
public sealed record DayCloseExistingRecordDto(
    DayCloseRecordKind Kind,
    Guid Id,
    DayCloseSide Side,
    DateOnly Date,
    decimal Amount,
    // Kullanıcının yazdığı ad; yoksa kategori ya da kişi adı. Boşsa türün
    // adını istemci yazar.
    string Title,
    // Kart tarafında tahsilatın POS'u; POS seçilmeden girilende boş.
    Guid? PosDefinitionId,
    // Paranın girdiği (ya da geçeceği) hesabın adı.
    string AccountName,
    bool IncludedByDefault,
    bool Included,
    // Kart tarafında bir alacağın kartla tahsili (satış değil); başlık kişinin
    // adıdır. Satış gibi varsayılan olarak düşülür (KP7).
    bool IsCardCollection = false);

/// <summary>Panelin nakit satırı: girilen, düşülen ve yazılacak tutar.</summary>
public sealed record DayCloseCashLineDto(
    // Nakit tutarı verildi ya da toplamdan hesaplandı; değilse nakit tarafına
    // dokunulmaz ve aşağıdaki tutarlar sıfırdır.
    bool Stated,
    decimal EnteredAmount,
    bool IsComputed,
    decimal DeductedAmount,
    decimal AmountToWrite,
    Guid? AccountId,
    string? AccountName,
    Guid? CategoryId,
    string? CategoryName);

/// <summary>Panelin bir POS satırı.</summary>
public sealed record DayClosePosLineDto(
    Guid PosDefinitionId,
    string Name,
    bool IsDefault,
    string AccountName,
    bool Stated,
    decimal EnteredAmount,
    bool IsComputed,
    decimal DeductedAmount,
    decimal AmountToWrite,
    decimal CommissionAmount,
    decimal NetAmount,
    DateOnly ExpectedTransferDate);

/// <summary>Günü kapatan bir gün sonunun kısa hâli.</summary>
public sealed record DayCloseSummaryDto(
    Guid Id,
    DateOnly ClosedOn,
    DateOnly? RangeStart,
    int? ZNumber,
    bool IsAdditional);

/// <summary>
/// Gün sonu panelinin önizlemesi. Hiçbir şey yazmaz; istemci hiçbir tutarı
/// kendisi hesaplamaz.
/// </summary>
public sealed record DayClosePreviewDto(
    DateOnly Date,
    DateOnly? RangeStart,
    CurrencyCode Currency,
    // Bu günü (ya da aralığı) zaten kapatmış gün sonları.
    IReadOnlyList<DayCloseSummaryDto> ClosedBy,
    DayCloseCashLineDto Cash,
    IReadOnlyList<DayClosePosLineDto> PosLines,
    decimal? TotalEntered,
    decimal TotalComputed,
    // Girilen toplam ile nakit + kart arasındaki fark; toplam verilmediyse
    // ya da tutuyorsa boş.
    decimal? TotalDifference,
    IReadOnlyList<DayCloseExistingRecordDto> ExistingRecords,
    // Bu girdiyle kayıt reddedilecekse nedeni; boşsa kaydedilebilir.
    ApplicationError? Blocker);

public sealed record DayCloseIncomeDto(
    Guid TransactionId,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    decimal Amount,
    DateOnly Date,
    TransactionScope Scope,
    bool IsCancelled);

/// <summary>
/// Gün sonu ve ürettiği kayıtlar. Tutarlar gün sonunda saklanmaz; kayıtlardan
/// toplanır.
/// </summary>
public sealed record DayCloseDto(
    Guid Id,
    DateOnly ClosedOn,
    DateOnly? RangeStart,
    int? ZNumber,
    bool IsAdditional,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<DayCloseIncomeDto> Incomes,
    IReadOnlyList<PosSettlementDto> Settlements,
    decimal CashAmount,
    decimal CardGrossAmount,
    decimal CommissionAmount,
    CurrencyCode Currency,
    // Gün sonunun saydığı (tek tek girilmiş ve tutardan düşülmüş) kayıtlar.
    // Geri alınmış gün sonunda boştur: bağ kalkmış, kayıtlar serbesttir.
    IReadOnlyList<DayCloseExistingRecordDto> CountedRecords,
    decimal CountedCashAmount,
    decimal CountedCardAmount);

/// <summary>
/// Bir günün bütünü: o günü kapatan gün sonları (ana ve ekler), her birinin
/// yazdığı ve saydığı kayıtlar, gün sonlarının dışında kalan tek tek
/// girilmiş kayıtlar ve günün toplamı.
/// </summary>
/// <remarks>
/// Toplam saklanmaz; geri alınmamış gün sonlarının yazdığı ve saydığı
/// kayıtlardan toplanır — kullanıcının o gün için yazdığı tutardır.
/// </remarks>
public sealed record DayCloseDayDto(
    DateOnly Date,
    IReadOnlyList<DayCloseDto> Closes,
    IReadOnlyList<DayCloseExistingRecordDto> OutsideRecords,
    decimal CashTotal,
    decimal CardTotal,
    CurrencyCode Currency);

/// <summary>Bir kaydın bir gün sonunda sayılıp sayılmadığını okur.</summary>
/// <remarks>
/// Sayılan kayıt tek başına iptal edilemez: iptal edilseydi gün sonunun
/// yazdığı tutar, artık var olmayan bir satışı düşmüş olurdu ve günün geliri
/// sessizce eksilirdi. İptal uçları aynı kuralı buradan okur.
/// </remarks>
public interface IDayCloseCountReader
{
    Task<bool> IsCountedAsync(
        Guid userId,
        DayCloseRecordKind kind,
        Guid recordId,
        CancellationToken cancellationToken);
}

/// <summary>Kasanın ve satış kategorisinin seçili gelecek değerleri.</summary>
public sealed record DayCloseDefaults(
    // Son gün sonunun nakit gelirini yazdığı hesap ve kategori.
    Guid? LastCashAccountId,
    Guid? LastCashCategoryId,
    // Şahsi etiketli olmayan aktif nakit hesaplar.
    IReadOnlyList<Guid> CashAccountIds);

public interface IDayCloseRepository
{
    Task<DayCloseDto?> GetAsync(Guid dayCloseId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Verilen aralıktaki günlerden birini kapatan, geri alınmamış gün sonları;
    /// kayıtlarıyla, günü yeni olan önce.
    /// </summary>
    Task<IReadOnlyList<DayCloseDto>> ListAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);

    /// <summary>Aralığı kapatan, geri alınmamış gün sonlarının kısa hâli.</summary>
    Task<IReadOnlyList<DayCloseSummaryDto>> ListCoveringAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);

    Task<bool> ZNumberExistsAsync(Guid userId, int zNumber, CancellationToken cancellationToken);

    /// <summary>
    /// Aralıkta tek tek girilmiş, gün sonu tutarının içinde olabilecek
    /// kayıtlar: nakit hesaba işletme gelirleri, POS tahsilatları ve nakit
    /// hesaba cari ya da alacak tahsilatları. Bir gün sonunun ürettiği ya da
    /// zaten saydığı kayıtlar bu listede yer almaz.
    /// </summary>
    Task<IReadOnlyList<DayCloseExistingRecordDto>> ListExistingRecordsAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);

    /// <summary>Aktif POS'lar; önce ana POS, sonra ada göre.</summary>
    Task<IReadOnlyList<PosDefinition>> ListActivePosDefinitionsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<DayCloseDefaults> GetDefaultsAsync(Guid userId, CancellationToken cancellationToken);

    Task<DayClose?> FindOwnedByIdAsync(
        Guid dayCloseId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>Gün sonunun ürettiği kayıtlar, izlenen hâlde.</summary>
    Task<(IReadOnlyList<BudgetTransaction> Incomes, IReadOnlyList<PosSettlement> Settlements)>
        FindRecordsAsync(Guid dayCloseId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Gün sonunu ve ürettiği kayıtları tek <c>SaveChanges</c> ile yazar.
    /// Eşzamanlı bir yazma kazandıysa <c>false</c> döner ve hiçbir şey
    /// yazılmaz.
    /// </summary>
    Task<bool> TryAddAsync(
        DayClose dayClose,
        IReadOnlyCollection<BudgetTransaction> incomes,
        IReadOnlyCollection<PosSettlement> settlements,
        IReadOnlyCollection<DayCloseCountedRecord> counted,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gün sonunun saydığı kayıtların bağını kaldırır; değişiklik
    /// <see cref="TrySaveAsync"/> ile, geri almayla aynı anda yazılır.
    /// </summary>
    Task ReleaseCountedAsync(Guid dayCloseId, Guid userId, CancellationToken cancellationToken);

    Task<bool> TrySaveAsync(CancellationToken cancellationToken);
}

public static class DayCloseErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError InvalidDate = new(
        "day_closes.invalid_date",
        "The closed day is required, cannot be in the future and a range must start before it.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidZNumber = new(
        "day_closes.invalid_z_number",
        "A Z number is a positive number.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidAmount = new(
        "day_closes.invalid_amount",
        "Amounts cannot be negative and each pos appears at most once.",
        ApplicationErrorType.Validation);

    /// <summary>Nakit, kart ve toplamdan en az biri; yalnız toplam yetmez.</summary>
    public static readonly ApplicationError AmountsRequired = new(
        "day_closes.amounts_required",
        "Enter the cash amount, the card amount, or two of cash, card and total.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError TotalBelowParts = new(
        "day_closes.total_below_parts",
        "The total cannot be less than the amount it is split from.",
        ApplicationErrorType.Validation);

    /// <summary>Kart tutarı toplamdan hesaplandı ama yazılacağı bir POS yok.</summary>
    public static readonly ApplicationError PosRequired = new(
        "day_closes.pos_required",
        "A card amount needs a pos to be written to.",
        ApplicationErrorType.Validation);

    /// <summary>Başka kullanıcının, pasif ya da var olmayan POS'un cevabı aynıdır.</summary>
    public static readonly ApplicationError PosUnavailable = new(
        "day_closes.pos_unavailable",
        "An active owned pos is required for each card amount.",
        ApplicationErrorType.NotFound);

    /// <summary>POS'un hesabı ya da kategorisi artık kullanılamıyor.</summary>
    public static readonly ApplicationError PosUnusable = new(
        "day_closes.pos_unusable",
        "The account or a category of the pos is no longer usable; edit the pos.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// İşaretli kayıtlar gün sonu tutarını aşıyor: ya tutar eksik yazıldı ya da
    /// işaretli bir kayıt gün sonunun içinde değil.
    /// </summary>
    public static readonly ApplicationError ExistingExceedsCash = new(
        "day_closes.existing_exceeds_cash",
        "The records already entered exceed the cash amount of the day close.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ExistingExceedsCard = new(
        "day_closes.existing_exceeds_card",
        "The records already entered exceed the card amount of the day close.",
        ApplicationErrorType.Validation);

    /// <summary>Nakit satış yazılacak ama kasa bilinmiyor; sunucu hesap uydurmaz.</summary>
    public static readonly ApplicationError CashAccountRequired = new(
        "day_closes.cash_account_required",
        "Choose the cash account the sales are written to.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CashAccountUnavailable = new(
        "day_closes.cash_account_unavailable",
        "An active owned cash account is required.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError CashCategoryRequired = new(
        "day_closes.cash_category_required",
        "Choose the income category the cash sales are written to.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CashCategoryUnavailable = new(
        "day_closes.cash_category_unavailable",
        "An active owned income category is required.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ScopeUnresolved = new(
        "day_closes.scope_unresolved",
        "The scope could not be resolved from the account or the category.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Gün zaten kapatılmış. İkinci bir cihazın gün sonuysa açıkça "ek gün
    /// sonu" olarak gönderilir; yanlış girildiyse önceki geri alınır.
    /// </summary>
    public static readonly ApplicationError AlreadyClosed = new(
        "day_closes.already_closed",
        "This day is already closed; send an additional day close or revert the existing one.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError NotClosedYet = new(
        "day_closes.not_closed_yet",
        "An additional day close needs a day that is already closed.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError ZNumberExists = new(
        "day_closes.z_number_exists",
        "A day close with this Z number already exists.",
        ApplicationErrorType.Conflict);

    /// <summary>Ürettiği bir tahsilat hesaba geçmiş; önce yatış geri alınır.</summary>
    public static readonly ApplicationError DepositLocked = new(
        "day_closes.deposit_locked",
        "A pos settlement of this day close is deposited; revert the deposit first.",
        ApplicationErrorType.Conflict);

    /// <summary>
    /// Günün ek gün sonu duruyor; ana gün sonu ondan önce geri alınamaz. Aksi
    /// hâlde ek sahipsiz kalır ve güne yeniden gün sonu girilemezdi.
    /// </summary>
    public static readonly ApplicationError AdditionalExists = new(
        "day_closes.additional_exists",
        "Revert the additional day closes of this day first.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError ConcurrentChange = new(
        "day_closes.concurrent_change",
        "The day changed while the day close was being written; read it again.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError InvalidRange = new(
        "day_closes.invalid_range",
        "The end date cannot be before the start date and the range is at most 366 days.",
        ApplicationErrorType.Validation);

    public static ApplicationError NotFound(Guid id) => new(
        "day_closes.not_found",
        $"Day close '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Validation(string message) => new(
        "day_closes.validation",
        message,
        ApplicationErrorType.Validation);

    public static ApplicationError Conflict(string message) => new(
        "day_closes.conflict",
        message,
        ApplicationErrorType.Conflict);
}
