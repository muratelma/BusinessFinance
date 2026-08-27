using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Verification;

internal sealed class EfVerificationCodeRepository(BusinessFinanceDbContext dbContext)
    : IVerificationCodeRepository
{
    public async Task AddAsync(VerificationCode code, CancellationToken cancellationToken)
    {
        await dbContext.VerificationCodes.AddAsync(code, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<VerificationCode?> FindLatestAsync(
        Guid userId,
        VerificationPurpose purpose,
        CancellationToken cancellationToken)
    {
        return dbContext.VerificationCodes
            .Where(code => code.UserId == userId && code.Purpose == purpose)
            .OrderByDescending(code => code.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(VerificationCode code, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(code).State == EntityState.Detached)
        {
            dbContext.Attach(code);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ConsumeAllAsync(
        Guid userId,
        VerificationPurpose purpose,
        DateTimeOffset consumedAtUtc,
        CancellationToken cancellationToken)
    {
        var pending = await dbContext.VerificationCodes
            .Where(code =>
                code.UserId == userId &&
                code.Purpose == purpose &&
                code.ConsumedAtUtc == null)
            .ToArrayAsync(cancellationToken);

        foreach (var code in pending)
        {
            code.Consume(consumedAtUtc);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
