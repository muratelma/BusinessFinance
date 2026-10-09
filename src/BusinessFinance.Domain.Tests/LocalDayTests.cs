namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Kullanıcının takvim günü sunucunun UTC gününden bir gün ileride olabilir.
/// Türkiye'de 9 Ekim gecesi 01:30, UTC'de hâlâ 8 Ekim 22:30'dur; o saatte
/// "bugün" (9 Ekim) tarihli kayıt reddedilmemelidir.
/// </summary>
public sealed class LocalDayTests
{
    [Theory]
    // Türkiye'de 9 Ekim 01:30 (UTC 8 Ekim 22:30): kullanıcının bugünü 9 Ekim.
    [InlineData(2026, 10, 8, 22, 30, 2026, 10, 9)]
    // Gündüz: pay yine bir gündür, iki gün sonrası kabul edilmez.
    [InlineData(2026, 10, 9, 12, 0, 2026, 10, 10)]
    // Ay ve yıl sınırı.
    [InlineData(2026, 12, 31, 23, 59, 2027, 1, 1)]
    public void LatestAllowed_IsOneDayAheadOfTheUtcDay(
        int year, int month, int day, int hour, int minute,
        int expectedYear, int expectedMonth, int expectedDay)
    {
        var utcNow = new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero);

        Assert.Equal(
            new DateOnly(expectedYear, expectedMonth, expectedDay),
            LocalDay.LatestAllowed(utcNow));
    }

    [Fact]
    public void LatestAllowed_ReadsTheUtcDayWhateverTheOffsetOfTheClock()
    {
        // Aynı an, Türkiye saatiyle verilmiş: 9 Ekim 01:30 +03:00.
        var istanbul = new DateTimeOffset(2026, 10, 9, 1, 30, 0, TimeSpan.FromHours(3));

        Assert.Equal(new DateOnly(2026, 10, 9), LocalDay.LatestAllowed(istanbul));
    }
}
