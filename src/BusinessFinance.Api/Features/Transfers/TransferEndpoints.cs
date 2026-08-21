using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Transfers;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Transfers;

public static class TransferEndpoints
{
    public static IEndpointRouteBuilder MapTransferEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/transfers")
            .WithTags("Transfers")
            .RequireAuthorization();

        group.MapPost("/", CreateAsync)
            .WithName("CreateTransfer")
            .Produces<TransferResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/", ListAsync)
            .WithName("ListTransfers")
            .Produces<TransferListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/{transferId:guid}", GetAsync)
            .WithName("GetTransfer")
            .Produces<TransferResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{transferId:guid}", CancelAsync)
            .WithName("CancelTransfer")
            .Produces<TransferResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateTransferRequest request,
        CreateTransferUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "transfers.invalid_amount");
        }

        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only TRY currency is currently supported.",
                "transfers.invalid_currency");
        }

        if (!FinanceContract.TryParseDate(request.TransferDate, out var date))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transfer date must use the yyyy-MM-dd format.",
                "transfers.invalid_date");
        }

        var result = await useCase.ExecuteAsync(
            new CreateTransferCommand(
                request.SourceAccountId,
                request.DestinationAccountId,
                amount,
                CurrencyCode.TRY,
                date,
                request.Description),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/transfers/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListTransfersUseCase useCase,
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

        var result = await useCase.ExecuteAsync(window, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new TransferListResponse(
                result.Value.Items.Select(ToResponse).ToArray(),
                result.Value.HasMore))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetAsync(
        Guid transferId,
        GetTransferUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(transferId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CancelAsync(
        Guid transferId,
        CancelTransferUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CancelTransferCommand(transferId),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static TransferResponse ToResponse(TransferDto transfer) => new(
        transfer.Id,
        transfer.SourceAccountId,
        transfer.DestinationAccountId,
        FinanceContract.Money(transfer.Amount),
        transfer.Currency.ToString(),
        FinanceContract.Date(transfer.TransferDate),
        transfer.Description,
        transfer.IsCancelled,
        transfer.CancelledAtUtc);
}
