using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

public sealed class DebtAgreementTests
{
    [Fact]
    public void Schedule_PreservesFourDecimalTotalAndRemainingAmount()
    {
        var debt = CreateDebt(1000m, 1100m, 3);
        Assert.Equal([366.6666m, 366.6666m, 366.6668m], debt.Installments.Select(x => x.Amount.Amount));
        Assert.Equal(1100m, debt.RemainingAmount);
    }

    [Fact]
    public void Payment_RequiresOwnedActiveAccountAndCannotRepeat()
    {
        var userId = Guid.NewGuid();
        var debt = CreateDebt(100m, 100m, 1, userId);
        var account = new Account(Guid.NewGuid(), userId, "Bank", AccountType.Bank, CurrencyCode.TRY);
        var installment = debt.GetInstallment(1);
        installment.MarkPaid(account, new DateOnly(2026, 8, 11), DateTimeOffset.UtcNow);
        Assert.Equal(0m, debt.RemainingAmount);
        Assert.True(debt.IsClosed);
        Assert.Throws<InvalidOperationException>(() => installment.MarkPaid(
            account, new DateOnly(2026, 8, 11), DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Rate_IsSolvedFromMoneyRatherThanTakenFromTheCaller()
    {
        // Oran artık girdi değil. Kullanıcı ne yazarsa yazsın plan paradan
        // bölünür, oran da paradan çözülür; ikisi çelişemez.
        var debt = CreateDebt(300m, 400m, 4);

        Assert.Equal(151.0780m, debt.AnnualInterestRate);
        Assert.Equal(100m, debt.TotalInterest);
    }

    [Fact]
    public void ZeroInterestDebt_ReportsZeroRate()
    {
        var debt = CreateDebt(300m, 300m, 3);

        Assert.Equal(0m, debt.AnnualInterestRate);
        Assert.Equal(0m, debt.TotalInterest);
        Assert.All(debt.InstallmentSplits, split => Assert.Equal(0m, split.Interest));
    }

    [Fact]
    public void InstallmentSplits_AddUpToPrincipalAndInterest()
    {
        var debt = CreateDebt(1000m, 1100m, 3);
        var splits = debt.InstallmentSplits;

        Assert.Equal(1000m, splits.Sum(x => x.Principal));
        Assert.Equal(100m, splits.Sum(x => x.Interest));
    }

    [Fact]
    public void TotalImplyingAnUnreachableRate_IsRejected()
    {
        // Anapara 300'e karşı 30.000 toplam, üç ayda %1000 yıllık sınırın
        // çok üstünde bir orana denk gelir. Sessizce sınıra kırpılırsa
        // kullanıcı yazdığından başka bir borç kaydeder.
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateDebt(300m, 30000m, 3));
    }

    [Fact]
    public void CashSource_RequiresAnAccountAndRefusesACategory()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Cash, openingAccount: null, category: null));

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Cash,
            openingAccount: ActiveAccount(userId), category: ExpenseCategory(userId)));
    }

    [Fact]
    public void ExpenseSource_RequiresACategoryAndRefusesAnAccount()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Expense, openingAccount: null, category: null));

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Expense,
            openingAccount: ActiveAccount(userId), category: ExpenseCategory(userId)));
    }

    [Fact]
    public void CategoricalSource_MustMatchTheDirection()
    {
        // Borç tüketir, alacak satar. Ters eşleşme parayı yanlış tarafa
        // yazardı: satılan bir şey gider, tüketilen bir şey gelir olurdu.
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Receivable, DebtSourceType.Expense,
            openingAccount: null, category: ExpenseCategory(userId)));

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Income,
            openingAccount: null, category: IncomeCategory(userId)));
    }

    [Fact]
    public void CategoricalSource_RefusesTheWrongCategoryType()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Expense,
            openingAccount: null, category: IncomeCategory(userId)));

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Receivable, DebtSourceType.Income,
            openingAccount: null, category: ExpenseCategory(userId)));
    }

    [Fact]
    public void IncomeSourcedReceivable_KeepsTheIncomeCategoryAndNoAccount()
    {
        // "Telefonumu sattım, üç ayda ödeyecek." Bu kaynak olmadan durum
        // ancak nakit alacak olarak girilebiliyordu: uygulama hesaptan para
        // çıkmış gibi davranıyor, satış da hiç gelir olarak görünmüyordu.
        var userId = Guid.NewGuid();
        var category = IncomeCategory(userId);

        var debt = Build(
            userId, DebtDirection.Receivable, DebtSourceType.Income, null, category);

        Assert.Equal(DebtSourceType.Income, debt.SourceType);
        Assert.Equal(category.Id, debt.CategoryId);
        Assert.Null(debt.OpeningAccountId);
    }

    [Fact]
    public void OpeningRecord_RefusesForeignOrInactiveOwners()
    {
        var userId = Guid.NewGuid();
        var foreignAccount = ActiveAccount(Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Cash,
            openingAccount: foreignAccount, category: null));

        var inactiveAccount = ActiveAccount(userId);
        inactiveAccount.Deactivate();
        Assert.Throws<InvalidOperationException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Cash,
            openingAccount: inactiveAccount, category: null));

        var inactiveCategory = ExpenseCategory(userId);
        inactiveCategory.Deactivate();
        Assert.Throws<InvalidOperationException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Expense,
            openingAccount: null, category: inactiveCategory));
    }

    [Fact]
    public void ExpenseSourcedDebt_KeepsTheCategoryAndNoAccount()
    {
        var userId = Guid.NewGuid();
        var category = ExpenseCategory(userId);

        var debt = Build(userId, DebtDirection.Payable, DebtSourceType.Expense, null, category);

        Assert.Equal(DebtSourceType.Expense, debt.SourceType);
        Assert.Equal(category.Id, debt.CategoryId);
        Assert.Null(debt.OpeningAccountId);
    }

    [Fact]
    public void CashSourcedDebt_KeepsTheAccountAndNoCategory()
    {
        var userId = Guid.NewGuid();
        var account = ActiveAccount(userId);

        var debt = Build(userId, DebtDirection.Payable, DebtSourceType.Cash, account, null);

        Assert.Equal(DebtSourceType.Cash, debt.SourceType);
        Assert.Equal(account.Id, debt.OpeningAccountId);
        Assert.Null(debt.CategoryId);
    }

    [Fact]
    public void Schedule_StoresTheSplitOnEachInstallment()
    {
        // Ayrım saklanıyor çünkü aylık gider raporu faiz payını SQL'de
        // toplamak zorunda; anüite ayrımı EF LINQ'e çevrilmiyor.
        var debt = CreateDebt(1000m, 1100m, 3);
        var installments = debt.Installments.OrderBy(x => x.Sequence).ToArray();

        Assert.All(installments, item => Assert.NotNull(item.PrincipalPortion));
        Assert.All(installments, item => Assert.NotNull(item.InterestPortion));
        Assert.Equal(1000m, installments.Sum(x => x.PrincipalPortion!.Value));
        Assert.Equal(100m, installments.Sum(x => x.InterestPortion!.Value));
        Assert.All(installments, item =>
            Assert.Equal(item.Amount.Amount, item.PrincipalPortion! + item.InterestPortion!));
    }

    [Fact]
    public void UnrecordedOpening_LacksTheSourceButStillKnowsItsSplit()
    {
        // İki boşluk bağımsız. Anapara/faiz ayrımı paradan türetilebilir, o
        // yüzden eski sürüm bir yedekten kurulan borç bile doğru ayrımı
        // taşır. Açılışta ne olduğu ise hiçbir sayıdan türetilemez — onu
        // yalnız kullanıcı bilir.
        var debt = UnrecordedDebt();

        Assert.True(debt.HasUnrecordedOpening);
        Assert.Null(debt.OpeningAccountId);
        Assert.Null(debt.CategoryId);
        Assert.Equal(300m, debt.InstallmentSplits.Sum(x => x.Principal));
        Assert.Equal(30m, debt.InstallmentSplits.Sum(x => x.Interest));
    }

    [Fact]
    public void RecordOpening_FillsTheSourceAndKeepsTheSplitConsistent()
    {
        var userId = Guid.NewGuid();
        var debt = UnrecordedDebt(userId);
        var category = ExpenseCategory(userId);

        debt.RecordOpening(DebtSourceType.Expense, null, category);

        Assert.False(debt.HasUnrecordedOpening);
        Assert.Equal(category.Id, debt.CategoryId);
        Assert.All(debt.Installments, item => Assert.NotNull(item.InterestPortion));
        Assert.Equal(300m, debt.InstallmentSplits.Sum(x => x.Principal));
        Assert.Equal(30m, debt.InstallmentSplits.Sum(x => x.Interest));
    }

    // Ayrımı `null` olan taksit yalnız veritabanından materialize edilebilir;
    // Domain'de onu üretecek bir yol yok ve sırf test için bir mutator
    // eklemek modeli kirletirdi. O yüzden geri düşme davranışı gerçek SQL
    // testinde kanıtlanıyor: `LegacyInstallmentWithoutSplit_ContributesNoInterest`.

    [Fact]
    public void RecordOpening_RunsOnlyOnceAndRefusesAnUnrecordedSource()
    {
        var userId = Guid.NewGuid();
        var debt = UnrecordedDebt(userId);

        Assert.Throws<ArgumentException>(
            () => debt.RecordOpening(DebtSourceType.Unrecorded, null, null));

        debt.RecordOpening(DebtSourceType.Cash, ActiveAccount(userId), null);

        // Kaydedilmiş bir açılışı değiştirmek, geçmişte yazılmış gideri ya da
        // bakiye hareketini geriye dönük silmek olurdu.
        Assert.Throws<InvalidOperationException>(
            () => debt.RecordOpening(DebtSourceType.Cash, ActiveAccount(userId), null));
    }

    [Fact]
    public void UnrecordedOpening_CannotBeReachedThroughTheConstructor()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Build(
            userId, DebtDirection.Payable, DebtSourceType.Unrecorded, null, null));
    }

    private static DebtAgreement UnrecordedDebt(Guid? userId = null) =>
        DebtAgreement.WithUnrecordedOpening(
            Guid.NewGuid(), userId ?? Guid.NewGuid(), "Legacy lender", DebtDirection.Payable,
            TransactionScope.Business,
            new Money(300m, CurrencyCode.TRY), new Money(330m, CurrencyCode.TRY),
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 15), 3, "Legacy debt");

    private static Account ActiveAccount(Guid userId) =>
        new(Guid.NewGuid(), userId, "Nakit", AccountType.Cash, CurrencyCode.TRY);

    private static Category ExpenseCategory(Guid userId) =>
        new(Guid.NewGuid(), userId, "Ulaşım", CategoryType.Expense);

    private static Category IncomeCategory(Guid userId) =>
        new(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);

    private static DebtAgreement Build(
        Guid userId,
        DebtDirection direction,
        DebtSourceType sourceType,
        Account? openingAccount,
        Category? category) => new(
        Guid.NewGuid(), userId, "Synthetic lender", direction,
        TransactionScope.Business,
        new Money(300m, CurrencyCode.TRY), new Money(330m, CurrencyCode.TRY),
        sourceType, openingAccount, category,
        new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 15), 3, "Test debt");

    private static DebtAgreement CreateDebt(decimal principal, decimal total, int count, Guid? userId = null)
    {
        var owner = userId ?? Guid.NewGuid();
        return new DebtAgreement(
            Guid.NewGuid(), owner, "Synthetic lender", DebtDirection.Payable,
            TransactionScope.Business,
            new Money(principal, CurrencyCode.TRY), new Money(total, CurrencyCode.TRY),
            DebtSourceType.Cash, ActiveAccount(owner), null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 15), count, "Test debt");
    }
}
