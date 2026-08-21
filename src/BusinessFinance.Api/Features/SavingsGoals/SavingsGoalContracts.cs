namespace BusinessFinance.Api.Features.SavingsGoals;

public sealed record CreateSavingsGoalRequest(
    string Name,
    string TargetAmount,
    string Currency,
    string TargetDate,
    string TrackingMode,
    Guid? AccountId,
    string? Description,
    string AsOfDate);

public sealed record AddSavingsGoalContributionRequest(
    string Amount,
    string Currency,
    string ContributionDate,
    Guid ClientRequestId,
    string? Note,
    string AsOfDate);

public sealed record SavingsGoalContributionResponse(
    Guid Id,
    string Amount,
    string Currency,
    string ContributionDate,
    Guid ClientRequestId,
    string? Note,
    DateTimeOffset CreatedAtUtc);

public sealed record SavingsGoalResponse(
    Guid Id,
    string Name,
    string TargetAmount,
    string Currency,
    string TargetDate,
    string TrackingMode,
    Guid? AccountId,
    string? Description,
    string AllocatedAmount,
    string RemainingAmount,
    string ProgressPercentage,
    string Status,
    IReadOnlyList<SavingsGoalContributionResponse> Contributions);

public sealed record SavingsGoalListResponse(IReadOnlyList<SavingsGoalResponse> Items);
