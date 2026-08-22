using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Transfers;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage125;

public sealed class FinancialActivityEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Feed_ReturnsEveryRealizedKindWithLosslessStringsAndOwnerIsolation()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "activity-api-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "activity-api-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var second = await CreateAccountAsync(owner, "Cash");
        var category = await GetCategoryAsync(owner, "expense");
        await CreateTransactionAsync(owner, account.Id, category.Id, "625.5000", "expense");
        await CreateTransferAsync(owner, account.Id, second.Id, "250.0000");

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        Assert.Equal(2, feed!.Pagination.TotalCount);
        var expense = Assert.Single(feed.Items, item => item.ActivityKind == "account-transaction");
        Assert.Equal("expense", expense.Effect);
        Assert.Equal("account", expense.SourceGroup);
        Assert.Equal("manual", expense.Origin);
        Assert.Equal("realized", expense.Status);
        // Money stays a four-decimal string end to end.
        Assert.Equal("625.5000", expense.Amount);
        Assert.Equal("TRY", expense.Currency);
        Assert.True(expense.CanCancel);
        Assert.True(expense.SupportsAttachments);

        var transfer = Assert.Single(feed.Items, item => item.ActivityKind == "transfer");
        Assert.Equal("neutral", transfer.Effect);
        Assert.Equal("Bank", transfer.SourceName);
        Assert.Equal("Cash", transfer.DestinationName);
        // Attachments exist only for budget transactions.
        Assert.False(transfer.SupportsAttachments);

        var strangerFeed = await stranger.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Empty(strangerFeed!.Items);
        Assert.Equal(0, strangerFeed.Pagination.TotalCount);
    }

    [Fact]
    public async Task Feed_RejectsUnknownFilterValueInsteadOfIgnoringIt()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-filter@example.test");

        using var response = await client.GetAsync(
            "/api/v1/financial-activities?sourceGroup=not-a-group");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "financial_activities.invalid_filter_value",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// A transaction created by realizing a recurring occurrence cannot be cancelled.
    /// The occurrence holds exactly one result id with no way back, so cancelling here
    /// would leave it realized and pointing at a cancelled row. The feed reports this
    /// through canCancel, and the endpoint enforces it so a direct call cannot bypass
    /// the hidden button.
    /// </summary>
    [Fact]
    public async Task CancelTransaction_ForRecurringResult_IsRejectedAndReportedByTheFeed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-recurring@example.test");
        var account = await CreateAccountAsync(client, "Bank");
        var category = await GetCategoryAsync(client, "expense");

        using var planResponse = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "100.0000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-01", null, "clamp-to-last-day", "Rent"));
        planResponse.EnsureSuccessStatusCode();

        using var generate = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-01"));
        generate.EnsureSuccessStatusCode();
        var generated = await generate.Content
            .ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var occurrence = Assert.Single(generated!.GeneratedOccurrences);

        using var realize = await client.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        realize.EnsureSuccessStatusCode();
        var realized = await realize.Content
            .ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        var transactionId = realized!.Transaction!.Id;

        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items, item => item.ActivityId == transactionId);
        Assert.Equal("recurring", activity.Origin);
        Assert.False(activity.CanCancel);

        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{transactionId}");

        Assert.Equal(HttpStatusCode.Conflict, cancel.StatusCode);
        var problem = await cancel.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "transactions.cancel_origin_locked",
            problem!["code"]!.GetValue<string>());
    }

    [Fact]
    public async Task CancelCardCharge_ForInstallmentResult_IsRejectedAndReportedByTheFeed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-installment@example.test");
        var category = await GetCategoryAsync(client, "expense");
        using var cardResponse = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "10000", "TRY", 15, 25));
        cardResponse.EnsureSuccessStatusCode();
        var card = await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>();

        using var planResponse = await client.PostAsJsonAsync(
            "/api/v1/installment-plans",
            new CreateInstallmentPlanRequest(
                card!.Id, category.Id, Guid.NewGuid(), "600.0000", "TRY", "business", 2, "2026-08-01", "Phone"));
        planResponse.EnsureSuccessStatusCode();
        var plan = await planResponse.Content.ReadFromJsonAsync<InstallmentPlanResponse>();

        using var realize = await client.PostAsync(
            $"/api/v1/installment-plans/{plan!.Id}/items/1/realize", null);
        realize.EnsureSuccessStatusCode();
        var charge = await realize.Content.ReadFromJsonAsync<CardChargeResponse>();

        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items, item => item.ActivityId == charge!.Id);
        Assert.Equal("installment", activity.Origin);
        Assert.False(activity.CanCancel);

        using var cancel = await client.DeleteAsync($"/api/v1/credit-card-charges/{charge!.Id}");

        Assert.Equal(HttpStatusCode.Conflict, cancel.StatusCode);
        var problem = await cancel.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "credit_card_charges.cancel_origin_locked",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// The origin guard must not break the cancel paths that always worked.
    /// </summary>
    [Fact]
    public async Task CancelTransaction_ForManualEntry_StillWorks()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-manual@example.test");
        var account = await CreateAccountAsync(client, "Bank");
        var category = await GetCategoryAsync(client, "expense");
        var transaction = await CreateTransactionAsync(
            client, account.Id, category.Id, "40.0000", "expense");

        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{transaction.Id}");

        cancel.EnsureSuccessStatusCode();
        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items);
        Assert.Equal("cancelled", activity.Status);
        Assert.NotNull(activity.CancelledAtUtc);
        // A cancelled activity stays in the history but can no longer be acted on.
        Assert.False(activity.CanCancel);

        var withoutCancelled = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?includeCancelled=false");
        Assert.Empty(withoutCancelled!.Items);
    }

    /// <summary>
    /// The planned view reports obligations that have not happened yet, with the enum
    /// strings and lossless money the client depends on, and reports no single total
    /// because summing income, expenses and neutral obligations would mislead.
    /// </summary>
    [Fact]
    public async Task PlannedFeed_ReportsUpcomingObligationsWithoutASingleTotal()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "planned-api-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "planned-api-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var category = await GetCategoryAsync(owner, "expense");

        using var plan = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "125.5000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-20", null, "clamp-to-last-day", "Rent"));
        plan.EnsureSuccessStatusCode();

        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");

        Assert.Equal("2026-08-15", planned!.AsOfDate);
        Assert.Equal(30, planned.DaysAhead);
        Assert.Equal(1, planned.TotalCount);
        Assert.Equal("2026-08-20", planned.NearestDueDate);
        var item = Assert.Single(planned.Items);
        Assert.Equal("recurring-occurrence", item.PlannedKind);
        Assert.Equal("expense", item.Effect);
        Assert.Equal("upcoming", item.Timing);
        Assert.Equal("ready", item.Readiness);
        Assert.Null(item.AttentionCode);
        Assert.Equal("realize", item.ActionKind);
        Assert.Equal("125.5000", item.Amount);
        Assert.Equal("TRY", item.Currency);
        Assert.Equal(account.Id, item.SourceId);
        Assert.Equal("Bank", item.SourceName);
        // No occurrence has been generated yet, so acting on it must generate one first.
        Assert.True(item.IsProjected);

        var strangerPlanned = await stranger.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");
        Assert.Equal(0, strangerPlanned!.TotalCount);
        Assert.Null(strangerPlanned.NearestDueDate);
    }

    /// <summary>
    /// The obligation flag separates money the user owes from money coming in, so the
    /// client does not re-derive that rule and drift from the server.
    /// </summary>
    [Fact]
    public async Task PlannedFeed_MarksWhichRowsAreTheUsersObligation()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "planned-api-burden@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var expense = await GetCategoryAsync(owner, "expense");
        var income = await GetCategoryAsync(owner, "income");

        using var bill = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, expense.Id, "125.5000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-20", null, "clamp-to-last-day", "Rent"));
        bill.EnsureSuccessStatusCode();
        using var salary = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, income.Id, "5000.0000", "TRY", "income", "business", "monthly",
                "2026-08-21", null, "clamp-to-last-day", "Salary"));
        salary.EnsureSuccessStatusCode();

        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");

        var billRow = Assert.Single(planned!.Items, item => item.Effect == "expense");
        var salaryRow = Assert.Single(planned.Items, item => item.Effect == "income");
        Assert.True(billRow.IsPaymentObligation);
        Assert.False(salaryRow.IsPaymentObligation);
    }

    [Fact]
    public async Task PlannedFeed_RejectsAHorizonThatIsNotOffered()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "planned-api-horizon@example.test");

        using var response = await client.GetAsync(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=45");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "planned_activities.invalid_days_ahead",
            problem!["code"]!.GetValue<string>());
    }

    private static async Task<HttpClient> CreateClientAsync(
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
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "bank", "TRY", "1000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<TransactionResponse> CreateTransactionAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, amount, "TRY", type, "business", "2026-08-14", "Synthetic"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static async Task CreateTransferAsync(
        HttpClient client,
        Guid sourceAccountId,
        Guid destinationAccountId,
        string amount)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transfers",
            new CreateTransferRequest(
                sourceAccountId, destinationAccountId, amount, "TRY", "2026-08-13", null));
        response.EnsureSuccessStatusCode();
    }
}
