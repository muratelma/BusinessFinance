using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Categories;

/// <summary>
/// Kategoriyi taşıyan kayıt türlerini sırayla sorar ve ilk bulduğunda durur.
/// </summary>
/// <remarks>
/// Sorgular ayrı ayrı gider: kategorinin tarafını değiştirmek seyrek bir
/// eylemdir ve tek bir birleşik sorgu, yeni bir kayıt türü eklendiğinde
/// okunması zor bir yere dönüşürdü. Kategori taşıyan yeni bir kayıt türü
/// <b>buraya</b> eklenir; aksi hâlde o türdeki kayıt kategorinin tarafıyla
/// çelişir hâle gelebilir.
/// </remarks>
internal sealed class EfCategoryUsageReader(BusinessFinanceDbContext dbContext)
    : ICategoryUsageReader
{
    public async Task<bool> IsUsedOutsideAsync(
        Guid categoryId,
        Guid userId,
        TransactionScope side,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Transactions.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        !item.IsCancelled && item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.CreditCardCharges.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        !item.IsCancelled && item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.MonthlyBudgets.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.InstallmentPlans.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.RecurringTransactions.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.DebtAgreements.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.CounterpartyCharges.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        !item.IsCancelled && item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        if (await dbContext.Obligations.AsNoTracking().AnyAsync(
                item => item.UserId == userId && item.CategoryId == categoryId &&
                        !item.IsCancelled && item.Scope != side,
                cancellationToken))
        {
            return true;
        }

        // POS satışı ve komisyonu işletmenindir: POS'ta kullanılan kategori
        // şahsiye çevrilemez. Eski kuralla şahsi yazılmış bir tahsilat da
        // işletmeye daraltmayı engeller.
        if (await dbContext.PosSettlements.AsNoTracking().AnyAsync(
                item => item.UserId == userId && !item.IsCancelled &&
                        (item.CategoryId == categoryId ||
                         item.CommissionCategoryId == categoryId) &&
                        (side == TransactionScope.Personal ||
                         (item.Scope != null && item.Scope != side)),
                cancellationToken))
        {
            return true;
        }

        return side == TransactionScope.Personal &&
            await dbContext.PosDefinitions.AsNoTracking().AnyAsync(
                item => item.UserId == userId &&
                        (item.SalesCategoryId == categoryId ||
                         item.CommissionCategoryId == categoryId),
                cancellationToken);
    }
}
