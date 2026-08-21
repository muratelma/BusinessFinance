using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.UpcomingPayments;

namespace BusinessFinance.Api.Tests.Features.Stage11;

public sealed class UpcomingPaymentEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task List_ClassifiesOwnerOccurrenceAndRejectsInvalidHorizon()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "upcoming-api-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "upcoming-api-other@example.test");
        var account = await CreateAccountAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);
        using var create = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "75.2500",
                "TRY",
                "bill-payment",
                "monthly",
                "2026-08-10",
                null,
                "clamp-to-last-day",
                "Internet"));
        create.EnsureSuccessStatusCode();
        using var generate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-10"));
        generate.EnsureSuccessStatusCode();

        var ownerPayments = await owner.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-11&daysAhead=30");
        Assert.Collection(
            ownerPayments!.Items,
            payment =>
            {
                Assert.Equal("recurring-occurrence", payment.SourceType);
                Assert.Equal("75.2500", payment.Amount);
                Assert.Equal("2026-08-10", payment.DueDate);
                Assert.Equal("overdue", payment.Timing);
            },
            payment =>
            {
                Assert.Equal("recurring-occurrence", payment.SourceType);
                Assert.Equal("75.2500", payment.Amount);
                Assert.Equal("2026-09-10", payment.DueDate);
                Assert.Equal("upcoming", payment.Timing);
            });
        var otherPayments = await other.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-11&daysAhead=30");
        Assert.Empty(otherPayments!.Items);

        using var invalid = await owner.GetAsync(
            "/api/v1/upcoming-payments?asOfDate=2026-08-11&daysAhead=91");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
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
            new CreateAccountRequest("Upcoming Account", "bank", "TRY", "0"));
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
