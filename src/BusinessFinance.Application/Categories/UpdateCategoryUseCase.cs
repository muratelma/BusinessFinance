using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

/// <summary>
/// Kategorinin tam güncel hâli; <see cref="DefaultScope"/> yetkilidir ve boş
/// gönderilmesi varsayılanı kaldırır.
/// </summary>
public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    bool IsActive,
    TransactionScope? DefaultScope);

public sealed class UpdateCategoryUseCase(
    ICurrentUser currentUser,
    ICategoryRepository repository)
{
    public async Task<ApplicationResult<CategoryListItemDto>> ExecuteAsync(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CategoryListItemDto>.Failure(CategoryErrors.AuthenticationRequired);
        }

        var category = await repository.FindOwnedByIdAsync(command.CategoryId, userId, cancellationToken);
        if (category is null)
        {
            return ApplicationResult<CategoryListItemDto>.Failure(CategoryErrors.NotFound(command.CategoryId));
        }

        try
        {
            var previousName = category.Name;
            category.Rename(command.Name);
            if (!string.Equals(previousName, category.Name, StringComparison.OrdinalIgnoreCase) &&
                await repository.ExistsByNameAndTypeAsync(
                    userId,
                    category.Name,
                    category.Type,
                    cancellationToken))
            {
                return ApplicationResult<CategoryListItemDto>.Failure(
                    CategoryErrors.DuplicateName(category.Name));
            }

            category.SetDefaultScope(command.DefaultScope);

            if (command.IsActive)
            {
                category.Activate();
            }
            else
            {
                category.Deactivate();
            }
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CategoryListItemDto>.Failure(
                CategoryErrors.Validation(exception.Message));
        }

        await repository.UpdateOwnedAsync(category, userId, cancellationToken);
        return ApplicationResult<CategoryListItemDto>.Success(CreateCategoryUseCase.ToDto(category));
    }
}
