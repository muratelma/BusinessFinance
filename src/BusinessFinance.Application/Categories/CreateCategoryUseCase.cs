using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public sealed record CreateCategoryCommand(
    string Name,
    CategoryType Type,
    TransactionScope? DefaultScope = null,
    bool IsTax = false);

public sealed class CreateCategoryUseCase(
    ICurrentUser currentUser,
    ICategoryRepository repository,
    IUserProfileRepository profileRepository)
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

        // İşletmesi olmayan kullanıcıya taraf sorulmaz; açtığı kategori şahsi
        // yazılır (ADR 0020 İ12). İşletmesi olan kullanıcıda boş taraf
        // "iki tarafa açık" demektir ve öyle kalır.
        var defaultScope = command.DefaultScope;
        if (defaultScope is null)
        {
            var profile = await profileRepository.FindAsync(
                userId, track: false, cancellationToken);
            if (!(profile?.HasBusiness ?? false))
            {
                defaultScope = TransactionScope.Personal;
            }
        }

        Category category;
        try
        {
            category = new Category(
                Guid.NewGuid(),
                userId,
                command.Name,
                command.Type,
                defaultScope,
                command.IsTax);
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
        category.DefaultScope,
        category.IsTax);
}
