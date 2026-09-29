using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Api.Features.Profiles;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.UserAccount;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.Budgets;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.Transfers;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.UpcomingPayments;
using BusinessFinance.Api.Features.DataPortability;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.SavingsGoals;
using BusinessFinance.Api.Features.Attachments;
using BusinessFinance.Api.Features.Imports;
using BusinessFinance.Api.Features.Receipts;
using BusinessFinance.Api.Features.Cash;
using BusinessFinance.Api.Features.Taxes;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Health;
using BusinessFinance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(
    options => options.ThrowOnBadRequest = true);
builder.Services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddApplicationUseCases();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiRateLimiting();
builder.Services.AddApiAuthenticationResponses();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddLocalWebClientCors(builder.Configuration);
}
builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("sql-server", tags: ["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseCors(LocalWebClientCorsExtensions.PolicyName);
}
app.UseAuthentication();
// Receipt analysis is partitioned by the authenticated subject, so the rate
// limiter must see the claims principal before it chooses a bucket.
app.UseRateLimiter();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapUserAccountEndpoints();
app.MapEmailVerificationEndpoints();
app.MapAccountEndpoints();
app.MapCategoryEndpoints();
app.MapUserProfileEndpoints();
app.MapBudgetEndpoints();
app.MapReportEndpoints();
app.MapTransactionContractEndpoints();
app.MapTransferEndpoints();
app.MapCreditCardEndpoints();
app.MapInstallmentEndpoints();
app.MapRecurringEndpoints();
app.MapUpcomingPaymentEndpoints();
app.MapFinancialActivityEndpoints();
app.MapDataPortabilityEndpoints();
app.MapDebtEndpoints();
app.MapCounterpartyEndpoints();
app.MapObligationEndpoints();
app.MapCashCountEndpoints();
app.MapTaxCalendarEndpoints();
app.MapPosSettlementEndpoints();
app.MapSavingsGoalEndpoints();
app.MapAttachmentEndpoints();
app.MapImportEndpoints();
app.MapReceiptEndpoints();
app.MapHealthChecks(
        "/health/live",
        new HealthCheckOptions
        {
            Predicate = _ => false
        })
    .AllowAnonymous()
    .DisableRateLimiting();
app.MapHealthChecks(
        "/health/ready",
        new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready")
        })
    .AllowAnonymous()
    .DisableRateLimiting();

app.Run();

public partial class Program;
