using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class TransferTests
{
    [Fact]
    public void Constructor_WithValidAccounts_PreservesSingleTransferEvent()
    {
        var userId = Guid.NewGuid();
        var source = CreateAccount(userId, "Source");
        var destination = CreateAccount(userId, "Destination");

        var transfer = new Transfer(
            Guid.NewGuid(),
            userId,
            source,
            destination,
            new Money(125.50m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10),
            "  Savings  ");

        Assert.Equal(source.Id, transfer.SourceAccountId);
        Assert.Equal(destination.Id, transfer.DestinationAccountId);
        Assert.Equal(new Money(125.50m, CurrencyCode.TRY), transfer.Amount);
        Assert.Equal("Savings", transfer.Description);
        Assert.False(transfer.IsCancelled);
    }

    [Fact]
    public void Constructor_WithSameAccount_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId, "Only");

        Action act = () => CreateTransfer(userId, account, account);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithForeignDestination_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        Action act = () => CreateTransfer(
            userId,
            CreateAccount(userId, "Source"),
            CreateAccount(Guid.NewGuid(), "Foreign"));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithInactiveAccount_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var source = CreateAccount(userId, "Source");
        source.Deactivate();

        Action act = () => CreateTransfer(
            userId,
            source,
            CreateAccount(userId, "Destination"));

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Constructor_WithDefaultDate_ThrowsArgumentOutOfRangeException()
    {
        var userId = Guid.NewGuid();

        Action act = () => new Transfer(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId, "Source"),
            CreateAccount(userId, "Destination"),
            new Money(10m, CurrencyCode.TRY),
            default);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Cancel_IsUtcOnlyAndIdempotent()
    {
        var userId = Guid.NewGuid();
        var transfer = CreateTransfer(
            userId,
            CreateAccount(userId, "Source"),
            CreateAccount(userId, "Destination"));
        var cancelledAt = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);

        transfer.Cancel(cancelledAt);
        transfer.Cancel(cancelledAt.AddMinutes(1));

        Assert.True(transfer.IsCancelled);
        Assert.Equal(cancelledAt, transfer.CancelledAtUtc);
        Assert.Throws<ArgumentException>(() => CreateTransfer(
            userId,
            CreateAccount(userId, "Second source"),
            CreateAccount(userId, "Second destination"))
            .Cancel(cancelledAt.ToOffset(TimeSpan.FromHours(3))));
    }

    private static Transfer CreateTransfer(Guid userId, Account source, Account destination) => new(
        Guid.NewGuid(),
        userId,
        source,
        destination,
        new Money(100m, CurrencyCode.TRY),
        new DateOnly(2026, 8, 10));

    private static Account CreateAccount(Guid userId, string name) => new(
        Guid.NewGuid(),
        userId,
        name,
        AccountType.Bank,
        CurrencyCode.TRY);
}
