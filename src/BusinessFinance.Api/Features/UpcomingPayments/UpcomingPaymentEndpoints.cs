using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.UpcomingPayments;

namespace BusinessFinance.Api.Features.UpcomingPayments;

public static class UpcomingPaymentEndpoints
{
    public static IEndpointRouteBuilder MapUpcomingPaymentEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/upcoming-payments", ListAsync)
            .WithTags("Upcoming Payments")
            .WithName("ListUpcomingPayments")
            .RequireAuthorization()
            .Produces<UpcomingPaymentListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        GetUpcomingPaymentsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? asOfDate = null,
        int daysAhead = 30)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedAsOfDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "As-of date must use the yyyy-MM-dd format.",
                "upcoming_payments.invalid_as_of_date");
        }

        var result = await useCase.ExecuteAsync(
            new GetUpcomingPaymentsQuery(parsedAsOfDate, daysAhead), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new UpcomingPaymentListResponse(
                FinanceContract.Date(parsedAsOfDate),
                daysAhead,
                result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static UpcomingPaymentResponse ToResponse(UpcomingPaymentDto payment) => new(
        payment.SourceId,
        SourceTypeValue(payment.SourceType),
        payment.Title,
        FinanceContract.Money(payment.Amount),
        payment.Currency.ToString(),
        FinanceContract.Date(payment.DueDate),
        payment.Timing.ToString().ToLowerInvariant(),
        payment.Description);

    private static string SourceTypeValue(UpcomingPaymentSourceType sourceType) => sourceType switch
    {
        UpcomingPaymentSourceType.RecurringOccurrence => "recurring-occurrence",
        UpcomingPaymentSourceType.CreditCardStatement => "credit-card-statement",
        UpcomingPaymentSourceType.Installment => "installment",
        UpcomingPaymentSourceType.DebtInstallment => "debt-installment",
        UpcomingPaymentSourceType.Obligation => "obligation",
        _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
    };
}
