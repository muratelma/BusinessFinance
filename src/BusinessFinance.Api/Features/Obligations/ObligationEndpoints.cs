using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Obligations;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Obligations;

public static class ObligationEndpoints
{
    public static IEndpointRouteBuilder MapObligationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/obligations", ListAsync)
            .WithTags("Obligations")
            .WithName("ListObligations")
            .RequireAuthorization()
            .Produces<ObligationListResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapPost("/api/v1/obligations", CreateAsync)
            .WithTags("Obligations")
            .WithName("CreateObligation")
            .RequireAuthorization()
            .Produces<ObligationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        endpoints.MapPost("/api/v1/obligations/{id:guid}/settlement", SettleAsync)
            .WithTags("Obligations")
            .WithName("SettleObligation")
            .RequireAuthorization()
            .Produces<ObligationResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        string? asOfDate,
        ListObligationsUseCase useCase,
        TimeProvider timeProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (asOfDate is not null && !FinanceContract.TryParseDate(asOfDate, out today))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "As-of date must use the yyyy-MM-dd format.",
                "obligations.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(today, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new ObligationListResponse(
                result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateAsync(
        CreateObligationRequest request,
        CreateObligationUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseDirection(request.Direction, out var direction))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Direction must be payable or receivable.",
                "obligations.invalid_direction");
        }

        if (!FinanceContract.TryParseAmount(request.Amount, out var amount))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be a decimal string with at most four decimals.",
                "obligations.invalid_amount");
        }

        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only TRY is supported.",
                "obligations.invalid_currency");
        }

        if (!FinanceContract.TryParseDate(request.IssueDate, out var issueDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Issue date must use the yyyy-MM-dd format.",
                "obligations.invalid_issue_date");
        }

        if (!FinanceContract.TryParseDate(request.DueDate, out var dueDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Due date must use the yyyy-MM-dd format.",
                "obligations.invalid_due_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "obligations.invalid_scope");
        }

        if (!VatContractMapper.TryParse(request.VatRate, request.VatAmount, out var vat))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Vat rate and amount must have at most four decimal places.",
                "obligations.invalid_vat");
        }

        var result = await useCase.ExecuteAsync(
            new CreateObligationCommand(
                direction,
                amount,
                CurrencyCode.TRY,
                request.CategoryId,
                scope,
                issueDate,
                dueDate,
                request.CounterpartyId,
                request.Description,
                vat,
                request.IsTaxDeductible),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/obligations/{response.Id}", response);
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

    private static async Task<IResult> SettleAsync(
        Guid id,
        SettleObligationRequest request,
        SettleObligationUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.SettlementDate, out var settlementDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Settlement date must use the yyyy-MM-dd format.",
                "obligations.invalid_settlement_date");
        }

        var result = await useCase.ExecuteAsync(
            new SettleObligationCommand(id, request.AccountId, settlementDate),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static ObligationResponse ToResponse(ObligationDto obligation) => new(
        obligation.Id,
        obligation.CounterpartyId,
        obligation.CategoryId,
        obligation.Direction == DebtDirection.Payable ? "payable" : "receivable",
        FinanceContract.Money(obligation.Amount),
        obligation.Currency.ToString(),
        FinanceContract.ScopeValue(obligation.Scope),
        FinanceContract.Date(obligation.IssueDate),
        FinanceContract.Date(obligation.DueDate),
        obligation.Description,
        obligation.Status.ToString().ToLowerInvariant(),
        obligation.CounterpartyName,
        obligation.CategoryName,
        obligation.IsOverdue,
        obligation.SettlementId,
        obligation.SettlementAccountId,
        obligation.SettlementDate is DateOnly settlementDate
            ? FinanceContract.Date(settlementDate)
            : null,
        VatContractMapper.ToContract(obligation.Vat),
        obligation.IsTaxDeductible);
}
