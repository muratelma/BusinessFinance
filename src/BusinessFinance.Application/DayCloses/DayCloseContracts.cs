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
/// Bir grubun (kişinin ya da faturanın) satışı ile tahsilatı girilen nakit
/// tutarında birlikte yer alıyorsa, ikisinde <b>ortak</b> olan tutar.
/// </summary>
/// <remarks>
/// Ortak tutar, tahsilatın hangi satışa ait olduğunu değil, girilen toplamda
/// satış ve tahsilatın nasıl sayıldığını söyler: iki kayıtta da görünen ama
/// girilen tutarda bir kez yer alan paradır. Düşülen = satışlar + tahsilatlar
/// − ortak tutar. Sıfır "ayrı ayrı sayıldı" demektir.
/// </remarks>
public sealed record DayCloseOverlap(Guid GroupId, decimal Amount);

/// <summary>
/// Gün sonunun girdisi; önizleme ve kayıt aynı girdiyi alır ve aynı hesaptan
/// geçer.
/// </summary>
/// <remarks>
/// Bir taraf yalnız <b>kendi tutarı yazıldıysa</b> kayıt üretir: yalnız nakit
/// ya da yalnız kart verilirse öbür tarafa dokunulmaz. Toplam hiçbir zaman
/// kayıt üretmez ve eksik tarafı hesaplamak için kullanılmaz; raporun toplamı
/// başka ödeme türlerini de (kredili satış, yemek kartı) içerebilir. Verildiyse
/// yalnız farkı gösterir.
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
    int? ZNumber,
    IReadOnlyList<DayCloseOverlap>? Overlaps = null);

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
    // Girilen tutara dahil mi. Boş: cevaplanmadı (yalnız cevap isteyen
    // kayıtlarda); cevaplanmadan gün sonu yazılmaz.
    bool? Included,
    // Kart tarafında bir alacağın kartla tahsili (satış değil); başlık kişinin
    // adıdır. Satış gibi varsayılan olarak düşülür (KP7).
    bool IsCardCollection = false,
    // Vadeli satış ve nakit tahsilat hazır cevapla gelmez: uygulama bunların
    // girilen tutarın içinde olup olmadığını kayıtlardan bilemez.
    bool RequiresAnswer = false,
    // Satışı ile tahsilatı aynı parayı gösterebilecek kayıtların grubu: cari
    // kayıtta kişi, alacak faturasında fatura. Öbür kayıtlarda boş.
    Guid? GroupId = null,
    string? GroupName = null,
    // Kaydın uygulamaya girildiği an; bu bilgiden önce yazılmış kayıtta boş.
    DateTimeOffset? CreatedAtUtc = null)
{
    /// <summary>Gelir yazmış ama parası (tamamı) alınmamış olabilecek satış.</summary>
    public bool IsDeferredSale =>
        Kind is DayCloseRecordKind.CounterpartyCharge or DayCloseRecordKind.Obligation;

    /// <summary>Daha önce gelir yazılmış bir alacağın nakit tahsilatı.</summary>
    public bool IsCollection =>
        Kind is DayCloseRecordKind.CounterpartyPayment or DayCloseRecordKind.ObligationSettlement;
}

/// <summary>Ortak tutarı sorulan grubun ne olduğu: bir kişi ya da bir fatura.</summary>
public enum DayCloseGroupKind
{
    Counterparty = 1,
    Obligation = 2
}

/// <summary>Grubun dahil edilen satışları ile tahsilatlarından büyük olanı.</summary>
public enum DayCloseLargerSide
{
    /// <summary>Satışlar tahsilatlardan büyük ya da ikisi eşit.</summary>
    Sales = 1,
    Collections = 2
}

/// <summary>
/// Satışı da tahsilatı da girilen nakit tutarına dahil edilmiş bir grup:
/// ortak tutarı sorulur.
/// </summary>
/// <remarks>
/// Panel üç cevabı sonuçlarıyla gösterir ve hiçbirini kendisi hesaplamaz:
/// "ayrı ayrı" <see cref="SeparateAmount"/>, "biri öbürünün içinde"
/// <see cref="InsideAmount"/>, "bir kısmı" yazılan tutarla
/// <see cref="DeductedAmount"/> kadar düşer.
/// </remarks>
public sealed record DayCloseOverlapGroupDto(
    Guid GroupId,
    string Name,
    decimal SalesAmount,
    decimal CollectionsAmount,
    // Ortak tutar en çok bu kadar olabilir: iki toplamdan küçük olanı.
    decimal MaximumOverlap,
    // Boş: cevaplanmadı.
    decimal? OverlapAmount,
    // Satışlar + tahsilatlar − ortak tutar; cevaplanmadıysa boş.
    decimal? DeductedAmount,
    DayCloseGroupKind Kind,
    // Ortak tutar sıfırken düşülen: satışlar + tahsilatlar.
    decimal SeparateAmount,
    // Ortak tutar en çokken düşülen: iki toplamdan büyük olanı.
    decimal InsideAmount,
    DayCloseLargerSide LargerSide)
{
    /// <summary>
    /// Grubun dahil edilen kayıtlarından ve (verildiyse) ortak tutarından kurar.
    /// Önizleme ve günün ekranı aynı hesabı kullanır.
    /// </summary>
    public static DayCloseOverlapGroupDto From(
        Guid groupId,
        IReadOnlyCollection<DayCloseExistingRecordDto> records,
        decimal? overlapAmount)
    {
        var sales = records.Where(record => record.IsDeferredSale).Sum(record => record.Amount);
        var collections = records.Where(record => record.IsCollection).Sum(record => record.Amount);
        return new DayCloseOverlapGroupDto(
            groupId,
            records.Select(record => record.GroupName).FirstOrDefault(name => name is not null)
                ?? string.Empty,
            sales,
            collections,
            Math.Min(sales, collections),
            overlapAmount,
            overlapAmount is decimal amount ? sales + collections - amount : null,
            records.Any(record => record.Kind
                is DayCloseRecordKind.Obligation or DayCloseRecordKind.ObligationSettlement)
                ? DayCloseGroupKind.Obligation
                : DayCloseGroupKind.Counterparty,
            sales + collections,
            Math.Max(sales, collections),
            collections > sales ? DayCloseLargerSide.Collections : DayCloseLargerSide.Sales);
    }
}

/// <summary>
/// Nakit tutarından düşülenin dökümü: dahil edilen kayıtlar türüne göre ve
/// iki kayıtta da görünen para. Dört tutarın toplamı eksi ortak tutar,
/// düşülen tutardır.
/// </summary>
public sealed record DayCloseCashDeductionsDto(
    // Tek tek girilmiş nakit satışlar.
    decimal SalesAmount,
    // Nakit tahsilatlar (cari tahsilat ve alacak faturasının tahsilatı).
    decimal CollectionsAmount,
    // O gün yazılmış veresiye satışlar.
    decimal CreditSalesAmount,
    // O gün yazılmış alacak faturaları.
    decimal InvoicesAmount,
    // Cevaplanmış ortak tutarların toplamı; bir kez düşülür.
    decimal SharedAmount);

/// <summary>Panelin nakit satırı: girilen, düşülen ve yazılacak tutar.</summary>
public sealed record DayCloseCashLineDto(
    // Nakit tutarı verildi; değilse nakit tarafına dokunulmaz ve aşağıdaki
    // tutarlar sıfırdır.
    bool Stated,
    decimal EnteredAmount,
    decimal DeductedAmount,
    decimal AmountToWrite,
    Guid? AccountId,
    string? AccountName,
    Guid? CategoryId,
    string? CategoryName,
    DayCloseCashDeductionsDto Deductions);

/// <summary>Panelin bir POS satırı.</summary>
public sealed record DayClosePosLineDto(
    Guid PosDefinitionId,
    string Name,
    bool IsDefault,
    string AccountName,
    bool Stated,
    decimal EnteredAmount,
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
    ApplicationError? Blocker,
    // Ortak tutarı sorulan gruplar; yoksa boş liste.
    IReadOnlyList<DayCloseOverlapGroupDto> OverlapGroups);

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
    // Nakit tutarından düşülen: sayılan nakit kayıtlar − ortak tutarlar.
    decimal CountedCashAmount,
    decimal CountedCardAmount,
    // Sayılan kayıtlar arasında iki kayıtta da görünen, bir kez düşülen para.
    IReadOnlyList<DayCloseOverlapGroupDto> Overlaps);

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
    /// kayıtlar: nakit hesaba işletme gelirleri, POS tahsilatları, nakit
    /// hesaba cari ya da alacak tahsilatları ve o gün yazılmış vadeli satışlar
    /// (veresiye satış, alacak faturası). Bir gün sonunun ürettiği ya da
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
        IReadOnlyCollection<DayCloseCountedOverlap> overlaps,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gün sonunun saydığı kayıtların bağını ve ortak tutarlarını kaldırır; değişiklik
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

    /// <summary>Nakit ya da kart tutarı; yalnız toplam yetmez.</summary>
    public static readonly ApplicationError AmountsRequired = new(
        "day_closes.amounts_required",
        "Enter the cash amount or a card amount; the total alone writes nothing.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError TotalBelowParts = new(
        "day_closes.total_below_parts",
        "The total cannot be less than the cash and card amounts entered.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Vadeli satış ya da nakit tahsilat için "girilen nakit tutarına dahil
    /// mi" cevabı verilmemiş. Sunucu cevap uydurmaz.
    /// </summary>
    public static readonly ApplicationError RecordsUnanswered = new(
        "day_closes.records_unanswered",
        "Say whether each deferred sale and cash collection is inside the cash amount.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Bir grubun satışı da tahsilatı da dahil edilmiş ama ortak tutarı
    /// verilmemiş: ikisinin toplamı mı, bir kez mi düşüleceği bilinmiyor.
    /// </summary>
    public static readonly ApplicationError OverlapUnanswered = new(
        "day_closes.overlap_unanswered",
        "Say how much of the sale and the collection is the same money in the cash amount.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Ortak tutar eksi, grubun dahil edilen satışlarından ya da
    /// tahsilatlarından büyük, ya da ortak tutarı sorulmayan bir grup için
    /// verilmiş.
    /// </summary>
    public static readonly ApplicationError InvalidOverlap = new(
        "day_closes.invalid_overlap",
        "The shared amount is between zero and the smaller of the group's sales and collections.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// İstek, listede artık olmayan bir kayıt için cevap taşıyor: kayıt iptal
    /// edildi ya da başka bir gün sonunda sayıldı. Panel güncel listeyi
    /// yeniden gösterir.
    /// </summary>
    public static readonly ApplicationError RecordsChanged = new(
        "day_closes.records_changed",
        "The records of the day changed; read them again.",
        ApplicationErrorType.Conflict);

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
    /// Açık seçim ya da kategori, kaydın alabileceği tarafla çelişiyor (ADR 0020 İ4).
    /// </summary>
    public static readonly ApplicationError ScopeConflict = new(
        "day_closes.scope_conflict",
        "The requested scope or the category conflicts with the side this record may take.",
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
