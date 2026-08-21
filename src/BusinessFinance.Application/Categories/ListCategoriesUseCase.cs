using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public sealed record ListCategoriesQuery(CategoryType? Type, bool? IsActive);

public sealed class ListCategoriesUseCase(
    ICurrentUser currentUser,
    ICategoryRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyList<CategoryListItemDto>>> ExecuteAsync(
        ListCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<CategoryListItemDto>>.Failure(
                CategoryErrors.AuthenticationRequired);
        }

        await repository.EnsureDefaultsAsync(userId, cancellationToken);
        var categories = await repository.ListAsync(userId, query.Type, query.IsActive, cancellationToken);
        return ApplicationResult<IReadOnlyList<CategoryListItemDto>>.Success(
            categories.Select(CreateCategoryUseCase.ToDto).ToArray());
    }
}
