using BusinessFinance.Application.Obligations;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Debts;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Obligations;

/// <summary>
/// Tek ödemeli borçların deposu: eski yükümlülük uçları bu planı okur ve yazar
/// (ADR 0021 dalga 1).
/// </summary>
internal sealed class EfObligationRepository(BusinessFinanceDbContext dbContext)
    : IObligationRepository
{
    public async Task AddAsync(Debt obligation, CancellationToken cancellationToken)
    {
        await dbContext.Debts.AddAsync(obligation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ObligationDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken) => await (
            from obligation in dbContext.Debts.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CategoryId }
                equals new { category.UserId, Id = (Guid?)category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            let settlement = obligation.Payments.FirstOrDefault()
            where obligation.UserId == userId && obligation.PlanKind == DebtPlanKind.Single
            orderby obligation.FirstDueDate, obligation.Id
            select new ObligationDto(
                obligation.Id,
                obligation.CounterpartyId,
                obligation.CategoryId!.Value,
                obligation.Direction,
                obligation.Principal.Amount,
                obligation.Principal.Currency,
                obligation.Scope,
                obligation.StartDate,
                obligation.FirstDueDate!.Value,
                obligation.Description,
                obligation.IsCancelled
                    ? ObligationStatus.Cancelled
                    : settlement == null
                        ? ObligationStatus.Open
                        : ObligationStatus.Settled,
                counterparty == null ? null : counterparty.Name,
                category.Name,
                !obligation.IsCancelled && settlement == null &&
                    obligation.FirstDueDate < asOfDate,
                settlement == null ? null : settlement.Id,
                settlement == null ? null : settlement.AccountId,
                settlement == null ? null : settlement.PaymentDate))
            .ToArrayAsync(cancellationToken);

    public Task<Debt?> FindOwnedByIdAsync(
        Guid obligationId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        EfDebtRepository.Aggregates(track ? dbContext.Debts : dbContext.Debts.AsNoTracking())
            .SingleOrDefaultAsync(
                obligation => obligation.Id == obligationId && obligation.UserId == userId &&
                              obligation.PlanKind == DebtPlanKind.Single,
                cancellationToken);

    public Task<PosSettlement?> FindCardSettlementAsync(
        Guid settlementId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.PosSettlements.SingleOrDefaultAsync(
            settlement => settlement.Id == settlementId && settlement.UserId == userId,
            cancellationToken);

    public async Task<bool> TrySaveCancellationAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Kartla tahsilin POS kaydı bu sırada bir yatışa bağlandı; kaybeden
            // yazma hiçbir şey değiştirmez.
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task SaveSettlementAsync(
        DebtPayment? newPayment,
        PosSettlement? cardSettlement,
        CancellationToken cancellationToken)
    {
        if (cardSettlement is not null)
        {
            await dbContext.PosSettlements.AddAsync(cardSettlement, cancellationToken);
        }

        if (newPayment is not null)
        {
            DebtPayments.MarkAdded(dbContext, newPayment);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception is DbUpdateConcurrencyException ||
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı borç bu sırada başka bir istekle değişti: sürümü ya da ödemenin
            // kimliği çarptı.
            dbContext.ChangeTracker.Clear();
            throw new ObligationConcurrencyException();
        }
    }
}
