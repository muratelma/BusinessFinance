using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;

namespace BusinessFinance.Infrastructure.Tests.Categories;

/// <summary>
/// Varsayılan kategori listesi elle bakılıyor; kendi kapısı olmalı.
/// </summary>
public sealed class DefaultCategoryTests
{
    [Fact]
    public void Defaults_HaveNoDuplicateNameWithinAType()
    {
        // Teklik `(kullanıcı, ad, tip)` üçlüsünde. Aynı tipte tekrarlanan bir
        // ad kayıt sırasında unique index ihlaliyle patlar — yani yeni
        // kullanıcı hiç açılamaz. Liste elle yazıldığı için bunu gözden
        // kaçırmak kolay.
        var duplicates = EfCategoryRepository.DefaultCategories
            .GroupBy(item => (item.Name, item.Type))
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key.Name} ({group.Key.Type})")
            .ToArray();

        Assert.True(
            duplicates.Length == 0,
            $"Aynı tipte tekrarlanan kategori adı: {string.Join(", ", duplicates)}");
    }

    [Fact]
    public void Defaults_AllowTheSameNameInBothTypes()
    {
        // Hediye hem alınır hem verilir; teklik tipi de kapsadığı için bu
        // çakışma değil. Kural yanlışlıkla ada indirgenirse bu düşer.
        var gifts = EfCategoryRepository.DefaultCategories
            .Where(item => item.Name == "Hediye")
            .Select(item => item.Type)
            .ToArray();

        Assert.Contains(CategoryType.Income, gifts);
        Assert.Contains(CategoryType.Expense, gifts);
    }

    [Fact]
    public void Defaults_CoverBothDirectionsOfMoney()
    {
        var byType = EfCategoryRepository.DefaultCategories
            .GroupBy(item => item.Type)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.True(byType.GetValueOrDefault(CategoryType.Income) > 0);
        Assert.True(byType.GetValueOrDefault(CategoryType.Expense) > 0);
    }

    [Fact]
    public void Defaults_KeepTheNamesOtherCodeDependsOn()
    {
        // Bu iki ad Flutter'daki eski İngilizce ad çevirisinin hedefi; adı
        // değiştirmek o eşlemeyi sessizce boşa çıkarır.
        var names = EfCategoryRepository.DefaultCategories
            .Select(item => item.Name)
            .ToArray();

        Assert.Contains("Maaş", names);
        Assert.Contains("Market Alışverişi", names);
    }

    [Fact]
    public void Defaults_HaveNoBlankOrUntrimmedName()
    {
        Assert.All(
            EfCategoryRepository.DefaultCategories,
            item =>
            {
                Assert.False(string.IsNullOrWhiteSpace(item.Name));
                Assert.Equal(item.Name.Trim(), item.Name);
            });
    }
}
