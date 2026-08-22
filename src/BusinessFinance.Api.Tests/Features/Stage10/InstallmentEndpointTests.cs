using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Stage10;

public sealed class InstallmentEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task PlanAndRealization_AreIdempotentAndOnlyRealizedItemAffectsReports()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "installments@example.test");
        var card = await CreateCardAsync(client);
        var category = await GetExpenseCategoryAsync(client);
        var request = new CreateInstallmentPlanRequest(
            card.Id,
            category.Id,
            Guid.NewGuid(),
            "100.0000",
            "TRY",
            "business",
            3,
            "2026-08-10",
            "Laptop");

        using var firstResponse = await client.PostAsJsonAsync("/api/v1/installment-plans", request);
        using var retryResponse = await client.PostAsJsonAsync("/api/v1/installment-plans", request);
        firstResponse.EnsureSuccessStatusCode();
        retryResponse.EnsureSuccessStatusCode();
        var first = await firstResponse.Content.ReadFromJsonAsync<InstallmentPlanResponse>();
        var retry = await retryResponse.Content.ReadFromJsonAsync<InstallmentPlanResponse>();
        Assert.Equal(first!.Id, retry!.Id);
        Assert.Equal(["33.3333", "33.3333", "33.3334"], first.Items.Select(item => item.Amount));
        Assert.Equal("0.0000", (await GetReportAsync(client)).TotalExpense);

        using var realize = await client.PostAsync(
            $"/api/v1/installment-plans/{first.Id}/items/1/realize", null);
        using var realizeRetry = await client.PostAsync(
            $"/api/v1/installment-plans/{first.Id}/items/1/realize", null);
        realize.EnsureSuccessStatusCode();
        realizeRetry.EnsureSuccessStatusCode();
        var charge = await realize.Content.ReadFromJsonAsync<CardChargeResponse>();
        var retryCharge = await realizeRetry.Content.ReadFromJsonAsync<CardChargeResponse>();
        Assert.Equal(charge!.Id, retryCharge!.Id);

        card = (await client.GetFromJsonAsync<CreditCardResponse>(
            $"/api/v1/credit-cards/{card.Id}"))!;
        var report = await GetReportAsync(client);
        var activity = await client.GetFromJsonAsync<CardActivityResponse>(
            $"/api/v1/credit-cards/{card.Id}/activity");
        var plans = await client.GetFromJsonAsync<InstallmentPlanListResponse>(
            "/api/v1/installment-plans");
        var persistedPlan = Assert.Single(plans!.Items);
        Assert.Equal("33.3333", card.CurrentDebt);
        Assert.Equal("33.3333", report.TotalExpense);
        Assert.Single(activity!.Charges);
        Assert.True(persistedPlan.Items.Single(item => item.Sequence == 1).IsRealized);
        Assert.False(persistedPlan.Items.Single(item => item.Sequence == 2).IsRealized);
    }

    private static async Task<MonthlyReportResponse> GetReportAsync(HttpClient client) =>
        (await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8"))!;

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

    private static async Task<CreditCardResponse> CreateCardAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "1000", "TRY", 10, 20));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreditCardResponse>())!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items[0];
    }
}
