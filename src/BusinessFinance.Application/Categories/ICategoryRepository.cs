using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public interface ICategoryRepository
{
    Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAndTypeAsync(
        Guid userId,
        string name,
        CategoryType type,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> ListAsync(
        Guid userId,
        CategoryType? type,
        bool? isActive,
        CancellationToken cancellationToken);
    Task<Category?> FindOwnedByIdAsync(
        Guid categoryId,
        Guid userId,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(Category category, Guid userId, CancellationToken cancellationToken);
}
