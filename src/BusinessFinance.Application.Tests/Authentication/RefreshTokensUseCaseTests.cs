using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Application.Authentication.RefreshTokens;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Authentication;

public sealed class RefreshTokensUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly DateTimeOffset UtcNow = new(
        2026,
        8,
        7,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public async Task ExecuteAsync_WithActiveSession_RotatesSessionAndReturnsNewPair()
    {
        var currentSession = CreateActiveSession();
        var repository = new RecordingRefreshSessionRepository(currentSession);
        var identityService = new FakeIdentityAccountService
        {
            Identity = new AuthenticatedIdentity(UserId, "user@example.com")
        };
        var tokenService = new FakeSecurityTokenService();
        var useCase = CreateUseCase(identityService, tokenService, repository);

        var result = await useCase.ExecuteAsync(new RefreshTokensCommand("old-raw-token"));

        Assert.True(result.IsSuccess);
        Assert.Equal("new-access-token", result.Value.AccessToken);
        Assert.Equal("new-raw-refresh-token", result.Value.RefreshToken);
        Assert.True(currentSession.IsRevoked);
        Assert.NotNull(repository.ReplacementSession);
        Assert.Equal(repository.ReplacementSession.Id, currentSession.ReplacedBySessionId);
        Assert.Equal("new-refresh-hash", repository.ReplacementSession.TokenHash);
    }

    [Fact]
    public async Task ExecuteAsync_WithRevokedSession_DetectsReuseAndRevokesUsersSessions()
    {
        var currentSession = CreateActiveSession();
        currentSession.Rotate(Guid.NewGuid(), UtcNow.AddMinutes(-30));
        var repository = new RecordingRefreshSessionRepository(currentSession);
        var useCase = CreateUseCase(
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository);

        var result = await useCase.ExecuteAsync(new RefreshTokensCommand("reused-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal("authentication.invalid_refresh_token", result.Error.Code);
        Assert.Equal(UtcNow, currentSession.ReuseDetectedAtUtc);
        Assert.Equal(UserId, repository.RevokedAllForUserId);
    }

    [Fact]
    public async Task ExecuteAsync_WithExpiredSession_RevokesAndReturnsGenericError()
    {
        var expiredSession = new RefreshSession(
            Guid.NewGuid(),
            UserId,
            "old-refresh-hash",
            UtcNow.AddDays(-31),
            UtcNow.AddDays(-1));
        var repository = new RecordingRefreshSessionRepository(expiredSession);
        var useCase = CreateUseCase(
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository);

        var result = await useCase.ExecuteAsync(new RefreshTokensCommand("expired-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal("authentication.invalid_refresh_token", result.Error.Code);
        Assert.Equal(UtcNow, expiredSession.RevokedAtUtc);
        Assert.Same(expiredSession, repository.UpdatedSession);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSessionDoesNotExist_ReturnsGenericError()
    {
        var repository = new RecordingRefreshSessionRepository(null);
        var useCase = CreateUseCase(
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository);

        var result = await useCase.ExecuteAsync(new RefreshTokensCommand("unknown-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal("authentication.invalid_refresh_token", result.Error.Code);
        Assert.Null(repository.ReplacementSession);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsInactive_RevokesSessionAndReturnsGenericError()
    {
        var currentSession = CreateActiveSession();
        var repository = new RecordingRefreshSessionRepository(currentSession);
        var useCase = CreateUseCase(
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository);

        var result = await useCase.ExecuteAsync(new RefreshTokensCommand("inactive-user-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UtcNow, currentSession.RevokedAtUtc);
        Assert.Same(currentSession, repository.UpdatedSession);
    }

    private static RefreshTokensUseCase CreateUseCase(
        IIdentityAccountService identityService,
        ISecurityTokenService tokenService,
        IRefreshSessionRepository repository)
    {
        return new RefreshTokensUseCase(
            identityService,
            tokenService,
            repository,
            new FixedTimeProvider(UtcNow));
    }

    private static RefreshSession CreateActiveSession()
    {
        return new RefreshSession(
            Guid.NewGuid(),
            UserId,
            "old-refresh-hash",
            UtcNow.AddDays(-1),
            UtcNow.AddDays(29));
    }

    private sealed class FakeIdentityAccountService : IIdentityAccountService
    {

        public Task<UserAccountProfile?> FindAccountAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> VerifyPasswordAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<PasswordChangeStatus> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task DeleteAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public AuthenticatedIdentity? Identity { get; init; }

        public Task<IdentityRegistrationResult> RegisterAsync(
            string email,
            string password,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<AuthenticatedIdentity?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<AuthenticatedIdentity?> FindActiveByIdAsync(
            Guid userId,
            CancellationToken cancellationToken) => Task.FromResult(Identity);
    }

    private sealed class FakeSecurityTokenService : ISecurityTokenService
    {
        public IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity)
        {
            return new IssuedAccessToken("new-access-token", UtcNow.AddMinutes(15));
        }

        public IssuedRefreshToken CreateRefreshToken()
        {
            return new IssuedRefreshToken(
                "new-raw-refresh-token",
                "new-refresh-hash",
                UtcNow,
                UtcNow.AddDays(30));
        }

        public string HashRefreshToken(string refreshToken)
        {
            return "old-refresh-hash";
        }
    }

    private sealed class RecordingRefreshSessionRepository(RefreshSession? session)
        : IRefreshSessionRepository
    {

        public Task<IReadOnlyList<RefreshSession>> ListActiveForUserAsync(
            Guid userId,
            DateTimeOffset utcNow,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<RefreshSession?> FindOwnedByIdAsync(
            Guid sessionId,
            Guid userId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public RefreshSession? UpdatedSession { get; private set; }
        public RefreshSession? ReplacementSession { get; private set; }
        public Guid? RevokedAllForUserId { get; private set; }

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
            CancellationToken cancellationToken)
        {
            UpdatedSession = currentSession;
            ReplacementSession = replacementSession;
            return Task.CompletedTask;
        }

        public Task RevokeAllActiveForUserAsync(
            Guid userId,
            DateTimeOffset revokedAtUtc,
            CancellationToken cancellationToken)
        {
            RevokedAllForUserId = userId;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
