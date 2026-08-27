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

    /// <summary>
    /// Kullanıcının açık oturumları. Süresi geçmiş veya iptal edilmiş oturum
    /// listeye girmez.
    /// </summary>
    Task<IReadOnlyList<RefreshSession>> ListActiveForUserAsync(
        Guid userId,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sahiplik kapsamlı tek oturum okuması: başka kullanıcının oturumu
    /// bulunamamış sayılır.
    /// </summary>
    Task<RefreshSession?> FindOwnedByIdAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task RevokeAllActiveForUserAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken);
}
