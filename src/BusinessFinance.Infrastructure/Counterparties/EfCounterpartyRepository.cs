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
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var query = ProjectBalances(userId, asOfDate);

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
                row.Id,
                row.Name,
                row.IsActive,
                row.Receivable,
                row.Payable,
                Math.Max(0m, row.OverdueReceivableCharges - row.ReceivablePayments) +
                    row.OverdueReceivableObligations,
                Math.Max(0m, row.OverduePayableCharges - row.PayablePayments) +
                    row.OverduePayableObligations))
            .ToArray();
    }

    public async Task<CounterpartyBalanceSummary?> FindBalanceAsync(
        Guid counterpartyId,
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        var row = await ProjectBalances(userId, asOfDate)
            .SingleOrDefaultAsync(candidate => candidate.Id == counterpartyId, cancellationToken);

        return row is null
            ? null
            : new CounterpartyBalanceSummary(
                row.Id,
                row.Name,
                row.IsActive,
                row.Receivable,
                row.Payable,
                Math.Max(0m, row.OverdueReceivableCharges - row.ReceivablePayments) +
                    row.OverdueReceivableObligations,
                Math.Max(0m, row.OverduePayableCharges - row.PayablePayments) +
                    row.OverduePayableObligations);
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
    /// Adı yazılan karşı tarafı bulur, yoksa kurar ve <b>kaydetmeden</b> döner.
    /// </summary>
    /// <remarks>
    /// Kaydetmeyi çağıran yazma işlemi üstlenir; böylece yeni karşı taraf ile
    /// onu isteyen sözleşme aynı SaveChanges sınırında yazılır. Ayrı kaydetmek,
    /// sözleşme doğrulamada düştüğünde ortada sahipsiz bir karşı taraf
    /// bırakırdı.
    ///
    /// Pasif bir karşı taraf da bulunur ve olduğu gibi döner: yeni iş yapmayı
    /// engellemek karşı tarafın değil, kaydın kuralıdır ve orada uygulanır.
    /// </remarks>
    public async Task<Counterparty> FindOrCreateByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken)
    {
        var normalized = name?.Trim() ?? string.Empty;
        var existing = await FindByNameAsync(userId, normalized, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        // Ad boşsa ya da sınırı aşıyorsa burada patlar; doğrulama tek yerde,
        // Counterparty'nin kendisinde durur.
        var created = new Counterparty(Guid.NewGuid(), userId, normalized);
        await dbContext.Counterparties.AddAsync(created, cancellationToken);
        return created;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ListNamesAsync(
        Guid userId,
        IReadOnlyCollection<Guid> counterpartyIds,
        CancellationToken cancellationToken)
    {
        if (counterpartyIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await dbContext.Counterparties
            .AsNoTracking()
            .Where(counterparty => counterparty.UserId == userId &&
                                   counterpartyIds.Contains(counterparty.Id))
            .ToDictionaryAsync(
                counterparty => counterparty.Id,
                counterparty => counterparty.Name,
                cancellationToken);
    }

    /// <remarks>
    /// SQL Server’da karşılaştırma kolonun harf duyarsız collation’ıyla yapılır;
    /// InMemory sağlayıcısı aynı şeyi yapmadığı için orada açıkça duyarsız
    /// karşılaştırılır. İki yolun ayrılması, testte bulunan bir adın gerçekte
    /// ikinci kez oluşmasını engelliyor.
    /// </remarks>
    private async Task<Counterparty?> FindByNameAsync(
        Guid userId,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Counterparties.Where(counterparty => counterparty.UserId == userId);

        if (string.Equals(
                dbContext.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal))
        {
            var candidates = await query.ToArrayAsync(cancellationToken);
            return candidates.SingleOrDefault(counterparty =>
                string.Equals(counterparty.Name, normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        return await query.SingleOrDefaultAsync(
            counterparty => counterparty.Name == normalizedName,
            cancellationToken);
    }

    public async Task<Counterparty?> FindOwnedByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken)
    {
        var normalized = name?.Trim() ?? string.Empty;
        return normalized.Length == 0
            ? null
            : await FindByNameAsync(userId, normalized, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        Guid? exceptCounterpartyId,
        CancellationToken cancellationToken)
    {
        var existing = await FindByNameAsync(
            userId, normalizedName?.Trim() ?? string.Empty, cancellationToken);
        return existing is not null && existing.Id != exceptCounterpartyId;
    }

    public async Task AddAsync(Counterparty counterparty, CancellationToken cancellationToken)
    {
        await dbContext.Counterparties.AddAsync(counterparty, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOwnedAsync(
        Counterparty counterparty,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (counterparty.UserId != userId)
        {
            throw new InvalidOperationException("Owned counterparty was not found.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <remarks>
    /// Hareket araması sözleşmeleri de kapsıyor: taksitli bir borcu olan karşı
    /// tarafın silinmesi, sözleşmeyi adsız bırakırdı.
    /// </remarks>
    public async Task<bool> DeleteIfWithoutHistoryAsync(
        Guid counterpartyId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var hasHistory =
            await dbContext.CounterpartyCharges.AnyAsync(
                charge => charge.UserId == userId && charge.CounterpartyId == counterpartyId,
                cancellationToken) ||
            await dbContext.CounterpartyPayments.AnyAsync(
                payment => payment.UserId == userId && payment.CounterpartyId == counterpartyId,
                cancellationToken) ||
            await dbContext.DebtAgreements.AnyAsync(
                debt => debt.UserId == userId && debt.CounterpartyId == counterpartyId,
                cancellationToken) ||
            await dbContext.Obligations.AnyAsync(
                obligation => obligation.UserId == userId &&
                              obligation.CounterpartyId == counterpartyId,
                cancellationToken);

        if (hasHistory)
        {
            return false;
        }

        var counterparty = await dbContext.Counterparties.SingleOrDefaultAsync(
            item => item.Id == counterpartyId && item.UserId == userId,
            cancellationToken);
        if (counterparty is null)
        {
            return false;
        }

        dbContext.Counterparties.Remove(counterparty);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddChargeAsync(
        CounterpartyCharge charge,
        CancellationToken cancellationToken)
    {
        await dbContext.CounterpartyCharges.AddAsync(charge, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPaymentAsync(
        CounterpartyPayment payment,
        CancellationToken cancellationToken)
    {
        await dbContext.CounterpartyPayments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CounterpartyCharge?> FindOwnedChargeAsync(
        Guid chargeId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.CounterpartyCharges.SingleOrDefaultAsync(
            charge => charge.Id == chargeId && charge.UserId == userId,
            cancellationToken);
    }

    public Task<CounterpartyPayment?> FindOwnedPaymentAsync(
        Guid paymentId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.CounterpartyPayments.SingleOrDefaultAsync(
            payment => payment.Id == paymentId && payment.UserId == userId,
            cancellationToken);
    }

    public Task SaveChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SavePaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
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
    ///
    /// Ödemeler tek bir borçlandırmaya bağlanmadığı için gecikme dağıtımında
    /// önce vadesi geçmiş hareketleri kapatır. Bu nedenle gecikmiş tutar,
    /// gecikmiş borçlandırmaların toplamından o yöndeki bütün ödemelerin
    /// düşülmesiyle ve sıfırın altına kırpılarak hesaplanır.
    /// </remarks>
    private IQueryable<BalanceRow> ProjectBalances(Guid userId, DateOnly asOfDate)
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
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m) +
                    (dbContext.Obligations
                        .Where(obligation => obligation.UserId == userId &&
                                             obligation.CounterpartyId == counterparty.Id &&
                                             !obligation.IsCancelled &&
                                             obligation.Settlement == null &&
                                             obligation.Direction == DebtDirection.Receivable)
                        .Sum(obligation => (decimal?)obligation.Amount.Amount) ?? 0m),
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
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m) +
                    (dbContext.Obligations
                        .Where(obligation => obligation.UserId == userId &&
                                             obligation.CounterpartyId == counterparty.Id &&
                                             !obligation.IsCancelled &&
                                             obligation.Settlement == null &&
                                             obligation.Direction == DebtDirection.Payable)
                        .Sum(obligation => (decimal?)obligation.Amount.Amount) ?? 0m),
                OverdueReceivableCharges =
                    dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Receivable &&
                                         charge.DueDate != null &&
                                         charge.DueDate < asOfDate)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m,
                OverduePayableCharges =
                    dbContext.CounterpartyCharges
                        .Where(charge => charge.UserId == userId &&
                                         charge.CounterpartyId == counterparty.Id &&
                                         !charge.IsCancelled &&
                                         charge.Direction == DebtDirection.Payable &&
                                         charge.DueDate != null &&
                                         charge.DueDate < asOfDate)
                        .Sum(charge => (decimal?)charge.Amount.Amount) ?? 0m,
                ReceivablePayments =
                    dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Receivable)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m,
                PayablePayments =
                    dbContext.CounterpartyPayments
                        .Where(payment => payment.UserId == userId &&
                                          payment.CounterpartyId == counterparty.Id &&
                                          !payment.IsCancelled &&
                                          payment.Direction == DebtDirection.Payable)
                        .Sum(payment => (decimal?)payment.Amount.Amount) ?? 0m,
                OverdueReceivableObligations =
                    dbContext.Obligations
                        .Where(obligation => obligation.UserId == userId &&
                                             obligation.CounterpartyId == counterparty.Id &&
                                             !obligation.IsCancelled &&
                                             obligation.Settlement == null &&
                                             obligation.Direction == DebtDirection.Receivable &&
                                             obligation.DueDate < asOfDate)
                        .Sum(obligation => (decimal?)obligation.Amount.Amount) ?? 0m,
                OverduePayableObligations =
                    dbContext.Obligations
                        .Where(obligation => obligation.UserId == userId &&
                                             obligation.CounterpartyId == counterparty.Id &&
                                             !obligation.IsCancelled &&
                                             obligation.Settlement == null &&
                                             obligation.Direction == DebtDirection.Payable &&
                                             obligation.DueDate < asOfDate)
                        .Sum(obligation => (decimal?)obligation.Amount.Amount) ?? 0m
            });
    }

    private sealed class BalanceRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public decimal Receivable { get; init; }
        public decimal Payable { get; init; }
        public decimal OverdueReceivableCharges { get; init; }
        public decimal OverduePayableCharges { get; init; }
        public decimal ReceivablePayments { get; init; }
        public decimal PayablePayments { get; init; }
        public decimal OverdueReceivableObligations { get; init; }
        public decimal OverduePayableObligations { get; init; }
    }
}
