using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public sealed record CategoryListItemDto(
    Guid Id,
    string Name,
    CategoryType Type,
    bool IsActive,
    TransactionScope? DefaultScope,
    bool? DefaultIsTaxDeductible = null);
