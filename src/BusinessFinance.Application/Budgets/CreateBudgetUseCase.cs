using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Budgets;

public sealed class CreateBudgetUseCase(
    ICurrentUser currentUser,
    ICategoryRepository categoryRepository,
    IBudgetRepository budgetRepository,
    IUserProfileRepository profileRepository)
{
    public async Task<ApplicationResult<BudgetDto>> ExecuteAsync(
        CreateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.AuthenticationRequired);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId,
            userId,
            cancellationToken);
        if (category is null || !category.IsActive || category.Type != CategoryType.Expense)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.CategoryUnavailable);
        }
        if (await budgetRepository.ExistsAsync(
                userId,
                category.Id,
                command.Year,
                command.Month,
                cancellationToken))
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.DuplicatePeriod);
        }

        // Bütçenin hesabı yoktur: kategori tarafı sınırlar, iki tarafa açıksa
        // kullanıcı seçer.
        var resolution = await TransactionScopeResolution.ResolveAsync(
            command.Scope,
            category.DefaultScope,
            null,
            profileRepository,
            userId,
            cancellationToken);
        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<BudgetDto>.Failure(resolution.ToError(
                BudgetErrors.ScopeUnresolved, BudgetErrors.ScopeConflict));
        }

        MonthlyBudget budget;
        try
        {
            budget = new MonthlyBudget(
                Guid.NewGuid(),
                userId,
                category,
                new Money(command.Limit, command.Currency),
                scope,
                command.Year,
                command.Month);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<BudgetDto>.Failure(BudgetErrors.Validation(exception.Message));
        }

        await budgetRepository.AddAsync(budget, cancellationToken);
        return ApplicationResult<BudgetDto>.Success(new BudgetDto(
            budget.Id,
            budget.CategoryId,
            category.Name,
            budget.Limit.Amount,
            0m,
            budget.Limit.Amount,
            0m,
            budget.Limit.Currency,
            budget.Scope,
            budget.Year,
            budget.Month));
    }
}
