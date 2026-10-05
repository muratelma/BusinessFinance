using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Pos;

internal sealed class EfPosSettlementRepository(
    BusinessFinanceDbContext dbContext,
    TimeProvider timeProvider)
    : IPosSettlementRepository
{
    public async Task AddAsync(PosSettlement settlement, CancellationToken cancellationToken)
    {
        await dbContext.PosSettlements.AddAsync(settlement, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PosSettlementListDto> ListAsync(
        Guid userId,
        PosSettlementListCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query =
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CategoryId }
                equals new { category.UserId, category.Id }
            join commissionCategory in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CommissionCategoryId }
                equals new { commissionCategory.UserId, Id = (Guid?)commissionCategory.Id }
                into commissionCategories
            from commissionCategory in commissionCategories.DefaultIfEmpty()
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { settlement.UserId, Id = settlement.PosDefinitionId }
                equals new { definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
            where settlement.UserId == userId &&
                  !settlement.IsCancelled &&
                  settlement.SettlementDate >= criteria.From &&
                  settlement.SettlementDate <= criteria.To
            select new
            {
                settlement,
                AccountName = account.Name,
                CategoryName = category.Name,
                CommissionCategoryName = commissionCategory == null
                    ? null
                    : commissionCategory.Name,
                DefinitionName = definition == null ? null : definition.Name,
                // Tahsilatı sayan gün sonu: bağ tutar taşımaz, yalnız kimliktir.
                CountedInDayCloseId = dbContext.DayCloseCountedRecords
                    .Where(counted => counted.UserId == userId &&
                                      counted.Kind == DayCloseRecordKind.PosSettlement &&
                                      counted.RecordId == settlement.Id)
                    .Select(counted => (Guid?)counted.DayCloseId)
                    .FirstOrDefault(),
            };

        if (criteria.InTransitOnly)
        {
            query = query.Where(row => row.settlement.TransferredOn == null);
        }

        var rows = await query
            .OrderByDescending(row => row.settlement.SettlementDate)
            .ThenByDescending(row => row.settlement.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        // Yoldaki toplam pencereden bağımsız okunur ve **net** toplanır: hesaba
        // geçecek olan odur (ADR 0015). Tek gruplanmış sorgu; tahsilat adediyle
        // büyümez.
        var transit = await dbContext.PosSettlements
            .AsNoTracking()
            .Where(settlement => settlement.UserId == userId &&
                                 !settlement.IsCancelled &&
                                 settlement.TransferredOn == null)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Amount = group.Sum(settlement =>
                    settlement.GrossAmount.Amount - settlement.CommissionAmount),
                Count = group.Count(),
            })
            .SingleOrDefaultAsync(cancellationToken);

        var asOfDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        return new PosSettlementListDto(
            rows
                .Select(row => new PosSettlementDto(
                    row.settlement.Id,
                    row.settlement.AccountId,
                    row.AccountName,
                    row.settlement.CategoryId,
                    row.CategoryName,
                    row.settlement.CommissionCategoryId,
                    row.CommissionCategoryName,
                    row.settlement.GrossAmount.Amount,
                    row.settlement.CommissionAmount,
                    row.settlement.NetAmount.Amount,
                    row.settlement.CommissionRate,
                    row.settlement.Currency,
                    row.settlement.Scope,
                    row.settlement.SettlementDate,
                    row.settlement.ExpectedTransferDate,
                    row.settlement.TransferredOn,
                    row.settlement.Description,
                    row.settlement.IsInTransit,
                    row.settlement.IsCancelled,
                    row.settlement.IsInTransit &&
                        row.settlement.ExpectedTransferDate < asOfDate,
                    row.settlement.PosDefinitionId,
                    row.DefinitionName,
                    row.settlement.PosDepositId,
                    row.settlement.DayCloseId,
                    row.CountedInDayCloseId))
                .ToArray(),
            transit?.Amount ?? 0m,
            transit?.Count ?? 0);
    }

    public Task<PosSettlement?> FindOwnedByIdAsync(
        Guid settlementId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.PosSettlements : dbContext.PosSettlements.AsNoTracking())
            .SingleOrDefaultAsync(
                settlement => settlement.Id == settlementId && settlement.UserId == userId,
                cancellationToken);

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Aynı anda bir yatış bu tahsilatı kapattı ya da bıraktı; kaybeden
            // yazma hiçbir şey değiştirmez ve çakışma olarak döner.
            dbContext.ChangeTracker.Clear();
            throw new InvalidOperationException(
                "The pos settlement changed while it was being saved.");
        }
    }
}
