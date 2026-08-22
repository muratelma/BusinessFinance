using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Budgets;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage7;

public sealed class FinancialQueryEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task TransactionList_FiltersMonthBoundaryAndUsesDeterministicDescendingOrder()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "query@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "query-other@example.test");
        var account = await CreateAccountAsync(client, "Query Account");
        var expense = await GetCategoryAsync(client, "expense");
        var januaryLast = await CreateTransactionAsync(
            client, account.Id, expense.Id, "20", "expense", "2026-01-31");
        await CreateTransactionAsync(
            client, account.Id, expense.Id, "30", "expense", "2026-02-01");
        var januaryFirst = await CreateTransactionAsync(
            client, account.Id, expense.Id, "10", "expense", "2026-01-01");

        var page = await client.GetFromJsonAsync<TransactionListResponse>(
            "/api/v1/transactions?dateFrom=2026-01-01&dateTo=2026-01-31&pageSize=10");

        Assert.NotNull(page);
        Assert.Equal(2, page.Pagination.TotalCount);
        Assert.Equal([januaryLast.Id, januaryFirst.Id], page.Items.Select(item => item.Id));
        using var detail = await client.GetAsync($"/api/v1/transactions/{januaryLast.Id}");
        Assert.Equal(HttpStatusCode.OK, detail.StatusCode);
        using var otherDetail = await other.GetAsync($"/api/v1/transactions/{januaryLast.Id}");
        Assert.Equal(HttpStatusCode.NotFound, otherDetail.StatusCode);
        var otherPage = await other.GetFromJsonAsync<TransactionListResponse>(
            "/api/v1/transactions?pageSize=10");
        Assert.NotNull(otherPage);
        Assert.Equal(0, otherPage.Pagination.TotalCount);
        Assert.Empty(otherPage.Items);
    }

    [Fact]
    public async Task MonthlyBudget_CalculatesSpentRemainingExceededAndSupportsLimitUpdate()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "budget@example.test");
        var account = await CreateAccountAsync(client, "Budget Account");
        var expense = await GetCategoryAsync(client, "expense");
        await CreateTransactionAsync(
            client, account.Id, expense.Id, "120", "expense", "2026-08-09");
        using var cardResponse = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Budget Card", "1000", "TRY", 10, 20));
        cardResponse.EnsureSuccessStatusCode();
        var card = await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>();
        using var chargeResponse = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card!.Id}/charges",
            new CreateCardChargeRequest(
                expense.Id, "30", "TRY", "business", "2026-08-10", "Budget card expense"));
        chargeResponse.EnsureSuccessStatusCode();

        using var createdResponse = await client.PostAsJsonAsync(
            "/api/v1/budgets",
            new CreateBudgetRequest(expense.Id, "100", "TRY", "business", 2026, 8));
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await createdResponse.Content.ReadFromJsonAsync<BudgetResponse>();
        using var duplicate = await client.PostAsJsonAsync(
            "/api/v1/budgets",
            new CreateBudgetRequest(expense.Id, "200", "TRY", "business", 2026, 8));
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var list = await client.GetFromJsonAsync<BudgetListResponse>(
            "/api/v1/budgets?year=2026&month=8");
        var exceeded = Assert.Single(list!.Items);
        Assert.Equal("150.0000", exceeded.Spent);
        Assert.Equal("0.0000", exceeded.Remaining);
        Assert.Equal("50.0000", exceeded.Exceeded);

        using var updatedResponse = await client.PutAsJsonAsync(
            $"/api/v1/budgets/{created!.Id}",
            new UpdateBudgetRequest("150", "TRY"));
        updatedResponse.EnsureSuccessStatusCode();
        var updated = await updatedResponse.Content.ReadFromJsonAsync<BudgetResponse>();
        Assert.Equal("0.0000", updated?.Remaining);
        Assert.Equal("0.0000", updated?.Exceeded);
    }

    [Fact]
    public async Task MonthlyReport_ExcludesCancelledAndOtherUsersData()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "report-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "report-other@example.test");
        var ownerAccount = await CreateAccountAsync(owner, "Owner", "100");
        var income = await GetCategoryAsync(owner, "income");
        var expense = await GetCategoryAsync(owner, "expense");
        await CreateTransactionAsync(owner, ownerAccount.Id, income.Id, "500", "income", "2026-08-01");
        await CreateTransactionAsync(owner, ownerAccount.Id, expense.Id, "125", "expense", "2026-08-31");
        var cancelled = await CreateTransactionAsync(
            owner, ownerAccount.Id, expense.Id, "999", "expense", "2026-08-15");
        using var cancel = await owner.DeleteAsync($"/api/v1/transactions/{cancelled.Id}");
        cancel.EnsureSuccessStatusCode();

        var otherAccount = await CreateAccountAsync(other, "Other");
        var otherIncome = await GetCategoryAsync(other, "income");
        await CreateTransactionAsync(other, otherAccount.Id, otherIncome.Id, "7000", "income", "2026-08-10");

        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");
        var dashboard = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/dashboard?year=2026&month=8");

        Assert.Equal("500.0000", report?.TotalIncome);
        Assert.Equal("125.0000", report?.TotalExpense);
        Assert.Equal("375.0000", report?.Net);
        Assert.Equal("475.0000", Assert.Single(report!.AccountBalances).Balance);
        Assert.Equal(report.TotalIncome, dashboard?.TotalIncome);
        Assert.Equal(report.TotalExpense, dashboard?.TotalExpense);
        Assert.Equal(report.Net, dashboard?.Net);
        Assert.Equal(report.CategoryExpenses.Count, dashboard?.CategoryExpenses.Count);
        Assert.Equal(report.AccountBalances.Count, dashboard?.AccountBalances.Count);
        Assert.Single(report.CategoryExpenses);
        Assert.Equal("cash", Assert.Single(report.AccountBalances).Type);
    }

    [Fact]
    public async Task MonthlyReport_CarriesAccountTypeSoTheSummaryCanTellThemApart()
    {
        using var factory = new BusinessFinanceApiFactory();
        var owner = await CreateAuthenticatedClientAsync(factory, "account-type@example.com");

        var cash = await CreateAccountAsync(owner, "Cüzdan", "100");
        var bank = await CreateAccountAsync(owner, "Banka", "900", type: "bank");

        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8");

        var balances = report!.AccountBalances.ToDictionary(item => item.AccountId);
        Assert.Equal("cash", balances[cash.Id].Type);
        Assert.Equal("bank", balances[bank.Id].Type);
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

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string openingBalance = "0",
        string type = "cash")
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", openingBalance));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetCategoryAsync(HttpClient client, string type)
    {
        var response = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return response!.Items[0];
    }

    private static async Task<TransactionResponse> CreateTransactionAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type,
        string date)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, amount, "TRY", type, "business", date, "Synthetic"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }
}
