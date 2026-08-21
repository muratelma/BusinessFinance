using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Authentication;

namespace BusinessFinance.Api.Tests.Features.Transactions;

public sealed class TransactionContractEndpointTests
{
    private const string Email = "transaction-contract@example.com";
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task List_WithoutBearerToken_ReturnsUnauthorizedProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/api/v1/transactions",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertProblemCodeAsync(response, "authentication.required");
    }

    [Fact]
    public async Task List_WithValidFilters_ReturnsEmptyOwnerScopedPage()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        using var response = await client.GetAsync(
            $"/api/v1/transactions?pageNumber=1&pageSize=20" +
            $"&dateFrom=2026-01-01&dateTo=2026-01-31" +
            $"&accountId={accountId}&categoryId={categoryId}" +
            "&type=expense&sort=date-desc",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Empty(document.RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(0, document.RootElement.GetProperty("pagination").GetProperty("totalCount").GetInt32());
    }

    [Theory]
    [InlineData("pageNumber=0", "transactions.invalid_pagination")]
    [InlineData("pageSize=101", "transactions.invalid_pagination")]
    [InlineData("dateFrom=2026/01/01", "transactions.invalid_date")]
    [InlineData(
        "dateFrom=2026-02-01&dateTo=2026-01-01",
        "transactions.invalid_date_range")]
    [InlineData("accountId=not-a-guid", "transactions.invalid_identifier")]
    [InlineData("categoryId=00000000-0000-0000-0000-000000000000", "transactions.invalid_identifier")]
    [InlineData("type=transfer", "transactions.invalid_type")]
    [InlineData("sort=amount-desc", "transactions.invalid_sort")]
    public async Task List_WithInvalidFilter_ReturnsValidationProblemDetails(
        string query,
        string expectedCode)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        using var response = await client.GetAsync(
            $"/api/v1/transactions?{query}",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedCode);
    }

    [Fact]
    public async Task List_WithMalformedNumber_ReturnsInvalidFormatProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        using var response = await client.GetAsync(
            "/api/v1/transactions?pageNumber=abc",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "request.invalid_format");
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory)
    {
        var client = factory.CreateClient();
        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password),
            CancellationToken.None);
        registerResponse.EnsureSuccessStatusCode();
        using var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);
        loginResponse.EnsureSuccessStatusCode();
        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None) ??
            throw new InvalidOperationException("Token response was empty.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokens.AccessToken);

        return client;
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(
            CancellationToken.None);
        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: CancellationToken.None);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
        Assert.True(document.RootElement.TryGetProperty("traceId", out _));
    }
}
