using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Counterparties;

public static class CounterpartyEndpoints
{
    public static IEndpointRouteBuilder MapCounterpartyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/counterparties")
            .WithTags("Counterparties")
            .RequireAuthorization();

        group.MapPost("/", CreateAsync)
            .WithName("CreateCounterparty")
            .Produces<CounterpartyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/", ListAsync)
            .WithName("ListCounterparties")
            .Produces<CounterpartyListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/{counterpartyId:guid}", GetAsync)
            .WithName("GetCounterparty")
            .Produces<CounterpartyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{counterpartyId:guid}", UpdateAsync)
            .WithName("UpdateCounterparty")
            .Produces<CounterpartyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        // Hareketi olan karşı taraf silinmez; 409 pasifleştirmeye yönlendirir.
        group.MapDelete("/{counterpartyId:guid}", DeleteAsync)
            .WithName("DeleteCounterparty")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/{counterpartyId:guid}/charges", CreateChargeAsync)
            .WithName("CreateCounterpartyCharge")
            .Produces<CounterpartyChargeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/{counterpartyId:guid}/payments", CreatePaymentAsync)
            .WithName("CreateCounterpartyPayment")
            .Produces<CounterpartyPaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // İptal, kart tarafındaki desenin aynısı: kayıt kendi köküyle
        // adreslenir, karşı tarafın altında değil.
        endpoints.MapDelete("/api/v1/counterparty-charges/{chargeId:guid}", CancelChargeAsync)
            .WithName("CancelCounterpartyCharge")
            .RequireAuthorization()
            .Produces<CounterpartyChargeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        endpoints.MapDelete("/api/v1/counterparty-payments/{paymentId:guid}", CancelPaymentAsync)
            .WithName("CancelCounterpartyPayment")
            .RequireAuthorization()
            .Produces<CounterpartyPaymentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateCounterpartyRequest request,
        CreateCounterpartyUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateCounterpartyCommand(request.Name, request.Note), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/counterparties/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListCounterpartiesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? balance = null,
        bool? isActive = null)
    {
        if (!TryParseFilter(balance, out var filter))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Balance filter must be all, open or settled.",
                "counterparties.invalid_balance_filter");
        }

        var result = await useCase.ExecuteAsync(filter, isActive, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new CounterpartyListResponse([.. result.Value.Select(ToResponse)]))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetAsync(
        Guid counterpartyId,
        GetCounterpartyUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(counterpartyId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UpdateAsync(
        Guid counterpartyId,
        UpdateCounterpartyRequest request,
        UpdateCounterpartyUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UpdateCounterpartyCommand(
                counterpartyId, request.Name, request.Note, request.IsActive),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> DeleteAsync(
        Guid counterpartyId,
        DeleteCounterpartyUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(counterpartyId, cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateChargeAsync(
        Guid counterpartyId,
        CreateCounterpartyChargeRequest request,
        CreateCounterpartyChargeUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseDirection(request.Direction, out var direction))
        {
            return InvalidDirection(httpContext);
        }

        if (!TryValidateMoney(request.Amount, request.Currency, httpContext, out var amount, out var error))
        {
            return error!;
        }

        if (!FinanceContract.TryParseDate(request.ChargeDate, out var chargeDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Charge date must use the yyyy-MM-dd format.",
                "counterparties.invalid_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "counterparties.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreateCounterpartyChargeCommand(
                counterpartyId,
                direction,
                amount,
                CurrencyCode.TRY,
                request.CategoryId,
                scope,
                chargeDate,
                request.Description),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToChargeResponse(result.Value);
        return Results.Created($"/api/v1/counterparty-charges/{response.Id}", response);
    }

    private static async Task<IResult> CreatePaymentAsync(
        Guid counterpartyId,
        CreateCounterpartyPaymentRequest request,
        CreateCounterpartyPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseDirection(request.Direction, out var direction))
        {
            return InvalidDirection(httpContext);
        }

        if (!TryValidateMoney(request.Amount, request.Currency, httpContext, out var amount, out var error))
        {
            return error!;
        }

        if (!FinanceContract.TryParseDate(request.PaymentDate, out var paymentDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Payment date must use the yyyy-MM-dd format.",
                "counterparties.invalid_date");
        }

        var result = await useCase.ExecuteAsync(
            new CreateCounterpartyPaymentCommand(
                counterpartyId,
                request.AccountId,
                direction,
                amount,
                CurrencyCode.TRY,
                paymentDate,
                request.Description),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToPaymentResponse(result.Value);
        return Results.Created($"/api/v1/counterparty-payments/{response.Id}", response);
    }

    private static async Task<IResult> CancelChargeAsync(
        Guid chargeId,
        CancelCounterpartyChargeUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(chargeId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToChargeResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CancelPaymentAsync(
        Guid paymentId,
        CancelCounterpartyPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(paymentId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToPaymentResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static bool TryParseFilter(string? value, out CounterpartyBalanceFilter filter)
    {
        filter = CounterpartyBalanceFilter.All;
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        switch (value.Trim().ToLowerInvariant())
        {
            case "all":
                filter = CounterpartyBalanceFilter.All;
                return true;
            case "open":
                filter = CounterpartyBalanceFilter.Open;
                return true;
            case "settled":
                filter = CounterpartyBalanceFilter.Settled;
                return true;
            default:
                return false;
        }
    }

    private static bool TryParseDirection(string? value, out DebtDirection direction)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "payable":
                direction = DebtDirection.Payable;
                return true;
            case "receivable":
                direction = DebtDirection.Receivable;
                return true;
            default:
                direction = default;
                return false;
        }
    }

    private static IResult InvalidDirection(HttpContext httpContext) =>
        ApiProblemResults.Validation(
            httpContext,
            "Direction must be payable or receivable.",
            "counterparties.invalid_direction");

    private static bool TryValidateMoney(
        string? amountText,
        string? currency,
        HttpContext httpContext,
        out decimal amount,
        out IResult? error)
    {
        if (!FinanceContract.TryParseAmount(amountText, out amount))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Amount must be a decimal string with at most four decimals.",
                "counterparties.invalid_amount");
            return false;
        }

        if (!string.Equals(currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Only TRY is supported.", "counterparties.invalid_currency");
            return false;
        }

        error = null;
        return true;
    }

    private static string DirectionValue(DebtDirection direction) =>
        direction == DebtDirection.Payable ? "payable" : "receivable";

    private static CounterpartyResponse ToResponse(CounterpartyDto counterparty) => new(
        counterparty.Id,
        counterparty.Name,
        counterparty.Note,
        counterparty.IsActive,
        FinanceContract.Money(counterparty.Receivable),
        FinanceContract.Money(counterparty.Payable),
        FinanceContract.Money(counterparty.Net),
        counterparty.IsSettled);

    private static CounterpartyChargeResponse ToChargeResponse(CounterpartyChargeDto charge) => new(
        charge.Id,
        charge.CounterpartyId,
        charge.CategoryId,
        DirectionValue(charge.Direction),
        FinanceContract.Money(charge.Amount),
        charge.Currency.ToString(),
        FinanceContract.ScopeValue(charge.Scope),
        FinanceContract.Date(charge.ChargeDate),
        charge.Description,
        charge.IsCancelled);

    private static CounterpartyPaymentResponse ToPaymentResponse(CounterpartyPaymentDto payment) => new(
        payment.Id,
        payment.CounterpartyId,
        payment.AccountId,
        DirectionValue(payment.Direction),
        FinanceContract.Money(payment.Amount),
        payment.Currency.ToString(),
        FinanceContract.Date(payment.PaymentDate),
        payment.Description,
        payment.IsCancelled);
}
