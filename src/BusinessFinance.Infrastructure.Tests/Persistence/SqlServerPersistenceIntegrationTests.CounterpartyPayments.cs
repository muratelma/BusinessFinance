using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Counterparties;
using Microsoft.EntityFrameworkCore;

namespace BusinessFinance.Infrastructure.Tests.Persistence;

public sealed partial class SqlServerPersistenceIntegrationTests
{
    /// <summary>
    /// Aynı istek kimliği aynı tahsilat kimliğini üretir. İki istek aynı anda
    /// gelirse ikisi de "henüz yok" görür; birincil anahtar yalnız birini yazar,
    /// öbürü hata değil "zaten yazıldı" cevabı alır ve kasa bir kez değişir.
    /// </summary>
    [SqlServerFact]
    public async Task CounterpartyPayment_SameIdFromTwoRequestsAtOnce_IsWrittenOnce()
    {
        await using var database = await SqlTestDatabase.CreateAsync(GetConnectionString());
        var user = CreateUser("ledger-payment-race@example.test");
        await database.SeedUsersAsync(user);

        var account = new Account(Guid.NewGuid(), user.Id, "Kasa", AccountType.Cash, CurrencyCode.TRY);
        var supplier = new Counterparty(Guid.NewGuid(), user.Id, "Yıldız Gıda");
        await using (var seed = database.CreateContext())
        {
            seed.AddRange(account, supplier);
            await seed.SaveChangesAsync(CancellationToken.None);
        }

        var paymentId = Guid.NewGuid();
        await using var firstContext = database.CreateContext();
        await using var secondContext = database.CreateContext();

        var outcomes = await Task.WhenAll(
            AddAsync(firstContext),
            AddAsync(secondContext));

        Assert.Single(outcomes, written => written);
        Assert.Single(outcomes, written => !written);
        await using var read = database.CreateContext();
        var payment = Assert.Single(await read.CounterpartyPayments.AsNoTracking().ToArrayAsync());
        Assert.Equal(paymentId, payment.Id);
        Assert.Equal(1000m, payment.Amount.Amount);

        async Task<bool> AddAsync(BusinessFinance.Infrastructure.Persistence.BusinessFinanceDbContext context)
        {
            var ownAccount = await context.Accounts.SingleAsync(item => item.Id == account.Id);
            var ownSupplier = await context.Counterparties.SingleAsync(item => item.Id == supplier.Id);
            var candidate = new CounterpartyPayment(
                paymentId, user.Id, ownSupplier, ownAccount, DebtDirection.Payable,
                new Money(1000m, CurrencyCode.TRY), new DateOnly(2026, 10, 15));
            return await new EfCounterpartyRepository(context)
                .AddPaymentAsync(candidate, null, CancellationToken.None);
        }
    }
}
