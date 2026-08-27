using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Application.Authentication.LoginUser;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Authentication;

public sealed class LoginUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsAuthenticatedUser()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var service = new FakeIdentityAccountService
        {
            AuthenticatedIdentity = new AuthenticatedIdentity(userId, "user@example.com")
        };
        var tokenService = new FakeSecurityTokenService();
        var repository = new RecordingRefreshSessionRepository();
        var useCase = new LoginUserUseCase(service, tokenService, repository);

        var result = await useCase.ExecuteAsync(
            new LoginUserCommand("user@example.com", "Valid-Password-123!"));

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal("user@example.com", result.Value.Email);
        Assert.Equal("access-token", result.Value.AccessToken);
        Assert.Equal("raw-refresh-token", result.Value.RefreshToken);
        var session = Assert.Single(repository.AddedSessions);
        Assert.Equal(userId, session.UserId);
        Assert.Equal("refresh-token-hash", session.TokenHash);
    }

    [Theory]
    [InlineData("unknown@example.com", "Valid-Password-123!")]
    [InlineData("user@example.com", "Wrong-Password-123!")]
    public async Task ExecuteAsync_WithInvalidCredentials_ReturnsSameGenericError(
        string email,
        string password)
    {
        var repository = new RecordingRefreshSessionRepository();
        var useCase = new LoginUserUseCase(
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository);

        var result = await useCase.ExecuteAsync(new LoginUserCommand(email, password));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Unauthorized, result.Error.Type);
        Assert.Equal("authentication.invalid_credentials", result.Error.Code);
        Assert.Equal("Email or password is invalid.", result.Error.Message);
        Assert.Empty(repository.AddedSessions);
    }

    [Fact]
    public async Task ExecuteAsync_ForwardsCancellationToken()
    {
        var service = new FakeIdentityAccountService();
        var useCase = new LoginUserUseCase(
            service,
            new FakeSecurityTokenService(),
            new RecordingRefreshSessionRepository());
        using var cancellationTokenSource = new CancellationTokenSource();

        await useCase.ExecuteAsync(
            new LoginUserCommand("user@example.com", "Valid-Password-123!"),
            cancellationTokenSource.Token);

        Assert.Equal(cancellationTokenSource.Token, service.ReceivedCancellationToken);
    }

    private sealed class FakeIdentityAccountService : IIdentityAccountService
    {
        public Task<Guid?> FindActiveUserIdByEmailAsync(
            string email,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task MarkEmailConfirmedAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<PasswordChangeStatus> SetPasswordAsync(
            Guid userId,
            string newPassword,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

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
        public AuthenticatedIdentity? AuthenticatedIdentity { get; init; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<IdentityRegistrationResult> RegisterAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<AuthenticatedIdentity?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(AuthenticatedIdentity);
        }

        public Task<AuthenticatedIdentity?> FindActiveByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class FakeSecurityTokenService : ISecurityTokenService
    {
        private static readonly DateTimeOffset UtcNow = new(
            2026,
            8,
            7,
            12,
            0,
            0,
            TimeSpan.Zero);

        public IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity)
        {
            return new IssuedAccessToken("access-token", UtcNow.AddMinutes(15));
        }

        public IssuedRefreshToken CreateRefreshToken()
        {
            return new IssuedRefreshToken(
                "raw-refresh-token",
                "refresh-token-hash",
                UtcNow,
                UtcNow.AddDays(30));
        }

        public string HashRefreshToken(string refreshToken)
        {
            return "refresh-token-hash";
        }
    }

    private sealed class RecordingRefreshSessionRepository : IRefreshSessionRepository
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
        public List<RefreshSession> AddedSessions { get; } = [];

        public Task AddAsync(RefreshSession session, CancellationToken cancellationToken)
        {
            AddedSessions.Add(session);
            return Task.CompletedTask;
        }

        public Task<RefreshSession?> FindByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task UpdateAsync(
            RefreshSession session,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task RotateAsync(
            RefreshSession currentSession,
            RefreshSession replacementSession,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task RevokeAllActiveForUserAsync(
            Guid userId,
            DateTimeOffset revokedAtUtc,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
