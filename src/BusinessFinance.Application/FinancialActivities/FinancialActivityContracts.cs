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
    /// kıpırdamaz. Komisyon buna dâhil değildir; kendi satırında durur.
    /// </summary>
    PosSale = 12,

    /// <summary>
    /// Bankanın kestiği komisyon: satışla aynı gün tanınan ayrı bir gider.
    /// Brüt tutardan düşülerek gösterilseydi kullanıcının gerçekten kestiği
    /// fatura küçülür ve komisyon görünmez bir gidere dönerdi.
    /// </summary>
    PosCommission = 13,

    /// <summary>
    /// Yoldaki paranın hesaba geçtiği an: hesap net tutar kadar artar,
    /// gelir/gider <b>yeniden tanınmaz</b> (ADR 0014). Bu satır olmasaydı
    /// hesabın feed'i bakiyesindeki artışı açıklayamazdı.
    /// </summary>
    PosTransfer = 14
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
    Installment = 4
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
    decimal? InterestPortion);

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
    bool IncludeCancelled);

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
            // Yükümlülük aggregate'i geçmişi iptal edebilse de birleşik feed
            // henüz bu iki kayıt için bir iptal endpoint'i sunmuyor.
            or FinancialActivityKind.Obligation
            or FinancialActivityKind.ObligationSettlement
            // POS tahsilatı tek kaydın üç satırıdır; birini iptal etmek
            // diğer ikisini sahipsiz bırakırdı. İptal kaydın kendi
            // ekranından, tek eylemle yapılır ve üç satırı birlikte kapatır.
            or FinancialActivityKind.PosSale
            or FinancialActivityKind.PosCommission
            or FinancialActivityKind.PosTransfer)
        {
            return false;
        }

        // Cari hareketin iki türü de iptal edilebilir ve iptalleri kendi
        // türevlerini birlikte geri alır: borçlandırma tanıdığı gelir/gideri,
        // tahsilat taşıdığı parayı. Sözleşmeden farkları burada: ikisi de tek
        // başına duran bir kayıt, geri dönüşü olmayan bir planın sonucu değil.

        return origin is not (FinancialActivityOrigin.Recurring or FinancialActivityOrigin.Installment);
    }

    /// <summary>
    /// Attachments exist only for budget transactions today, so every other kind hides
    /// the document action instead of offering one that cannot work.
    /// </summary>
    public static bool SupportsAttachments(FinancialActivityKind kind) =>
        kind == FinancialActivityKind.AccountTransaction;
}
