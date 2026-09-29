using System.Globalization;
using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.CreditCards;

public static class CreditCardEndpoints
{
    public static IEndpointRouteBuilder MapCreditCardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/credit-cards")
            .WithTags("Credit Cards")
            .RequireAuthorization();

        group.MapPost("/", CreateAsync)
            .WithName("CreateCreditCard")
            .Produces<CreditCardResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/", ListAsync)
            .WithName("ListCreditCards")
            .Produces<CreditCardListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/{creditCardId:guid}", GetAsync)
            .WithName("GetCreditCard")
            .Produces<CreditCardResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{creditCardId:guid}", UpdateAsync)
            .WithName("UpdateCreditCard")
            .Produces<CreditCardResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/{creditCardId:guid}/charges", CreateChargeAsync)
            .WithName("CreateCreditCardCharge")
            .Produces<CardChargeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/{creditCardId:guid}/payments", CreatePaymentAsync)
            .WithName("CreateCreditCardPayment")
            .Produces<CardPaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/{creditCardId:guid}/activity", ListActivityAsync)
            .WithName("ListCreditCardActivity")
            .Produces<CardActivityResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // Sabit parçalı yol, `{year:int}` kalıbından önce eşleşmeli.
        group.MapGet("/{creditCardId:guid}/statements/current", GetCurrentStatementAsync)
            .WithName("GetCurrentCreditCardStatement")
            .Produces<CurrentCreditCardStatementResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapGet("/{creditCardId:guid}/statements/{year:int}/{month:int}", GetStatementAsync)
            .WithName("GetCreditCardStatement")
            .Produces<CreditCardStatementResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        endpoints.MapDelete("/api/v1/credit-card-charges/{chargeId:guid}", CancelChargeAsync)
            .WithTags("Credit Cards")
            .WithName("CancelCreditCardCharge")
            .RequireAuthorization()
            .Produces<CardChargeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        endpoints.MapDelete("/api/v1/credit-card-payments/{paymentId:guid}", CancelPaymentAsync)
            .WithTags("Credit Cards")
            .WithName("CancelCreditCardPayment")
            .RequireAuthorization()
            .Produces<CardPaymentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentStatementAsync(
        Guid creditCardId,
        GetCurrentCreditCardStatementUseCase useCase,
        TimeProvider timeProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? asOf = null)
    {
        DateOnly asOfDate;
        if (asOf is null)
        {
            asOfDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        }
        else if (!FinanceContract.TryParseDate(asOf, out asOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext, "As-of date must use the yyyy-MM-dd format.", "credit_cards.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(creditCardId, asOfDate, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new CurrentCreditCardStatementResponse(
                result.Value.CreditCardId,
                result.Value.Statement is null
                    ? null
                    : ToStatementResponse(result.Value.Statement)))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetStatementAsync(
        Guid creditCardId,
        int year,
        int month,
        GetCreditCardStatementUseCase useCase,
        TimeProvider timeProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? asOf = null)
    {
        DateOnly asOfDate;
        if (asOf is null)
        {
            asOfDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        }
        else if (!FinanceContract.TryParseDate(asOf, out asOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext, "As-of date must use the yyyy-MM-dd format.", "credit_cards.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(
            new GetCreditCardStatementQuery(creditCardId, year, month, asOfDate),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToStatementResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateChargeAsync(
        Guid creditCardId,
        CreateCardChargeRequest request,
        CreateCardChargeUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryValidateActivityMoney(request.Amount, request.Currency, httpContext, out var amount, out var error))
        {
            return error!;
        }
        if (!FinanceContract.TryParseDate(request.ChargeDate, out var date))
        {
            return ApiProblemResults.Validation(
                httpContext, "Charge date must use the yyyy-MM-dd format.", "credit_cards.invalid_charge_date");
        }
        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Charge scope must be business, personal or empty.",
                "credit_cards.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(new CreateCardChargeCommand(
            creditCardId, request.CategoryId, amount, CurrencyCode.TRY, scope, date,
            request.Description),
            cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);
        var response = ToChargeResponse(result.Value);
        return Results.Created($"/api/v1/credit-card-charges/{response.Id}", response);
    }

    private static async Task<IResult> CreatePaymentAsync(
        Guid creditCardId,
        CreateCardPaymentRequest request,
        CreateCardPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryValidateActivityMoney(request.Amount, request.Currency, httpContext, out var amount, out var error))
        {
            return error!;
        }
        if (!FinanceContract.TryParseDate(request.PaymentDate, out var date))
        {
            return ApiProblemResults.Validation(
                httpContext, "Payment date must use the yyyy-MM-dd format.", "credit_cards.invalid_payment_date");
        }

        var result = await useCase.ExecuteAsync(new CreateCardPaymentCommand(
            creditCardId, request.AccountId, amount, CurrencyCode.TRY, date, request.Description), cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(httpContext);
        var response = ToPaymentResponse(result.Value);
        return Results.Created($"/api/v1/credit-card-payments/{response.Id}", response);
    }

    private static async Task<IResult> ListActivityAsync(
        Guid creditCardId,
        ListCardActivityUseCase useCase,
        TimeProvider timeProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? from = null,
        string? to = null,
        bool all = false)
    {
        if (!HistoryWindowContract.TryParse(
                from, to, all, timeProvider, httpContext, out var window, out var error))
        {
            return error!;
        }

        var result = await useCase.ExecuteAsync(creditCardId, window, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new CardActivityResponse(
                result.Value.Charges.Select(ToChargeResponse).ToArray(),
                result.Value.Payments.Select(ToPaymentResponse).ToArray(),
                result.Value.HasMore))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CancelChargeAsync(
        Guid chargeId,
        CancelCardChargeUseCase useCase,
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
        CancelCardPaymentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(paymentId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToPaymentResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateAsync(
        CreateCreditCardRequest request,
        CreateCreditCardUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryValidateMoney(request.Limit, request.Currency, httpContext, out var limit, out var error))
        {
            return error!;
        }

        if (!TryParseMinimumPaymentRate(request.MinimumPaymentRate, out var rate))
        {
            return InvalidMinimumPaymentRate(httpContext);
        }

        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var defaultScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "credit_cards.invalid_default_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreateCreditCardCommand(
                request.Name,
                limit,
                CurrencyCode.TRY,
                request.StatementClosingDay,
                request.PaymentDueDay,
                rate,
                defaultScope),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/credit-cards/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListCreditCardsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new CreditCardListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetAsync(
        Guid creditCardId,
        GetCreditCardUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(creditCardId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UpdateAsync(
        Guid creditCardId,
        UpdateCreditCardRequest request,
        UpdateCreditCardUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryValidateMoney(request.Limit, request.Currency, httpContext, out var limit, out var error))
        {
            return error!;
        }

        if (!TryParseMinimumPaymentRate(request.MinimumPaymentRate, out var rate))
        {
            return InvalidMinimumPaymentRate(httpContext);
        }

        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var defaultScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "credit_cards.invalid_default_scope");
        }

        var result = await useCase.ExecuteAsync(
            new UpdateCreditCardCommand(
                creditCardId,
                request.Name,
                limit,
                CurrencyCode.TRY,
                request.StatementClosingDay,
                request.PaymentDueDay,
                rate,
                request.IsActive,
                defaultScope),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    /// <summary>
    /// Asgari ödeme oranını okur; alan gönderilmemişse <c>null</c> döner.
    /// </summary>
    /// <remarks>
    /// Yüzde olarak geliyor ("20" = %20), borç faiz oranıyla aynı biçim.
    /// Gönderilmemek geçerli: oluşturmada varsayılan, güncellemede mevcut
    /// oran kullanılır.
    /// </remarks>
    private static bool TryParseMinimumPaymentRate(string? value, out decimal? rate)
    {
        rate = null;
        if (value is null)
        {
            return true;
        }

        if (!decimal.TryParse(
                value,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var parsed) ||
            parsed is < 0m or > CreditCard.MaximumMinimumPaymentRate ||
            decimal.Round(parsed, 4) != parsed)
        {
            return false;
        }

        rate = parsed;
        return true;
    }

    private static IResult InvalidMinimumPaymentRate(HttpContext httpContext) =>
        ApiProblemResults.Validation(
            httpContext,
            "Minimum payment rate must be a percentage between 0 and 100.",
            "credit_cards.invalid_minimum_payment_rate");

    private static bool TryValidateMoney(
        string rawLimit,
        string currency,
        HttpContext httpContext,
        out decimal limit,
        out IResult? error)
    {
        if (!FinanceContract.TryParseAmount(rawLimit, out limit) || limit <= 0m)
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Limit must be greater than zero and have at most four decimal places.",
                "credit_cards.invalid_limit");
            return false;
        }

        if (!string.Equals(currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Only TRY currency is currently supported.",
                "credit_cards.invalid_currency");
            return false;
        }

        error = null;
        return true;
    }

    private static bool TryValidateActivityMoney(
        string rawAmount,
        string currency,
        HttpContext httpContext,
        out decimal amount,
        out IResult? error)
    {
        if (!FinanceContract.TryParseAmount(rawAmount, out amount) || amount <= 0m)
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "credit_cards.invalid_amount");
            return false;
        }
        if (!string.Equals(currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            error = ApiProblemResults.Validation(
                httpContext, "Only TRY currency is currently supported.", "credit_cards.invalid_currency");
            return false;
        }
        error = null;
        return true;
    }

    internal static CreditCardResponse ToResponse(CreditCardDto card) => new(
        card.Id,
        card.Name,
        FinanceContract.Money(card.Limit),
        FinanceContract.Money(card.CurrentDebt),
        FinanceContract.Money(card.AvailableLimit),
        card.Currency.ToString(),
        card.StatementClosingDay,
        card.PaymentDueDay,
        FinanceContract.Money(card.MinimumPaymentRate),
        card.IsActive,
        FinanceContract.OptionalScopeValue(card.DefaultScope));

    internal static CardChargeResponse ToChargeResponse(CardChargeDto charge) => new(
        charge.Id,
        charge.CreditCardId,
        charge.CategoryId,
        FinanceContract.Money(charge.Amount),
        charge.Currency.ToString(),
        FinanceContract.ScopeValue(charge.Scope),
        FinanceContract.Date(charge.ChargeDate),
        charge.Description,
        charge.IsCancelled,
        charge.CancelledAtUtc);

    internal static CardPaymentResponse ToPaymentResponse(CardPaymentDto payment) => new(
        payment.Id,
        payment.CreditCardId,
        payment.AccountId,
        FinanceContract.Money(payment.Amount),
        payment.Currency.ToString(),
        FinanceContract.Date(payment.PaymentDate),
        payment.Description,
        payment.IsCancelled,
        payment.CancelledAtUtc);

    internal static CreditCardStatementResponse ToStatementResponse(CreditCardStatementDto statement) => new(
        statement.CreditCardId,
        statement.Year,
        statement.Month,
        FinanceContract.Date(statement.PeriodStart),
        FinanceContract.Date(statement.ClosingDate),
        FinanceContract.Date(statement.DueDate),
        FinanceContract.Money(statement.PreviousBalance),
        FinanceContract.Money(statement.PeriodCharges),
        FinanceContract.Money(statement.PaymentsThroughClosing),
        FinanceContract.Money(statement.StatementBalance),
        FinanceContract.Money(statement.PaymentsAfterClosing),
        FinanceContract.Money(statement.RemainingBalance),
        FinanceContract.Money(statement.MinimumPayment),
        FinanceContract.Money(statement.RemainingMinimumPayment),
        FinanceContract.Money(statement.MinimumPaymentRate),
        statement.Currency.ToString(),
        statement.PaymentStatus.ToString().ToLowerInvariant());
}
