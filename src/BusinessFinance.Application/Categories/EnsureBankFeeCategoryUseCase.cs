using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public sealed class EnsureBankFeeCategoryUseCase(ICurrentUser user, ICategoryRepository categories)
{
    public async Task<ApplicationResult<CategoryListItemDto>> ExecuteAsync(CancellationToken ct)
    {
        if (user.UserId is not Guid owner || owner == Guid.Empty)
            return ApplicationResult<CategoryListItemDto>.Failure(CategoryErrors.AuthenticationRequired);
        var category = await categories.FindOrOpenAsync(owner, CategoryRole.BankFee, ct);
        return ApplicationResult<CategoryListItemDto>.Success(CreateCategoryUseCase.ToDto(category));
    }
}
