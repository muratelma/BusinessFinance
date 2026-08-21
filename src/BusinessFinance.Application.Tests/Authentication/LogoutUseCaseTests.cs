using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Logout;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Authentication;

public sealed class LogoutUseCaseTests
{
    private static readonly DateTimeOffset UtcNow = new(
        2026,
        8,
        7,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public async Task ExecuteAsync_WithActiveSession_RevokesSession()
    {
        var session = new RefreshSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "refresh-hash",
            UtcNow.AddDays(-1),
            UtcNow.AddDays(29));
        var repository = new RecordingRepository(session);
        var useCase = new LogoutUseCase(
            new FakeTokenService(),
            repository,
            new FixedTimeProvider(UtcNow));

        var result = await useCase.ExecuteAsync(new LogoutCommand("raw-refresh-token"));

        Assert.True(result.IsSuccess);
        Assert.Equal(UtcNow, session.RevokedAtUtc);
        Assert.Same(session, repository.UpdatedSession);
    }

    [Theory]
    [InlineData("")]
    [InlineData("unknown-token")]
    public async Task ExecuteAsync_WithoutMatchingSession_RemainsIdempotentlySuccessful(
        string refreshToken)
    {
        var repository = new RecordingRepository(null);
        var useCase = new LogoutUseCase(
            new FakeTokenService(),
            repository,
            new FixedTimeProvider(UtcNow));

        var result = await useCase.ExecuteAsync(new LogoutCommand(refreshToken));

        Assert.True(result.IsSuccess);
        Assert.Null(repository.UpdatedSession);
    }

    private sealed class FakeTokenService : ISecurityTokenService
    {
        public IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity)
            => throw new NotSupportedException();

        public IssuedRefreshToken CreateRefreshToken()
            => throw new NotSupportedException();

        public string HashRefreshToken(string refreshToken) => "refresh-hash";
    }

    private sealed class RecordingRepository(RefreshSession? session)
        : IRefreshSessionRepository
    {
        public RefreshSession? UpdatedSession { get; private set; }

        public Task AddAsync(RefreshSession newSession, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<RefreshSession?> FindByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken) => Task.FromResult(session);

        public Task UpdateAsync(
            RefreshSession updatedSession,
            CancellationToken cancellationToken)
        {
            UpdatedSession = updatedSession;
            return Task.CompletedTask;
        }

        public Task RotateAsync(
            RefreshSession currentSession,
            RefreshSession replacementSession,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task RevokeAllActiveForUserAsync(
            Guid userId,
            DateTimeOffset revokedAtUtc,
            CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
