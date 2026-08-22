using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public sealed record CreateCategoryCommand(
    string Name,
    CategoryType Type,
    TransactionScope? DefaultScope = null);

public sealed class CreateCategoryUseCase(
    ICurrentUser currentUser,
    ICategoryRepository repository)
{
    public async Task<ApplicationResult<CategoryListItemDto>> ExecuteAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CategoryListItemDto>.Failure(CategoryErrors.AuthenticationRequired);
        }

        await repository.EnsureDefaultsAsync(userId, cancellationToken);

        Category category;
        try
        {
            category = new Category(
                Guid.NewGuid(), userId, command.Name, command.Type, command.DefaultScope);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<CategoryListItemDto>.Failure(
                CategoryErrors.Validation(exception.Message));
        }

        if (await repository.ExistsByNameAndTypeAsync(
                userId,
                category.Name,
                category.Type,
                cancellationToken))
        {
            return ApplicationResult<CategoryListItemDto>.Failure(
                CategoryErrors.DuplicateName(category.Name));
        }

        await repository.AddAsync(category, cancellationToken);
        return ApplicationResult<CategoryListItemDto>.Success(ToDto(category));
    }

    internal static CategoryListItemDto ToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Type,
        category.IsActive,
        category.DefaultScope);
}
