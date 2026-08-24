using BusinessFinance.Domain;

namespace BusinessFinance.Application.Obligations;

public sealed record CreateObligationCommand(
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    Guid CategoryId,
    TransactionScope? Scope,
    DateOnly IssueDate,
    DateOnly DueDate,
    Guid? CounterpartyId,
    string? Description);

public sealed record ObligationDto(
    Guid Id,
    Guid? CounterpartyId,
    Guid CategoryId,
    DebtDirection Direction,
    decimal Amount,
    CurrencyCode Currency,
    TransactionScope Scope,
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Description,
    ObligationStatus Status);

public interface IObligationRepository
{
    Task AddAsync(Obligation obligation, CancellationToken cancellationToken);
}
