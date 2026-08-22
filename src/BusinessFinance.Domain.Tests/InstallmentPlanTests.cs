using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class InstallmentPlanTests
{
    [Fact]
    public void Constructor_SplitsTotalAtFourDecimalsAndKeepsRemainderInLastItem()
    {
        var userId = Guid.NewGuid();
        var plan = CreatePlan(userId, 100m, 3);
        var items = plan.Items.OrderBy(item => item.Sequence).ToArray();

        Assert.Equal([33.3333m, 33.3333m, 33.3334m], items.Select(item => item.Amount.Amount));
        Assert.Equal(100m, items.Sum(item => item.Amount.Amount));
        Assert.Equal(
            [new DateOnly(2026, 8, 10), new DateOnly(2026, 9, 10), new DateOnly(2026, 10, 10)],
            items.Select(item => item.ScheduledDate));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(61)]
    public void Constructor_WithUnsupportedCount_IsRejected(int count)
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentOutOfRangeException>(() => CreatePlan(userId, 100m, count));
    }

    [Fact]
    public void Constructor_WhenTotalCannotProducePositiveItems_IsRejected()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentOutOfRangeException>(() => CreatePlan(userId, 0.0002m, 3));
    }

    [Fact]
    public void ItemRealize_IsIdempotentForSameChargeAndRejectsAnotherCharge()
    {
        var userId = Guid.NewGuid();
        var item = CreatePlan(userId, 100m, 2).GetItem(1);
        var chargeId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);

        item.Realize(chargeId, now);
        item.Realize(chargeId, now.AddMinutes(1));

        Assert.True(item.IsRealized);
        Assert.Equal(chargeId, item.CreditCardChargeId);
        Assert.Equal(now, item.RealizedAtUtc);
        Assert.Throws<InvalidOperationException>(() => item.Realize(Guid.NewGuid(), now));
    }

    private static InstallmentPlan CreatePlan(Guid userId, decimal total, int count)
    {
        var card = new CreditCard(
            Guid.NewGuid(), userId, "Card", new Money(1000m, CurrencyCode.TRY), 10, 20);
        var category = new Category(Guid.NewGuid(), userId, "Shopping", CategoryType.Expense);
        return new InstallmentPlan(
            Guid.NewGuid(),
            userId,
            card,
            category,
            Guid.NewGuid(),
            new Money(total, CurrencyCode.TRY),
            TransactionScope.Business,
            count,
            new DateOnly(2026, 8, 10),
            "Laptop");
    }
}
