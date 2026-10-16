namespace BusinessFinance.Domain.Tests;

/// <summary>
/// ADR 0021 dalga 2: borç kendi kapanışlarıyla kapanır — kısmi ödeme, sıra,
/// kartla ödeme, indirim, geri alma ve plan değişikliği.
/// </summary>
public sealed class DebtPaymentTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 10, 28);

    [Fact]
    public void PartialPayment_LeavesTheRestOpen_AndTheSecondClosesTheDebt()
    {
        var (debt, account) = Invoice(10_000m);

        var first = debt.Pay(Guid.NewGuid(), 3_000m, Today, Now, account);

        Assert.Equal(7_000m, debt.RemainingAmount);
        Assert.Equal(ObligationStatus.Open, debt.Status);
        Assert.Null(debt.Settlement);
        Assert.Equal(-3_000m, first.SignedAccountEffect);

        var second = debt.Pay(Guid.NewGuid(), 7_000m, Today, Now, account);
        Assert.True(debt.IsClosed);
        Assert.Equal(ObligationStatus.Settled, debt.Status);
        Assert.Same(second, debt.Settlement);
    }

    [Fact]
    public void Payment_CannotExceedTheRemaining()
    {
        var (debt, account) = Invoice(950m);
        debt.Pay(Guid.NewGuid(), 500m, Today, Now, account);

        var error = Assert.Throws<DebtAmountExceedsRemainingException>(
            () => debt.Pay(Guid.NewGuid(), 450.0001m, Today, Now, account));
        Assert.Equal(450m, error.Remaining);
        Assert.Equal(450m, debt.RemainingAmount);
    }

    [Fact]
    public void InstallmentPayment_StartsFromTheOldestOpenItem()
    {
        var (debt, account) = Loan(4, 1_000m);

        var payment = debt.Pay(Guid.NewGuid(), 1_500m, Today, Now, account);

        var items = debt.CurrentItems;
        Assert.True(debt.IsPaid(items[0]));
        Assert.Equal(500m, debt.PaidAmount(items[1]));
        Assert.Equal(500m, debt.OpenAmount(items[1]));
        Assert.Equal(
            [(items[0].Id, 1_000m), (items[1].Id, 500m)],
            payment.Allocations.Select(allocation => (allocation.ScheduleItemId, allocation.Amount)));

        debt.Pay(Guid.NewGuid(), 700m, Today, Now, account);
        Assert.True(debt.IsPaid(items[1]));
        Assert.Equal(200m, debt.PaidAmount(items[2]));
        Assert.Equal(1_800m, debt.RemainingAmount);
    }

    [Fact]
    public void InterestBearingDebt_IsPaidInWholeInstallmentsOnly()
    {
        var userId = Guid.NewGuid();
        var account = Account(userId);
        var debt = Debt.CreateInstallments(
            Guid.NewGuid(), userId, Person(userId), DebtDirection.Payable, TransactionScope.Business,
            Try(1_000m), Try(1_100m), DebtSourceType.Cash, account, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 9, 1), 2);

        Assert.Throws<InvalidOperationException>(() => debt.Pay(Guid.NewGuid(), 300m, Today, Now, account));
        debt.Pay(Guid.NewGuid(), 550m, Today, Now, account);
        Assert.Equal(550m, debt.RemainingAmount);
        Assert.Throws<InvalidOperationException>(
            () => debt.ChangePlan(DebtPlanKind.Single, Today, 1));
    }

    [Fact]
    public void CardPayment_MovesNoAccount_AndOnlyPaysADebt()
    {
        var (debt, _) = Invoice(950m);
        var card = Card(debt.UserId);

        var payment = debt.Pay(Guid.NewGuid(), 950m, Today, Now, null, card);

        Assert.Equal(card.Id, payment.CreditCardId);
        Assert.Null(payment.AccountId);
        Assert.Equal(0m, payment.SignedAccountEffect);
        Assert.True(debt.IsClosed);

        var (receivable, account) = Invoice(500m, DebtDirection.Receivable);
        Assert.Throws<InvalidOperationException>(
            () => receivable.Pay(Guid.NewGuid(), 500m, Today, Now, null, Card(receivable.UserId)));
        Assert.Throws<ArgumentException>(
            () => receivable.Pay(Guid.NewGuid(), 500m, Today, Now, account, Card(receivable.UserId)));
    }

    [Fact]
    public void Discount_LowersTheRemainingWithoutMovingMoney()
    {
        var (debt, account) = Invoice(10_000m);
        debt.Pay(Guid.NewGuid(), 3_000m, new DateOnly(2026, 10, 1), Now, account);

        var discount = debt.ApplyDiscount(Guid.NewGuid(), 1_000m, Today, Now);

        Assert.Equal(DebtPaymentKind.Discount, discount.Kind);
        Assert.Null(discount.AccountId);
        Assert.Equal(0m, discount.SignedAccountEffect);
        Assert.Equal(6_000m, debt.RemainingAmount);
        Assert.Throws<DebtAmountExceedsRemainingException>(
            () => debt.ApplyDiscount(Guid.NewGuid(), 6_000.01m, Today, Now));
    }

    [Fact]
    public void Discount_IsOnlyForARecordedPurchaseOrSale()
    {
        var (loan, _) = Loan(2, 500m);
        Assert.Throws<InvalidOperationException>(() => loan.ApplyDiscount(Guid.NewGuid(), 100m, Today, Now));
    }

    [Fact]
    public void ReversingAPayment_ReopensOnlyWhatItClosed()
    {
        var (debt, account) = Loan(3, 1_000m);
        var first = debt.Pay(Guid.NewGuid(), 1_000m, Today, Now, account);
        debt.Pay(Guid.NewGuid(), 1_000m, Today, Now, account);

        debt.ReversePayment(first.Id, Now.AddHours(1));

        var items = debt.CurrentItems;
        Assert.False(debt.IsPaid(items[0]));
        Assert.True(debt.IsPaid(items[1]));
        Assert.Equal(2_000m, debt.RemainingAmount);
        Assert.True(first.IsCancelled);
        Assert.Same(first, debt.ReversePayment(first.Id, Now.AddHours(2)));
    }

    [Fact]
    public void ChangingThePlan_SplitsTheRemaining_AndKeepsThePaymentsAndTheExpense()
    {
        var (debt, account) = Invoice(10_000m);
        var upfront = debt.Pay(Guid.NewGuid(), 3_000m, new DateOnly(2026, 10, 1), Now, account);

        debt.ChangePlan(DebtPlanKind.Installments, new DateOnly(2026, 11, 30), 2);

        Assert.Equal(DebtPlanKind.Installments, debt.PlanKind);
        Assert.Equal(1, debt.PlanRevision);
        Assert.Equal(
            [(new DateOnly(2026, 11, 30), 3_500m), (new DateOnly(2026, 12, 30), 3_500m)],
            debt.CurrentItems.Select(item => (item.DueDate!.Value, item.Amount.Amount)));
        Assert.Equal(7_000m, debt.RemainingAmount);
        Assert.Equal(10_000m, debt.Principal.Amount);
        Assert.False(upfront.IsCancelled);

        // Eski plana yapılmış ödeme geri alınınca tutarı yeni planın ilk açık
        // taksidine eklenir (ADR 0021 T2).
        debt.ReversePayment(upfront.Id, Now.AddHours(1));
        Assert.Equal(
            [6_500m, 3_500m],
            debt.CurrentItems.Select(item => item.Amount.Amount));
        Assert.Equal(10_000m, debt.RemainingAmount);
    }

    [Fact]
    public void NoPlan_HasNoDueDateAndIsNeverOverdue()
    {
        var (debt, account) = Invoice(4_000m);
        debt.ChangePlan(DebtPlanKind.None, null, 1);

        Assert.Null(debt.FirstDueDate);
        Assert.Null(Assert.Single(debt.CurrentItems).DueDate);
        Assert.False(debt.IsOverdueOn(new DateOnly(2027, 1, 1)));
        debt.Pay(Guid.NewGuid(), 4_000m, Today, Now, account);
        Assert.Throws<InvalidOperationException>(() => debt.ChangePlan(DebtPlanKind.Single, Today, 1));
    }

    [Fact]
    public void EveryChange_AdvancesTheRevision()
    {
        var (debt, account) = Invoice(1_000m);
        var start = debt.Revision;
        var payment = debt.Pay(Guid.NewGuid(), 100m, Today, Now, account);
        debt.ApplyDiscount(Guid.NewGuid(), 100m, Today, Now);
        debt.ReversePayment(payment.Id, Now);
        debt.ChangePlan(DebtPlanKind.None, null, 1);
        debt.Cancel(Now);
        Assert.Equal(start + 5, debt.Revision);
    }

    private static (Debt Debt, Account Account) Invoice(
        decimal amount, DebtDirection direction = DebtDirection.Payable)
    {
        var userId = Guid.NewGuid();
        var categoryType = direction == DebtDirection.Payable ? CategoryType.Expense : CategoryType.Income;
        var debt = Debt.CreateSingle(
            Guid.NewGuid(), userId, new Category(Guid.NewGuid(), userId, "Raf", categoryType),
            direction, Try(amount), TransactionScope.Business,
            new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 30), Now);
        return (debt, Account(userId));
    }

    private static (Debt Debt, Account Account) Loan(int count, decimal each)
    {
        var userId = Guid.NewGuid();
        var account = Account(userId);
        var debt = Debt.CreateInstallments(
            Guid.NewGuid(), userId, Person(userId), DebtDirection.Payable, TransactionScope.Personal,
            Try(count * each), Try(count * each), DebtSourceType.Cash, account, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 9, 10), count);
        return (debt, account);
    }

    private static Account Account(Guid userId) =>
        new(Guid.NewGuid(), userId, "Ziraat Vadesiz", AccountType.Bank, CurrencyCode.TRY);

    private static Counterparty Person(Guid userId) => new(Guid.NewGuid(), userId, "Test Toptancı");

    private static CreditCard Card(Guid userId) =>
        new(Guid.NewGuid(), userId, "İş kartı", Try(50_000m), 10, 20);

    private static Money Try(decimal amount) => new(amount, CurrencyCode.TRY);
}
