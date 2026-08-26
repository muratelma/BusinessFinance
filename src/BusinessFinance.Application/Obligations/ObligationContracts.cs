using BusinessFinance.Application.Taxes;
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
    string? Description,

    // Belgedeki KDV; yoksa boştur (ADR 0016). Sunucu hiçbir vergi tutarını
    // hesaplamaz — ne geldiyse o taşınır.
    VatDto? Vat = null);

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
    ObligationStatus Status,
    string? CounterpartyName = null,
    string? CategoryName = null,
    bool IsOverdue = false,
    Guid? SettlementId = null,
    Guid? SettlementAccountId = null,
    DateOnly? SettlementDate = null,
    VatDto? Vat = null);

public sealed record SettleObligationCommand(
    Guid ObligationId,
    Guid AccountId,
    DateOnly SettlementDate);

public interface IObligationRepository
{
    Task AddAsync(Obligation obligation, CancellationToken cancellationToken);
    Task<IReadOnlyList<ObligationDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken);
    Task<Obligation?> FindOwnedByIdAsync(
        Guid obligationId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    Task SaveSettlementAsync(CancellationToken cancellationToken);
}

public sealed class ObligationConcurrencyException : Exception
{
    public ObligationConcurrencyException()
        : base("Obligation settlement was changed by another request.") { }
}
