using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class CreditCardActivityTests
{
    [Fact]
    public void Charge_RequiresActiveOwnedCardAndExpenseCategory()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);
        var expense = new Category(Guid.NewGuid(), userId, "Food", CategoryType.Expense);

        var charge = new CreditCardCharge(
            Guid.NewGuid(), userId, card, expense, new Money(100m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10), "  Dinner  ");

        Assert.Equal(card.Id, charge.CreditCardId);
        Assert.Equal(expense.Id, charge.CategoryId);
        Assert.Equal("Dinner", charge.Description);

        card.Update(card.Name, card.Limit, 10, 20, card.MinimumPaymentRate, false);
        Assert.Throws<InvalidOperationException>(() => new CreditCardCharge(
            Guid.NewGuid(), userId, card, expense, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void Charge_WithIncomeCategoryOrForeignOwner_IsRejected()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);
        var income = new Category(Guid.NewGuid(), userId, "Salary", CategoryType.Income);
        var foreignExpense = new Category(Guid.NewGuid(), Guid.NewGuid(), "Food", CategoryType.Expense);

        Assert.Throws<InvalidOperationException>(() => new CreditCardCharge(
            Guid.NewGuid(), userId, card, income, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10)));
        Assert.Throws<ArgumentException>(() => new CreditCardCharge(
            Guid.NewGuid(), userId, card, foreignExpense, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void Payment_AllowsInactiveOwnedCardButRequiresActiveOwnedAccount()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);
        card.Update(card.Name, card.Limit, 10, 20, card.MinimumPaymentRate, false);
        var account = new Account(
            Guid.NewGuid(), userId, "Bank", AccountType.Bank, CurrencyCode.TRY, 1000m);

        var payment = new CreditCardPayment(
            Guid.NewGuid(), userId, account, card, new Money(100m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10), "Payment");

        Assert.Equal(account.Id, payment.AccountId);
        Assert.Equal(card.Id, payment.CreditCardId);
        account.Deactivate();
        Assert.Throws<InvalidOperationException>(() => new CreditCardPayment(
            Guid.NewGuid(), userId, account, card, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void ChargeAndPayment_CancellationIsUtcAndIdempotent()
    {
        var userId = Guid.NewGuid();
        var card = CreateCard(userId);
        var account = new Account(Guid.NewGuid(), userId, "Bank", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), userId, "Food", CategoryType.Expense);
        var charge = new CreditCardCharge(
            Guid.NewGuid(), userId, card, category, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10));
        var payment = new CreditCardPayment(
            Guid.NewGuid(), userId, account, card, new Money(10m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 10));
        var now = new DateTimeOffset(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);

        charge.Cancel(now);
        charge.Cancel(now.AddMinutes(1));
        payment.Cancel(now);
        payment.Cancel(now.AddMinutes(1));

        Assert.Equal(now, charge.CancelledAtUtc);
        Assert.Equal(now, payment.CancelledAtUtc);
    }

    private static CreditCard CreateCard(Guid userId) => new(
        Guid.NewGuid(), userId, "Card", new Money(1000m, CurrencyCode.TRY), 10, 20);
}
