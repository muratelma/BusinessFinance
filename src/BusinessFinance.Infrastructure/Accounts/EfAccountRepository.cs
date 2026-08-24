using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Accounts;

internal sealed class EfAccountRepository(BusinessFinanceDbContext dbContext)
    : IAccountRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid userId,
        string normalizedName,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId);

        if (string.Equals(
                dbContext.Database.ProviderName,
                "Microsoft.EntityFrameworkCore.InMemory",
                StringComparison.Ordinal))
        {
            var names = await query
                .Select(account => account.Name)
                .ToArrayAsync(cancellationToken);

            return names.Contains(normalizedName, StringComparer.OrdinalIgnoreCase);
        }

        return await query.AnyAsync(
            account => account.Name == normalizedName,
            cancellationToken);
    }

    public async Task<AccountListPage> ListAsync(
        Guid userId,
        AccountListCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Account> query = dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId);

        if (criteria.IsActive is bool isActive)
        {
            query = query.Where(account => account.IsActive == isActive);
        }

        if (criteria.Type is AccountType type)
        {
            query = query.Where(account => account.Type == type);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(account => account.Name)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new AccountListPage(items, totalCount);
    }

    public Task<Account?> FindOwnedByIdAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Accounts.SingleOrDefaultAsync(
            account => account.Id == accountId && account.UserId == userId,
            cancellationToken);
    }

    public async Task UpdateOwnedAsync(
        Account account,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (account.UserId != userId)
        {
            throw new InvalidOperationException("Owned account was not found.");
        }

        var entry = dbContext.Entry(account);
        if (entry.State == EntityState.Detached)
        {
            var exists = await dbContext.Accounts
                .AsNoTracking()
                .AnyAsync(
                    candidate => candidate.Id == account.Id && candidate.UserId == userId,
                    cancellationToken);

            if (!exists)
            {
                throw new InvalidOperationException("Owned account was not found.");
            }

            dbContext.Attach(account);
            entry.Property(candidate => candidate.IsActive).IsModified = true;
            entry.Property(candidate => candidate.Name).IsModified = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> CalculateBalanceAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var openingBalance = await dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.Id == accountId && account.UserId == userId)
            .Select(account => (decimal?)account.OpeningBalance)
            .SingleOrDefaultAsync(cancellationToken);

        if (openingBalance is null)
        {
            throw new InvalidOperationException("Owned account was not found.");
        }

        var movementBalance = await dbContext.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.AccountId == accountId &&
                                  transaction.UserId == userId &&
                                  !transaction.IsCancelled)
            .SumAsync(
                transaction => transaction.Type == TransactionType.Income
                    ? transaction.Amount.Amount
                    : -transaction.Amount.Amount,
                cancellationToken);

        var outgoingTransfers = await dbContext.Transfers
            .AsNoTracking()
            .Where(transfer => transfer.SourceAccountId == accountId &&
                               transfer.UserId == userId &&
                               !transfer.IsCancelled)
            .SumAsync(transfer => transfer.Amount.Amount, cancellationToken);
        var incomingTransfers = await dbContext.Transfers
            .AsNoTracking()
            .Where(transfer => transfer.DestinationAccountId == accountId &&
                               transfer.UserId == userId &&
                               !transfer.IsCancelled)
            .SumAsync(transfer => transfer.Amount.Amount, cancellationToken);
        var cardPayments = await dbContext.CreditCardPayments
            .AsNoTracking()
            .Where(payment => payment.AccountId == accountId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled)
            .SumAsync(payment => payment.Amount.Amount, cancellationToken);
        var debtMovements = await (
                from installment in dbContext.DebtInstallments.AsNoTracking()
                join debt in dbContext.DebtAgreements.AsNoTracking()
                    on new { installment.UserId, DebtId = installment.DebtAgreementId }
                    equals new { debt.UserId, DebtId = debt.Id }
                where installment.UserId == userId &&
                      installment.PaymentAccountId == accountId
                select debt.Direction == DebtDirection.Receivable
                    ? installment.Amount.Amount
                    : -installment.Amount.Amount)
            .SumAsync(cancellationToken);

        // Borcun açılışı. Nakit kaynaklı bir borçta para hesaba girmiştir,
        // alacakta çıkmıştır; ikisi de gelir/gider değildir. Bu hareket
        // olmadan taksitler hesabı boşaltıyor ama karşılığında hiçbir şey
        // girmemiş görünüyordu. Gider kaynaklı borç parayı hiç hareket
        // ettirmez: tüketim zaten gider olarak yazılır.
        var debtOpenings = await dbContext.DebtAgreements
            .AsNoTracking()
            .Where(debt => debt.UserId == userId &&
                           debt.OpeningAccountId == accountId &&
                           debt.SourceType == DebtSourceType.Cash)
            .SumAsync(
                debt => debt.Direction == DebtDirection.Payable
                    ? debt.Principal.Amount
                    : -debt.Principal.Amount,
                cancellationToken);

        // Cari tahsilat/ödeme parayı taşır: tahsilat kasayı artırır, ödeme
        // azaltır. Gelir/gider üretmediği için rapora değil yalnız buraya
        // girer (ADR 0014).
        var counterpartySettlements = await dbContext.CounterpartyPayments
            .AsNoTracking()
            .Where(payment => payment.AccountId == accountId &&
                              payment.UserId == userId &&
                              !payment.IsCancelled)
            .SumAsync(
                payment => payment.Direction == DebtDirection.Receivable
                    ? payment.Amount.Amount
                    : -payment.Amount.Amount,
                cancellationToken);

        var obligationSettlements = await dbContext.ObligationSettlements
            .AsNoTracking()
            .Where(settlement => settlement.AccountId == accountId &&
                                 settlement.UserId == userId &&
                                 !settlement.IsCancelled)
            .SumAsync(
                settlement => settlement.Direction == DebtDirection.Receivable
                    ? settlement.Amount.Amount
                    : -settlement.Amount.Amount,
                cancellationToken);

        return openingBalance.Value + movementBalance + incomingTransfers - outgoingTransfers -
               cardPayments + debtMovements + debtOpenings + counterpartySettlements +
               obligationSettlements;
    }
}
