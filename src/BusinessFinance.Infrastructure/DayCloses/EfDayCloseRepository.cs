using System.Linq.Expressions;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Application.Pos;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.DayCloses;

internal sealed class EfDayCloseRepository(
    BusinessFinanceDbContext dbContext,
    TimeProvider timeProvider)
    : IDayCloseRepository, IDayCloseCountReader
{
    public async Task<DayCloseDto?> GetAsync(
        Guid dayCloseId,
        Guid userId,
        CancellationToken cancellationToken) =>
        (await LoadAsync(
            userId, dayClose => dayClose.Id == dayCloseId, cancellationToken))
        .SingleOrDefault();

    public Task<IReadOnlyList<DayCloseDto>> ListAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken) =>
        LoadAsync(
            userId,
            dayClose => !dayClose.IsCancelled &&
                        dayClose.ClosedOn >= from &&
                        (dayClose.RangeStart ?? dayClose.ClosedOn) <= to,
            cancellationToken);

    public async Task<IReadOnlyList<DayCloseSummaryDto>> ListCoveringAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken) =>
        await dbContext.DayCloses.AsNoTracking()
            .Where(dayClose => dayClose.UserId == userId &&
                               !dayClose.IsCancelled &&
                               dayClose.ClosedOn >= from &&
                               (dayClose.RangeStart ?? dayClose.ClosedOn) <= to)
            .OrderBy(dayClose => dayClose.ClosedOn)
            .ThenBy(dayClose => dayClose.CreatedAtUtc)
            .Select(dayClose => new DayCloseSummaryDto(
                dayClose.Id,
                dayClose.ClosedOn,
                dayClose.RangeStart,
                dayClose.ZNumber,
                dayClose.IsAdditional))
            .ToArrayAsync(cancellationToken);

    public Task<bool> ZNumberExistsAsync(
        Guid userId,
        int zNumber,
        CancellationToken cancellationToken) =>
        dbContext.DayCloses.AsNoTracking().AnyAsync(
            dayClose => dayClose.UserId == userId &&
                        !dayClose.IsCancelled &&
                        dayClose.ZNumber == zNumber,
            cancellationToken);

    public Task<bool> IsCountedAsync(
        Guid userId,
        DayCloseRecordKind kind,
        Guid recordId,
        CancellationToken cancellationToken) =>
        dbContext.DayCloseCountedRecords.AsNoTracking().AnyAsync(
            counted => counted.UserId == userId &&
                       counted.Kind == kind &&
                       counted.RecordId == recordId,
            cancellationToken);

    public async Task<IReadOnlyList<DayCloseExistingRecordDto>> ListExistingRecordsAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken) =>
        // Bir gün sonunun zaten saydığı kayıt yeniden sayılamaz: listede yoktur.
        (await ReadRecordsAsync(userId, from, to, cancellationToken))
        .Where(row => row.CountedBy is null)
        .Select(row => row.Record)
        .ToArray();

    public async Task<IReadOnlyList<PosDefinition>> ListActivePosDefinitionsAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        await dbContext.PosDefinitions.AsNoTracking()
            .Where(definition => definition.UserId == userId && definition.IsActive)
            .OrderByDescending(definition => definition.IsDefault)
            .ThenBy(definition => definition.Name)
            .ToArrayAsync(cancellationToken);

    public async Task<DayCloseDefaults> GetDefaultsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var last = await dbContext.Transactions.AsNoTracking()
            .Where(transaction => transaction.UserId == userId &&
                                  transaction.DayCloseId != null &&
                                  !transaction.IsCancelled)
            .OrderByDescending(transaction => transaction.TransactionDate)
            .ThenByDescending(transaction =>
                EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName))
            .Select(transaction => new { transaction.AccountId, transaction.CategoryId })
            .FirstOrDefaultAsync(cancellationToken);

        // Kasa: şahsi etiketli olmayan aktif nakit hesaplar (KP15).
        var cashAccountIds = await dbContext.Accounts.AsNoTracking()
            .Where(account => account.UserId == userId &&
                              account.IsActive &&
                              account.Type == AccountType.Cash &&
                              account.DefaultScope != TransactionScope.Personal)
            .OrderBy(account => account.Name)
            .Select(account => account.Id)
            .ToArrayAsync(cancellationToken);

        return new DayCloseDefaults(last?.AccountId, last?.CategoryId, cashAccountIds);
    }

    public Task<DayClose?> FindOwnedByIdAsync(
        Guid dayCloseId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken) =>
        (track ? dbContext.DayCloses : dbContext.DayCloses.AsNoTracking())
            .SingleOrDefaultAsync(
                dayClose => dayClose.Id == dayCloseId && dayClose.UserId == userId,
                cancellationToken);

    public async Task<(IReadOnlyList<BudgetTransaction> Incomes, IReadOnlyList<PosSettlement> Settlements)>
        FindRecordsAsync(Guid dayCloseId, Guid userId, CancellationToken cancellationToken)
    {
        var incomes = await dbContext.Transactions
            .Where(transaction => transaction.UserId == userId &&
                                  transaction.DayCloseId == dayCloseId)
            .ToArrayAsync(cancellationToken);
        var settlements = await dbContext.PosSettlements
            .Where(settlement => settlement.UserId == userId &&
                                 settlement.DayCloseId == dayCloseId)
            .ToArrayAsync(cancellationToken);
        return (incomes, settlements);
    }

    public async Task<bool> TryAddAsync(
        DayClose dayClose,
        IReadOnlyCollection<BudgetTransaction> incomes,
        IReadOnlyCollection<PosSettlement> settlements,
        IReadOnlyCollection<DayCloseCountedRecord> counted,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.DayCloses.AddAsync(dayClose, cancellationToken);
            await dbContext.Transactions.AddRangeAsync(incomes, cancellationToken);
            await dbContext.PosSettlements.AddRangeAsync(settlements, cancellationToken);
            await dbContext.DayCloseCountedRecords.AddRangeAsync(counted, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Aynı istek kimliği aynı gün sonu kimliğini üretir; aynı günü
            // kapatan ya da aynı kaydı sayan eşzamanlı ikinci istek tekil
            // indekse çarpar.
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task ReleaseCountedAsync(
        Guid dayCloseId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var counted = await dbContext.DayCloseCountedRecords
            .Where(record => record.UserId == userId && record.DayCloseId == dayCloseId)
            .ToArrayAsync(cancellationToken);
        dbContext.DayCloseCountedRecords.RemoveRange(counted);
    }

    public async Task<bool> TrySaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Ürettiği bir tahsilat bu sırada bir yatışla kapandı.
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    private sealed record RecordRow(DayCloseExistingRecordDto Record, Guid? CountedBy);

    /// <summary>
    /// Aralıkta tek tek girilmiş, bir gün sonunda sayılabilecek kayıtlar ve
    /// (sayıldıysa) onları sayan gün sonu.
    /// </summary>
    /// <remarks>
    /// "Zaten girilmiş kayıtlar" listesi ile gün sonunun "saydığı kayıtlar"
    /// aynı okumadır; yalnız biri sayılmamışları, diğeri sayılmışları alır.
    /// Kural ikinci bir yerde yeniden yazılmaz.
    /// </remarks>
    private async Task<IReadOnlyList<RecordRow>> ReadRecordsAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        // Nakit tarafı: nakit hesaba tek tek girilmiş işletme gelirleri. Banka
        // hesabına yazılan gelir (havale) Z'de ne NAKİT ne KART'tır; şahsi
        // nakit gelir satış değildir.
        var incomes = await (
            from transaction in dbContext.Transactions.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { transaction.UserId, Id = transaction.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { transaction.UserId, Id = transaction.CategoryId }
                equals new { category.UserId, category.Id }
            where transaction.UserId == userId &&
                  !transaction.IsCancelled &&
                  transaction.DayCloseId == null &&
                  transaction.Type == TransactionType.Income &&
                  transaction.Scope == TransactionScope.Business &&
                  account.Type == AccountType.Cash &&
                  transaction.TransactionDate >= @from &&
                  transaction.TransactionDate <= to
            orderby transaction.TransactionDate
            select new
            {
                transaction.Id,
                Date = transaction.TransactionDate,
                Amount = transaction.Amount.Amount,
                Title = transaction.Description ?? category.Name,
                AccountName = account.Name,
                CountedBy = dbContext.DayCloseCountedRecords
                    .Where(counted => counted.UserId == userId &&
                                      counted.Kind == DayCloseRecordKind.Income &&
                                      counted.RecordId == transaction.Id)
                    .Select(counted => (Guid?)counted.DayCloseId)
                    .FirstOrDefault(),
            })
            .ToArrayAsync(cancellationToken);

        // Kart tarafı: tek tek girilmiş POS tahsilatları.
        var settlements = await (
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CategoryId }
                equals new { category.UserId, category.Id }
            where settlement.UserId == userId &&
                  !settlement.IsCancelled &&
                  settlement.DayCloseId == null &&
                  settlement.SettlementDate >= @from &&
                  settlement.SettlementDate <= to
            orderby settlement.SettlementDate, settlement.CreatedAtUtc
            select new
            {
                settlement.Id,
                Date = settlement.SettlementDate,
                Amount = settlement.GrossAmount.Amount,
                Title = settlement.Description ?? category.Name,
                settlement.PosDefinitionId,
                AccountName = account.Name,
                CountedBy = dbContext.DayCloseCountedRecords
                    .Where(counted => counted.UserId == userId &&
                                      counted.Kind == DayCloseRecordKind.PosSettlement &&
                                      counted.RecordId == settlement.Id)
                    .Select(counted => (Guid?)counted.DayCloseId)
                    .FirstOrDefault(),
            })
            .ToArrayAsync(cancellationToken);

        // Nakit cari tahsilat çoğu zaman yazar kasadan geçmez; listelenir ama
        // işaretsiz gelir (ADR 0019 T2).
        var counterpartyPayments = await (
            from payment in dbContext.CounterpartyPayments.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { payment.UserId, Id = payment.AccountId }
                equals new { account.UserId, account.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { payment.UserId, Id = payment.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
            where payment.UserId == userId &&
                  !payment.IsCancelled &&
                  payment.Direction == DebtDirection.Receivable &&
                  account.Type == AccountType.Cash &&
                  payment.PaymentDate >= @from &&
                  payment.PaymentDate <= to
            orderby payment.PaymentDate
            select new
            {
                payment.Id,
                Date = payment.PaymentDate,
                Amount = payment.Amount.Amount,
                Title = payment.Description ?? counterparty.Name,
                AccountName = account.Name,
                CountedBy = dbContext.DayCloseCountedRecords
                    .Where(counted => counted.UserId == userId &&
                                      counted.Kind == DayCloseRecordKind.CounterpartyPayment &&
                                      counted.RecordId == payment.Id)
                    .Select(counted => (Guid?)counted.DayCloseId)
                    .FirstOrDefault(),
            })
            .ToArrayAsync(cancellationToken);

        var obligationSettlements = await (
            from settlement in dbContext.ObligationSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join obligation in dbContext.Obligations.AsNoTracking()
                on new { settlement.UserId, Id = settlement.ObligationId }
                equals new { obligation.UserId, obligation.Id }
            where settlement.UserId == userId &&
                  !settlement.IsCancelled &&
                  settlement.Direction == DebtDirection.Receivable &&
                  account.Type == AccountType.Cash &&
                  settlement.SettlementDate >= @from &&
                  settlement.SettlementDate <= to
            orderby settlement.SettlementDate
            select new
            {
                settlement.Id,
                Date = settlement.SettlementDate,
                Amount = settlement.Amount.Amount,
                Title = obligation.Description ?? string.Empty,
                AccountName = account.Name,
                CountedBy = dbContext.DayCloseCountedRecords
                    .Where(counted => counted.UserId == userId &&
                                      counted.Kind == DayCloseRecordKind.ObligationSettlement &&
                                      counted.RecordId == settlement.Id)
                    .Select(counted => (Guid?)counted.DayCloseId)
                    .FirstOrDefault(),
            })
            .ToArrayAsync(cancellationToken);

        return
        [
            .. incomes.Select(row => new RecordRow(
                new DayCloseExistingRecordDto(
                    DayCloseRecordKind.Income, row.Id, DayCloseSide.Cash, row.Date, row.Amount,
                    row.Title, null, row.AccountName, true, true),
                row.CountedBy)),
            .. counterpartyPayments.Select(row => new RecordRow(
                new DayCloseExistingRecordDto(
                    DayCloseRecordKind.CounterpartyPayment, row.Id, DayCloseSide.Cash, row.Date,
                    row.Amount, row.Title, null, row.AccountName, false, false),
                row.CountedBy)),
            .. obligationSettlements.Select(row => new RecordRow(
                new DayCloseExistingRecordDto(
                    DayCloseRecordKind.ObligationSettlement, row.Id, DayCloseSide.Cash, row.Date,
                    row.Amount, row.Title, null, row.AccountName, false, false),
                row.CountedBy)),
            .. settlements.Select(row => new RecordRow(
                new DayCloseExistingRecordDto(
                    DayCloseRecordKind.PosSettlement, row.Id, DayCloseSide.Card, row.Date,
                    row.Amount, row.Title, row.PosDefinitionId, row.AccountName, true, true),
                row.CountedBy)),
        ];
    }

    /// <summary>
    /// Gün sonlarını yazdıkları ve saydıkları kayıtlarla okur; sorgu sayısı gün
    /// sonu adediyle büyümez.
    /// </summary>
    private async Task<IReadOnlyList<DayCloseDto>> LoadAsync(
        Guid userId,
        Expression<Func<DayClose, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var dayCloses = await dbContext.DayCloses.AsNoTracking()
            .Where(dayClose => dayClose.UserId == userId)
            .Where(predicate)
            .OrderByDescending(dayClose => dayClose.ClosedOn)
            .ThenByDescending(dayClose => dayClose.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
        if (dayCloses.Length == 0)
        {
            return [];
        }

        var ids = dayCloses.Select(dayClose => (Guid?)dayClose.Id).ToArray();
        var incomes = await (
            from transaction in dbContext.Transactions.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { transaction.UserId, Id = transaction.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { transaction.UserId, Id = transaction.CategoryId }
                equals new { category.UserId, category.Id }
            where transaction.UserId == userId && ids.Contains(transaction.DayCloseId)
            orderby transaction.TransactionDate
            select new
            {
                transaction.DayCloseId,
                Income = new DayCloseIncomeDto(
                    transaction.Id,
                    account.Id,
                    account.Name,
                    category.Id,
                    category.Name,
                    transaction.Amount.Amount,
                    transaction.TransactionDate,
                    transaction.Scope,
                    transaction.IsCancelled),
            })
            .ToArrayAsync(cancellationToken);

        var settlements = await (
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CategoryId }
                equals new { category.UserId, category.Id }
            join commissionCategory in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CommissionCategoryId }
                equals new { commissionCategory.UserId, Id = (Guid?)commissionCategory.Id }
                into commissionCategories
            from commissionCategory in commissionCategories.DefaultIfEmpty()
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { settlement.UserId, Id = settlement.PosDefinitionId }
                equals new { definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
            where settlement.UserId == userId && ids.Contains(settlement.DayCloseId)
            orderby settlement.SettlementDate, settlement.CreatedAtUtc
            select new
            {
                settlement,
                AccountName = account.Name,
                CategoryName = category.Name,
                CommissionCategoryName = commissionCategory == null
                    ? null
                    : commissionCategory.Name,
                DefinitionName = definition == null ? null : definition.Name,
            })
            .ToArrayAsync(cancellationToken);

        // Sayılan kayıtlar gün sonunun kapattığı günlerdedir.
        var counted = (await ReadRecordsAsync(
                userId,
                dayCloses.Min(dayClose => dayClose.FirstDay),
                dayCloses.Max(dayClose => dayClose.ClosedOn),
                cancellationToken))
            .Where(row => row.CountedBy is not null)
            .ToArray();

        var asOfDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        return dayCloses
            .Select(dayClose =>
            {
                var ownIncomes = incomes
                    .Where(row => row.DayCloseId == dayClose.Id)
                    .Select(row => row.Income)
                    .ToArray();
                var ownSettlements = settlements
                    .Where(row => row.settlement.DayCloseId == dayClose.Id)
                    .ToArray();
                var ownCounted = counted
                    .Where(row => row.CountedBy == dayClose.Id)
                    .Select(row => row.Record)
                    .ToArray();
                return new DayCloseDto(
                    dayClose.Id,
                    dayClose.ClosedOn,
                    dayClose.RangeStart,
                    dayClose.ZNumber,
                    dayClose.IsAdditional,
                    dayClose.IsCancelled,
                    dayClose.CancelledAtUtc,
                    dayClose.CreatedAtUtc,
                    ownIncomes,
                    ownSettlements
                        .Select(row => PosSettlementMapper.ToDto(
                            row.settlement,
                            row.AccountName,
                            row.CategoryName,
                            row.CommissionCategoryName,
                            asOfDate,
                            row.DefinitionName))
                        .ToArray(),
                    ownIncomes.Sum(income => income.Amount),
                    ownSettlements.Sum(row => row.settlement.GrossAmount.Amount),
                    ownSettlements.Sum(row => row.settlement.CommissionAmount),
                    CurrencyCode.TRY,
                    ownCounted,
                    ownCounted.Where(record => record.Side == DayCloseSide.Cash)
                        .Sum(record => record.Amount),
                    ownCounted.Where(record => record.Side == DayCloseSide.Card)
                        .Sum(record => record.Amount));
            })
            .ToArray();
    }
}
