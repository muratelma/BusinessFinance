using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Debts;

/// <summary>
/// Taksitli borçların deposu. Tek ödemeli borçlar aynı tablodadır ama yükümlülük
/// uçlarından okunur (ADR 0021 dalga 1: ekranlar değişmez).
/// </summary>
internal sealed class EfDebtRepository(BusinessFinanceDbContext dbContext) : IDebtRepository
{
    public async Task AddAsync(Debt debt, CancellationToken cancellationToken)
    {
        dbContext.Debts.Add(debt);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Debt>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        await Aggregates(dbContext.Debts.AsNoTracking())
            .Where(debt => debt.UserId == userId && debt.PlanKind == DebtPlanKind.Installments)
            .OrderBy(debt => debt.FirstDueDate).ThenBy(debt => debt.Id)
            .ToArrayAsync(cancellationToken);

    public Task<Debt?> FindOwnedByIdAsync(
        Guid id, Guid userId, bool track, CancellationToken cancellationToken) =>
        Aggregates(track ? dbContext.Debts : dbContext.Debts.AsNoTracking())
            .SingleOrDefaultAsync(
                debt => debt.Id == id && debt.UserId == userId &&
                        debt.PlanKind == DebtPlanKind.Installments,
                cancellationToken);

    // Açılış tamamlanırken kalemlerin anapara/faiz ayrımı da doluyor, yani
    // aggregate'in tamamı tek `SaveChanges` sınırında yazılır.
    public async Task SaveOpeningAsync(Debt debt, CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public async Task SavePaymentAsync(DebtPayment payment, CancellationToken cancellationToken)
    {
        DebtPayments.MarkAdded(dbContext, payment);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception is DbUpdateConcurrencyException ||
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı taksit bu sırada başka bir istekle ödendi: tahsisin tekliği
            // ya da ödemenin kimliği (taksidinkidir) çarptı.
            dbContext.ChangeTracker.Clear();
            throw new DebtConcurrencyException();
        }
    }

    internal static IQueryable<Debt> Aggregates(IQueryable<Debt> debts) => debts
        .Include(debt => debt.Items)
        .Include(debt => debt.Payments).ThenInclude(payment => payment.Allocations)
        .AsSplitQuery();
}

/// <summary>Yeni ödemenin izlenme durumu.</summary>
internal static class DebtPayments
{
    /// <summary>
    /// Kimliği önceden atanmış yeni ödeme, izlenen borcun koleksiyonundan
    /// bulunduğunda "değişmiş" sanılabilir; açıkça eklenir.
    /// </summary>
    /// <remarks>
    /// Grafla eklenir: tek tek durum atamak tutarın sahiplenilmiş parçasını
    /// (<see cref="Money"/>) atlıyor ve ödeme tutarsız yazılıyordu.
    /// </remarks>
    public static void MarkAdded(BusinessFinanceDbContext dbContext, DebtPayment payment) =>
        dbContext.DebtPayments.Add(payment);
}
