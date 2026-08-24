using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Obligations;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Obligations;

public static class ObligationEndpoints
{
    public static IEndpointRouteBuilder MapObligationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/obligations", CreateAsync)
            .WithTags("Obligations")
            .WithName("CreateObligation")
            .RequireAuthorization()
            .Produces<ObligationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
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
                request.Description),
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
        obligation.Status.ToString().ToLowerInvariant());
}
