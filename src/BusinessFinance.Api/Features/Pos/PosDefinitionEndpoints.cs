using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Pos;

namespace BusinessFinance.Api.Features.Pos;

/// <summary>
/// POS tanımının tam hâli (ADR 0019 T4). Oran ondalık kesirdir:
/// <c>"0.0179"</c> = %1,79; sıfır meşrudur.
/// </summary>
public sealed record SavePosDefinitionRequest(
    string Name,
    Guid AccountId,
    Guid SalesCategoryId,
    string CommissionRate,
    int TransferDays,
    bool BusinessDaysOnly,
    Guid? CommissionCategoryId = null);

public sealed record SetPosDefinitionActiveRequest(bool IsActive);

public sealed record PosDefinitionResponse(
    Guid Id,
    string Name,
    Guid AccountId,
    string AccountName,
    Guid SalesCategoryId,
    string SalesCategoryName,
    Guid? CommissionCategoryId,
    string? CommissionCategoryName,
    string CommissionRate,
    int TransferDays,
    bool BusinessDaysOnly,
    bool IsActive,
    bool IsDefault);

public sealed record PosDefinitionListResponse(IReadOnlyList<PosDefinitionResponse> Items);

/// <summary>
/// Tanımla yazılacak tahsilatın önizlemesi. Para istemcide hesaplanmaz; form
/// canlı neti buradan okur.
/// </summary>
public sealed record PosSettlementPreviewResponse(
    string GrossAmount,
    string CommissionAmount,
    string NetAmount,
    string Currency,
    string SettlementDate,
    string ExpectedTransferDate);

public static class PosDefinitionEndpoints
{
    public static IEndpointRouteBuilder MapPosDefinitionEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/pos-definitions")
            .WithTags("PosDefinitions")
            .RequireAuthorization();

        group.MapGet("/", ListAsync)
            .WithName("ListPosDefinitions")
            .Produces<PosDefinitionListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/", CreateAsync)
            .WithName("CreatePosDefinition")
            .Produces<PosDefinitionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdatePosDefinition")
            .Produces<PosDefinitionResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPatch("/{id:guid}/active", SetActiveAsync)
            .WithName("SetPosDefinitionActive")
            .Produces<PosDefinitionResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // Ana POS: tahsilat formunda seçili gelen; kullanıcı başına bir tane.
        group.MapPut("/{id:guid}/default", SetDefaultAsync)
            .WithName("SetDefaultPosDefinition")
            .Produces<PosDefinitionResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        // Tahsilatı olan tanım silinmez (409), pasife alınır.
        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeletePosDefinition")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/{id:guid}/preview", PreviewAsync)
            .WithName("PreviewPosSettlement")
            .Produces<PosSettlementPreviewResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        ListPosDefinitionsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new PosDefinitionListResponse([.. result.Value.Select(ToResponse)]))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> CreateAsync(
        SavePosDefinitionRequest request,
        CreatePosDefinitionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryCreateCommand(request, httpContext, out var command, out var error))
        {
            return error!;
        }

        var result = await useCase.ExecuteAsync(command!, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/pos-definitions/{response.Id}", response);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        SavePosDefinitionRequest request,
        UpdatePosDefinitionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryCreateCommand(request, httpContext, out var command, out var error))
        {
            return error!;
        }

        var result = await useCase.ExecuteAsync(id, command!, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> SetActiveAsync(
        Guid id,
        SetPosDefinitionActiveRequest request,
        SetPosDefinitionActiveUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, request.IsActive, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> SetDefaultAsync(
        Guid id,
        SetDefaultPosDefinitionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeletePosDefinitionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> PreviewAsync(
        Guid id,
        string? grossAmount,
        string? settlementDate,
        PreviewPosSettlementUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(grossAmount, out var gross) || gross <= 0m)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Gross amount must be greater than zero and have at most four decimal places.",
                "pos_definitions.invalid_gross_amount");
        }

        if (!FinanceContract.TryParseDate(settlementDate, out var date))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Settlement date must use the yyyy-MM-dd format.",
                "pos_definitions.invalid_settlement_date");
        }

        var result = await useCase.ExecuteAsync(id, gross, date, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var value = result.Value;
        return Results.Ok(new PosSettlementPreviewResponse(
            FinanceContract.Money(value.GrossAmount),
            FinanceContract.Money(value.CommissionAmount),
            FinanceContract.Money(value.NetAmount),
            value.Currency.ToString(),
            FinanceContract.Date(value.SettlementDate),
            FinanceContract.Date(value.ExpectedTransferDate)));
    }

    private static bool TryCreateCommand(
        SavePosDefinitionRequest request,
        HttpContext httpContext,
        out SavePosDefinitionCommand? command,
        out IResult? error)
    {
        command = null;
        error = null;
        if (!FinanceContract.TryParseAmount(request.CommissionRate, out var rate))
        {
            error = ApiProblemResults.Validation(
                httpContext,
                "Commission rate must be a decimal string with at most four decimals.",
                "pos_definitions.invalid_commission_rate");
            return false;
        }

        command = new SavePosDefinitionCommand(
            request.Name,
            request.AccountId,
            request.SalesCategoryId,
            rate,
            request.CommissionCategoryId,
            request.TransferDays,
            request.BusinessDaysOnly);
        return true;
    }

    private static PosDefinitionResponse ToResponse(PosDefinitionDto definition) => new(
        definition.Id,
        definition.Name,
        definition.AccountId,
        definition.AccountName,
        definition.SalesCategoryId,
        definition.SalesCategoryName,
        definition.CommissionCategoryId,
        definition.CommissionCategoryName,
        FinanceContract.Money(definition.CommissionRate),
        definition.TransferDays,
        definition.BusinessDaysOnly,
        definition.IsActive,
        definition.IsDefault);
}
