using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class RecurringTransactionOccurrenceTests
{
    [Fact]
    public void Create_SnapshotsCurrentScheduleWithoutRealizingFinancialTransaction()
    {
        var recurring = CreateRecurring(RecurringTransactionKind.BillPayment);
        var scheduledDate = recurring.NextOccurrenceDate!.Value;

        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, scheduledDate);

        Assert.Equal(recurring.UserId, occurrence.UserId);
        Assert.Equal(recurring.Id, occurrence.RecurringTransactionId);
        Assert.Equal(recurring.AccountId, occurrence.AccountId);
        Assert.Equal(recurring.CategoryId, occurrence.CategoryId);
        Assert.Equal(recurring.Amount, occurrence.Amount);
        Assert.Equal(RecurringOccurrenceStatus.Planned, occurrence.Status);
        Assert.False(occurrence.IsRealized);
        Assert.Null(occurrence.BudgetTransactionId);
        Assert.Equal($"{recurring.Id:N}:20260831", occurrence.OccurrenceKey);
        Assert.Equal(RecurringTransactionOccurrence.OccurrenceKeyLength, occurrence.OccurrenceKey.Length);
    }

    [Fact]
    public void Create_WithDateOtherThanCurrentSchedule_IsRejected()
    {
        var recurring = CreateRecurring(RecurringTransactionKind.Expense);

        Assert.Throws<InvalidOperationException>(() => RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value.AddDays(1)));
    }

    [Fact]
    public void Realize_IsIdempotentForSameTransactionAndRejectsAnotherTransaction()
    {
        var recurring = CreateRecurring(RecurringTransactionKind.Income);
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var transactionId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

        occurrence.RealizeWithTransaction(transactionId, now);
        occurrence.RealizeWithTransaction(transactionId, now.AddMinutes(1));

        Assert.True(occurrence.IsRealized);
        Assert.Equal(RecurringOccurrenceStatus.Realized, occurrence.Status);
        Assert.Equal(transactionId, occurrence.BudgetTransactionId);
        Assert.Null(occurrence.CreditCardChargeId);
        Assert.Equal(now, occurrence.RealizedAtUtc);
        Assert.Equal(TransactionType.Income, occurrence.GetTransactionType());
        Assert.Throws<InvalidOperationException>(
            () => occurrence.RealizeWithTransaction(Guid.NewGuid(), now));
    }

    [Fact]
    public void Create_FromCardPlan_CarriesCardSourceWithoutAccount()
    {
        var recurring = CreateCardRecurring(RecurringTransactionKind.BillPayment);

        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);

        Assert.Equal(RecurringSourceType.CreditCard, occurrence.SourceType);
        Assert.Equal(recurring.CreditCardId, occurrence.CreditCardId);
        Assert.Null(occurrence.AccountId);
        Assert.Null(occurrence.BudgetTransactionId);
        Assert.Null(occurrence.CreditCardChargeId);
    }

    [Fact]
    public void RealizeWithCharge_IsIdempotentForSameChargeAndRejectsAnotherCharge()
    {
        var recurring = CreateCardRecurring(RecurringTransactionKind.Expense);
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);
        var chargeId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

        occurrence.RealizeWithCharge(chargeId, now);
        occurrence.RealizeWithCharge(chargeId, now.AddMinutes(1));

        Assert.True(occurrence.IsRealized);
        Assert.Equal(chargeId, occurrence.CreditCardChargeId);
        Assert.Null(occurrence.BudgetTransactionId);
        Assert.Equal(now, occurrence.RealizedAtUtc);
        Assert.Throws<InvalidOperationException>(
            () => occurrence.RealizeWithCharge(Guid.NewGuid(), now));
    }

    [Fact]
    public void Realization_CannotCrossSourceTypes()
    {
        var accountOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(),
            CreateRecurring(RecurringTransactionKind.Expense),
            new DateOnly(2026, 8, 31));
        var cardRecurring = CreateCardRecurring(RecurringTransactionKind.Expense);
        var cardOccurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), cardRecurring, cardRecurring.NextOccurrenceDate!.Value);
        var now = new DateTimeOffset(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

        Assert.Throws<InvalidOperationException>(
            () => accountOccurrence.RealizeWithCharge(Guid.NewGuid(), now));
        Assert.Throws<InvalidOperationException>(
            () => cardOccurrence.RealizeWithTransaction(Guid.NewGuid(), now));
        Assert.False(accountOccurrence.IsRealized);
        Assert.False(cardOccurrence.IsRealized);
    }

    [Theory]
    [InlineData(RecurringTransactionKind.Expense)]
    [InlineData(RecurringTransactionKind.BillPayment)]
    public void GetTransactionType_MapsExpenseKindsToExpense(RecurringTransactionKind kind)
    {
        var recurring = CreateRecurring(kind);
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), recurring, recurring.NextOccurrenceDate!.Value);

        Assert.Equal(TransactionType.Expense, occurrence.GetTransactionType());
    }

    private static RecurringTransaction CreateRecurring(RecurringTransactionKind kind)
    {
        var userId = Guid.NewGuid();
        var account = new Account(
            Guid.NewGuid(), userId, "Account", AccountType.Bank, CurrencyCode.TRY);
        var categoryType = kind == RecurringTransactionKind.Income
            ? CategoryType.Income
            : CategoryType.Expense;
        var category = new Category(Guid.NewGuid(), userId, "Category", categoryType);
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            account,
            category,
            new Money(500m, CurrencyCode.TRY),
            kind,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31),
            description: "Snapshot");
    }

    private static RecurringTransaction CreateCardRecurring(RecurringTransactionKind kind)
    {
        var userId = Guid.NewGuid();
        var card = new CreditCard(
            Guid.NewGuid(), userId, "Card", new Money(10_000m, CurrencyCode.TRY), 15, 25);
        var category = new Category(Guid.NewGuid(), userId, "Category", CategoryType.Expense);
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            card,
            category,
            new Money(500m, CurrencyCode.TRY),
            kind,
            TransactionScope.Business,
            RecurrenceFrequency.Monthly,
            new DateOnly(2026, 8, 31),
            description: "Snapshot");
    }
}
