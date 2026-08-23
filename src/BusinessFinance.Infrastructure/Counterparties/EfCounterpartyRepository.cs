using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Counterparties;

internal sealed class EfCounterpartyRepository(BusinessFinanceDbContext dbContext)
    : ICounterpartyRepository
{
    public async Task<IReadOnlyList<CounterpartyBalanceSummary>> ListBalancesAsync(
        Guid userId,
        CounterpartyBalanceFilter filter,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var query = ProjectBalances(userId);

        if (isActive is bool active)
        {
            query = query.Where(row => row.IsActive == active);
        }

        query = filter switch
        {
            CounterpartyBalanceFilter.Open =>
                query.Where(row => row.Receivable != 0m || row.Payable != 0m),
            CounterpartyBalanceFilter.Settled =>
                query.Where(row => row.Receivable == 0m && row.Payable == 0m),
            _ => query
        };

        // Sıralama da veritabanında: açık hesabı en büyük olan başta, eşitlikte
        // ada göre. Bellekte sıralamak, listenin tamamını okumayı zorunlu
        // kılardı.
        var rows = await query
            .OrderByDescending(row => Math.Abs(row.Receivable - row.Payable))
            .ThenBy(row => row.Name)
            .ToArrayAsync(cancellationToken);

        return rows
            .Select(row => new CounterpartyBalanceSummary(
                row.Id, row.Name, row.IsActive, row.Receivable, row.Payable))
            .ToArray();
    }

    public async Task<CounterpartyBalanceSummary?> FindBalanceAsync(
        Guid counterpartyId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var row = await ProjectBalances(userId)
            .SingleOrDefaultAsync(candidate => candidate.Id == counterpartyId, cancellationToken);

        return row is null
            ? null
            : new CounterpartyBalanceSummary(
                row.Id, row.Name, row.IsActive, row.Receivable, row.Payable);
    }

    public Task<Counterparty?> FindOwnedByIdAsync(
        Guid counterpartyId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Counterparties.SingleOrDefaultAsync(
            counterparty => counterparty.Id == counterpartyId && counterparty.UserId == userId,
            cancellationToken);
    }

    /// <summary>
    /// Bakiye = borçlandırmalar − tahsilatlar, iki yön ayrı ayrı.
    /// </summary>
    /// <remarks>
    /// Toplamlar karşı tarafın satırının içinde, ilişkili alt sorgular olarak
    /// duruyor: karşı taraf sayısı ne olursa olsun veritabanına <b>tek</b>
    /// ifade gider. Kişi başına ayrı bir toplam sorgusu, elli kayıtlık bir
    /// listede yüz sorgu demek olurdu.
    ///
    /// İptal edilmiş hareket hiç sayılmaz; silme yerine iptal kuralının
    /// buradaki karşılığı bu filtredir.
    /// </remarks>
    private IQueryable<BalanceRow> ProjectBalances(Guid userId)
    {
        return dbContext.Counterparties
            .AsNoTracking()
            .Where(counterparty => counterparty.UserId == userId)
            .Select(counterparty => new BalanceRow
            {
                Id = counterparty.Id,
                Name = counterparty.Name,
                IsActive = counterparty.IsActive,
                Receivable =
                    (dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Receivable)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m) -
                    (dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Receivable)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m),
                Payable =
                    (dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Payable)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m) -
                    (dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Payable)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m)
            });
    }

    private sealed class BalanceRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public decimal Receivable { get; init; }
        public decimal Payable { get; init; }
    }
}
