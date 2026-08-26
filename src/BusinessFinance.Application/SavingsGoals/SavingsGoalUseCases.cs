using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.SavingsGoals;

public static class SavingsGoalErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "auth.authentication_required", "Authentication is required.", ApplicationErrorType.Unauthorized);
    public static readonly ApplicationError AccountUnavailable = new(
        "goal.account_unavailable", "An active owned account with matching currency is required.",
        ApplicationErrorType.Validation);
    public static ApplicationError NotFound(Guid id) => new(
        "goal.not_found", $"Savings goal '{id}' was not found.", ApplicationErrorType.NotFound);
    public static ApplicationError Validation(string message) => new(
        "goal.validation", message, ApplicationErrorType.Validation);
    public static readonly ApplicationError HasContributions = new(
        "goal.has_contributions",
        "Katkı geçmişi bulunan tasarruf hedefi silinemez.",
        ApplicationErrorType.Conflict);
}

public sealed class DeleteSavingsGoalUseCase(
    ICurrentUser currentUser,
    ISavingsGoalRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(
        DeleteSavingsGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<Guid>.Failure(SavingsGoalErrors.AuthenticationRequired);

        var result = await repository.DeleteOwnedIfWithoutContributionsAsync(
            command.GoalId, userId, cancellationToken);
        return result switch
        {
            SavingsGoalDeletionResult.Deleted =>
                ApplicationResult<Guid>.Success(command.GoalId),
            SavingsGoalDeletionResult.NotFound =>
                ApplicationResult<Guid>.Failure(SavingsGoalErrors.NotFound(command.GoalId)),
            SavingsGoalDeletionResult.HasContributions =>
                ApplicationResult<Guid>.Failure(SavingsGoalErrors.HasContributions),
            _ => throw new InvalidOperationException("Unknown savings goal deletion result.")
        };
    }
}

public sealed class CreateSavingsGoalUseCase(
    ICurrentUser currentUser,
    ISavingsGoalRepository repository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<SavingsGoalDto>> ExecuteAsync(
        CreateSavingsGoalCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.AuthenticationRequired);
        if (command.TrackingMode == SavingsGoalTrackingMode.AccountBalance)
        {
            var account = command.AccountId is Guid accountId
                ? await accountRepository.FindOwnedByIdAsync(accountId, userId, cancellationToken)
                : null;
            if (account is null || !account.IsActive || account.Currency != command.Currency)
                return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.AccountUnavailable);
        }

        try
        {
            var goal = new SavingsGoal(
                Guid.NewGuid(), userId, command.Name,
                new Money(command.TargetAmount, command.Currency), command.TargetDate,
                command.TrackingMode, command.AccountId, timeProvider.GetUtcNow(),
                command.Description, command.Scope);
            await repository.AddAsync(goal, cancellationToken);
            var allocated = await CalculateAllocatedAsync(
                goal, userId, asOfDate, accountRepository, cancellationToken);
            return ApplicationResult<SavingsGoalDto>.Success(ToDto(goal, allocated, asOfDate));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.Validation(exception.Message));
        }
    }

    internal static async Task<decimal> CalculateAllocatedAsync(
        SavingsGoal goal,
        Guid userId,
        DateOnly asOfDate,
        IAccountRepository accountRepository,
        CancellationToken cancellationToken) =>
        goal.TrackingMode == SavingsGoalTrackingMode.AccountBalance
            ? Math.Max(0m, await accountRepository.CalculateBalanceAsync(
                goal.AccountId!.Value, userId, cancellationToken))
            : goal.Contributions
                .Where(item => item.ContributionDate <= asOfDate)
                .Sum(item => item.Amount.Amount);

    internal static SavingsGoalDto ToDto(SavingsGoal goal, decimal allocated, DateOnly asOfDate)
    {
        var remaining = Math.Max(0m, goal.TargetAmount.Amount - allocated);
        var progress = decimal.Round(
            Math.Min(100m, allocated / goal.TargetAmount.Amount * 100m), 4);
        var status = remaining == 0m ? "completed" : goal.TargetDate < asOfDate ? "overdue" : "active";
        return new SavingsGoalDto(
            goal.Id, goal.Name, goal.TargetAmount.Amount, goal.TargetAmount.Currency,
            goal.TargetDate, goal.TrackingMode, goal.AccountId, goal.Description,
            allocated, remaining, progress, status,
            goal.Contributions.OrderBy(item => item.ContributionDate).ThenBy(item => item.Id)
                .Select(item => new SavingsGoalContributionDto(
                    item.Id, item.Amount.Amount, item.Amount.Currency, item.ContributionDate,
                    item.ClientRequestId, item.Note, item.CreatedAtUtc))
                .ToArray(),
            goal.Scope);
    }
}

public sealed class ListSavingsGoalsUseCase(
    ICurrentUser currentUser,
    ISavingsGoalRepository repository,
    IAccountRepository accountRepository)
{
    /// <summary>
    /// Hedefleri listeler.
    /// </summary>
    /// <remarks>
    /// <paramref name="scope"/> verilirse liste yalnız o kapsamı taşır ve
    /// <b>kapsamsız hedefler de düşer</b> — filtreli okumanın her yerdeki
    /// kuralı bu (ADR 0013). Kırılım yalnız filtresiz okumada döner: filtreli
    /// okumada zaten tek kova var.
    /// </remarks>
    public async Task<ApplicationResult<SavingsGoalListDto>> ExecuteAsync(
        DateOnly asOfDate,
        TransactionScope? scope = null,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<SavingsGoalListDto>.Failure(
                SavingsGoalErrors.AuthenticationRequired);
        var goals = await repository.ListAsync(userId, cancellationToken);
        var all = new List<SavingsGoalDto>(goals.Count);
        foreach (var goal in goals)
        {
            var allocated = await CreateSavingsGoalUseCase.CalculateAllocatedAsync(
                goal, userId, asOfDate, accountRepository, cancellationToken);
            all.Add(CreateSavingsGoalUseCase.ToDto(goal, allocated, asOfDate));
        }

        if (scope is TransactionScope requested)
        {
            return ApplicationResult<SavingsGoalListDto>.Success(new SavingsGoalListDto(
                [.. all.Where(goal => goal.Scope == requested)],
                null));
        }

        return ApplicationResult<SavingsGoalListDto>.Success(new SavingsGoalListDto(
            all,
            new SavingsGoalBreakdownDto(
                Totals(all, TransactionScope.Business),
                Totals(all, TransactionScope.Personal),
                Totals(all, null))));
    }

    private static SavingsGoalScopeTotalsDto Totals(
        IReadOnlyList<SavingsGoalDto> goals,
        TransactionScope? scope)
    {
        var bucket = goals.Where(goal => goal.Scope == scope).ToArray();
        return new SavingsGoalScopeTotalsDto(
            bucket.Length,
            bucket.Sum(goal => goal.TargetAmount),
            bucket.Sum(goal => goal.AllocatedAmount),
            bucket.Sum(goal => goal.RemainingAmount));
    }
}

public sealed class AddSavingsGoalContributionUseCase(
    ICurrentUser currentUser,
    ISavingsGoalRepository repository,
    IAccountRepository accountRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<SavingsGoalDto>> ExecuteAsync(
        AddSavingsGoalContributionCommand command,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.AuthenticationRequired);
        var goal = await repository.FindOwnedByIdAsync(command.GoalId, userId, cancellationToken);
        if (goal is null)
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.NotFound(command.GoalId));
        try
        {
            var existing = await repository.FindContributionAsync(
                goal.Id, userId, command.ClientRequestId, cancellationToken);
            if (existing is null)
            {
                var contribution = goal.AddContribution(
                    Guid.NewGuid(), new Money(command.Amount, command.Currency),
                    command.ContributionDate, command.ClientRequestId, timeProvider.GetUtcNow(), command.Note);
                var persisted = await repository.AddOrGetContributionAsync(contribution, cancellationToken);
                if (persisted.Id != contribution.Id)
                {
                    goal = await repository.FindOwnedByIdAsync(
                        command.GoalId, userId, cancellationToken) ?? goal;
                }
            }
            var allocated = await CreateSavingsGoalUseCase.CalculateAllocatedAsync(
                goal, userId, asOfDate, accountRepository, cancellationToken);
            return ApplicationResult<SavingsGoalDto>.Success(
                CreateSavingsGoalUseCase.ToDto(goal, allocated, asOfDate));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<SavingsGoalDto>.Failure(SavingsGoalErrors.Validation(exception.Message));
        }
    }
}
