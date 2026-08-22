using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Accounts.CreateAccount;
using BusinessFinance.Application.Accounts.DeleteAccount;
using BusinessFinance.Application.Accounts.ListAccounts;
using BusinessFinance.Application.Accounts.GetAccount;
using BusinessFinance.Application.Accounts.UpdateAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Accounts;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/v1/accounts")
            .WithTags("Accounts")
            .RequireAuthorization();

        group.MapPost(string.Empty, CreateAsync)
            .WithName("CreateAccount")
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet(string.Empty, ListAsync)
            .WithName("ListAccounts")
            .Produces<AccountListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/{accountId:guid}", GetAsync)
            .WithName("GetAccount")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{accountId:guid}", UpdateAsync)
            .WithName("UpdateAccount")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapDelete("/{accountId:guid}", DeleteAsync)
            .WithName("DeleteUnusedAccount")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateAccountRequest request,
        CreateAccountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParseAccountType(request.Type, out var accountType))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Account type must be cash or bank.",
                "accounts.invalid_type");
        }

        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only TRY currency is currently supported.",
                "accounts.invalid_currency");
        }

        if (!FinanceContract.TryParseAmount(request.OpeningBalance, out var openingBalance) ||
            openingBalance < 0)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Opening balance must be a non-negative value with at most four decimal places.",
                "accounts.invalid_opening_balance");
        }

        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var defaultScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "accounts.invalid_default_scope");
        }

        var result = await useCase.ExecuteAsync(
            new CreateAccountCommand(
                request.Name,
                accountType,
                CurrencyCode.TRY,
                openingBalance,
                defaultScope),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = new AccountResponse(
            result.Value.Id,
            result.Value.Name,
            ToContractValue(result.Value.Type),
            result.Value.Currency.ToString(),
            true,
            FinanceContract.Money(result.Value.OpeningBalance),
            FinanceContract.Money(result.Value.OpeningBalance),
            FinanceContract.OptionalScopeValue(result.Value.DefaultScope));

        return Results.Created($"/api/v1/accounts/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ListAccountsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 20,
        bool? isActive = null,
        string? type = null)
    {
        AccountType? accountType = null;

        if (type is not null)
        {
            if (!TryParseAccountType(type, out var parsedType))
            {
                return ApiProblemResults.Validation(
                    httpContext,
                    "Account type must be cash or bank.",
                    "accounts.invalid_type");
            }

            accountType = parsedType;
        }

        if (pageNumber < 1 || pageSize is < 1 or > ListAccountsQuery.MaximumPageSize)
        {
            return ApiProblemResults.Validation(
                httpContext,
                $"Page number must be at least 1 and page size must be between 1 and " +
                $"{ListAccountsQuery.MaximumPageSize}.",
                "accounts.invalid_pagination");
        }

        var result = await useCase.ExecuteAsync(
            new ListAccountsQuery(pageNumber, pageSize, isActive, accountType),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var items = result.Value.Items
            .Select(item => new AccountResponse(
                item.Id,
                item.Name,
                ToContractValue(item.Type),
                item.Currency.ToString(),
                item.IsActive,
                FinanceContract.Money(item.OpeningBalance),
                FinanceContract.Money(item.Balance),
                FinanceContract.OptionalScopeValue(item.DefaultScope)))
            .ToArray();
        var totalPages = result.Value.TotalCount == 0
            ? 0
            : (int)Math.Ceiling((double)result.Value.TotalCount / result.Value.PageSize);
        var pagination = new PaginationMetadata(
            result.Value.PageNumber,
            result.Value.PageSize,
            result.Value.TotalCount,
            totalPages,
            result.Value.PageNumber > 1,
            result.Value.PageNumber < totalPages);

        return Results.Ok(new AccountListResponse(items, pagination));
    }

    private static async Task<IResult> GetAsync(
        Guid accountId,
        GetAccountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetAccountQuery(accountId), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        return Results.Ok(new AccountResponse(
            result.Value.Id,
            result.Value.Name,
            FinanceContract.AccountTypeValue(result.Value.Type),
            result.Value.Currency.ToString(),
            result.Value.IsActive,
            FinanceContract.Money(result.Value.OpeningBalance),
            FinanceContract.Money(result.Value.Balance),
            FinanceContract.OptionalScopeValue(result.Value.DefaultScope)));
    }

    private static async Task<IResult> UpdateAsync(
        Guid accountId,
        UpdateAccountRequest request,
        UpdateAccountUseCase useCase,
        GetAccountUseCase getUseCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseOptionalScope(request.DefaultScope, out var defaultScope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Default scope must be business, personal or empty.",
                "accounts.invalid_default_scope");
        }

        var update = await useCase.ExecuteAsync(
            new UpdateAccountCommand(accountId, request.Name, request.IsActive, defaultScope),
            cancellationToken);
        if (!update.IsSuccess)
        {
            return update.Error.ToProblemResult(httpContext);
        }

        return await GetAsync(accountId, getUseCase, httpContext, cancellationToken);
    }

    private static async Task<IResult> DeleteAsync(
        Guid accountId,
        DeleteAccountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new DeleteAccountCommand(accountId),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        return Results.NoContent();
    }

    private static bool TryParseAccountType(
        string? value,
        out AccountType accountType)
    {
        return Enum.TryParse(value, ignoreCase: true, out accountType) &&
               accountType is AccountType.Cash or AccountType.Bank;
    }

    private static string ToContractValue(AccountType accountType)
    {
        return FinanceContract.AccountTypeValue(accountType);
    }
}
