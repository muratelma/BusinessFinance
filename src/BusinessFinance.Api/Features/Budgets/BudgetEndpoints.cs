using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Budgets;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Budgets;

public static class BudgetEndpoints
{
    public static IEndpointRouteBuilder MapBudgetEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/budgets")
            .WithTags("Budgets")
            .RequireAuthorization();
        group.MapPost(string.Empty, CreateAsync)
            .WithName("CreateMonthlyBudget")
            .Produces<BudgetResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet(string.Empty, ListAsync)
            .WithName("ListMonthlyBudgets")
            .Produces<BudgetListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPut("/{budgetId:guid}", UpdateAsync)
            .WithName("UpdateMonthlyBudget")
            .Produces<BudgetResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateBudgetRequest request,
        CreateBudgetUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseMoney(request.Limit, request.Currency, httpContext, out var limit, out var error))
        {
            return error!;
        }
        var result = await useCase.ExecuteAsync(
            new CreateBudgetCommand(
                request.CategoryId,
                limit,
                CurrencyCode.TRY,
                request.Year,
                request.Month),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }
        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/budgets/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        int year,
        int month,
        ListBudgetsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(year, month, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new BudgetListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UpdateAsync(
        Guid budgetId,
        UpdateBudgetRequest request,
        UpdateBudgetUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseMoney(request.Limit, request.Currency, httpContext, out var limit, out var error))
        {
            return error!;
        }
        var result = await useCase.ExecuteAsync(
            new UpdateBudgetCommand(budgetId, limit, CurrencyCode.TRY),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static bool TryParseMoney(
        string value,
        string currency,
        HttpContext httpContext,
        out decimal amount,
        out IResult? error)
    {
        error = null;
        if (!FinanceContract.TryParseAmount(value, out amount) || amount <= 0)
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Limit must be greater than zero and have at most four decimal places.",
                "budgets.invalid_limit");
            return false;
        }
        if (!string.Equals(currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Only TRY currency is currently supported.",
                "budgets.invalid_currency");
            return false;
        }
        return true;
    }

    private static BudgetResponse ToResponse(BudgetDto budget) => new(
        budget.Id,
        budget.CategoryId,
        budget.CategoryName,
        FinanceContract.Money(budget.Limit),
        FinanceContract.Money(budget.Spent),
        FinanceContract.Money(budget.Remaining),
        FinanceContract.Money(budget.Exceeded),
        budget.Currency.ToString(),
        budget.Year,
        budget.Month);
}
