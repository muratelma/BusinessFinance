using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Stage10;

public sealed class CardActivityEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task ChargeAndPayment_UpdateDebtAndBankBalanceWithoutDoubleExpense()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "card-flow@example.test");
        var account = await CreateAccountAsync(client, "Bank", "1000");
        var card = await CreateCardAsync(client, "Card", "500");
        var category = await GetExpenseCategoryAsync(client);

        using var chargeResponse = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(
                category.Id, "300.0000", "TRY", "business", "2026-08-10", "Synthetic purchase"));
        Assert.Equal(HttpStatusCode.Created, chargeResponse.StatusCode);
        var charge = await chargeResponse.Content.ReadFromJsonAsync<CardChargeResponse>();
        card = (await client.GetFromJsonAsync<CreditCardResponse>(
            $"/api/v1/credit-cards/{card.Id}"))!;
        var report = await GetReportAsync(client);
        Assert.Equal("300.0000", card.CurrentDebt);
        Assert.Equal("200.0000", card.AvailableLimit);
        Assert.Equal("300.0000", report.TotalExpense);

        using var deactivate = await client.PutAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}",
            new UpdateCreditCardRequest(card.Name, card.Limit, "TRY", 10, 20, false));
        deactivate.EnsureSuccessStatusCode();
        using var paymentResponse = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(
                account.Id, "100.0000", "TRY", "2026-08-10", "Card payment"));
        Assert.Equal(HttpStatusCode.Created, paymentResponse.StatusCode);
        var payment = await paymentResponse.Content.ReadFromJsonAsync<CardPaymentResponse>();

        card = (await client.GetFromJsonAsync<CreditCardResponse>(
            $"/api/v1/credit-cards/{card.Id}"))!;
        account = (await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}"))!;
        report = await GetReportAsync(client);
        var activity = await client.GetFromJsonAsync<CardActivityResponse>(
            $"/api/v1/credit-cards/{card.Id}/activity");
        Assert.Equal("200.0000", card.CurrentDebt);
        Assert.Equal("300.0000", card.AvailableLimit);
        Assert.Equal("900.0000", account.Balance);
        Assert.Equal("300.0000", report.TotalExpense);
        Assert.Single(activity!.Charges);
        Assert.Single(activity.Payments);

        using var cancelPayment = await client.DeleteAsync(
            $"/api/v1/credit-card-payments/{payment!.Id}");
        using var cancelCharge = await client.DeleteAsync(
            $"/api/v1/credit-card-charges/{charge!.Id}");
        cancelPayment.EnsureSuccessStatusCode();
        cancelCharge.EnsureSuccessStatusCode();
        card = (await client.GetFromJsonAsync<CreditCardResponse>(
            $"/api/v1/credit-cards/{card.Id}"))!;
        account = (await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}"))!;
        report = await GetReportAsync(client);
        Assert.Equal("0.0000", card.CurrentDebt);
        Assert.Equal("1000.0000", account.Balance);
        Assert.Equal("0.0000", report.TotalExpense);
    }

    [Fact]
    public async Task ChargeOverLimitAndPaymentOverDebt_AreRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "card-limits@example.test");
        var account = await CreateAccountAsync(client, "Bank", "1000");
        var card = await CreateCardAsync(client, "Card", "500");
        var category = await GetExpenseCategoryAsync(client);

        using var overLimit = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(category.Id, "500.0001", "TRY", "business", "2026-08-10", null));
        using var overPayment = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(account.Id, "1", "TRY", "2026-08-10", null));

        Assert.Equal(HttpStatusCode.Conflict, overLimit.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, overPayment.StatusCode);
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
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client, string name, string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "bank", "TRY", openingBalance));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CreditCardResponse> CreateCardAsync(HttpClient client, string name, string limit)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest(name, limit, "TRY", 10, 20));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreditCardResponse>())!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items.First(item => item.DefaultScope == "business");
    }
}
