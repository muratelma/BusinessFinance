using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Budgets;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests;

public sealed class SqlServerApiIntegrationTests
{
    [SqlServerFact]
    public async Task Mvp1FinancialFlow_UsesConfiguredSqlServer()
    {
        await using var factory = new BusinessFinanceApiFactory(
            useConfiguredSqlServer: true);
        using var client = factory.CreateClient();
        var email = $"stage7-{Guid.NewGuid():N}@example.test";
        const string password = "Stage6!Synthetic#2026";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, password, HasBusiness: true),
            CancellationToken.None);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, password),
            CancellationToken.None);
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.NotNull(tokens);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        using var createAccount = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Stage 7 Synthetic Cash", "cash", "TRY", "100"),
            CancellationToken.None);
        var account = await createAccount.Content.ReadFromJsonAsync<AccountResponse>(
            CancellationToken.None);
        using var listAccounts = await client.GetAsync(
            "/api/v1/accounts",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, createAccount.StatusCode);
        Assert.Equal(HttpStatusCode.OK, listAccounts.StatusCode);
        Assert.NotNull(account);

        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);
        var category = categories!.Items.First(item => item.DefaultScope == "business");
        using var createTransaction = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id,
                category.Id,
                "125.5000",
                "TRY",
                "expense",
                "business",
                "2026-08-09",
                "SQL synthetic expense"),
            CancellationToken.None);
        using var createBudget = await client.PostAsJsonAsync(
            "/api/v1/budgets",
            new CreateBudgetRequest(category.Id, "100", "TRY", "business", 2026, 8),
            CancellationToken.None);
        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8",
            CancellationToken.None);
        var budgets = await client.GetFromJsonAsync<BudgetListResponse>(
            "/api/v1/budgets?year=2026&month=8",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, createTransaction.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createBudget.StatusCode);
        Assert.Equal("125.5000", report?.TotalExpense);
        Assert.Equal("25.5000", Assert.Single(budgets!.Items).Exceeded);
    }
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(
                "BUSINESS_FINANCE_SQL_TEST_CONNECTION")))
        {
            Skip = "BUSINESS_FINANCE_SQL_TEST_CONNECTION is not configured.";
        }
    }
}
