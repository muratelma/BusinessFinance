using BusinessFinance.Application.Abstractions.Queries;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Transfers;

public sealed record TransferDto(
    Guid Id,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly TransferDate,
    string? Description,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc);

public sealed record CreateTransferCommand(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    CurrencyCode Currency,
    DateOnly TransferDate,
    string? Description);

public sealed record CancelTransferCommand(Guid TransferId);

public interface ITransferRepository
{
    Task AddAsync(Transfer transfer, CancellationToken cancellationToken);
    Task<Transfer?> FindOwnedByIdAsync(
        Guid transferId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task<BoundedList<Transfer>> ListAsync(
        Guid userId,
        HistoryWindow window,
        CancellationToken cancellationToken);
    Task UpdateOwnedAsync(
        Transfer transfer,
        Guid userId,
        CancellationToken cancellationToken);
}
