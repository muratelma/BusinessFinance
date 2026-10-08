using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Obligations;

public sealed class ObligationEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task UnpaidInvoice_RecognizesExpenseWithoutMovingCash_AndIsOwnerScoped()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "obligation-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "obligation-stranger@example.test");
        var category = await FirstCategoryAsync(owner, "expense");
        var counterparty = await CreateCounterpartyAsync(owner, "Enerji Tedarik");

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                "payable",
                "412.6000",
                "TRY",
                category.Id,
                "2026-08-05",
                "2026-08-20",
                "business",
                counterparty.Id,
                "Ağustos elektrik faturası"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var obligation = (await create.Content.ReadFromJsonAsync<ObligationResponse>())!;
        Assert.Equal("2026-08-05", obligation.IssueDate);
        Assert.Equal("2026-08-20", obligation.DueDate);
        Assert.Equal("open", obligation.Status);

        var monthly = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("412.6000", monthly!.TotalExpense);
        Assert.Empty(monthly.AccountBalances);

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=20");
        var activity = Assert.Single(feed!.Items);
        Assert.Equal("obligation", activity.ActivityKind);
        Assert.Equal("obligation", activity.SourceGroup);
        Assert.Equal("expense", activity.Effect);
        Assert.Equal("business", activity.Scope);
        Assert.Equal(counterparty.Id, activity.SourceId);

        var list = await owner.GetFromJsonAsync<ObligationListResponse>(
            "/api/v1/obligations?asOfDate=2026-08-21");
        var listed = Assert.Single(list!.Items);
        Assert.True(listed.IsOverdue);
        Assert.Equal("Enerji Tedarik", listed.CounterpartyName);

        var counterpartyBefore = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{counterparty.Id}?asOfDate=2026-08-21");
        Assert.Equal("412.6000", counterpartyBefore!.Payable);
        Assert.Equal("412.6000", counterpartyBefore.OverduePayable);

        var advanced = await owner.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-10"
            + "&trendMonths=2&daysAhead=30");
        Assert.Equal("412.6000", advanced!.NetWorth.PayableDebt);
        Assert.Equal("-412.6000", advanced.NetWorth.NetWorth);

        var account = await CreateAccountAsync(owner);
        using var settle = await owner.PostAsJsonAsync(
            $"/api/v1/obligations/{obligation.Id}/settlement",
            new SettleObligationRequest(account.Id, "2026-08-21"));
        Assert.True(
            settle.StatusCode == HttpStatusCode.OK,
            await settle.Content.ReadAsStringAsync());
        var settled = (await settle.Content.ReadFromJsonAsync<ObligationResponse>())!;
        Assert.Equal("settled", settled.Status);
        Assert.NotNull(settled.SettlementId);

        using var settleAgain = await owner.PostAsJsonAsync(
            $"/api/v1/obligations/{obligation.Id}/settlement",
            new SettleObligationRequest(account.Id, "2026-08-21"));
        var settledAgain = (await settleAgain.Content.ReadFromJsonAsync<ObligationResponse>())!;
        Assert.Equal(settled.SettlementId, settledAgain.SettlementId);

        var accountAfter = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("587.4000", accountAfter!.Balance);
        var monthlyAfter = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        Assert.Equal("412.6000", monthlyAfter!.TotalExpense);
        var advancedAfter = await owner.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            "/api/v1/reports/advanced?year=2026&month=8&asOfDate=2026-08-21"
            + "&trendMonths=2&daysAhead=30");
        Assert.Equal("0.0000", advancedAfter!.NetWorth.PayableDebt);
        Assert.Equal("587.4000", advancedAfter.NetWorth.NetWorth);

        var counterpartyAfter = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{counterparty.Id}?asOfDate=2026-08-21");
        Assert.Equal("0.0000", counterpartyAfter!.Payable);
        Assert.Equal("0.0000", counterpartyAfter.OverduePayable);

        var plannedAfter = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-21&daysAhead=30");
        Assert.DoesNotContain(
            plannedAfter!.Items,
            item => item.PlannedActivityId == obligation.Id);

        var settledFeed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=20");
        Assert.Contains(settledFeed!.Items, item =>
            item.ActivityKind == "obligation-settlement" &&
            item.Effect == "neutral" &&
            item.SourceId == account.Id);

        using var foreignWrite = await stranger.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                "payable",
                "10.0000",
                "TRY",
                category.Id,
                "2026-08-05",
                "2026-08-20",
                "business",
                counterparty.Id));
        Assert.Equal(HttpStatusCode.BadRequest, foreignWrite.StatusCode);
        var strangerList = await stranger.GetFromJsonAsync<ObligationListResponse>(
            "/api/v1/obligations?asOfDate=2026-08-21");
        Assert.Empty(strangerList!.Items);
        using var foreignSettlement = await stranger.PostAsJsonAsync(
            $"/api/v1/obligations/{obligation.Id}/settlement",
            new SettleObligationRequest(account.Id, "2026-08-21"));
        Assert.Equal(HttpStatusCode.NotFound, foreignSettlement.StatusCode);
        var strangerFeed = await stranger.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?pageNumber=1&pageSize=20");
        Assert.Empty(strangerFeed!.Items);
    }

    [Fact]
    public async Task Create_RejectsDueDateBeforeIssueDate()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "obligation-date@example.test");
        var category = await FirstCategoryAsync(owner, "expense");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                "payable",
                "10.0000",
                "TRY",
                category.Id,
                "2026-08-05",
                "2026-08-04",
                "business"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items.First(item => item.DefaultScope == "business");
    }

    private static async Task<CounterpartyResponse> CreateCounterpartyAsync(
        HttpClient client,
        string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest(name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyResponse>())!;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Sentetik kasa", "cash", "TRY", "1000.0000"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
