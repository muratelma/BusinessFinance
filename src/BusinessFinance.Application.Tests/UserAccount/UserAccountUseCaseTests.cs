using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.UserAccount;

public sealed class UserAccountUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly DateTimeOffset Now =
        new(2026, 8, 27, 9, 0, 0, TimeSpan.Zero);

    private static int _step;

    [Fact]
    public async Task GetAccount_ReturnsEmailAndOpenSessionCount()
    {
        var repository = new FakeRefreshSessionRepository();
        repository.Sessions.Add(CreateSession(UserId));
        repository.Sessions.Add(CreateSession(UserId));
        repository.Sessions.Add(CreateSession(OtherUserId));

        var useCase = new GetUserAccountUseCase(
            new FakeCurrentUser(UserId),
            new FakeIdentityAccountService(),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.test", result.Value.Email);
        Assert.Equal(2, result.Value.ActiveSessionCount);
    }

    [Fact]
    public async Task ListSessions_DoesNotReturnAnotherUsersSessions()
    {
        var repository = new FakeRefreshSessionRepository();
        var own = CreateSession(UserId);
        repository.Sessions.Add(own);
        repository.Sessions.Add(CreateSession(OtherUserId));

        var useCase = new ListUserSessionsUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(own.Id, Assert.Single(result.Value).SessionId);
    }

    [Fact]
    public async Task RevokeSession_WithAnotherUsersSession_ReturnsNotFoundAndLeavesItOpen()
    {
        var repository = new FakeRefreshSessionRepository();
        var foreignSession = CreateSession(OtherUserId);
        repository.Sessions.Add(foreignSession);

        var useCase = new RevokeUserSessionUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(
            new RevokeUserSessionCommand(foreignSession.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal("account.session_not_found", result.Error.Code);
        Assert.False(foreignSession.IsRevoked);
    }

    [Fact]
    public async Task RevokeSession_WithOwnSession_ClosesIt()
    {
        var repository = new FakeRefreshSessionRepository();
        var session = CreateSession(UserId);
        repository.Sessions.Add(session);

        var useCase = new RevokeUserSessionUseCase(
            new FakeCurrentUser(UserId),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(new RevokeUserSessionCommand(session.Id));

        Assert.True(result.IsSuccess);
        Assert.True(session.IsRevoked);
    }

    [Fact]
    public async Task ChangePassword_ClosesEverySessionAndIssuesAFreshPairForTheCaller()
    {
        var repository = new FakeRefreshSessionRepository();
        var existing = CreateSession(UserId);
        repository.Sessions.Add(existing);

        var useCase = new ChangePasswordUseCase(
            new FakeCurrentUser(UserId),
            new FakeIdentityAccountService(),
            new FakeSecurityTokenService(),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(
            new ChangePasswordCommand("Old-Password-123!", "New-Password-123!"));

        Assert.True(result.IsSuccess);
        Assert.True(existing.IsRevoked);

        var issued = Assert.Single(repository.Sessions, session => !session.IsRevoked);
        Assert.Equal(issued.Id, result.Value.SessionId);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.RefreshToken));
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ChangesNothing()
    {
        var repository = new FakeRefreshSessionRepository();
        var existing = CreateSession(UserId);
        repository.Sessions.Add(existing);

        var useCase = new ChangePasswordUseCase(
            new FakeCurrentUser(UserId),
            new FakeIdentityAccountService
            {
                PasswordChangeOutcome = PasswordChangeStatus.InvalidCurrentPassword
            },
            new FakeSecurityTokenService(),
            repository,
            new FixedTimeProvider(Now));

        var result = await useCase.ExecuteAsync(
            new ChangePasswordCommand("wrong", "New-Password-123!"));

        Assert.False(result.IsSuccess);
        Assert.Equal("account.invalid_password", result.Error.Code);
        Assert.False(existing.IsRevoked);
    }

    [Fact]
    public async Task DeleteAccount_WithoutConfirmation_ErasesNothing()
    {
        var identity = new FakeIdentityAccountService();
        var eraser = new RecordingEraser();

        var useCase = new DeleteUserAccountUseCase(
            new FakeCurrentUser(UserId),
            identity,
            eraser);

        var result = await useCase.ExecuteAsync(
            new DeleteUserAccountCommand("Valid-Password-123!", Confirmed: false));

        Assert.False(result.IsSuccess);
        Assert.Equal("account.delete_not_confirmed", result.Error.Code);
        Assert.Empty(eraser.ErasedUserIds);
        Assert.Empty(identity.DeletedUserIds);
    }

    [Fact]
    public async Task DeleteAccount_WithWrongPassword_ErasesNothing()
    {
        var identity = new FakeIdentityAccountService { PasswordIsValid = false };
        var eraser = new RecordingEraser();

        var useCase = new DeleteUserAccountUseCase(
            new FakeCurrentUser(UserId),
            identity,
            eraser);

        var result = await useCase.ExecuteAsync(
            new DeleteUserAccountCommand("wrong", Confirmed: true));

        Assert.False(result.IsSuccess);
        Assert.Equal("account.invalid_password", result.Error.Code);
        Assert.Empty(eraser.ErasedUserIds);
        Assert.Empty(identity.DeletedUserIds);
    }

    [Fact]
    public async Task DeleteAccount_ErasesTheDataBeforeTheIdentity()
    {
        var identity = new FakeIdentityAccountService();
        var eraser = new RecordingEraser();

        var useCase = new DeleteUserAccountUseCase(
            new FakeCurrentUser(UserId),
            identity,
            eraser);

        var result = await useCase.ExecuteAsync(
            new DeleteUserAccountCommand("Valid-Password-123!", Confirmed: true));

        Assert.True(result.IsSuccess);
        Assert.Equal(UserId, Assert.Single(eraser.ErasedUserIds));
        Assert.Equal(UserId, Assert.Single(identity.DeletedUserIds));
        Assert.True(eraser.ErasedAtStep < identity.DeletedAtStep);
    }

    private static RefreshSession CreateSession(Guid userId)
    {
        return new RefreshSession(
            Guid.NewGuid(),
            userId,
            $"hash-{Guid.NewGuid():N}",
            Now.AddDays(-1),
            Now.AddDays(29));
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class RecordingEraser : IUserAccountEraser
    {
        public List<Guid> ErasedUserIds { get; } = [];
        public int ErasedAtStep { get; private set; }

        public Task EraseAsync(Guid userId, CancellationToken cancellationToken)
        {
            ErasedUserIds.Add(userId);
            ErasedAtStep = Interlocked.Increment(ref _step);
            return Task.CompletedTask;
        }
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
        public bool PasswordIsValid { get; init; } = true;

        public PasswordChangeStatus PasswordChangeOutcome { get; init; } =
            PasswordChangeStatus.Succeeded;

        public List<Guid> DeletedUserIds { get; } = [];
        public int DeletedAtStep { get; private set; }

        public Task<IdentityRegistrationResult> RegisterAsync(
            string email,
            string password,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<AuthenticatedIdentity?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<AuthenticatedIdentity?> FindActiveByIdAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            Task.FromResult<AuthenticatedIdentity?>(
                new AuthenticatedIdentity(userId, "user@example.test"));

        public Task<UserAccountProfile?> FindAccountAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            Task.FromResult<UserAccountProfile?>(new UserAccountProfile(
                userId,
                "user@example.test",
                EmailConfirmed: false,
                Now.AddDays(-30)));

        public Task<bool> VerifyPasswordAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken) =>
            Task.FromResult(PasswordIsValid);

        public Task<PasswordChangeStatus> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken) =>
            Task.FromResult(PasswordChangeOutcome);

        public Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            DeletedUserIds.Add(userId);
            DeletedAtStep = Interlocked.Increment(ref _step);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSecurityTokenService : ISecurityTokenService
    {
        public IssuedAccessToken CreateAccessToken(AuthenticatedIdentity identity) =>
            new("access-token", Now.AddMinutes(15));

        public IssuedRefreshToken CreateRefreshToken() =>
            new(
                $"refresh-{Guid.NewGuid():N}",
                $"hash-{Guid.NewGuid():N}",
                Now,
                Now.AddDays(30));

        public string HashRefreshToken(string refreshToken) => $"hash-of-{refreshToken}";
    }

    private sealed class FakeRefreshSessionRepository : IRefreshSessionRepository
    {
        public List<RefreshSession> Sessions { get; } = [];

        public Task AddAsync(RefreshSession session, CancellationToken cancellationToken)
        {
            Sessions.Add(session);
            return Task.CompletedTask;
        }

        public Task<RefreshSession?> FindByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken) =>
            Task.FromResult(Sessions.SingleOrDefault(
                session => session.TokenHash == tokenHash));

        public Task UpdateAsync(RefreshSession session, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task RotateAsync(
            RefreshSession currentSession,
            RefreshSession replacementSession,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<RefreshSession>> ListActiveForUserAsync(
            Guid userId,
            DateTimeOffset utcNow,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<RefreshSession> items = Sessions
                .Where(session =>
                    session.UserId == userId &&
                    !session.IsRevoked &&
                    session.ExpiresAtUtc > utcNow)
                .ToArray();

            return Task.FromResult(items);
        }

        public Task<RefreshSession?> FindOwnedByIdAsync(
            Guid sessionId,
            Guid userId,
            CancellationToken cancellationToken) =>
            Task.FromResult(Sessions.SingleOrDefault(
                session => session.Id == sessionId && session.UserId == userId));

        public Task RevokeAllActiveForUserAsync(
            Guid userId,
            DateTimeOffset revokedAtUtc,
            CancellationToken cancellationToken)
        {
            foreach (var session in Sessions.Where(session =>
                session.UserId == userId && !session.IsRevoked))
            {
                session.Revoke(revokedAtUtc);
            }

            return Task.CompletedTask;
        }
    }
}
