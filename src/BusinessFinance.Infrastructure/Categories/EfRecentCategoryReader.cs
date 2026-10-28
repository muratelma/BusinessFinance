using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Infrastructure.Reports;

namespace BusinessFinance.Infrastructure.Categories;

/// <summary>
/// Son kullanılan kategorileri gelir ve gider yazan kayıtların tek listesinden
/// (<see cref="RecognizedItems"/>) okur.
/// </summary>
/// <remarks>
/// Ayrı bir kaynak listesi yazılmaz: kategori taşıyan yeni bir tanıma yolu
/// <see cref="RecognizedItems"/>'a eklendiğinde burada da kendiliğinden
/// görünür. Bir kaydın parçası olan kalemler (<c>Part</c> dolu: transfer
/// masrafı, yatış kesintisi, POS komisyonu, borç faizi) sayılmaz; onları
/// kullanıcı formda seçmedi. Sıra giriş anıdır; giriş anı bilinmeyen eski
/// kayıt sona düşer ve kendi aralarında güne göre dizilir.
/// </remarks>
internal sealed class EfRecentCategoryReader(BusinessFinanceDbContext dbContext)
    : IRecentCategoryReader
{
    public async Task<IReadOnlyList<RecentCategoryDto>> ListAsync(
        Guid userId,
        CategoryType type,
        int take,
        CancellationToken cancellationToken)
    {
        var recognizedType = (int)(type == CategoryType.Income
            ? TransactionType.Income
            : TransactionType.Expense);
        var activeCategories = dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId && category.IsActive && category.Type == type)
            .Select(category => category.Id);

        var rows = await RecognizedItems.Query(dbContext, userId)
            .Where(item => item.Part == null &&
                           item.Type == recognizedType &&
                           item.CategoryId != null &&
                           activeCategories.Contains(item.CategoryId.Value))
            .GroupBy(item => new { item.CategoryId, item.Scope })
            .Select(group => new
            {
                group.Key.CategoryId,
                group.Key.Scope,
                LastEntry = group.Max(item => item.EntryAtUtc),
                LastDate = group.Max(item => item.Date)
            })
            .OrderByDescending(row => row.LastEntry)
            .ThenByDescending(row => row.LastDate)
            .Take(take)
            .ToArrayAsync(cancellationToken);

        return rows
            .Select(row => new RecentCategoryDto(row.CategoryId!.Value, (TransactionScope)row.Scope))
            .ToArray();
    }
}
