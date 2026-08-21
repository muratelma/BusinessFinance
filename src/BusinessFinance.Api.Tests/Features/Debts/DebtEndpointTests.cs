using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.UpcomingPayments;

namespace BusinessFinance.Api.Tests.Features.Debts;

public sealed class DebtEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task PayableDebt_PaymentChangesLiquidityButNotMonthlyExpense()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "debt-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "debt-other@example.test");
        var account = await CreateAccountAsync(owner);

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Synthetic Lender", "payable", "1000.0000", "1200.0000", null, "TRY",
                "cash", account.Id, null,
                "2026-07-01", "2026-08-10", 3, "Test debt", "2026-08-11"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var debt = (await create.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal(["400.0000", "400.0000", "400.0000"],
            debt.Installments.Select(item => item.Amount));
        Assert.Equal("overdue", debt.Installments[0].Status);

        // Anüite: faiz azalan bakiyeye işler, yani ilk taksit en çok faizi,
        // son taksit en çok anaparayı taşır.
        Assert.Equal("302.9897", debt.Installments[0].PrincipalPortion);
        Assert.Equal("97.0103", debt.Installments[0].InterestPortion);
        Assert.Equal("364.6274", debt.Installments[2].PrincipalPortion);
        Assert.Equal("35.3726", debt.Installments[2].InterestPortion);
        Assert.Equal("200.0000", debt.TotalInterest);

        var foreignList = await other.GetFromJsonAsync<DebtListResponse>(
            "/api/v1/debts?asOfDate=2026-08-11");
        Assert.Empty(foreignList!.Items);
        using var foreignPay = await other.PostAsJsonAsync(
            $"/api/v1/debts/{debt.Id}/installments/1/pay",
            new PayDebtInstallmentRequest(account.Id, "2026-08-11", "2026-08-11"));
        Assert.Equal(HttpStatusCode.NotFound, foreignPay.StatusCode);

        using var pay = await owner.PostAsJsonAsync(
            $"/api/v1/debts/{debt.Id}/installments/1/pay",
            new PayDebtInstallmentRequest(account.Id, "2026-08-11", "2026-08-11"));
        pay.EnsureSuccessStatusCode();
        var paidDebt = (await pay.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("paid", paidDebt.Installments[0].Status);
        // Sözleşmenin `RemainingAmount`'ı hâlâ kalan **ödemelerin** toplamı:
        // "daha ne kadar ödeyeceğim" sorusunun yanıtı budur ve iki taksit ×
        // 400 eder. Net varlıktaki borç bundan ayrı ölçülür (aşağıda).
        Assert.Equal("800.0000", paidDebt.RemainingAmount);

        // Hesap 1000 ile açıldı, borç 1000 TL nakit getirdi, bir taksitte 400
        // çıktı: 1600. Açılış hareketi eklenmeden önce burada 600 yazıyordu —
        // yani 1000 TL hiç girmemiş, ama taksitler yine de çıkıyordu.
        var accounts = await owner.GetFromJsonAsync<AccountListResponse>("/api/v1/accounts");
        Assert.Equal("1600.0000", Assert.Single(accounts!.Items).Balance);
        var report = await owner.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-11&trendMonths=2&daysAhead=30");

        // Anapara gider değil, faiz giderdir. 1000 anaparaya 1200 geri ödeme,
        // üç eşit taksit: ilk taksitin 302,9897'si anapara, 97,0103'ü faiz.
        // Ağustos gideri yalnız o faizdir — 400'ün tamamı yazılsaydı borç
        // alınan para gider sayılır ve net durum olduğundan kötü görünürdü.
        Assert.Equal("97.0103", report!.PeriodComparison.Current.Expense);
        Assert.Equal("1600.0000", report.NetWorth.LiquidAssets);

        // Net varlıktaki borç **kalan anaparadır**, kalan ödemelerin toplamı
        // değil: 1000 anaparanın 302,9897'si ödendi, 697,0103 duruyor.
        //
        // Burada eskiden 800 yazıyordu ve net varlık 800 çıkıyordu. O sayı
        // kullanıcıyı 102,9897 TL fazla fakir gösteriyordu — tam olarak henüz
        // ödenmemiş faiz kadar. Gelecekteki faiz doğmamış bir yükümlülüktür;
        // ödendiği ay hem gidere yazılır hem borcu azaltır, iki kez
        // düşülemez.
        Assert.Equal("697.0103", report.NetWorth.PayableDebt);

        // Aldığı parayı henüz harcamadıysa net durumu yalnız **tahakkuk eden
        // faiz** kadar bozulmalı: kendi parası 1000, ödediği faiz 97,0103,
        // kalan 902,9897. Gelir tablosunun söylediğiyle birebir aynı sayı —
        // bilanço ile gider raporu ancak böyle tutuyor.
        Assert.Equal("902.9897", report.NetWorth.NetWorth);
        Assert.Equal("400.0000", report.FutureLoad.DebtInstallmentAmount);

        var upcoming = await owner.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-11&daysAhead=30");
        var debtPayment = Assert.Single(upcoming!.Items);
        Assert.Equal("debt-installment", debtPayment.SourceType);
        Assert.Equal("400.0000", debtPayment.Amount);
        Assert.Equal("2026-09-10", debtPayment.DueDate);
    }

    [Fact]
    public async Task DebtEndpoints_RequireAuthenticationAndValidateContract()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await anonymous.GetAsync("/api/v1/debts?asOfDate=2026-08-11")).StatusCode);

        using var owner = await CreateAuthenticatedClientAsync(factory, "debt-validation@example.test");
        var account = await CreateAccountAsync(owner);

        using var invalid = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "expense", "100.0000", "120.0000", null, "TRY",
                "cash", account.Id, null,
                "2026-08-01", "2026-09-01", 1, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        // Ne toplam ne oran: borcun ne kadara mal olacağı hiç belli değil.
        using var neither = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "payable", "100.0000", null, null, "TRY",
                "cash", account.Id, null,
                "2026-08-01", "2026-09-01", 1, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.BadRequest, neither.StatusCode);

        // `unrecorded` dışarıdan gönderilemez; o durum yalnız migration ve
        // eski sürüm yedeklerin ürettiği bir geçmiş kalıntısıdır.
        using var unrecorded = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "payable", "100.0000", "120.0000", null, "TRY",
                "unrecorded", null, null,
                "2026-08-01", "2026-09-01", 1, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.BadRequest, unrecorded.StatusCode);
    }

    [Fact]
    public async Task DebtSource_MustBeExactlyOneAndReceivableCannotBeAnExpense()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "debt-source@example.test");
        var account = await CreateAccountAsync(owner);
        var categories = await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        var expenseCategory = categories!.Items[0];

        // Nakit kaynağı hesap ister, kategori istemez.
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("payable", "cash", account.Id, expenseCategory.Id))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("payable", "cash", null, null))).StatusCode);

        // Gider kaynağı kategori ister, hesap istemez.
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("payable", "expense", account.Id, null))).StatusCode);

        // Alacakta gider kaynağı yok: gideri iki kez saydırırdı.
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("receivable", "expense", null, expenseCategory.Id))).StatusCode);

        using var valid = await owner.PostAsJsonAsync(
            "/api/v1/debts", Debt("payable", "expense", null, expenseCategory.Id));
        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
        var debt = (await valid.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("expense", debt.SourceType);
        Assert.Equal(expenseCategory.Id, debt.CategoryId);
        Assert.Null(debt.OpeningAccountId);

        static CreateDebtRequest Debt(string direction, string source, Guid? accountId, Guid? categoryId) =>
            new("Lender", direction, "300.0000", "330.0000", null, "TRY",
                source, accountId, categoryId,
                "2026-08-01", "2026-09-01", 3, null, "2026-08-11");
    }

    [Fact]
    public async Task IncomeSourcedReceivable_BooksTheSaleAsIncomeAndLeavesTheAccountAlone()
    {
        // "Telefonumu 5.000'e sattım, iki taksitte ödeyecek." Gelir kaynağı
        // eklenmeden önce bu ancak nakit alacak olarak girilebiliyordu: hesap
        // 5.000 azalmış gibi görünüyor, satış da hiç gelir olmuyordu.
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "sale-owner@example.test");
        var account = await CreateAccountAsync(owner);
        var categories = await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income");
        var incomeCategory = categories!.Items[0];

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Alıcı", "receivable", "5000.0000", "5000.0000", null, "TRY",
                "income", null, incomeCategory.Id,
                "2026-08-01", "2026-08-15", 2, "Telefon satışı", "2026-08-11"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var debt = (await create.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("income", debt.SourceType);
        Assert.Equal(incomeCategory.Id, debt.CategoryId);

        var report = await owner.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-11&trendMonths=2&daysAhead=30");

        // Satış geliri açılış günü yazıldı; hesaba henüz para girmedi.
        Assert.Equal("5000.0000", report!.PeriodComparison.Current.Income);
        Assert.Equal("0.0000", report.PeriodComparison.Current.Expense);
        var accounts = await owner.GetFromJsonAsync<AccountListResponse>("/api/v1/accounts");
        Assert.Equal("1000.0000", Assert.Single(accounts!.Items).Balance);

        // Tahsilat parayı getirir ama geliri tekrar yazmaz.
        using var collect = await owner.PostAsJsonAsync(
            $"/api/v1/debts/{debt.Id}/installments/1/pay",
            new PayDebtInstallmentRequest(account.Id, "2026-08-15", "2026-08-15"));
        collect.EnsureSuccessStatusCode();

        var afterAccounts = await owner.GetFromJsonAsync<AccountListResponse>("/api/v1/accounts");
        Assert.Equal("3500.0000", Assert.Single(afterAccounts!.Items).Balance);
        var afterReport = await owner.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-15&trendMonths=2&daysAhead=30");
        Assert.Equal("5000.0000", afterReport!.PeriodComparison.Current.Income);
    }

    [Fact]
    public async Task CategoricalSource_MustMatchTheDirection()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "source-match@example.test");
        var expense = (await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense"))!.Items[0];
        var income = (await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income"))!.Items[0];

        // Alacak satar, borç tüketir; ters eşleşme parayı yanlış tarafa yazardı.
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("payable", "income", income.Id))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("receivable", "expense", expense.Id))).StatusCode);

        // Doğru yön, yanlış kategori tipi.
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync(
            "/api/v1/debts",
            Debt("receivable", "income", expense.Id))).StatusCode);

        static CreateDebtRequest Debt(string direction, string source, Guid categoryId) =>
            new("Karşı taraf", direction, "300.0000", "330.0000", null, "TRY",
                source, null, categoryId,
                "2026-08-01", "2026-09-01", 3, null, "2026-08-11");
    }

    [Fact]
    public async Task InterestRate_AndTotalRepayment_AreTwoWaysToSayTheSameThing()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "debt-interest@example.test");
        var account = await CreateAccountAsync(owner);

        // Toplamı veren kullanıcı oranı öğrenir.
        using var fromTotal = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "payable", "300.0000", "400.0000", null, "TRY",
                "cash", account.Id, null,
                "2026-08-01", "2026-09-01", 4, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.Created, fromTotal.StatusCode);
        var byTotal = (await fromTotal.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("151.0780", byTotal.AnnualInterestRate);
        Assert.Equal("400.0000", byTotal.TotalRepayment);
        Assert.Equal("100.0000", byTotal.TotalInterest);

        // Oranı veren kullanıcı toplamı öğrenir.
        using var fromRate = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "payable", "300.0000", null, "10.0000", "TRY",
                "cash", account.Id, null,
                "2026-08-01", "2026-09-01", 3, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.Created, fromRate.StatusCode);
        var byRate = (await fromRate.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("305.0138", byRate.TotalRepayment);

        // İkisi birden gelip çelişirse sessizce biri seçilmez.
        using var conflict = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Lender", "payable", "300.0000", "400.0000", "10.0000", "TRY",
                "cash", account.Id, null,
                "2026-08-01", "2026-09-01", 3, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.BadRequest, conflict.StatusCode);
    }

    [Fact]
    public async Task ReceivableDebt_AcceptsZeroInterestAndEqualRepayment()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "receivable-owner@example.test");
        var account = await CreateAccountAsync(owner);

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/debts",
            new CreateDebtRequest(
                "Synthetic Borrower", "receivable", "60.0000", "60.0000", null, "TRY",
                "cash", account.Id, null,
                "2026-08-13", "2026-08-13", 2, null, "2026-08-13"));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var debt = (await create.Content.ReadFromJsonAsync<DebtResponse>())!;
        Assert.Equal("receivable", debt.Direction);
        Assert.Equal(["30.0000", "30.0000"], debt.Installments.Select(item => item.Amount));
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Debt Account", "bank", "TRY", "1000.0000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }
}
