using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Pos;

public static class PosSettlementEndpoints
{
    public static IEndpointRouteBuilder MapPosSettlementEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/pos-settlements", ListAsync)
            .WithTags("PosSettlements")
            .WithName("ListPosSettlements")
            .RequireAuthorization()
            .Produces<PosSettlementListResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapPost("/api/v1/pos-settlements", CreateAsync)
            .WithTags("PosSettlements")
            .WithName("CreatePosSettlement")
            .RequireAuthorization()
            .Produces<PosSettlementResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        endpoints.MapPost("/api/v1/pos-settlements/{id:guid}/transfer", MarkTransferredAsync)
            .WithTags("PosSettlements")
            .WithName("MarkPosSettlementTransferred")
            .RequireAuthorization()
            .Produces<PosSettlementResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        bool? inTransitOnly,
        string? from,
        string? to,
        ListPosSettlementsUseCase useCase,
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
                "pos_settlements.invalid_from");
        }

        var end = today;
        if (to is not null && !FinanceContract.TryParseDate(to, out end))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "End date must use the yyyy-MM-dd format.",
                "pos_settlements.invalid_to");
        }

        var result = await useCase.ExecuteAsync(
            new PosSettlementListCriteria(inTransitOnly ?? false, start, end),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new PosSettlementListResponse(
                result.Value.Items.Select(ToResponse).ToArray(),
                FinanceContract.Money(result.Value.MoneyInTransit),
                result.Value.InTransitCount))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateAsync(
        CreatePosSettlementRequest request,
        CreatePosSettlementUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.GrossAmount, out var grossAmount))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Gross amount must be a decimal string with at most four decimals.",
                "pos_settlements.invalid_gross_amount");
        }

        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only TRY is supported.",
                "pos_settlements.invalid_currency");
        }

        decimal? commissionAmount = null;
        if (request.CommissionAmount is not null)
        {
            if (!FinanceContract.TryParseAmount(request.CommissionAmount, out var parsed))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Commission amount must be a decimal string with at most four decimals.",
                    "pos_settlements.invalid_commission_amount");
            }

            commissionAmount = parsed;
        }

        decimal? commissionRate = null;
        if (request.CommissionRate is not null)
        {
            if (!FinanceContract.TryParseAmount(request.CommissionRate, out var parsed))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Commission rate must be a decimal string with at most four decimals.",
                    "pos_settlements.invalid_commission_rate");
            }

            commissionRate = parsed;
        }

        if (!FinanceContract.TryParseDate(request.SettlementDate, out var settlementDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Settlement date must use the yyyy-MM-dd format.",
                "pos_settlements.invalid_settlement_date");
        }

        if (!FinanceContract.TryParseDate(
                request.ExpectedTransferDate, out var expectedTransferDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Expected transfer date must use the yyyy-MM-dd format.",
                "pos_settlements.invalid_expected_transfer_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "pos_settlements.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreatePosSettlementCommand(
                request.AccountId,
                request.CategoryId,
                grossAmount,
                CurrencyCode.TRY,
                commissionAmount,
                commissionRate,
                request.CommissionCategoryId,
                scope,
                settlementDate,
                expectedTransferDate,
                request.Description),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/pos-settlements/{response.Id}", response);
    }

    private static async Task<IResult> MarkTransferredAsync(
        Guid id,
        MarkPosSettlementTransferredRequest request,
        MarkPosSettlementTransferredUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.TransferDate, out var transferDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transfer date must use the yyyy-MM-dd format.",
                "pos_settlements.invalid_transfer_date");
        }

        var result = await useCase.ExecuteAsync(
            new MarkPosSettlementTransferredCommand(id, transferDate), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static PosSettlementResponse ToResponse(PosSettlementDto settlement) => new(
        settlement.Id,
        settlement.AccountId,
        settlement.AccountName,
        settlement.CategoryId,
        settlement.CategoryName,
        settlement.CommissionCategoryId,
        settlement.CommissionCategoryName,
        FinanceContract.Money(settlement.GrossAmount),
        FinanceContract.Money(settlement.CommissionAmount),
        FinanceContract.Money(settlement.NetAmount),
        FinanceContract.Money(settlement.CommissionRate),
        settlement.Currency.ToString(),
        FinanceContract.ScopeValue(settlement.Scope),
        FinanceContract.Date(settlement.SettlementDate),
        FinanceContract.Date(settlement.ExpectedTransferDate),
        settlement.TransferredOn is DateOnly transferredOn
            ? FinanceContract.Date(transferredOn)
            : null,
        settlement.Description,
        settlement.IsInTransit,
        settlement.IsCancelled,
        settlement.IsLate);
}
