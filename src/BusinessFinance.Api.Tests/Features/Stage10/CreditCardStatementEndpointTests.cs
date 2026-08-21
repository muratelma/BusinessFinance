using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;

namespace BusinessFinance.Api.Tests.Features.Stage10;

public sealed class CreditCardStatementEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Statement_CalculatesPreviousCarryOpenOverdueAndPaidStates()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "statement@example.test");
        var account = await CreateAccountAsync(client);
        var card = await CreateCardAsync(client);
        var category = await GetExpenseCategoryAsync(client);
        await CreateChargeAsync(client, card.Id, category.Id, "100", "2026-04-01");
        await CreateChargeAsync(client, card.Id, category.Id, "300", "2026-04-11");
        await CreatePaymentAsync(client, card.Id, account.Id, "50", "2026-05-05");
        await CreatePaymentAsync(client, card.Id, account.Id, "100", "2026-05-15");

        var open = await GetStatementAsync(client, card.Id, "2026-05-15");
        var overdue = await GetStatementAsync(client, card.Id, "2026-05-21");

        Assert.Equal("2026-04-11", open.PeriodStart);
        Assert.Equal("2026-05-10", open.ClosingDate);
        Assert.Equal("2026-05-20", open.DueDate);
        Assert.Equal("100.0000", open.PreviousBalance);
        Assert.Equal("300.0000", open.PeriodCharges);
        Assert.Equal("50.0000", open.PaymentsThroughClosing);
        Assert.Equal("350.0000", open.StatementBalance);
        Assert.Equal("100.0000", open.PaymentsAfterClosing);
        Assert.Equal("250.0000", open.RemainingBalance);
        Assert.Equal("open", open.PaymentStatus);
        Assert.Equal("overdue", overdue.PaymentStatus);

        await CreatePaymentAsync(client, card.Id, account.Id, "250", "2026-05-20");
        var paid = await GetStatementAsync(client, card.Id, "2026-05-20");
        Assert.Equal("0.0000", paid.RemainingBalance);
        Assert.Equal("paid", paid.PaymentStatus);
    }

    [Fact]
    public async Task Statement_BeforeClosingAndForeignCard_AreRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "statement-a@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "statement-b@example.test");
        var card = await CreateCardAsync(owner);

        using var beforeClosing = await owner.GetAsync(
            $"/api/v1/credit-cards/{card.Id}/statements/2026/5?asOf=2026-05-09");
        using var foreign = await other.GetAsync(
            $"/api/v1/credit-cards/{card.Id}/statements/2026/5?asOf=2026-05-20");

        Assert.Equal(HttpStatusCode.BadRequest, beforeClosing.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);
    }

    [Fact]
    public async Task Statement_ReportsTheMinimumPaymentFromTheCardRate()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "statement-min@example.test");
        var account = await CreateAccountAsync(client);
        var card = await CreateCardAsync(client, minimumPaymentRate: "40");
        var category = await GetExpenseCategoryAsync(client);
        await CreateChargeAsync(client, card.Id, category.Id, "400", "2026-04-11");

        var statement = await GetStatementAsync(client, card.Id, "2026-05-15");

        Assert.Equal("40.0000", card.MinimumPaymentRate);
        Assert.Equal("400.0000", statement.StatementBalance);
        Assert.Equal("160.0000", statement.MinimumPayment);
        Assert.Equal("160.0000", statement.RemainingMinimumPayment);
        Assert.Equal("40.0000", statement.MinimumPaymentRate);

        // Kesim sonrası ödeme asgariyi de eritir.
        await CreatePaymentAsync(client, card.Id, account.Id, "60", "2026-05-15");
        var afterPayment = await GetStatementAsync(client, card.Id, "2026-05-15");
        Assert.Equal("100.0000", afterPayment.RemainingMinimumPayment);
        Assert.Equal("340.0000", afterPayment.RemainingBalance);
    }

    [Fact]
    public async Task CurrentStatement_ReturnsTheLastClosedPeriod()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "statement-current@example.test");
        var card = await CreateCardAsync(client);
        var category = await GetExpenseCategoryAsync(client);
        await CreateChargeAsync(client, card.Id, category.Id, "200", "2026-05-05");

        // Kesim günü 10; 15 Mayıs'ta güncel ekstre mayıs dönemidir.
        var afterClosing = await GetCurrentStatementAsync(client, card.Id, "2026-05-15");
        Assert.NotNull(afterClosing.Statement);
        Assert.Equal(2026, afterClosing.Statement!.Year);
        Assert.Equal(5, afterClosing.Statement.Month);
        Assert.Equal("200.0000", afterClosing.Statement.StatementBalance);

        // 5 Mayıs'ta ise mayıs henüz kesilmedi; güncel ekstre nisan dönemidir.
        var beforeClosing = await GetCurrentStatementAsync(client, card.Id, "2026-05-05");
        Assert.NotNull(beforeClosing.Statement);
        Assert.Equal(4, beforeClosing.Statement!.Month);
        Assert.Equal("0.0000", beforeClosing.Statement.StatementBalance);
    }

    [Fact]
    public async Task CurrentStatement_ForAForeignCard_IsNotFound()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "current-a@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "current-b@example.test");
        var card = await CreateCardAsync(owner);

        using var response = await other.GetAsync(
            $"/api/v1/credit-cards/{card.Id}/statements/current");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Card_RejectsAMinimumPaymentRateAbove100()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "rate-guard@example.test");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Bad Rate", "1000", "TRY", 10, 20, "101"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<CurrentCreditCardStatementResponse> GetCurrentStatementAsync(
        HttpClient client,
        Guid cardId,
        string asOf) =>
        (await client.GetFromJsonAsync<CurrentCreditCardStatementResponse>(
            $"/api/v1/credit-cards/{cardId}/statements/current?asOf={asOf}"))!;

    private static async Task<CreditCardStatementResponse> GetStatementAsync(
        HttpClient client,
        Guid cardId,
        string asOf) =>
        (await client.GetFromJsonAsync<CreditCardStatementResponse>(
            $"/api/v1/credit-cards/{cardId}/statements/2026/5?asOf={asOf}"))!;

    private static async Task CreateChargeAsync(
        HttpClient client, Guid cardId, Guid categoryId, string amount, string date)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{cardId}/charges",
            new CreateCardChargeRequest(categoryId, amount, "TRY", date, null));
        response.EnsureSuccessStatusCode();
    }

    private static async Task CreatePaymentAsync(
        HttpClient client, Guid cardId, Guid accountId, string amount, string date)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{cardId}/payments",
            new CreateCardPaymentRequest(accountId, amount, "TRY", date, null));
        response.EnsureSuccessStatusCode();
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
            "/api/v1/accounts", new CreateAccountRequest("Bank", "bank", "TRY", "1000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CreditCardResponse> CreateCardAsync(
        HttpClient client,
        string? minimumPaymentRate = null)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "1000", "TRY", 10, 20, minimumPaymentRate));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreditCardResponse>())!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items[0];
    }
}
