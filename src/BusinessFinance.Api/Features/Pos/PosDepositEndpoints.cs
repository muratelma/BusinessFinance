using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Pos;

namespace BusinessFinance.Api.Features.Pos;

/// <summary>
/// "Hesaba geçenleri işaretle" (ADR 0019 T5): seçilen yoldaki tahsilatlar,
/// bankanın gerçekten yatırdığı tutar ve gün.
/// </summary>
/// <remarks>
/// <c>ClientRequestId</c> isteği idempotent yapar: aynı kimlikle tekrar gelen
/// istek ilk yatışı döner. <c>DeductionCategoryId</c> yalnız yatan tutar
/// beklenenden azsa kullanılır; boşsa tahsilatların POS'undaki komisyon
/// kategorisi uygulanır.
/// </remarks>
public sealed record CreatePosDepositRequest(
    Guid ClientRequestId,
    IReadOnlyList<Guid> SettlementIds,
    string DepositedAmount,
    string DepositDate,
    Guid? DeductionCategoryId = null,
    string? Scope = null);

/// <summary>
/// Yatışın önizlemesi. Beklenen toplam ve kesinti istemcide hesaplanmaz; form
/// buradan okur.
/// </summary>
public sealed record PosDepositPreviewResponse(
    Guid AccountId,
    string AccountName,
    int SettlementCount,
    string ExpectedAmount,
    string DepositedAmount,
    string DeductionAmount,
    bool ExceedsExpected,
    Guid? DeductionCategoryId,
    string? DeductionCategoryName,
    string Currency,
    string EarliestDepositDate);

public sealed record PosDepositResponse(
    Guid Id,
    Guid AccountId,
    string AccountName,
    string DepositDate,
    string ExpectedAmount,
    string DepositedAmount,
    string DeductionAmount,
    Guid? DeductionTransactionId,
    Guid? DeductionCategoryId,
    string? DeductionCategoryName,
    string Currency,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    IReadOnlyList<PosSettlementResponse> Settlements,
    // Yatıştan hemen sonra hesabın bakiyesi; yalnız `GET` cevabında dolar.
    string? BalanceAfter = null,
    // Kapattığı tahsilatların brüt satış ve komisyon toplamı; geri alınmış
    // yatışta boş. Komisyon satış günü gider yazılmıştır, burada bilgidir.
    string? GrossAmount = null,
    string? CommissionAmount = null);

public static class PosDepositEndpoints
{
    public static IEndpointRouteBuilder MapPosDepositEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/pos-deposits")
            .WithTags("PosDeposits")
            .RequireAuthorization();

        group.MapGet("/preview", PreviewAsync)
            .WithName("PreviewPosDeposit")
            .Produces<PosDepositPreviewResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPost("/", CreateAsync)
            .WithName("CreatePosDeposit")
            .Produces<PosDepositResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/{id:guid}", GetAsync)
            .WithName("GetPosDeposit")
            .Produces<PosDepositResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // Silme yerine geri alma: yatış kaydı kalır, kapattığı tahsilatlar
        // yola döner ve kesinti gideri iptal olur.
        group.MapDelete("/{id:guid}", RevertAsync)
            .WithName("RevertPosDeposit")
            .Produces<PosDepositResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> PreviewAsync(
        Guid[]? settlementIds,
        string? depositedAmount,
        PreviewPosDepositUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        decimal? amount = null;
        if (depositedAmount is not null)
        {
            if (!FinanceContract.TryParseAmount(depositedAmount, out var parsed))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Deposited amount must be a decimal string with at most four decimals.",
                    "pos_deposits.invalid_amount");
            }

            amount = parsed;
        }

        var result = await useCase.ExecuteAsync(
            new PreviewPosDepositQuery(settlementIds ?? [], amount), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var preview = result.Value;
        return Results.Ok(new PosDepositPreviewResponse(
            preview.AccountId,
            preview.AccountName,
            preview.SettlementCount,
            FinanceContract.Money(preview.ExpectedAmount),
            FinanceContract.Money(preview.DepositedAmount),
            FinanceContract.Money(preview.DeductionAmount),
            preview.ExceedsExpected,
            preview.DeductionCategoryId,
            preview.DeductionCategoryName,
            preview.Currency.ToString(),
            FinanceContract.Date(preview.EarliestDepositDate)));
    }

    private static async Task<IResult> CreateAsync(
        CreatePosDepositRequest request,
        CreatePosDepositUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.DepositedAmount, out var depositedAmount))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Deposited amount must be a decimal string with at most four decimals.",
                "pos_deposits.invalid_amount");
        }

        if (!FinanceContract.TryParseDate(request.DepositDate, out var depositDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Deposit date must use the yyyy-MM-dd format.",
                "pos_deposits.invalid_deposit_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Scope must be business, personal or empty.",
                "pos_deposits.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreatePosDepositCommand(
                request.ClientRequestId,
                request.SettlementIds ?? [],
                depositedAmount,
                depositDate,
                request.DeductionCategoryId,
                scope),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/pos-deposits/{response.Id}", response);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetPosDepositUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RevertAsync(
        Guid id,
        RevertPosDepositUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static PosDepositResponse ToResponse(PosDepositDto deposit) => new(
        deposit.Id,
        deposit.AccountId,
        deposit.AccountName,
        FinanceContract.Date(deposit.DepositDate),
        FinanceContract.Money(deposit.ExpectedAmount),
        FinanceContract.Money(deposit.DepositedAmount),
        FinanceContract.Money(deposit.DeductionAmount),
        deposit.DeductionTransactionId,
        deposit.DeductionCategoryId,
        deposit.DeductionCategoryName,
        deposit.Currency.ToString(),
        deposit.IsCancelled,
        deposit.CancelledAtUtc,
        deposit.Settlements.Select(PosSettlementEndpoints.ToResponse).ToArray(),
        deposit.BalanceAfter is decimal balance ? FinanceContract.Money(balance) : null,
        deposit.GrossAmount is decimal gross ? FinanceContract.Money(gross) : null,
        deposit.CommissionAmount is decimal commission
            ? FinanceContract.Money(commission)
            : null);
}
