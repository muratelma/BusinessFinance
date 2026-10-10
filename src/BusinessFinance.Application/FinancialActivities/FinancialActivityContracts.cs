using BusinessFinance.Domain;

namespace BusinessFinance.Application.FinancialActivities;

/// <summary>Which real economic event happened.</summary>
public enum FinancialActivityKind
{
    AccountTransaction = 1,
    Transfer = 2,
    CardCharge = 3,
    CardPayment = 4,
    DebtPayment = 5,
    DebtCollection = 6,

    /// <summary>
    /// Borcun doğduğu an. Nakit kaynaklıysa para hesaba girmiş (borç) ya da
    /// çıkmıştır (alacak) ve etkisi nötrdür; gider kaynaklıysa tüketim tam o
    /// gün gider olarak yazılır. Taksit ödemeleri buna ek bir gider üretmez.
    /// </summary>
    DebtOpening = 7,

    /// <summary>
    /// Veresiye satış ya da tedarikçiden vadeli alım: gelir/gider tam o gün
    /// tanınır, kasa kıpırdamaz (ADR 0014).
    /// </summary>
    CounterpartyCharge = 8,

    /// <summary>
    /// Cari tahsilat ya da ödeme: kasa değişir, gelir/gider üretilmez.
    /// Ekonomik olay borçlandırmada zaten tanınmıştır.
    /// </summary>
    CounterpartySettlement = 9,

    /// <summary>
    /// Tek seferlik vadeli ekonomik olay. Yöne göre gelir/gideri düzenleme
    /// tarihinde tanır; ödeme daha sonra ayrı, nötr bir hareket olacaktır.
    /// </summary>
    Obligation = 10,
    ObligationSettlement = 11,

    /// <summary>
    /// POS satışının tanındığı an: gelir brüt tutar kadar yazılır, hesap
    /// kıpırdamaz. Bankanın kestiği komisyon ayrı bir satır <b>değildir</b>;
    /// bu satırın parçasıdır (<see cref="FinancialActivityRow.FeeAmount"/>).
    /// Tutar yine brüttür: komisyon ondan düşülerek gösterilseydi kullanıcının
    /// gerçekten kestiği fatura küçülürdü.
    /// </summary>
    /// <remarks>
    /// 13 değeri eskiden komisyonun kendi satırıydı (<c>pos-commission</c>) ve
    /// 2 Ekim 2026'da kalktı: beş satışın beş komisyonu alt alta beş ayrı
    /// satır olunca hangisinin hangi satışa ait olduğu okunmuyordu.
    /// </remarks>
    PosSale = 12,

    /// <summary>
    /// Yoldaki paranın hesaba yattığı an (ADR 0019 T5): bir yatış bir ya da
    /// birkaç tahsilatı kapatır, gelir/gider <b>yeniden tanınmaz</b>
    /// (ADR 0014). Satırın tutarı bankanın gerçekten yatırdığı tutardır;
    /// beklenenden eksik kalan kısım (kesinti) bu satırın parçasıdır
    /// (<see cref="FinancialActivityRow.FeeAmount"/>), ayrı satır değildir.
    /// </summary>
    PosDeposit = 14
}

/// <summary>Effect on the income/expense report.</summary>
public enum FinancialActivityEffect
{
    Income = 1,
    Expense = 2,
    Neutral = 3
}

/// <summary>The group shown to the user, not the storage table.</summary>
public enum FinancialActivitySourceGroup
{
    Account = 1,
    CreditCard = 2,
    Transfer = 3,
    Debt = 4,

    /// <summary>
    /// Açık cari. Taksitli sözleşme <see cref="Debt"/> olarak kalır: ikisi
    /// aynı kişiye ait olsa da farklı sorular sorar — biri yürüyen bir
    /// hesap, diğeri vadesi belli bir plan.
    /// </summary>
    Counterparty = 5,
    Obligation = 6,

    /// <summary>
    /// POS tahsilatı. Kredi kartından ayrı bir gruptur ve olmak zorundadır:
    /// biri borçlandığın kart, diğeri müşterinin ödediği para (ADR 0015).
    /// </summary>
    Pos = 7
}

/// <summary>How the activity was produced.</summary>
public enum FinancialActivityOrigin
{
    Manual = 1,
    CsvImport = 2,
    Recurring = 3,
    Installment = 4,

    /// <summary>
    /// Bir POS yatışının kesinti gideri: yatışla birlikte doğar ve yalnız
    /// onunla birlikte geri alınır.
    /// </summary>
    PosDeposit = 5,

    /// <summary>
    /// Bir gün sonunun ürettiği nakit gelir ya da POS satışı (ADR 0019 T1):
    /// gün sonuyla birlikte doğar ve yalnız onunla birlikte geri alınır.
    /// </summary>
    DayClose = 6
}

public enum FinancialActivityStatus
{
    Realized = 1,
    Cancelled = 2
}

/// <summary>
/// One realized economic event, projected from whichever write model produced it.
/// This is a read shape only: no table backs it and nothing is written through it.
/// </summary>
/// <remarks>
/// <see cref="Title"/> carries user-owned text (a category, account, card or
/// counterparty name), never a server-side sentence. The type label belongs to the
/// client, which already localises its own strings; baking one language into the API
/// would strand every other client.
/// </remarks>
public sealed record FinancialActivityRow(
    Guid ActivityId,
    FinancialActivityKind ActivityKind,
    FinancialActivityEffect Effect,
    FinancialActivitySourceGroup SourceGroup,
    FinancialActivityOrigin Origin,
    FinancialActivityStatus Status,
    DateOnly ActivityDate,
    decimal Amount,
    CurrencyCode Currency,
    string Title,
    string? Description,
    Guid? CategoryId,
    string? CategoryName,
    Guid? SourceId,
    string? SourceName,
    Guid? DestinationId,
    string? DestinationName,
    DateTimeOffset? CancelledAtUtc,

    /// <summary>
    /// Kaydın kapsamı; transfer ve kart ödemesinde <c>null</c>.
    /// </summary>
    /// <remarks>
    /// İkisi de gelir/gider raporuna sıfır etki eder (ADR 0002, ADR 0003) ve
    /// kapsam taşımaz. Kapsam filtresi verildiğinde bu satırlar feed'den düşer:
    /// ikisini birden iki listede birden göstermek, kullanıcı tarafları
    /// karşılaştırdığında aynı para hareketini iki kez saydırırdı.
    /// </remarks>
    TransactionScope? Scope,

    /// <summary>
    /// Borç taksidinin faiz payı; diğer türlerde <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Faiz ayrı bir hareket **değildir**: taksit ödemesi tek para hareketi
    /// (2.600) ve içindeki faiz (100) o hareketin bir bölünmesi. Çift taraflı
    /// muhasebede de tek yevmiye kaydının ayrı bir ayağıdır, ayrı bir kayıt
    /// değil — ayrı yazılsaydı işlemler toplamı hesaptan çıkan parayla
    /// tutmazdı.
    ///
    /// İkisi de taşınıyor, anapara istemcide çıkarılmıyor: para aritmetiği
    /// istemcide yapılmaz. Ayrışma riski de yok — ikisi de aynı satırda
    /// sözleşme anında sabitlenmiş hâlleriyle saklı.
    /// </remarks>
    decimal? PrincipalPortion,

    /// <inheritdoc cref="PrincipalPortion" />
    decimal? InterestPortion,

    /// <summary>
    /// Kaydın geldiği POS'un adı. Satışta tahsilatın POS'u; yatışta kapattığı
    /// tahsilatların hepsi aynı POS'tansa onun adı. POS seçilmeden girilende
    /// ve karışık yatışta <c>null</c>.
    /// </summary>
    string? ChannelName = null,

    /// <summary>
    /// Kaydın <b>parçası</b> olan gider: POS satışında komisyon, yatışta
    /// kesinti. Sıfırsa <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Bu tutar bir gider olarak tanınmıştır ve raporlarda, bütçede sayılır;
    /// yalnız akışta ayrı satır olmaz, bağlı olduğu kaydın satırında ve
    /// ayrıntısında gösterilir. <see cref="Amount"/> ondan etkilenmez: satışta
    /// brüt, yatışta gerçekten yatan tutardır.
    /// </remarks>
    decimal? FeeAmount = null,

    /// <summary>POS satışında hesaba geçecek (ya da geçmiş) net tutar.</summary>
    decimal? NetAmount = null,

    /// <summary>POS satışında paranın beklendiği gün.</summary>
    DateOnly? ExpectedTransferDate = null,

    /// <summary>POS satışında paranın hesaba geçtiği gün; yoldaysa <c>null</c>.</summary>
    DateOnly? TransferredOn = null,

    /// <summary>Yatışın kapattığı tahsilat sayısı; geri alınmış yatışta sıfır.</summary>
    int? SettlementCount = null,

    /// <summary>
    /// Cari ve yükümlülük kayıtlarında yön: alacak mı, borç mu. Aynı tür iki
    /// yönü de taşır (tahsilat / ödeme); istemci adı ve paranın akış yönünü
    /// bundan kurar. Diğer türlerde boştur.
    /// </summary>
    DebtDirection? Direction = null,

    /// <summary>
    /// Kaydın bağlı olduğu gün sonu: gün sonunun <b>yazdığı</b> kayıtta
    /// (kökeni <see cref="FinancialActivityOrigin.DayClose"/>) ya da gün
    /// sonunun <b>saydığı</b>, tek tek girilmiş kayıtta doludur. İkisi de tek
    /// başına iptal edilemez; gün sonu geri alınır.
    /// </summary>
    Guid? DayCloseId = null,

    /// <summary>
    /// Kaydın iptalini başka bir kayıt kilitliyor: kapanışı bir gün sonunda
    /// sayılmış ya da kartla tahsilin parası bir yatışla hesaba geçmiş
    /// yükümlülük. Yalnız <c>canCancel</c> hesabında kullanılır; sözleşmeye
    /// çıkmaz. Yükümlülük satırı <see cref="DayCloseId"/> değerini yalnız
    /// kendisi sayıldıysa taşır (nakit tutarından düşülen alacak faturası);
    /// yalnız kapanışı sayıldıysa kilit buradan gelir.
    /// </summary>
    bool CancelLocked = false);

/// <summary>
/// Bakiyesi gösterilen yer: bir hesap, bir kredi kartı, bir karşı tarafın
/// carisi ya da bir borç anlaşması.
/// </summary>
public enum ActivityBalanceHolder
{
    Account = 1,
    CreditCard = 2,

    /// <summary>Karşı tarafın açık cari bakiyesi (alacak ya da borç).</summary>
    Counterparty = 3,

    /// <summary>Borç anlaşmasının kalan tutarı (ödenmemiş taksitler).</summary>
    Debt = 4
}

/// <summary>
/// Cari ve borç bakiyesinin tarafı. Tutar hep artıdır; kimin kime borçlu
/// olduğunu bu söyler, istemci işaretten türetmez.
/// </summary>
public enum ActivityBalanceSide
{
    /// <summary>Karşı taraf bize borçlu.</summary>
    Receivable = 1,

    /// <summary>Biz karşı tarafa borçluyuz.</summary>
    Payable = 2,

    /// <summary>Açık tutar kalmadı.</summary>
    Settled = 3
}

/// <summary>
/// Hareketin o bakiyeye ne yaptığı: artırdı, azalttı ya da dokunmadı.
/// </summary>
/// <remarks>
/// İstemci bunu hareketin türünden türetmez: cari tahsilatın ve yükümlülük
/// kapanışının yönü satırda yoktur. <see cref="Unchanged"/> yalnız tanıyan ama
/// para taşımayan harekette döner (POS satışı): para henüz yoldadır.
/// </remarks>
public enum ActivityBalanceChange
{
    Unchanged = 0,
    Increased = 1,
    Decreased = 2
}

/// <summary>
/// Bir hareketten <b>hemen sonra</b> hesabın bakiyesi ya da kartın borcu.
/// </summary>
/// <remarks>
/// Kalıcı bir alan değildir; okunduğu anda hareketlerden hesaplanır (bakiye
/// kalıcı kolon değildir). "Sonra", akışın sırasıdır: önce kaydın günü, gün
/// içinde kaydın girildiği an.
/// </remarks>
public sealed record ActivityBalanceAfter(
    ActivityBalanceHolder Holder,
    Guid HolderId,
    string Name,
    decimal Balance,
    CurrencyCode Currency,
    ActivityBalanceChange Change,
    // Yalnız kartta: o andaki borca göre kalan limit. Limitin geçmişi
    // tutulmaz; kartın <b>bugünkü</b> limitiyle hesaplanır.
    decimal? AvailableLimit = null,
    // Yalnız cari ve borçta.
    ActivityBalanceSide? Side = null,
    // Yalnız caride: hareketten hemen önceki taraf. Sonrakinden farklıysa
    // hareket tarafı çevirmiştir (alacak kapandı, geriye borç kaldı).
    ActivityBalanceSide? PreviousSide = null);

public interface IActivityBalanceReader
{
    /// <summary>
    /// Hareketin dokunduğu hesap ve kartların o hareketten sonraki bakiyesi.
    /// </summary>
    /// <returns>
    /// Hareket bu kullanıcıya ait değilse ya da yoksa <c>null</c>. Hareket
    /// iptal edilmişse ya da ne zaman girildiği bilinmiyorsa boş liste:
    /// bilinmeyen bir sıra için bakiye uydurulmaz. POS satışı hesabını
    /// <b>değişmemiş</b> olarak döner: satış tanır, para yatışla geçer.
    /// Cari kayıt karşı tarafın açık bakiyesini, borç kaydı anlaşmanın kalan
    /// tutarını da döner.
    /// </returns>
    Task<IReadOnlyList<ActivityBalanceAfter>?> GetBalancesAfterAsync(
        Guid userId,
        FinancialActivityKind kind,
        Guid activityId,
        CancellationToken cancellationToken);
}

/// <summary>
/// A <see cref="FinancialActivityRow"/> plus the capabilities the client may act on.
/// </summary>
public sealed record FinancialActivityDto(
    FinancialActivityRow Activity,
    bool CanCancel,
    bool SupportsAttachments);

/// <summary>
/// Filters are single valued in this version: the feed is one UNION ALL query and
/// multi-select would multiply its shape without a proven need.
/// </summary>
public sealed record FinancialActivityListCriteria(
    int PageNumber,
    int PageSize,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    FinancialActivitySourceGroup? SourceGroup,
    FinancialActivityKind? ActivityKind,
    FinancialActivityEffect? Effect,
    FinancialActivityOrigin? Origin,
    Guid? AccountId,
    Guid? CreditCardId,
    Guid? CategoryId,

    /// <summary>
    /// Bir kişiyle olan bütün geçmiş: cari hareketler <b>ve</b> o kişiyle
    /// yapılmış taksitli sözleşmenin hareketleri. Karşı taraf ayrıntı
    /// ekranı tek bir soru sorar ("Ahmet'le ne oldu?") ve cevabı iki ayrı
    /// listeye bölmek onu kullanıcıya birleştirtirdi.
    /// </summary>
    Guid? CounterpartyId,

    // Boşsa toplam. Doluysa kapsamsız satırlar (transfer, kart ödemesi) da düşer.
    TransactionScope? Scope,
    bool IncludeCancelled,

    /// <summary>
    /// Kayıt adı, açıklama, kategori ve hesap/karşı taraf adında geçen metin;
    /// boşsa filtre yok.
    /// </summary>
    /// <remarks>
    /// Sayfalı listeyi istemcide süzmek yalnız yüklenmiş sayfayı arardı; arama
    /// bu yüzden aynı tek sorgunun bir filtresidir ve sayım da ona göre yapılır.
    /// </remarks>
    string? Search = null);

public sealed record FinancialActivityPage(
    IReadOnlyList<FinancialActivityRow> Items,
    int TotalCount);

public sealed record FinancialActivityListResult(
    IReadOnlyList<FinancialActivityDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);

public interface IFinancialActivityRepository
{
    /// <summary>
    /// Merges every realized event of one owner into a single ordered, paged result.
    /// Ordering, paging and counting must happen in the database: reading the whole
    /// history into memory to page it would make every page cost grow with the size
    /// of the history.
    /// </summary>
    Task<FinancialActivityPage> ListAsync(
        Guid userId,
        FinancialActivityListCriteria criteria,
        CancellationToken cancellationToken);
}

/// <summary>
/// Reads what produced a single already-persisted result, so a cancel path can apply
/// the same origin rule the feed reports through <c>canCancel</c>.
/// </summary>
public interface IActivityOriginReader
{
    Task<FinancialActivityOrigin> GetTransactionOriginAsync(
        Guid userId,
        Guid transactionId,
        CancellationToken cancellationToken);

    Task<FinancialActivityOrigin> GetCardChargeOriginAsync(
        Guid userId,
        Guid creditCardChargeId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Which realized activities may still be cancelled.
/// </summary>
/// <remarks>
/// A recurring occurrence and an installment item each carry exactly one result id and
/// have no way back, so cancelling their result would strand the source in a realized
/// state pointing at a cancelled row. Debt payments have no reversal either. The rule
/// lives here so the feed, the detail sheet and the cancel endpoints all read it from
/// one place rather than each deciding for itself.
/// </remarks>
public static class FinancialActivityCapabilities
{
    public static bool CanCancel(
        FinancialActivityKind kind,
        FinancialActivityOrigin origin,
        FinancialActivityStatus status)
    {
        if (status != FinancialActivityStatus.Realized) return false;

        // Borç açılışı satırın kendisi değil sözleşmenin ta kendisidir; onu
        // iptal etmek borcu silmek olurdu ve ödenmiş taksitler sahipsiz
        // kalırdı. Borcu bitirmenin yolu bu satır değil, sözleşme akışıdır.
        if (kind is FinancialActivityKind.DebtPayment
            or FinancialActivityKind.DebtCollection
            or FinancialActivityKind.DebtOpening
            // Yükümlülüğün kapanışı tek başına geri alınmaz: yükümlülük bir
            // bütün olarak iptal edilir ve kapanışı onunla birlikte iptal olur.
            // İptal edilebilen satır yükümlülüğün kendisidir.
            or FinancialActivityKind.ObligationSettlement
            // POS tahsilatı kendi ekranından iptal edilir: iptal satışı,
            // komisyonu ve yoldaki tutarı birlikte kaldırır.
            or FinancialActivityKind.PosSale
            // Yatış kendi ucundan geri alınır: geri alma kapattığı
            // tahsilatları yola döndürür ve kesinti giderini iptal eder.
            or FinancialActivityKind.PosDeposit)
        {
            return false;
        }

        // Cari hareketin iki türü de iptal edilebilir ve iptalleri kendi
        // türevlerini birlikte geri alır: borçlandırma tanıdığı gelir/gideri,
        // tahsilat taşıdığı parayı. Sözleşmeden farkları burada: ikisi de tek
        // başına duran bir kayıt, geri dönüşü olmayan bir planın sonucu değil.

        // Yatışın kesinti gideri tek başına iptal edilemez: iptal edilseydi
        // yatış, hesaba gerçekte geçmemiş bir tutarı geçmiş gösterirdi. Gün
        // sonunun geliri de öyle: gün sonu bir bütün olarak geri alınır.
        return origin is not (FinancialActivityOrigin.Recurring
            or FinancialActivityOrigin.Installment
            or FinancialActivityOrigin.PosDeposit
            or FinancialActivityOrigin.DayClose);
    }

    /// <summary>
    /// Attachments exist only for budget transactions today, so every other kind hides
    /// the document action instead of offering one that cannot work.
    /// </summary>
    public static bool SupportsAttachments(FinancialActivityKind kind) =>
        kind == FinancialActivityKind.AccountTransaction;
}
