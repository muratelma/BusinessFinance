using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.FinancialActivities;

/// <summary>
/// The single projection of everything that has not happened yet: recurring dates,
/// card installments, card statements, both directions of debt and one-time obligations.
/// </summary>
/// <remarks>
/// Unlike the realized feed this is assembled in memory on purpose. A statement is a
/// computed projection rather than a table, and the result set is bounded from above by
/// the 7, 30 or 90 day horizon, so there is no page whose cost could grow with history.
/// </remarks>
internal sealed class EfPlannedActivityRepository(BusinessFinanceDbContext dbContext)
    : IPlannedActivityRepository
{
    /// <summary>
    /// Henüz gerçekleşmemiş hareketler; <paramref name="scope"/> verilirse yalnız
    /// o kapsam.
    /// </summary>
    /// <remarks>
    /// Kapsam filtresi kapsamsız satırları da eler ve elemesi gerekir: kart
    /// ekstresi bir ödeme yükümlülüğüdür ama kapsam taşımaz, çünkü kart ödemesi
    /// gelir/gider raporuna sıfır etki eder (ADR 0003). Onu her iki kapsamda da
    /// göstermek, kullanıcı iki tarafı toplayınca aynı borcu iki kez saydırırdı.
    /// Filtre her kaynağın kendi sorgusuna iniyor; bellekte eleme yok.
    /// </remarks>
    public async Task<IReadOnlyList<PlannedActivityDto>> ListAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts.AsNoTracking()
            .Where(account => account.UserId == userId)
            .ToDictionaryAsync(account => account.Id, cancellationToken);
        var categories = await dbContext.Categories.AsNoTracking()
            .Where(category => category.UserId == userId)
            .ToDictionaryAsync(category => category.Id, cancellationToken);
        var cards = await dbContext.CreditCards.AsNoTracking()
            .Where(card => card.UserId == userId)
            .ToDictionaryAsync(card => card.Id, cancellationToken);

        var availableLimits = await CalculateAvailableLimitsAsync(userId, cards, cancellationToken);

        var items = new List<PlannedActivityDto>();
        items.AddRange(await ListRecurringAsync(
            userId, asOfDate, horizonDate, accounts, categories, cards, availableLimits,
            scope, cancellationToken));
        items.AddRange(await ListInstallmentsAsync(
            userId, asOfDate, horizonDate, cards, availableLimits, scope, cancellationToken));

        // Ekstre kapsam taşımaz; kapsam anahtarı bir tarafa çevriliyse listeden düşer.
        if (scope is null)
        {
            items.AddRange(await ListStatementsAsync(
                userId, asOfDate, horizonDate, cards, cancellationToken));
        }

        items.AddRange(await ListDebtAsync(userId, asOfDate, horizonDate, scope, cancellationToken));
        items.AddRange(await ListObligationsAsync(
            userId, asOfDate, horizonDate, scope, cancellationToken));
        return items;
    }

    private sealed record CardMovement(Guid CreditCardId, DateOnly Date, decimal Amount);

    /// <summary>
    /// Totals are summed in SQL rather than read row by row. A card's available limit
    /// depends on its whole history, so materialising every charge would make this cost
    /// grow with the years a user has been on the card.
    /// </summary>
    private async Task<IReadOnlyDictionary<Guid, decimal>> CalculateAvailableLimitsAsync(
        Guid userId,
        IReadOnlyDictionary<Guid, CreditCard> cards,
        CancellationToken cancellationToken)
    {
        if (cards.Count == 0) return new Dictionary<Guid, decimal>();

        var charged = await ChargeTotalsAsync(
            dbContext.CreditCardCharges.AsNoTracking()
                .Where(charge => charge.UserId == userId && !charge.IsCancelled),
            cancellationToken);
        var paid = await PaymentTotalsAsync(
            dbContext.CreditCardPayments.AsNoTracking()
                .Where(payment => payment.UserId == userId && !payment.IsCancelled),
            cancellationToken);

        return cards.Values.ToDictionary(
            card => card.Id,
            card => card.CalculateAvailableLimit(
                Math.Max(0m, charged.GetValueOrDefault(card.Id) - paid.GetValueOrDefault(card.Id))));
    }

    // Grouped on the entity rather than on a projected shape: the provider can only
    // translate the aggregate to SQL when it groups the mapped column directly.
    private static Task<Dictionary<Guid, decimal>> ChargeTotalsAsync(
        IQueryable<CreditCardCharge> charges,
        CancellationToken cancellationToken) =>
        charges
            .GroupBy(charge => charge.CreditCardId)
            .Select(group => new { CardId = group.Key, Total = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.CardId, item => item.Total, cancellationToken);

    private static Task<Dictionary<Guid, decimal>> PaymentTotalsAsync(
        IQueryable<CreditCardPayment> payments,
        CancellationToken cancellationToken) =>
        payments
            .GroupBy(payment => payment.CreditCardId)
            .Select(group => new { CardId = group.Key, Total = group.Sum(item => item.Amount.Amount) })
            .ToDictionaryAsync(item => item.CardId, item => item.Total, cancellationToken);

    /// <summary>
    /// Generated occurrences first, then the dates a schedule will still produce.
    /// A projected date is dropped when its occurrence already exists, so the same
    /// obligation never appears twice.
    /// </summary>
    private async Task<IReadOnlyList<PlannedActivityDto>> ListRecurringAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        IReadOnlyDictionary<Guid, Account> accounts,
        IReadOnlyDictionary<Guid, Category> categories,
        IReadOnlyDictionary<Guid, CreditCard> cards,
        IReadOnlyDictionary<Guid, decimal> availableLimits,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        // A pending occurrence whose schedule was deactivated is not an
        // obligation any more, so it drops out of the planned view exactly like
        // the projected dates behind it already do.
        var occurrences = await dbContext.RecurringTransactionOccurrences.AsNoTracking()
            .Where(occurrence => occurrence.UserId == userId &&
                                 occurrence.Status == RecurringOccurrenceStatus.Planned &&
                                 (scope == null || occurrence.Scope == scope) &&
                                 occurrence.ScheduledDate <= horizonDate &&
                                 dbContext.RecurringTransactions.Any(schedule =>
                                     schedule.UserId == userId &&
                                     schedule.Id == occurrence.RecurringTransactionId &&
                                     schedule.IsActive))
            .ToArrayAsync(cancellationToken);
        var schedules = await dbContext.RecurringTransactions.AsNoTracking()
            .Where(recurring => recurring.UserId == userId &&
                                recurring.IsActive &&
                                (scope == null || recurring.Scope == scope) &&
                                recurring.NextOccurrenceDate != null &&
                                recurring.NextOccurrenceDate <= horizonDate)
            .ToArrayAsync(cancellationToken);

        var items = new List<PlannedActivityDto>();
        var covered = new HashSet<(Guid ScheduleId, DateOnly Date)>();
        foreach (var occurrence in occurrences)
        {
            covered.Add((occurrence.RecurringTransactionId, occurrence.ScheduledDate));
            items.Add(BuildRecurring(
                occurrence.Id,
                occurrence.RecurringTransactionId,
                occurrence.SourceType,
                occurrence.AccountId,
                occurrence.CreditCardId,
                occurrence.CategoryId,
                occurrence.Amount,
                occurrence.Kind,
                occurrence.ScheduledDate,
                occurrence.Description,
                isProjected: false,
                asOfDate,
                accounts,
                categories,
                cards,
                availableLimits));
        }

        foreach (var schedule in schedules)
        {
            var dueDate = schedule.NextOccurrenceDate;
            var projectedOccurrenceCount = schedule.GeneratedOccurrenceCount;
            while (dueDate is DateOnly date && date <= horizonDate)
            {
                if (!covered.Contains((schedule.Id, date)))
                {
                    // No occurrence row exists yet, so the schedule identifies the
                    // planned item. Acting on it generates the occurrence first.
                    items.Add(BuildRecurring(
                        schedule.Id,
                        schedule.Id,
                        schedule.SourceType,
                        schedule.AccountId,
                        schedule.CreditCardId,
                        schedule.CategoryId,
                        schedule.Amount,
                        schedule.Kind,
                        date,
                        schedule.Description,
                        isProjected: true,
                        asOfDate,
                        accounts,
                        categories,
                        cards,
                        availableLimits));
                }

                projectedOccurrenceCount++;
                if (schedule.OccurrenceLimit is int occurrenceLimit &&
                    projectedOccurrenceCount >= occurrenceLimit)
                {
                    break;
                }

                if (date == horizonDate) break;
                dueDate = schedule.GetFollowingDate(date);
            }
        }

        return items;
    }

    private static PlannedActivityDto BuildRecurring(
        Guid plannedActivityId,
        Guid scheduleId,
        RecurringSourceType sourceType,
        Guid? accountId,
        Guid? creditCardId,
        Guid categoryId,
        Money amount,
        RecurringTransactionKind kind,
        DateOnly dueDate,
        string? description,
        bool isProjected,
        DateOnly asOfDate,
        IReadOnlyDictionary<Guid, Account> accounts,
        IReadOnlyDictionary<Guid, Category> categories,
        IReadOnlyDictionary<Guid, CreditCard> cards,
        IReadOnlyDictionary<Guid, decimal> availableLimits)
    {
        _ = scheduleId;
        var category = categories.GetValueOrDefault(categoryId);
        PlannedActivityAttention? attention = null;
        Guid? sourceId;
        string? sourceName;

        if (sourceType == RecurringSourceType.CreditCard && creditCardId is Guid cardId)
        {
            var card = cards.GetValueOrDefault(cardId);
            sourceId = cardId;
            sourceName = card?.Name;
            if (card is null || !card.IsActive)
            {
                attention = PlannedActivityAttention.CardInactive;
            }
            else if (availableLimits.GetValueOrDefault(cardId) < amount.Amount)
            {
                attention = PlannedActivityAttention.CardLimitInsufficient;
            }
        }
        else
        {
            var account = accountId is Guid id ? accounts.GetValueOrDefault(id) : null;
            sourceId = accountId;
            sourceName = account?.Name;
            if (account is null || !account.IsActive)
            {
                attention = PlannedActivityAttention.AccountInactive;
            }
        }

        // A blocking source outranks a blocking category: the user has to fix the money
        // side before the category matters.
        if (attention is null && (category is null || !category.IsActive))
        {
            attention = PlannedActivityAttention.CategoryInactive;
        }

        return new PlannedActivityDto(
            plannedActivityId,
            PlannedActivityKind.RecurringOccurrence,
            kind == RecurringTransactionKind.Income
                ? FinancialActivityEffect.Income
                : FinancialActivityEffect.Expense,
            PlannedActivityRules.Classify(dueDate, asOfDate),
            attention is null
                ? PlannedActivityReadiness.Ready
                : PlannedActivityReadiness.NeedsAttention,
            attention,
            PlannedActivityAction.Realize,
            dueDate,
            amount.Amount,
            amount.Currency,
            description ?? category?.Name ?? string.Empty,
            description,
            sourceId,
            sourceName,
            categoryId,
            category?.Name,
            isProjected,

            // Üretilmiş satır kendi occurrence'ını adresler; üretilmemiş satır
            // **planı** adresler ve tarihiyle birlikte gider.
            //
            // Eskiden burada `null` vardı ve sonucu şuydu: planlanan görünümdeki
            // her projeksiyon satırı kalıcı olarak kapalı bir "Gerçekleştir"
            // taşıyordu ve üretme düğmesi başka bir ekrandaydı. Üretmek bir
            // kullanıcı kararı değil, sistemin idempotentlik için tuttuğu bir
            // defter işi; karar gerçekleştirmektir.
            isProjected ? scheduleId : plannedActivityId,
            null);
    }

    private async Task<IReadOnlyList<PlannedActivityDto>> ListInstallmentsAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        IReadOnlyDictionary<Guid, CreditCard> cards,
        IReadOnlyDictionary<Guid, decimal> availableLimits,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var rows = await (
                from item in dbContext.InstallmentItems.AsNoTracking()
                join plan in dbContext.InstallmentPlans.AsNoTracking()
                    on new { item.UserId, PlanId = item.InstallmentPlanId }
                    equals new { plan.UserId, PlanId = plan.Id }
                where item.UserId == userId &&
                      item.CreditCardChargeId == null &&
                      (scope == null || plan.Scope == scope) &&
                      item.ScheduledDate <= horizonDate
                select new
                {
                    item.Id,
                    PlanId = plan.Id,
                    plan.CreditCardId,
                    Amount = item.Amount.Amount,
                    Currency = item.Amount.Currency,
                    item.ScheduledDate,
                    plan.Description,
                    item.Sequence,
                    plan.InstallmentCount
                })
            .ToArrayAsync(cancellationToken);

        return rows.Select(row =>
        {
            var card = cards.GetValueOrDefault(row.CreditCardId);
            PlannedActivityAttention? attention = null;
            if (card is null || !card.IsActive)
            {
                attention = PlannedActivityAttention.CardInactive;
            }
            else if (availableLimits.GetValueOrDefault(row.CreditCardId) < row.Amount)
            {
                attention = PlannedActivityAttention.CardLimitInsufficient;
            }

            return new PlannedActivityDto(
                row.Id,
                PlannedActivityKind.CardInstallment,
                FinancialActivityEffect.Expense,
                PlannedActivityRules.Classify(row.ScheduledDate, asOfDate),
                attention is null
                    ? PlannedActivityReadiness.Ready
                    : PlannedActivityReadiness.NeedsAttention,
                attention,
                PlannedActivityAction.Realize,
                row.ScheduledDate,
                row.Amount,
                row.Currency,
                row.Description ?? card?.Name ?? string.Empty,
                row.Description,
                row.CreditCardId,
                card?.Name,
                null,
                null,
                IsProjected: false,
                ActionTargetId: row.PlanId,
                ActionSequence: row.Sequence);
        }).ToArray();
    }

    /// <summary>
    /// A statement has no table of its own; it is recomputed from the card's cycle and
    /// its charges and payments, which is why it carries the card id as its planned id.
    /// </summary>
    private async Task<IReadOnlyList<PlannedActivityDto>> ListStatementsAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        IReadOnlyDictionary<Guid, CreditCard> cards,
        CancellationToken cancellationToken)
    {
        if (cards.Count == 0) return [];

        var periods = cards.Values.ToDictionary(
            card => card.Id,
            card =>
            {
                var closingMonth = asOfDate.Day >= card.StatementClosingDay
                    ? new DateOnly(asOfDate.Year, asOfDate.Month, 1)
                    : new DateOnly(asOfDate.Year, asOfDate.Month, 1).AddMonths(-1);
                return CreditCardStatementPeriod.ForClosingMonth(
                    card, closingMonth.Year, closingMonth.Month);
            });
        // Only the open statement windows are read in detail. Anything settled before the
        // earliest window is folded into the carried-over balance as a SQL sum, so a long
        // card history never has to be materialised.
        var earliestPeriodStart = periods.Values.Min(period => period.PeriodStart);
        var chargeQuery = dbContext.CreditCardCharges.AsNoTracking()
            .Where(charge => charge.UserId == userId && !charge.IsCancelled);
        var paymentQuery = dbContext.CreditCardPayments.AsNoTracking()
            .Where(payment => payment.UserId == userId && !payment.IsCancelled);

        var historicalCharges = await ChargeTotalsAsync(
            chargeQuery.Where(charge => charge.ChargeDate < earliestPeriodStart),
            cancellationToken);
        var historicalPayments = await PaymentTotalsAsync(
            paymentQuery.Where(payment => payment.PaymentDate < earliestPeriodStart),
            cancellationToken);

        // Movements after the as-of date belong to a future statement.
        var chargesByCard = (await chargeQuery
                .Where(charge => charge.ChargeDate >= earliestPeriodStart &&
                                 charge.ChargeDate <= asOfDate)
                .Select(charge => new CardMovement(
                    charge.CreditCardId, charge.ChargeDate, charge.Amount.Amount))
                .ToArrayAsync(cancellationToken))
            .ToLookup(item => item.CreditCardId);
        var paymentsByCard = (await paymentQuery
                .Where(payment => payment.PaymentDate >= earliestPeriodStart &&
                                  payment.PaymentDate <= asOfDate)
                .Select(payment => new CardMovement(
                    payment.CreditCardId, payment.PaymentDate, payment.Amount.Amount))
                .ToArrayAsync(cancellationToken))
            .ToLookup(item => item.CreditCardId);

        var items = new List<PlannedActivityDto>();
        foreach (var card in cards.Values)
        {
            var period = periods[card.Id];
            if (period.DueDate > horizonDate) continue;

            var cardCharges = chargesByCard[card.Id];
            var cardPayments = paymentsByCard[card.Id];
            var previousCharges = historicalCharges.GetValueOrDefault(card.Id) +
                                  cardCharges.Where(item => item.Date < period.PeriodStart)
                                      .Sum(item => item.Amount);
            var previousPayments = historicalPayments.GetValueOrDefault(card.Id) +
                                   cardPayments.Where(item => item.Date < period.PeriodStart)
                                       .Sum(item => item.Amount);
            var periodCharges = cardCharges
                .Where(item => item.Date >= period.PeriodStart && item.Date <= period.ClosingDate)
                .Sum(item => item.Amount);
            var paymentsThroughClosing = cardPayments
                .Where(item => item.Date >= period.PeriodStart && item.Date <= period.ClosingDate)
                .Sum(item => item.Amount);
            var paymentsAfterClosing = cardPayments
                .Where(item => item.Date > period.ClosingDate && item.Date <= asOfDate)
                .Sum(item => item.Amount);
            var statement = CreditCardStatement.Create(
                card,
                period.ClosingDate.Year,
                period.ClosingDate.Month,
                asOfDate,
                Math.Max(0m, previousCharges - previousPayments),
                periodCharges,
                paymentsThroughClosing,
                paymentsAfterClosing);
            if (statement.RemainingBalance == 0m) continue;

            // Paying a statement settles debt already counted as expense when the
            // charges happened, so it is neutral and adds nothing to the report.
            items.Add(new PlannedActivityDto(
                card.Id,
                PlannedActivityKind.CardStatement,
                FinancialActivityEffect.Neutral,
                PlannedActivityRules.Classify(statement.DueDate, asOfDate),
                PlannedActivityReadiness.Ready,
                null,
                PlannedActivityAction.PayCard,
                statement.DueDate,
                statement.RemainingBalance,
                statement.Currency,
                card.Name,
                $"{statement.Year:D4}-{statement.Month:D2}",
                card.Id,
                card.Name,
                null,
                null,
                IsProjected: false,
                ActionTargetId: card.Id,
                ActionSequence: null));
        }

        return items;
    }

    private async Task<IReadOnlyList<PlannedActivityDto>> ListDebtAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var rows = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                join counterparty in dbContext.Counterparties.AsNoTracking()
                    on new { debt.UserId, Id = debt.CounterpartyId }
                    equals new { counterparty.UserId, counterparty.Id }
                where installment.UserId == userId &&
                      installment.PaymentAccountId == null &&
                      (scope == null || debt.Scope == scope) &&
                      installment.DueDate <= horizonDate
                select new
                {
                    installment.Id,
                    DebtId = debt.Id,
                    installment.Sequence,
                    debt.Direction,
                    CounterpartyName = counterparty.Name,
                    debt.Description,
                    Amount = installment.Amount.Amount,
                    Currency = installment.Amount.Currency,
                    installment.DueDate
                })
            .ToArrayAsync(cancellationToken);

        // Debt movements change liquidity, not income or expense.
        return rows.Select(row => new PlannedActivityDto(
            row.Id,
            row.Direction == DebtDirection.Payable
                ? PlannedActivityKind.DebtInstallment
                : PlannedActivityKind.ReceivableInstallment,
            FinancialActivityEffect.Neutral,
            PlannedActivityRules.Classify(row.DueDate, asOfDate),
            PlannedActivityReadiness.Ready,
            null,
            row.Direction == DebtDirection.Payable
                ? PlannedActivityAction.PayDebt
                : PlannedActivityAction.CollectDebt,
            row.DueDate,
            row.Amount,
            row.Currency,
            row.Description ?? row.CounterpartyName,
            row.Description,
            null,
            null,
            null,
            null,
            IsProjected: false,
            ActionTargetId: row.DebtId,
            ActionSequence: row.Sequence)).ToArray();
    }

    /// <summary>
    /// Open one-time obligations are read in one owner-scoped SQL projection. Whether
    /// an item is still actionable comes from the current obligation/settlement state;
    /// no overdue or readiness flag is stored.
    /// </summary>
    private async Task<IReadOnlyList<PlannedActivityDto>> ListObligationsAsync(
        Guid userId,
        DateOnly asOfDate,
        DateOnly horizonDate,
        TransactionScope? scope,
        CancellationToken cancellationToken)
    {
        var rows = await (
                from obligation in dbContext.Obligations.AsNoTracking()
                join category in dbContext.Categories.AsNoTracking()
                    on new { obligation.UserId, Id = obligation.CategoryId }
                    equals new { category.UserId, category.Id }
                join counterparty in dbContext.Counterparties.AsNoTracking()
                    on new { obligation.UserId, Id = obligation.CounterpartyId }
                    equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                    into counterparties
                from counterparty in counterparties.DefaultIfEmpty()
                where obligation.UserId == userId &&
                      !obligation.IsCancelled &&
                      !dbContext.ObligationSettlements.Any(settlement =>
                          settlement.UserId == userId &&
                          settlement.ObligationId == obligation.Id) &&
                      (scope == null || obligation.Scope == scope) &&
                      obligation.DueDate <= horizonDate
                select new
                {
                    obligation.Id,
                    obligation.Direction,
                    Amount = obligation.Amount.Amount,
                    Currency = obligation.Amount.Currency,
                    obligation.DueDate,
                    obligation.Description,
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    CounterpartyName = counterparty == null ? null : counterparty.Name
                })
            .ToArrayAsync(cancellationToken);

        return rows.Select(row => new PlannedActivityDto(
            row.Id,
            row.Direction == DebtDirection.Payable
                ? PlannedActivityKind.PayableObligation
                : PlannedActivityKind.ReceivableObligation,
            // The economic event was recognized on IssueDate. Settlement only carries
            // cash, so the remaining planned movement is neutral (ADR 0014).
            FinancialActivityEffect.Neutral,
            PlannedActivityRules.Classify(row.DueDate, asOfDate),
            PlannedActivityReadiness.Ready,
            AttentionCode: null,
            row.Direction == DebtDirection.Payable
                ? PlannedActivityAction.PayObligation
                : PlannedActivityAction.CollectObligation,
            row.DueDate,
            row.Amount,
            row.Currency,
            row.Description ?? row.CounterpartyName ?? row.CategoryName,
            row.Description,
            SourceId: null,
            SourceName: null,
            row.CategoryId,
            row.CategoryName,
            IsProjected: false,
            ActionTargetId: row.Id,
            ActionSequence: null)).ToArray();
    }
}
