using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

public interface ICounterpartyRepository
{
    /// <summary>
    /// Karşı taraf başına cari bakiye. Sorgu sayısı sabittir (bakiye ve
    /// gecikme için iki sorgu): liste kaç kişi büyürse büyüsün değişmez.
    /// </summary>
    Task<IReadOnlyList<CounterpartyBalanceSummary>> ListBalancesAsync(
        Guid userId,
        CounterpartyBalanceFilter filter,
        bool? isActive,
        DateOnly asOfDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Tek karşı tarafın bakiyesi; başkasının kaydı ve olmayan kayıt aynı
    /// <c>null</c> sonucuna gider.
    /// </summary>
    Task<CounterpartyBalanceSummary?> FindBalanceAsync(
        Guid counterpartyId,
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken);

    Task<Counterparty?> FindOwnedByIdAsync(
        Guid counterpartyId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adı yazılan karşı tarafı bulur, yoksa kurar. Kullanıcı borç açarken
    /// önce karşı taraf oluşturmak zorunda kalmasın diye.
    /// </summary>
    /// <remarks>
    /// <b>Kaydetmez.</b> Yeni karşı taraf, onu isteyen yazma işlemiyle aynı
    /// kaydetme sınırında yazılır; ayrı kaydetmek, borç doğrulamada
    /// düştüğünde ortada sahipsiz bir karşı taraf bırakırdı.
    /// </remarks>
    Task<Counterparty> FindOrCreateByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// Verilen karşı tarafların adları, tek sorguda. Ad artık yalnız burada
    /// yaşadığı için okuma yolları onu buradan alır.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> ListNamesAsync(
        Guid userId,
        IReadOnlyCollection<Guid> counterpartyIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adı verilen karşı tarafı arar ve <b>kurmaz</b>. Fiş okuma bu yolu
    /// kullanır: model bir ad okur, uygulama onu kullanıcının kendi
    /// kayıtlarında arar ve bulduğunu <b>önerir</b> (ADR 0011). Eşleştirme
    /// hoşgörülüdür (<see cref="CounterpartyNameMatcher"/>): büyük-küçük harf,
    /// şirket unvanı ve küçük yazım farkı eşleşmeyi bozmaz; birden çok aday
    /// aynı ölçüde uyuyorsa sonuç boştur.
    /// </summary>
    Task<Counterparty?> FindOwnedByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        Guid? exceptCounterpartyId,
        CancellationToken cancellationToken);

    Task AddAsync(Counterparty counterparty, CancellationToken cancellationToken);

    Task UpdateOwnedAsync(
        Counterparty counterparty,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Hiç hareketi olmayan karşı tarafı siler; hareketi varsa <c>false</c>
    /// döner ve kayıt yerinde kalır (boş hesap kuralının aynısı).
    /// </summary>
    Task<bool> DeleteIfWithoutHistoryAsync(
        Guid counterpartyId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <returns>
    /// Aynı kimlikle bir borçlandırma zaten yazılmışsa <c>false</c>; çağıran
    /// var olanı döner (tekrar gönderilen istek ikinci kayıt yazmaz).
    /// </returns>
    /// <remarks>
    /// <paramref name="paidNow"/> verilirse peşin ya da avans tutarı satışla
    /// aynı <c>SaveChanges</c> içinde yazılır: biri yazılıp öbürü yazılmasaydı
    /// ya para alınmış ama satış görünmez ya da satış avanssız görünürdü.
    /// </remarks>
    Task<bool> AddChargeAsync(
        CounterpartyCharge charge,
        CancellationToken cancellationToken,
        CounterpartyPayment? paidNow = null);

    /// <summary>Satışla birlikte yazılmış, iptal edilmemiş tahsilat ya da ödemeler (izlenen).</summary>
    Task<IReadOnlyList<CounterpartyPayment>> FindPaidWithChargeAsync(
        Guid chargeId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Tahsilatı ve kartla tahsilse POS kaydını tek <c>SaveChanges</c> ile
    /// yazar: biri yazılıp öbürü yazılmasaydı ya cari kapanır ama para hiçbir
    /// yerde görünmez ya da para yolda görünür ama cari açık kalırdı.
    /// </summary>
    /// <returns>
    /// Aynı kimlikli tahsilat bu sırada yazıldıysa <c>false</c>: aynı istek
    /// kimliğiyle eşzamanlı gelen ikinci istek birincil anahtara çarpar.
    /// </returns>
    /// <remarks>
    /// <paramref name="discountedCharges"/>: <c>Kalanı indirim say</c> ile
    /// indirim yazılan alımlar; indirimler ödemeyle aynı yazmadadır.
    /// </remarks>
    Task<bool> AddPaymentAsync(
        CounterpartyPayment payment,
        PosSettlement? cardSettlement,
        CancellationToken cancellationToken,
        IReadOnlyCollection<CounterpartyCharge>? discountedCharges = null);

    /// <summary>
    /// Bir taraftaki açık kalemler, en yeniden eskiye (gün, giriş anı):
    /// alımlar (indirimler düşülmüş) ve borç para. Ödemeler en eski kalemi önce
    /// kapattığı için kalan tutar bu sıranın başındakilere aittir (ADR 0022 İ2).
    /// Devreden bakiye listede yoktur; en eski kalem odur.
    /// </summary>
    Task<IReadOnlyList<CounterpartyOpenItem>> ListItemsNewestFirstAsync(
        Guid counterpartyId,
        Guid userId,
        DebtDirection direction,
        CancellationToken cancellationToken);

    /// <summary>
    /// Kişi başına alım-satımlarının ve vazgeçmelerinin kapsamları (iptal
    /// edilmemiş). Türü bilinmeyen eski kişi <c>İşletme</c> ya da <c>Şahsi</c>
    /// görünümüne bunlarla girer (`04-SUNUCU-TASARIMI.md` §3.8).
    /// </summary>
    Task<IReadOnlyDictionary<Guid, IReadOnlySet<TransactionScope>>> ListRecordScopesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>Verilen alımlar, izlenen hâlde ve indirimleriyle.</summary>
    Task<IReadOnlyList<CounterpartyCharge>> FindOwnedChargesAsync(
        IReadOnlyCollection<Guid> chargeIds,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>Bu ödemeyle birlikte indirim yazılmış alımlar (izlenen).</summary>
    Task<IReadOnlyList<CounterpartyCharge>> FindChargesDiscountedByPaymentAsync(
        Guid paymentId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<CounterpartyCharge?> FindOwnedChargeAsync(
        Guid chargeId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<CounterpartyPayment?> FindOwnedPaymentAsync(
        Guid paymentId,
        Guid userId,
        CancellationToken cancellationToken);

    Task SaveChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken);

    /// <summary>Kartla tahsilin POS kaydı, izlenen hâlde.</summary>
    Task<PosSettlement?> FindCardSettlementAsync(
        Guid settlementId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Tahsilattaki değişikliği (ve varsa POS kaydındakini) yazar. POS kaydı bu
    /// sırada bir yatışa bağlandıysa <c>false</c> döner ve hiçbir şey yazılmaz.
    /// </summary>
    Task<bool> TrySavePaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken);
}
