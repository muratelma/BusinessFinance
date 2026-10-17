using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Debts;

/// <summary>Listede bir satırın bölümü (ADR 0021 T8, CB7).</summary>
public enum DebtBookSection
{
    Overdue = 1,
    Upcoming = 2,
    Undated = 3
}

/// <summary>Açık bir borcun listedeki satırı için okunan ham bilgi.</summary>
public sealed record DebtBookDebtRow(
    Guid DebtId,
    DebtDirection Direction,
    DebtPlanKind PlanKind,
    Guid? CounterpartyId,
    string? CounterpartyName,
    string? Description,
    DateOnly StartDate,
    decimal Remaining,
    decimal Overdue,
    DateOnly? NextDueDate,
    decimal NextDueAmount,
    bool HasClosings,
    bool IsCarriedOver);

/// <summary>Kapanmış borç: son kapanışının günüyle.</summary>
public sealed record DebtBookClosedRow(
    Guid DebtId,
    DebtDirection Direction,
    DebtPlanKind PlanKind,
    Guid? CounterpartyId,
    string? CounterpartyName,
    string? Description,
    decimal Amount,
    DateOnly ClosedOn);

public interface IDebtBookReader
{
    Task<IReadOnlyList<DebtBookDebtRow>> ListOpenAsync(
        Guid userId, DateOnly asOfDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<DebtBookClosedRow>> ListClosedAsync(
        Guid userId, CancellationToken cancellationToken);
}

/// <summary>
/// Listenin bir satırı: bir borç ya da bir kişinin cari hesabı. Başlık
/// açıklamadır, yoksa kişinin adı (CB24); cari satırında kişinin adı.
/// </summary>
public sealed record DebtBookItemDto(
    string Kind,
    Guid Id,
    DebtDirection Direction,
    DebtBookSection Section,
    string Title,
    Guid? CounterpartyId,
    string? CounterpartyName,
    DebtPlanKind? PlanKind,
    decimal Remaining,
    decimal Overdue,
    DateOnly? NextDueDate,
    decimal NextDueAmount,
    bool HasClosings,
    bool IsCarriedOver,
    DateOnly? StartDate);

/// <summary>Kişiler görünümü: kişi başına iki yönün toplamı (cari dahil).</summary>
public sealed record DebtBookPersonDto(
    Guid CounterpartyId,
    string Name,
    decimal OwedToYou,
    decimal OwedByYou,
    decimal LedgerOwedToYou,
    decimal LedgerOwedByYou,
    int OpenDebtCount);

public sealed record DebtBookDto(
    decimal PayableTotal,
    decimal ReceivableTotal,
    IReadOnlyList<DebtBookItemDto> Items,
    IReadOnlyList<DebtBookPersonDto> People,
    int ClosedPayableCount,
    int ClosedReceivableCount);

public sealed record DebtBookClosedItemDto(
    Guid DebtId,
    DebtDirection Direction,
    string Title,
    Guid? CounterpartyId,
    string? CounterpartyName,
    DebtPlanKind PlanKind,
    decimal Amount,
    DateOnly ClosedOn);

/// <summary>
/// "Borç ve alacaklar" listesi (ADR 0021 T8): her açık borç ve cari hesabı
/// açık her kişi bir satır; bölümü gecikme ve sıradaki vadeden; toplamlar ve
/// kişi başına özet sunucudan.
/// </summary>
/// <remarks>
/// Cari satırı listede bir kez durur ve tutarı ekranda yazılacak bakiyedir
/// (<see cref="CounterpartyBalanceSummary.OwedToYou"/>,
/// <see cref="CounterpartyBalanceSummary.OwedByYou"/>): fazla ödeme kırpılmaz,
/// öbür yönde görünür. Gecikmiş tutar ve sıradaki vade ödemelerin önce en eski
/// alımı düşürdüğü sırayla hesaplanır.
/// </remarks>
public sealed class GetDebtBookUseCase(
    ICurrentUser currentUser,
    IDebtBookReader reader,
    ICounterpartyRepository counterpartyRepository)
{
    public async Task<ApplicationResult<DebtBookDto>> ExecuteAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtBookDto>.Failure(DebtErrors.AuthenticationRequired);

        var debts = await reader.ListOpenAsync(userId, asOfDate, cancellationToken);
        var closed = await reader.ListClosedAsync(userId, cancellationToken);
        var ledgers = await counterpartyRepository.ListBalancesAsync(
            userId, CounterpartyBalanceFilter.Open, null, asOfDate, cancellationToken);

        var items = new List<DebtBookItemDto>();
        foreach (var debt in debts)
        {
            items.Add(new DebtBookItemDto(
                "debt", debt.DebtId, debt.Direction,
                SectionOf(debt.Overdue, debt.NextDueDate),
                debt.Description ?? debt.CounterpartyName ?? string.Empty,
                debt.CounterpartyId, debt.CounterpartyName, debt.PlanKind,
                debt.Remaining, debt.Overdue, debt.NextDueDate, debt.NextDueAmount,
                debt.HasClosings, debt.IsCarriedOver, debt.StartDate));
        }

        foreach (var ledger in ledgers)
        {
            AddLedger(items, ledger, DebtDirection.Receivable, ledger.OwedToYou,
                ledger.OverdueReceivable, ledger.NextReceivableDueDate, ledger.NextReceivableDueAmount);
            AddLedger(items, ledger, DebtDirection.Payable, ledger.OwedByYou,
                ledger.OverduePayable, ledger.NextPayableDueDate, ledger.NextPayableDueAmount);
        }

        var ordered = items
            .OrderBy(item => item.Section)
            .ThenBy(item => item.Section == DebtBookSection.Upcoming ? item.NextDueDate : null)
            .ThenByDescending(item => item.Remaining)
            .ThenBy(item => item.Title, StringComparer.CurrentCulture)
            .ToArray();

        var people = items
            .Where(item => item.CounterpartyId is not null)
            .GroupBy(item => item.CounterpartyId!.Value)
            .Select(group => new DebtBookPersonDto(
                group.Key,
                group.First().CounterpartyName ?? string.Empty,
                group.Where(item => item.Direction == DebtDirection.Receivable).Sum(item => item.Remaining),
                group.Where(item => item.Direction == DebtDirection.Payable).Sum(item => item.Remaining),
                group.Where(item => item.Kind == "ledger" && item.Direction == DebtDirection.Receivable).Sum(item => item.Remaining),
                group.Where(item => item.Kind == "ledger" && item.Direction == DebtDirection.Payable).Sum(item => item.Remaining),
                group.Count(item => item.Kind == "debt")))
            .OrderByDescending(person => Math.Max(person.OwedToYou, person.OwedByYou))
            .ThenBy(person => person.Name, StringComparer.CurrentCulture)
            .ToArray();

        return ApplicationResult<DebtBookDto>.Success(new DebtBookDto(
            items.Where(item => item.Direction == DebtDirection.Payable).Sum(item => item.Remaining),
            items.Where(item => item.Direction == DebtDirection.Receivable).Sum(item => item.Remaining),
            ordered,
            people,
            closed.Count(item => item.Direction == DebtDirection.Payable),
            closed.Count(item => item.Direction == DebtDirection.Receivable)));
    }

    private static void AddLedger(
        List<DebtBookItemDto> items,
        CounterpartyBalanceSummary ledger,
        DebtDirection direction,
        decimal amount,
        decimal overdue,
        DateOnly? nextDue,
        decimal nextAmount)
    {
        if (amount <= 0m)
        {
            return;
        }

        items.Add(new DebtBookItemDto(
            "ledger", ledger.CounterpartyId, direction, SectionOf(overdue, nextDue),
            ledger.Name, ledger.CounterpartyId, ledger.Name, null,
            amount, overdue, nextDue, nextAmount, false, false, null));
    }

    internal static DebtBookSection SectionOf(decimal overdue, DateOnly? nextDue) =>
        overdue > 0m
            ? DebtBookSection.Overdue
            : nextDue is not null
                ? DebtBookSection.Upcoming
                : DebtBookSection.Undated;
}

/// <summary>Kapanmış borçlar, en yeni kapanış üstte.</summary>
public sealed class ListClosedDebtsUseCase(ICurrentUser currentUser, IDebtBookReader reader)
{
    public async Task<ApplicationResult<IReadOnlyList<DebtBookClosedItemDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<IReadOnlyList<DebtBookClosedItemDto>>.Failure(DebtErrors.AuthenticationRequired);
        var closed = await reader.ListClosedAsync(userId, cancellationToken);
        return ApplicationResult<IReadOnlyList<DebtBookClosedItemDto>>.Success(closed
            .OrderByDescending(item => item.ClosedOn)
            .Select(item => new DebtBookClosedItemDto(
                item.DebtId, item.Direction, item.Description ?? item.CounterpartyName ?? string.Empty,
                item.CounterpartyId, item.CounterpartyName, item.PlanKind, item.Amount, item.ClosedOn))
            .ToArray());
    }
}
