using Microsoft.EntityFrameworkCore;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// Yükümlülük ile borç planının tek borç modelinde birleşmesi (ADR 0021 dalga 1),
/// dolu bir veritabanında.
/// </summary>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Yükseltme yolu: eski dört tablodaki her kayıt kimliğiyle yeni dört tabloya
    /// geçer. Yükümlülük borç olur ve tek kalemi onun kimliğini taşır; kapanış
    /// ödeme olur, POS bağı ve iptali korunur; ödenmiş taksit kendi kimliğiyle
    /// ödeme olur. Gün sonunun saydığı kayıtlar kimlikle bağlı olduğu için
    /// yerinde kalır.
    /// </summary>
    [SqlServerFact]
    public async Task MergeObligationsAndDebtAgreements_MovesEveryRecordWithItsIdentity()
    {
        await using var database = await SqlTestDatabase.CreateAsync(
            GetConnectionString(), "AddCategoryRolesAndTransferFees");
        var user = CreateUser("debt-merge-upgrade@example.test");
        await database.SeedUsersAsync(user);

        var createdAt = new DateTimeOffset(2026, 8, 10, 9, 0, 0, TimeSpan.Zero);
        var bank = new Account(Guid.NewGuid(), user.Id, "Banka", AccountType.Bank, CurrencyCode.TRY);
        var bills = new Category(Guid.NewGuid(), user.Id, "Fatura", CategoryType.Expense);
        var sales = new Category(Guid.NewGuid(), user.Id, "Satış", CategoryType.Income);
        var supplier = new Counterparty(Guid.NewGuid(), user.Id, "Toptancı");
        var card = PosSettlement.Collect(
            Guid.NewGuid(), user.Id, bank, new Money(90m, CurrencyCode.TRY), 0m,
            null, new DateOnly(2026, 8, 12), new DateOnly(2026, 8, 14), createdAt.AddDays(2));
        var dayClose = DayClose.Record(
            Guid.NewGuid(), user.Id, new DateOnly(2026, 8, 12), createdAt.AddDays(2), [], []);
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(bank, bills, sales, card, dayClose);
            await seed.SaveChangesAsync();
        }

        // Kişi o şemanın kolonlarıyla yazılır: sonraki migration'ların eklediği
        // kolonlar (devreden bakiye) bu noktada yoktur.
        await database.ExecuteAsync(
            "INSERT INTO [Counterparties] ([Id], [UserId], [Name], [NameKey], [Note], [IsActive]) " +
            "VALUES ({0}, {1}, {2}, {3}, NULL, 1)",
            supplier.Id, user.Id, supplier.Name, supplier.NameKey);

        var open = Guid.NewGuid();
        var settled = Guid.NewGuid();
        var byCard = Guid.NewGuid();
        var cancelled = Guid.NewGuid();
        var settlement = Guid.NewGuid();
        var cardSettlement = Guid.NewGuid();
        var cancelledSettlement = Guid.NewGuid();
        var cancelledAt = new DateTimeOffset(2026, 8, 13, 9, 0, 0, TimeSpan.Zero);

        async Task ObligationAsync(
            Guid id, DebtDirection direction, Guid categoryId, Guid? counterpartyId,
            decimal amount, bool isCancelled)
        {
            await database.ExecuteAsync(
                "INSERT INTO [Obligations] ([Id], [UserId], [CounterpartyId], [CategoryId], [Direction], " +
                "[Amount], [Currency], [Scope], [IssueDate], [DueDate], [Description], [CreatedAtUtc], " +
                "[IsCancelled], [CancelledAtUtc]) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, 1, '2026-08-10', '2026-08-20', N'Fatura', {6}, {7}, {8})",
                id, user.Id, counterpartyId, categoryId, (byte)direction, amount, createdAt,
                isCancelled, isCancelled ? cancelledAt : null);
        }

        async Task SettlementAsync(
            Guid id, Guid obligationId, DebtDirection direction, decimal amount, Guid? posId, bool isCancelled)
        {
            await database.ExecuteAsync(
                "INSERT INTO [ObligationSettlements] ([Id], [UserId], [ObligationId], [AccountId], [Direction], " +
                "[Amount], [Currency], [SettlementDate], [SettledAtUtc], [PosSettlementId], [IsCancelled], " +
                "[CancelledAtUtc]) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 1, '2026-08-12', {6}, {7}, {8}, {9})",
                id, user.Id, obligationId, bank.Id, (byte)direction, amount,
                createdAt.AddDays(2), posId, isCancelled, isCancelled ? cancelledAt : null);
        }

        await ObligationAsync(open, DebtDirection.Payable, bills.Id, supplier.Id, 250m, false);
        await ObligationAsync(settled, DebtDirection.Payable, bills.Id, null, 120m, false);
        await ObligationAsync(byCard, DebtDirection.Receivable, sales.Id, null, 90m, false);
        await ObligationAsync(cancelled, DebtDirection.Payable, bills.Id, null, 75m, true);
        await SettlementAsync(settlement, settled, DebtDirection.Payable, 120m, null, false);
        await SettlementAsync(cardSettlement, byCard, DebtDirection.Receivable, 90m, card.Id, false);
        await SettlementAsync(cancelledSettlement, cancelled, DebtDirection.Payable, 75m, null, true);

        // Taksitli borç: üç taksit, ilki ödenmiş. Açılışı bilinmeyen eski borç:
        // kaynağı ve ayrımı boş.
        var loan = Guid.NewGuid();
        var legacy = Guid.NewGuid();
        var loanItems = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var legacyItem = Guid.NewGuid();
        await database.ExecuteAsync(
            "INSERT INTO [DebtAgreements] ([Id], [UserId], [CounterpartyId], [Direction], [Scope], [Principal], " +
            "[Currency], [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType], " +
            "[OpeningAccountId], [CategoryId], [StartDate], [FirstDueDate], [InstallmentCount], " +
            "[Description], [CreatedAtUtc]) " +
            "VALUES ({0}, {1}, {2}, 1, 1, 300, 'TRY', 300, 'TRY', 0, 1, {3}, NULL, " +
            "'2026-08-01', '2026-08-15', 3, N'Kredi', {4})",
            loan, user.Id, supplier.Id, bank.Id, createdAt);
        await database.ExecuteAsync(
            "INSERT INTO [DebtAgreements] ([Id], [UserId], [CounterpartyId], [Direction], [Scope], [Principal], " +
            "[Currency], [TotalRepayment], [TotalCurrency], [AnnualInterestRate], [SourceType], " +
            "[OpeningAccountId], [CategoryId], [StartDate], [FirstDueDate], [InstallmentCount], " +
            "[Description], [CreatedAtUtc]) " +
            "VALUES ({0}, {1}, {2}, 2, 2, 50, 'TRY', 50, 'TRY', 0, 0, NULL, NULL, " +
            "'2026-07-01', '2026-07-15', 1, NULL, NULL)",
            legacy, user.Id, supplier.Id);
        for (var index = 0; index < loanItems.Length; index++)
        {
            var paid = index == 0;
            await database.ExecuteAsync(
                "INSERT INTO [DebtInstallments] ([Id], [UserId], [DebtAgreementId], [Sequence], [Amount], " +
                "[Currency], [PrincipalPortion], [InterestPortion], [DueDate], [PaymentAccountId], " +
                "[PaymentDate], [PaidAtUtc]) " +
                "VALUES ({0}, {1}, {2}, {3}, 100, 'TRY', 100, 0, {4}, {5}, {6}, {7})",
                loanItems[index], user.Id, loan, index + 1, new DateOnly(2026, 8 + index, 15),
                paid ? bank.Id : null, paid ? new DateOnly(2026, 8, 15) : null,
                paid ? createdAt.AddDays(5) : null);
        }

        await database.ExecuteAsync(
            "INSERT INTO [DebtInstallments] ([Id], [UserId], [DebtAgreementId], [Sequence], [Amount], " +
            "[Currency], [PrincipalPortion], [InterestPortion], [DueDate], [PaymentAccountId], " +
            "[PaymentDate], [PaidAtUtc]) " +
            "VALUES ({0}, {1}, {2}, 1, 50, 'TRY', NULL, NULL, '2026-07-15', NULL, NULL, NULL)",
            legacyItem, user.Id, legacy);

        // Gün sonu kapanışı ve faturayı saymıştı; bağ kimlikle durur, FK'si yok.
        await database.ExecuteAsync(
            "INSERT INTO [DayCloseCountedRecords] ([DayCloseId], [UserId], [Kind], [RecordId]) " +
            "VALUES ({0}, {1}, {2}, {3}), ({0}, {1}, {4}, {5})",
            dayClose.Id, user.Id, (byte)DayCloseRecordKind.ObligationSettlement, cardSettlement,
            (byte)DayCloseRecordKind.Obligation, byCard);

        await database.MigrateToLatestAsync();

        await using var read = database.CreateContext();
        var debts = await read.Debts.AsNoTracking()
            .Include(debt => debt.Items)
            .Include(debt => debt.Payments).ThenInclude(payment => payment.Allocations)
            .Where(debt => debt.UserId == user.Id)
            .ToDictionaryAsync(debt => debt.Id);
        Assert.Equal(6, debts.Count);

        var openDebt = debts[open];
        Assert.Equal(DebtPlanKind.Single, openDebt.PlanKind);
        Assert.Equal(DebtSourceType.Expense, openDebt.SourceType);
        Assert.Equal(supplier.Id, openDebt.CounterpartyId);
        Assert.Equal(bills.Id, openDebt.CategoryId);
        Assert.Equal(new Money(250m, CurrencyCode.TRY), openDebt.Principal);
        Assert.Equal(openDebt.Principal, openDebt.TotalRepayment);
        Assert.Equal(new DateOnly(2026, 8, 10), openDebt.StartDate);
        Assert.Equal(new DateOnly(2026, 8, 20), openDebt.DueDate);
        Assert.Equal(createdAt, openDebt.CreatedAtUtc);
        Assert.Equal("Fatura", openDebt.Description);
        Assert.Equal(ObligationStatus.Open, openDebt.Status);
        var openItem = Assert.Single(openDebt.Items);
        Assert.Equal(open, openItem.Id);
        Assert.Equal(250m, openItem.PrincipalPortion);
        Assert.Equal(0m, openItem.InterestPortion);
        Assert.Empty(openDebt.Payments);

        var settledDebt = debts[settled];
        Assert.Equal(ObligationStatus.Settled, settledDebt.Status);
        Assert.Null(settledDebt.CounterpartyId);
        Assert.Equal(settlement, settledDebt.Settlement!.Id);
        Assert.Equal(bank.Id, settledDebt.Settlement.AccountId);
        Assert.Equal(new DateOnly(2026, 8, 12), settledDebt.Settlement.PaymentDate);
        Assert.Equal(createdAt.AddDays(2), settledDebt.Settlement.PaidAtUtc);
        var allocation = Assert.Single(settledDebt.Settlement.Allocations);
        Assert.Equal(settled, allocation.ScheduleItemId);
        Assert.Equal(120m, allocation.Amount);

        var cardDebt = debts[byCard];
        Assert.Equal(DebtSourceType.Income, cardDebt.SourceType);
        Assert.Equal(card.Id, cardDebt.Settlement!.PosSettlementId);
        Assert.Equal(cardSettlement, cardDebt.Settlement.Id);

        var cancelledDebt = debts[cancelled];
        Assert.Equal(ObligationStatus.Cancelled, cancelledDebt.Status);
        Assert.Equal(cancelledAt, cancelledDebt.CancelledAtUtc);
        Assert.True(cancelledDebt.Settlement!.IsCancelled);
        Assert.Equal(cancelledAt, cancelledDebt.Settlement.CancelledAtUtc);

        var loanDebt = debts[loan];
        Assert.Equal(DebtPlanKind.Installments, loanDebt.PlanKind);
        Assert.Equal(3, loanDebt.InstallmentCount);
        Assert.Equal(DebtSourceType.Cash, loanDebt.SourceType);
        Assert.Equal(bank.Id, loanDebt.OpeningAccountId);
        Assert.Equal(createdAt, loanDebt.CreatedAtUtc);
        Assert.Equal(loanItems, loanDebt.Items.OrderBy(item => item.Sequence).Select(item => item.Id));
        var loanPayment = Assert.Single(loanDebt.Payments);
        Assert.Equal(loanItems[0], loanPayment.Id);
        Assert.Equal(DebtDirection.Payable, loanPayment.Direction);
        Assert.Equal(new DateOnly(2026, 8, 15), loanPayment.PaymentDate);
        Assert.Equal(createdAt.AddDays(5), loanPayment.PaidAtUtc);
        Assert.Equal(loanItems[0], Assert.Single(loanPayment.Allocations).ScheduleItemId);
        Assert.True(loanDebt.IsPaid(loanDebt.GetInstallment(1)));
        Assert.False(loanDebt.IsPaid(loanDebt.GetInstallment(2)));
        Assert.Equal(200m, loanDebt.RemainingAmount);

        // Açılışı bilinmeyen eski borç uydurulmuş bir giriş anı ya da ayrım almaz.
        var legacyDebt = debts[legacy];
        Assert.True(legacyDebt.HasUnrecordedOpening);
        Assert.Null(legacyDebt.CreatedAtUtc);
        var legacyScheduleItem = Assert.Single(legacyDebt.Items);
        Assert.Equal(legacyItem, legacyScheduleItem.Id);
        Assert.Null(legacyScheduleItem.PrincipalPortion);
        Assert.Null(legacyScheduleItem.InterestPortion);

        // Gün sonunun bağları aynı kimlikleri gösterir; okuma görünümleri de.
        var counted = await read.DayCloseCountedRecords.AsNoTracking()
            .Where(record => record.DayCloseId == dayClose.Id)
            .Select(record => record.RecordId)
            .ToArrayAsync();
        Assert.Equal(new[] { byCard, cardSettlement }.Order(), counted.Order());
        Assert.Equal(4, await read.Obligations.CountAsync(item => item.UserId == user.Id));
        Assert.Equal(3, await read.ObligationSettlements.CountAsync(item => item.UserId == user.Id));
        Assert.Equal(3, await read.DebtOpenItems.CountAsync(item => item.UserId == user.Id && item.PlanKind == DebtPlanKind.Installments));
        Assert.Equal(
            bank.Id,
            await read.DebtPaymentRows
                .Where(item => item.Id == loanItems[0])
                .Select(item => item.AccountId)
                .SingleAsync());
    }
}
