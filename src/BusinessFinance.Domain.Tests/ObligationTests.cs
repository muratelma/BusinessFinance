namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 03 Grup 1: tek seferlik yükümlülüğün tanıma/taşıma ve vade kuralları.
/// </summary>
public sealed class ObligationTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 8, 24, 10, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(DebtDirection.Payable, CategoryType.Expense, TransactionType.Expense)]
    [InlineData(DebtDirection.Receivable, CategoryType.Income, TransactionType.Income)]
    public void Obligation_RecognizesAccordingToDirectionWithoutAnAccount(
        DebtDirection direction,
        CategoryType categoryType,
        TransactionType recognizedType)
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(userId, direction, categoryType);

        Assert.Equal(recognizedType, obligation.RecognizedType);
        Assert.Null(obligation.Settlement);
        Assert.Equal(ObligationStatus.Open, obligation.Status);
        Assert.DoesNotContain(
            "AccountId",
            typeof(Obligation).GetProperties().Select(property => property.Name));
    }

    [Fact]
    public void Obligation_AllowsNoCounterpartyAndKeepsAnOwnedActiveOne()
    {
        var userId = Guid.NewGuid();
        var withoutCounterparty = NewObligation(userId);
        var counterparty = new Counterparty(Guid.NewGuid(), userId, "Elektrik şirketi");
        var withCounterparty = NewObligation(userId, counterparty: counterparty);

        Assert.Null(withoutCounterparty.CounterpartyId);
        Assert.Equal(counterparty.Id, withCounterparty.CounterpartyId);

        var foreign = new Counterparty(Guid.NewGuid(), Guid.NewGuid(), "Yabancı taraf");
        Assert.Throws<ArgumentException>(() => NewObligation(userId, counterparty: foreign));

        counterparty.Deactivate();
        Assert.Throws<InvalidOperationException>(() =>
            NewObligation(userId, counterparty: counterparty));
    }

    [Fact]
    public void Obligation_RequiresOwnedActiveCategoryMatchingTheDirection()
    {
        var userId = Guid.NewGuid();
        var foreignExpense = new Category(
            Guid.NewGuid(), Guid.NewGuid(), "Elektrik", CategoryType.Expense);
        var income = new Category(Guid.NewGuid(), userId, "Satış", CategoryType.Income);

        Assert.Throws<ArgumentException>(() => NewObligation(userId, category: foreignExpense));
        Assert.Throws<InvalidOperationException>(() => NewObligation(userId, category: income));

        var expense = new Category(Guid.NewGuid(), userId, "Elektrik", CategoryType.Expense);
        expense.Deactivate();
        Assert.Throws<InvalidOperationException>(() => NewObligation(userId, category: expense));
    }

    [Fact]
    public void Obligation_RequiresAValidScopeAndNormalizesDescription()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(userId, description: "  Ağustos faturası  ");

        Assert.Equal(TransactionScope.Business, obligation.Scope);
        Assert.Equal("Ağustos faturası", obligation.Description);
        Assert.Throws<ArgumentOutOfRangeException>(() => NewObligation(
            userId,
            scope: (TransactionScope)7));
        Assert.Throws<ArgumentException>(() => NewObligation(
            userId,
            description: new string('a', Obligation.MaximumDescriptionLength + 1)));
    }

    [Fact]
    public void IssueDateCannotBeFutureButPastAndFutureDueDatesAreValid()
    {
        var userId = Guid.NewGuid();

        var overdue = NewObligation(
            userId,
            issueDate: new DateOnly(2026, 8, 1),
            dueDate: new DateOnly(2026, 8, 10));
        var upcoming = NewObligation(
            userId,
            issueDate: new DateOnly(2026, 8, 24),
            dueDate: new DateOnly(2026, 9, 10));

        Assert.Equal(new DateOnly(2026, 8, 10), overdue.DueDate);
        Assert.Equal(new DateOnly(2026, 9, 10), upcoming.DueDate);
        // Kullanıcının günü sunucunun UTC gününden bir gün ileride olabilir
        // (`LocalDay`): gece yarısından sonra bugünün tarihi kabul edilir, iki
        // gün sonrası reddedilir.
        Assert.Equal(
            new DateOnly(2026, 8, 25),
            NewObligation(
                userId,
                issueDate: new DateOnly(2026, 8, 25),
                dueDate: new DateOnly(2026, 9, 10)).IssueDate);
        Assert.Throws<ArgumentOutOfRangeException>(() => NewObligation(
            userId,
            issueDate: new DateOnly(2026, 8, 26),
            dueDate: new DateOnly(2026, 9, 10)));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewObligation(
            userId,
            issueDate: new DateOnly(2026, 8, 10),
            dueDate: new DateOnly(2026, 8, 9)));
    }

    [Fact]
    public void OverdueStateIsDerivedFromTheDateAndOnlyWhileOpen()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(
            userId,
            dueDate: new DateOnly(2026, 8, 25));

        Assert.False(obligation.IsOverdueOn(new DateOnly(2026, 8, 25)));
        Assert.True(obligation.IsOverdueOn(new DateOnly(2026, 8, 26)));
        Assert.DoesNotContain("Overdue", Enum.GetNames<ObligationStatus>());

        obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 26),
            new DateTimeOffset(2026, 8, 26, 9, 0, 0, TimeSpan.Zero));

        Assert.False(obligation.IsOverdueOn(new DateOnly(2026, 9, 1)));
    }

    [Theory]
    [InlineData(DebtDirection.Payable, -250)]
    [InlineData(DebtDirection.Receivable, 250)]
    public void SettlementMovesCashWithoutCategoryOrScope(
        DebtDirection direction,
        decimal signedEffect)
    {
        var userId = Guid.NewGuid();
        var categoryType = direction == DebtDirection.Payable
            ? CategoryType.Expense
            : CategoryType.Income;
        var obligation = NewObligation(userId, direction, categoryType);

        var settlement = obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 24),
            CreatedAtUtc);
        var properties = typeof(ObligationSettlement)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.Equal(signedEffect, settlement.SignedAccountEffect);
        Assert.Equal(ObligationStatus.Settled, obligation.Status);
        Assert.DoesNotContain("CategoryId", properties);
        Assert.DoesNotContain("Scope", properties);
    }

    [Fact]
    public void SettlingTwiceReturnsTheOriginalCashMovement()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(userId);
        var firstId = Guid.NewGuid();

        var first = obligation.Settle(
            firstId,
            NewAccount(userId),
            new DateOnly(2026, 8, 24),
            CreatedAtUtc);
        var second = obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 24),
            CreatedAtUtc.AddMinutes(1));

        Assert.Same(first, second);
        Assert.Equal(firstId, second.Id);
    }

    [Fact]
    public void SettlementRequiresAnOwnedActiveAccount()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(userId);
        var foreignAccount = NewAccount(Guid.NewGuid());
        var inactiveAccount = NewAccount(userId);
        inactiveAccount.Deactivate();

        Assert.Throws<ArgumentException>(() => obligation.Settle(
            Guid.NewGuid(), foreignAccount, new DateOnly(2026, 8, 24), CreatedAtUtc));
        Assert.Throws<InvalidOperationException>(() => obligation.Settle(
            Guid.NewGuid(), inactiveAccount, new DateOnly(2026, 8, 24), CreatedAtUtc));
    }

    [Fact]
    public void SettlementDateCannotPrecedeTheIssueDateOrBeInTheFuture()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(
            userId,
            issueDate: new DateOnly(2026, 8, 20));

        Assert.Throws<ArgumentOutOfRangeException>(() => obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 19),
            CreatedAtUtc));
        // Kullanıcının günü sunucunun UTC gününden bir gün ileride olabilir
        // (`LocalDay`): gece yarısından sonra bugünün tarihi kabul edilir, iki
        // gün sonrası reddedilir.
        Assert.Throws<ArgumentOutOfRangeException>(() => obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 26),
            CreatedAtUtc));
        var settledAtNight = obligation.Settle(
            Guid.NewGuid(),
            NewAccount(userId),
            new DateOnly(2026, 8, 25),
            CreatedAtUtc);
        Assert.Equal(new DateOnly(2026, 8, 25), settledAtNight.SettlementDate);
    }

    [Fact]
    public void CancellationIsIdempotentAndReversesAnExistingSettlement()
    {
        var userId = Guid.NewGuid();
        var obligation = NewObligation(userId);
        var settlement = obligation.Settle(
            Guid.NewGuid(), NewAccount(userId), new DateOnly(2026, 8, 24), CreatedAtUtc);
        var cancelledAt = CreatedAtUtc.AddHours(1);

        obligation.Cancel(cancelledAt);
        obligation.Cancel(cancelledAt.AddHours(1));

        Assert.Equal(ObligationStatus.Cancelled, obligation.Status);
        Assert.Equal(cancelledAt, obligation.CancelledAtUtc);
        Assert.True(settlement.IsCancelled);
        Assert.Equal(cancelledAt, settlement.CancelledAtUtc);
        Assert.Throws<InvalidOperationException>(() => obligation.Settle(
            Guid.NewGuid(), NewAccount(userId), new DateOnly(2026, 8, 24), CreatedAtUtc));
    }

    private static Obligation NewObligation(
        Guid userId,
        DebtDirection direction = DebtDirection.Payable,
        CategoryType categoryType = CategoryType.Expense,
        Category? category = null,
        TransactionScope scope = TransactionScope.Business,
        DateOnly? issueDate = null,
        DateOnly? dueDate = null,
        Counterparty? counterparty = null,
        string? description = null) => new(
        Guid.NewGuid(),
        userId,
        category ?? new Category(Guid.NewGuid(), userId, "Fatura", categoryType),
        direction,
        new Money(250m, CurrencyCode.TRY),
        scope,
        issueDate ?? new DateOnly(2026, 8, 20),
        dueDate ?? new DateOnly(2026, 8, 25),
        CreatedAtUtc,
        counterparty,
        description);

    private static Account NewAccount(Guid userId) =>
        new(Guid.NewGuid(), userId, "Dükkân kasası", AccountType.Cash, CurrencyCode.TRY);
}
