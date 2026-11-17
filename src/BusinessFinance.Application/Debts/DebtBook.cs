using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Debts;

/// <summary>
/// Listede bir satırın bölümü: <c>Geciken</c> · <c>Yaklaşan</c> (30 gün) ·
/// <c>Diğer</c> (vadesi daha ileride ya da vadesiz).
/// </summary>
public enum DebtBookSection
{
    Overdue = 1,
    Upcoming = 2,
    Other = 3
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
    bool IsCarriedOver,
    TransactionScope? Scope = null,
    DateOnly? OldestOverdueDate = null,
    int? InstallmentCount = null,
    int OpenInstallmentCount = 0);

/// <summary>Kapanmış borç: son kapanışının günüyle.</summary>
public sealed record DebtBookClosedRow(
    Guid DebtId,
    DebtDirection Direction,
    DebtPlanKind PlanKind,
    Guid? CounterpartyId,
    string? CounterpartyName,
    string? Description,
    decimal Amount,
    DateOnly ClosedOn,
    TransactionScope? Scope = null);

public interface IDebtBookReader
{
    Task<IReadOnlyList<DebtBookDebtRow>> ListOpenAsync(
        Guid userId, DateOnly asOfDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<DebtBookClosedRow>> ListClosedAsync(
        Guid userId, CancellationToken cancellationToken);
}

/// <summary>
/// Listenin bir satırı: bir kişinin hesabı (<c>ledger</c>; kimlik kişinindir)
/// ya da taksitli veya kişisiz bir kayıt (<c>debt</c>). Başlık açıklamadır, yoksa
/// kişinin adı; kişi satırında kişinin adı.
/// </summary>
/// <param name="Scope">
/// Kişi satırında kişinin türü (bilinmiyorsa boş), kayıt satırında kaydın
/// kapsamı. <c>Hepsi</c> görünümünde satırın etiketi olur.
/// </param>
/// <param name="AdvanceAmount">
/// Kişi satırında tutarın karşı yöndeki fazla ödemeden gelen kısmı: borç
/// satırında alınan avans, alacak satırında verilen avans. Satırın tamamı
/// avanssa ekran "Alınan avans" / "Verilen avans" yazar.
/// </param>
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
    DateOnly? StartDate,
    TransactionScope? Scope = null,
    decimal AdvanceAmount = 0m,
    DateOnly? OldestOverdueDate = null,
    int? InstallmentCount = null,
    int OpenInstallmentCount = 0);

/// <summary>
/// Kişiler sekmesi: bütün kişiler, bakiyesi olmayan ve pasif olan dahil; iki
/// yönün toplamı kişinin hesabı ve kişili kayıtlarıdır.
/// </summary>
public sealed record DebtBookPersonDto(
    Guid CounterpartyId,
    string Name,
    decimal OwedToYou,
    decimal OwedByYou,
    decimal LedgerOwedToYou,
    decimal LedgerOwedByYou,
    int OpenDebtCount,
    bool IsActive = true,
    TransactionScope? Scope = null);

public sealed record DebtBookDto(
    decimal PayableTotal,
    decimal ReceivableTotal,
    IReadOnlyList<DebtBookItemDto> Items,
    IReadOnlyList<DebtBookPersonDto> People,
    int ClosedPayableCount,
    int ClosedReceivableCount,
    decimal PayableOverdue = 0m,
    decimal ReceivableOverdue = 0m);

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
/// <c>Borç ve alacak</c> listesi (ADR 0022 İ6; `02-TASARIM.md` §6.1): her kişinin
/// hesabı bir satır, her taksitli ya da kişisiz kayıt bir satır. Toplamlar,
/// bölüm ve sıra sunucudandır; satırların toplamı özetteki sayıdır.
/// </summary>
/// <remarks>
/// <para>
/// Kişi satırının tutarı ekranda yazılacak bakiyedir
/// (<see cref="CounterpartyBalanceSummary.OwedToYou"/>,
/// <see cref="CounterpartyBalanceSummary.OwedByYou"/>): fazla ödeme kırpılmaz,
/// öbür yönde görünür. Gecikmiş tutar ve vadeler ödemelerin önce en eski kalemi
/// kapattığı sırayla hesaplanır.
/// </para>
/// <para>
/// Kapsam (K4): kişi satırı kişinin türüyle, kayıt satırı kendi kapsamıyla
/// süzülür. Türü bilinmeyen eski kişi <c>Hepsi</c>de durur; <c>İşletme</c> ya
/// da <c>Şahsi</c>de o kapsamda alım-satımı varsa görünür. Toplamlar seçili
/// kapsamındır.
/// </para>
/// </remarks>
public sealed class GetDebtBookUseCase(
    ICurrentUser currentUser,
    IDebtBookReader reader,
    ICounterpartyRepository counterpartyRepository)
{
    /// <summary>Yaklaşan bölümünün ufku (gün).</summary>
    public const int UpcomingDays = 30;

    public async Task<ApplicationResult<DebtBookDto>> ExecuteAsync(
        DateOnly asOfDate,
        CancellationToken cancellationToken = default,
        TransactionScope? scope = null)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
            return ApplicationResult<DebtBookDto>.Failure(DebtErrors.AuthenticationRequired);

        var debts = await reader.ListOpenAsync(userId, asOfDate, cancellationToken);
        var closed = await reader.ListClosedAsync(userId, cancellationToken);
        var people = await counterpartyRepository.ListBalancesAsync(
            userId, CounterpartyBalanceFilter.All, null, asOfDate, cancellationToken);
        var recordScopes = scope is null
            ? new Dictionary<Guid, IReadOnlySet<TransactionScope>>()
            : await counterpartyRepository.ListRecordScopesAsync(userId, cancellationToken);

        bool PersonInView(CounterpartyBalanceSummary person) =>
            scope is not TransactionScope wanted ||
            (person.Scope is TransactionScope kind
                ? kind == wanted
                : recordScopes.TryGetValue(person.CounterpartyId, out var scopes) && scopes.Contains(wanted));

        var visiblePeople = people.Where(PersonInView).ToArray();
        var items = new List<DebtBookItemDto>();
        foreach (var debt in debts.Where(debt => scope is null || debt.Scope == scope))
        {
            items.Add(new DebtBookItemDto(
                "debt", debt.DebtId, debt.Direction,
                SectionOf(debt.Overdue, debt.NextDueDate, asOfDate),
                debt.Description ?? debt.CounterpartyName ?? string.Empty,
                debt.CounterpartyId, debt.CounterpartyName, debt.PlanKind,
                debt.Remaining, debt.Overdue, debt.NextDueDate, debt.NextDueAmount,
                debt.HasClosings, debt.IsCarriedOver, debt.StartDate,
                debt.Scope, 0m, debt.OldestOverdueDate, debt.InstallmentCount, debt.OpenInstallmentCount));
        }

        foreach (var person in visiblePeople)
        {
            AddLedger(items, person, DebtDirection.Receivable, person.OwedToYou,
                Math.Max(-person.Payable, 0m), person.OverdueReceivable, person.NextReceivableDueDate,
                person.NextReceivableDueAmount, person.OldestOverdueReceivableDate, asOfDate);
            AddLedger(items, person, DebtDirection.Payable, person.OwedByYou,
                Math.Max(-person.Receivable, 0m), person.OverduePayable, person.NextPayableDueDate,
                person.NextPayableDueAmount, person.OldestOverduePayableDate, asOfDate);
        }

        // Bölüm içinde sıra sabit (K11): gecikenlerde en eski vade, yaklaşanlarda
        // en yakın vade, diğerlerinde büyük tutar üstte.
        var ordered = items
            .OrderBy(item => item.Section)
            .ThenBy(item => item.Section switch
            {
                DebtBookSection.Overdue => item.OldestOverdueDate,
                DebtBookSection.Upcoming => item.NextDueDate,
                _ => null
            })
            .ThenByDescending(item => item.Remaining)
            .ThenBy(item => item.Title, StringComparer.CurrentCulture)
            .ThenBy(item => item.Id)
            .ToArray();

        var personItems = items
            .Where(item => item.CounterpartyId is not null)
            .ToLookup(item => item.CounterpartyId!.Value);
        var peopleRows = visiblePeople
            .Select(person =>
            {
                var rows = personItems[person.CounterpartyId];
                return new DebtBookPersonDto(
                    person.CounterpartyId,
                    person.Name,
                    rows.Where(item => item.Direction == DebtDirection.Receivable).Sum(item => item.Remaining),
                    rows.Where(item => item.Direction == DebtDirection.Payable).Sum(item => item.Remaining),
                    rows.Where(item => item.Kind == "ledger" && item.Direction == DebtDirection.Receivable)
                        .Sum(item => item.Remaining),
                    rows.Where(item => item.Kind == "ledger" && item.Direction == DebtDirection.Payable)
                        .Sum(item => item.Remaining),
                    rows.Count(item => item.Kind == "debt"),
                    person.IsActive,
                    person.Scope);
            })
            .OrderBy(person => person.Name, StringComparer.CurrentCulture)
            .ThenBy(person => person.CounterpartyId)
            .ToArray();

        var closedInView = closed.Where(item => scope is null || item.Scope == scope).ToArray();
        return ApplicationResult<DebtBookDto>.Success(new DebtBookDto(
            items.Where(item => item.Direction == DebtDirection.Payable).Sum(item => item.Remaining),
            items.Where(item => item.Direction == DebtDirection.Receivable).Sum(item => item.Remaining),
            ordered,
            peopleRows,
            closedInView.Count(item => item.Direction == DebtDirection.Payable),
            closedInView.Count(item => item.Direction == DebtDirection.Receivable),
            items.Where(item => item.Direction == DebtDirection.Payable).Sum(item => item.Overdue),
            items.Where(item => item.Direction == DebtDirection.Receivable).Sum(item => item.Overdue)));
    }

    private static void AddLedger(
        List<DebtBookItemDto> items,
        CounterpartyBalanceSummary person,
        DebtDirection direction,
        decimal amount,
        decimal advance,
        decimal overdue,
        DateOnly? nextDue,
        decimal nextAmount,
        DateOnly? oldestOverdue,
        DateOnly asOfDate)
    {
        if (amount <= 0m)
        {
            return;
        }

        items.Add(new DebtBookItemDto(
            "ledger", person.CounterpartyId, direction, SectionOf(overdue, nextDue, asOfDate),
            person.Name, person.CounterpartyId, person.Name, null,
            amount, overdue, nextDue, nextAmount, false, false, null,
            person.Scope, Math.Min(advance, amount), oldestOverdue));
    }

    internal static DebtBookSection SectionOf(decimal overdue, DateOnly? nextDue, DateOnly asOfDate) =>
        overdue > 0m
            ? DebtBookSection.Overdue
            : nextDue is DateOnly due && due <= asOfDate.AddDays(UpcomingDays)
                ? DebtBookSection.Upcoming
                : DebtBookSection.Other;
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
