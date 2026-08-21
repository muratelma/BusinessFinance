namespace BusinessFinance.Api.Features.Categories;

public sealed record CreateCategoryRequest(string Name, string Type);
public sealed record UpdateCategoryRequest(string Name, bool IsActive);
public sealed record CategoryResponse(Guid Id, string Name, string Type, bool IsActive);
public sealed record CategoryListResponse(IReadOnlyList<CategoryResponse> Items);
