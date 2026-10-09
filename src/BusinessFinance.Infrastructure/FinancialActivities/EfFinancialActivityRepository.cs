using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Accounts;
using BusinessFinance.Infrastructure.Counterparties;
using BusinessFinance.Infrastructure.CreditCards;
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
    : IFinancialActivityRepository, IActivityOriginReader, IActivityBalanceReader
{
    public async Task<IReadOnlyList<ActivityBalanceAfter>?> GetBalancesAfterAsync(
        Guid userId,
        FinancialActivityKind kind,
        Guid activityId,
        CancellationToken cancellationToken)
    {
        var located = await LocateAsync(userId, kind, activityId, cancellationToken);
        if (located is null)
        {
            return null;
        }

        // İptal edilmiş hareketin bakiye etkisi yoktur; giriş anı bilinmeyen
        // eski kaydın gün içindeki yeri bilinmez. İkisinde de sayı uydurulmaz.
        if (located.IsCancelled || located.EntryAtUtc is not DateTimeOffset entryAtUtc)
        {
            return [];
        }

        var cutoff = new EntryCutoff(located.Date, entryAtUtc, located.OwnExpenseId);
        var balances = new List<ActivityBalanceAfter>();
        // İkinci hesap yalnız transferin hedefidir: para ona girer.
        var accounts = new (Guid? Id, ActivityBalanceChange Change)[]
        {
            (located.AccountId, located.AccountIncreases switch
            {
                true => ActivityBalanceChange.Increased,
                false => ActivityBalanceChange.Decreased,
                null => ActivityBalanceChange.Unchanged
            }),
            (located.SecondAccountId, ActivityBalanceChange.Increased)
        };
        foreach (var (holderId, change) in accounts)
        {
            if (holderId is not Guid accountId)
            {
                continue;
            }

            var account = await dbContext.Accounts.AsNoTracking()
                .Where(item => item.UserId == userId && item.Id == accountId)
                .Select(item => new { item.Name, item.OpeningBalance, item.Currency })
                .SingleAsync(cancellationToken);
            var movements = await AccountMovements.SumAsync(
                dbContext, accountId, userId, cutoff, cancellationToken);
            balances.Add(new ActivityBalanceAfter(
                ActivityBalanceHolder.Account,
                accountId,
                account.Name,
                account.OpeningBalance + movements,
                account.Currency,
                change));
        }

        if (located.CreditCardId is Guid creditCardId)
        {
            var card = await dbContext.CreditCards.AsNoTracking()
                .Where(item => item.UserId == userId && item.Id == creditCardId)
                .Select(item => new { item.Name, item.Limit.Currency, Limit = item.Limit.Amount })
                .SingleAsync(cancellationToken);
            var debt = await CardDebt.SumAsync(
                dbContext, creditCardId, userId, cutoff, cancellationToken);
            balances.Add(new ActivityBalanceAfter(
                ActivityBalanceHolder.CreditCard,
                creditCardId,
                card.Name,
                debt,
                card.Currency,
                located.CardDebtIncreases
                    ? ActivityBalanceChange.Increased
                    : ActivityBalanceChange.Decreased,
                // Limitin geçmişi tutulmaz: bugünkü limitle hesaplanır.
                // Kartın kendi kuralı gibi sıfırın altına inmez.
                AvailableLimit: Math.Max(0m, card.Limit - debt)));
        }

        if (located.CounterpartyId is Guid counterpartyId)
        {
            var name = await dbContext.Counterparties.AsNoTracking()
                .Where(item => item.UserId == userId && item.Id == counterpartyId)
                .Select(item => item.Name)
                .SingleAsync(cancellationToken);
            // Artı: karşı taraf bize borçlu. Eksi: biz ona borçluyuz. Fazla
            // tahsilat kırpılmaz; taraf değiştirir.
            var net = await CounterpartyNet.SumAsync(
                dbContext, counterpartyId, userId, cutoff, cancellationToken);
            var side = SideOf(net);
            // Gösterilen tutar hep artıdır; "arttı" o tutarın büyüdüğünü söyler.
            var grew = side switch
            {
                ActivityBalanceSide.Receivable => located.CounterpartyNetDelta > 0m,
                ActivityBalanceSide.Payable => located.CounterpartyNetDelta < 0m,
                _ => false
            };
            balances.Add(new ActivityBalanceAfter(
                ActivityBalanceHolder.Counterparty,
                counterpartyId,
                name,
                Math.Abs(net),
                CurrencyCode.TRY,
                grew ? ActivityBalanceChange.Increased : ActivityBalanceChange.Decreased,
                Side: side,
                // Hareketin kendi etkisi geri alınınca önceki bakiye kalır.
                PreviousSide: SideOf(net - located.CounterpartyNetDelta)));
        }

        if (located.DebtAgreementId is Guid debtId)
        {
            var debt = await dbContext.DebtAgreements.AsNoTracking()
                .Where(item => item.UserId == userId && item.Id == debtId)
                .Select(item => new { item.Description, item.Direction, item.Principal.Currency })
                .SingleAsync(cancellationToken);
            // Kalan: bu hareketin anında henüz ödenmemiş taksitler. Giriş anı
            // bilinmeyen ödeme günün en eskisi sayılır (akışın sırası).
            var date = cutoff.Date;
            var entryAt = cutoff.EntryAtUtc;
            var remaining = await dbContext.DebtInstallments.AsNoTracking()
                .Where(item => item.UserId == userId &&
                               item.DebtAgreementId == debtId &&
                               (item.PaymentAccountId == null ||
                                item.PaymentDate > date ||
                                (item.PaymentDate == date &&
                                 item.PaidAtUtc != null &&
                                 item.PaidAtUtc > entryAt)))
                .SumAsync(item => item.Amount.Amount, cancellationToken);
            balances.Add(new ActivityBalanceAfter(
                ActivityBalanceHolder.Debt,
                debtId,
                debt.Description ?? string.Empty,
                remaining,
                debt.Currency,
                located.DebtIncreases
                    ? ActivityBalanceChange.Increased
                    : ActivityBalanceChange.Decreased,
                Side: remaining == 0m
                    ? ActivityBalanceSide.Settled
                    : debt.Direction == DebtDirection.Payable
                        ? ActivityBalanceSide.Payable
                        : ActivityBalanceSide.Receivable));
        }

        return balances;
    }

    private static ActivityBalanceSide SideOf(decimal net) =>
        net > 0m
            ? ActivityBalanceSide.Receivable
            : net < 0m ? ActivityBalanceSide.Payable : ActivityBalanceSide.Settled;

    /// <summary>
    /// Hareketi kendi tablosunda, sahiplik kapsamıyla bulur: günü, giriş anı
    /// ve dokunduğu hesaplar ile kart.
    /// </summary>
    /// <remarks>
    /// Birleşik sorgu üzerinden aranmaz: tek satır için on üç kaynağı
    /// birleştirmek gereksizdir. Veresiye ve yükümlülük hesap taşımaz ama
    /// karşı tarafın carisini değiştirir. POS satışı hesabını taşır ama ona
    /// dokunmaz: bakiye o an neyse odur, para yoldadır.
    /// </remarks>
    private async Task<LocatedActivity?> LocateAsync(
        Guid userId,
        FinancialActivityKind kind,
        Guid activityId,
        CancellationToken cancellationToken)
    {
        switch (kind)
        {
            case FinancialActivityKind.AccountTransaction:
                return await dbContext.Transactions.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.TransactionDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        null,
                        null)
                    {
                        AccountIncreases = item.Type == TransactionType.Income
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.Transfer:
                return await dbContext.Transfers.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.TransferDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        item.SourceAccountId,
                        item.DestinationAccountId,
                        null,
                        null)
                    {
                        AccountIncreases = false
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.CardCharge:
                return await dbContext.CreditCardCharges.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.ChargeDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        null,
                        null,
                        item.CreditCardId,
                        null)
                    {
                        CardDebtIncreases = true
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.CardPayment:
                return await dbContext.CreditCardPayments.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.PaymentDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        item.CreditCardId,
                        null)
                    {
                        AccountIncreases = false,
                        CardDebtIncreases = false
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.DebtPayment:
            case FinancialActivityKind.DebtCollection:
                var direction = kind == FinancialActivityKind.DebtPayment
                    ? DebtDirection.Payable
                    : DebtDirection.Receivable;
                return await (
                        from installment in dbContext.DebtInstallments.AsNoTracking()
                        join debt in dbContext.DebtAgreements.AsNoTracking()
                            on new { installment.UserId, Id = installment.DebtAgreementId }
                            equals new { debt.UserId, debt.Id }
                        where installment.UserId == userId &&
                              installment.Id == activityId &&
                              installment.PaymentAccountId != null &&
                              installment.PaymentDate != null &&
                              debt.Direction == direction
                        select new LocatedActivity(
                            installment.PaymentDate!.Value,
                            installment.PaidAtUtc,
                            false,
                            installment.PaymentAccountId,
                            null,
                            null,
                            null)
                        {
                            AccountIncreases = debt.Direction == DebtDirection.Receivable,
                            DebtAgreementId = debt.Id
                        })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.DebtOpening:
                // Nakit kaynaklı açılış bir hesaba dokunur; gider ya da gelir
                // kaynaklı açılışta hesap boştur ve bakiye dönmez.
                return await dbContext.DebtAgreements.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.StartDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        false,
                        item.SourceType == DebtSourceType.Cash ? item.OpeningAccountId : null,
                        null,
                        null,
                        null)
                    {
                        AccountIncreases = item.Direction == DebtDirection.Payable,
                        DebtAgreementId = item.Id,
                        DebtIncreases = true
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.CounterpartySettlement:
                return await dbContext.CounterpartyPayments.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.PaymentDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        null,
                        null)
                    {
                        // Kartla tahsil hesaba dokunmaz: para yoldadır, yatışla
                        // girer (POS satışı gibi "değişmedi").
                        AccountIncreases = item.PosSettlementId != null
                            ? null
                            : item.Direction == DebtDirection.Receivable,
                        CounterpartyId = item.CounterpartyId,
                        // Tahsilat alacağı, ödeme borcu kapatır.
                        CounterpartyNetDelta = item.Direction == DebtDirection.Payable
                            ? item.Amount.Amount
                            : -item.Amount.Amount
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.ObligationSettlement:
                return await dbContext.ObligationSettlements.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.SettlementDate,
                        item.SettledAtUtc,
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        null,
                        null)
                    {
                        // Kişiye bağlı yükümlülüğün kapanışı cari bakiyeyi
                        // değiştirmez; cari satırı dönmez.
                        AccountIncreases = item.PosSettlementId != null
                            ? null
                            : item.Direction == DebtDirection.Receivable
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.PosDeposit:
                // Kesinti gideri yatışla aynı yazmada doğar ama saati ondan bir
                // an sonradır; yatışın "sonrası" onu da içermelidir.
                return await dbContext.PosDeposits.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.DepositDate,
                        item.CreatedAtUtc,
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        null,
                        item.DeductionTransactionId)
                    {
                        AccountIncreases = true
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.CounterpartyCharge:
                // Borçlandırma tanır: hesaba dokunmaz, cariyi değiştirir.
                return await dbContext.CounterpartyCharges.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.ChargeDate,
                        EF.Property<DateTimeOffset?>(item, EntryTimestamp.PropertyName),
                        item.IsCancelled,
                        null,
                        null,
                        null,
                        null)
                    {
                        CounterpartyId = item.CounterpartyId,
                        CounterpartyNetDelta = item.Direction == DebtDirection.Receivable
                            ? item.Amount.Amount
                            : -item.Amount.Amount
                    })
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.Obligation:
                // Yükümlülük hesaba dokunmaz ve kişisi olsa da cari bakiyeye
                // girmez: gösterilecek bir bakiye yoktur.
                return await dbContext.Obligations.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.IssueDate,
                        item.CreatedAtUtc,
                        item.IsCancelled,
                        null,
                        null,
                        null,
                        null))
                    .SingleOrDefaultAsync(cancellationToken);

            case FinancialActivityKind.PosSale:
                return await dbContext.PosSettlements.AsNoTracking()
                    .Where(item => item.UserId == userId && item.Id == activityId)
                    .Select(item => new LocatedActivity(
                        item.SettlementDate,
                        item.CreatedAtUtc,
                        item.IsCancelled,
                        item.AccountId,
                        null,
                        null,
                        null))
                    .SingleOrDefaultAsync(cancellationToken);

            default:
                return null;
        }
    }

    /// <param name="Date">Hareketin günü.</param>
    /// <param name="EntryAtUtc">Yazıldığı an; eski kayıtta boş.</param>
    /// <param name="OwnExpenseId">Yatışın kesinti gideri.</param>
    private sealed record LocatedActivity(
        DateOnly Date,
        DateTimeOffset? EntryAtUtc,
        bool IsCancelled,
        Guid? AccountId,
        Guid? SecondAccountId,
        Guid? CreditCardId,
        Guid? OwnExpenseId)
    {
        /// <summary>
        /// Para hesaba girdiyse <c>true</c>, çıktıysa <c>false</c>; hareket
        /// hesaba dokunmadıysa (POS satışı) boş.
        /// </summary>
        public bool? AccountIncreases { get; init; }

        /// <summary>Kartın borcu arttıysa (harcama) <c>true</c>; ödemede azalır.</summary>
        public bool CardDebtIncreases { get; init; }

        /// <summary>Carisi değişen karşı taraf; yoksa boş.</summary>
        public Guid? CounterpartyId { get; init; }

        /// <summary>
        /// Hareketin net cariye (alacak − borç) etkisi: alacak yazılınca ya
        /// da borç ödenince artı, tahsilatta ya da borç yazılınca eksi.
        /// </summary>
        public decimal CounterpartyNetDelta { get; init; }

        /// <summary>Kalan tutarı değişen borç anlaşması; yoksa boş.</summary>
        public Guid? DebtAgreementId { get; init; }

        /// <summary>Borç açıldıysa <c>true</c>; taksitte kalan azalır.</summary>
        public bool DebtIncreases { get; init; }
    }

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

        if (await dbContext.RecurringTransactionOccurrences.AsNoTracking().AnyAsync(
                occurrence => occurrence.UserId == userId &&
                              occurrence.BudgetTransactionId == transactionId,
                cancellationToken))
        {
            return FinancialActivityOrigin.Recurring;
        }

        if (await dbContext.Transactions.AsNoTracking().AnyAsync(
                transaction => transaction.UserId == userId &&
                               transaction.Id == transactionId &&
                               transaction.DayCloseId != null,
                cancellationToken))
        {
            return FinancialActivityOrigin.DayClose;
        }

        // Geri alınmış yatışın gideri de yatışa aittir: kökeni değişmez,
        // zaten iptal edilmiştir.
        return await dbContext.PosDeposits.AsNoTracking().AnyAsync(
            deposit => deposit.UserId == userId &&
                       deposit.DeductionTransactionId == transactionId,
            cancellationToken)
            ? FinancialActivityOrigin.PosDeposit
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
            // Gün içinde giriş sırası, en yeni üstte. Giriş anı bilinmeyen
            // eski kayıtlar günün sonuna düşer (NULL en küçüktür) ve kendi
            // aralarında eski sıralarını korur.
            .ThenByDescending(row => row.EntryAtUtc)
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
                // Yatışın kesinti gideri ayrı satır değildir: yatış satırının
                // parçasıdır ve orada gösterilir. Gider olarak raporda sayılır.
            where transaction.UserId == userId &&
                  !dbContext.PosDeposits.Any(deposit =>
                      deposit.UserId == userId &&
                      deposit.DeductionTransactionId == transaction.Id)
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
                        : transaction.DayCloseId != null
                            ? (int)FinancialActivityOrigin.DayClose
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(transaction, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                // Gün sonunun yazdığı ya da saydığı kayıt.
                CancelLocked = false,
                DayCloseId = transaction.DayCloseId ?? dbContext.DayCloseCountedRecords
                        .Where(counted => counted.UserId == userId &&
                                          counted.Kind == DayCloseRecordKind.Income &&
                                          counted.RecordId == transaction.Id)
                        .Select(counted => (Guid?)counted.DayCloseId)
                        .FirstOrDefault(),
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
                // Açıklama yoksa başlık boştur: istemci türün adını yazar.
                // Hedef hesabın adı alt satırdaki "kaynak → hedef"te zaten var.
                Title = transfer.Description ?? string.Empty,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(transfer, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                Title = payment.Description ?? string.Empty,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                EntryAtUtc = installment.PaidAtUtc,
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(debt, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(charge, EntryTimestamp.PropertyName),
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)charge.Direction,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = (Guid?)null,
                MatchAccountId = null,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = counterparty.Id
            };

        // Tahsilat/ödeme taşır: kasayı değiştirir, gelir/gider üretmez ve bu
        // yüzden kart ödemesi gibi kapsamsızdır. Kartla tahsil (ADR 0019 T5)
        // ayrı satır değildir: POS kaydının komisyonu, neti ve günleri bu
        // satırın parçasıdır, tıpkı POS satışında olduğu gibi.
        var counterpartySettlements =
            from payment in dbContext.CounterpartyPayments.AsNoTracking()
            join counterparty in dbContext.Counterparties.AsNoTracking()
                on new { payment.UserId, Id = payment.CounterpartyId }
                equals new { counterparty.UserId, counterparty.Id }
            join account in dbContext.Accounts.AsNoTracking()
                on new { payment.UserId, Id = payment.AccountId }
                equals new { account.UserId, account.Id }
            join card in dbContext.PosSettlements.AsNoTracking()
                on new { payment.UserId, Id = payment.PosSettlementId }
                equals new { card.UserId, Id = (Guid?)card.Id }
                into cards
            from card in cards.DefaultIfEmpty()
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { UserId = (Guid?)card.UserId, Id = card.PosDefinitionId }
                equals new { UserId = (Guid?)definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
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
                EntryAtUtc = EF.Property<DateTimeOffset?>(payment, EntryTimestamp.PropertyName),
                ChannelName = definition == null ? null : definition.Name,
                FeeAmount = card != null && card.CommissionAmount > 0m
                    ? (decimal?)card.CommissionAmount
                    : null,
                NetAmount = card == null
                    ? null
                    : (decimal?)(card.GrossAmount.Amount - card.CommissionAmount),
                ExpectedTransferDate = card == null ? null : (DateOnly?)card.ExpectedTransferDate,
                TransferredOn = card == null ? null : card.TransferredOn,
                Direction = (int?)payment.Direction,
                SettlementCount = (int?)null,
                // Nakit tahsilat kendi kaydıyla, kartla tahsil POS kaydıyla
                // sayılır (gün sonunun kart tarafı).
                CancelLocked = false,
                DayCloseId = dbContext.DayCloseCountedRecords
                        .Where(counted => counted.UserId == userId &&
                                          ((counted.Kind == DayCloseRecordKind.CounterpartyPayment &&
                                            counted.RecordId == payment.Id) ||
                                           (counted.Kind == DayCloseRecordKind.PosSettlement &&
                                            (Guid?)counted.RecordId == payment.PosSettlementId)))
                        .Select(counted => (Guid?)counted.DayCloseId)
                        .FirstOrDefault(),
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
                EntryAtUtc = (DateTimeOffset?)obligation.CreatedAtUtc,
                ChannelName = (string?)null,
                FeeAmount = (decimal?)null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)obligation.Direction,
                SettlementCount = (int?)null,
                // Kapanışı bir gün sonunda sayılmış ya da kartla tahsilin parası
                // bir yatışla hesaba geçmiş yükümlülük iptal edilemez; iptal ucu
                // aynı iki kuralı uygular. Satır bunu bilmezse düğme görünür ve
                // ret ancak dokununca gelir.
                CancelLocked = dbContext.ObligationSettlements.Any(closing =>
                    closing.UserId == userId &&
                    closing.ObligationId == obligation.Id &&
                    !closing.IsCancelled &&
                    (dbContext.DayCloseCountedRecords.Any(counted =>
                         counted.UserId == userId &&
                         ((counted.Kind == DayCloseRecordKind.ObligationSettlement &&
                           counted.RecordId == closing.Id) ||
                          (counted.Kind == DayCloseRecordKind.PosSettlement &&
                           (Guid?)counted.RecordId == closing.PosSettlementId))) ||
                     dbContext.PosSettlements.Any(pos =>
                         pos.UserId == userId &&
                         (Guid?)pos.Id == closing.PosSettlementId &&
                         !pos.IsCancelled &&
                         pos.PosDepositId != null))),
                DayCloseId = (Guid?)null,
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
            join card in dbContext.PosSettlements.AsNoTracking()
                on new { settlement.UserId, Id = settlement.PosSettlementId }
                equals new { card.UserId, Id = (Guid?)card.Id }
                into cards
            from card in cards.DefaultIfEmpty()
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { UserId = (Guid?)card.UserId, Id = card.PosDefinitionId }
                equals new { UserId = (Guid?)definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
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
                // Kaynak hep hesap, hedef hep karşı taraf — cari tahsilat ve
                // ödemeyle aynı. Paranın hangi yöne aktığını `Direction` söyler;
                // ad yön değiştirince istemci hesabı karşı taraf sanıyordu.
                SourceId = (Guid?)account.Id,
                SourceName = account.Name,
                DestinationId = counterparty == null ? null : (Guid?)counterparty.Id,
                DestinationName = counterparty == null ? null : counterparty.Name,
                CancelledAtUtc = settlement.CancelledAtUtc,
                Scope = null,
                PrincipalPortion = null,
                InterestPortion = null,
                EntryAtUtc = (DateTimeOffset?)settlement.SettledAtUtc,
                ChannelName = definition == null ? null : definition.Name,
                FeeAmount = card != null && card.CommissionAmount > 0m
                    ? (decimal?)card.CommissionAmount
                    : null,
                NetAmount = card == null
                    ? null
                    : (decimal?)(card.GrossAmount.Amount - card.CommissionAmount),
                ExpectedTransferDate = card == null ? null : (DateOnly?)card.ExpectedTransferDate,
                TransferredOn = card == null ? null : card.TransferredOn,
                Direction = (int?)settlement.Direction,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = dbContext.DayCloseCountedRecords
                        .Where(counted => counted.UserId == userId &&
                                          ((counted.Kind == DayCloseRecordKind.ObligationSettlement &&
                                            counted.RecordId == settlement.Id) ||
                                           (counted.Kind == DayCloseRecordKind.PosSettlement &&
                                            (Guid?)counted.RecordId == settlement.PosSettlementId)))
                        .Select(counted => (Guid?)counted.DayCloseId)
                        .FirstOrDefault(),
                MatchAccountId = account.Id,
                MatchSecondAccountId = null,
                MatchCreditCardId = null,
                MatchCategoryId = null,
                MatchCounterpartyId = obligation.CounterpartyId
            };

        // POS tahsilatı feed'de tek satırdır: satışın tanındığı gün, brüt
        // tutarla. Komisyon aynı gün tanınan bir giderdir ama ayrı satır
        // değildir; satışın parçası olarak taşınır (`FeeAmount`). Paranın
        // hesaba geçişi tahsilatın değil yatışın satırıdır.
        // Kartla tahsil bu satırlara girmez: tahsilatın kendi satırının
        // parçasıdır ve gelir yazmaz.
        var posSales =
            from settlement in dbContext.PosSettlements.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { settlement.UserId, Id = settlement.AccountId }
                equals new { account.UserId, account.Id }
            join category in dbContext.Categories.AsNoTracking()
                on new { settlement.UserId, Id = settlement.CategoryId }
                equals new { category.UserId, Id = (Guid?)category.Id }
            join definition in dbContext.PosDefinitions.AsNoTracking()
                on new { settlement.UserId, Id = settlement.PosDefinitionId }
                equals new { definition.UserId, Id = (Guid?)definition.Id }
                into definitions
            from definition in definitions.DefaultIfEmpty()
            where settlement.UserId == userId && settlement.Kind == PosSettlementKind.Sale
            select new ActivityRow
            {
                ActivityId = settlement.Id,
                ActivityKind = (int)FinancialActivityKind.PosSale,
                Effect = (int)FinancialActivityEffect.Income,
                SourceGroup = (int)FinancialActivitySourceGroup.Pos,
                Origin = settlement.DayCloseId != null
                    ? (int)FinancialActivityOrigin.DayClose
                    : (int)FinancialActivityOrigin.Manual,
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
                EntryAtUtc = (DateTimeOffset?)settlement.CreatedAtUtc,
                ChannelName = definition == null ? null : definition.Name,
                FeeAmount = settlement.CommissionAmount > 0m
                    ? (decimal?)settlement.CommissionAmount
                    : null,
                NetAmount = (decimal?)(settlement.GrossAmount.Amount - settlement.CommissionAmount),
                ExpectedTransferDate = (DateOnly?)settlement.ExpectedTransferDate,
                TransferredOn = settlement.TransferredOn,
                Direction = (int?)null,
                SettlementCount = (int?)null,
                CancelLocked = false,
                DayCloseId = settlement.DayCloseId ?? dbContext.DayCloseCountedRecords
                        .Where(counted => counted.UserId == userId &&
                                          counted.Kind == DayCloseRecordKind.PosSettlement &&
                                          counted.RecordId == settlement.Id)
                        .Select(counted => (Guid?)counted.DayCloseId)
                        .FirstOrDefault(),
                MatchAccountId = account.Id,
                MatchSecondAccountId = (Guid?)null,
                MatchCreditCardId = (Guid?)null,
                MatchCategoryId = category.Id,
                MatchCounterpartyId = (Guid?)null
            };

        // Yatış tek para hareketidir ve tek satırdır: kaç tahsilatı kapatırsa
        // kapatsın bankanın hesaba yatırdığı tutarı gösterir (ADR 0019 T5).
        // Beklenenden eksik kalan kısım (kesinti) bu satırın parçasıdır
        // (`FeeAmount`); kesinti gideri akışta ayrı satır olmaz.
        var posDeposits =
            from deposit in dbContext.PosDeposits.AsNoTracking()
            join account in dbContext.Accounts.AsNoTracking()
                on new { deposit.UserId, Id = deposit.AccountId }
                equals new { account.UserId, account.Id }
            where deposit.UserId == userId
            select new ActivityRow
            {
                ActivityId = deposit.Id,
                ActivityKind = (int)FinancialActivityKind.PosDeposit,
                Effect = (int)FinancialActivityEffect.Neutral,
                SourceGroup = (int)FinancialActivitySourceGroup.Pos,
                Origin = (int)FinancialActivityOrigin.Manual,
                Status = deposit.IsCancelled
                    ? (int)FinancialActivityStatus.Cancelled
                    : (int)FinancialActivityStatus.Realized,
                ActivityDate = deposit.DepositDate,
                Amount = deposit.DepositedAmount.Amount,
                Currency = (int)deposit.DepositedAmount.Currency,
                Title = string.Empty,
                Description = (string?)null,
                // Yatış parayı taşır: gelir/gider üretmediği için ne kategori
                // ne kapsam taşır (ADR 0014). Kapsam filtresi verildiğinde bu
                // satır düşer, tıpkı transfer ve kart ödemesi gibi.
                CategoryId = (Guid?)null,
                CategoryName = (string?)null,
                SourceId = (Guid?)null,
                SourceName = (string?)null,
                DestinationId = account.Id,
                DestinationName = account.Name,
                CancelledAtUtc = deposit.CancelledAtUtc,
                Scope = (int?)null,
                PrincipalPortion = (decimal?)null,
                InterestPortion = (decimal?)null,
                EntryAtUtc = (DateTimeOffset?)deposit.CreatedAtUtc,
                ChannelName = dbContext.PosSettlements
                        .Where(closed => closed.UserId == userId &&
                                         closed.PosDepositId == deposit.Id)
                        .Select(closed => closed.PosDefinitionId)
                        .Distinct()
                        .Count() == 1
                    ? (from closed in dbContext.PosSettlements
                       join definition in dbContext.PosDefinitions
                           on new { closed.UserId, Id = closed.PosDefinitionId }
                           equals new { definition.UserId, Id = (Guid?)definition.Id }
                       where closed.UserId == userId && closed.PosDepositId == deposit.Id
                       select definition.Name).Min()
                    : null,
                FeeAmount = deposit.DeductionAmount > 0m
                    ? (decimal?)deposit.DeductionAmount
                    : null,
                NetAmount = (decimal?)null,
                ExpectedTransferDate = (DateOnly?)null,
                TransferredOn = (DateOnly?)null,
                Direction = (int?)null,
                SettlementCount = (int?)dbContext.PosSettlements.Count(closed =>
                    closed.UserId == userId && closed.PosDepositId == deposit.Id),
                CancelLocked = false,
                DayCloseId = (Guid?)null,
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
            .Concat(posDeposits);
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
                (row.DestinationName != null && row.DestinationName.Contains(search)) ||
                (row.ChannelName != null && row.ChannelName.Contains(search)));
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
        row.InterestPortion,
        row.ChannelName,
        row.FeeAmount,
        row.NetAmount,
        row.ExpectedTransferDate,
        row.TransferredOn,
        row.SettlementCount,
        row.Direction is int direction ? (DebtDirection)direction : null,
        row.DayCloseId,
        row.CancelLocked);

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

        /// <summary>
        /// Kaydın yazıldığı an; gün içi sırayı belirler. Eski kayıtlarda
        /// <c>null</c>.
        /// </summary>
        public DateTimeOffset? EntryAtUtc { get; init; }

        /// <summary>POS satışı ve yatışının alanları; diğer türlerde <c>null</c>.</summary>
        public string? ChannelName { get; init; }
        public decimal? FeeAmount { get; init; }
        public decimal? NetAmount { get; init; }
        public DateOnly? ExpectedTransferDate { get; init; }
        public DateOnly? TransferredOn { get; init; }
        public int? SettlementCount { get; init; }
        public int? Direction { get; init; }

        /// <summary>Kaydı yazan ya da sayan gün sonu; yoksa <c>null</c>.</summary>
        public Guid? DayCloseId { get; init; }
        public bool CancelLocked { get; init; }
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
