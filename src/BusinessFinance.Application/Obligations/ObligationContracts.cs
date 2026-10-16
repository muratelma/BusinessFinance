using BusinessFinance.Application.Pos;
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
    ObligationStatus Status,
    string? CounterpartyName = null,
    string? CategoryName = null,
    bool IsOverdue = false,
    Guid? SettlementId = null,
    Guid? SettlementAccountId = null,
    DateOnly? SettlementDate = null);

/// <remarks>
/// <see cref="Card"/> doluysa alacak kartla (POS) tahsil edilmiştir (ADR 0019
/// T5): alacak bugün kapanır, para hesaba yatışla geçer. Kartla yalnız alacak
/// kapanır; ödenecek fatura POS'tan geçmez.
/// </remarks>
public sealed record SettleObligationCommand(
    Guid ObligationId,
    Guid? AccountId,
    DateOnly SettlementDate,
    CardCollectionInput? Card = null);

public interface IObligationRepository
{
    Task AddAsync(Debt obligation, CancellationToken cancellationToken);
    Task<IReadOnlyList<ObligationDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        CancellationToken cancellationToken);
    Task<Debt?> FindOwnedByIdAsync(
        Guid obligationId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);
    /// <summary>
    /// Kapanışı ve kartla tahsilse POS kaydını tek <c>SaveChanges</c> ile yazar.
    /// Kapanış zaten varsa <paramref name="newPayment"/> boştur.
    /// </summary>
    Task SaveSettlementAsync(
        DebtPayment? newPayment,
        PosSettlement? cardSettlement,
        CancellationToken cancellationToken);

    /// <summary>Kartla kapatılmış alacağın yoldaki POS kaydı, izlenen hâlde.</summary>
    Task<PosSettlement?> FindCardSettlementAsync(
        Guid settlementId,
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// İptali (yükümlülük, kapanışı ve varsa POS kaydı) tek <c>SaveChanges</c>
    /// ile yazar. POS kaydı bu sırada bir yatışa bağlandıysa <c>false</c> döner
    /// ve hiçbir şey yazılmaz.
    /// </summary>
    Task<bool> TrySaveCancellationAsync(CancellationToken cancellationToken);
}

public sealed class ObligationConcurrencyException : Exception
{
    public ObligationConcurrencyException()
        : base("Obligation settlement was changed by another request.") { }
}
