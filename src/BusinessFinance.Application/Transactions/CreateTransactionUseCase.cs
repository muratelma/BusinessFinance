using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public sealed record CreateTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    CurrencyCode Currency,
    TransactionType Type,
    TransactionScope Scope,
    DateOnly TransactionDate,
    string? Description);

public sealed class CreateTransactionUseCase(
    ICurrentUser currentUser,
    IAccountRepository accountRepository,
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository)
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
                command.Scope,
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
