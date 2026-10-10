using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Pos;

internal sealed class EfPosDefinitionRepository(BusinessFinanceDbContext dbContext)
    : IPosDefinitionRepository
{
    public async Task AddAsync(PosDefinition definition, CancellationToken cancellationToken)
    {
        await dbContext.PosDefinitions.AddAsync(definition, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        Guid userId,
        string name,
        Guid? exceptDefinitionId,
        CancellationToken cancellationToken)
    {
        var key = NameKeys.Of(name);
        return dbContext.PosDefinitions.AsNoTracking().AnyAsync(
            definition => definition.UserId == userId &&
                          definition.Id != exceptDefinitionId &&
                          definition.NameKey == key,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PosDefinitionDto>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Tek sorgu: adlar sahiplik kapsamındaki birleşimlerle okunur; tanım
        // adediyle büyüyen ikinci bir sorgu yoktur.
        var query =
            from definition in dbContext.PosDefinitions.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { definition.UserId, Id = definition.AccountId }
                equals new { account.UserId, account.Id }
            join salesCategory in dbContext.Categories.AsNoTracking()
                on new { definition.UserId, Id = definition.SalesCategoryId }
                equals new { salesCategory.UserId, salesCategory.Id }
            join commissionCategory in dbContext.Categories.AsNoTracking()
                on new { definition.UserId, Id = definition.CommissionCategoryId }
                equals new { commissionCategory.UserId, Id = (Guid?)commissionCategory.Id }
                into commissionCategories
            from commissionCategory in commissionCategories.DefaultIfEmpty()
            where definition.UserId == userId
            orderby definition.IsDefault descending, definition.IsActive descending,
                definition.Name, definition.Id
            select new PosDefinitionDto(
                definition.Id,
                definition.Name,
                definition.AccountId,
                account.Name,
                definition.SalesCategoryId,
                salesCategory.Name,
                definition.CommissionCategoryId,
                commissionCategory == null ? null : commissionCategory.Name,
                definition.CommissionRate,
                definition.TransferDays,
                definition.BusinessDaysOnly,
                definition.IsActive,
                definition.IsDefault);

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task<bool> HasDefaultAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.PosDefinitions.AsNoTracking().AnyAsync(
            definition => definition.UserId == userId && definition.IsDefault,
            cancellationToken);

    public async Task<IReadOnlyList<PosDefinition>> FindOtherDefaultsAsync(
        Guid userId,
        Guid exceptDefinitionId,
        CancellationToken cancellationToken) =>
        await dbContext.PosDefinitions
            .Where(definition => definition.UserId == userId &&
                                 definition.IsDefault &&
                                 definition.Id != exceptDefinitionId)
            .ToArrayAsync(cancellationToken);

    public Task<PosDefinition?> FindOwnedByIdAsync(
        Guid definitionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.PosDefinitions : dbContext.PosDefinitions.AsNoTracking())
            .SingleOrDefaultAsync(
                definition => definition.Id == definitionId && definition.UserId == userId,
                cancellationToken);

    public Task<bool> HasSettlementsAsync(
        Guid definitionId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.PosSettlements.AsNoTracking().AnyAsync(
            settlement => settlement.UserId == userId &&
                          settlement.PosDefinitionId == definitionId,
            cancellationToken);

    public async Task RemoveAsync(PosDefinition definition, CancellationToken cancellationToken)
    {
        dbContext.PosDefinitions.Remove(definition);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
