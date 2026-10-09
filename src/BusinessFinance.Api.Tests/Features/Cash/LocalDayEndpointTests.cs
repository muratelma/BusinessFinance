using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Cash;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.DayCloses;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.SavingsGoals;

namespace BusinessFinance.Api.Tests.Features.Cash;

/// <summary>
/// Kullanıcının takvim günü sunucunun UTC gününden bir gün ileride olabilir.
/// </summary>
/// <remarks>
/// Türkiye'de gece yarısından 03:00'e kadar istemcinin "bugün" diye gönderdiği
/// tarih, sunucunun UTC gününün bir gün ilerisidir. 9 Ekim 2026'ya kadar bu
/// saatlerde bugünün tarihiyle kasa sayımı, gün sonu, POS tahsilatı, yatış,
/// yükümlülük ve kapanışı "gün gelecekte olamaz" diye reddediliyordu (gider ve
/// cari kayıt kabul ediliyordu). Testler istemcinin o saatlerde göndereceği
/// tarihi gönderir: <c>UTC günü + 1</c>. İki gün sonrası hâlâ reddedilir.
/// </remarks>
public sealed class LocalDayEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task RecordsDatedTheUsersTodayAfterMidnight_AreAccepted_AndTwoDaysAheadIsNot()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "local-day@example.test");
        var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
        var night = Date(utcToday.AddDays(1));
        var twoAhead = Date(utcToday.AddDays(2));
        var due = Date(utcToday.AddDays(20));

        var till = await CreateAccountAsync(owner, "Kasa", "cash", "1000.0000");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var sales = await FirstCategoryAsync(owner, "income");
        var costs = await FirstCategoryAsync(owner, "expense");
        using var posResponse = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Sentetik POS", bank.Id, sales.Id, "0.0150", 1, false, costs.Id));
        var pos = (await posResponse.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;

        // Kasa sayımı: yazılır ve kasa kartı onu "bugünün sayımı" diye okur.
        using var count = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts", new CreateCashCountRequest(till.Id, "1000.0000", night));
        Assert.True(count.StatusCode == HttpStatusCode.Created, await count.Content.ReadAsStringAsync());
        var todayForUser = await owner.GetFromJsonAsync<CashCountTodayResponse>(
            $"/api/v1/cash-counts/today?accountId={till.Id}&date={night}");
        Assert.Equal(night, todayForUser!.Count?.CountDate);
        // Tarih gönderilmezse sunucunun günü sorulur: sayım henüz o güne ait
        // değildir. Bozuk tarih reddedilir; uzak tarih sunucunun gününe döner.
        var todayForServer = await owner.GetFromJsonAsync<CashCountTodayResponse>(
            $"/api/v1/cash-counts/today?accountId={till.Id}");
        Assert.Null(todayForServer!.Count);
        using var badDate = await owner.GetAsync(
            $"/api/v1/cash-counts/today?accountId={till.Id}&date=10.10.2026");
        Assert.Equal(HttpStatusCode.BadRequest, badDate.StatusCode);
        var farDate = await owner.GetFromJsonAsync<CashCountTodayResponse>(
            $"/api/v1/cash-counts/today?accountId={till.Id}&date={Date(utcToday.AddDays(9))}");
        Assert.Null(farDate!.Count);
        // Son sayımlar listesi de o sayımı içerir.
        var listed = await owner.GetFromJsonAsync<CashCountListResponse>(
            $"/api/v1/cash-counts?accountId={till.Id}");
        Assert.Contains(listed!.Items, item => item.CountDate == night);

        // POS tahsilatı: yazılır ve varsayılan listede görünür.
        using var sale = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(null, null, "200.0000", "TRY", night, PosDefinitionId: pos.Id));
        Assert.True(sale.StatusCode == HttpStatusCode.Created, await sale.Content.ReadAsStringAsync());
        var sales200 = await owner.GetFromJsonAsync<PosSettlementListResponse>("/api/v1/pos-settlements");
        Assert.Contains(sales200!.Items, item => item.SettlementDate == night);

        // Yükümlülük ve bir gün önce yazılmış yükümlülüğün gece ödenmesi.
        using var invoice = await owner.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest("payable", "300.0000", "TRY", costs.Id, night, due, "business"));
        Assert.True(invoice.StatusCode == HttpStatusCode.Created, await invoice.Content.ReadAsStringAsync());
        using var earlier = await owner.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                "payable", "150.0000", "TRY", costs.Id, Date(utcToday), due, "business"));
        var earlierInvoice = (await earlier.Content.ReadFromJsonAsync<ObligationResponse>())!;
        using var settle = await owner.PostAsJsonAsync(
            $"/api/v1/obligations/{earlierInvoice.Id}/settlement",
            new SettleObligationRequest(bank.Id, night));
        Assert.True(settle.StatusCode == HttpStatusCode.OK, await settle.Content.ReadAsStringAsync());

        // POS yatışı: bir gün önceki satışın parası gece yatırılır.
        using var earlierSale = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "400.0000", "TRY", Date(utcToday), PosDefinitionId: pos.Id));
        var earlierSettlement = (await earlierSale.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        using var deposit = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(Guid.NewGuid(), [earlierSettlement.Id], "394.0000", night));
        Assert.True(deposit.StatusCode == HttpStatusCode.Created, await deposit.Content.ReadAsStringAsync());

        // Tasarruf hedefine katkı.
        using var goalResponse = await owner.PostAsJsonAsync(
            "/api/v1/goals",
            new CreateSavingsGoalRequest(
                "Sentetik hedef", "5000.0000", "TRY", due, "manual-contributions", null, null,
                Date(utcToday)));
        Assert.True(
            goalResponse.StatusCode == HttpStatusCode.Created,
            await goalResponse.Content.ReadAsStringAsync());
        var goal = (await goalResponse.Content.ReadFromJsonAsync<SavingsGoalResponse>())!;
        using var contribution = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{goal.Id}/contributions",
            new AddSavingsGoalContributionRequest("100.0000", "TRY", night, Guid.NewGuid(), null, night));
        Assert.True(
            contribution.IsSuccessStatusCode, await contribution.Content.ReadAsStringAsync());

        // Gün sonu: önizleme ve kayıt.
        var dayClose = new DayCloseRequest(
            night, "500.0000", CashAccountId: till.Id, CashCategoryId: sales.Id);
        using var preview = await owner.PostAsJsonAsync("/api/v1/day-closes/preview", dayClose);
        Assert.True(preview.StatusCode == HttpStatusCode.OK, await preview.Content.ReadAsStringAsync());
        using var closed = await owner.PostAsJsonAsync(
            "/api/v1/day-closes", dayClose with { ClientRequestId = Guid.NewGuid() });
        Assert.True(closed.StatusCode == HttpStatusCode.Created, await closed.Content.ReadAsStringAsync());

        // Pay bir gündür: iki gün sonrası hiçbir kayıtta kabul edilmez.
        using var farCount = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts", new CreateCashCountRequest(till.Id, "1000.0000", twoAhead));
        Assert.Equal(HttpStatusCode.BadRequest, farCount.StatusCode);
        using var farSale = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(null, null, "200.0000", "TRY", twoAhead, PosDefinitionId: pos.Id));
        Assert.Equal(HttpStatusCode.BadRequest, farSale.StatusCode);
        using var farInvoice = await owner.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest("payable", "300.0000", "TRY", costs.Id, twoAhead, due, "business"));
        Assert.Equal(HttpStatusCode.BadRequest, farInvoice.StatusCode);
        using var farClose = await owner.PostAsJsonAsync(
            "/api/v1/day-closes/preview", dayClose with { Date = twoAhead });
        Assert.Equal(HttpStatusCode.BadRequest, farClose.StatusCode);
        Assert.Equal("day_closes.invalid_date", await CodeAsync(farClose));
        using var farContribution = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{goal.Id}/contributions",
            new AddSavingsGoalContributionRequest(
                "100.0000", "TRY", twoAhead, Guid.NewGuid(), null, night));
        Assert.False(farContribution.IsSuccessStatusCode);
    }

    private static string Date(DateOnly value) => value.ToString("yyyy-MM-dd");

    private static async Task<string?> CodeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonObject>();
        return body?["code"]?.GetValue<string>();
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", openingBalance, "business"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
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
