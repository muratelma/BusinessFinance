using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Cash;

namespace BusinessFinance.Api.Features.Cash;

public static class CashCountEndpoints
{
    public static IEndpointRouteBuilder MapCashCountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/cash-counts", ListAsync)
            .WithTags("CashCounts")
            .WithName("ListCashCounts")
            .RequireAuthorization()
            .Produces<CashCountListResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapGet("/api/v1/cash-counts/today", GetTodayAsync)
            .WithTags("CashCounts")
            .WithName("GetCashCountToday")
            .RequireAuthorization()
            .Produces<CashCountTodayResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        endpoints.MapPost("/api/v1/cash-counts", CreateAsync)
            .WithTags("CashCounts")
            .WithName("CreateCashCount")
            .RequireAuthorization()
            .Produces<CashCountResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        endpoints.MapPost("/api/v1/cash-counts/{id:guid}/adjustment", ConfirmAsync)
            .WithTags("CashCounts")
            .WithName("ConfirmCashCountDifference")
            .RequireAuthorization()
            .Produces<CashCountResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        Guid? accountId,
        string? from,
        string? to,
        ListCashCountsUseCase useCase,
        TimeProvider timeProvider,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var start = today.AddDays(-30);
        if (from is not null && !FinanceContract.TryParseDate(from, out start))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Start date must use the yyyy-MM-dd format.",
                "cash_counts.invalid_from");
        }

        var end = today;
        if (to is not null && !FinanceContract.TryParseDate(to, out end))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "End date must use the yyyy-MM-dd format.",
                "cash_counts.invalid_to");
        }

        var result = await useCase.ExecuteAsync(
            new CashCountListCriteria(accountId, start, end), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new CashCountListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetTodayAsync(
        Guid accountId,
        GetCashCountTodayUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(accountId, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var today = result.Value;
        return Results.Ok(new CashCountTodayResponse(
            today.AccountId,
            today.AccountName,
            FinanceContract.Money(today.ExpectedBalance),
            today.Currency.ToString(),
            today.Count is null ? null : ToResponse(today.Count)));
    }

    private static async Task<IResult> CreateAsync(
        CreateCashCountRequest request,
        CreateCashCountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        // Sayılan tutar sıfır olabilir: kasası boşalan esnaf da sayım yapar.
        if (!FinanceContract.TryParseAmount(request.CountedAmount, out var countedAmount))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Counted amount must be a decimal string with at most four decimals.",
                "cash_counts.invalid_counted_amount");
        }

        if (!FinanceContract.TryParseDate(request.CountDate, out var countDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Count date must use the yyyy-MM-dd format.",
                "cash_counts.invalid_count_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "cash_counts.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreateCashCountCommand(
                request.AccountId, countedAmount, countDate, scope, request.Note),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/cash-counts/{response.Id}", response);
    }

    private static async Task<IResult> ConfirmAsync(
        Guid id,
        ConfirmCashCountDifferenceRequest request,
        ConfirmCashCountDifferenceUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConfirmCashCountDifferenceCommand(id, request.CategoryId), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static CashCountResponse ToResponse(CashCountDto count) => new(
        count.Id,
        count.AccountId,
        count.AccountName,
        FinanceContract.Date(count.CountDate),
        FinanceContract.Money(count.CountedAmount),
        count.Currency.ToString(),
        FinanceContract.ScopeValue(count.Scope),
        count.Note,
        count.IsCancelled,
        count.AdjustmentTransactionId,
        count.ExpectedBalance is decimal expected ? FinanceContract.Money(expected) : null,
        count.Difference is decimal difference ? FinanceContract.Money(difference) : null);
}
