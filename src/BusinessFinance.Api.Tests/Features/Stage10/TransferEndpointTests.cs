using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transfers;

namespace BusinessFinance.Api.Tests.Features.Stage10;

public sealed class TransferEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Transfer_AdjustsBothBalancesWithoutChangingIncomeOrExpense()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "transfer-owner@example.test");
        var source = await CreateAccountAsync(client, "Source", "1000.0000");
        var destination = await CreateAccountAsync(client, "Destination", "100.0000");

        using var create = await client.PostAsJsonAsync(
            "/api/v1/transfers",
            new CreateTransferRequest(
                source.Id,
                destination.Id,
                "250.5000",
                "TRY",
                "2026-08-10",
                "Synthetic transfer"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var transfer = await create.Content.ReadFromJsonAsync<TransferResponse>();

        var sourceAfter = await client.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{source.Id}");
        var destinationAfter = await client.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{destination.Id}");
        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");

        Assert.Equal("749.5000", sourceAfter?.Balance);
        Assert.Equal("350.5000", destinationAfter?.Balance);
        Assert.Equal("0.0000", report?.TotalIncome);
        Assert.Equal("0.0000", report?.TotalExpense);
        Assert.Equal("0.0000", report?.Net);

        using var cancel = await client.DeleteAsync($"/api/v1/transfers/{transfer!.Id}");
        cancel.EnsureSuccessStatusCode();
        sourceAfter = await client.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{source.Id}");
        destinationAfter = await client.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{destination.Id}");
        Assert.Equal("1000.0000", sourceAfter?.Balance);
        Assert.Equal("100.0000", destinationAfter?.Balance);
    }

    [Fact]
    public async Task Transfer_RejectsSameAndForeignAccountsAndHidesForeignDetail()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "transfer-a@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "transfer-b@example.test");
        var ownerAccount = await CreateAccountAsync(owner, "Owner", "100");
        var otherAccount = await CreateAccountAsync(other, "Other", "100");

        using var same = await owner.PostAsJsonAsync(
            "/api/v1/transfers",
            Request(ownerAccount.Id, ownerAccount.Id));
        using var foreign = await owner.PostAsJsonAsync(
            "/api/v1/transfers",
            Request(ownerAccount.Id, otherAccount.Id));
        Assert.Equal(HttpStatusCode.BadRequest, same.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, foreign.StatusCode);

        var ownerDestination = await CreateAccountAsync(owner, "Destination", "0");
        using var createdResponse = await owner.PostAsJsonAsync(
            "/api/v1/transfers",
            Request(ownerAccount.Id, ownerDestination.Id));
        createdResponse.EnsureSuccessStatusCode();
        var created = await createdResponse.Content.ReadFromJsonAsync<TransferResponse>();

        using var foreignDetail = await other.GetAsync($"/api/v1/transfers/{created!.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignDetail.StatusCode);
    }

    private static CreateTransferRequest Request(Guid sourceId, Guid destinationId) => new(
        sourceId,
        destinationId,
        "10.0000",
        "TRY",
        "2026-08-10",
        null);

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

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, "bank", "TRY", openingBalance));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }
}
