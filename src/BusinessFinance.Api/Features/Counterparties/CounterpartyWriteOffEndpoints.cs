using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Counterparties;

/// <summary>
/// Vazgeçme uçları (ADR 0021 İ4): kalan alacaktan ya da borçtan vazgeçmek o gün
/// gider ya da gelir yazar; para hareket etmez.
/// </summary>
public static class CounterpartyWriteOffEndpoints
{
    public static IEndpointRouteBuilder MapCounterpartyWriteOffEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/counterparties/{counterpartyId:guid}/write-offs", CreateAsync)
            .WithName("CreateCounterpartyWriteOff")
            .WithTags("Counterparties")
            .RequireAuthorization()
            .Produces<CounterpartyWriteOffResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        endpoints.MapDelete("/api/v1/counterparty-write-offs/{writeOffId:guid}", CancelAsync)
            .WithName("CancelCounterpartyWriteOff")
            .WithTags("Counterparties")
            .RequireAuthorization()
            .Produces<CounterpartyWriteOffResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        Guid counterpartyId,
        CreateCounterpartyWriteOffRequest request,
        CreateCounterpartyWriteOffUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!CounterpartyEndpoints.TryParseDirection(request.Direction, out var direction))
        {
            return CounterpartyEndpoints.InvalidDirection(httpContext);
        }

        if (!CounterpartyEndpoints.TryValidateMoney(
                request.Amount, request.Currency, httpContext, out var amount, out var error))
        {
            return error!;
        }

        if (!FinanceContract.TryParseDate(request.WriteOffDate, out var writeOffDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Write-off date must use the yyyy-MM-dd format.",
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
            new CreateCounterpartyWriteOffCommand(
                counterpartyId,
                request.CategoryId,
                direction,
                amount,
                CurrencyCode.TRY,
                scope,
                writeOffDate,
                request.Description,
                request.ClientRequestId),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/counterparty-write-offs/{response.Id}", response);
    }

    private static async Task<IResult> CancelAsync(
        Guid writeOffId,
        CancelCounterpartyWriteOffUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(writeOffId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static CounterpartyWriteOffResponse ToResponse(CounterpartyWriteOffDto writeOff) => new(
        writeOff.Id,
        writeOff.CounterpartyId,
        writeOff.CategoryId,
        CounterpartyEndpoints.DirectionValue(writeOff.Direction),
        FinanceContract.Money(writeOff.Amount),
        writeOff.Currency.ToString(),
        FinanceContract.ScopeValue(writeOff.Scope),
        FinanceContract.Date(writeOff.WriteOffDate),
        writeOff.Description,
        writeOff.IsCancelled);
}
