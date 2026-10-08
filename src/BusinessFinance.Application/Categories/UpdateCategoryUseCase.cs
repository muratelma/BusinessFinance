using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

/// <summary>
/// Kategorinin tam güncel hâli; <see cref="DefaultScope"/> yetkilidir ve boş
/// gönderilmesi kategoriyi iki tarafa açar.
/// </summary>
/// <remarks>
/// Kategorinin tarafı <b>yalnız genişler</b> (ADR 0020 İ6): tek taraflı
/// kategori her zaman iki tarafa açılabilir; daraltmak ya da çevirmek yalnız
/// öbür tarafta kayıt, plan ve bütçe yoksa mümkündür. Yazılmış kayıt yeniden
/// yorumlanmaz; aksi hâlde kategorisinin izin vermediği bir tarafta dururdu.
/// </remarks>
/// <param name="IsTax">
/// Vergi işareti (ADR 0018 T6); boşsa değişmez. Alanı bilmeyen bir istemci
/// kategoriyi yeniden adlandırırken işareti sessizce kaldırmasın diye.
/// </param>
public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    bool IsActive,
    TransactionScope? DefaultScope,
    bool? IsTax = null);

public sealed class UpdateCategoryUseCase(
    ICurrentUser currentUser,
    ICategoryRepository repository,
    ICategoryUsageReader usageReader)
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

        if (command.DefaultScope is TransactionScope side &&
            category.DefaultScope != side &&
            await usageReader.IsUsedOutsideAsync(category.Id, userId, side, cancellationToken))
        {
            return ApplicationResult<CategoryListItemDto>.Failure(CategoryErrors.ScopeInUse);
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
            if (command.IsTax is bool isTax) category.SetTax(isTax);

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
