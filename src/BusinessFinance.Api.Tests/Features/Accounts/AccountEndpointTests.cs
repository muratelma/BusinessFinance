using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;

namespace BusinessFinance.Api.Tests.Features.Accounts;

public sealed class AccountEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task CreateAndList_WithoutBearerToken_ReturnUnauthorizedProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var createResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Cash", "cash", "TRY"),
            CancellationToken.None);
        using var listResponse = await client.GetAsync(
            "/api/v1/accounts",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, listResponse.StatusCode);
        await AssertProblemCodeAsync(createResponse, "authentication.required");
        await AssertProblemCodeAsync(listResponse, "authentication.required");
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAccount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "create@example.com");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("  Main Cash  ", "CASH", "try"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var account = await response.Content.ReadFromJsonAsync<AccountResponse>(
            CancellationToken.None);
        Assert.NotNull(account);
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal("Main Cash", account.Name);
        Assert.Equal("cash", account.Type);
        Assert.Equal("TRY", account.Currency);
        Assert.True(account.IsActive);
        Assert.Equal($"/api/v1/accounts/{account.Id}", response.Headers.Location?.ToString());
    }

    [Theory]
    [InlineData("investment", "TRY", "accounts.invalid_type")]
    [InlineData("cash", "USD", "accounts.invalid_currency")]
    public async Task Create_WithUnsupportedContractValue_ReturnsValidationProblemDetails(
        string type,
        string currency,
        string expectedCode)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            $"{expectedCode.Replace('.', '-')}@example.com");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Account", type, currency),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedCode);
    }

    [Fact]
    public async Task Create_WithBlankName_ReturnsDomainValidationProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "blank-name@example.com");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("   ", "cash", "TRY"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "accounts.validation");
    }

    [Fact]
    public async Task Create_WithDuplicateName_ReturnsConflictProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "duplicate@example.com");
        await CreateAccountAsync(client, "Main Account", "bank");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("main account", "cash", "TRY"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "accounts.duplicate_name");
    }

    [Fact]
    public async Task List_WithPaginationAndFilter_ReturnsMetadataAndSortedItems()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "list@example.com");
        await CreateAccountAsync(client, "Zeta Bank", "bank");
        await CreateAccountAsync(client, "Alpha Cash", "cash");

        using var pageResponse = await client.GetAsync(
            "/api/v1/accounts?pageNumber=1&pageSize=1",
            CancellationToken.None);
        var page = await pageResponse.Content.ReadFromJsonAsync<AccountListResponse>(
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, pageResponse.StatusCode);
        Assert.NotNull(page);
        Assert.Equal("Alpha Cash", Assert.Single(page.Items).Name);
        Assert.Equal(2, page.Pagination.TotalCount);
        Assert.Equal(2, page.Pagination.TotalPages);
        Assert.False(page.Pagination.HasPreviousPage);
        Assert.True(page.Pagination.HasNextPage);

        using var filterResponse = await client.GetAsync(
            "/api/v1/accounts?type=bank",
            CancellationToken.None);
        var filtered = await filterResponse.Content
            .ReadFromJsonAsync<AccountListResponse>(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, filterResponse.StatusCode);
        Assert.NotNull(filtered);
        var bank = Assert.Single(filtered.Items);
        Assert.Equal("Zeta Bank", bank.Name);
        Assert.Equal("bank", bank.Type);
    }

    [Fact]
    public async Task List_WithInvalidPagination_ReturnsValidationProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "pagination@example.com");

        using var response = await client.GetAsync(
            "/api/v1/accounts?pageNumber=0&pageSize=101",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "accounts.invalid_pagination");
    }

    [Fact]
    public async Task List_DoesNotExposeAnotherUsersAccounts()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var ownerClient = await CreateAuthenticatedClientAsync(
            factory,
            "owner@example.com");
        using var otherClient = await CreateAuthenticatedClientAsync(
            factory,
            "other@example.com");
        await CreateAccountAsync(ownerClient, "Owner Cash", "cash");

        using var ownerResponse = await ownerClient.GetAsync(
            "/api/v1/accounts",
            CancellationToken.None);
        using var otherResponse = await otherClient.GetAsync(
            "/api/v1/accounts",
            CancellationToken.None);
        var ownerPage = await ownerResponse.Content
            .ReadFromJsonAsync<AccountListResponse>(CancellationToken.None);
        var otherPage = await otherResponse.Content
            .ReadFromJsonAsync<AccountListResponse>(CancellationToken.None);

        Assert.NotNull(ownerPage);
        Assert.NotNull(otherPage);
        Assert.Single(ownerPage.Items);
        Assert.Empty(otherPage.Items);
        Assert.Equal(0, otherPage.Pagination.TotalCount);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password),
            CancellationToken.None);
        registerResponse.EnsureSuccessStatusCode();
        using var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password),
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

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY"),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccountResponse>(
                   CancellationToken.None) ??
               throw new InvalidOperationException("Account response was empty.");
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
