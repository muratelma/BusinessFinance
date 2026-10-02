using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Pos;

internal sealed class EfPosDepositRepository(
    BusinessFinanceDbContext dbContext,
    TimeProvider timeProvider)
    : IPosDepositRepository
{
    public async Task<IReadOnlyList<PosSettlement>> FindOwnedSettlementsAsync(
        IReadOnlyCollection<Guid> settlementIds,
        Guid userId,
        CancellationToken cancellationToken) =>
        await dbContext.PosSettlements
            .Where(settlement => settlement.UserId == userId &&
                                 settlementIds.Contains(settlement.Id))
            .OrderBy(settlement => settlement.SettlementDate)
            .ThenBy(settlement => settlement.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<PosSettlement>> FindSettlementsOfDepositAsync(
        Guid depositId,
        Guid userId,
        CancellationToken cancellationToken) =>
        await dbContext.PosSettlements
            .Where(settlement => settlement.UserId == userId &&
                                 settlement.PosDepositId == depositId)
            .ToArrayAsync(cancellationToken);

    public Task<PosDeposit?> FindOwnedByIdAsync(
        Guid depositId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.PosDeposits : dbContext.PosDeposits.AsNoTracking())
            .SingleOrDefaultAsync(
                deposit => deposit.Id == depositId && deposit.UserId == userId,
                cancellationToken);

    public async Task<PosDepositDto?> GetAsync(
        Guid depositId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var header = await (
            from deposit in dbContext.PosDeposits.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { deposit.UserId, Id = deposit.AccountId }
                equals new { account.UserId, account.Id }
            join transaction in dbContext.Transactions.AsNoTracking()
                on new { deposit.UserId, Id = deposit.DeductionTransactionId }
                equals new { transaction.UserId, Id = (Guid?)transaction.Id }
                into transactions
            from transaction in transactions.DefaultIfEmpty()
            join category in dbContext.Categories.AsNoTracking()
                on new { deposit.UserId, Id = (Guid?)transaction.CategoryId }
                equals new { category.UserId, Id = (Guid?)category.Id }
                into categories
            from category in categories.DefaultIfEmpty()
            where deposit.Id == depositId && deposit.UserId == userId
            select new
            {
                deposit,
                AccountName = account.Name,
                DeductionCategoryId = category == null ? (Guid?)null : category.Id,
                DeductionCategoryName = category == null ? null : category.Name,
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (header is null)
        {
            return null;
        }

        var rows = await (
            from settlement in dbContext.PosSettlements.AsNoTracking()
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
            where settlement.UserId == userId && settlement.PosDepositId == depositId
            orderby settlement.SettlementDate, settlement.CreatedAtUtc
            select new
            {
                settlement,
                CategoryName = category.Name,
                CommissionCategoryName = commissionCategory == null
                    ? null
                    : commissionCategory.Name,
                DefinitionName = definition == null ? null : definition.Name,
            })
            .ToArrayAsync(cancellationToken);

        var asOfDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        return new PosDepositDto(
            header.deposit.Id,
            header.deposit.AccountId,
            header.AccountName,
            header.deposit.DepositDate,
            header.deposit.ExpectedAmount,
            header.deposit.DepositedAmount.Amount,
            header.deposit.DeductionAmount,
            header.deposit.DeductionTransactionId,
            header.DeductionCategoryId,
            header.DeductionCategoryName,
            header.deposit.Currency,
            header.deposit.IsCancelled,
            header.deposit.CancelledAtUtc,
            rows
                .Select(row => PosSettlementMapper.ToDto(
                    row.settlement,
                    header.AccountName,
                    row.CategoryName,
                    row.CommissionCategoryName,
                    asOfDate,
                    row.DefinitionName))
                .ToArray(),
            GrossAmount: rows.Length == 0
                ? null
                : rows.Sum(row => row.settlement.GrossAmount.Amount),
            CommissionAmount: rows.Length == 0
                ? null
                : rows.Sum(row => row.settlement.CommissionAmount));
    }

    public async Task<IReadOnlyList<Guid>> ListDeductionCategoryCandidatesAsync(
        IReadOnlyCollection<Guid> settlementIds,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // POS'un bugünkü komisyon kategorisi önce gelir: kullanıcı POS'u
        // düzenlediyse kesinti yeni kategoriye yazılır. POS'u olmayan tahsilat
        // kendi komisyon kategorisini önerir.
        var candidates = await (
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { settlement.UserId, Id = settlement.PosDefinitionId }
                equals new { definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
            where settlement.UserId == userId && settlementIds.Contains(settlement.Id)
            select definition != null && definition.CommissionCategoryId != null
                ? definition.CommissionCategoryId
                : settlement.CommissionCategoryId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        return candidates.OfType<Guid>().ToArray();
    }

    public async Task<bool> TryAddAsync(
        PosDeposit deposit,
        BudgetTransaction? deductionTransaction,
        CancellationToken cancellationToken)
    {
        try
        {
            // Kapatılan tahsilatlar izleniyor; yatış ve kesinti gideriyle aynı
            // SaveChanges içinde yazılır.
            if (deductionTransaction is not null)
            {
                await dbContext.Transactions.AddAsync(deductionTransaction, cancellationToken);
            }

            await dbContext.PosDeposits.AddAsync(deposit, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Tahsilatlardan biri bu sırada başka bir yatışla kapandı ya da
            // iptal edildi.
            dbContext.ChangeTracker.Clear();
            return false;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı istek kimliği aynı yatış kimliğini üretir; eşzamanlı ikinci
            // istek birincil anahtara çarpar.
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<bool> TrySaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }
}
