namespace BusinessFinance.Api.Features.Categories;

/// <param name="IsTax">
/// Vergi işareti: bu kategorideki giderler Vergiler › Ödenenler'de görünür.
/// Yalnız gider kategorisi işaretlenebilir.
/// </param>
public sealed record CreateCategoryRequest(
    string Name,
    string Type,
    string? DefaultScope = null,
    bool IsTax = false);

/// <summary>
/// Kategorinin tam güncel hâli; <see cref="DefaultScope"/> boş gönderilirse
/// varsayılan kapsam kaldırılır.
/// </summary>
/// <remarks><c>IsTax</c> boş gönderilirse işaret değişmez.</remarks>
public sealed record UpdateCategoryRequest(
    string Name,
    bool IsActive,
    string? DefaultScope = null,
    bool? IsTax = null);
public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Type,
    bool IsActive,
    string? DefaultScope,
    bool IsTax);
public sealed record CategoryListResponse(IReadOnlyList<CategoryResponse> Items);
