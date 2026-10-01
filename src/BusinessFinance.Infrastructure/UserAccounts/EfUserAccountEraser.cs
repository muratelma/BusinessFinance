using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.UserAccounts;

/// <summary>
/// Hesabın bütün verisini siler (ADR 0017).
/// </summary>
/// <remarks>
/// Şemadaki bütün foreign key'ler <c>Restrict</c> davranışındadır — kimlik
/// kaydını silmek finansal satırları düşürmez, silme girişimini reddeder. Bu
/// bilinçli bir korumadır: veri yalnız buradaki gibi açıkça yazılmış bir sırayla
/// silinir. Sıra çocuktan ebeveyne doğrudur ve tek transaction içinde çalışır;
/// yarıda kalan bir silme, yarısı kaybolmuş bir defter bırakırdı.
/// </remarks>
internal sealed class EfUserAccountEraser(
    BusinessFinanceDbContext dbContext,
    IAttachmentObjectStore attachmentObjectStore)
    : IUserAccountEraser
{
    public async Task EraseAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return;
        }

        // Dosyalar veritabanının dışında yaşıyor; anahtarları satırlar silinmeden
        // önce okunur, dosyaların kendisi transaction başarıyla kapandıktan sonra
        // silinir. Ters sırada bir hata, kaydı duran ama dosyası olmayan bir ek
        // bırakırdı.
        var attachmentKeys = await dbContext.FinancialAttachments
            .AsNoTracking()
            .Where(attachment => attachment.UserId == userId)
            .Select(attachment => attachment.ObjectKey)
            .ToArrayAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

            await dbContext.ImportRows
                .Where(row => row.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.ImportBatches
                .Where(batch => batch.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.FinancialAttachments
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.SavingsGoalContributions
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.SavingsGoals
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.ObligationSettlements
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Obligations
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.PosSettlements
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            // Tanım, tahsilatlar gittikten sonra silinir (tahsilat ona bağlıdır).
            await dbContext.PosDefinitions
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CashCounts
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CounterpartyPayments
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CounterpartyCharges
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.DebtInstallments
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.DebtAgreements
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Counterparties
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.RecurringTransactionOccurrences
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.RecurringTransactions
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.InstallmentItems
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.InstallmentPlans
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CreditCardPayments
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CreditCardCharges
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.CreditCards
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Transfers
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Transactions
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.MonthlyBudgets
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Accounts
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.Categories
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.UserProfiles
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.RefreshSessions
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await dbContext.VerificationCodes
                .Where(item => item.UserId == userId).ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });

        foreach (var objectKey in attachmentKeys)
        {
            await attachmentObjectStore.DeleteIfExistsAsync(objectKey, cancellationToken);
        }
    }
}
