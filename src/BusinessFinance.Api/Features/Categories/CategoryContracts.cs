namespace BusinessFinance.Api.Features.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    string Type,
    string? DefaultScope = null,

    // Bu kategoriye yazılan giderlerin indirilebilirlik varsayılanı
    // (ADR 0016); yalnız gider kategorisinde anlamlıdır.
    bool? DefaultIsTaxDeductible = null);

/// <summary>
/// Kategorinin tam güncel hâli; <see cref="DefaultScope"/> boş gönderilirse
/// varsayılan kapsam kaldırılır.
/// </summary>
public sealed record UpdateCategoryRequest(
    string Name,
    bool IsActive,
    string? DefaultScope = null,
    bool? DefaultIsTaxDeductible = null);
public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Type,
    bool IsActive,
    string? DefaultScope,
    bool? DefaultIsTaxDeductible);
public sealed record CategoryListResponse(IReadOnlyList<CategoryResponse> Items);
