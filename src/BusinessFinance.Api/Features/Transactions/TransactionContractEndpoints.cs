using System.Globalization;
using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Transactions;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Transactions;

public static class TransactionContractEndpoints
{
    private const int MaximumPageSize = 100;

    public static IEndpointRouteBuilder MapTransactionContractEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/transactions", ListContractAsync)
            .WithTags("Transactions")
            .WithName("ListTransactions")
            .WithSummary("Lists the authenticated user's transactions")
            .WithDescription("Lists owner-scoped transactions with deterministic date-desc ordering.")
            .RequireAuthorization()
            .Produces<TransactionListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapGet("/api/v1/transactions/{transactionId:guid}", GetAsync)
            .WithTags("Transactions")
            .WithName("GetTransaction")
            .RequireAuthorization()
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        endpoints.MapPost("/api/v1/transactions", CreateAsync)
            .WithTags("Transactions")
            .WithName("CreateTransaction")
            .RequireAuthorization()
            .Produces<TransactionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapDelete("/api/v1/transactions/{transactionId:guid}", CancelAsync)
            .WithTags("Transactions")
            .WithName("CancelTransaction")
            .RequireAuthorization()
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateTransactionRequest request,
        CreateTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseAmount(request.Amount, out var amount) || amount <= 0)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Amount must be greater than zero and have at most four decimal places.",
                "transactions.invalid_amount");
        }
        if (!string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only TRY currency is currently supported.",
                "transactions.invalid_currency");
        }
        if (!TryParseType(request.Type, out var type))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transaction type must be income or expense.",
                "transactions.invalid_type");
        }
        if (!FinanceContract.TryParseScope(request.Scope, out var scope))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transaction scope must be business or personal.",
                "transactions.invalid_scope");
        }
        if (!FinanceContract.TryParseDate(request.TransactionDate, out var date))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transaction date must use the yyyy-MM-dd format.",
                "transactions.invalid_date");
        }

        var result = await useCase.ExecuteAsync(
            new CreateTransactionCommand(
                request.AccountId,
                request.CategoryId,
                amount,
                CurrencyCode.TRY,
                type,
                scope,
                date,
                request.Description),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/transactions/{response.Id}", response);
    }

    private static async Task<IResult> CancelAsync(
        Guid transactionId,
        CancelTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CancelTransactionCommand(transactionId),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    internal static TransactionResponse ToResponse(TransactionDto transaction) => new(
        transaction.Id,
        transaction.AccountId,
        transaction.CategoryId,
        FinanceContract.Money(transaction.Amount),
        transaction.Currency.ToString(),
        FinanceContract.TransactionTypeValue(transaction.Type),
        FinanceContract.ScopeValue(transaction.Scope),
        FinanceContract.Date(transaction.TransactionDate),
        transaction.Description,
        transaction.IsCancelled,
        transaction.CancelledAtUtc);

    private static bool TryParseType(string? value, out TransactionType type) =>
        Enum.TryParse(value, true, out type) && type is TransactionType.Income or TransactionType.Expense;

    private static async Task<IResult> ListContractAsync(
        HttpContext httpContext,
        ListTransactionsUseCase useCase,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 20,
        string? dateFrom = null,
        string? dateTo = null,
        string? accountId = null,
        string? categoryId = null,
        string? type = null,
        string sort = "date-desc")
    {
        if (pageNumber < 1 || pageSize is < 1 or > MaximumPageSize)
        {
            return ApiProblemResults.Validation(
                httpContext,
                $"Page number must be at least 1 and page size must be between 1 and " +
                $"{MaximumPageSize}.",
                "transactions.invalid_pagination");
        }

        if (!TryParseDate(dateFrom, out var parsedDateFrom) ||
            !TryParseDate(dateTo, out var parsedDateTo))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Dates must use the yyyy-MM-dd format.",
                "transactions.invalid_date");
        }

        if (parsedDateFrom is not null && parsedDateTo is not null &&
            parsedDateFrom > parsedDateTo)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Date-from cannot be after date-to.",
                "transactions.invalid_date_range");
        }

        if (!TryParseOptionalGuid(accountId, out var parsedAccountId) ||
            !TryParseOptionalGuid(categoryId, out var parsedCategoryId))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Account and category filters must be non-empty GUID values.",
                "transactions.invalid_identifier");
        }

        if (type is not null &&
            !string.Equals(type, "income", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(type, "expense", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Transaction type must be income or expense.",
                "transactions.invalid_type");
        }

        if (!string.Equals(sort, "date-desc", StringComparison.OrdinalIgnoreCase))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Only date-desc sorting is currently supported.",
                "transactions.invalid_sort");
        }

        TransactionType? parsedType = type?.ToLowerInvariant() switch
        {
            "income" => TransactionType.Income,
            "expense" => TransactionType.Expense,
            _ => null
        };
        var result = await useCase.ExecuteAsync(
            new ListTransactionsQuery(
                pageNumber,
                pageSize,
                parsedDateFrom,
                parsedDateTo,
                parsedAccountId,
                parsedCategoryId,
                parsedType),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var totalPages = result.Value.TotalCount == 0
            ? 0
            : (int)Math.Ceiling((double)result.Value.TotalCount / result.Value.PageSize);
        return Results.Ok(new TransactionListResponse(
            result.Value.Items.Select(ToResponse).ToArray(),
            new PaginationMetadata(
                result.Value.PageNumber,
                result.Value.PageSize,
                result.Value.TotalCount,
                totalPages,
                result.Value.PageNumber > 1,
                result.Value.PageNumber < totalPages)));
    }

    private static async Task<IResult> GetAsync(
        Guid transactionId,
        GetTransactionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(transactionId, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static bool TryParseDate(string? value, out DateOnly? date)
    {
        date = null;

        if (value is null)
        {
            return true;
        }

        if (!DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return false;
        }

        date = parsed;
        return true;
    }

    private static bool TryParseOptionalGuid(string? value, out Guid? id)
    {
        id = null;
        if (value is null)
        {
            return true;
        }
        if (!Guid.TryParse(value, out var parsed) || parsed == Guid.Empty)
        {
            return false;
        }
        id = parsed;
        return true;
    }
}
