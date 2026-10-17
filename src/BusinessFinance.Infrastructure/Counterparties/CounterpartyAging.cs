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
    public sealed record Aging(decimal Overdue, DateOnly? NextDueDate, decimal NextDueAmount);

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
        var charges = await dbContext.CounterpartyCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId &&
                             !charge.IsCancelled &&
                             ids.Contains(charge.CounterpartyId))
            .Select(charge => new ChargeRow(
                charge.CounterpartyId, charge.Direction, charge.ChargeDate,
                EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),
                charge.Id,
                charge.Amount.Amount -
                    (dbContext.CounterpartyChargeDiscounts
                        .Where(discount => discount.UserId == userId &&
                                           discount.ChargeId == charge.Id &&
                                           !discount.IsCancelled)
                        .Sum(discount => (decimal?)discount.Amount) ?? 0m),
                charge.DueDate))
            .ToArrayAsync(cancellationToken);

        var result = new Dictionary<(Guid, DebtDirection), Aging>();
        foreach (var group in charges.GroupBy(charge => (charge.CounterpartyId, charge.Direction)))
        {
            // Devreden bakiye en eski kalemdir: ödemeler önce onu kapatır.
            var unpaid = payments.GetValueOrDefault(group.Key) - openings.GetValueOrDefault(group.Key);
            var overdue = 0m;
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

            result[group.Key] = new Aging(overdue, nextDue, nextAmount);
        }

        return result;
    }
}
