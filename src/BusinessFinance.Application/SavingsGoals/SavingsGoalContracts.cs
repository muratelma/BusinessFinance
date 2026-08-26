using BusinessFinance.Domain;

namespace BusinessFinance.Application.SavingsGoals;

public sealed record CreateSavingsGoalCommand(
    string Name,
    decimal TargetAmount,
    CurrencyCode Currency,
    DateOnly TargetDate,
    SavingsGoalTrackingMode TrackingMode,
    Guid? AccountId,
    string? Description,

    // Hedefin kimin parasını kenara koyduğu; boş bırakmak meşrudur (ADR 0013).
    TransactionScope? Scope = null);

public sealed record AddSavingsGoalContributionCommand(
    Guid GoalId,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly ContributionDate,
    Guid ClientRequestId,
    string? Note);

public sealed record DeleteSavingsGoalCommand
{
    public DeleteSavingsGoalCommand(Guid goalId)
    {
        if (goalId == Guid.Empty)
            throw new ArgumentException("Savings goal id cannot be empty.", nameof(goalId));
        GoalId = goalId;
    }

    public Guid GoalId { get; }
}

public enum SavingsGoalDeletionResult
{
    Deleted,
    NotFound,
    HasContributions
}

public sealed record SavingsGoalContributionDto(
    Guid Id,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly ContributionDate,
    Guid ClientRequestId,
    string? Note,
    DateTimeOffset CreatedAtUtc);

public sealed record SavingsGoalDto(
    Guid Id,
    string Name,
    decimal TargetAmount,
    CurrencyCode Currency,
    DateOnly TargetDate,
    SavingsGoalTrackingMode TrackingMode,
    Guid? AccountId,
    string? Description,
    decimal AllocatedAmount,
    decimal RemainingAmount,
    decimal ProgressPercentage,
    string Status,
    IReadOnlyList<SavingsGoalContributionDto> Contributions,
    TransactionScope? Scope = null);

/// <summary>
/// Bir kapsamın hedef toplamı.
/// </summary>
/// <remarks>
/// Aylık raporun kapsam kırılımıyla aynı desen: toplamı istemci çıkarmaz,
/// sunucu verir. Üçüncü bir kova burada <b>vardır</b> — hedef kapsam taşımak
/// zorunda değildir ve etiketsiz hedefleri bir tarafa saymak, olmayan bir
/// cevabı uydurmak olurdu.
/// </remarks>
public sealed record SavingsGoalScopeTotalsDto(
    int GoalCount,
    decimal TargetAmount,
    decimal AllocatedAmount,
    decimal RemainingAmount);

public sealed record SavingsGoalBreakdownDto(
    SavingsGoalScopeTotalsDto Business,
    SavingsGoalScopeTotalsDto Personal,
    SavingsGoalScopeTotalsDto Unscoped);

/// <summary>
/// Hedef listesi; kırılım yalnız <b>filtresiz</b> okumada döner.
/// </summary>
public sealed record SavingsGoalListDto(
    IReadOnlyList<SavingsGoalDto> Items,
    SavingsGoalBreakdownDto? ScopeBreakdown);

public interface ISavingsGoalRepository
{
    Task AddAsync(SavingsGoal goal, CancellationToken cancellationToken);
    Task<IReadOnlyList<SavingsGoal>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<SavingsGoal?> FindOwnedByIdAsync(Guid goalId, Guid userId, CancellationToken cancellationToken);
    Task<SavingsGoalContribution?> FindContributionAsync(
        Guid goalId, Guid userId, Guid clientRequestId, CancellationToken cancellationToken);
    Task<SavingsGoalContribution> AddOrGetContributionAsync(
        SavingsGoalContribution contribution, CancellationToken cancellationToken);
    Task<SavingsGoalDeletionResult> DeleteOwnedIfWithoutContributionsAsync(
        Guid goalId, Guid userId, CancellationToken cancellationToken);
}
