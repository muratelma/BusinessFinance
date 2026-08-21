using BusinessFinance.Application.Reports;
using BusinessFinance.Infrastructure.Reports;

namespace BusinessFinance.Infrastructure.Tests.Reports;

/// <summary>
/// Halka grafiğin dilimleri. Gruplama sunucuda çünkü "Diğer" bir finansal
/// toplam ve istemci parayı ikinci kez hesaplamaz.
/// </summary>
public sealed class CategoryExpenseSliceTests
{
    [Fact]
    public void Slices_KeepEveryCategoryWhenTheyAlreadyFit()
    {
        // Beş kategori zaten sığıyor: dördünü gösterip beşinciyi "Diğer" diye
        // saklamak, adı olan tek bir kategoriyi gizlemek olurdu.
        var slices = EfFinancialReportRepository.ToSlices(Categories(5));

        Assert.Equal(5, slices.Count);
        Assert.All(slices, slice => Assert.NotNull(slice.CategoryId));
    }

    [Fact]
    public void Slices_CollapseTheTailIntoOther()
    {
        var slices = EfFinancialReportRepository.ToSlices(Categories(7));

        Assert.Equal(EfFinancialReportRepository.SliceCategoryCount + 1, slices.Count);
        var other = slices[^1];
        Assert.Null(other.CategoryId);
        Assert.Equal(EfFinancialReportRepository.OtherSliceName, other.CategoryName);
    }

    [Fact]
    public void Slices_PreserveTheTotalToTheKurus()
    {
        // Asıl kural: dilimlerin toplamı kategori toplamına eşit olmalı.
        // Eşit olmasaydı grafik, altındaki "Gider" sayısıyla tutmazdı.
        var categories = Categories(9);
        var slices = EfFinancialReportRepository.ToSlices(categories);

        Assert.Equal(
            categories.Sum(item => item.Amount),
            slices.Sum(item => item.Amount));
    }

    [Fact]
    public void Slices_KeepTheLargestCategoriesInOrder()
    {
        var categories = Categories(7);
        var slices = EfFinancialReportRepository.ToSlices(categories);

        Assert.Equal(
            categories.Take(EfFinancialReportRepository.SliceCategoryCount)
                .Select(item => item.CategoryName),
            slices.Take(EfFinancialReportRepository.SliceCategoryCount)
                .Select(item => item.CategoryName));
    }

    [Fact]
    public void Slices_OfAnEmptyMonthAreEmpty()
    {
        Assert.Empty(EfFinancialReportRepository.ToSlices([]));
    }

    /// <summary>Tutara göre azalan, gerçek raporun sırasıyla aynı.</summary>
    private static CategoryExpenseDto[] Categories(int count) =>
        [.. Enumerable.Range(0, count).Select(index => new CategoryExpenseDto(
            Guid.NewGuid(),
            $"Kategori {index}",
            (count - index) * 10.25m))];
}
