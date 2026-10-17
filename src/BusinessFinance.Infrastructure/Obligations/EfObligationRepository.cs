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

    // Kapanış: faturayı kapatan son ödeme; iptal edilmiş faturada iptal edilen
    // son ödeme. Kısmen ödenmiş fatura açıktır.
    public async Task<IReadOnlyList<ObligationDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken) => await (
            from obligation in dbContext.Obligations
            // Eski liste kategorili faturayı yazar; devredilmiş borç yeni
            // listededir (`debt-book`) ve bu uç Flutter geçişinde kalkar.
            join category in dbContext.Categories.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CategoryId }
                equals new { category.UserId, Id = (Guid?)category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            let closed = obligation.IsCancelled || obligation.RemainingAmount == 0m
            let settlement = dbContext.DebtPayments
                .Where(payment => payment.UserId == obligation.UserId &&
                                  payment.DebtId == obligation.Id &&
                                  payment.Kind == DebtPaymentKind.Payment &&
                                  (obligation.IsCancelled || !payment.IsCancelled))
                .OrderByDescending(payment => payment.PaidAtUtc)
                .ThenByDescending(payment => payment.Id)
                .FirstOrDefault()
            where obligation.UserId == userId
            orderby obligation.DueDate, obligation.Id
            select new ObligationDto(
                obligation.Id,
                obligation.CounterpartyId,
                category.Id,
                obligation.Direction,
                obligation.Amount,
                obligation.Currency,
                obligation.Scope,
                obligation.IssueDate,
                obligation.DueDate,
                obligation.Description,
                obligation.IsCancelled
                    ? ObligationStatus.Cancelled
                    : obligation.RemainingAmount == 0m
                        ? ObligationStatus.Settled
                        : ObligationStatus.Open,
                counterparty == null ? null : counterparty.Name,
                category.Name,
                !obligation.IsCancelled && obligation.RemainingAmount > 0m &&
                    obligation.DueDate < asOfDate,
                !closed || settlement == null ? null : settlement.Id,
                !closed || settlement == null ? null : settlement.AccountId,
                !closed || settlement == null ? null : settlement.PaymentDate))
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
