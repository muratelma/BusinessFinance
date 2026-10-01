using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Features.Taxes;
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
        group.MapPut("/{recurringTransactionId:guid}", UpdateAsync)
            .WithName("UpdateRecurringTransaction")
            .Produces<RecurringTransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
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
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/occurrences/{occurrenceId:guid}/undo", UndoAsync)
            .WithName("UndoRecurringOccurrence")
            .Produces<RecurringOccurrenceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
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
        group.MapPost("/{recurringTransactionId:guid}/occurrences/amount", SetAmountAsync)
            .WithName("SetRecurringOccurrenceAmount")
            .Produces<RecurringOccurrenceResponse>(StatusCodes.Status200OK)
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
        if (!TryCreateCommand(request, httpContext, out var command, out var error))
        {
            return error!;
        }

        var result = await useCase.ExecuteAsync(command!, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/recurring-transactions/{response.Id}", response);
    }

    /// <summary>
    /// Oluşturma isteğinin biçim denetimi; tek plan ve toplu vergi tanımlama
    /// aynı kuralı kullanır.
    /// </summary>
    internal static bool TryCreateCommand(
        CreateRecurringTransactionRequest request,
        HttpContext httpContext,
        out CreateRecurringTransactionCommand? command,
        out IResult? error)
    {
        command = null;
        if (!TaxContractValues.TryParseTaxKind(request.TaxKind, out var taxKind))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Tax kind is not supported.", "recurring.invalid_tax_kind");
            return false;
        }

        if (!TryParsePlanAmount(request.Amount, taxKind is not null, httpContext, out var amount, out error))
        {
            return false;
        }

        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Only TRY currency is currently supported.", "recurring.invalid_currency");
            return false;
        }

        if (!TryParseKind(request.Kind, out var kind))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Kind must be income, expense, or bill-payment.",
                "recurring.invalid_kind");
            return false;
        }

        if (!TryParseRhythm(
                request.Frequency,
                request.MonthEndBehavior,
                request.StartDate,
                request.EndDate,
                request.Months,
                httpContext,
                out var rhythm,
                out error))
        {
            return false;
        }

        if (!TryParseSourceType(request.SourceType, request.AccountId, request.CreditCardId, out var sourceType))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Source type must be account or credit-card.",
                "recurring.invalid_source_type");
            return false;
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Plan scope must be business, personal or empty.",
                "recurring.invalid_scope");
            return false;
        }

        command = new CreateRecurringTransactionCommand(
            sourceType,
            request.AccountId,
            request.CreditCardId,
            request.CategoryId,
            amount,
            CurrencyCode.TRY,
            kind,
            scope,
            rhythm.Frequency,
            rhythm.StartDate,
            rhythm.EndDate,
            rhythm.MonthEndBehavior,
            request.Description,
            request.OccurrenceLimit,
            taxKind,
            request.DayOfMonth,
            rhythm.SelectedMonths);
        error = null;
        return true;
    }

    private static async Task<IResult> UpdateAsync(
        Guid recurringTransactionId,
        UpdateRecurringTransactionRequest request,
        UpdateRecurringTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        // Tutar boş olabilir mi, planın vergi olup olmadığına bağlıdır; bunu
        // domain bilir. Burada yalnız biçim denetlenir.
        if (!TryParsePlanAmount(request.Amount, allowEmpty: true, httpContext, out var amount, out var amountError))
        {
            return amountError!;
        }

        if (!TryParseRhythm(
                request.Frequency,
                request.MonthEndBehavior,
                request.StartDate,
                request.EndDate,
                request.Months,
                httpContext,
                out var rhythm,
                out var rhythmError))
        {
            return rhythmError!;
        }

        if (!TryParseSourceType(request.SourceType, request.AccountId, request.CreditCardId, out var sourceType))
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

        var result = await useCase.ExecuteAsync(new UpdateRecurringTransactionCommand(
            recurringTransactionId,
            sourceType,
            request.AccountId,
            request.CreditCardId,
            request.CategoryId,
            amount,
            scope,
            request.Description,
            rhythm.Frequency,
            rhythm.StartDate,
            rhythm.EndDate,
            rhythm.MonthEndBehavior,
            request.DayOfMonth,
            rhythm.SelectedMonths), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
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

        if (!TryParsePayment(request.PaidOn, request.AccountId, request.CreditCardId, httpContext,
                out var payment, out var paymentError))
        {
            return paymentError!;
        }

        var result = await useCase.ExecuteAsync(
            new RealizeDueRecurringCommand(recurringTransactionId, scheduledDate, dueAmount, payment),
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

        if (!TryParsePayment(request?.PaidOn, request?.AccountId, request?.CreditCardId, httpContext,
                out var payment, out var paymentError))
        {
            return paymentError!;
        }

        var result = await useCase.ExecuteAsync(
            new RealizeRecurringOccurrenceCommand(occurrenceId, amount, payment), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        return Results.Ok(ToRealizedResponse(result.Value));
    }

    private static async Task<IResult> SetAmountAsync(
        Guid recurringTransactionId,
        SetOccurrenceAmountRequest request,
        SetOccurrenceAmountUseCase useCase,
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

        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0m)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "recurring.invalid_amount");
        }

        var result = await useCase.ExecuteAsync(
            new SetOccurrenceAmountCommand(recurringTransactionId, scheduledDate, amount), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UndoAsync(
        Guid occurrenceId,
        UndoRecurringOccurrenceUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UndoRecurringOccurrenceCommand(occurrenceId), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
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
        OptionalSourceTypeValue(recurring.SourceType),
        recurring.AccountId,
        recurring.CreditCardId,
        recurring.CategoryId,
        FinanceContract.OptionalMoney(recurring.Amount),
        recurring.Currency.ToString(),
        KindValue(recurring.Kind),
        FinanceContract.ScopeValue(recurring.Scope),
        TaxContractValues.FrequencyValue(recurring.Frequency),
        FinanceContract.Date(recurring.StartDate),
        recurring.EndDate is DateOnly endDate ? FinanceContract.Date(endDate) : null,
        recurring.OccurrenceLimit,
        recurring.GeneratedOccurrenceCount,
        recurring.NextOccurrenceDate is DateOnly nextDate ? FinanceContract.Date(nextDate) : null,
        MonthEndBehaviorValue(recurring.MonthEndBehavior),
        recurring.Description,
        recurring.IsActive,
        TaxContractValues.OptionalTaxKindValue(recurring.TaxKind),
        recurring.DayOfMonth,
        TaxContractValues.MonthsValue(recurring.SelectedMonths));

    internal static RecurringOccurrenceResponse ToResponse(RecurringOccurrenceDto occurrence) => new(
        occurrence.Id,
        occurrence.RecurringTransactionId,
        occurrence.OccurrenceKey,
        OptionalSourceTypeValue(occurrence.SourceType),
        occurrence.AccountId,
        occurrence.CreditCardId,
        occurrence.CategoryId,
        FinanceContract.OptionalMoney(occurrence.Amount),
        occurrence.Currency.ToString(),
        KindValue(occurrence.Kind),
        FinanceContract.ScopeValue(occurrence.Scope),
        FinanceContract.Date(occurrence.ScheduledDate),
        occurrence.Description,
        occurrence.Status.ToString().ToLowerInvariant(),
        occurrence.BudgetTransactionId,
        occurrence.CreditCardChargeId,
        occurrence.RealizedAtUtc,
        occurrence.ClosedByTransactionId,
        occurrence.ClosedByChargeId,
        occurrence.ClosedAtUtc);

    private sealed record Rhythm(
        RecurrenceFrequency Frequency,
        DateOnly StartDate,
        DateOnly? EndDate,
        MonthEndBehavior MonthEndBehavior,
        int? SelectedMonths);

    private static bool TryParseRhythm(
        string frequencyValue,
        string monthEndBehaviorValue,
        string startDateValue,
        string? endDateValue,
        IReadOnlyList<int>? months,
        HttpContext httpContext,
        out Rhythm rhythm,
        out IResult? error)
    {
        rhythm = null!;
        error = null;
        if (!TaxContractValues.TryParseFrequency(frequencyValue, out var frequency))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Frequency must be daily, weekly, monthly, quarterly, yearly, or selected-months.",
                "recurring.invalid_frequency");
            return false;
        }

        if (!TryParseMonthEndBehavior(monthEndBehaviorValue, out var monthEndBehavior))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Month-end behavior must be clamp-to-last-day or skip-invalid-period.",
                "recurring.invalid_month_end_behavior");
            return false;
        }

        if (!FinanceContract.TryParseDate(startDateValue, out var startDate) ||
            !TryParseOptionalDate(endDateValue, out var endDate))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Dates must use the yyyy-MM-dd format.", "recurring.invalid_date");
            return false;
        }

        if (!TaxContractValues.TryParseMonths(months, out var selectedMonths))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Months must be distinct values between 1 and 12.", "recurring.invalid_months");
            return false;
        }

        rhythm = new Rhythm(frequency, startDate, endDate, monthEndBehavior, selectedMonths);
        return true;
    }

    /// <summary>
    /// An absent value means account when an account is sent, card when only a card
    /// is sent, and no source when neither is — the last is legal only for a tax
    /// plan, which the use case decides.
    /// </summary>
    private static bool TryParseSourceType(
        string? value,
        Guid? accountId,
        Guid? creditCardId,
        out RecurringSourceType? sourceType)
    {
        if (value is null)
        {
            sourceType = accountId is not null
                ? RecurringSourceType.Account
                : creditCardId is not null ? RecurringSourceType.CreditCard : null;
            return true;
        }

        sourceType = value.ToLowerInvariant() switch
        {
            "account" => RecurringSourceType.Account,
            "credit-card" => RecurringSourceType.CreditCard,
            _ => null
        };
        return sourceType is not null;
    }

    private static string SourceTypeValue(RecurringSourceType sourceType) => sourceType switch
    {
        RecurringSourceType.Account => "account",
        RecurringSourceType.CreditCard => "credit-card",
        _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
    };

    private static string? OptionalSourceTypeValue(RecurringSourceType? sourceType) =>
        sourceType is RecurringSourceType value ? SourceTypeValue(value) : null;

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
    /// Planın beklenen tutarı: sıfırdan büyük; boşsa yalnız izin verildiğinde
    /// (vergi planı) geçerlidir.
    /// </summary>
    private static bool TryParsePlanAmount(
        string? value,
        bool allowEmpty,
        HttpContext httpContext,
        out decimal? amount,
        out IResult? error)
    {
        error = null;
        amount = null;
        if (value is null && allowEmpty) return true;
        if (FinanceContract.TryParseAmount(value, out var parsed) && parsed > 0m)
        {
            amount = parsed;
            return true;
        }

        error = ApiProblemResults.Validation(
            httpContext,
            "Amount must be greater than zero and have at most four decimal places.",
            "recurring.invalid_amount");
        return false;
    }

    /// <summary>
    /// Gerçekleştirme sırasında gönderilen tutar; boş bırakmak meşrudur ve
    /// "kalemdeki tutar doğru" demektir.
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

    /// <summary>"Ödedim" ayrıntısı; hiçbiri verilmezse eski davranış.</summary>
    private static bool TryParsePayment(
        string? paidOnValue,
        Guid? accountId,
        Guid? creditCardId,
        HttpContext httpContext,
        out RecurringPaymentDetails? payment,
        out IResult? error)
    {
        payment = null;
        error = null;
        if (paidOnValue is null && accountId is null && creditCardId is null) return true;

        if (!TryParseOptionalDate(paidOnValue, out var paidOn))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Paid-on date must use the yyyy-MM-dd format.", "recurring.invalid_paid_on");
            return false;
        }

        payment = new RecurringPaymentDetails(paidOn, accountId, creditCardId);
        return true;
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
