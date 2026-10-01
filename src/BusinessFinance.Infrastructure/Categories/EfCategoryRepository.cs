using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Categories;

internal sealed class EfCategoryRepository(BusinessFinanceDbContext dbContext)
    : ICategoryRepository
{
    /// <summary>
    /// Borç faizinin raporlandığı kategori adı.
    /// </summary>
    /// <remarks>
    /// Faiz kalıcı bir <c>BudgetTransaction</c> üretmiyor — taksit ödemesi
    /// hesabı zaten tutarın tamamı kadar düşürüyor, ikinci bir kayıt aynı
    /// parayı iki kez düşerdi. Ama kategori dağılımında görünmesi gerekiyor,
    /// yoksa gider toplamı ile kategori listesi açıklamasız biçimde
    /// tutmuyordu. Rapor faizi bu kovaya yazıyor.
    ///
    /// Arama **kanonik ad** üzerinden: etiket çevirisi değişirse eşleme
    /// düşmemeli (ikon eşlemesinde öğrenilen aynı ders).
    /// </remarks>
    internal const string InterestExpenseCategoryName = "Faiz ve finansman gideri";

    /// <inheritdoc cref="InterestExpenseCategoryName" />
    internal const string InterestIncomeCategoryName = "Faiz geliri";

    /// <summary>Test ve seed kodunun aynı listeyi okuyabilmesi için.</summary>
    internal static IReadOnlyList<(string Name, CategoryType Type)> DefaultCategories =>
        [.. DefaultCategorySets.PersonalSet.Select(item => (item.Name, item.Type))];

    /// <summary>
    /// Hedef kullanıcının kategori alanı "boş" sayılır mı — geri yükleme, hiç
    /// dokunulmamış bir başlangıç setini yedektekiyle değiştirebilir.
    /// </summary>
    /// <remarks>
    /// İki set de kabul edilir: kullanıcının hangi cevabı verdiğini geri yükleme
    /// anında bilmek gerekmez, dokunulmamış olması yeter.
    /// </remarks>
    internal static bool IsPristineDefaultSet(IReadOnlyCollection<Category> categories) =>
        IsPristine(categories, DefaultCategorySets.PersonalSet) ||
        IsPristine(categories, DefaultCategorySets.BusinessSet);

    private static bool IsPristine(
        IReadOnlyCollection<Category> categories,
        DefaultCategorySets.DefaultCategory[] set) =>
        categories.Count == set.Length &&
        set.All(item => categories.Count(category =>
            category.IsActive &&
            category.Type == item.Type &&
            string.Equals(category.Name, item.Name, StringComparison.Ordinal)) == 1);

    /// <summary>
    /// Kullanıcının ilk kategori setini kurar.
    /// </summary>
    /// <remarks>
    /// Set, kullanıcının kaydolurken verdiği cevaba göre seçilir; profili yoksa
    /// "işletmesi yok" sayılır. Liste yalnız <b>hiç kategorisi olmayan</b>
    /// kullanıcıya uygulanır ve mevcut hesaba sonradan eklenmez: kullanıcının
    /// sildiği bir kategoriyi her açılışta geri getirmek, silme eylemini
    /// anlamsız kılardı. Aynı sebeple cevabını sonradan değiştiren kullanıcının
    /// kategorileri de değişmez — o noktada liste artık kullanıcınındır.
    /// </remarks>
    public async Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Categories
            .Where(category => category.UserId == userId)
            .ToArrayAsync(cancellationToken);
        if (existing.Length == 0)
        {
            var hasBusiness = await dbContext.UserProfiles
                .AsNoTracking()
                .Where(profile => profile.UserId == userId)
                .Select(profile => profile.HasBusiness)
                .FirstOrDefaultAsync(cancellationToken);
            var categories = DefaultCategorySets.For(hasBusiness).Select(item =>
                new Category(
                    Guid.NewGuid(),
                    userId,
                    item.Name,
                    item.Type,
                    item.Scope,
                    item.IsTax));
            await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var changed = false;
        foreach (var item in DefaultCategorySets.PersonalSet)
        {
            var legacy = existing.SingleOrDefault(category =>
                category.Type == item.Type &&
                string.Equals(category.Name, item.LegacyName, StringComparison.OrdinalIgnoreCase));
            var translatedNameExists = existing.Any(category =>
                category.Type == item.Type &&
                string.Equals(category.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            if (legacy is not null && !translatedNameExists)
            {
                legacy.Rename(item.Name);
                changed = true;
            }
        }

        if (changed)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndTypeAsync(
        Guid userId,
        string name,
        CategoryType type,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Categories.AsNoTracking().Where(
            category => category.UserId == userId && category.Type == type);

        if (string.Equals(
                dbContext.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal))
        {
            var names = await query.Select(category => category.Name).ToArrayAsync(cancellationToken);
            return names.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        return await query.AnyAsync(category => category.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> ListAsync(
        Guid userId,
        CategoryType? type,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        IQueryable<Category> query = dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId);
        if (type is CategoryType categoryType)
        {
            query = query.Where(category => category.Type == categoryType);
        }
        if (isActive is bool active)
        {
            query = query.Where(category => category.IsActive == active);
        }

        return await query
            .OrderBy(category => category.Type)
            .ThenBy(category => category.Name)
            .ThenBy(category => category.Id)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Category?> FindOwnedByIdAsync(
        Guid categoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Categories.SingleOrDefaultAsync(
            category => category.Id == categoryId && category.UserId == userId,
            cancellationToken);
    }

    public async Task UpdateOwnedAsync(
        Category category,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (category.UserId != userId)
        {
            throw new InvalidOperationException("Owned category was not found.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
