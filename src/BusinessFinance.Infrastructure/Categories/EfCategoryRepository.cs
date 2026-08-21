using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Categories;

internal sealed class EfCategoryRepository(BusinessFinanceDbContext dbContext)
    : ICategoryRepository
{
    /// <summary>
    /// Yeni kullanıcıya açılışta eklenen kategoriler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sekiz kategoriyle başlanmıştı ve fazla darmış: kullanıcı kendi
    /// kategorisini ekleyebiliyor ama eklemek istemeyen biri her harcamayı üç
    /// kovaya sıkıştırıyordu. Liste, gündelik kullanımda gerçekten ayrı ayrı
    /// izlenen kalemleri kapsayacak kadar genişledi.
    /// </para>
    /// <para>
    /// <b>Varlık sınıfı kategori değildir.</b> "Borsa" ya da "Kripto" gibi bir
    /// kategori alımı da satımı da aynı kovaya atar ve rapor anlamsızlaşır;
    /// kazanç <c>Yatırım getirisi</c> ve <c>Temettü</c> olarak izlenir. Gerçek
    /// portföy takibi (maliyet, güncel değer, realize olmayan kâr) ayrı bir
    /// kavramdır ve kategoriyle temsil edilemez.
    /// </para>
    /// <para>
    /// Aynı ad iki tipte birden bulunabilir — <c>Hediye</c> hem alınır hem
    /// verilir — çünkü teklik <c>(kullanıcı, ad, tip)</c> üçlüsündedir.
    /// </para>
    /// <para>
    /// <c>LegacyName</c> yalnız ilk sekizde anlamlı: o kayıtlar bir zamanlar
    /// İngilizce adlarla oluşturulmuştu ve okunurken Türkçeye taşınıyorlar.
    /// Sonradan eklenenler baştan Türkçe, o yüzden iki ad aynı.
    /// </para>
    /// <para>
    /// Liste yalnız <b>hiç kategorisi olmayan</b> kullanıcıya uygulanır. Mevcut
    /// hesaba sonradan eklenmez: kullanıcının sildiği bir kategoriyi her
    /// açılışta geri getirmek, silme eylemini anlamsız kılardı.
    /// </para>
    /// </remarks>
    private static readonly (string LegacyName, string Name, CategoryType Type)[] Defaults =
    [
        ("Salary", "Maaş", CategoryType.Income),
        ("Ek iş", "Ek iş", CategoryType.Income),
        ("Kira geliri", "Kira geliri", CategoryType.Income),
        ("Yatırım getirisi", "Yatırım getirisi", CategoryType.Income),
        ("Temettü", "Temettü", CategoryType.Income),
        ("Faiz geliri", "Faiz geliri", CategoryType.Income),
        ("Hediye", "Hediye", CategoryType.Income),
        ("İade ve geri ödeme", "İade ve geri ödeme", CategoryType.Income),
        ("Other Income", "Diğer Gelir", CategoryType.Income),

        ("Groceries", "Market Alışverişi", CategoryType.Expense),
        ("Housing", "Konut", CategoryType.Expense),
        ("Bills", "Faturalar", CategoryType.Expense),
        ("Transport", "Ulaşım", CategoryType.Expense),
        ("Yakıt", "Yakıt", CategoryType.Expense),
        ("Health", "Sağlık", CategoryType.Expense),
        ("Eğitim", "Eğitim", CategoryType.Expense),
        ("Yeme-içme", "Yeme-içme", CategoryType.Expense),
        ("Giyim", "Giyim", CategoryType.Expense),
        ("Entertainment", "Eğlence", CategoryType.Expense),
        ("Abonelikler", "Abonelikler", CategoryType.Expense),
        ("Kişisel bakım", "Kişisel bakım", CategoryType.Expense),
        ("Ev eşyası", "Ev eşyası", CategoryType.Expense),
        ("Evcil hayvan", "Evcil hayvan", CategoryType.Expense),
        ("Hediye", "Hediye", CategoryType.Expense),
        ("Vergi ve harç", "Vergi ve harç", CategoryType.Expense),
        ("Sigorta", "Sigorta", CategoryType.Expense),

        // Borcun faizi gerçek bir giderdir ve şimdiye kadar kategorisizdi.
        ("Faiz ve finansman gideri", "Faiz ve finansman gideri", CategoryType.Expense),
        ("Bağış", "Bağış", CategoryType.Expense),
        ("Diğer gider", "Diğer gider", CategoryType.Expense)
    ];

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
        [.. Defaults.Select(item => (item.Name, item.Type))];

    internal static bool IsPristineDefaultSet(IReadOnlyCollection<Category> categories) =>
        categories.Count == Defaults.Length &&
        Defaults.All(item => categories.Count(category =>
            category.IsActive &&
            category.Type == item.Type &&
            string.Equals(category.Name, item.Name, StringComparison.Ordinal)) == 1);

    public async Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Categories
            .Where(category => category.UserId == userId)
            .ToArrayAsync(cancellationToken);
        if (existing.Length == 0)
        {
            var categories = Defaults.Select(item =>
                new Category(Guid.NewGuid(), userId, item.Name, item.Type));
            await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var changed = false;
        foreach (var item in Defaults)
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
