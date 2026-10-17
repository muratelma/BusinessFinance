using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Debts;

/// <summary>
/// "Borç ve alacaklar" listesinin borç satırları: kalan, gecikmiş tutar ve
/// sıradaki vade açık kalemlerden (<see cref="DebtViews.OpenItems"/>) okunur.
/// Sorgu sayısı borç sayısına bağlı değildir.
/// </summary>
internal sealed class EfDebtBookReader(BusinessFinanceDbContext dbContext) : IDebtBookReader
{
    public async Task<IReadOnlyList<DebtBookDebtRow>> ListOpenAsync(
        Guid userId, DateOnly asOfDate, CancellationToken cancellationToken)
    {
        var debts = await DebtsAsync(userId, cancellationToken);
        var open = await dbContext.DebtOpenItems
            .Where(item => item.UserId == userId)
            .Select(item => new { item.DebtId, item.DueDate, item.OpenAmount })
            .ToArrayAsync(cancellationToken);
        var closings = await ClosingsAsync(userId, cancellationToken);

        return [.. open
            .GroupBy(item => item.DebtId)
            .Where(group => debts.ContainsKey(group.Key))
            .Select(group =>
            {
                var debt = debts[group.Key];
                var upcoming = group.Where(item => item.DueDate >= asOfDate).ToArray();
                var next = upcoming.Length == 0 ? null : upcoming.Min(item => item.DueDate);
                return new DebtBookDebtRow(
                    debt.Id, debt.Direction, debt.PlanKind, debt.CounterpartyId, debt.CounterpartyName,
                    debt.Description, debt.StartDate,
                    group.Sum(item => item.OpenAmount),
                    group.Where(item => item.DueDate < asOfDate).Sum(item => item.OpenAmount),
                    next,
                    next is null ? 0m : upcoming.Where(item => item.DueDate == next).Sum(item => item.OpenAmount),
                    closings.ContainsKey(debt.Id),
                    debt.IsCarriedOver);
            })];
    }

    public async Task<IReadOnlyList<DebtBookClosedRow>> ListClosedAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        var debts = await DebtsAsync(userId, cancellationToken);
        var openIds = (await dbContext.DebtOpenItems
                .Where(item => item.UserId == userId)
                .Select(item => item.DebtId)
                .Distinct()
                .ToArrayAsync(cancellationToken))
            .ToHashSet();
        var closings = await ClosingsAsync(userId, cancellationToken);

        return [.. debts.Values
            .Where(debt => !openIds.Contains(debt.Id) && closings.ContainsKey(debt.Id))
            .Select(debt => new DebtBookClosedRow(
                debt.Id, debt.Direction, debt.PlanKind, debt.CounterpartyId, debt.CounterpartyName,
                debt.Description, closings[debt.Id].Total, closings[debt.Id].LastDate))];
    }

    private sealed record DebtHead(
        Guid Id, DebtDirection Direction, DebtPlanKind PlanKind, Guid? CounterpartyId,
        string? CounterpartyName, string? Description, DateOnly StartDate, bool IsCarriedOver);

    private sealed record ClosingTotals(decimal Total, DateOnly LastDate);

    private async Task<Dictionary<Guid, DebtHead>> DebtsAsync(Guid userId, CancellationToken cancellationToken) =>
        await (
                from debt in dbContext.Debts.AsNoTracking()
                join counterparty in dbContext.Counterparties.AsNoTracking()
                    on new { debt.UserId, Id = debt.CounterpartyId }
                    equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                    into counterparties
                from counterparty in counterparties.DefaultIfEmpty()
                where debt.UserId == userId && !debt.IsCancelled
                select new DebtHead(
                    debt.Id, debt.Direction, debt.PlanKind, debt.CounterpartyId,
                    counterparty == null ? null : counterparty.Name,
                    debt.Description, debt.StartDate, debt.IsCarriedOver))
            .ToDictionaryAsync(debt => debt.Id, cancellationToken);

    private async Task<Dictionary<Guid, ClosingTotals>> ClosingsAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.DebtPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId && !payment.IsCancelled)
            .GroupBy(payment => payment.DebtId)
            .Select(group => new
            {
                DebtId = group.Key,
                Total = group.Sum(payment => payment.Amount.Amount),
                LastDate = group.Max(payment => payment.PaymentDate)
            })
            .ToDictionaryAsync(
                row => row.DebtId,
                row => new ClosingTotals(row.Total, row.LastDate),
                cancellationToken);
}
