using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage11;

public sealed class RecurringEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task RecurringFlow_GeneratesOnceIsolatesUsersAndRealizesOnce()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "recurring-api-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "recurring-api-other@example.test");
        var account = await CreateAccountAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);
        var request = new CreateRecurringTransactionRequest(
            account.Id,
            category.Id,
            "100.5000",
            "TRY",
            "bill-payment",
            "business",
            "monthly",
            "2026-08-31",
            null,
            "clamp-to-last-day",
            "Synthetic rent");

        using var createResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions", request);
        createResponse.EnsureSuccessStatusCode();
        var recurring = await createResponse.Content.ReadFromJsonAsync<RecurringTransactionResponse>();
        Assert.Equal("100.5000", recurring!.Amount);
        Assert.Equal("2026-08-31", recurring.NextOccurrenceDate);
        Assert.Equal("0.0000", (await GetReportAsync(owner)).TotalExpense);

        var generateRequest = new GenerateRecurringOccurrencesRequest("2026-08-31");
        using var firstGenerate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate", generateRequest);
        using var retryGenerate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate", generateRequest);
        firstGenerate.EnsureSuccessStatusCode();
        retryGenerate.EnsureSuccessStatusCode();
        var first = await firstGenerate.Content.ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var retry = await retryGenerate.Content.ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var occurrence = Assert.Single(first!.GeneratedOccurrences);
        Assert.Empty(retry!.GeneratedOccurrences);
        Assert.Equal("planned", occurrence.Status);
        Assert.Equal("0.0000", (await GetReportAsync(owner)).TotalExpense);

        var otherOccurrences = await other.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences");
        Assert.Empty(otherOccurrences!.Items);

        using var realize = await owner.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        using var realizeRetry = await owner.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        realize.EnsureSuccessStatusCode();
        realizeRetry.EnsureSuccessStatusCode();
        var realizeResult = await realize.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        var retried = await realizeRetry.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();

        Assert.Equal("account", realizeResult!.SourceType);
        Assert.Null(realizeResult.Charge);
        var transaction = realizeResult.Transaction!;
        Assert.Equal(transaction.Id, retried!.Transaction!.Id);
        Assert.Equal("expense", transaction.Type);
        Assert.Equal("100.5000", (await GetReportAsync(owner)).TotalExpense);
        var occurrences = await owner.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences");
        var realized = Assert.Single(occurrences!.Items);
        Assert.Equal("realized", realized.Status);
        Assert.Equal(transaction.Id, realized.BudgetTransactionId);
    }

    [Fact]
    public async Task OccurrenceLimitedPlan_GeneratesTwelveThenCompletesAndRetryIsEmpty()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory,
            "recurring-limited-owner@example.test");
        var account = await CreateAccountAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);
        var request = new CreateRecurringTransactionRequest(
            account.Id,
            category.Id,
            "100.0000",
            "TRY",
            "expense",
            "business",
            "monthly",
            "2026-01-01",
            null,
            "clamp-to-last-day",
            "Twelve month contract",
            OccurrenceLimit: 12);

        using var createResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            request);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content
            .ReadFromJsonAsync<RecurringTransactionResponse>();
        Assert.Equal(12, created!.OccurrenceLimit);
        Assert.Equal(0, created.GeneratedOccurrenceCount);

        var generateRequest = new GenerateRecurringOccurrencesRequest("2027-01-01");
        using var generatedResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            generateRequest);
        using var retryResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            generateRequest);
        generatedResponse.EnsureSuccessStatusCode();
        retryResponse.EnsureSuccessStatusCode();

        var generated = await generatedResponse.Content
            .ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var retry = await retryResponse.Content
            .ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        Assert.Equal(12, generated!.GeneratedOccurrences.Count);
        Assert.Empty(retry!.GeneratedOccurrences);

        var plans = await owner.GetFromJsonAsync<RecurringTransactionListResponse>(
            "/api/v1/recurring-transactions");
        var completed = Assert.Single(plans!.Items);
        Assert.False(completed.IsActive);
        Assert.Null(completed.NextOccurrenceDate);
        Assert.Equal(12, completed.GeneratedOccurrenceCount);
    }

    [Fact]
    public async Task Create_WithNumericEnumLikeValues_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "recurring-invalid@example.test");
        var account = await CreateAccountAsync(client);
        var category = await GetExpenseCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "10",
                "TRY",
                "3",
                "business",
                "3",
                "2026-08-31",
                null,
                "1",
                null));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// End to end for a credit-card subscription: the plan is created against a card,
    /// generation produces a card-sourced occurrence, and realizing it yields a card
    /// charge instead of a budget transaction. The charge is an expense, so the monthly
    /// report moves — but it must move exactly once.
    /// </summary>
    [Fact]
    public async Task CardRecurringFlow_RealizesIntoCardChargeAndCountsExpenseOnce()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "recurring-card-owner@example.test");
        var card = await CreateCardAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);

        using var createResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            CardPlanRequest(card.Id, category.Id, "bill-payment"));
        createResponse.EnsureSuccessStatusCode();
        var plan = await createResponse.Content.ReadFromJsonAsync<RecurringTransactionResponse>();
        Assert.Equal("credit-card", plan!.SourceType);
        Assert.Equal(card.Id, plan.CreditCardId);
        Assert.Null(plan.AccountId);

        using var generate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-31"));
        generate.EnsureSuccessStatusCode();
        var generated = await generate.Content.ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var occurrence = Assert.Single(generated!.GeneratedOccurrences);
        Assert.Equal("credit-card", occurrence.SourceType);
        Assert.Equal(card.Id, occurrence.CreditCardId);

        using var realize = await owner.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        using var realizeRetry = await owner.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        realize.EnsureSuccessStatusCode();
        realizeRetry.EnsureSuccessStatusCode();
        var realized = await realize.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        var retried = await realizeRetry.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();

        Assert.Equal("credit-card", realized!.SourceType);
        Assert.Null(realized.Transaction);
        Assert.Equal(realized.Charge!.Id, retried!.Charge!.Id);
        Assert.Equal("149.9000", realized.Charge.Amount);
        Assert.Equal("149.9000", (await GetReportAsync(owner)).TotalExpense);

        var occurrences = await owner.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences");
        var stored = Assert.Single(occurrences!.Items);
        Assert.Equal("realized", stored.Status);
        Assert.Equal(realized.Charge.Id, stored.CreditCardChargeId);
        Assert.Null(stored.BudgetTransactionId);
    }

    [Fact]
    public async Task CardRecurringPlan_WithIncomeKind_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "recurring-card-income@example.test");
        var card = await CreateCardAsync(owner);
        var categories = await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            CardPlanRequest(card.Id, categories!.Items[0].Id, "income"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<System.Text.Json.Nodes.JsonObject>();
        Assert.Equal(
            "recurring.income_card_source_not_supported",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// A pre-Stage-12.5 client sends no source type and only an account id.
    /// </summary>
    [Fact]
    public async Task RecurringPlan_WithoutSourceType_StillCreatesAnAccountPlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "recurring-legacy-client@example.test");
        var account = await CreateAccountAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "100.5000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-31", null, "clamp-to-last-day", "Legacy client"));

        response.EnsureSuccessStatusCode();
        var plan = await response.Content.ReadFromJsonAsync<RecurringTransactionResponse>();
        Assert.Equal("account", plan!.SourceType);
        Assert.Equal(account.Id, plan.AccountId);
        Assert.Null(plan.CreditCardId);
    }

    /// <summary>
    /// A due date nobody generated an occurrence for is realized in one call, and
    /// the same call twice still produces one movement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The planned view projects dates from the schedule long before anyone
    /// generates their occurrence rows. Those rows used to carry no target id at
    /// all, so the screen drew a permanently disabled action and the only way
    /// forward was a "generate" button on a different screen. Generating is
    /// bookkeeping the system needs for idempotency; the decision is to realize.
    /// </para>
    /// <para>
    /// Dates are relative to today on purpose: the rule under test is "has the
    /// day come", and a hard-coded date would stop testing it the moment it
    /// passes.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task RealizeDue_UngeneratedDate_RealizesOnceAndRefusesTheFuture()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "recurring-realize-due@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "recurring-realize-due-other@example.test");
        var account = await CreateAccountAsync(owner);
        var category = await GetExpenseCategoryAsync(owner);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dueDate = today.AddDays(-1);
        using var createResponse = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "250.0000",
                "TRY",
                "bill-payment",
                "business",
                "monthly",
                dueDate.ToString("yyyy-MM-dd"),
                null,
                "clamp-to-last-day",
                "Synthetic subscription"));
        createResponse.EnsureSuccessStatusCode();
        var plan = await createResponse.Content.ReadFromJsonAsync<RecurringTransactionResponse>();

        // Nobody generated anything: the list is empty right up to the call.
        using var beforeResponse = await owner.GetAsync(
            "/api/v1/recurring-transactions/occurrences");
        beforeResponse.EnsureSuccessStatusCode();
        var before = await beforeResponse.Content
            .ReadFromJsonAsync<RecurringOccurrenceListResponse>();
        Assert.Empty(before!.Items);

        var realizePath =
            $"/api/v1/recurring-transactions/{plan!.Id}/occurrences/realize";
        var realizeRequest = new RealizeDueRecurringRequest(
            dueDate.ToString("yyyy-MM-dd"));

        using var first = await owner.PostAsJsonAsync(realizePath, realizeRequest);
        using var second = await owner.PostAsJsonAsync(realizePath, realizeRequest);
        first.EnsureSuccessStatusCode();
        second.EnsureSuccessStatusCode();

        var firstRealized = await first.Content
            .ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        var secondRealized = await second.Content
            .ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        Assert.Equal("account", firstRealized!.SourceType);
        Assert.Equal("250.0000", firstRealized.Transaction!.Amount);
        // The second call finds the movement the first one wrote instead of
        // writing a second: the same day must not be paid twice.
        Assert.Equal(firstRealized.Transaction.Id, secondRealized!.Transaction!.Id);

        // A date that has not arrived is refused on the server too; the screen
        // hiding the action is not where a financial rule is enforced.
        using var future = await owner.PostAsJsonAsync(
            realizePath,
            new RealizeDueRecurringRequest(today.AddMonths(1).ToString("yyyy-MM-dd")));
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, future.StatusCode);

        // Another user's plan is not found rather than refused: existence is not
        // leaked.
        using var foreign = await stranger.PostAsJsonAsync(realizePath, realizeRequest);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, foreign.StatusCode);
    }

    private static CreateRecurringTransactionRequest CardPlanRequest(
        Guid cardId, Guid categoryId, string kind) =>
        new(null, categoryId, "149.9000", "TRY", kind, "business", "monthly", "2026-08-31", null,
            "clamp-to-last-day", "Streaming", "credit-card", cardId);

    private static async Task<CreditCardResponse> CreateCardAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Recurring Card", "10000", "TRY", 15, 25));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreditCardResponse>())!;
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
            new CreateAccountRequest("Recurring Account", "bank", "TRY", "0"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items[0];
    }

    private static async Task<MonthlyReportResponse> GetReportAsync(HttpClient client) =>
        (await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8"))!;
}
