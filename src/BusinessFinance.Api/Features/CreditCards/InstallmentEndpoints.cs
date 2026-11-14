using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.CreditCards;

public static class InstallmentEndpoints
{
    public static IEndpointRouteBuilder MapInstallmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/installment-plans")
            .WithTags("Credit Cards")
            .RequireAuthorization();
        group.MapPost("/", CreateAsync)
            .WithName("CreateInstallmentPlan")
            .Produces<InstallmentPlanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapGet("/", ListAsync)
            .WithName("ListInstallmentPlans")
            .Produces<InstallmentPlanListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/preview", Preview)
            .WithName("PreviewInstallmentPlan")
            .Produces<InstallmentPreviewResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/{installmentPlanId:guid}/items/{sequence:int}/realize", RealizeAsync)
            .WithName("RealizeInstallment")
            .Produces<CardChargeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateInstallmentPlanRequest request,
        CreateInstallmentPlanUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.TotalAmount, out var totalAmount) || totalAmount <= 0m)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Total amount must be greater than zero and have at most four decimal places.",
                "installments.invalid_total_amount");
        }
        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext, "Only TRY currency is currently supported.", "installments.invalid_currency");
        }
        if (!FinanceContract.TryParseDate(request.FirstInstallmentDate, out var firstDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "First installment date must use the yyyy-MM-dd format.",
                "installments.invalid_first_date");
        }

        if (!FinanceContract.TryParseOptionalScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Plan scope must be business, personal or empty.",
                "installments.invalid_scope");
        }

        var result = await useCase.ExecuteAsync(new CreateInstallmentPlanCommand(
            request.CreditCardId,
            request.CategoryId,
            request.ClientRequestId,
            totalAmount,
            CurrencyCode.TRY,
            scope,
            request.InstallmentCount,
            firstDate,
            request.Description), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ListAsync(
        ListInstallmentPlansUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new InstallmentPlanListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    /// <summary>
    /// Formun önizlemesi: her taksit sayısı için aylık tutar ve ilk/son gün.
    /// Bölmeyi plan kurulurken kullanılan kuralla sunucu yapar; istemci tutar
    /// hesaplamaz. Kayıt yazmaz.
    /// </summary>
    private static IResult Preview(HttpContext httpContext, string? totalAmount = null,
        string? firstInstallmentDate = null, string? counts = null)
    {
        if (!FinanceContract.TryParseAmount(totalAmount, out var total) || total <= 0m)
        {
            return ApiProblemResults.Validation(httpContext,
                "Total amount must be greater than zero and have at most four decimal places.",
                "installments.invalid_total_amount");
        }
        if (!FinanceContract.TryParseDate(firstInstallmentDate, out var first))
        {
            return ApiProblemResults.Validation(httpContext,
                "First installment date must use the yyyy-MM-dd format.", "installments.invalid_first_date");
        }
        var values = new List<int>();
        foreach (var part in (counts ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(part, out var count) ||
                count is < InstallmentPlan.MinimumInstallmentCount or > InstallmentPlan.MaximumInstallmentCount ||
                values.Contains(count) || values.Count == 12)
            {
                return ApiProblemResults.Validation(httpContext,
                    "Counts must be up to twelve distinct numbers between 2 and 60.", "installments.invalid_count");
            }
            values.Add(count);
        }
        if (values.Count == 0)
        {
            return ApiProblemResults.Validation(httpContext,
                "At least one installment count is required.", "installments.invalid_count");
        }
        return Results.Ok(new InstallmentPreviewResponse(values.Select(count =>
        {
            var schedule = InstallmentPlan.Schedule(total, count, first);
            return new InstallmentPreviewOption(count, FinanceContract.Money(schedule[0].Amount),
                FinanceContract.Money(schedule[^1].Amount), FinanceContract.Date(schedule[0].Date),
                FinanceContract.Date(schedule[^1].Date));
        }).ToArray()));
    }

    private static async Task<IResult> RealizeAsync(
        Guid installmentPlanId,
        int sequence,
        RealizeInstallmentUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RealizeInstallmentCommand(installmentPlanId, sequence), cancellationToken);
        return result.IsSuccess
            ? Results.Ok(CreditCardEndpoints.ToChargeResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static InstallmentPlanResponse ToResponse(InstallmentPlanDto plan) => new(
        plan.Id,
        plan.CreditCardId,
        plan.CategoryId,
        plan.ClientRequestId,
        FinanceContract.Money(plan.TotalAmount),
        plan.Currency.ToString(),
        FinanceContract.ScopeValue(plan.Scope),
        plan.InstallmentCount,
        FinanceContract.Date(plan.FirstInstallmentDate),
        plan.Description,
        plan.Items.Select(item => new InstallmentItemResponse(
            item.Id,
            item.Sequence,
            FinanceContract.Money(item.Amount),
            item.Currency.ToString(),
            FinanceContract.Date(item.ScheduledDate),
            item.IsRealized,
            item.CreditCardChargeId,
            item.RealizedAtUtc)).ToArray());
}

public sealed record InstallmentPreviewOption(int InstallmentCount, string InstallmentAmount,
    string LastInstallmentAmount, string FirstInstallmentDate, string LastInstallmentDate);
public sealed record InstallmentPreviewResponse(IReadOnlyList<InstallmentPreviewOption> Options);
