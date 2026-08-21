using System.Net;
using System.Text.Json;

namespace BusinessFinance.Api.Tests.OpenApi;

public sealed class OpenApiDocumentTests
{
    [Fact]
    public async Task DevelopmentDocument_ContainsAuthenticationPaths()
    {
        await using var factory = new BusinessFinanceApiFactory("Development");
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/openapi/v1.json",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var stream = await response.Content.ReadAsStreamAsync(
            CancellationToken.None);
        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: CancellationToken.None);
        var paths = document.RootElement.GetProperty("paths");

        Assert.True(paths.TryGetProperty("/api/v1/auth/register", out _));
        Assert.True(paths.TryGetProperty("/api/v1/auth/login", out _));
        Assert.True(paths.TryGetProperty("/api/v1/auth/refresh", out _));
        Assert.True(paths.TryGetProperty("/api/v1/auth/logout", out _));
        Assert.True(paths.TryGetProperty("/api/v1/accounts", out _));
        Assert.True(paths.TryGetProperty("/api/v1/transactions", out var transactions));
        var transactionGet = transactions.GetProperty("get");
        var parameterNames = transactionGet
            .GetProperty("parameters")
            .EnumerateArray()
            .Select(parameter => parameter.GetProperty("name").GetString())
            .ToArray();
        string[] expectedParameters =
        [
            "pageNumber",
            "pageSize",
            "dateFrom",
            "dateTo",
            "accountId",
            "categoryId",
            "type",
            "sort"
        ];

        Assert.All(
            expectedParameters,
            expected => Assert.Contains(expected, parameterNames));
        var responses = transactionGet.GetProperty("responses");
        Assert.True(responses.TryGetProperty("200", out _));
        Assert.True(responses.TryGetProperty("400", out _));
        Assert.True(responses.TryGetProperty("401", out _));
        Assert.False(responses.TryGetProperty("501", out _));
        Assert.True(paths.TryGetProperty("/api/v1/categories", out var categories));
        Assert.True(categories.TryGetProperty("get", out var categoriesGet));
        Assert.True(categories.TryGetProperty("post", out var categoriesPost));
        Assert.True(categoriesGet.GetProperty("responses").TryGetProperty("200", out _));
        Assert.True(categoriesPost.GetProperty("responses").TryGetProperty("201", out _));

        Assert.True(paths.TryGetProperty("/api/v1/budgets", out var budgets));
        Assert.True(budgets.TryGetProperty("get", out var budgetsGet));
        Assert.True(budgets.TryGetProperty("post", out var budgetsPost));
        Assert.True(budgetsGet.GetProperty("responses").TryGetProperty("200", out _));
        Assert.True(budgetsPost.GetProperty("responses").TryGetProperty("201", out _));

        Assert.True(paths.TryGetProperty("/api/v1/reports/monthly", out var reports));
        Assert.True(reports.TryGetProperty("get", out var reportsGet));
        Assert.True(reportsGet.GetProperty("responses").TryGetProperty("200", out _));

        Assert.True(paths.TryGetProperty("/api/v1/dashboard", out var dashboard));
        Assert.True(dashboard.TryGetProperty("get", out var dashboardGet));
        Assert.True(dashboardGet.GetProperty("responses").TryGetProperty("200", out _));

        Assert.True(paths.TryGetProperty("/api/v1/transfers", out var transfers));
        Assert.True(transfers.TryGetProperty("get", out _));
        Assert.True(transfers.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty("/api/v1/transfers/{transferId}", out var transfer));
        Assert.True(transfer.TryGetProperty("get", out _));
        Assert.True(transfer.TryGetProperty("delete", out _));

        Assert.True(paths.TryGetProperty("/api/v1/credit-cards", out var creditCards));
        Assert.True(creditCards.TryGetProperty("get", out _));
        Assert.True(creditCards.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-cards/{creditCardId}", out var creditCard));
        Assert.True(creditCard.TryGetProperty("get", out _));
        Assert.True(creditCard.TryGetProperty("put", out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-cards/{creditCardId}/charges", out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-cards/{creditCardId}/payments", out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-cards/{creditCardId}/activity", out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/credit-cards/{creditCardId}/statements/{year}/{month}",
            out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-card-charges/{chargeId}", out _));
        Assert.True(paths.TryGetProperty("/api/v1/credit-card-payments/{paymentId}", out _));

        Assert.True(paths.TryGetProperty("/api/v1/installment-plans", out var installmentPlans));
        Assert.True(installmentPlans.TryGetProperty("get", out _));
        Assert.True(installmentPlans.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/installment-plans/{installmentPlanId}/items/{sequence}/realize",
            out _));

        Assert.True(paths.TryGetProperty(
            "/api/v1/recurring-transactions",
            out var recurringTransactions));
        Assert.True(recurringTransactions.TryGetProperty("get", out _));
        Assert.True(recurringTransactions.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/recurring-transactions/{recurringTransactionId}/active",
            out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/recurring-transactions/occurrences/generate",
            out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/recurring-transactions/occurrences",
            out _));
        Assert.True(paths.TryGetProperty(
            "/api/v1/recurring-transactions/occurrences/{occurrenceId}/realize",
            out _));
        Assert.True(paths.TryGetProperty("/api/v1/upcoming-payments", out var upcomingPayments));
        Assert.True(upcomingPayments.TryGetProperty("get", out _));
        Assert.True(paths.TryGetProperty("/api/v1/reports/advanced", out var advancedReports));
        Assert.True(advancedReports.TryGetProperty("get", out _));
        Assert.True(paths.TryGetProperty("/api/v1/receipts/analyze", out var receiptAnalysis));
        var receiptPost = receiptAnalysis.GetProperty("post");
        var receiptResponses = receiptPost.GetProperty("responses");
        Assert.True(receiptResponses.TryGetProperty("200", out _));
        Assert.True(receiptResponses.TryGetProperty("401", out _));
        Assert.True(receiptResponses.TryGetProperty("413", out _));
        Assert.True(receiptResponses.TryGetProperty("429", out _));
        Assert.True(receiptResponses.TryGetProperty("503", out _));
    }
}
