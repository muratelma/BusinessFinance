using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.Tokens;

public interface IRefreshSessionRepository
{
    Task AddAsync(RefreshSession session, CancellationToken cancellationToken);

    Task<RefreshSession?> FindByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task UpdateAsync(RefreshSession session, CancellationToken cancellationToken);

    Task RotateAsync(
        RefreshSession currentSession,
        RefreshSession replacementSession,
        CancellationToken cancellationToken);

    Task RevokeAllActiveForUserAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken);
}
