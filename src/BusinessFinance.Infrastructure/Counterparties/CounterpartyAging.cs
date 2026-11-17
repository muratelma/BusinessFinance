using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Counterparties;

/// <summary>
/// Carinin gecikmiş tutarı ve sıradaki vadesi (ADR 0021 T8, CB23).
/// </summary>
/// <remarks>
/// Ödemeler önce <b>en eski alımı</b> düşürür (alım günü, sonra giriş anı).
/// Gecikmiş tutar, vadesi geçmiş alımların bu sırayla kapanmamış kısmıdır;
/// sıradaki vade, kapanmamış kısmı kalan ve vadesi gelmemiş en yakın alımdır.
/// Eski hesap bütün ödemeleri önce vadeli alımdan düşüyordu: vadesiz eski bir
/// alıma yapılan ödeme vadeli alımın gecikmesini de siliyordu. Fazla ödeme
/// kırpılmaz; burada yalnız alımları sıfıra indirir.
/// </remarks>
internal static class CounterpartyAging
{
    /// <param name="OldestOverdueDate">
    /// Kapanmamış kısmı kalan, vadesi geçmiş en eski kalemin vadesi; listede
    /// gecikenler bununla sıralanır (en eski üstte).
    /// </param>
    public sealed record Aging(
        decimal Overdue, DateOnly? NextDueDate, decimal NextDueAmount, DateOnly? OldestOverdueDate = null);

    private sealed record ChargeRow(
        Guid CounterpartyId, DebtDirection Direction, DateOnly Date, DateTimeOffset? EntryAt,
        Guid Id, decimal Amount, DateOnly? DueDate);

    public static async Task<Dictionary<(Guid CounterpartyId, DebtDirection Direction), Aging>> ComputeAsync(
        BusinessFinanceDbContext dbContext,
        Guid userId,
        IReadOnlyCollection<Guid> counterpartyIds,
        IReadOnlyDictionary<(Guid CounterpartyId, DebtDirection Direction), decimal> payments,
        IReadOnlyDictionary<(Guid CounterpartyId, DebtDirection Direction), decimal> openings,
        DateOnly asOfDate,
        CancellationToken cancellationToken)
    {
        if (counterpartyIds.Count == 0)
        {
            return [];
        }

        var ids = counterpartyIds.ToArray();
        // Borç para da açık bir kalemdir: vadesi varsa gecikir. İkisi tek
        // sorguda (UNION ALL) okunur; kişi listesinin sorgu sayısı kayıt türüyle
        // büyümez. EF kurucudan sonra birleşim çeviremediği için satır bellekte
        // kurulur.
        var rows = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             ids.Contains(charge.CounterpartyId))
            .Select(charge => new
            {
                charge.CounterpartyId,
                charge.Direction,
                Date = charge.ChargeDate,
                EntryAt = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),
                charge.Id,
                Amount = charge.Amount.Amount -
                    (dbContext.CounterpartyChargeDiscounts
                        .Where(discount => discount.UserId == userId &&
                                           discount.ChargeId == charge.Id &&
                                           !discount.IsCancelled)
                        .Sum(discount => (decimal?)discount.Amount) ?? 0m),
                charge.DueDate
            })
            .Concat(dbContext.CounterpartyLoans.AsNoTracking()
                .Where(loan => loan.UserId == userId &&
                               !loan.IsCancelled &&
                               ids.Contains(loan.CounterpartyId))
                .Select(loan => new
                {
                    loan.CounterpartyId,
                    loan.Direction,
                    Date = loan.LoanDate,
                    EntryAt = EF.Property<DateTimeOffset?>(loan, EntryTimestamp.PropertyName),
                    loan.Id,
                    Amount = loan.Amount.Amount,
                    loan.DueDate
                }))
            .ToArrayAsync(cancellationToken);
        var charges = rows.Select(row => new ChargeRow(
            row.CounterpartyId, row.Direction, row.Date, row.EntryAt, row.Id, row.Amount, row.DueDate));

        var result = new Dictionary<(Guid, DebtDirection), Aging>();
        foreach (var group in charges.GroupBy(charge => (charge.CounterpartyId, charge.Direction)))
        {
            // Devreden bakiye en eski kalemdir: ödemeler önce onu kapatır.
            var unpaid = payments.GetValueOrDefault(group.Key) - openings.GetValueOrDefault(group.Key);
            var overdue = 0m;
            DateOnly? oldestOverdue = null;
            DateOnly? nextDue = null;
            var nextAmount = 0m;
            foreach (var charge in group
                         .OrderBy(charge => charge.Date)
                         .ThenBy(charge => charge.EntryAt ?? DateTimeOffset.MinValue)
                         .ThenBy(charge => charge.Id))
            {
                var covered = Math.Min(Math.Max(unpaid, 0m), charge.Amount);
                unpaid -= covered;
                var open = charge.Amount - covered;
                if (open <= 0m || charge.DueDate is not DateOnly due)
                {
                    continue;
                }

                if (due < asOfDate)
                {
                    overdue += open;
                    if (oldestOverdue is null || due < oldestOverdue)
                    {
                        oldestOverdue = due;
                    }
                }
                else if (nextDue is null || due < nextDue)
                {
                    nextDue = due;
                    nextAmount = open;
                }
                else if (due == nextDue)
                {
                    nextAmount += open;
                }
            }

            result[group.Key] = new Aging(overdue, nextDue, nextAmount, oldestOverdue);
        }

        return result;
    }

    /// <summary>Kişinin hesabında vadesi olan, kapanmamış bir kalemin açık kısmı.</summary>
    public sealed record OpenDue(Guid CounterpartyId, DebtDirection Direction, DateOnly DueDate, decimal Amount);

    /// <summary>
    /// Bütün kişilerin hesabındaki vadeli açık kalemler: gecikmeyle aynı sıra
    /// (devreden bakiye en eski; sonra gün, giriş anı), ödemeler ve vazgeçmeler
    /// en eskiden düşülür. Planlananlar ve hatırlatmalar bunu okur (S11).
    /// Sorgu sayısı kişi sayısına bağlı değildir.
    /// </summary>
    public static async Task<IReadOnlyList<OpenDue>> ListOpenDuesAsync(
        BusinessFinanceDbContext dbContext,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var paid = await dbContext.CounterpartyPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId && !payment.IsCancelled)
            .Select(payment => new { payment.CounterpartyId, payment.Direction, Amount = payment.Amount.Amount })
            .Concat(dbContext.CounterpartyWriteOffs.AsNoTracking()
                .Where(writeOff => writeOff.UserId == userId && !writeOff.IsCancelled)
                .Select(writeOff => new
                {
                    writeOff.CounterpartyId,
                    writeOff.Direction,
                    Amount = writeOff.Amount.Amount
                }))
            .GroupBy(row => new { row.CounterpartyId, row.Direction })
            .Select(group => new { group.Key.CounterpartyId, group.Key.Direction, Total = group.Sum(row => row.Amount) })
            .ToArrayAsync(cancellationToken);
        var openings = await dbContext.Counterparties.AsNoTracking()
            .Where(counterparty => counterparty.UserId == userId &&
                                   (counterparty.OpeningReceivable > 0m || counterparty.OpeningPayable > 0m))
            .Select(counterparty => new
            {
                counterparty.Id,
                counterparty.OpeningReceivable,
                counterparty.OpeningPayable
            })
            .ToArrayAsync(cancellationToken);
        var rows = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId && !charge.IsCancelled)
            .Select(charge => new
            {
                charge.CounterpartyId,
                charge.Direction,
                Date = charge.ChargeDate,
                EntryAt = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),
                charge.Id,
                Amount = charge.Amount.Amount -
                    (dbContext.CounterpartyChargeDiscounts
                        .Where(discount => discount.UserId == userId &&
                                           discount.ChargeId == charge.Id &&
                                           !discount.IsCancelled)
                        .Sum(discount => (decimal?)discount.Amount) ?? 0m),
                charge.DueDate
            })
            .Concat(dbContext.CounterpartyLoans.AsNoTracking()
                .Where(loan => loan.UserId == userId && !loan.IsCancelled)
                .Select(loan => new
                {
                    loan.CounterpartyId,
                    loan.Direction,
                    Date = loan.LoanDate,
                    EntryAt = EF.Property<DateTimeOffset?>(loan, EntryTimestamp.PropertyName),
                    loan.Id,
                    Amount = loan.Amount.Amount,
                    loan.DueDate
                }))
            .ToArrayAsync(cancellationToken);

        var paidBy = paid.ToDictionary(row => (row.CounterpartyId, row.Direction), row => row.Total);
        var openingBy = new Dictionary<(Guid, DebtDirection), decimal>();
        foreach (var opening in openings)
        {
            openingBy[(opening.Id, DebtDirection.Receivable)] = opening.OpeningReceivable;
            openingBy[(opening.Id, DebtDirection.Payable)] = opening.OpeningPayable;
        }

        var result = new List<OpenDue>();
        foreach (var group in rows.GroupBy(row => (row.CounterpartyId, row.Direction)))
        {
            var unpaid = paidBy.GetValueOrDefault(group.Key) - openingBy.GetValueOrDefault(group.Key);
            foreach (var row in group
                         .OrderBy(row => row.Date)
                         .ThenBy(row => row.EntryAt ?? DateTimeOffset.MinValue)
                         .ThenBy(row => row.Id))
            {
                var covered = Math.Min(Math.Max(unpaid, 0m), row.Amount);
                unpaid -= covered;
                var open = row.Amount - covered;
                if (open > 0m && row.DueDate is DateOnly due)
                {
                    result.Add(new OpenDue(group.Key.CounterpartyId, group.Key.Direction, due, open));
                }
            }
        }

        return result;
    }
}
