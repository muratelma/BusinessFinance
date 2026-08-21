using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.SavingsGoals;

namespace BusinessFinance.Api.Tests.Features.SavingsGoals;

public sealed class SavingsGoalEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task ProgressSources_AreMutuallyExclusiveAndContributionRetryIsIdempotent()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "goal-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "goal-other@example.test");
        var account = await CreateAccountAsync(owner);

        var accountGoal = await CreateGoalAsync(owner, new CreateSavingsGoalRequest(
            "Account reserve", "1000.0000", "TRY", "2026-12-31",
            "account-balance", account.Id, null, "2026-08-11"));
        Assert.Equal("500.0000", accountGoal.AllocatedAmount);
        Assert.Equal("500.0000", accountGoal.RemainingAmount);
        Assert.Equal("50.0000", accountGoal.ProgressPercentage);

        var manualGoal = await CreateGoalAsync(owner, new CreateSavingsGoalRequest(
            "Manual reserve", "1000.0000", "TRY", "2026-12-31",
            "manual-contributions", null, null, "2026-08-11"));
        Assert.Equal("0.0000", manualGoal.AllocatedAmount);
        var requestId = Guid.NewGuid();
        var contributionRequest = new AddSavingsGoalContributionRequest(
            "250.1250", "TRY", "2026-08-11", requestId, "Synthetic deposit", "2026-08-11");
        using var first = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{manualGoal.Id}/contributions", contributionRequest);
        first.EnsureSuccessStatusCode();
        using var retry = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{manualGoal.Id}/contributions", contributionRequest);
        retry.EnsureSuccessStatusCode();
        var retried = (await retry.Content.ReadFromJsonAsync<SavingsGoalResponse>())!;
        Assert.Equal("250.1250", retried.AllocatedAmount);
        Assert.Equal("749.8750", retried.RemainingAmount);
        Assert.Equal("25.0125", retried.ProgressPercentage);
        Assert.Single(retried.Contributions);

        using var mixedSource = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{accountGoal.Id}/contributions", contributionRequest);
        Assert.Equal(HttpStatusCode.BadRequest, mixedSource.StatusCode);
        var ownerGoals = await owner.GetFromJsonAsync<SavingsGoalListResponse>(
            "/api/v1/goals?asOfDate=2026-08-11");
        Assert.Equal(2, ownerGoals!.Items.Count);
        var foreignGoals = await other.GetFromJsonAsync<SavingsGoalListResponse>(
            "/api/v1/goals?asOfDate=2026-08-11");
        Assert.Empty(foreignGoals!.Items);
        using var foreignContribution = await other.PostAsJsonAsync(
            $"/api/v1/goals/{manualGoal.Id}/contributions", contributionRequest);
        Assert.Equal(HttpStatusCode.NotFound, foreignContribution.StatusCode);
    }

    [Fact]
    public async Task GoalEndpoints_RequireAuthenticationAndRejectForeignAccount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await anonymous.GetAsync("/api/v1/goals?asOfDate=2026-08-11")).StatusCode);
        using var first = await CreateAuthenticatedClientAsync(factory, "goal-first@example.test");
        using var second = await CreateAuthenticatedClientAsync(factory, "goal-second@example.test");
        var foreignAccount = await CreateAccountAsync(first);
        using var invalid = await second.PostAsJsonAsync(
            "/api/v1/goals",
            new CreateSavingsGoalRequest(
                "Foreign", "1000.0000", "TRY", "2026-12-31",
                "account-balance", foreignAccount.Id, null, "2026-08-11"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task GoalDeletion_RemovesOnlyOwnedGoalsWithoutContributionHistory()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "goal-delete-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "goal-delete-other@example.test");
        var unused = await CreateGoalAsync(owner, new CreateSavingsGoalRequest(
            "Unused goal", "1000.0000", "TRY", "2026-12-31",
            "manual-contributions", null, null, "2026-08-13"));

        using var foreignDelete = await other.DeleteAsync($"/api/v1/goals/{unused.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignDelete.StatusCode);
        using var unusedDelete = await owner.DeleteAsync($"/api/v1/goals/{unused.Id}");
        Assert.Equal(HttpStatusCode.NoContent, unusedDelete.StatusCode);

        var used = await CreateGoalAsync(owner, new CreateSavingsGoalRequest(
            "Used goal", "1000.0000", "TRY", "2026-12-31",
            "manual-contributions", null, null, "2026-08-13"));
        using var contribution = await owner.PostAsJsonAsync(
            $"/api/v1/goals/{used.Id}/contributions",
            new AddSavingsGoalContributionRequest(
                "10.0000", "TRY", "2026-08-13", Guid.NewGuid(), null, "2026-08-13"));
        contribution.EnsureSuccessStatusCode();

        using var usedDelete = await owner.DeleteAsync($"/api/v1/goals/{used.Id}");
        Assert.Equal(HttpStatusCode.Conflict, usedDelete.StatusCode);
        Assert.Contains(
            "Katkı geçmişi bulunan tasarruf hedefi silinemez.",
            await usedDelete.Content.ReadAsStringAsync());
        var remaining = await owner.GetFromJsonAsync<SavingsGoalListResponse>(
            "/api/v1/goals?asOfDate=2026-08-13");
        Assert.Equal(used.Id, Assert.Single(remaining!.Items).Id);
    }

    private static async Task<SavingsGoalResponse> CreateGoalAsync(
        HttpClient client,
        CreateSavingsGoalRequest request)
    {
        using var response = await client.PostAsJsonAsync("/api/v1/goals", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SavingsGoalResponse>())!;
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
            new CreateAccountRequest("Goal Account", "bank", "TRY", "500.0000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }
}
