using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.CreditCards;

namespace BusinessFinance.Api.Tests.Features.Stage10;

public sealed class CreditCardEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task CreditCard_CreateListAndUpdate_ExposeCalculatedLimitSnapshot()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "card-owner@example.test");

        using var create = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Main Card", "10000.0000", "TRY", 10, 20));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var card = await create.Content.ReadFromJsonAsync<CreditCardResponse>();
        Assert.Equal("0.0000", card?.CurrentDebt);
        Assert.Equal("10000.0000", card?.AvailableLimit);

        var list = await client.GetFromJsonAsync<CreditCardListResponse>("/api/v1/credit-cards");
        Assert.Equal(card?.Id, Assert.Single(list!.Items).Id);

        using var update = await client.PutAsJsonAsync(
            $"/api/v1/credit-cards/{card!.Id}",
            new UpdateCreditCardRequest("Reserve Card", "12000", "TRY", 12, 24, false));
        update.EnsureSuccessStatusCode();
        var updated = await update.Content.ReadFromJsonAsync<CreditCardResponse>();
        Assert.Equal("Reserve Card", updated?.Name);
        Assert.Equal("12000.0000", updated?.AvailableLimit);
        Assert.False(updated?.IsActive);
    }

    [Fact]
    public async Task CreditCard_DuplicateUnsafeDayAndForeignDetail_AreRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "card-a@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "card-b@example.test");
        var request = new CreateCreditCardRequest("Main Card", "10000", "TRY", 10, 20);

        using var create = await owner.PostAsJsonAsync("/api/v1/credit-cards", request);
        create.EnsureSuccessStatusCode();
        var card = await create.Content.ReadFromJsonAsync<CreditCardResponse>();
        using var duplicate = await owner.PostAsJsonAsync("/api/v1/credit-cards", request);
        using var unsafeDay = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards",
            request with { Name = "Other", StatementClosingDay = 29 });
        using var foreign = await other.GetAsync($"/api/v1/credit-cards/{card!.Id}");

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, unsafeDay.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokens!.AccessToken);
        return client;
    }
}
