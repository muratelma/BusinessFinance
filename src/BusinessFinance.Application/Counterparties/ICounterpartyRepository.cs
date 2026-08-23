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
        CancellationToken cancellationToken);

    /// <summary>
    /// Tek karşı tarafın bakiyesi; başkasının kaydı ve olmayan kayıt aynı
    /// <c>null</c> sonucuna gider.
    /// </summary>
    Task<CounterpartyBalanceSummary?> FindBalanceAsync(
        Guid counterpartyId,
        Guid userId,
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
}
