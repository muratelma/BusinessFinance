using BusinessFinance.Application.Abstractions.Queries;

namespace BusinessFinance.Application.Tests.Abstractions;

/// <summary>
/// Geçmiş listelerinin sınırı. Bu kural elle uygulanıyor (her repository kendi
/// sorgusunu yazıyor), o yüzden kuralın kendisinin kapısı burada.
/// </summary>
public sealed class HistoryWindowTests
{
    [Fact]
    public void Default_LooksBackThreeMonthsAndHasNoUpperBound()
    {
        var window = HistoryWindow.DefaultFor(new DateOnly(2026, 8, 17));

        Assert.Equal(new DateOnly(2026, 5, 17), window.From);
        Assert.Null(window.To);
    }

    [Fact]
    public void Default_IsNotUnbounded()
    {
        // Asıl kusur buydu: varsayılan yokken listeler tüm geçmişi çekiyordu.
        var window = HistoryWindow.DefaultFor(new DateOnly(2026, 8, 17));

        Assert.NotNull(window.From);
        Assert.Null(HistoryWindow.Unbounded.From);
    }

    [Theory]
    [InlineData("2026-05-16", false)]
    [InlineData("2026-05-17", true)]
    [InlineData("2026-08-17", true)]
    public void Contains_IncludesTheBoundaryDay(string date, bool expected)
    {
        var window = HistoryWindow.DefaultFor(new DateOnly(2026, 8, 17));

        Assert.Equal(expected, window.Contains(DateOnly.Parse(date)));
    }

    [Fact]
    public void Contains_WithBothEnds_ExcludesOutsideDates()
    {
        var window = new HistoryWindow(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31));

        Assert.True(window.Contains(new DateOnly(2026, 3, 15)));
        Assert.False(window.Contains(new DateOnly(2026, 2, 28)));
        Assert.False(window.Contains(new DateOnly(2026, 4, 1)));
    }

    [Fact]
    public void UnboundedWindow_StillCarriesARowCeiling()
    {
        // "Tümü" tarih sınırını kaldırır ama sorguyu sınırsız bırakmaz;
        // aksi hâlde bu seçenek sorunun kendisini geri getirirdi.
        Assert.Null(HistoryWindow.Unbounded.From);
        Assert.Null(HistoryWindow.Unbounded.To);
        Assert.True(HistoryWindow.MaximumRows > 0);
    }
}
