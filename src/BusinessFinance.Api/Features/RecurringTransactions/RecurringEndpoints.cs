using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.RecurringTransactions;

public static class RecurringEndpoints
{
    public static IEndpointRouteBuilder MapRecurringEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/recurring-transactions")
            .WithTags("Recurring Transactions")
            .RequireAuthorization();
        group.MapPost("/", CreateAsync)
            .WithName("CreateRecurringTransaction")
            .Produces<RecurringTransactionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/", ListAsync)
            .WithName("ListRecurringTransactions")
            .Produces<RecurringTransactionListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPatch("/{recurringTransactionId:guid}/active", SetActiveAsync)
            .WithName("SetRecurringTransactionActive")
            .Produces<RecurringTransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{recurringTransactionId:guid}", DeleteAsync)
            .WithName("DeleteRecurringTransaction")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/occurrences/generate", GenerateAsync)
            .WithName("GenerateRecurringOccurrences")
            .Produces<GenerateRecurringOccurrencesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/occurrences", ListOccurrencesAsync)
            .WithName("ListRecurringOccurrences")
            .Produces<RecurringOccurrenceListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/occurrences/{occurrenceId:guid}/realize", RealizeAsync)
            .WithName("RealizeRecurringOccurrence")
            .Produces<RealizeRecurringOccurrenceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // Addressed by plan and date rather than by occurrence id: the planned
        // view projects dates whose occurrence row does not exist yet, and those
        // rows had no id to send. Realizing one is a single decision, so it is a
        // single call.
        group.MapPost("/{recurringTransactionId:guid}/occurrences/realize", RealizeDueAsync)
            .WithName("RealizeDueRecurring")
            .Produces<RealizeRecurringOccurrenceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateRecurringTransactionRequest request,
        CreateRecurringTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0m)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "recurring.invalid_amount");
        }
        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext, "Only TRY currency is currently supported.", "recurring.invalid_currency");
        }
        if (!TryParseKind(request.Kind, out var kind))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Kind must be income, expense, or bill-payment.",
                "recurring.invalid_kind");
        }
        if (!TryParseFrequency(request.Frequency, out var frequency))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Frequency must be daily, weekly, monthly, quarterly, or yearly.",
                "recurring.invalid_frequency");
        }
        if (!TryParseMonthEndBehavior(request.MonthEndBehavior, out var monthEndBehavior))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Month-end behavior must be clamp-to-last-day or skip-invalid-period.",
                "recurring.invalid_month_end_behavior");
        }
        if (!FinanceContract.TryParseDate(request.StartDate, out var startDate) ||
            !TryParseOptionalDate(request.EndDate, out var endDate))
        {
            return ApiProblemResults.Validation(
                httpContext, "Dates must use the yyyy-MM-dd format.", "recurring.invalid_date");
        }
        if (!TryParseSourceType(request.SourceType, out var sourceType))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Source type must be account or credit-card.",
                "recurring.invalid_source_type");
        }
        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Plan scope must be business, personal or empty.",
                "recurring.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(new CreateRecurringTransactionCommand(
            sourceType,
            request.AccountId,
            request.CreditCardId,
            request.CategoryId,
            amount,
            CurrencyCode.TRY,
            kind,
            scope,
            frequency,
            startDate,
            endDate,
            monthEndBehavior,
            request.Description,
            request.OccurrenceLimit), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/recurring-transactions/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListRecurringTransactionsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new RecurringTransactionListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> SetActiveAsync(
        Guid recurringTransactionId,
        SetRecurringActiveRequest request,
        SetRecurringActiveUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new SetRecurringActiveCommand(recurringTransactionId, request.IsActive), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> DeleteAsync(
        Guid recurringTransactionId,
        DeleteRecurringTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new DeleteRecurringTransactionCommand(recurringTransactionId), cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GenerateAsync(
        GenerateRecurringOccurrencesRequest request,
        GenerateRecurringOccurrencesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.ThroughDate, out var throughDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Through date must use the yyyy-MM-dd format.",
                "recurring.invalid_through_date");
        }

        var result = await useCase.ExecuteAsync(
            new GenerateRecurringOccurrencesCommand(throughDate), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new GenerateRecurringOccurrencesResponse(
                result.Value.GeneratedOccurrences.Select(ToResponse).ToArray(),
                result.Value.HasMoreDue))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ListOccurrencesAsync(
        ListRecurringOccurrencesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new RecurringOccurrenceListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RealizeDueAsync(
        Guid recurringTransactionId,
        RealizeDueRecurringRequest request,
        RealizeDueRecurringUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.ScheduledDate, out var scheduledDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scheduled date must use the yyyy-MM-dd format.",
                "recurring.invalid_scheduled_date");
        }

        if (!TryParseCorrectedAmount(request.Amount, httpContext, out var dueAmount, out var dueError))
        {
            return dueError!;
        }

        var result = await useCase.ExecuteAsync(
            new RealizeDueRecurringCommand(recurringTransactionId, scheduledDate, dueAmount),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToRealizedResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RealizeAsync(
        Guid occurrenceId,
        RealizeRecurringOccurrenceUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        RealizeRecurringOccurrenceRequest? request = null)
    {
        if (!TryParseCorrectedAmount(request?.Amount, httpContext, out var amount, out var error))
        {
            return error!;
        }

        var result = await useCase.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrenceId, amount), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        return Results.Ok(ToRealizedResponse(result.Value));
    }

    private static RealizeRecurringOccurrenceResponse ToRealizedResponse(
        RealizeRecurringOccurrenceResult realized) =>
        new(
            SourceTypeValue(realized.SourceType),
            realized.Transaction is null
                ? null
                : TransactionContractEndpoints.ToResponse(realized.Transaction),
            realized.Charge is null
                ? null
                : CreditCardEndpoints.ToChargeResponse(realized.Charge));

    internal static RecurringTransactionResponse ToResponse(RecurringTransactionDto recurring) => new(
        recurring.Id,
        SourceTypeValue(recurring.SourceType),
        recurring.AccountId,
        recurring.CreditCardId,
        recurring.CategoryId,
        FinanceContract.Money(recurring.Amount),
        recurring.Currency.ToString(),
        KindValue(recurring.Kind),
        FinanceContract.ScopeValue(recurring.Scope),
        recurring.Frequency.ToString().ToLowerInvariant(),
        FinanceContract.Date(recurring.StartDate),
        recurring.EndDate is DateOnly endDate ? FinanceContract.Date(endDate) : null,
        recurring.OccurrenceLimit,
        recurring.GeneratedOccurrenceCount,
        recurring.NextOccurrenceDate is DateOnly nextDate ? FinanceContract.Date(nextDate) : null,
        MonthEndBehaviorValue(recurring.MonthEndBehavior),
        recurring.Description,
        recurring.IsActive);

    internal static RecurringOccurrenceResponse ToResponse(RecurringOccurrenceDto occurrence) => new(
        occurrence.Id,
        occurrence.RecurringTransactionId,
        occurrence.OccurrenceKey,
        SourceTypeValue(occurrence.SourceType),
        occurrence.AccountId,
        occurrence.CreditCardId,
        occurrence.CategoryId,
        FinanceContract.Money(occurrence.Amount),
        occurrence.Currency.ToString(),
        KindValue(occurrence.Kind),
        FinanceContract.ScopeValue(occurrence.Scope),
        FinanceContract.Date(occurrence.ScheduledDate),
        occurrence.Description,
        occurrence.Status.ToString().ToLowerInvariant(),
        occurrence.BudgetTransactionId,
        occurrence.CreditCardChargeId,
        occurrence.RealizedAtUtc);

    /// <summary>
    /// An absent value means account, so clients written before credit-card sources
    /// keep working unchanged.
    /// </summary>
    private static bool TryParseSourceType(string? value, out RecurringSourceType sourceType)
    {
        if (value is null)
        {
            sourceType = RecurringSourceType.Account;
            return true;
        }

        sourceType = value.ToLowerInvariant() switch
        {
            "account" => RecurringSourceType.Account,
            "credit-card" => RecurringSourceType.CreditCard,
            _ => default
        };
        return sourceType != default;
    }

    private static string SourceTypeValue(RecurringSourceType sourceType) => sourceType switch
    {
        RecurringSourceType.Account => "account",
        RecurringSourceType.CreditCard => "credit-card",
        _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
    };

    private static bool TryParseOptionalDate(string? value, out DateOnly? date)
    {
        date = null;
        if (value is null) return true;
        if (!FinanceContract.TryParseDate(value, out var parsed)) return false;
        date = parsed;
        return true;
    }

    private static bool TryParseKind(string? value, out RecurringTransactionKind kind)
    {
        kind = value?.ToLowerInvariant() switch
        {
            "income" => RecurringTransactionKind.Income,
            "expense" => RecurringTransactionKind.Expense,
            "bill-payment" => RecurringTransactionKind.BillPayment,
            _ => default
        };
        return kind != default;
    }

    /// <summary>
    /// Gerçekleştirme sırasında gönderilen tutar; boş bırakmak meşrudur ve
    /// "plandaki tutar doğru" demektir.
    /// </summary>
    private static bool TryParseCorrectedAmount(
        string? value,
        HttpContext httpContext,
        out decimal? amount,
        out IResult? error)
    {
        error = null;
        if (!FinanceContract.TryParseOptionalAmount(value, out amount) ||
            amount is <= 0m)
        {
            amount = null;
            error = ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "recurring.invalid_amount");
            return false;
        }

        return true;
    }

    private static bool TryParseFrequency(string? value, out RecurrenceFrequency frequency)
    {
        frequency = value?.ToLowerInvariant() switch
        {
            "daily" => RecurrenceFrequency.Daily,
            "weekly" => RecurrenceFrequency.Weekly,
            "monthly" => RecurrenceFrequency.Monthly,
            "yearly" => RecurrenceFrequency.Yearly,
            "quarterly" => RecurrenceFrequency.Quarterly,
            _ => default
        };
        return frequency != default;
    }

    private static bool TryParseMonthEndBehavior(string? value, out MonthEndBehavior behavior)
    {
        behavior = value?.ToLowerInvariant() switch
        {
            "clamp-to-last-day" => MonthEndBehavior.ClampToLastDay,
            "skip-invalid-period" => MonthEndBehavior.SkipInvalidPeriod,
            _ => default
        };
        return behavior != default;
    }

    private static string KindValue(RecurringTransactionKind kind) => kind switch
    {
        RecurringTransactionKind.Income => "income",
        RecurringTransactionKind.Expense => "expense",
        RecurringTransactionKind.BillPayment => "bill-payment",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };

    private static string MonthEndBehaviorValue(MonthEndBehavior behavior) => behavior switch
    {
        MonthEndBehavior.ClampToLastDay => "clamp-to-last-day",
        MonthEndBehavior.SkipInvalidPeriod => "skip-invalid-period",
        _ => throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null)
    };
}
