using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Budgets;

public sealed class UpdateBudgetUseCase(
    ICurrentUser currentUser,
    IBudgetRepository repository)
{
    public async Task<ApplicationResult<BudgetDto>> ExecuteAsync(
        UpdateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.AuthenticationRequired);
        }

        var budget = await repository.FindOwnedByIdAsync(command.BudgetId, userId, cancellationToken);
        if (budget is null)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.NotFound(command.BudgetId));
        }

        try
        {
            budget.UpdateLimit(new Money(command.Limit, command.Currency));
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.Validation(exception.Message));
        }

        await repository.UpdateOwnedAsync(budget, userId, cancellationToken);
        var item = (await repository.ListWithProgressAsync(
            userId,
            budget.Year,
            budget.Month,
            cancellationToken)).Single(value => value.Id == budget.Id);
        return ApplicationResult<BudgetDto>.Success(item);
    }
}
