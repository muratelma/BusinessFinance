using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Counterparties;

/// <summary>Tahsilat ya da ödeme panelinin kısayol tutarları (yalnız kişinin hesabı).</summary>
public sealed record CounterpartyShortcutsDto(decimal Full, decimal Overdue);

/// <summary>
/// Kişi sayfası (S6, `02-TASARIM.md` §6.2). Özet kişinin hesabını <b>ve</b>
/// kişili taksitli kayıtlarının kalanını içerir (K14); kısayollar yalnız
/// hesabındır, çünkü <c>Tahsilat</c> ve <c>Ödeme</c> yalnız hesaba yazar (ADR
/// 0022 İ7). Hareketler birleşik akıştan kişi ve yön süzgeciyle okunur.
/// </summary>
public sealed record CounterpartyPageDto(
    Guid CounterpartyId,
    string Name,
    string? Note,
    bool IsActive,
    TransactionScope? Scope,
    decimal OpeningReceivable,
    decimal OpeningPayable,
    decimal OwedToYou,
    decimal OwedByYou,
    decimal OverdueReceivable,
    decimal OverduePayable,
    DateOnly? NextReceivableDueDate,
    decimal NextReceivableDueAmount,
    DateOnly? NextPayableDueDate,
    decimal NextPayableDueAmount,
    CounterpartyShortcutsDto Collect,
    CounterpartyShortcutsDto Pay,
    IReadOnlyList<DebtBookItemDto> Records);

public sealed class GetCounterpartyPageUseCase(
    ICurrentUser currentUser,
    ICounterpartyRepository repository,
    IDebtBookReader debtBookReader,
    TimeProvider timeProvider)
{
    /// <param name="asOfDate">Gecikmenin bakıldığı gün; boşsa bugün (Türkiye günü).</param>
    public async Task<ApplicationResult<CounterpartyPageDto>> ExecuteAsync(
        Guid counterpartyId,
        DateOnly? asOfDate,
        CancellationToken cancellationToken = default)
    {
        var day = asOfDate ?? LocalDay.Today(timeProvider.GetUtcNow());
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<CounterpartyPageDto>.Failure(CounterpartyErrors.AuthenticationRequired);
        }

        var counterparty = await repository.FindOwnedByIdAsync(counterpartyId, userId, cancellationToken);
        if (counterparty is null)
        {
            return ApplicationResult<CounterpartyPageDto>.Failure(CounterpartyErrors.NotFound(counterpartyId));
        }

        var ledger = await repository.FindBalanceAsync(counterpartyId, userId, day, cancellationToken)
            ?? throw new InvalidOperationException("An owned counterparty has no balance row.");
        var debts = (await debtBookReader.ListOpenAsync(userId, day, cancellationToken))
            .Where(debt => debt.CounterpartyId == counterpartyId)
            .ToArray();

        var records = debts
            .Select(debt => new DebtBookItemDto(
                "debt", debt.DebtId, debt.Direction,
                GetDebtBookUseCase.SectionOf(debt.Overdue, debt.NextDueDate, day),
                debt.Description ?? debt.CounterpartyName ?? string.Empty,
                debt.CounterpartyId, debt.CounterpartyName, debt.PlanKind,
                debt.Remaining, debt.Overdue, debt.NextDueDate, debt.NextDueAmount,
                debt.HasClosings, debt.IsCarriedOver, debt.StartDate,
                debt.Scope, 0m, debt.OldestOverdueDate, debt.InstallmentCount, debt.OpenInstallmentCount))
            .OrderBy(item => item.Section)
            .ThenBy(item => item.NextDueDate ?? item.OldestOverdueDate)
            .ThenByDescending(item => item.Remaining)
            .ToArray();

        var (nextReceivable, nextReceivableAmount) = Next(
            ledger.NextReceivableDueDate, ledger.NextReceivableDueAmount, debts, DebtDirection.Receivable);
        var (nextPayable, nextPayableAmount) = Next(
            ledger.NextPayableDueDate, ledger.NextPayableDueAmount, debts, DebtDirection.Payable);

        return ApplicationResult<CounterpartyPageDto>.Success(new CounterpartyPageDto(
            counterparty.Id,
            counterparty.Name,
            counterparty.Note,
            counterparty.IsActive,
            counterparty.Scope,
            counterparty.OpeningReceivable,
            counterparty.OpeningPayable,
            ledger.OwedToYou + Sum(debts, DebtDirection.Receivable, debt => debt.Remaining),
            ledger.OwedByYou + Sum(debts, DebtDirection.Payable, debt => debt.Remaining),
            ledger.OverdueReceivable + Sum(debts, DebtDirection.Receivable, debt => debt.Overdue),
            ledger.OverduePayable + Sum(debts, DebtDirection.Payable, debt => debt.Overdue),
            nextReceivable,
            nextReceivableAmount,
            nextPayable,
            nextPayableAmount,
            new CounterpartyShortcutsDto(ledger.OwedToYou, ledger.OverdueReceivable),
            new CounterpartyShortcutsDto(ledger.OwedByYou, ledger.OverduePayable),
            records));
    }

    private static decimal Sum(
        IEnumerable<DebtBookDebtRow> debts, DebtDirection direction, Func<DebtBookDebtRow, decimal> amount) =>
        debts.Where(debt => debt.Direction == direction).Sum(amount);

    /// <summary>Hesabın ve kayıtların en yakın vadesi; aynı gündekiler toplanır.</summary>
    private static (DateOnly? Date, decimal Amount) Next(
        DateOnly? ledgerDate, decimal ledgerAmount, IEnumerable<DebtBookDebtRow> debts, DebtDirection direction)
    {
        var candidates = debts
            .Where(debt => debt.Direction == direction && debt.NextDueDate is not null)
            .Select(debt => (Date: debt.NextDueDate!.Value, Amount: debt.NextDueAmount))
            .ToList();
        if (ledgerDate is DateOnly date)
        {
            candidates.Add((date, ledgerAmount));
        }

        if (candidates.Count == 0)
        {
            return (null, 0m);
        }

        var nearest = candidates.Min(candidate => candidate.Date);
        return (nearest, candidates.Where(candidate => candidate.Date == nearest).Sum(candidate => candidate.Amount));
    }
}
