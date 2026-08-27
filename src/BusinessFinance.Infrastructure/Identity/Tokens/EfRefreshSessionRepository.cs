using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Identity.Tokens;

internal sealed class EfRefreshSessionRepository(BusinessFinanceDbContext dbContext)
    : IRefreshSessionRepository
{
    public async Task AddAsync(
        RefreshSession session,
        CancellationToken cancellationToken)
    {
        await dbContext.RefreshSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshSession?> FindByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return dbContext.RefreshSessions.SingleOrDefaultAsync(
            session => session.TokenHash == tokenHash,
            cancellationToken);
    }

    public async Task UpdateAsync(
        RefreshSession session,
        CancellationToken cancellationToken)
    {
        EnsureTracked(session);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RotateAsync(
        RefreshSession currentSession,
        RefreshSession replacementSession,
        CancellationToken cancellationToken)
    {
        EnsureTracked(currentSession);
        await dbContext.RefreshSessions.AddAsync(replacementSession, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshSession>> ListActiveForUserAsync(
        Guid userId,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken)
    {
        return await dbContext.RefreshSessions
            .AsNoTracking()
            .Where(session =>
                session.UserId == userId &&
                session.RevokedAtUtc == null &&
                session.ExpiresAtUtc > utcNow)
            .OrderByDescending(session => session.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public Task<RefreshSession?> FindOwnedByIdAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.RefreshSessions.SingleOrDefaultAsync(
            session => session.Id == sessionId && session.UserId == userId,
            cancellationToken);
    }

    public async Task RevokeAllActiveForUserAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken)
    {
        var sessions = await dbContext.RefreshSessions
            .Where(session => session.UserId == userId && session.RevokedAtUtc == null)
            .ToArrayAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke(revokedAtUtc);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void EnsureTracked(RefreshSession session)
    {
        if (dbContext.Entry(session).State == EntityState.Detached)
        {
            dbContext.Attach(session);
        }
    }
}
