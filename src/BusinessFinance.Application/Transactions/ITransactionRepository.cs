using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transactions;

public interface ITransactionRepository
{
    Task AddAsync(BudgetTransaction transaction, CancellationToken cancellationToken);
    Task<BudgetTransaction?> FindOwnedByIdAsync(
        Guid transactionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(
        BudgetTransaction transaction,
        Guid userId,
        CancellationToken cancellationToken);
    Task<TransactionListPage> ListAsync(
        Guid userId,
        TransactionListCriteria criteria,
        CancellationToken cancellationToken);
}
