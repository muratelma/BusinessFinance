namespace BusinessFinance.Api.Features.SavingsGoals;

public sealed record CreateSavingsGoalRequest(
    string Name,
    string TargetAmount,
    string Currency,
    string TargetDate,
    string TrackingMode,
    Guid? AccountId,
    string? Description,
    string AsOfDate,

    // Hedefin kimin parasını kenara koyduğu; boş bırakmak meşrudur (ADR 0013).
    string? Scope = null);

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
    IReadOnlyList<SavingsGoalContributionResponse> Contributions,
    string? Scope);

public sealed record SavingsGoalScopeTotalsResponse(
    int GoalCount,
    string TargetAmount,
    string AllocatedAmount,
    string RemainingAmount);

/// <summary>
/// Hedeflerin kapsam kırılımı; yalnız filtresiz okumada döner.
/// </summary>
/// <remarks>
/// Üçüncü kova (<see cref="Unscoped"/>) burada vardır: hedef kapsam taşımak
/// zorunda değildir ve etiketsizleri bir tarafa saymak, olmayan bir cevabı
/// uydurmak olurdu.
/// </remarks>
public sealed record SavingsGoalBreakdownResponse(
    SavingsGoalScopeTotalsResponse Business,
    SavingsGoalScopeTotalsResponse Personal,
    SavingsGoalScopeTotalsResponse Unscoped);

public sealed record SavingsGoalListResponse(
    IReadOnlyList<SavingsGoalResponse> Items,
    SavingsGoalBreakdownResponse? ScopeBreakdown = null);
