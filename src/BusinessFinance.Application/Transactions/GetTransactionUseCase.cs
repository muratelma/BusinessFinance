using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Transactions;

public sealed class GetTransactionUseCase(
    ICurrentUser currentUser,
    ITransactionRepository repository)
{
    public async Task<ApplicationResult<TransactionDto>> ExecuteAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AuthenticationRequired);
        }

        var transaction = await repository.FindOwnedByIdAsync(
            transactionId,
            userId,
            track: false,
            cancellationToken);
        return transaction is null
            ? ApplicationResult<TransactionDto>.Failure(TransactionErrors.NotFound(transactionId))
            : ApplicationResult<TransactionDto>.Success(CreateTransactionUseCase.ToDto(transaction));
    }
}
