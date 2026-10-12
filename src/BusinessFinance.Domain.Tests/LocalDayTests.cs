namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Gün Türkiye saatiyle hesaplanır (UTC+3, yıl boyu). Türkiye'de 9 Ekim gecesi
/// 01:30, UTC'de hâlâ 8 Ekim 22:30'dur; o saatte "bugün" 9 Ekim'dir ve 9 Ekim
/// tarihli kayıt reddedilmemelidir. Pay yoktur: yarının tarihi günün hiçbir
/// saatinde kabul edilmez.
/// </summary>
public sealed class LocalDayTests
{
    [Theory]
    // Türkiye'de 9 Ekim 01:30 (UTC 8 Ekim 22:30): bugün 9 Ekim.
    [InlineData(2026, 10, 8, 22, 30, 2026, 10, 9)]
    // Gece yarısının iki yanı: UTC 20:59 hâlâ 8 Ekim, 21:00 artık 9 Ekim.
    [InlineData(2026, 10, 8, 20, 59, 2026, 10, 8)]
    [InlineData(2026, 10, 8, 21, 0, 2026, 10, 9)]
    // Gündüz: bugün bugündür.
    [InlineData(2026, 10, 9, 12, 0, 2026, 10, 9)]
    // Ay ve yıl sınırı.
    [InlineData(2026, 12, 31, 23, 59, 2027, 1, 1)]
    public void Today_IsTheCalendarDayInTurkey(
        int year, int month, int day, int hour, int minute,
        int expectedYear, int expectedMonth, int expectedDay)
    {
        var utcNow = new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero);
        var expected = new DateOnly(expectedYear, expectedMonth, expectedDay);

        Assert.Equal(expected, LocalDay.Today(utcNow));
        // En ileri gün bugündür; yarına pay tanınmaz.
        Assert.Equal(expected, LocalDay.LatestAllowed(utcNow));
    }

    [Fact]
    public void Today_ReadsTheSameInstantWhateverTheOffsetOfTheClock()
    {
        // Aynı an üç saatle: UTC, Türkiye ve New York.
        var utc = new DateTimeOffset(2026, 10, 8, 22, 30, 0, TimeSpan.Zero);
        var istanbul = new DateTimeOffset(2026, 10, 9, 1, 30, 0, TimeSpan.FromHours(3));
        var newYork = new DateTimeOffset(2026, 10, 8, 18, 30, 0, TimeSpan.FromHours(-4));

        Assert.Equal(new DateOnly(2026, 10, 9), LocalDay.Today(utc));
        Assert.Equal(new DateOnly(2026, 10, 9), LocalDay.Today(istanbul));
        Assert.Equal(new DateOnly(2026, 10, 9), LocalDay.Today(newYork));
    }
}
