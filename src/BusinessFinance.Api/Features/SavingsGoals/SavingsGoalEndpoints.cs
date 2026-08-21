using System.Globalization;
using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.SavingsGoals;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.SavingsGoals;

public static class SavingsGoalEndpoints
{
    public static IEndpointRouteBuilder MapSavingsGoalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/goals").WithTags("Savings Goals")
            .RequireAuthorization();
        group.MapPost("/", CreateAsync).Produces<SavingsGoalResponse>(201)
            .ProducesProblem(400).ProducesProblem(401);
        group.MapGet("/", ListAsync).Produces<SavingsGoalListResponse>()
            .ProducesProblem(400).ProducesProblem(401);
        group.MapPost("/{goalId:guid}/contributions", AddContributionAsync)
            .Produces<SavingsGoalResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404);
        group.MapDelete("/{goalId:guid}", DeleteAsync)
            .Produces(204).ProducesProblem(401).ProducesProblem(404).ProducesProblem(409);
        return endpoints;
    }

    private static async Task<IResult> DeleteAsync(
        Guid goalId,
        DeleteSavingsGoalUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new DeleteSavingsGoalCommand(goalId), cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> CreateAsync(
        CreateSavingsGoalRequest request,
        CreateSavingsGoalUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.TargetAmount, out var targetAmount) ||
            targetAmount <= 0m ||
            !FinanceContract.TryParseDate(request.TargetDate, out var targetDate) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOfDate) ||
            !TryTrackingMode(request.TrackingMode, out var mode) ||
            !string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(
                context, "Savings goal fields are invalid.", "goal.invalid_contract");
        var result = await useCase.ExecuteAsync(
            new CreateSavingsGoalCommand(
                request.Name, targetAmount, CurrencyCode.TRY, targetDate, mode,
                request.AccountId, request.Description),
            asOfDate,
            cancellationToken);
        return result.IsSuccess
            ? Results.Created($"/api/v1/goals/{result.Value.Id}", ToResponse(result.Value))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ListAsync(
        string asOfDate,
        ListSavingsGoalsUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var parsedDate))
            return ApiProblemResults.Validation(
                context, "asOfDate must use yyyy-MM-dd.", "goal.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(parsedDate, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new SavingsGoalListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> AddContributionAsync(
        Guid goalId,
        AddSavingsGoalContributionRequest request,
        AddSavingsGoalContributionUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0m ||
            !FinanceContract.TryParseDate(request.ContributionDate, out var contributionDate) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOfDate) ||
            request.ClientRequestId == Guid.Empty ||
            !string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(
                context, "Contribution fields are invalid.", "goal.invalid_contribution");
        var result = await useCase.ExecuteAsync(
            new AddSavingsGoalContributionCommand(
                goalId, amount, CurrencyCode.TRY, contributionDate,
                request.ClientRequestId, request.Note),
            asOfDate,
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(context);
    }

    internal static SavingsGoalResponse ToResponse(SavingsGoalDto goal) => new(
        goal.Id, goal.Name, FinanceContract.Money(goal.TargetAmount), goal.Currency.ToString(),
        FinanceContract.Date(goal.TargetDate),
        goal.TrackingMode == SavingsGoalTrackingMode.AccountBalance
            ? "account-balance"
            : "manual-contributions",
        goal.AccountId, goal.Description, FinanceContract.Money(goal.AllocatedAmount),
        FinanceContract.Money(goal.RemainingAmount),
        goal.ProgressPercentage.ToString("0.0000", CultureInfo.InvariantCulture),
        goal.Status,
        goal.Contributions.Select(item => new SavingsGoalContributionResponse(
            item.Id, FinanceContract.Money(item.Amount), item.Currency.ToString(),
            FinanceContract.Date(item.ContributionDate), item.ClientRequestId,
            item.Note, item.CreatedAtUtc)).ToArray());

    private static bool TryTrackingMode(string value, out SavingsGoalTrackingMode mode)
    {
        mode = string.Equals(value, "account-balance", StringComparison.OrdinalIgnoreCase)
            ? SavingsGoalTrackingMode.AccountBalance
            : SavingsGoalTrackingMode.ManualContributions;
        return string.Equals(value, "account-balance", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "manual-contributions", StringComparison.OrdinalIgnoreCase);
    }
}
