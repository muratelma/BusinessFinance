namespace BusinessFinance.Domain.Tests;

public sealed class RefreshSessionTests
{
    private static readonly DateTimeOffset CreatedAtUtc = new(
        2026,
        8,
        7,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public void Constructor_WithValidValues_CreatesActiveSessionWithoutRawToken()
    {
        var session = CreateSession();

        Assert.False(session.IsRevoked);
        Assert.Equal("hashed-token", session.TokenHash);
        Assert.Null(session.RevokedAtUtc);
        Assert.Null(session.ReplacedBySessionId);
        Assert.DoesNotContain(
            typeof(RefreshSession).GetProperties(),
            property => property.Name == "Token");
    }

    [Fact]
    public void Constructor_WithExpiryNotAfterCreation_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new RefreshSession(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "hashed-token",
                CreatedAtUtc,
                CreatedAtUtc));
    }

    [Fact]
    public void Constructor_WithOversizedTokenHash_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new RefreshSession(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new string('A', RefreshSession.MaximumTokenHashLength + 1),
                CreatedAtUtc,
                CreatedAtUtc.AddDays(30)));
    }

    [Fact]
    public void IsExpired_AtExpiryBoundary_ReturnsTrue()
    {
        var session = CreateSession();

        Assert.True(session.IsExpired(session.ExpiresAtUtc));
    }

    [Fact]
    public void Rotate_RevokesSessionAndLinksReplacement()
    {
        var session = CreateSession();
        var replacementId = Guid.NewGuid();
        var rotatedAtUtc = CreatedAtUtc.AddMinutes(5);

        session.Rotate(replacementId, rotatedAtUtc);

        Assert.True(session.IsRevoked);
        Assert.Equal(rotatedAtUtc, session.RevokedAtUtc);
        Assert.Equal(replacementId, session.ReplacedBySessionId);
    }

    [Fact]
    public void Rotate_WhenAlreadyRevoked_ThrowsInvalidOperationException()
    {
        var session = CreateSession();
        session.Revoke(CreatedAtUtc.AddMinutes(1));

        Assert.Throws<InvalidOperationException>(
            () => session.Rotate(Guid.NewGuid(), CreatedAtUtc.AddMinutes(2)));
    }

    [Fact]
    public void Revoke_WhenCalledTwice_PreservesFirstRevocationTime()
    {
        var session = CreateSession();
        var firstTime = CreatedAtUtc.AddMinutes(1);
        session.Revoke(firstTime);

        session.Revoke(CreatedAtUtc.AddMinutes(2));

        Assert.Equal(firstTime, session.RevokedAtUtc);
    }

    [Fact]
    public void MarkReuseDetected_RecordsDetectionAndRevokesSession()
    {
        var session = CreateSession();
        var detectedAtUtc = CreatedAtUtc.AddMinutes(3);

        session.MarkReuseDetected(detectedAtUtc);

        Assert.Equal(detectedAtUtc, session.ReuseDetectedAtUtc);
        Assert.Equal(detectedAtUtc, session.RevokedAtUtc);
    }

    [Fact]
    public void Revoke_WithNonUtcTime_ThrowsArgumentException()
    {
        var session = CreateSession();
        var nonUtcTime = new DateTimeOffset(
            2026,
            8,
            7,
            15,
            0,
            0,
            TimeSpan.FromHours(3));

        Assert.Throws<ArgumentException>(() => session.Revoke(nonUtcTime));
    }

    private static RefreshSession CreateSession()
    {
        return new RefreshSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "hashed-token",
            CreatedAtUtc,
            CreatedAtUtc.AddDays(30));
    }
}
