using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Pos;

/// <summary>
/// Kartla tahsil (ADR 0019 T5, Aşama 06.3 Grup 5 teslim 3/3): müşteri veresiye
/// borcunu POS'tan kartla öder. Cari bugün brüt tutarla kapanır, gelir ikinci
/// kez yazılmaz, para yola çıkar ve hesaba satışlarla aynı yatışla geçer.
/// </summary>
public sealed class CardCollectionEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Bütün yol tek testte: tanıma (veresiye), taşıma (yola çıkış ve yatış),
    /// komisyon gideri ve İ6 kapısı (kullanılabilir bakiye ile net varlığın
    /// farkı tam olarak yoldaki tutardır).
    /// </summary>
    [Fact]
    public async Task CardCollection_ClosesTheReceivable_WritesNoIncome_AndReachesTheAccountByDeposit()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "card-collection@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var customer = await CreateCounterpartyAsync(owner, "Ahmet Bakkal");

        // Veresiye satış: gelir bugün tanınır.
        using (var sale = await owner.PostAsJsonAsync(
                   $"/api/v1/counterparties/{customer.Id}/charges",
                   new CreateCounterpartyChargeRequest(
                       "receivable", "1000.0000", "TRY", f.SalesCategoryId, Date(today), "business")))
        {
            Assert.Equal(HttpStatusCode.Created, sale.StatusCode);
        }

        var before = await AdvancedAsync(owner, today);
        Assert.Equal("1000.0000", before.NetWorth.ReceivableDebt);

        // Kartla tahsil: POS'tan hesap, oran ve beklenen gün dolar.
        using var collect = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "1000.0000", "TRY", null, Date(today),
                Card: new CardCollectionRequest(PosDefinitionId: f.PosDefinitionId)));
        Assert.True(
            collect.StatusCode == HttpStatusCode.Created,
            await collect.Content.ReadAsStringAsync());
        var payment = (await collect.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!;
        Assert.Equal(f.AccountId, payment.AccountId);
        Assert.NotNull(payment.PosSettlementId);

        // Cari brüt tutarla kapandı.
        var closed = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{customer.Id}");
        Assert.Equal("0.0000", closed!.Receivable);

        // Hesap kıpırdamadı: para yolda, net (1000 − %1,5).
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));
        var settlements = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        var inTransit = Assert.Single(settlements!.Items);
        Assert.Equal(payment.PosSettlementId, inTransit.Id);
        Assert.Equal("collection", inTransit.Kind);
        Assert.Null(inTransit.CategoryId);
        Assert.Null(inTransit.CategoryName);
        Assert.Equal("Ahmet Bakkal", inTransit.CounterpartyName);
        Assert.Equal("15.0000", inTransit.CommissionAmount);
        Assert.Equal("985.0000", inTransit.NetAmount);
        Assert.Equal("business", inTransit.Scope);
        Assert.Equal("985.0000", settlements.MoneyInTransit);

        // Gelir yalnız veresiye satıştır; komisyon gider olarak yazıldı.
        var report = await ReportAsync(owner, today);
        Assert.Equal(1000m, report.Income);
        Assert.Equal(15m, report.Expense);

        // İ6: alacak yoldaki paraya döndü, aradaki fark komisyon kadardır.
        var during = await AdvancedAsync(owner, today);
        Assert.Equal("0.0000", during.NetWorth.ReceivableDebt);
        Assert.Equal("985.0000", during.NetWorth.MoneyInTransit);
        Assert.Equal("1000.0000", during.NetWorth.LiquidAssets);
        Assert.Equal(
            Parse(during.NetWorth.LiquidAssets) + Parse(during.NetWorth.MoneyInTransit),
            Parse(during.NetWorth.NetWorth));

        // İşlemler: tek satır, cari tahsilatın satırı; POS satışı satırı yok.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=50");
        Assert.DoesNotContain(feed!.Items, item => item.ActivityKind == "pos-sale");
        var row = Assert.Single(feed.Items, item => item.ActivityKind == "counterparty-settlement");
        Assert.Equal("1000.0000", row.Amount);
        Assert.Equal("15.0000", row.FeeAmount);
        Assert.Equal("985.0000", row.NetAmount);
        Assert.Equal("Sentetik POS", row.ChannelName);
        Assert.Null(row.TransferredOn);
        Assert.NotNull(row.ExpectedTransferDate);
        Assert.True(row.CanCancel);

        // İşlem sonrası: hesap "değişmedi" (para yolda), cari kapandı.
        var after = await owner.GetFromJsonAsync<ActivityBalanceListResponse>(
            $"/api/v1/financial-activities/counterparty-settlement/{payment.Id}/balances");
        var accountAfter = Assert.Single(after!.Items, item => item.Holder == "account");
        Assert.Equal("unchanged", accountAfter.Change);
        Assert.Equal("1000.0000", accountAfter.Balance);

        // Kasa'dan tek başına iptal edilemez: tahsilatın parçasıdır.
        using (var cancelFromPos = await owner.DeleteAsync(
                   $"/api/v1/pos-settlements/{payment.PosSettlementId}"))
        {
            Assert.Equal(HttpStatusCode.Conflict, cancelFromPos.StatusCode);
            Assert.Equal("pos_settlements.collection_locked", await CodeAsync(cancelFromPos));
        }

        // Yatış satışla aynı yoldan kapatır; hesaba net girer, gelir yazılmaz.
        var deposit = await DepositAsync(owner, [inTransit.Id], "985.0000", today);
        Assert.Equal("1000.0000", deposit.GrossAmount);
        Assert.Equal("1000.0000", deposit.CollectionAmount);
        Assert.Equal("0.0000", deposit.SaleAmount);
        Assert.Equal("Ahmet Bakkal", Assert.Single(deposit.Settlements).CounterpartyName);
        Assert.Equal("1985.0000", await BalanceAsync(owner, f.AccountId));
        var afterDeposit = await ReportAsync(owner, today);
        Assert.Equal(1000m, afterDeposit.Income);

        // Para hesaba geçtiyse tahsilat önce yatış geri alınmadan iptal edilemez.
        var depositedFeed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=50");
        Assert.False(Assert.Single(
            depositedFeed!.Items,
            item => item.ActivityKind == "counterparty-settlement").CanCancel);
        using (var locked = await owner.DeleteAsync($"/api/v1/counterparty-payments/{payment.Id}"))
        {
            Assert.Equal(HttpStatusCode.Conflict, locked.StatusCode);
            Assert.Equal("counterparty_payments.deposit_locked", await CodeAsync(locked));
        }

        // Yatış geri alınınca tahsilat iptal edilebilir: cari yeniden açılır,
        // yoldaki para ve komisyon düşer.
        using (var revert = await owner.DeleteAsync($"/api/v1/pos-deposits/{deposit.Id}"))
        {
            Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        }

        using (var cancel = await owner.DeleteAsync($"/api/v1/counterparty-payments/{payment.Id}"))
        {
            Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        }

        var reopened = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{customer.Id}");
        Assert.Equal("1000.0000", reopened!.Receivable);
        var end = await AdvancedAsync(owner, today);
        Assert.Equal("0.0000", end.NetWorth.MoneyInTransit);
        Assert.Equal("1000.0000", end.NetWorth.LiquidAssets);
        var endReport = await ReportAsync(owner, today);
        Assert.Equal(1000m, endReport.Income);
        Assert.Equal(0m, endReport.Expense);
    }

    /// <summary>
    /// Kartla yalnız tahsilat alınır; tedarikçiye ödeme POS'tan geçmez. POS'suz
    /// ("Elle gir") tahsilde hesap ve beklenen gün zorunludur, komisyon yoksa
    /// kapsam da yoktur.
    /// </summary>
    [Fact]
    public async Task CardCollection_RejectsPayments_AndAcceptsAManualCollectionWithoutCommission()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "card-manual@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var supplier = await CreateCounterpartyAsync(owner, "Toptancı");

        using (var payable = await owner.PostAsJsonAsync(
                   $"/api/v1/counterparties/{supplier.Id}/payments",
                   new CreateCounterpartyPaymentRequest(
                       "payable", "100.0000", "TRY", null, Date(today),
                       Card: new CardCollectionRequest(PosDefinitionId: f.PosDefinitionId))))
        {
            Assert.Equal(HttpStatusCode.BadRequest, payable.StatusCode);
            Assert.Equal("counterparty_payments.card_requires_collection", await CodeAsync(payable));
        }

        using (var missing = await owner.PostAsJsonAsync(
                   $"/api/v1/counterparties/{supplier.Id}/payments",
                   new CreateCounterpartyPaymentRequest(
                       "receivable", "100.0000", "TRY", null, Date(today),
                       Card: new CardCollectionRequest())))
        {
            Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
            Assert.Equal("pos_settlements.details_required", await CodeAsync(missing));
        }

        using var manual = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "100.0000", "TRY", f.AccountId, Date(today),
                Card: new CardCollectionRequest(ExpectedTransferDate: Date(today.AddDays(2)))));
        Assert.True(
            manual.StatusCode == HttpStatusCode.Created,
            await manual.Content.ReadAsStringAsync());
        var settlements = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        var collection = Assert.Single(settlements!.Items);
        Assert.Equal("collection", collection.Kind);
        Assert.Null(collection.Scope);
        Assert.Null(collection.PosDefinitionId);
        Assert.Equal("0.0000", collection.CommissionAmount);
        Assert.Equal("100.0000", settlements.MoneyInTransit);
    }

    /// <summary>
    /// Tek seferlik alacak da kartla kapanır: alacak bugün kapanır, para yola
    /// çıkar; ödenecek fatura POS'tan kapanamaz.
    /// </summary>
    [Fact]
    public async Task ReceivableObligation_CanBeSettledByCard_AndAPayableCannot()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "card-obligation@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var customer = await CreateCounterpartyAsync(owner, "Ayşe Hanım");

        var receivable = await CreateObligationAsync(
            owner, "receivable", f.SalesCategoryId, customer.Id, today);
        using var settle = await owner.PostAsJsonAsync(
            $"/api/v1/obligations/{receivable.Id}/settlement",
            new SettleObligationRequest(
                null, Date(today), new CardCollectionRequest(PosDefinitionId: f.PosDefinitionId)));
        Assert.True(
            settle.StatusCode == HttpStatusCode.OK,
            await settle.Content.ReadAsStringAsync());
        Assert.Equal("settled", (await settle.Content.ReadFromJsonAsync<ObligationResponse>())!.Status);

        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));
        var settlements = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        var collection = Assert.Single(settlements!.Items);
        Assert.Equal("collection", collection.Kind);
        Assert.Equal("Ayşe Hanım", collection.CounterpartyName);
        Assert.Equal("492.5000", collection.NetAmount);
        var report = await ReportAsync(owner, today);
        Assert.Equal(500m, report.Income);
        Assert.Equal(7.5m, report.Expense);

        var payable = await CreateObligationAsync(
            owner, "payable", (await FirstCategoryAsync(owner, "expense")).Id, customer.Id, today);
        using var refused = await owner.PostAsJsonAsync(
            $"/api/v1/obligations/{payable.Id}/settlement",
            new SettleObligationRequest(
                null, Date(today), new CardCollectionRequest(PosDefinitionId: f.PosDefinitionId)));
        Assert.Equal(HttpStatusCode.BadRequest, refused.StatusCode);
        Assert.Equal("obligations.card_requires_receivable", await CodeAsync(refused));
    }

    /// <summary>Başka kullanıcının POS'u kartla tahsilde kullanılamaz.</summary>
    [Fact]
    public async Task CardCollection_CannotUseAnotherUsersPos()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "card-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(factory, "card-stranger@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var strangersCustomer = await CreateCounterpartyAsync(stranger, "Yabancı müşteri");

        using var response = await stranger.PostAsJsonAsync(
            $"/api/v1/counterparties/{strangersCustomer.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "100.0000", "TRY", null, Date(today),
                Card: new CardCollectionRequest(PosDefinitionId: f.PosDefinitionId)));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("pos_settlements.definition_unavailable", await CodeAsync(response));
        var settlements = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Empty(settlements!.Items);
    }

    private sealed record Seed(
        Guid AccountId, Guid SalesCategoryId, Guid CommissionCategoryId, Guid PosDefinitionId);

    /// <summary>
    /// İşletme etiketli banka hesabı (1000) ve %1,5 komisyonlu bir POS.
    /// </summary>
    private static async Task<Seed> SeedAsync(HttpClient client)
    {
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Sentetik banka", "bank", "TRY", "1000.0000", "business"));
        Assert.Equal(HttpStatusCode.Created, accountResponse.StatusCode);
        var account = (await accountResponse.Content.ReadFromJsonAsync<AccountResponse>())!;
        var sales = await FirstCategoryAsync(client, "income");
        var commission = await FirstCategoryAsync(client, "expense");
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest(
                "Sentetik POS", account.Id, sales.Id, "0.0150", 1, false, commission.Id));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var definition = (await response.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        return new Seed(account.Id, sales.Id, commission.Id, definition.Id);
    }

    private static async Task<ObligationResponse> CreateObligationAsync(
        HttpClient client,
        string direction,
        Guid categoryId,
        Guid counterpartyId,
        DateOnly today)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                direction, "500.0000", "TRY", categoryId, Date(today), Date(today.AddDays(10)),
                "business", counterpartyId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ObligationResponse>())!;
    }

    private static async Task<PosDepositResponse> DepositAsync(
        HttpClient client,
        IReadOnlyList<Guid> settlementIds,
        string depositedAmount,
        DateOnly depositDate)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), settlementIds, depositedAmount, Date(depositDate)));
        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<PosDepositResponse>())!;
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

    private static async Task<string> BalanceAsync(HttpClient client, Guid accountId)
    {
        var account = await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{accountId}");
        return account!.Balance;
    }

    private static async Task<string?> CodeAsync(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        return problem?["code"]?.GetValue<string>();
    }

    private static string Date(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static decimal Parse(string value) =>
        decimal.Parse(value, CultureInfo.InvariantCulture);

    private static async Task<(decimal Income, decimal Expense)> ReportAsync(
        HttpClient client,
        DateOnly today)
    {
        var report = (await client.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={today.Year}&month={today.Month}"))!;
        return (Parse(report.TotalIncome), Parse(report.TotalExpense));
    }

    private static async Task<AdvancedFinancialReportResponse> AdvancedAsync(
        HttpClient client,
        DateOnly today)
    {
        return (await client.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            $"/api/v1/reports/advanced?year={today.Year}&month={today.Month}"
            + $"&asOfDate={Date(today)}&trendMonths=2&daysAhead=30"))!;
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items.First(item => item.DefaultScope == "business");
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
