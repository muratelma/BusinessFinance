using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class CategoryRoleTests
{
    [Theory]
    [InlineData(CategoryRole.BankFee, CategoryType.Expense)]
    [InlineData(CategoryRole.CashDifference, CategoryType.Expense)]
    [InlineData(CategoryRole.InterestExpense, CategoryType.Expense)]
    [InlineData(CategoryRole.InterestIncome, CategoryType.Income)]
    public void RoleSurvivesRenameAndCannotBeNarrowed(CategoryRole role, CategoryType type)
    {
        var category = new Category(Guid.NewGuid(), Guid.NewGuid(), "İlk ad", type,
            TransactionScope.Personal, role: role);
        Assert.Null(category.DefaultScope);
        category.Rename("Yeni ad");
        Assert.Equal(role, category.Role);
        Assert.Throws<ArgumentException>(() => category.SetDefaultScope(TransactionScope.Business));
        Assert.Throws<ArgumentException>(() => category.SetDefaultScope(TransactionScope.Personal));
        Assert.Null(category.DefaultScope);
    }

    [Fact]
    public void RoleCannotBeAssignedToTheWrongDirection() => Assert.Throws<ArgumentException>(() =>
        new Category(Guid.NewGuid(), Guid.NewGuid(), "Faiz", CategoryType.Expense, role: CategoryRole.InterestIncome));

    [Fact]
    public void TransferCancellationCancelsItsOwnFeeExactlyOnce()
    {
        var owner = Guid.NewGuid();
        var source = new Account(Guid.NewGuid(), owner, "Çıkan", AccountType.Bank, CurrencyCode.TRY);
        var target = new Account(Guid.NewGuid(), owner, "Giren", AccountType.Bank, CurrencyCode.TRY);
        var category = new Category(Guid.NewGuid(), owner, "Masraf", CategoryType.Expense, role: CategoryRole.BankFee);
        var day = new DateOnly(2026, 8, 1);
        var fee = new BudgetTransaction(Guid.NewGuid(), owner, source, category, new Money(1.2345m, CurrencyCode.TRY),
            TransactionType.Expense, TransactionScope.Personal, day);
        var transfer = new Transfer(Guid.NewGuid(), owner, source, target, new Money(10m, CurrencyCode.TRY), day, feeTransaction: fee);
        var now = new DateTimeOffset(2026, 8, 2, 0, 0, 0, TimeSpan.Zero);
        transfer.Cancel(now); transfer.Cancel(now.AddMinutes(1));
        Assert.True(fee.IsCancelled);
        Assert.Equal(now, fee.CancelledAtUtc);
        Assert.Equal(now, transfer.CancelledAtUtc);
    }
}
