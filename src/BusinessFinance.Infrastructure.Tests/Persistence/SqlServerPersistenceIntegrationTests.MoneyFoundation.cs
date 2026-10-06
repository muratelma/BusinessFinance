using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.Pos;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Categories;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Para tarafının güvence senaryosu: her kayıt türünden iki kapsamda birer
/// örnek içeren tek veri kümesi üzerinde, <b>aynı sayıyı hesaplayan bütün
/// yolların birbirini tuttuğunu</b> doğrular.
/// </summary>
/// <remarks>
/// Tek tek okumaların doğruluğunu kendi testleri tutar. Buradaki soru
/// başkadır: bütçedeki harcama kategori dağılımıyla, eğilimin ayı dönem
/// toplamıyla, rapordaki bakiye hesabın bakiyesiyle aynı mı. Bir kaynak bir
/// yola eklenip ötekinde unutulduğunda kırılan test budur.
/// </remarks>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    private static readonly DateOnly FoundationAsOf = new(2026, 8, 31);

    private static readonly TransactionScope?[] FoundationScopes =
        [null, TransactionScope.Business, TransactionScope.Personal];

    [SqlServerFact]
    public async Task MoneyFoundation_IncomeAndExpenseAgreeAcrossEveryRead()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("money-foundation-recognition@example.test");
        await database.SeedUsersAsync(user);
        var seed = await SeedMoneyFoundationAsync(database, user.Id);

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();

        var monthlyByScope = new Dictionary<int, MonthlyReportDto>();
        foreach (var filter in FoundationScopes)
        {
            var monthly = await reports.GetMonthlyAsync(user.Id, 2026, 8, filter, default);
            var advanced = await reports.GetAdvancedAsync(
                user.Id, 2026, 8, FoundationAsOf, 3, 30, filter, default);
            monthlyByScope[(int?)filter ?? 0] = monthly;

            // Sayıların kendisi: kurgu değişmeden bunlar değişmez.
            Assert.Equal(seed.ExpectedIncome(filter), monthly.TotalIncome);
            Assert.Equal(seed.ExpectedExpense(filter), monthly.TotalExpense);

            // Toplam ile kategori dağılımı aynı gideri anlatır.
            Assert.Equal(monthly.TotalExpense, monthly.CategoryExpenses.Sum(item => item.Amount));

            // Dönem karşılaştırması ve eğilim aynı ayı aynı sayıyla okur.
            Assert.Equal(monthly.TotalIncome, advanced.PeriodComparison.Current.Income);
            Assert.Equal(monthly.TotalExpense, advanced.PeriodComparison.Current.Expense);
            var august = Assert.Single(
                advanced.CashFlowTrend, point => point.Year == 2026 && point.Month == 8);
            Assert.Equal(monthly.TotalIncome, august.Income);
            Assert.Equal(monthly.TotalExpense, august.Expense);
        }

        // İki taraf filtresiz okumayı tam olarak böler; kırılım da aynısını söyler.
        var all = monthlyByScope[0];
        var business = monthlyByScope[(int)TransactionScope.Business];
        var personal = monthlyByScope[(int)TransactionScope.Personal];
        Assert.Equal(all.TotalIncome, business.TotalIncome + personal.TotalIncome);
        Assert.Equal(all.TotalExpense, business.TotalExpense + personal.TotalExpense);
        Assert.NotNull(all.ScopeBreakdown);
        Assert.Equal(business.TotalIncome, all.ScopeBreakdown.Business.Income);
        Assert.Equal(business.TotalExpense, all.ScopeBreakdown.Business.Expense);
        Assert.Equal(personal.TotalIncome, all.ScopeBreakdown.Personal.Income);
        Assert.Equal(personal.TotalExpense, all.ScopeBreakdown.Personal.Expense);

        // Kategori dağılımı kurgunun kalemleriyle birebir.
        Assert.Equal(seed.GoodsBusiness, CategoryAmount(business, seed.GoodsId));
        Assert.Equal(seed.CommissionBusiness, CategoryAmount(business, seed.CommissionId));
        Assert.Equal(seed.InterestBusiness, CategoryAmount(business, seed.InterestId));
        Assert.Equal(seed.FoodPersonal, CategoryAmount(personal, seed.FoodId));
        Assert.Equal(seed.CommissionPersonal, CategoryAmount(personal, seed.CommissionId));
        Assert.Equal(seed.InterestPersonal, CategoryAmount(personal, seed.InterestId));
    }

    [SqlServerFact]
    public async Task MoneyFoundation_BudgetSpendingIsTheCategoryExpenseOfItsScope()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("money-foundation-budget@example.test");
        await database.SeedUsersAsync(user);
        var seed = await SeedMoneyFoundationAsync(database, user.Id);

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var budgets = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();

        var listed = await budgets.ListWithProgressAsync(user.Id, 2026, 8, default);
        var advanced = await reports.GetAdvancedAsync(
            user.Id, 2026, 8, FoundationAsOf, 3, 30, null, default);
        Assert.Equal(4, listed.Count);
        Assert.Equal(4, advanced.BudgetVariances.Count);

        foreach (var budget in listed)
        {
            // Bütçenin harcaması, raporda o kategorinin o kapsamdaki gideridir:
            // tek seferlik borç, POS komisyonu ve borç faizi dahil.
            var monthly = await reports.GetMonthlyAsync(user.Id, 2026, 8, budget.Scope, default);
            var expected = CategoryAmount(monthly, budget.CategoryId);
            Assert.True(expected > 0m, $"{budget.CategoryName}: kurgu harcama üretmeli.");
            Assert.Equal(expected, budget.Spent);

            // Bütçeler ekranı ile rapordaki bütçe sapması aynı sayıyı gösterir.
            var variance = Assert.Single(
                advanced.BudgetVariances, item => item.CategoryId == budget.CategoryId);
            Assert.Equal(expected, variance.Spent);
        }
    }

    [SqlServerFact]
    public async Task MoneyFoundation_BalancesDebtsAndTransitAgreeAcrossEveryRead()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("money-foundation-balances@example.test");
        await database.SeedUsersAsync(user);
        var seed = await SeedMoneyFoundationAsync(database, user.Id);

        await using var services = CreateServiceProvider(database.ConnectionString);
        await using var scope = services.CreateAsyncScope();
        var reports = scope.ServiceProvider.GetRequiredService<IFinancialReportRepository>();
        var accounts = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var dayFlow = scope.ServiceProvider.GetRequiredService<IAccountDayFlowReader>();
        var cards = scope.ServiceProvider.GetRequiredService<ICreditCardRepository>();
        var counterparties = scope.ServiceProvider.GetRequiredService<ICounterpartyRepository>();
        var posSettlements = scope.ServiceProvider.GetRequiredService<IPosSettlementRepository>();

        var monthly = await reports.GetMonthlyAsync(user.Id, 2026, 8, null, default);
        var advanced = await reports.GetAdvancedAsync(
            user.Id, 2026, 8, FoundationAsOf, 3, 30, null, default);

        foreach (var (accountId, opening, expected) in new[]
                 {
                     (seed.BankId, seed.BankOpening, seed.BankBalance),
                     (seed.TillId, seed.TillOpening, seed.TillBalance),
                 })
        {
            // Hesabın bakiyesi, iki rapordaki bakiyesi ve günlerin giren/çıkan
            // toplamı aynı hareket listesini okur.
            var balance = await accounts.CalculateBalanceAsync(accountId, user.Id, default);
            Assert.Equal(expected, balance);
            Assert.Equal(
                balance, Assert.Single(monthly.AccountBalances, item => item.AccountId == accountId).Balance);
            Assert.Equal(
                balance,
                Assert.Single(advanced.AccountDistribution, item => item.AccountId == accountId).Balance);

            var flowed = 0m;
            for (var day = new DateOnly(2026, 8, 1); day <= FoundationAsOf; day = day.AddDays(1))
            {
                var (inflow, outflow) = await dayFlow.CalculateDayFlowAsync(
                    accountId, user.Id, day, default);
                flowed += inflow - outflow;
            }

            Assert.Equal(balance, opening + flowed);
        }

        // Kart borcu: kartın kendi okuması ile rapor.
        var cardDebt = await cards.CalculateCurrentDebtAsync(seed.CardId, user.Id, default);
        Assert.Equal(seed.CardDebt, cardDebt);
        Assert.Equal(cardDebt, Assert.Single(advanced.CardDistribution).Debt);
        Assert.Equal(cardDebt, advanced.NetWorth.CreditCardDebt);

        // Cari: kişi listesindeki açık tutarlar ile net varlıktaki alacak/borç.
        var balances = await counterparties.ListBalancesAsync(
            user.Id, CounterpartyBalanceFilter.All, null, FoundationAsOf, default);
        Assert.Equal(seed.CounterpartyReceivable, balances.Sum(item => item.Receivable));
        Assert.Equal(seed.CounterpartyPayable, balances.Sum(item => item.Payable));
        Assert.Equal(
            seed.DebtReceivablePrincipal + balances.Sum(item => item.Receivable),
            advanced.NetWorth.ReceivableDebt);
        Assert.Equal(
            seed.DebtPayablePrincipal + balances.Sum(item => item.Payable),
            advanced.NetWorth.PayableDebt);

        // Yoldaki para: POS listesi ile net varlık.
        var listedPos = await posSettlements.ListAsync(
            user.Id,
            new PosSettlementListCriteria(false, new DateOnly(2026, 8, 1), FoundationAsOf),
            default);
        Assert.Equal(seed.MoneyInTransit, listedPos.MoneyInTransit);
        Assert.Equal(listedPos.MoneyInTransit, advanced.NetWorth.MoneyInTransit);
    }

    private static decimal CategoryAmount(MonthlyReportDto report, Guid categoryId) =>
        report.CategoryExpenses.SingleOrDefault(item => item.CategoryId == categoryId)?.Amount ?? 0m;

    /// <summary>Kurgunun kimlikleri ve elle hesaplanmış beklenen sayıları.</summary>
    private sealed record MoneyFoundationSeed(
        Guid BankId,
        Guid TillId,
        Guid CardId,
        Guid GoodsId,
        Guid FoodId,
        Guid CommissionId,
        Guid InterestId,
        decimal InterestBusiness,
        decimal InterestPersonal,
        decimal BankBalance,
        decimal DebtReceivablePrincipal,
        decimal DebtPayablePrincipal)
    {
        public decimal BankOpening => 10_000m;
        public decimal TillOpening => 1_000m;

        // Mal alımı: hesaptan 200 + kart 300 + vadeli alım 150 + tek seferlik
        // borç 120 + borçla alınan mal 500.
        public decimal GoodsBusiness => 1_270m;

        // POS komisyonu 20 + yatış kesintisi 5 + kartla tahsilin komisyonu 2.
        public decimal CommissionBusiness => 27m;

        // Yemek: hesaptan 50 + kart 70 + vadeli 30 + tek seferlik borç 25 +
        // borçla alınan 80.
        public decimal FoodPersonal => 255m;
        public decimal CommissionPersonal => 4m;

        // Kasa: açılış 1.000 − gider 50 + transfer 300 + cari tahsilat 250.
        public decimal TillBalance => 1_500m;

        // Kart: harcama 300 + 70 − ödeme 150.
        public decimal CardDebt => 220m;

        // Müşteri: veresiye 400 − nakit tahsilat 250 − kartla tahsil 100 +
        // açık tek seferlik alacak 90.
        public decimal CounterpartyReceivable => 140m;

        // Toptancı: vadeli 150 + 30 − ödeme 100; arkadaş: açık borç 25.
        public decimal CounterpartyPayable => 105m;

        // Şahsi POS satışının neti 196 + kartla tahsilin neti 98.
        public decimal MoneyInTransit => 294m;

        public decimal ExpectedIncome(TransactionScope? scope) => scope switch
        {
            // Satış 1.000 + veresiye 400 + tek seferlik alacak 90 + vadeli
            // satış 600 + POS satışı 1.000.
            TransactionScope.Business => 3_090m,
            TransactionScope.Personal => 200m,
            _ => 3_290m
        };

        public decimal ExpectedExpense(TransactionScope? scope) => scope switch
        {
            TransactionScope.Business => GoodsBusiness + CommissionBusiness + InterestBusiness,
            TransactionScope.Personal => FoodPersonal + CommissionPersonal + InterestPersonal,
            _ => GoodsBusiness + CommissionBusiness + InterestBusiness +
                 FoodPersonal + CommissionPersonal + InterestPersonal
        };
    }

    private static async Task<MoneyFoundationSeed> SeedMoneyFoundationAsync(
        SqlTestDatabase database,
        Guid userId)
    {
        var now = new DateTimeOffset(2026, 8, 20, 9, 0, 0, TimeSpan.Zero);
        static Money Try(decimal amount) => new(amount, CurrencyCode.TRY);
        static DateOnly Day(int day) => new(2026, 8, day);

        var bank = new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 10_000m);
        var till = new Account(Guid.NewGuid(), userId, "Kasa", AccountType.Cash, CurrencyCode.TRY, 1_000m);
        var card = new CreditCard(Guid.NewGuid(), userId, "Kart", Try(20_000m), 15, 25);
        var sales = new Category(Guid.NewGuid(), userId, "Satış", CategoryType.Income);
        var goods = new Category(Guid.NewGuid(), userId, "Mal alımı", CategoryType.Expense);
        var food = new Category(Guid.NewGuid(), userId, "Yemek", CategoryType.Expense);
        var commission = new Category(Guid.NewGuid(), userId, "POS komisyonu", CategoryType.Expense);
        var interest = new Category(
            Guid.NewGuid(), userId, EfCategoryRepository.InterestExpenseCategoryName, CategoryType.Expense);
        var supplier = new Counterparty(Guid.NewGuid(), userId, "Toptancı");
        var customer = new Counterparty(Guid.NewGuid(), userId, "Müşteri");
        var lender = new Counterparty(Guid.NewGuid(), userId, "Banka kredisi");
        var friend = new Counterparty(Guid.NewGuid(), userId, "Arkadaş");

        // Hesaptan gelir ve gider; iptal edilen hiçbir yere girmez.
        var income = new BudgetTransaction(Guid.NewGuid(), userId, bank, sales,
            Try(1_000m), TransactionType.Income, TransactionScope.Business, Day(2));
        var businessExpense = new BudgetTransaction(Guid.NewGuid(), userId, bank, goods,
            Try(200m), TransactionType.Expense, TransactionScope.Business, Day(3));
        var personalExpense = new BudgetTransaction(Guid.NewGuid(), userId, till, food,
            Try(50m), TransactionType.Expense, TransactionScope.Personal, Day(3));
        var cancelledExpense = new BudgetTransaction(Guid.NewGuid(), userId, till, goods,
            Try(40m), TransactionType.Expense, TransactionScope.Business, Day(3));
        cancelledExpense.Cancel(now);

        // Kart harcaması gideri yazar; ödemesi yalnız taşır.
        var businessCharge = new CreditCardCharge(Guid.NewGuid(), userId, card, goods,
            Try(300m), TransactionScope.Business, Day(4));
        var personalCharge = new CreditCardCharge(Guid.NewGuid(), userId, card, food,
            Try(70m), TransactionScope.Personal, Day(4));
        var cardPayment = new CreditCardPayment(Guid.NewGuid(), userId, bank, card, Try(150m), Day(11));

        // Cari: borçlandırma tanır, tahsilat ve ödeme taşır.
        var creditSale = new CounterpartyCharge(Guid.NewGuid(), userId, customer, sales,
            DebtDirection.Receivable, Try(400m), TransactionScope.Business, Day(5));
        var creditPurchase = new CounterpartyCharge(Guid.NewGuid(), userId, supplier, goods,
            DebtDirection.Payable, Try(150m), TransactionScope.Business, Day(5));
        var personalCreditPurchase = new CounterpartyCharge(Guid.NewGuid(), userId, supplier, food,
            DebtDirection.Payable, Try(30m), TransactionScope.Personal, Day(5));
        var cashCollection = new CounterpartyPayment(Guid.NewGuid(), userId, customer, till,
            DebtDirection.Receivable, Try(250m), Day(12));
        var supplierPayment = new CounterpartyPayment(Guid.NewGuid(), userId, supplier, bank,
            DebtDirection.Payable, Try(100m), Day(12));

        // Tek seferlik: biri kapanır (hesaptan çıkar), ikisi açık kalır.
        var settledObligation = new Obligation(Guid.NewGuid(), userId, goods, DebtDirection.Payable,
            Try(120m), TransactionScope.Business, Day(6), Day(20), now, supplier);
        var obligationSettlement = settledObligation.Settle(Guid.NewGuid(), bank, Day(13), now);
        var openPersonalObligation = new Obligation(Guid.NewGuid(), userId, food, DebtDirection.Payable,
            Try(25m), TransactionScope.Personal, Day(6), Day(25), now, friend);
        var openReceivableObligation = new Obligation(Guid.NewGuid(), userId, sales,
            DebtDirection.Receivable, Try(90m), TransactionScope.Business, Day(6), Day(25), now, customer);

        // Borç açılışı: mal ya da satış karşılığı olan o gün tanır; nakit olan
        // yalnız taşır ve faizi ödendikçe gider yazar.
        var goodsOnCredit = new DebtAgreement(Guid.NewGuid(), userId, supplier, DebtDirection.Payable,
            TransactionScope.Business, Try(500m), Try(500m), DebtSourceType.Expense, null, goods,
            Day(7), new DateOnly(2026, 9, 7), 1);
        var foodOnCredit = new DebtAgreement(Guid.NewGuid(), userId, friend, DebtDirection.Payable,
            TransactionScope.Personal, Try(80m), Try(80m), DebtSourceType.Expense, null, food,
            Day(7), new DateOnly(2026, 9, 7), 1);
        var saleOnCredit = new DebtAgreement(Guid.NewGuid(), userId, customer, DebtDirection.Receivable,
            TransactionScope.Business, Try(600m), Try(600m), DebtSourceType.Income, null, sales,
            Day(7), new DateOnly(2026, 9, 7), 1);
        var personalLoan = new DebtAgreement(Guid.NewGuid(), userId, lender, DebtDirection.Payable,
            TransactionScope.Personal, Try(1_000m), Try(1_100m), DebtSourceType.Cash, bank, null,
            Day(1), Day(10), 2);
        personalLoan.GetInstallment(1).MarkPaid(bank, Day(10), now);
        var businessLoan = new DebtAgreement(Guid.NewGuid(), userId, lender, DebtDirection.Payable,
            TransactionScope.Business, Try(2_000m), Try(2_400m), DebtSourceType.Cash, bank, null,
            Day(1), Day(15), 2);
        businessLoan.GetInstallment(1).MarkPaid(bank, Day(15), now);

        // POS: satış tahsil edildiği gün brüt gelir ve komisyon gideri yazar;
        // para hesaba yatışla ve net geçer, eksik yatan kısım kesintidir.
        var depositedSale = new PosSettlement(Guid.NewGuid(), userId, bank, sales, Try(1_000m), 20m,
            TransactionScope.Business, Day(8), Day(10), now, commission);
        var deduction = new BudgetTransaction(Guid.NewGuid(), userId, bank, commission,
            Try(5m), TransactionType.Expense, TransactionScope.Business, Day(10));
        var deposit = PosDeposit.Record(Guid.NewGuid(), userId, bank, [depositedSale],
            Try(975m), Day(10), now, deduction);
        var personalSaleInTransit = new PosSettlement(Guid.NewGuid(), userId, bank, sales, Try(200m), 4m,
            TransactionScope.Personal, Day(9), Day(20), now, commission);

        // Kartla tahsil: gelir yazmaz, cariyi kapatır; parası yoldadır ve
        // hesaba yatışla girer. Komisyonu giderdir.
        var cardCollection = PosSettlement.Collect(Guid.NewGuid(), userId, bank, Try(100m), 2m,
            TransactionScope.Business, Day(11), Day(14), now, commission);
        var cardCollectionPayment = new CounterpartyPayment(Guid.NewGuid(), userId, customer, bank,
            DebtDirection.Receivable, Try(100m), Day(11), null, cardCollection);

        var transfer = new Transfer(Guid.NewGuid(), userId, bank, till, Try(300m), Day(9));

        var budgets = new[]
        {
            new MonthlyBudget(Guid.NewGuid(), userId, goods, Try(5_000m), TransactionScope.Business, 2026, 8),
            new MonthlyBudget(Guid.NewGuid(), userId, commission, Try(100m), TransactionScope.Business, 2026, 8),
            new MonthlyBudget(Guid.NewGuid(), userId, interest, Try(500m), TransactionScope.Personal, 2026, 8),
            new MonthlyBudget(Guid.NewGuid(), userId, food, Try(300m), TransactionScope.Personal, 2026, 8),
        };

        await using (var context = database.CreateContext())
        {
            context.AddRange(bank, till, card, sales, goods, food, commission, interest);
            context.AddRange(supplier, customer, lender, friend);
            context.AddRange(income, businessExpense, personalExpense, cancelledExpense);
            context.AddRange(businessCharge, personalCharge, cardPayment);
            context.AddRange(creditSale, creditPurchase, personalCreditPurchase);
            context.AddRange(cashCollection, supplierPayment);
            context.AddRange(settledObligation, obligationSettlement);
            context.AddRange(openPersonalObligation, openReceivableObligation);
            context.AddRange(goodsOnCredit, foodOnCredit, saleOnCredit, personalLoan, businessLoan);
            context.AddRange(depositedSale, deduction, deposit, personalSaleInTransit);
            context.AddRange(cardCollection, cardCollectionPayment, transfer);
            context.AddRange(budgets.Cast<object>().ToArray());
            await context.SaveChangesAsync(CancellationToken.None);
        }

        static decimal RemainingPrincipal(params DebtAgreement[] debts) => debts
            .SelectMany(debt => debt.Installments)
            .Where(installment => installment.PaymentDate is null)
            .Sum(installment => installment.PrincipalPortion ?? installment.Amount.Amount);

        var personalInstallment = personalLoan.GetInstallment(1);
        var businessInstallment = businessLoan.GetInstallment(1);

        // Banka: açılış 10.000 + satış 1.000 − gider 200 + iki kredinin
        // açılışı 3.000 − iki taksit + yatışın neti 980 − kesinti 5 − transfer
        // 300 − kart ödemesi 150 − toptancı ödemesi 100 − tek seferlik borç 120.
        var bankBalance = 10_000m + 1_000m - 200m + 1_000m + 2_000m
            - personalInstallment.Amount.Amount - businessInstallment.Amount.Amount
            + 980m - 5m - 300m - 150m - 100m - 120m;

        return new MoneyFoundationSeed(
            bank.Id,
            till.Id,
            card.Id,
            goods.Id,
            food.Id,
            commission.Id,
            interest.Id,
            businessInstallment.InterestPortion!.Value,
            personalInstallment.InterestPortion!.Value,
            bankBalance,
            RemainingPrincipal(saleOnCredit),
            RemainingPrincipal(goodsOnCredit, foodOnCredit, personalLoan, businessLoan));
    }
}
