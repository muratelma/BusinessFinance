using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Categories;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/categories")
            .WithTags("Categories")
            .RequireAuthorization();

        group.MapPost(string.Empty, CreateAsync)
            .WithName("CreateCategory")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet(string.Empty, ListAsync)
            .WithName("ListCategories")
            .Produces<CategoryListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPut("/{categoryId:guid}", UpdateAsync)
            .WithName("UpdateCategory")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateCategoryRequest request,
        CreateCategoryUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseType(request.Type, out var type))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Category type must be income or expense.",
                "categories.invalid_type");
        }

        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var createScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "categories.invalid_default_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreateCategoryCommand(
                request.Name, type, createScope, request.DefaultIsTaxDeductible),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/categories/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListCategoriesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        string? type = null,
        bool? isActive = null)
    {
        CategoryType? parsedType = null;
        if (type is not null)
        {
            if (!TryParseType(type, out var categoryType))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Category type must be income or expense.",
                    "categories.invalid_type");
            }
            parsedType = categoryType;
        }

        var result = await useCase.ExecuteAsync(
            new ListCategoriesQuery(parsedType, isActive),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        return Results.Ok(new CategoryListResponse(result.Value.Select(ToResponse).ToArray()));
    }

    private static async Task<IResult> UpdateAsync(
        Guid categoryId,
        UpdateCategoryRequest request,
        UpdateCategoryUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var updateScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "categories.invalid_default_scope");
        }

        var result = await useCase.ExecuteAsync(
            new UpdateCategoryCommand(
                categoryId,
                request.Name,
                request.IsActive,
                updateScope,
                request.DefaultIsTaxDeductible),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static bool TryParseType(string? value, out CategoryType type) =>
        Enum.TryParse(value, true, out type) && type is CategoryType.Income or CategoryType.Expense;

    private static CategoryResponse ToResponse(CategoryListItemDto item) => new(
        item.Id,
        item.Name,
        FinanceContract.CategoryTypeValue(item.Type),
        item.IsActive,
        FinanceContract.OptionalScopeValue(item.DefaultScope),
        item.DefaultIsTaxDeductible);
}
