using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record CreateTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    TransactionType Type,

    // Kullanıcının açık seçimi. Tek taraflı kategoride gerekmez ve onunla
    // çelişirse istek reddedilir; iki tarafa açık kategoride boşsa hesabın
    // etiketi ön değerdir (ADR 0020).
    TransactionScope? Scope,
    DateOnly TransactionDate,
    string? Description);

public sealed class CreateTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    IUserProfileRepository profileRepository)
{
    public async Task<ApplicationResult<TransactionDto>> ExecuteAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AuthenticationRequired);
        }

        var account = await accountRepository.FindOwnedByIdAsync(
            command.AccountId,
            userId,
            cancellationToken);
        if (account is null || !account.IsActive)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AccountUnavailable);
        }

        var category = await categoryRepository.FindOwnedByIdAsync(
            command.CategoryId,
            userId,
            cancellationToken);
        if (category is null || !category.IsActive)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.CategoryUnavailable);
        }

        var resolution = await TransactionScopeResolution.ResolveAsync(
            command.Scope,
            category.DefaultScope,
            account.DefaultScope,
            profileRepository,
            userId,
            cancellationToken);
        if (resolution.Scope is not TransactionScope scope)
        {
            return ApplicationResult<TransactionDto>.Failure(resolution.ToError(
                TransactionErrors.ScopeUnresolved, TransactionErrors.ScopeConflict));
        }

        BudgetTransaction transaction;
        try
        {
            transaction = new BudgetTransaction(
                Guid.NewGuid(),
                userId,
                account,
                category,
                new Money(command.Amount, command.Currency),
                command.Type,
                scope,
                command.TransactionDate,
                command.Description);
        }
        catch (ArgumentException exception)
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.Validation(exception.Message));
        }

        await transactionRepository.AddAsync(transaction, cancellationToken);
        return ApplicationResult<TransactionDto>.Success(ToDto(transaction));
    }

    internal static TransactionDto ToDto(BudgetTransaction transaction) => new(
        transaction.Id,
        transaction.AccountId,
        transaction.CategoryId,
        transaction.Amount.Amount,
        transaction.Amount.Currency,
        transaction.Type,
        transaction.Scope,
        transaction.TransactionDate,
        transaction.Description,
        transaction.IsCancelled,
        transaction.CancelledAtUtc);
}
