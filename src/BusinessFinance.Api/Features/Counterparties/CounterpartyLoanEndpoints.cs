using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Counterparties;

/// <summary>
/// Borç para uçları (ADR 0022 İ3): kişinin bakiyesini artırır, parayı
/// taşır, gelir ya da gider yazmaz.
/// </summary>
public static class CounterpartyLoanEndpoints
{
    public static IEndpointRouteBuilder MapCounterpartyLoanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/counterparties/{counterpartyId:guid}/loans", CreateAsync)
            .WithName("CreateCounterpartyLoan")
            .WithTags("Counterparties")
            .RequireAuthorization()
            .Produces<CounterpartyLoanResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        endpoints.MapDelete("/api/v1/counterparty-loans/{loanId:guid}", CancelAsync)
            .WithName("CancelCounterpartyLoan")
            .WithTags("Counterparties")
            .RequireAuthorization()
            .Produces<CounterpartyLoanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        Guid counterpartyId,
        CreateCounterpartyLoanRequest request,
        CreateCounterpartyLoanUseCase useCase,
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

        if (!FinanceContract.TryParseDate(request.LoanDate, out var loanDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Loan date must use the yyyy-MM-dd format.",
                "counterparties.invalid_date");
        }

        DateOnly? dueDate = null;
        if (!string.IsNullOrWhiteSpace(request.DueDate))
        {
            if (!FinanceContract.TryParseDate(request.DueDate, out var parsedDueDate))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Due date must use the yyyy-MM-dd format or be empty.",
                    "counterparties.invalid_due_date");
            }

            dueDate = parsedDueDate;
        }

        var result = await useCase.ExecuteAsync(
            new CreateCounterpartyLoanCommand(
                counterpartyId,
                request.AccountId,
                direction,
                amount,
                CurrencyCode.TRY,
                loanDate,
                dueDate,
                request.Description,
                request.ClientRequestId),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/counterparty-loans/{response.Id}", response);
    }

    private static async Task<IResult> CancelAsync(
        Guid loanId,
        CancelCounterpartyLoanUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(loanId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static CounterpartyLoanResponse ToResponse(CounterpartyLoanDto loan) => new(
        loan.Id,
        loan.CounterpartyId,
        loan.AccountId,
        CounterpartyEndpoints.DirectionValue(loan.Direction),
        FinanceContract.Money(loan.Amount),
        loan.Currency.ToString(),
        FinanceContract.Date(loan.LoanDate),
        loan.DueDate is DateOnly dueDate ? FinanceContract.Date(dueDate) : null,
        loan.Description,
        loan.IsCancelled);
}
