using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Attachments;

internal sealed class EfAttachmentRepository(BusinessFinanceDbContext dbContext)
    : IAttachmentRepository
{
    public async Task AddAsync(FinancialAttachment attachment, CancellationToken cancellationToken)
    {
        dbContext.FinancialAttachments.Add(attachment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FinancialAttachment>> ListAsync(
        Guid transactionId,
        Guid userId,
        CancellationToken cancellationToken) =>
        await dbContext.FinancialAttachments.AsNoTracking()
            .Where(x => x.TransactionId == transactionId && x.UserId == userId)
            .OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

    public Task<FinancialAttachment?> FindOwnedByIdAsync(
        Guid attachmentId,
        Guid userId,
        CancellationToken cancellationToken) =>
        dbContext.FinancialAttachments.AsNoTracking().SingleOrDefaultAsync(
            x => x.Id == attachmentId && x.UserId == userId,
            cancellationToken);
}
