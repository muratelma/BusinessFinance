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
}
