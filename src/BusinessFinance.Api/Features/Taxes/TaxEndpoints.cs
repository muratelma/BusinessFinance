using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Taxes;

namespace BusinessFinance.Api.Features.Taxes;

/// <summary>
/// Hazır türün önerisi (ADR 0018 T2). <b>Tutar taşımaz</b>; adı ve ipucunu
/// istemci kurar. <c>dayOfMonth</c> 31 ay sonudur.
/// </summary>
public sealed record TaxCalendarSuggestionResponse(
    string TaxKind,
    string Frequency,
    IReadOnlyList<int> Months,
    int DayOfMonth);

public sealed record TaxCalendarSuggestionListResponse(
    IReadOnlyList<TaxCalendarSuggestionResponse> Items);

public sealed record TaxItemReferenceRequest(Guid RecurringTransactionId, string ScheduledDate);

/// <summary>
/// "Vergi ödemesi ekle" (ADR 0018 İ4, T5). Tam olarak biri: <c>accountId</c>
/// ya da <c>creditCardId</c>.
/// </summary>
/// <param name="ClientRequestId">
/// İstemcinin bu isteğe verdiği kimlik; aynı kimlikle tekrar gönderilen istek
/// ikinci bir gider yazmaz, ilk ödemeyi döner.
/// </param>
/// <param name="Closes">Kapatılan tanımlı kalemler; boş olabilir.</param>
public sealed record CreateTaxPaymentRequest(
    Guid ClientRequestId,
    string Amount,
    string PaidOn,
    Guid CategoryId,
    Guid? AccountId = null,
    Guid? CreditCardId = null,
    string? Scope = null,
    string? Note = null,
    IReadOnlyList<TaxItemReferenceRequest>? Closes = null);

public sealed record TaxSettledItemResponse(
    Guid OccurrenceId,
    Guid RecurringTransactionId,
    string ScheduledDate,
    string? Name,
    string? TaxKind);

public sealed record TaxPaymentResponse(
    Guid PaymentId,
    string SourceType,
    Guid SourceId,
    string SourceName,
    Guid CategoryId,
    string CategoryName,
    string Amount,
    string Currency,
    string PaidOn,
    string? Description,
    string Scope,
    TaxSettledItemResponse? RealizedItem,
    IReadOnlyList<TaxSettledItemResponse> ClosedItems,
    bool IsCancelled);

public sealed record TaxPaymentListResponse(IReadOnlyList<TaxPaymentResponse> Items, bool HasMore);

public sealed record TaxOverviewResponse(
    string AsOfDate,
    int DaysAhead,
    IReadOnlyList<RecurringTransactionResponse> Plans,
    IReadOnlyList<PlannedActivityResponse> Pending,

    // Bekleyenlerin (gecikenler dahil) tutarı belli olanlarının toplamı; tutarı
    // belli olmayanlar toplama girmez, sayıları ayrıca döner.
    string PendingTotal,
    int PendingUnknownAmountCount,
    IReadOnlyList<TaxPaymentResponse> RecentPayments,
    bool HasMorePayments);

public sealed record TaxPlanHistoryItemResponse(
    Guid OccurrenceId,
    string ScheduledDate,
    string Status,
    string? Amount,
    TaxPaymentResponse Payment);

public sealed record TaxPlanDetailResponse(
    RecurringTransactionResponse Plan,
    IReadOnlyList<PlannedActivityResponse> Upcoming,
    IReadOnlyList<TaxPlanHistoryItemResponse> History);

/// <summary>
/// "Vergilerimi tanımla": seçilen vergilerin hepsi birden. Her öğe tek plan
/// oluşturmanın isteğidir ve vergi türü taşır; ya hepsi yazılır ya hiçbiri.
/// </summary>
public sealed record CreateTaxPlansRequest(IReadOnlyList<CreateRecurringTransactionRequest> Items);

public sealed record TaxPlanListResponse(IReadOnlyList<RecurringTransactionResponse> Items);

public static class TaxEndpoints
{
    public static IEndpointRouteBuilder MapTaxEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/tax-calendar/suggestions", ListSuggestions)
            .WithTags("Taxes")
            .WithName("ListTaxCalendarSuggestions")
            .RequireAuthorization()
            .Produces<TaxCalendarSuggestionListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapGet("/api/v1/taxes", GetOverviewAsync)
            .WithTags("Taxes")
            .WithName("GetTaxOverview")
            .RequireAuthorization()
            .Produces<TaxOverviewResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapPost("/api/v1/taxes/plans", CreatePlansAsync)
            .WithTags("Taxes")
            .WithName("CreateTaxPlans")
            .RequireAuthorization()
            .Produces<TaxPlanListResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapGet("/api/v1/taxes/plans/{recurringTransactionId:guid}", GetPlanDetailAsync)
            .WithTags("Taxes")
            .WithName("GetTaxPlanDetail")
            .RequireAuthorization()
            .Produces<TaxPlanDetailResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        var payments = endpoints.MapGroup("/api/v1/tax-payments")
            .WithTags("Taxes")
            .RequireAuthorization();
        payments.MapGet("/", ListPaymentsAsync)
            .WithName("ListTaxPayments")
            .Produces<TaxPaymentListResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        payments.MapPost("/", CreatePaymentAsync)
            .WithName("CreateTaxPayment")
            .Produces<TaxPaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        payments.MapPost("/{paymentId:guid}/undo", UndoPaymentAsync)
            .WithName("UndoTaxPayment")
            .Produces<TaxPaymentResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    /// <summary>
    /// Hazır türleri döner. Bu uç <b>hiçbir şey yazmaz</b>: tür, mevcut
    /// tekrarlayan plan ucundan vergi türüyle kurulur.
    /// </summary>
    private static IResult ListSuggestions() => Results.Ok(
        new TaxCalendarSuggestionListResponse(
            [.. TaxCalendarSuggestions.All.Select(suggestion => new TaxCalendarSuggestionResponse(
                TaxContractValues.TaxKindValue(suggestion.TaxKind),
                TaxContractValues.FrequencyValue(suggestion.Frequency),
                suggestion.Months,
                suggestion.DayOfMonth))]));

    private static async Task<IResult> GetOverviewAsync(
        string? asOfDate,
        int? daysAhead,
        GetTaxOverviewUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedAsOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext, "As-of date must use the yyyy-MM-dd format.", "taxes.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(parsedAsOfDate, daysAhead ?? 30, cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);

        var value = result.Value;
        return Results.Ok(new TaxOverviewResponse(
            FinanceContract.Date(value.AsOfDate),
            value.DaysAhead,
            [.. value.Plans.Select(RecurringEndpoints.ToResponse)],
            [.. value.Pending.Select(FinancialActivityEndpoints.ToResponse)],
            FinanceContract.Money(value.PendingTotal),
            value.PendingUnknownAmountCount,
            [.. value.RecentPayments.Select(ToResponse)],
            value.HasMorePayments));
    }

    private static async Task<IResult> CreatePlansAsync(
        CreateTaxPlansRequest request,
        CreateTaxPlansUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var items = request.Items ?? [];
        var commands = new List<CreateRecurringTransactionCommand>(items.Count);
        foreach (var item in items)
        {
            if (!RecurringEndpoints.TryCreateCommand(item, httpContext, out var command, out var error))
            {
                return error!;
            }

            commands.Add(command!);
        }

        var result = await useCase.ExecuteAsync(commands, cancellationToken);
        return result.IsSuccess
            ? Results.Created(
                "/api/v1/taxes",
                new TaxPlanListResponse([.. result.Value.Select(RecurringEndpoints.ToResponse)]))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetPlanDetailAsync(
        Guid recurringTransactionId,
        string? asOfDate,
        GetTaxPlanDetailUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedAsOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext, "As-of date must use the yyyy-MM-dd format.", "taxes.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(recurringTransactionId, parsedAsOfDate, cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);

        var value = result.Value;
        return Results.Ok(new TaxPlanDetailResponse(
            RecurringEndpoints.ToResponse(value.Plan),
            [.. value.Upcoming.Select(FinancialActivityEndpoints.ToResponse)],
            [.. value.History.Select(item => new TaxPlanHistoryItemResponse(
                item.OccurrenceId,
                FinanceContract.Date(item.ScheduledDate),
                item.Status.ToString().ToLowerInvariant(),
                FinanceContract.OptionalMoney(item.Amount),
                ToResponse(item.Payment)))]));
    }

    private static async Task<IResult> ListPaymentsAsync(
        int? skip,
        int? take,
        ListTaxPaymentsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(skip ?? 0, take ?? 20, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new TaxPaymentListResponse([.. result.Value.Items.Select(ToResponse)], result.Value.HasMore))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreatePaymentAsync(
        CreateTaxPaymentRequest request,
        CreateTaxPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0m)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "tax_payments.invalid_amount");
        }

        if (!FinanceContract.TryParseDate(request.PaidOn, out var paidOn))
        {
            return ApiProblemResults.Validation(
                httpContext, "Paid-on date must use the yyyy-MM-dd format.", "tax_payments.invalid_paid_on");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext, "Scope must be business, personal or empty.", "tax_payments.invalid_scope");
        }

        var closes = new List<TaxItemReference>();
        foreach (var item in request.Closes ?? [])
        {
            if (!FinanceContract.TryParseDate(item.ScheduledDate, out var scheduledDate))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Scheduled date must use the yyyy-MM-dd format.",
                    "tax_payments.invalid_scheduled_date");
            }

            closes.Add(new TaxItemReference(item.RecurringTransactionId, scheduledDate));
        }

        var result = await useCase.ExecuteAsync(new CreateTaxPaymentCommand(
            request.ClientRequestId,
            amount,
            paidOn,
            request.AccountId,
            request.CreditCardId,
            request.CategoryId,
            scope,
            request.Note,
            closes), cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/tax-payments/{response.PaymentId}", response);
    }

    private static async Task<IResult> UndoPaymentAsync(
        Guid paymentId,
        UndoTaxPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(paymentId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static TaxPaymentResponse ToResponse(TaxPaymentDto payment) => new(
        payment.PaymentId,
        payment.SourceType == Domain.RecurringSourceType.CreditCard ? "credit-card" : "account",
        payment.SourceId,
        payment.SourceName,
        payment.CategoryId,
        payment.CategoryName,
        FinanceContract.Money(payment.Amount),
        payment.Currency.ToString(),
        FinanceContract.Date(payment.PaidOn),
        payment.Description,
        FinanceContract.ScopeValue(payment.Scope),
        payment.RealizedItem is TaxSettledItemDto realized ? ToResponse(realized) : null,
        [.. payment.ClosedItems.Select(ToResponse)],
        payment.IsCancelled);

    private static TaxSettledItemResponse ToResponse(TaxSettledItemDto item) => new(
        item.OccurrenceId,
        item.RecurringTransactionId,
        FinanceContract.Date(item.ScheduledDate),
        item.Name,
        TaxContractValues.OptionalTaxKindValue(item.TaxKind));
}
