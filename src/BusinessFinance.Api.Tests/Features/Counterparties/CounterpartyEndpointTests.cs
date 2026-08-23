using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Counterparties;

public sealed class CounterpartyEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Aşama 02'nin çıkış senaryosu: bir müşteriye üç satış, iki kısmi tahsilat.
    /// Cari bakiye, gelir raporu ve kasa birbirini tutmalı ve hiçbir tutar iki
    /// kez sayılmamalı.
    /// </summary>
    [Fact]
    public async Task CreditSalesAndPartialCollections_AgreeAcrossBalanceReportAndCash()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cari-owner@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var incomeCategory = await FirstCategoryAsync(owner, "income");

        var shopkeeper = await CreateCounterpartyAsync(owner, "Ahmet Bakkal");
        Assert.Equal("0.0000", shopkeeper.Net);
        Assert.True(shopkeeper.IsSettled);

        // Üç veresiye satış: gelir bugün yazılır, kasa kıpırdamaz.
        foreach (var amount in new[] { "300.0000", "250.0000", "150.0000" })
        {
            using var sale = await owner.PostAsJsonAsync(
                $"/api/v1/counterparties/{shopkeeper.Id}/charges",
                new CreateCounterpartyChargeRequest(
                    "receivable", amount, "TRY", incomeCategory.Id, "2026-08-05", "business"));
            Assert.Equal(HttpStatusCode.Created, sale.StatusCode);
        }

        var afterSales = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("700.0000", afterSales!.TotalIncome);
        Assert.Equal("1000.0000", Assert.Single(afterSales.AccountBalances).Balance);

        // İki kısmi tahsilat: kasa değişir, gelir değişmez.
        var firstCollection = await CollectAsync(owner, shopkeeper.Id, account.Id, "200.0000");
        await CollectAsync(owner, shopkeeper.Id, account.Id, "100.0000");

        var afterCollections = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("700.0000", afterCollections!.TotalIncome);
        Assert.Equal("0.0000", afterCollections.TotalExpense);
        Assert.Equal("1300.0000", Assert.Single(afterCollections.AccountBalances).Balance);

        var balance = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{shopkeeper.Id}");
        Assert.Equal("400.0000", balance!.Receivable);
        Assert.Equal("400.0000", balance.Net);
        Assert.False(balance.IsSettled);

        // Feed beş boyutu dolduruyor ve iki tür ayrı sınıflanıyor.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=50");
        var charges = feed!.Items
            .Where(item => item.ActivityKind == "counterparty-charge")
            .ToArray();
        var settlements = feed.Items
            .Where(item => item.ActivityKind == "counterparty-settlement")
            .ToArray();
        Assert.Equal(3, charges.Length);
        Assert.Equal(2, settlements.Length);
        Assert.All(charges, item =>
        {
            Assert.Equal("income", item.Effect);
            Assert.Equal("counterparty", item.SourceGroup);
            Assert.Equal("business", item.Scope);
            Assert.True(item.CanCancel);
        });
        Assert.All(settlements, item =>
        {
            // Tahsilat ikinci bir gelir değil; kapsam da taşımaz.
            Assert.Equal("neutral", item.Effect);
            Assert.Null(item.Scope);
            Assert.True(item.CanCancel);
        });

        // Tahsilatı iptal etmek parayı kasadan geri alır ve açık bakiyeyi
        // yeniden doğurur; geliri değiştirmez.
        using var cancel = await owner.DeleteAsync(
            $"/api/v1/counterparty-payments/{firstCollection.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);

        var afterCancel = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("700.0000", afterCancel!.TotalIncome);
        Assert.Equal("1100.0000", Assert.Single(afterCancel.AccountBalances).Balance);
        var reopened = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{shopkeeper.Id}");
        Assert.Equal("600.0000", reopened!.Receivable);
    }

    /// <summary>
    /// Vadeli alım gideri o gün yazar ve bütçeyi tüketir; ödeme yalnız kasayı
    /// azaltır.
    /// </summary>
    [Fact]
    public async Task CreditPurchase_RecognisesExpenseAndPaymentOnlyMovesCash()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cari-supplier@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var expenseCategory = await FirstCategoryAsync(owner, "expense");
        var supplier = await CreateCounterpartyAsync(owner, "Toptancı Zeynep");

        using var purchase = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "400.0000", "TRY", expenseCategory.Id, "2026-08-05", "business"));
        Assert.Equal(HttpStatusCode.Created, purchase.StatusCode);

        // Yön kategorinin türünü belirler: borç doğuran kayıt gelir
        // kategorisiyle yazılamaz.
        var incomeCategory = await FirstCategoryAsync(owner, "income");
        using var wrongCategory = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "50.0000", "TRY", incomeCategory.Id, "2026-08-05", "business"));
        Assert.Equal(HttpStatusCode.BadRequest, wrongCategory.StatusCode);

        using var pay = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "payable", "150.0000", "TRY", account.Id, "2026-08-06"));
        Assert.Equal(HttpStatusCode.Created, pay.StatusCode);

        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("400.0000", report!.TotalExpense);
        Assert.Equal("850.0000", Assert.Single(report.AccountBalances).Balance);

        // Gider dağılımı toplamla tutuyor: vadeli alım kendi kategorisinde.
        Assert.Equal(
            decimal.Parse(report.TotalExpense, CultureInfo.InvariantCulture),
            FinanceSum(report.CategoryExpenses.Select(item => item.Amount)));

        var balance = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{supplier.Id}");
        Assert.Equal("250.0000", balance!.Payable);
        Assert.Equal("-250.0000", balance.Net);
    }

    /// <summary>
    /// Pasif karşı taraf yeni borçlandırma almaz ama kalan borcunu ödeyebilir;
    /// hareketi olan kayıt silinmez.
    /// </summary>
    [Fact]
    public async Task DeactivatedCounterparty_TakesNoNewChargeButStillSettlesAndIsNotDeleted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cari-inactive@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "500");
        var incomeCategory = await FirstCategoryAsync(owner, "income");
        var customer = await CreateCounterpartyAsync(owner, "Eski Müşteri");

        using var sale = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "200.0000", "TRY", incomeCategory.Id, "2026-08-05", "business"));
        Assert.Equal(HttpStatusCode.Created, sale.StatusCode);

        using var deactivate = await owner.PutAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}",
            new UpdateCounterpartyRequest("Eski Müşteri", false, "Artık çalışmıyoruz"));
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        using var newCharge = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "50.0000", "TRY", incomeCategory.Id, "2026-08-06", "business"));
        Assert.Equal(HttpStatusCode.Conflict, newCharge.StatusCode);

        // Kalan borç tahsil edilebilmeli; aksi hâlde bakiye kapanamazdı.
        using var collect = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "200.0000", "TRY", account.Id, "2026-08-07"));
        Assert.Equal(HttpStatusCode.Created, collect.StatusCode);

        // Hareketi olan karşı taraf silinmez; boş hesap kuralının aynısı.
        using var delete = await owner.DeleteAsync($"/api/v1/counterparties/{customer.Id}");
        Assert.Equal(HttpStatusCode.Conflict, delete.StatusCode);

        // Kapanmış cari listede kalır ama ayrı okunur.
        var settled = await owner.GetFromJsonAsync<CounterpartyListResponse>(
            "/api/v1/counterparties?balance=settled");
        Assert.Equal(customer.Id, Assert.Single(settled!.Items).Id);
        var open = await owner.GetFromJsonAsync<CounterpartyListResponse>(
            "/api/v1/counterparties?balance=open");
        Assert.Empty(open!.Items);

        // Hiç hareketi olmayan karşı taraf silinebilir.
        var unused = await CreateCounterpartyAsync(owner, "Hiç Alışveriş Yok");
        using var deleteUnused = await owner.DeleteAsync($"/api/v1/counterparties/{unused.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteUnused.StatusCode);
    }

    /// <summary>
    /// Başka kullanıcının karşı tarafı ve hareketleri hiçbir uçtan görünmez.
    /// </summary>
    [Fact]
    public async Task AnotherUsersLedger_IsNeitherReadableNorWritable()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cari-isolation@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "cari-isolation-other@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "500");
        var incomeCategory = await FirstCategoryAsync(owner, "income");
        var customer = await CreateCounterpartyAsync(owner, "Ortak Ad");

        using var sale = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "120.0000", "TRY", incomeCategory.Id, "2026-08-05", "business"));
        var charge = (await sale.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;

        // Aynı ad yabancıda ayrı bir karşı taraf; teklik kullanıcı içindedir.
        var strangerCounterparty = await CreateCounterpartyAsync(stranger, "Ortak Ad");
        Assert.NotEqual(customer.Id, strangerCounterparty.Id);

        Assert.Equal(
            HttpStatusCode.NotFound,
            (await stranger.GetAsync($"/api/v1/counterparties/{customer.Id}")).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await stranger.DeleteAsync($"/api/v1/counterparty-charges/{charge.Id}")).StatusCode);

        using var foreignCollect = await stranger.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "10.0000", "TRY", account.Id, "2026-08-06"));
        Assert.Equal(HttpStatusCode.NotFound, foreignCollect.StatusCode);

        var strangerList = await stranger.GetFromJsonAsync<CounterpartyListResponse>(
            "/api/v1/counterparties");
        Assert.Equal(strangerCounterparty.Id, Assert.Single(strangerList!.Items).Id);

        // Sahibinin borçlandırması yerinde ve iptal edilebilir durumda.
        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-charges/{charge.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        var cancelled = (await cancel.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
        Assert.True(cancelled.IsCancelled);

        // İptal tanınan geliri de geri alır.
        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("0.0000", report!.TotalIncome);
    }

    private static decimal FinanceSum(IEnumerable<string> amounts) =>
        amounts.Sum(value => decimal.Parse(value, CultureInfo.InvariantCulture));

    private static async Task<CounterpartyPaymentResponse> CollectAsync(
        HttpClient client,
        Guid counterpartyId,
        Guid accountId,
        string amount)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/counterparties/{counterpartyId}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", amount, "TRY", accountId, "2026-08-06"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!;
    }

    private static async Task<CounterpartyResponse> CreateCounterpartyAsync(
        HttpClient client,
        string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest(name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyResponse>())!;
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, "cash", "TRY", openingBalance));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
