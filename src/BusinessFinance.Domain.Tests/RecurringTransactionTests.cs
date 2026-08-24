using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class RecurringTransactionTests
{
    [Fact]
    public void Constructor_CreatesActiveTemplateWithoutCreatingFinancialTransaction()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.BillPayment,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 31));

        Assert.True(recurring.IsActive);
        Assert.Equal(new DateOnly(2026, 1, 31), recurring.NextOccurrenceDate);
        Assert.Equal(RecurringTransactionKind.BillPayment, recurring.Kind);
        Assert.Equal("Rent", recurring.Description);
    }

    [Theory]
    [InlineData(RecurringTransactionKind.Income, CategoryType.Expense)]
    [InlineData(RecurringTransactionKind.Expense, CategoryType.Income)]
    [InlineData(RecurringTransactionKind.BillPayment, CategoryType.Income)]
    public void Constructor_WithIncompatibleCategory_IsRejected(
        RecurringTransactionKind kind,
        CategoryType categoryType)
    {
        var userId = Guid.NewGuid();
        var account = CreateAccount(userId);
        var category = new Category(Guid.NewGuid(), userId, "Category", categoryType);

        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(100m, CurrencyCode.TRY),
            kind,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void Constructor_WithCrossUserAccount_IsRejected()
    {
        var userId = Guid.NewGuid();
        var category = new Category(Guid.NewGuid(), userId, "Rent", CategoryType.Expense);

        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(Guid.NewGuid()),
            category,
            new Money(100m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 1, 31)));
    }

    [Fact]
    public void MonthlyClamp_UsesMonthEndWithoutLosingOriginalAnchorDay()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2027, 1, 31));

        recurring.AdvanceAfter(new DateOnly(2027, 1, 31));
        Assert.Equal(new DateOnly(2027, 2, 28), recurring.NextOccurrenceDate);

        recurring.AdvanceAfter(new DateOnly(2027, 2, 28));
        Assert.Equal(new DateOnly(2027, 3, 31), recurring.NextOccurrenceDate);
    }

    [Fact]
    public void MonthlySkip_OmitsMonthsWithoutAnchorDay()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 31),
            monthEndBehavior: MonthEndBehavior.SkipInvalidPeriod);

        recurring.AdvanceAfter(new DateOnly(2026, 1, 31));

        Assert.Equal(new DateOnly(2026, 3, 31), recurring.NextOccurrenceDate);
    }

    [Fact]
    public void YearlyClamp_RestoresLeapDayInNextLeapYear()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Income,
            RecurrenceFrequency.Yearly,
            new DateOnly(2024, 2, 29));

        recurring.AdvanceAfter(new DateOnly(2024, 2, 29));
        Assert.Equal(new DateOnly(2025, 2, 28), recurring.NextOccurrenceDate);

        recurring.AdvanceAfter(new DateOnly(2025, 2, 28));
        recurring.AdvanceAfter(new DateOnly(2026, 2, 28));
        recurring.AdvanceAfter(new DateOnly(2027, 2, 28));
        Assert.Equal(new DateOnly(2028, 2, 29), recurring.NextOccurrenceDate);
    }

    [Fact]
    public void AdvanceAfter_EndDateCompletesAndDeactivatesSchedule()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Weekly,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 1),
            occurrenceLimit: 12);

        recurring.AdvanceAfter(new DateOnly(2026, 8, 1));

        Assert.False(recurring.IsActive);
        Assert.Null(recurring.NextOccurrenceDate);
        Assert.Throws<InvalidOperationException>(() => recurring.Activate());
    }

    [Fact]
    public void AdvanceAfter_OccurrenceLimitCompletesAndDeactivatesSchedule()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 1),
            occurrenceLimit: 2);

        recurring.AdvanceAfter(new DateOnly(2026, 1, 1));
        Assert.Equal(1, recurring.GeneratedOccurrenceCount);
        Assert.Equal(new DateOnly(2026, 2, 1), recurring.NextOccurrenceDate);

        recurring.AdvanceAfter(new DateOnly(2026, 2, 1));

        Assert.Equal(2, recurring.GeneratedOccurrenceCount);
        Assert.False(recurring.IsActive);
        Assert.Null(recurring.NextOccurrenceDate);
        Assert.Throws<InvalidOperationException>(() => recurring.Activate());
    }

    [Fact]
    public void Constructor_WithNonPositiveOccurrenceLimit_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 1, 1),
            occurrenceLimit: 0));
    }

    [Fact]
    public void AdvanceAfter_WithUnexpectedDate_IsRejectedWithoutChangingSchedule()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Daily,
            new DateOnly(2026, 8, 1));

        Assert.Throws<InvalidOperationException>(() => recurring.AdvanceAfter(new DateOnly(2026, 8, 2)));
        Assert.Equal(new DateOnly(2026, 8, 1), recurring.NextOccurrenceDate);
    }

    [Fact]
    public void GetFollowingDate_ProjectsScheduleWithoutChangingItsState()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31));

        var september = recurring.GetFollowingDate(new DateOnly(2026, 8, 31));
        var october = recurring.GetFollowingDate(september!.Value);

        Assert.Equal(new DateOnly(2026, 9, 30), september);
        Assert.Equal(new DateOnly(2026, 10, 31), october);
        Assert.Equal(new DateOnly(2026, 8, 31), recurring.NextOccurrenceDate);
    }

    [Fact]
    public void AccountSource_LeavesCreditCardEmpty()
    {
        var recurring = CreateRecurring(
            RecurringTransactionKind.Expense,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1));

        Assert.Equal(RecurringSourceType.Account, recurring.SourceType);
        Assert.NotNull(recurring.AccountId);
        Assert.Null(recurring.CreditCardId);
    }

    [Theory]
    [InlineData(RecurringTransactionKind.Expense)]
    [InlineData(RecurringTransactionKind.BillPayment)]
    public void CreditCardSource_IsAcceptedForExpenseAndBillPayment(RecurringTransactionKind kind)
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);

        var recurring = CreateCardRecurring(userId, card, kind);

        Assert.Equal(RecurringSourceType.CreditCard, recurring.SourceType);
        Assert.Equal(card.Id, recurring.CreditCardId);
        Assert.Null(recurring.AccountId);
    }

    [Fact]
    public void CreditCardSource_RejectsIncome()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => CreateCardRecurring(
            userId, CreateCard(userId), RecurringTransactionKind.Income));
    }

    [Fact]
    public void CreditCardSource_RejectsInactiveCard()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);
        card.Update(
            card.Name, card.Limit, card.StatementClosingDay, card.PaymentDueDay,
            card.MinimumPaymentRate, isActive: false);

        Assert.Throws<InvalidOperationException>(() => CreateCardRecurring(
            userId, card, RecurringTransactionKind.Expense));
    }

    [Fact]
    public void CreditCardSource_RejectsCardOwnedByAnotherUser()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => CreateCardRecurring(
            userId, CreateCard(Guid.NewGuid()), RecurringTransactionKind.Expense));
    }

    [Fact]
    public void CreditCardSource_RejectsIncomeCategoryForExpenseKind()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateCard(userId),
            new Category(Guid.NewGuid(), userId, "Category", CategoryType.Income),
            new Money(100m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1)));
    }

    private static RecurringTransaction CreateCardRecurring(
        Guid userId,
        CreditCard card,
        RecurringTransactionKind kind)
    {
        var categoryType = kind == RecurringTransactionKind.Income
            ? CategoryType.Income
            : CategoryType.Expense;
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            card,
            new Category(Guid.NewGuid(), userId, "Category", categoryType),
            new Money(100m, CurrencyCode.TRY),
            kind,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 1));
    }

    private static CreditCard CreateCard(Guid userId)
    {
        return new CreditCard(
            Guid.NewGuid(), userId, "Card", new Money(10_000m, CurrencyCode.TRY), 15, 25);
    }

    private static RecurringTransaction CreateRecurring(
        RecurringTransactionKind kind,
        RecurrenceFrequency frequency,
        DateOnly startDate,
        DateOnly? endDate = null,
        MonthEndBehavior monthEndBehavior = MonthEndBehavior.ClampToLastDay,
        int? occurrenceLimit = null)
    {
        var userId = Guid.NewGuid();
        var categoryType = kind == RecurringTransactionKind.Income
            ? CategoryType.Income
            : CategoryType.Expense;
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            CreateAccount(userId),
            new Category(Guid.NewGuid(), userId, "Category", categoryType),
            new Money(100m, CurrencyCode.TRY),
            kind,
            TransactionScope.Business,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            " Rent ",
            occurrenceLimit);
    }

    private static Account CreateAccount(Guid userId)
    {
        return new Account(Guid.NewGuid(), userId, "Cash", AccountType.Cash, CurrencyCode.TRY);
    }
}
