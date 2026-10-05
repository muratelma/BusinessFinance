using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

public interface ICounterpartyRepository
{
    /// <summary>
    /// Karşı taraf başına cari bakiye. <b>Tek sorgu</b>: liste kaç kişi
    /// büyürse büyüsün sorgu sayısı sabit kalır.
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
    /// kayıtlarında arar ve bulduğunu <b>önerir</b> (ADR 0011).
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

    Task AddChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken);

    /// <summary>
    /// Tahsilatı ve kartla tahsilse POS kaydını tek <c>SaveChanges</c> ile
    /// yazar: biri yazılıp öbürü yazılmasaydı ya cari kapanır ama para hiçbir
    /// yerde görünmez ya da para yolda görünür ama cari açık kalırdı.
    /// </summary>
    Task AddPaymentAsync(
        CounterpartyPayment payment,
        PosSettlement? cardSettlement,
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
