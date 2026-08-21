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
}
