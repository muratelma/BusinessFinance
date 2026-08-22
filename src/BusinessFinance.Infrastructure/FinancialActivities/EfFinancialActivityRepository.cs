using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.FinancialActivities;

/// <summary>
/// Projects every realized economic event into one shape and merges them with UNION ALL
/// so the database does the ordering, paging and counting.
/// </summary>
/// <remarks>
/// Reading each source separately and merging in memory would satisfy a bounded
/// query-count check while still loading the owner's entire history on every page
/// request. Keeping the merge in SQL is what makes page three cost the same as page one.
/// </remarks>
internal sealed class EfFinancialActivityRepository(BusinessFinanceDbContext dbContext)
    : IFinancialActivityRepository, IActivityOriginReader
{
    public async Task<FinancialActivityOrigin> GetTransactionOriginAsync(
        Guid userId,
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.ImportRows.AsNoTracking().AnyAsync(
                row => row.UserId == userId && row.BudgetTransactionId == transactionId,
                cancellationToken))
        {
            return FinancialActivityOrigin.CsvImport;
        }

        return await dbContext.RecurringTransactionOccurrences.AsNoTracking().AnyAsync(
            occurrence => occurrence.UserId == userId &&
                          occurrence.BudgetTransactionId == transactionId,
            cancellationToken)
            ? FinancialActivityOrigin.Recurring
            : FinancialActivityOrigin.Manual;
    }

    public async Task<FinancialActivityOrigin> GetCardChargeOriginAsync(
        Guid userId,
        Guid creditCardChargeId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.RecurringTransactionOccurrences.AsNoTracking().AnyAsync(
                occurrence => occurrence.UserId == userId &&
                              occurrence.CreditCardChargeId == creditCardChargeId,
                cancellationToken))
        {
            return FinancialActivityOrigin.Recurring;
        }

        return await dbContext.InstallmentItems.AsNoTracking().AnyAsync(
            item => item.UserId == userId && item.CreditCardChargeId == creditCardChargeId,
            cancellationToken)
            ? FinancialActivityOrigin.Installment
            : FinancialActivityOrigin.Manual;
    }

    public async Task<FinancialActivityPage> ListAsync(
        Guid userId,
        FinancialActivityListCriteria criteria,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var merged = BuildMergedQuery(userId);
        merged = ApplyFilters(merged, criteria);

        var totalCount = await merged.CountAsync(cancellationToken);
        var rows = await merged
            .OrderByDescending(row => row.ActivityDate)
            .ThenBy(row => row.ActivityKind)
            .ThenBy(row => row.ActivityId)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new FinancialActivityPage(
            rows.Select(Map).ToArray(),
            totalCount);
    }

    /// <summary>
    /// The six write models, each projected to the identical anonymous shape that
    /// <see cref="Queryable.Concat"/> needs to become a single UNION ALL.
    /// </summary>
    private IQueryable<ActivityRow> BuildMergedQuery(Guid userId)
    {
        var accountTransactions =
            from transaction in dbContext.Transactions.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { transaction.UserId, Id = transaction.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { transaction.UserId, Id = transaction.CategoryId }
                equals new { category.UserId, category.Id }
            where transaction.UserId == userId
            select new ActivityRow
            {
                ActivityId = transaction.Id,
                ActivityKind = (int)FinancialActivityKind.AccountTransaction,
                Effect = transaction.Type == TransactionType.Income
                    ? (int)FinancialActivityEffect.Income
                    : (int)FinancialActivityEffect.Expense,
                SourceGroup = (int)FinancialActivitySourceGroup.Account,
                // Correlated EXISTS rather than joins: both links are backed by filtered
                // unique indexes, and a join would multiply rows if one ever were not.
                Origin = dbContext.ImportRows.Any(row =>
                        row.UserId == userId && row.BudgetTransactionId == transaction.Id)
                    ? (int)FinancialActivityOrigin.CsvImport
                    : dbContext.RecurringTransactionOccurrences.Any(occurrence =>
                        occurrence.UserId == userId &&
                        occurrence.BudgetTransactionId == transaction.Id)
                        ? (int)FinancialActivityOrigin.Recurring
                        : (int)FinancialActivityOrigin.Manual,
                Status = transaction.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = transaction.TransactionDate,
                Amount = transaction.Amount.Amount,
                Currency = (int)transaction.Amount.Currency,
                // A category is a reporting bucket shared by many movements, so
                // it cannot identify one. Whatever the user wrote wins; the
                // category names the row only when they wrote nothing.
                Title = transaction.Description ?? category.Name,
                Description = transaction.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = account.Id,
                SourceName = account.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = transaction.CancelledAtUtc,
                Scope = (int?)transaction.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = category.Id
            };

        var transfers =
            from transfer in dbContext.Transfers.AsNoTracking()
            join source in dbContext.Accounts.AsNoTracking()
                on new { transfer.UserId, Id = transfer.SourceAccountId }
                equals new { source.UserId, source.Id }
            join destination in dbContext.Accounts.AsNoTracking()
                on new { transfer.UserId, Id = transfer.DestinationAccountId }
                equals new { destination.UserId, destination.Id }
            where transfer.UserId == userId
            select new ActivityRow
            {
                ActivityId = transfer.Id,
                ActivityKind = (int)FinancialActivityKind.Transfer,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Transfer,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = transfer.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = transfer.TransferDate,
                Amount = transfer.Amount.Amount,
                Currency = (int)transfer.Amount.Currency,
                Title = transfer.Description ?? destination.Name,
                Description = transfer.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = source.Id,
                SourceName = source.Name,
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                CancelledAtUtc = transfer.CancelledAtUtc,
                Scope = (int?)null,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = source.Id,
                MatchSecondAccountId = destination.Id,
                MatchCreditCardId = null,
                MatchCategoryId = null
            };

        var cardCharges =
            from charge in dbContext.CreditCardCharges.AsNoTracking()
            join card in dbContext.CreditCards.AsNoTracking()
                on new { charge.UserId, Id = charge.CreditCardId }
                equals new { card.UserId, card.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { charge.UserId, Id = charge.CategoryId }
                equals new { category.UserId, category.Id }
            where charge.UserId == userId
            select new ActivityRow
            {
                ActivityId = charge.Id,
                ActivityKind = (int)FinancialActivityKind.CardCharge,
                Effect = (int)FinancialActivityEffect.Expense,
                SourceGroup = (int)FinancialActivitySourceGroup.CreditCard,
                Origin = dbContext.RecurringTransactionOccurrences.Any(occurrence =>
                        occurrence.UserId == userId &&
                        occurrence.CreditCardChargeId == charge.Id)
                    ? (int)FinancialActivityOrigin.Recurring
                    : dbContext.InstallmentItems.Any(item =>
                        item.UserId == userId && item.CreditCardChargeId == charge.Id)
                        ? (int)FinancialActivityOrigin.Installment
                        : (int)FinancialActivityOrigin.Manual,
                Status = charge.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = charge.ChargeDate,
                Amount = charge.Amount.Amount,
                Currency = (int)charge.Amount.Currency,
                Title = charge.Description ?? category.Name,
                Description = charge.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = card.Id,
                SourceName = card.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = charge.CancelledAtUtc,
                Scope = (int?)charge.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = null,
                MatchSecondAccountId = null,
                MatchCreditCardId = card.Id,
                MatchCategoryId = category.Id
            };

        var cardPayments =
            from payment in dbContext.CreditCardPayments.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { payment.UserId, Id = payment.AccountId }
                equals new { account.UserId, account.Id }
            join card in dbContext.CreditCards.AsNoTracking()
                on new { payment.UserId, Id = payment.CreditCardId }
                equals new { card.UserId, card.Id }
            where payment.UserId == userId
            select new ActivityRow
            {
                ActivityId = payment.Id,
                ActivityKind = (int)FinancialActivityKind.CardPayment,
                // Paying the card moves money without spending it. Counting this as an
                // expense would charge the same purchase twice.
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.CreditCard,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = payment.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = payment.PaymentDate,
                Amount = payment.Amount.Amount,
                Currency = (int)payment.Amount.Currency,
                Title = payment.Description ?? card.Name,
                Description = payment.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = account.Id,
                SourceName = account.Name,
                DestinationId = card.Id,
                DestinationName = card.Name,
                CancelledAtUtc = payment.CancelledAtUtc,
                Scope = (int?)null,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = card.Id,
                MatchCategoryId = null
            };

        // Only paid installments are realized events. Unpaid ones belong to the planned
        // view and must never reach this feed.
        var debtActivities =
            from installment in dbContext.DebtInstallments.AsNoTracking()
            join debt in dbContext.DebtAgreements.AsNoTracking()
                on new { installment.UserId, Id = installment.DebtAgreementId }
                equals new { debt.UserId, debt.Id }
            join account in dbContext.Accounts.AsNoTracking()
                on new { installment.UserId, Id = installment.PaymentAccountId!.Value }
                equals new { account.UserId, account.Id }
            where installment.UserId == userId && installment.PaymentAccountId != null
            select new ActivityRow
            {
                ActivityId = installment.Id,
                ActivityKind = debt.Direction == DebtDirection.Payable
                    ? (int)FinancialActivityKind.DebtPayment
                    : (int)FinancialActivityKind.DebtCollection,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Debt,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = (int)FinancialActivityStatus.Realized,
                ActivityDate = installment.PaymentDate!.Value,
                Amount = installment.Amount.Amount,
                // Debt money stores its currency as varchar while every other money
                // column is tinyint, so a plain cast would ask SQL Server to turn 'TRY'
                // into an int. Compare instead, which translates to a CASE over the
                // string column and keeps the union's column types aligned. An unknown
                // currency lands on 0 and is rejected loudly in Map rather than being
                // silently read back as a different currency.
                Currency = installment.Amount.Currency == CurrencyCode.TRY
                    ? (int)CurrencyCode.TRY
                    : 0,
                Title = debt.Description ?? debt.CounterpartyName,
                Description = debt.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = account.Id,
                SourceName = account.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = null,
                PrincipalPortion = installment.PrincipalPortion,
                Scope = (int?)debt.Scope,
                InterestPortion = installment.InterestPortion,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = null
            };

        // Borcun doğduğu an. Bu satır olmadan feed yalnız parayı çıkarken
        // gösteriyor, nereden geldiğini hiç göstermiyordu. İki kaynak iki ayrı
        // dal: nakitte bir hesap, giderde bir kategori bağlanır ve her dal
        // kendi inner join'iyle kalır — tek dalda iki left join, birleşimin
        // kolon tiplerini gereksiz yere nullable yapardı.
        var debtCashOpenings =
            from debt in dbContext.DebtAgreements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { debt.UserId, Id = debt.OpeningAccountId!.Value }
                equals new { account.UserId, account.Id }
            where debt.UserId == userId && debt.SourceType == DebtSourceType.Cash
            select new ActivityRow
            {
                ActivityId = debt.Id,
                ActivityKind = (int)FinancialActivityKind.DebtOpening,

                // Para el değiştirdi, tüketilmedi: ne gelir ne gider.
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Debt,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = (int)FinancialActivityStatus.Realized,
                ActivityDate = debt.StartDate,
                Amount = debt.Principal.Amount,
                Currency = debt.Principal.Currency == CurrencyCode.TRY
                    ? (int)CurrencyCode.TRY
                    : 0,
                Title = debt.CounterpartyName,
                Description = debt.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = account.Id,
                SourceName = account.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = null,
                Scope = (int?)debt.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = null
            };

        var debtCategoricalOpenings =
            from debt in dbContext.DebtAgreements.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking()
                on new { debt.UserId, Id = debt.CategoryId!.Value }
                equals new { category.UserId, category.Id }
            where debt.UserId == userId &&
                  (debt.SourceType == DebtSourceType.Expense ||
                   debt.SourceType == DebtSourceType.Income)
            select new ActivityRow
            {
                ActivityId = debt.Id,
                ActivityKind = (int)FinancialActivityKind.DebtOpening,

                // Olay tam bu gün oldu: borçta tüketim, alacakta satış.
                // Taksitler ayrıca gider/gelir üretmez; üretselerdi aynı olay
                // iki kez sayılırdı.
                Effect = debt.SourceType == DebtSourceType.Expense
                    ? (int)FinancialActivityEffect.Expense
                    : (int)FinancialActivityEffect.Income,
                SourceGroup = (int)FinancialActivitySourceGroup.Debt,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = (int)FinancialActivityStatus.Realized,
                ActivityDate = debt.StartDate,
                Amount = debt.Principal.Amount,
                Currency = debt.Principal.Currency == CurrencyCode.TRY
                    ? (int)CurrencyCode.TRY
                    : 0,
                Title = debt.CounterpartyName,
                Description = debt.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = null,
                SourceName = null,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = null,
                Scope = (int?)debt.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = null,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = category.Id
            };

        return accountTransactions
            .Concat(transfers)
            .Concat(cardCharges)
            .Concat(cardPayments)
            .Concat(debtActivities)
            .Concat(debtCashOpenings)
            .Concat(debtCategoricalOpenings);
    }

    private static IQueryable<ActivityRow> ApplyFilters(
        IQueryable<ActivityRow> query,
        FinancialActivityListCriteria criteria)
    {
        if (!criteria.IncludeCancelled)
        {
            query = query.Where(row => row.Status != (int)FinancialActivityStatus.Cancelled);
        }

        if (criteria.DateFrom is DateOnly from)
        {
            query = query.Where(row => row.ActivityDate >= from);
        }

        if (criteria.DateTo is DateOnly to)
        {
            query = query.Where(row => row.ActivityDate <= to);
        }

        if (criteria.SourceGroup is FinancialActivitySourceGroup sourceGroup)
        {
            query = query.Where(row => row.SourceGroup == (int)sourceGroup);
        }

        if (criteria.ActivityKind is FinancialActivityKind kind)
        {
            query = query.Where(row => row.ActivityKind == (int)kind);
        }

        if (criteria.Effect is FinancialActivityEffect effect)
        {
            query = query.Where(row => row.Effect == (int)effect);
        }

        if (criteria.Origin is FinancialActivityOrigin origin)
        {
            query = query.Where(row => row.Origin == (int)origin);
        }

        // Kapsam filtresi kapsamsız satırları da eler ve elemesi gerekir.
        // Transfer ile kart ödemesinin kapsamı yoktur (ADR 0002, ADR 0003);
        // ikisini birden iki listede birden göstermek, kullanıcı iki tarafı
        // karşılaştırdığında aynı para hareketini iki kez saydırırdı.
        if (criteria.Scope is TransactionScope scope)
        {
            query = query.Where(row => row.Scope == (int)scope);
        }

        // Matched against the dedicated columns rather than SourceId, so an account
        // filter can never match a card that happens to share the position.
        if (criteria.AccountId is Guid accountId)
        {
            query = query.Where(row =>
                row.MatchAccountId == accountId || row.MatchSecondAccountId == accountId);
        }

        if (criteria.CreditCardId is Guid creditCardId)
        {
            query = query.Where(row => row.MatchCreditCardId == creditCardId);
        }

        if (criteria.CategoryId is Guid categoryId)
        {
            query = query.Where(row => row.MatchCategoryId == categoryId);
        }

        return query;
    }

    private static FinancialActivityRow Map(ActivityRow row)
    {
        if (!Enum.IsDefined((CurrencyCode)row.Currency))
        {
            throw new InvalidOperationException(
                $"Activity '{row.ActivityId}' carries a currency this projection cannot map.");
        }

        return MapCore(row);
    }

    private static FinancialActivityRow MapCore(ActivityRow row) => new(
        row.ActivityId,
        (FinancialActivityKind)row.ActivityKind,
        (FinancialActivityEffect)row.Effect,
        (FinancialActivitySourceGroup)row.SourceGroup,
        (FinancialActivityOrigin)row.Origin,
        (FinancialActivityStatus)row.Status,
        row.ActivityDate,
        row.Amount,
        (CurrencyCode)row.Currency,
        row.Title,
        row.Description,
        row.CategoryId,
        row.CategoryName,
        row.SourceId,
        row.SourceName,
        row.DestinationId,
        row.DestinationName,
        row.CancelledAtUtc,
        row.Scope is int scope ? (TransactionScope)scope : null,
        row.PrincipalPortion,
        row.InterestPortion);

    /// <summary>
    /// The shared UNION ALL shape. Enums are carried as int so every branch produces the
    /// same column types; the Match* columns exist only for filtering.
    /// </summary>
    private sealed class ActivityRow
    {
        public Guid ActivityId { get; init; }
        public int ActivityKind { get; init; }
        public int Effect { get; init; }
        public int SourceGroup { get; init; }
        public int Origin { get; init; }
        public int Status { get; init; }
        public DateOnly ActivityDate { get; init; }
        public decimal Amount { get; init; }
        public int Currency { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public Guid? CategoryId { get; init; }
        public string? CategoryName { get; init; }
        public Guid? SourceId { get; init; }
        public string? SourceName { get; init; }
        public Guid? DestinationId { get; init; }
        public string? DestinationName { get; init; }
        public DateTimeOffset? CancelledAtUtc { get; init; }

        /// <summary>
        /// Kaydın kapsamı; transfer ve kart ödemesinde <c>null</c>, çünkü ikisi
        /// de gelir/gider raporuna sıfır etki eder ve kapsam taşımaz.
        /// </summary>
        public int? Scope { get; init; }

        /// <summary>Borç taksidinin payları; diğer türlerde <c>null</c>.</summary>
        public decimal? PrincipalPortion { get; init; }
        public decimal? InterestPortion { get; init; }
        public Guid? MatchAccountId { get; init; }
        public Guid? MatchSecondAccountId { get; init; }
        public Guid? MatchCreditCardId { get; init; }
        public Guid? MatchCategoryId { get; init; }
    }
}
