using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class SavingsGoalTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void TrackingMode_RequiresExactlyItsOwnProgressSource()
    {
        var accountId = Guid.NewGuid();
        var linked = CreateGoal(SavingsGoalTrackingMode.AccountBalance, accountId);
        var manual = CreateGoal(SavingsGoalTrackingMode.ManualContributions, null);

        Assert.Equal(accountId, linked.AccountId);
        Assert.Null(manual.AccountId);
        Assert.Throws<ArgumentException>(() =>
            CreateGoal(SavingsGoalTrackingMode.AccountBalance, null));
        Assert.Throws<ArgumentException>(() =>
            CreateGoal(SavingsGoalTrackingMode.ManualContributions, accountId));
        Assert.Throws<InvalidOperationException>(() => linked.AddContribution(
            Guid.NewGuid(), new Money(100m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 11), Guid.NewGuid(), Now));
    }

    [Fact]
    public void ManualContribution_PreservesLosslessAmountAndClientRequestId()
    {
        var goal = CreateGoal(SavingsGoalTrackingMode.ManualContributions, null);
        var requestId = Guid.NewGuid();

        var contribution = goal.AddContribution(
            Guid.NewGuid(), new Money(123.4567m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 11), requestId, Now, "  First deposit  ");

        Assert.Equal(123.4567m, contribution.Amount.Amount);
        Assert.Equal(requestId, contribution.ClientRequestId);
        Assert.Equal("First deposit", contribution.Note);
        Assert.Single(goal.Contributions);
    }

    private static SavingsGoal CreateGoal(SavingsGoalTrackingMode mode, Guid? accountId) => new(
        Guid.NewGuid(), Guid.NewGuid(), "Emergency fund", new Money(1000m, CurrencyCode.TRY),
        new DateOnly(2026, 12, 31), mode, accountId, Now, "Safety buffer");

    /// <summary>
    /// Aşama 05 Grup 6: hedef kapsam taşıyabilir ve taşımayabilir; boş olması
    /// eksik veri değildir (ADR 0013).
    /// </summary>
    [Fact]
    public void Goal_CarriesAnOptionalScope()
    {
        var userId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2026, 8, 26, 9, 0, 0, TimeSpan.Zero);

        var reserve = new SavingsGoal(
            Guid.NewGuid(), userId, "Vergi karşılığı",
            new Money(10000m, CurrencyCode.TRY), new DateOnly(2026, 12, 31),
            SavingsGoalTrackingMode.ManualContributions, null, createdAt,
            "KDV için", TransactionScope.Business);
        var unscoped = new SavingsGoal(
            Guid.NewGuid(), userId, "Etiketsiz",
            new Money(1000m, CurrencyCode.TRY), new DateOnly(2026, 12, 31),
            SavingsGoalTrackingMode.ManualContributions, null, createdAt);

        Assert.Equal(TransactionScope.Business, reserve.Scope);
        Assert.Null(unscoped.Scope);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SavingsGoal(
            Guid.NewGuid(), userId, "Geçersiz",
            new Money(1000m, CurrencyCode.TRY), new DateOnly(2026, 12, 31),
            SavingsGoalTrackingMode.ManualContributions, null, createdAt,
            null, (TransactionScope)7));
    }
}
