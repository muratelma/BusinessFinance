using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage11;

public sealed class AdvancedReportEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task AdvancedReport_ReturnsLosslessMoneyAndZeroTrendMonth()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetExpenseCategoryAsync(client);
        using var transaction = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id,
                category.Id,
                "100.2500",
                "TRY",
                "expense",
                "2026-08-02",
                "Fixture"));
        transaction.EnsureSuccessStatusCode();

        var report = await client.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-11&trendMonths=2&daysAhead=30");

        Assert.Equal("899.7500", report!.NetWorth.LiquidAssets);
        Assert.Equal("0.0000", report.NetWorth.CreditCardDebt);
        Assert.Equal("899.7500", report.NetWorth.NetWorth);
        Assert.Equal("100.2500", report.PeriodComparison.Current.Expense);
        Assert.Equal("-100.2500", report.PeriodComparison.Current.Net);
        Assert.Equal(2, report.CashFlowTrend.Count);
        Assert.Equal(7, report.CashFlowTrend[0].Month);
        Assert.Equal("0.0000", report.CashFlowTrend[0].Expense);
        Assert.Equal(8, report.CashFlowTrend[1].Month);
        Assert.Equal("100.2500", report.CashFlowTrend[1].Expense);
        Assert.Equal("0.0000", report.FutureLoad.TotalAmount);

        using var invalid = await client.GetAsync(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=invalid&trendMonths=2&daysAhead=30");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest("advanced-api@example.test", Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("advanced-api@example.test", Password));
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
            new CreateAccountRequest("Report Account", "bank", "TRY", "1000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items[0];
    }
}
