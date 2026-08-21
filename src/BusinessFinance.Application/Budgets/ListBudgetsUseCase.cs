using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Budgets;

public sealed class ListBudgetsUseCase(
    ICurrentUser currentUser,
    IBudgetRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<BudgetDto>>> ExecuteAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<BudgetDto>>.Failure(
                BudgetErrors.AuthenticationRequired);
        }
        if (year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear ||
            month is < 1 or > 12)
        {
            return ApplicationResult<IReadOnlyList<BudgetDto>>.Failure(
                BudgetErrors.Validation("Year or month is outside the supported range."));
        }

        return ApplicationResult<IReadOnlyList<BudgetDto>>.Success(
            await repository.ListWithProgressAsync(userId, year, month, cancellationToken));
    }
}
