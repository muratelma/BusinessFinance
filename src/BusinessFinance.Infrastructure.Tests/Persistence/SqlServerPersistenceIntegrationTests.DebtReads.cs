using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.CreditCards;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.Obligations;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

/// <summary>
/// ADR 0021 dalga 2b: okumalar borcun kapanışlarından okur — kısmi ödeme,
/// indirim ve kartla ödeme, gerçek SQL Server'da.
/// </summary>
public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Kısmen ödenmiş taksit planlananlarda kalanıyla, net varlıkta kalan
    /// anaparasıyla görünür; kısmen ödenmiş fatura açık kalır; indirim gideri
    /// fatura gününde azaltır; kartla ödenen fatura hesabı oynatmaz, kart
    /// borcunu artırır ve ikinci bir gider yazmaz.
    /// </summary>
    [SqlServerFact]
    public async Task DebtReads_FollowPartialPaymentsDiscountsAndCardPayments()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("debt-reads@example.test");
        await database.SeedUsersAsync(user);

        var now = new DateTimeOffset(2026, 10, 28, 9, 0, 0, TimeSpan.Zero);
        var bank = new Account(Guid.NewGuid(), user.Id, "Ziraat Vadesiz", AccountType.Bank, CurrencyCode.TRY, 10_000m);
        var fixtures = new Category(Guid.NewGuid(), user.Id, "Demirbaş", CategoryType.Expense);
        var supplier = new Counterparty(Guid.NewGuid(), user.Id, "Test Toptancı");
        var card = new CreditCard(Guid.NewGuid(), user.Id, "İş kartı", new Money(50_000m, CurrencyCode.TRY), 10, 20);

        // Borç para: 4 × 1.000, faizsiz; 1.500 ödendi → birinci taksit kapandı,
        // ikincinin 500'ü ödendi.
        var loan = Debt.CreateInstallments(
            Guid.NewGuid(), user.Id, supplier, DebtDirection.Payable, TransactionScope.Business,
            Try(4_000m), Try(4_000m), DebtSourceType.Cash, bank, null,
            new DateOnly(2026, 8, 1), new DateOnly(2026, 9, 10), 4);
        loan.Pay(Guid.NewGuid(), 1_500m, new DateOnly(2026, 10, 5), now, bank);

        // Raf faturası: 10.000; 3.000 peşin, 1.000 indirim → kalan 6.000.
        var shelf = Debt.CreateSingle(
            Guid.NewGuid(), user.Id, fixtures, DebtDirection.Payable, Try(10_000m), TransactionScope.Business,
            new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 30), now, supplier, "Raf faturası");
        shelf.Pay(Guid.NewGuid(), 3_000m, new DateOnly(2026, 10, 1), now, bank);
        shelf.ApplyDiscount(Guid.NewGuid(), 1_000m, new DateOnly(2026, 10, 28), now);

        // Nakliye faturası: 950, kartla ödendi.
        var freight = Debt.CreateSingle(
            Guid.NewGuid(), user.Id, fixtures, DebtDirection.Payable, Try(950m), TransactionScope.Business,
            new DateOnly(2026, 10, 20), new DateOnly(2026, 10, 25), now, supplier, "Nakliye faturası");
        freight.Pay(Guid.NewGuid(), 950m, new DateOnly(2026, 10, 28), now, null, card);

        await using (var seed = database.CreateContext())
        {
            seed.AddRange(bank, fixtures, supplier, card, loan, shelf, freight);
            await seed.SaveChangesAsync();
        }

        await using var provider = CreateServiceProvider(database.ConnectionString);
        await using var scope = provider.CreateAsyncScope();
        var services = scope.ServiceProvider;

        // Hesap: 10.000 + 4.000 borç − 1.500 taksit − 3.000 peşin = 9.500.
        Assert.Equal(9_500m, await services.GetRequiredService<IAccountRepository>()
            .CalculateBalanceAsync(bank.Id, user.Id, CancellationToken.None));

        // Gider: raf 9.000 (indirimle, fatura gününde) + nakliye 950. Kartla
        // ödeme ikinci bir gider değildir.
        var reports = services.GetRequiredService<IFinancialReportRepository>();
        var october = await reports.GetMonthlyAsync(user.Id, 2026, 10, null, CancellationToken.None);
        Assert.Equal(9_950m, october.TotalExpense);

        Assert.Equal(950m, await services.GetRequiredService<ICreditCardRepository>()
            .CalculateCurrentDebtAsync(card.Id, user.Id, CancellationToken.None));

        var obligations = await services.GetRequiredService<IObligationRepository>()
            .ListAsync(user.Id, new DateOnly(2026, 10, 28), CancellationToken.None);
        var shelfRow = Assert.Single(obligations, row => row.Id == shelf.Id);
        Assert.Equal(ObligationStatus.Open, shelfRow.Status);
        Assert.Null(shelfRow.SettlementId);
        var freightRow = Assert.Single(obligations, row => row.Id == freight.Id);
        Assert.Equal(ObligationStatus.Settled, freightRow.Status);
        Assert.Null(freightRow.SettlementAccountId);

        var planned = await services.GetRequiredService<IPlannedActivityRepository>().ListAsync(
            user.Id, new DateOnly(2026, 10, 28), new DateOnly(2026, 12, 31), null, CancellationToken.None);
        Assert.Equal(
            [500m, 1_000m, 1_000m],
            planned.Where(row => row.ActionTargetId == loan.Id).OrderBy(row => row.DueDate).Select(row => row.Amount));
        Assert.Equal(6_000m, Assert.Single(planned, row => row.PlannedActivityId == shelf.Id).Amount);
        Assert.DoesNotContain(planned, row => row.PlannedActivityId == freight.Id);

        // Net varlıkta borç: kalan anapara 2.500 + açık fatura 6.000.
        var advanced = await reports.GetAdvancedAsync(
            user.Id, 2026, 10, new DateOnly(2026, 10, 28), 1, 30, null, CancellationToken.None);
        Assert.Equal(8_500m, advanced.NetWorth.PayableDebt);

        // Kartla ödenen borç kart borcudur: net varlıkta ve kartın ekstresinde.
        Assert.Equal(950m, advanced.NetWorth.CreditCardDebt);
        var november = await services.GetRequiredService<ICreditCardStatementRepository>().GetActivityAsync(
            card.Id, user.Id, CreditCardStatementPeriod.ForClosingMonth(card, 2026, 11),
            new DateOnly(2026, 10, 28), CancellationToken.None);
        Assert.Equal(950m, november.PeriodCharges);
    }

    private static Money Try(decimal amount) => new(amount, CurrencyCode.TRY);
}
