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
        group.MapPost("/preview", PreviewAsync);
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

        decimal feeAmount = 0m;
        if (request.FeeAmount is not null &&
            (!FinanceContract.TryParseAmount(request.FeeAmount, out feeAmount) || feeAmount < 0m))
            return ApiProblemResults.Validation(httpContext, "Fee must be nonnegative with at most four decimals.", "transfers.invalid_fee");
        TransactionScope? feeScope = null;
        if (request.FeeScope is not null)
        {
            if (!Enum.TryParse<TransactionScope>(request.FeeScope, true, out var parsed) || !Enum.IsDefined(parsed))
                return ApiProblemResults.Validation(httpContext, "Fee scope is invalid.", "transfers.invalid_scope");
            feeScope = parsed;
        }
        var result = await useCase.ExecuteAsync(
            new CreateTransferCommand(
                request.SourceAccountId,
                request.DestinationAccountId,
                amount,
                CurrencyCode.TRY,
                date,
                request.Description, feeAmount, feeScope),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/transfers/{response.Id}", response);
    }

    private static async Task<IResult> PreviewAsync(CreateTransferRequest request, PreviewTransferUseCase useCase,
        HttpContext context, CancellationToken ct)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0m ||
            !FinanceContract.TryParseDate(request.TransferDate, out var date) ||
            !string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(context, "The transfer input is invalid.", "transfers.validation");
        var fee = 0m;
        if (request.FeeAmount is not null && (!FinanceContract.TryParseAmount(request.FeeAmount, out fee) || fee < 0m))
            return ApiProblemResults.Validation(context, "Fee is invalid.", "transfers.invalid_fee");
        TransactionScope? scope = null;
        if (request.FeeScope is not null)
        {
            if (!Enum.TryParse<TransactionScope>(request.FeeScope, true, out var value) || !Enum.IsDefined(value))
                return ApiProblemResults.Validation(context, "Fee scope is invalid.", "transfers.invalid_scope");
            scope = value;
        }
        var result = await useCase.ExecuteAsync(new(request.SourceAccountId, request.DestinationAccountId,
            amount, CurrencyCode.TRY, date, request.Description, fee, scope), ct);
        if (!result.IsSuccess) return result.Error.ToProblemResult(context);
        var p = result.Value;
        return Results.Ok(new TransferPreviewResponse(p.SourceAccountId, p.SourceAccountName, FinanceContract.Money(p.TotalOutflow),
            FinanceContract.Money(p.FeeAmount), p.FeeCategoryId, p.FeeCategoryName, FinanceContract.OptionalScopeValue(p.ResolvedScope), p.NeedsScope));
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
        transfer.CancelledAtUtc,
        transfer.FeeTransactionId,
        transfer.FeeAmount > 0m ? FinanceContract.Money(transfer.FeeAmount) : null,
        transfer.FeeCategoryId,
        FinanceContract.OptionalScopeValue(transfer.FeeScope));
}
