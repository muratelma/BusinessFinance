using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class VerificationCodeTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 27, 9, 0, 0, TimeSpan.Zero);

    private static readonly string Hash = new('a', VerificationCode.CodeHashLength);

    [Fact]
    public void Constructor_KeepsTheHashAndTheLifetime()
    {
        var code = Create();

        Assert.Equal(Hash, code.CodeHash);
        Assert.Equal(VerificationPurpose.EmailConfirmation, code.Purpose);
        Assert.False(code.IsConsumed);
        Assert.False(code.IsExhausted);
        Assert.True(code.IsUsable(Now));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("too-short")]
    public void Constructor_RejectsAHashThatIsNotTheExpectedLength(string hash)
    {
        Assert.Throws<ArgumentException>(() => new VerificationCode(
            Guid.NewGuid(),
            Guid.NewGuid(),
            VerificationPurpose.EmailConfirmation,
            hash,
            Now,
            Now.AddMinutes(15)));
    }

    [Fact]
    public void Constructor_RejectsAnExpiryThatIsNotAfterCreation()
    {
        Assert.Throws<ArgumentException>(() => new VerificationCode(
            Guid.NewGuid(),
            Guid.NewGuid(),
            VerificationPurpose.EmailConfirmation,
            Hash,
            Now,
            Now));
    }

    [Fact]
    public void Constructor_RejectsALocalTime()
    {
        Assert.Throws<ArgumentException>(() => new VerificationCode(
            Guid.NewGuid(),
            Guid.NewGuid(),
            VerificationPurpose.EmailConfirmation,
            Hash,
            new DateTimeOffset(2026, 8, 27, 12, 0, 0, TimeSpan.FromHours(3)),
            Now.AddMinutes(15)));
    }

    [Fact]
    public void ExpiredCode_IsNoLongerUsable()
    {
        var code = Create();

        Assert.False(code.IsExpired(Now.AddMinutes(14)));
        Assert.True(code.IsExpired(Now.AddMinutes(15)));
        Assert.False(code.IsUsable(Now.AddMinutes(15)));
    }

    [Fact]
    public void Consume_IsIdempotentAndKeepsTheFirstMoment()
    {
        var code = Create();

        code.Consume(Now.AddMinutes(1));
        code.Consume(Now.AddMinutes(5));

        Assert.True(code.IsConsumed);
        Assert.Equal(Now.AddMinutes(1), code.ConsumedAtUtc);
        Assert.False(code.IsUsable(Now.AddMinutes(2)));
    }

    [Fact]
    public void FailedAttempts_KillTheCodeAtTheLimit()
    {
        var code = Create();

        for (var attempt = 0; attempt < VerificationCode.MaximumFailedAttempts - 1; attempt++)
        {
            code.RegisterFailedAttempt();
            Assert.True(code.IsUsable(Now));
        }

        code.RegisterFailedAttempt();

        Assert.True(code.IsExhausted);
        Assert.False(code.IsUsable(Now));
    }

    [Fact]
    public void FailedAttempts_AfterConsumption_ChangeNothing()
    {
        var code = Create();
        code.Consume(Now.AddMinutes(1));

        code.RegisterFailedAttempt();

        Assert.Equal(0, code.FailedAttemptCount);
    }

    private static VerificationCode Create() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        VerificationPurpose.EmailConfirmation,
        Hash,
        Now,
        Now.AddMinutes(15));
}
