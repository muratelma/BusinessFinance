using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Cash;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Cash;

public sealed class CashCountEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Sayım bir gözlemdir: tek başına hiçbir finansal kayıt üretmez. Fark
    /// ancak kullanıcı onaylayınca <b>tek</b> gelir/gider kaydına dönüşür.
    /// </summary>
    [Fact]
    public async Task CashCount_RecordsNothingUntilTheDifferenceIsConfirmed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "cash-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "cash-stranger@example.test");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik kasa", "cash", "1000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");

        var before = await owner.GetFromJsonAsync<CashCountTodayResponse>(
            $"/api/v1/cash-counts/today?accountId={account.Id}");
        Assert.Equal("1000.0000", before!.ExpectedBalance);
        Assert.Null(before.Count);

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts",
            new CreateCashCountRequest(
                account.Id, "940.0000", Date(today), "business", "Gün sonu sayımı"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var count = (await create.Content.ReadFromJsonAsync<CashCountResponse>())!;
        Assert.Equal("940.0000", count.CountedAmount);
        Assert.Equal("1000.0000", count.ExpectedBalance);
        Assert.Equal("-60.0000", count.Difference);
        Assert.Null(count.AdjustmentTransactionId);

        // Sayım para hareketi değildir: ne bakiye ne rapor kıpırdadı.
        var accountAfterCount = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1000.0000", accountAfterCount!.Balance);
        var monthlyAfterCount = await MonthlyAsync(owner, today);
        Assert.Equal("0.0000", monthlyAfterCount.TotalExpense);

        using var confirm = await owner.PostAsJsonAsync(
            $"/api/v1/cash-counts/{count.Id}/adjustment",
            new ConfirmCashCountDifferenceRequest(expense.Id));
        Assert.Equal(HttpStatusCode.OK, confirm.StatusCode);
        var adjusted = (await confirm.Content.ReadFromJsonAsync<CashCountResponse>())!;
        Assert.NotNull(adjusted.AdjustmentTransactionId);
        // Düzeltmeden sonra beklenen bakiye sayılan tutara eşit: fark kapandı.
        Assert.Equal("940.0000", adjusted.ExpectedBalance);
        Assert.Equal("0.0000", adjusted.Difference);

        var accountAfterAdjustment = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("940.0000", accountAfterAdjustment!.Balance);
        var monthlyAfterAdjustment = await MonthlyAsync(owner, today);
        Assert.Equal("60.0000", monthlyAfterAdjustment.TotalExpense);

        // Tekrar onaylamak ikinci bir kayıt yazmaz.
        using var confirmAgain = await owner.PostAsJsonAsync(
            $"/api/v1/cash-counts/{count.Id}/adjustment",
            new ConfirmCashCountDifferenceRequest(expense.Id));
        Assert.Equal(HttpStatusCode.OK, confirmAgain.StatusCode);
        var adjustedAgain = (await confirmAgain.Content.ReadFromJsonAsync<CashCountResponse>())!;
        Assert.Equal(adjusted.AdjustmentTransactionId, adjustedAgain.AdjustmentTransactionId);
        var monthlyAgain = await MonthlyAsync(owner, today);
        Assert.Equal("60.0000", monthlyAgain.TotalExpense);

        // Yabancı kullanıcı ne okuyabilir ne yazabilir.
        var strangerList = await stranger.GetFromJsonAsync<CashCountListResponse>(
            "/api/v1/cash-counts");
        Assert.Empty(strangerList!.Items);
        using var foreignConfirm = await stranger.PostAsJsonAsync(
            $"/api/v1/cash-counts/{count.Id}/adjustment",
            new ConfirmCashCountDifferenceRequest(expense.Id));
        Assert.Equal(HttpStatusCode.NotFound, foreignConfirm.StatusCode);
        using var foreignToday = await stranger.GetAsync(
            new Uri($"/api/v1/cash-counts/today?accountId={account.Id}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.NotFound, foreignToday.StatusCode);
    }

    /// <summary>
    /// Gün sonunda kasayı iki kez saymak bir düzeltmedir, iki ayrı gözlem
    /// değil: ilki iptal edilir ve listede iptal edilmiş hâliyle kalır.
    /// </summary>
    [Fact]
    public async Task SecondCountOfTheSameDay_SupersedesTheFirst()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "cash-supersede@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik kasa", "cash", "500.0000");

        using var first = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts",
            new CreateCashCountRequest(account.Id, "480.0000", Date(today), "business"));
        var firstCount = (await first.Content.ReadFromJsonAsync<CashCountResponse>())!;

        using var second = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts",
            new CreateCashCountRequest(account.Id, "495.0000", Date(today), "business"));
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var secondCount = (await second.Content.ReadFromJsonAsync<CashCountResponse>())!;

        var today2 = await owner.GetFromJsonAsync<CashCountTodayResponse>(
            $"/api/v1/cash-counts/today?accountId={account.Id}");
        Assert.Equal(secondCount.Id, today2!.Count!.Id);
        Assert.Equal("-5.0000", today2.Count.Difference);

        var list = await owner.GetFromJsonAsync<CashCountListResponse>(
            $"/api/v1/cash-counts?accountId={account.Id}");
        Assert.Equal(2, list!.Items.Count);
        Assert.True(list.Items.Single(item => item.Id == firstCount.Id).IsCancelled);
        Assert.False(list.Items.Single(item => item.Id == secondCount.Id).IsCancelled);
        // Geçmiş sayımın farkı listede yeniden hesaplanmaz.
        Assert.Null(list.Items[0].Difference);
    }

    /// <summary>
    /// Banka bakiyesi elle sayılmaz: sayılabilen tek şey kasadaki nakittir.
    /// </summary>
    [Fact]
    public async Task Create_RejectsABankAccount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "cash-bank@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank", "10.0000");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts",
            new CreateCashCountRequest(account.Id, "10.0000", Date(today), "business"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Sayım tuttuğunda yazılacak bir şey yoktur; sıfır tutarlı bir kayıt
    /// üretmek defteri gürültüyle doldururdu.
    /// </summary>
    [Fact]
    public async Task Confirm_RefusesWhenTheCountMatchesTheExpectedBalance()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "cash-balanced@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik kasa", "cash", "250.0000");
        var expense = await FirstCategoryAsync(owner, "expense");

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/cash-counts",
            new CreateCashCountRequest(account.Id, "250.0000", Date(today), "business"));
        var count = (await create.Content.ReadFromJsonAsync<CashCountResponse>())!;
        Assert.Equal("0.0000", count.Difference);

        using var confirm = await owner.PostAsJsonAsync(
            $"/api/v1/cash-counts/{count.Id}/adjustment",
            new ConfirmCashCountDifferenceRequest(expense.Id));

        Assert.Equal(HttpStatusCode.Conflict, confirm.StatusCode);
    }

    private static string Date(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static async Task<MonthlyReportResponse> MonthlyAsync(
        HttpClient client,
        DateOnly today)
    {
        return (await client.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={today.Year}&month={today.Month}"))!;
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
        string type,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", openingBalance));
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
