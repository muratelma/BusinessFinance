using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Application.Transactions;

public sealed record CancelTransactionCommand(Guid TransactionId);

public sealed class CancelTransactionUseCase(
    ICurrentUser currentUser,
    ITransactionRepository repository,
    IActivityOriginReader originReader,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<TransactionDto>> ExecuteAsync(
        CancelTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<TransactionDto>.Failure(TransactionErrors.AuthenticationRequired);
        }

        var transaction = await repository.FindOwnedByIdAsync(
            command.TransactionId,
            userId,
            track: true,
            cancellationToken);
        if (transaction is null)
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.NotFound(command.TransactionId));
        }

        // A transaction produced by a recurring occurrence cannot be cancelled: the
        // occurrence holds exactly one result id and has no way back, so cancelling
        // here would leave it realized and pointing at a cancelled row. Enforced on the
        // server, not only by hiding the button, so a direct API call cannot bypass it.
        // Asked as "may this kind and origin ever be cancelled", so an already-cancelled
        // manual transaction keeps its idempotent no-op behaviour.
        var origin = await originReader.GetTransactionOriginAsync(
            userId, transaction.Id, cancellationToken);
        if (!FinancialActivityCapabilities.CanCancel(
                FinancialActivityKind.AccountTransaction,
                origin,
                FinancialActivityStatus.Realized))
        {
            return ApplicationResult<TransactionDto>.Failure(
                TransactionErrors.CancelOriginLocked);
        }

        transaction.Cancel(timeProvider.GetUtcNow());
        await repository.UpdateOwnedAsync(transaction, userId, cancellationToken);
        return ApplicationResult<TransactionDto>.Success(CreateTransactionUseCase.ToDto(transaction));
    }
}
