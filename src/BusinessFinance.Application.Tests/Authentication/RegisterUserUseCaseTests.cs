using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Application.Authentication.RegisterUser;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Tests.Verification;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Authentication;

public sealed class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenIdentityRegistrationSucceeds_ReturnsUser()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var service = new FakeIdentityAccountService
        {
            RegistrationResult = new IdentityRegistrationResult(
                IdentityRegistrationStatus.Succeeded,
                userId,
                "user@example.com")
        };
        var profiles = new RecordingUserProfileRepository();
        var useCase = CreateUseCase(service, profiles);

        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand("user@example.com", "Valid-Password-123!"));

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal("user@example.com", result.Value.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailExists_ReturnsGenericConflict()
    {
        var service = new FakeIdentityAccountService
        {
            RegistrationResult = new IdentityRegistrationResult(
                IdentityRegistrationStatus.DuplicateEmail,
                null,
                null)
        };
        var profiles = new RecordingUserProfileRepository();
        var useCase = CreateUseCase(service, profiles);

        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand("user@example.com", "Valid-Password-123!"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Conflict, result.Error.Type);
        Assert.Equal("authentication.registration_conflict", result.Error.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWeak_ReturnsPasswordPolicyValidation()
    {
        var service = new FakeIdentityAccountService
        {
            RegistrationResult = new IdentityRegistrationResult(
                IdentityRegistrationStatus.InvalidPassword,
                null,
                null)
        };
        var profiles = new RecordingUserProfileRepository();
        var useCase = CreateUseCase(service, profiles);

        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand("user@example.com", "weak"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrorType.Validation, result.Error.Type);
        Assert.Equal("authentication.password_policy", result.Error.Code);
    }

    [Fact]
    public async Task ExecuteAsync_ForwardsCancellationToken()
    {
        var service = new FakeIdentityAccountService();
        var profiles = new RecordingUserProfileRepository();
        var useCase = CreateUseCase(service, profiles);
        using var cancellationTokenSource = new CancellationTokenSource();

        await useCase.ExecuteAsync(
            new RegisterUserCommand("user@example.com", "Valid-Password-123!"),
            cancellationTokenSource.Token);

        Assert.Equal(cancellationTokenSource.Token, service.ReceivedCancellationToken);
    }

    private sealed class RecordingUserProfileRepository : IUserProfileRepository
    {
        private readonly List<UserProfile> _saved = [];

        public IReadOnlyList<UserProfile> Saved => _saved;

        public Task<UserProfile?> FindAsync(Guid userId, bool track, CancellationToken cancellationToken) =>
            Task.FromResult(_saved.SingleOrDefault(profile => profile.UserId == userId));

        public Task AddAsync(UserProfile profile, CancellationToken cancellationToken)
        {
            _saved.Add(profile);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    /// <summary>
    /// Kayıt artık doğrulama kodu da gönderiyor; testler kodun gidip
    /// gitmediğini görebilsin diye ikizler dışarıda kurulur.
    /// </summary>
    private static RegisterUserUseCase CreateUseCase(
        FakeIdentityAccountService service,
        RecordingUserProfileRepository profiles,
        FakeVerificationCodeService? codeService = null,
        InMemoryVerificationCodeRepository? codes = null,
        RecordingVerificationEmailSender? sender = null)
    {
        return new RegisterUserUseCase(
            service,
            profiles,
            codeService ?? new FakeVerificationCodeService(),
            codes ?? new InMemoryVerificationCodeRepository(),
            sender ?? new RecordingVerificationEmailSender(),
            new MutableTimeProvider(new DateTimeOffset(2026, 8, 27, 9, 0, 0, TimeSpan.Zero)));
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
        public IdentityRegistrationResult RegistrationResult { get; init; } = new(
            IdentityRegistrationStatus.InvalidRegistration,
            null,
            null);
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<IdentityRegistrationResult> RegisterAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(RegistrationResult);
        }

        public Task<AuthenticatedIdentity?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<AuthenticatedIdentity?> FindActiveByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
