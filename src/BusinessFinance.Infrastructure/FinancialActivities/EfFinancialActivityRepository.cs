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
    /// The write models, each projected to the identical anonymous shape that
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
                MatchCategoryId = category.Id,
                MatchCounterpartyId = null
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
                MatchCategoryId = null,
                MatchCounterpartyId = null
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
                MatchCategoryId = category.Id,
                MatchCounterpartyId = null
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
                MatchCategoryId = null,
                MatchCounterpartyId = null
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
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { debt.UserId, Id = debt.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
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
                Title = debt.Description ?? counterparty.Name,
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
                MatchCategoryId = null,
                MatchCounterpartyId = debt.CounterpartyId
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
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { debt.UserId, Id = debt.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
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
                Title = counterparty.Name,
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
                MatchCategoryId = null,
                MatchCounterpartyId = debt.CounterpartyId
            };

        var debtCategoricalOpenings =
            from debt in dbContext.DebtAgreements.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking()
                on new { debt.UserId, Id = debt.CategoryId!.Value }
                equals new { category.UserId, category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { debt.UserId, Id = debt.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
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
                Title = counterparty.Name,
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
                MatchCategoryId = category.Id,
                MatchCounterpartyId = debt.CounterpartyId
            };

        // Açık cari, ADR 0014'ün iki yüzü. Borçlandırma tanır: yön kategorinin
        // türünü belirlediği için etki doğrudan yönden okunur — alacak doğuran
        // satış gelirdir, borç doğuran alım giderdir.
        var counterpartyCharges =
            from charge in dbContext.CounterpartyCharges.AsNoTracking()
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { charge.UserId, Id = charge.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { charge.UserId, Id = charge.CategoryId }
                equals new { category.UserId, category.Id }
            where charge.UserId == userId
            select new ActivityRow
            {
                ActivityId = charge.Id,
                ActivityKind = (int)FinancialActivityKind.CounterpartyCharge,
                Effect = charge.Direction == DebtDirection.Receivable
                    ? (int)FinancialActivityEffect.Income
                    : (int)FinancialActivityEffect.Expense,
                SourceGroup = (int)FinancialActivitySourceGroup.Counterparty,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = charge.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = charge.ChargeDate,
                Amount = charge.Amount.Amount,
                Currency = (int)charge.Amount.Currency,

                // Kaydın adı kullanıcının yazdığından gelir; yoksa karşı tarafın
                // adı kategoriden daha çok şey söyler: "Ahmet Bakkal" bir satırı
                // "Mal alımı"ndan iyi ayırır.
                Title = charge.Description ?? counterparty.Name,
                Description = charge.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = counterparty.Id,
                SourceName = counterparty.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = charge.CancelledAtUtc,
                Scope = (int?)charge.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,

                // Hesap eşleşmesi yok: borçlandırma hiçbir kasadan geçmez.
                MatchAccountId = null,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = counterparty.Id
            };

        // Tahsilat/ödeme taşır: kasayı değiştirir, gelir/gider üretmez ve bu
        // yüzden kart ödemesi gibi kapsamsızdır.
        var counterpartySettlements =
            from payment in dbContext.CounterpartyPayments.AsNoTracking()
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { payment.UserId, Id = payment.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
            join account in dbContext.Accounts.AsNoTracking()
                on new { payment.UserId, Id = payment.AccountId }
                equals new { account.UserId, account.Id }
            where payment.UserId == userId
            select new ActivityRow
            {
                ActivityId = payment.Id,
                ActivityKind = (int)FinancialActivityKind.CounterpartySettlement,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Counterparty,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = payment.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = payment.PaymentDate,
                Amount = payment.Amount.Amount,
                Currency = (int)payment.Amount.Currency,
                Title = payment.Description ?? counterparty.Name,
                Description = payment.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = account.Id,
                SourceName = account.Name,
                DestinationId = counterparty.Id,
                DestinationName = counterparty.Name,
                CancelledAtUtc = payment.CancelledAtUtc,
                Scope = (int?)null,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = null,
                MatchCounterpartyId = counterparty.Id
            };

        var obligations =
            from obligation in dbContext.Obligations.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CategoryId }
                equals new { category.UserId, category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            where obligation.UserId == userId
            select new ActivityRow
            {
                ActivityId = obligation.Id,
                ActivityKind = (int)FinancialActivityKind.Obligation,
                Effect = obligation.Direction == DebtDirection.Receivable
                    ? (int)FinancialActivityEffect.Income
                    : (int)FinancialActivityEffect.Expense,
                SourceGroup = (int)FinancialActivitySourceGroup.Obligation,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = obligation.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = obligation.IssueDate,
                Amount = obligation.Amount.Amount,
                Currency = (int)obligation.Amount.Currency,
                Title = obligation.Description ??
                        (counterparty == null ? category.Name : counterparty.Name),
                Description = obligation.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = counterparty == null ? null : counterparty.Id,
                SourceName = counterparty == null ? null : counterparty.Name,
                DestinationId = null,
                DestinationName = null,
                CancelledAtUtc = obligation.CancelledAtUtc,
                Scope = (int?)obligation.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = null,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = obligation.CounterpartyId
            };

        var obligationSettlements =
            from settlement in dbContext.ObligationSettlements.AsNoTracking()
            join obligation in dbContext.Obligations.AsNoTracking()
                on new { settlement.UserId, Id = settlement.ObligationId }
                equals new { obligation.UserId, obligation.Id }
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CategoryId }
                equals new { category.UserId, category.Id }
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { obligation.UserId, Id = obligation.CounterpartyId }
                equals new { counterparty.UserId, Id = (Guid?)counterparty.Id }
                into counterparties
            from counterparty in counterparties.DefaultIfEmpty()
            where settlement.UserId == userId
            select new ActivityRow
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.ObligationSettlement,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Obligation,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = settlement.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = settlement.SettlementDate,
                Amount = settlement.Amount.Amount,
                Currency = (int)settlement.Amount.Currency,
                Title = obligation.Description ??
                        (counterparty == null ? category.Name : counterparty.Name),
                Description = obligation.Description,
                CategoryId = null,
                CategoryName = null,
                SourceId = settlement.Direction == DebtDirection.Payable
                    ? account.Id
                    : counterparty == null ? null : counterparty.Id,
                SourceName = settlement.Direction == DebtDirection.Payable
                    ? account.Name
                    : counterparty == null ? null : counterparty.Name,
                DestinationId = settlement.Direction == DebtDirection.Receivable
                    ? account.Id
                    : counterparty == null ? null : counterparty.Id,
                DestinationName = settlement.Direction == DebtDirection.Receivable
                    ? account.Name
                    : counterparty == null ? null : counterparty.Name,
                CancelledAtUtc = settlement.CancelledAtUtc,
                Scope = null,
                PrincipalPortion = null,
                InterestPortion = null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = null,
                MatchCounterpartyId = obligation.CounterpartyId
            };

        // POS tahsilatı tek kayıttır ama feed'de üç satırdır, çünkü üç ayrı
        // ekonomik an taşır (ADR 0014): satışın tanındığı gün gelir, aynı gün
        // komisyon gideri, geçiş günü ise gelir/gider üretmeyen para hareketi.
        // Üçü de aynı kaydın kimliğini taşır; istemci satırı `tür + kimlik`
        // ikilisiyle anahtarlar, bu yüzden çakışmazlar.
        var posSales =
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CategoryId }
                equals new { category.UserId, category.Id }
            where settlement.UserId == userId
            select new ActivityRow
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.PosSale,
                Effect = (int)FinancialActivityEffect.Income,
                SourceGroup = (int)FinancialActivitySourceGroup.Pos,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = settlement.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = settlement.SettlementDate,
                // Gelir brüt tutar kadar tanınır; komisyon ondan düşülmez.
                Amount = settlement.GrossAmount.Amount,
                Currency = (int)settlement.GrossAmount.Currency,
                Title = settlement.Description ?? category.Name,
                Description = settlement.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                // Para henüz bu hesapta değil; hesap satışın nereye geçeceğini
                // söyleyen hedeftir, çıktığı kaynak değil.
                SourceId = (Guid?)null,
                SourceName = (string?)null,
                DestinationId = account.Id,
                DestinationName = account.Name,
                CancelledAtUtc = settlement.CancelledAtUtc,
                Scope = (int?)settlement.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = (Guid?)null,
                MatchCreditCardId = (Guid?)null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = (Guid?)null
            };

        // Komisyonsuz tahsilatın komisyon satırı da yoktur: sıfır tutarlı bir
        // gider yazmak, olmamış bir gideri kayda geçirmek olurdu.
        var posCommissions =
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CommissionCategoryId }
                equals new { category.UserId, Id = (Guid?)category.Id }
            where settlement.UserId == userId && settlement.CommissionAmount > 0m
            select new ActivityRow
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.PosCommission,
                Effect = (int)FinancialActivityEffect.Expense,
                SourceGroup = (int)FinancialActivitySourceGroup.Pos,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = settlement.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = settlement.SettlementDate,
                Amount = settlement.CommissionAmount,
                Currency = (int)settlement.GrossAmount.Currency,
                Title = category.Name,
                Description = settlement.Description,
                CategoryId = category.Id,
                CategoryName = category.Name,
                SourceId = (Guid?)null,
                SourceName = (string?)null,
                DestinationId = account.Id,
                DestinationName = account.Name,
                CancelledAtUtc = settlement.CancelledAtUtc,
                Scope = (int?)settlement.Scope,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = (Guid?)null,
                MatchCreditCardId = (Guid?)null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = (Guid?)null
            };

        var posTransfers =
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            where settlement.UserId == userId && settlement.TransferredOn != null
            select new ActivityRow
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.PosTransfer,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Pos,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = settlement.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = settlement.TransferredOn!.Value,
                // Hesaba giren net tutar; kalıcı kolon değil, brütten komisyon
                // düşülerek okunuyor.
                Amount = settlement.GrossAmount.Amount - settlement.CommissionAmount,
                Currency = (int)settlement.GrossAmount.Currency,
                Title = settlement.Description ?? account.Name,
                Description = settlement.Description,
                // Geçiş parayı taşır: gelir/gider üretmediği için ne kategori
                // ne kapsam taşır (ADR 0014). Kapsam filtresi verildiğinde bu
                // satır düşer, tıpkı transfer ve kart ödemesi gibi.
                CategoryId = (Guid?)null,
                CategoryName = (string?)null,
                SourceId = (Guid?)null,
                SourceName = (string?)null,
                DestinationId = account.Id,
                DestinationName = account.Name,
                CancelledAtUtc = settlement.CancelledAtUtc,
                Scope = (int?)null,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                MatchAccountId = account.Id,
                MatchSecondAccountId = (Guid?)null,
                MatchCreditCardId = (Guid?)null,
                MatchCategoryId = (Guid?)null,
                MatchCounterpartyId = (Guid?)null
            };

        return accountTransactions
            .Concat(transfers)
            .Concat(cardCharges)
            .Concat(cardPayments)
            .Concat(debtActivities)
            .Concat(debtCashOpenings)
            .Concat(debtCategoricalOpenings)
            .Concat(counterpartyCharges)
            .Concat(counterpartySettlements)
            .Concat(obligations)
            .Concat(obligationSettlements)
            .Concat(posSales)
            .Concat(posCommissions)
            .Concat(posTransfers);
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

        if (criteria.CounterpartyId is Guid counterpartyId)
        {
            query = query.Where(row => row.MatchCounterpartyId == counterpartyId);
        }

        if (criteria.CategoryId is Guid categoryId)
        {
            query = query.Where(row => row.MatchCategoryId == categoryId);
        }

        // Arama birleşik projeksiyonun görünen metinlerinde yapılır; `Contains`
        // SQL'de kaçışlı `LIKE` olur ve harf duyarlılığı veritabanı
        // harmanlamasından gelir (varsayılan büyük/küçük harfe duyarsız).
        if (criteria.Search is { Length: > 0 } search)
        {
            query = query.Where(row =>
                row.Title.Contains(search) ||
                (row.Description != null && row.Description.Contains(search)) ||
                (row.CategoryName != null && row.CategoryName.Contains(search)) ||
                (row.SourceName != null && row.SourceName.Contains(search)) ||
                (row.DestinationName != null && row.DestinationName.Contains(search)));
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

        /// <summary>
        /// Satırın hangi kişiyle ilgili olduğu: cari hareketlerde karşı
        /// tarafın kendisi, borç satırlarında sözleşmenin karşı tarafı.
        /// Diğer türlerde <c>null</c>.
        /// </summary>
        public Guid? MatchCounterpartyId { get; init; }
        public Guid? MatchSecondAccountId { get; init; }
        public Guid? MatchCreditCardId { get; init; }
        public Guid? MatchCategoryId { get; init; }
    }
}
