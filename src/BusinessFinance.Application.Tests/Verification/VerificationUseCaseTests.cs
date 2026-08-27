using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Verification;

public sealed class VerificationUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private const string Email = "user@example.test";
    private static readonly DateTimeOffset Start =
        new(2026, 8, 27, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task SendVerification_SendsACodeToTheUsersOwnAddress()
    {
        var harness = new Harness();

        var result = await harness.SendVerification().ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.CodeSent);
        var sent = Assert.Single(harness.Sender.Sent);
        Assert.Equal(Email, sent.Email);
        Assert.Equal(VerificationPurpose.EmailConfirmation, sent.Purpose);

        // Kodun kendisi saklanmaz; yalnız hash'i.
        var stored = Assert.Single(harness.Codes.Codes);
        Assert.Equal(harness.CodeService.HashCode(sent.Code), stored.CodeHash);
    }

    [Fact]
    public async Task SendVerification_ForAConfirmedAddress_SendsNothing()
    {
        var harness = new Harness(emailConfirmed: true);

        var result = await harness.SendVerification().ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.AlreadyConfirmed);
        Assert.False(result.Value.CodeSent);
        Assert.Empty(harness.Sender.Sent);
    }

    [Fact]
    public async Task SendVerification_TwiceWithinTheInterval_IsRejectedWithoutASecondMail()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();

        harness.Time.Advance(TimeSpan.FromSeconds(30));
        var result = await harness.SendVerification().ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("account.verification_code_too_soon", result.Error.Code);
        Assert.Single(harness.Sender.Sent);
    }

    [Fact]
    public async Task SendVerification_AfterTheInterval_IssuesAFreshCodeAndKillsTheOldOne()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();
        var firstCode = harness.Sender.Sent[0].Code;

        harness.Time.Advance(VerificationPolicy.ResendInterval);
        await harness.SendVerification().ExecuteAsync();

        Assert.Equal(2, harness.Sender.Sent.Count);

        // Eski kod artık çalışmaz: aynı anda iki geçerli kod yaşamaz.
        var withOldCode = await harness.ConfirmEmail().ExecuteAsync(
            new ConfirmEmailCommand(firstCode));
        Assert.False(withOldCode.IsSuccess);
    }

    [Fact]
    public async Task ConfirmEmail_WithTheEmailedCode_MarksTheAddressConfirmed()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();

        var result = await harness.ConfirmEmail().ExecuteAsync(
            new ConfirmEmailCommand(harness.Sender.Sent[0].Code));

        Assert.True(result.IsSuccess);
        Assert.Equal(UserId, Assert.Single(harness.Identity.ConfirmedUserIds));
    }

    [Fact]
    public async Task ConfirmEmail_WithTheSameCodeTwice_FailsTheSecondTime()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();
        var code = harness.Sender.Sent[0].Code;

        Assert.True((await harness.ConfirmEmail().ExecuteAsync(
            new ConfirmEmailCommand(code))).IsSuccess);

        var second = await harness.ConfirmEmail().ExecuteAsync(new ConfirmEmailCommand(code));

        Assert.False(second.IsSuccess);
        Assert.Equal("account.invalid_verification_code", second.Error.Code);
        Assert.Single(harness.Identity.ConfirmedUserIds);
    }

    [Fact]
    public async Task ConfirmEmail_AfterTheCodeExpires_Fails()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();
        var code = harness.Sender.Sent[0].Code;

        harness.Time.Advance(VerificationPolicy.CodeLifetime);
        var result = await harness.ConfirmEmail().ExecuteAsync(new ConfirmEmailCommand(code));

        Assert.False(result.IsSuccess);
        Assert.Equal("account.invalid_verification_code", result.Error.Code);
        Assert.Empty(harness.Identity.ConfirmedUserIds);
    }

    [Fact]
    public async Task ConfirmEmail_AfterFiveWrongGuesses_KillsTheCode()
    {
        var harness = new Harness();
        await harness.SendVerification().ExecuteAsync();
        var code = harness.Sender.Sent[0].Code;

        for (var attempt = 0; attempt < VerificationCode.MaximumFailedAttempts; attempt++)
        {
            var wrong = await harness.ConfirmEmail().ExecuteAsync(
                new ConfirmEmailCommand("000000"));
            Assert.False(wrong.IsSuccess);
        }

        // Altı haneli bir kodu tahmin etmeye çalışan için kapı burada kapanır:
        // doğru kod bile artık kabul edilmez, kullanıcı yenisini ister.
        var withRightCode = await harness.ConfirmEmail().ExecuteAsync(
            new ConfirmEmailCommand(code));

        Assert.False(withRightCode.IsSuccess);
        Assert.Empty(harness.Identity.ConfirmedUserIds);
    }

    [Fact]
    public async Task RequestPasswordReset_ForAnUnknownAddress_LooksExactlyLikeAKnownOne()
    {
        var harness = new Harness();

        var known = await harness.RequestReset().ExecuteAsync(
            new RequestPasswordResetCommand(Email));
        var unknown = await harness.RequestReset().ExecuteAsync(
            new RequestPasswordResetCommand("nobody@example.test"));

        Assert.True(known.IsSuccess);
        Assert.True(unknown.IsSuccess);

        // Cevaplar aynı; fark yalnız gerçekten giden postada.
        var sent = Assert.Single(harness.Sender.Sent);
        Assert.Equal(Email, sent.Email);
        Assert.Equal(VerificationPurpose.PasswordReset, sent.Purpose);
    }

    [Fact]
    public async Task RequestPasswordReset_WithinTheInterval_StaysSilentInsteadOfSayingWait()
    {
        var harness = new Harness();
        await harness.RequestReset().ExecuteAsync(new RequestPasswordResetCommand(Email));

        harness.Time.Advance(TimeSpan.FromSeconds(10));
        var result = await harness.RequestReset().ExecuteAsync(
            new RequestPasswordResetCommand(Email));

        // "Biraz bekleyin" demek, adresin kayıtlı olduğunu söylerdi.
        Assert.True(result.IsSuccess);
        Assert.Single(harness.Sender.Sent);
    }

    [Fact]
    public async Task ResetPassword_WithTheEmailedCode_SetsThePasswordAndClosesEverySession()
    {
        var harness = new Harness();
        harness.Sessions.Sessions.Add(CreateSession());
        await harness.RequestReset().ExecuteAsync(new RequestPasswordResetCommand(Email));

        var result = await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand(Email, harness.Sender.Sent[0].Code, "Another-Password-456!"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Another-Password-456!", harness.Identity.LastPasswordSet);
        Assert.All(harness.Sessions.Sessions, session => Assert.True(session.IsRevoked));
    }

    [Fact]
    public async Task ResetPassword_WithAWrongCode_ChangesNothing()
    {
        var harness = new Harness();
        harness.Sessions.Sessions.Add(CreateSession());
        await harness.RequestReset().ExecuteAsync(new RequestPasswordResetCommand(Email));

        var result = await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand(Email, "000000", "Another-Password-456!"));

        Assert.False(result.IsSuccess);
        Assert.Equal("authentication.invalid_reset_code", result.Error.Code);
        Assert.Null(harness.Identity.LastPasswordSet);
        Assert.All(harness.Sessions.Sessions, session => Assert.False(session.IsRevoked));
    }

    [Fact]
    public async Task ResetPassword_ForAnUnknownAddress_AnswersLikeAWrongCode()
    {
        var harness = new Harness();

        var result = await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand("nobody@example.test", "123456", "Another-Password-456!"));

        Assert.False(result.IsSuccess);
        Assert.Equal("authentication.invalid_reset_code", result.Error.Code);
    }

    [Fact]
    public async Task ResetPassword_WithTheSameCodeTwice_FailsTheSecondTime()
    {
        var harness = new Harness();
        await harness.RequestReset().ExecuteAsync(new RequestPasswordResetCommand(Email));
        var code = harness.Sender.Sent[0].Code;

        Assert.True((await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand(Email, code, "Another-Password-456!"))).IsSuccess);

        var second = await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand(Email, code, "Third-Password-789!"));

        Assert.False(second.IsSuccess);
        Assert.Equal("Another-Password-456!", harness.Identity.LastPasswordSet);
    }

    [Fact]
    public async Task ResetPassword_WithAWeakPassword_SaysSoInsteadOfHidingIt()
    {
        var harness = new Harness();
        harness.Identity.PasswordChangeOutcome = PasswordChangeStatus.PasswordPolicy;
        await harness.RequestReset().ExecuteAsync(new RequestPasswordResetCommand(Email));

        var result = await harness.ResetPassword().ExecuteAsync(
            new ResetPasswordCommand(Email, harness.Sender.Sent[0].Code, "weak"));

        Assert.False(result.IsSuccess);
        Assert.Equal("authentication.password_policy", result.Error.Code);
    }

    private static RefreshSession CreateSession() => new(
        Guid.NewGuid(),
        UserId,
        $"hash-{Guid.NewGuid():N}",
        Start.AddDays(-1),
        Start.AddDays(29));

    private sealed class Harness
    {
        public FakeVerificationCodeService CodeService { get; } = new();
        public InMemoryVerificationCodeRepository Codes { get; } = new();
        public RecordingVerificationEmailSender Sender { get; } = new();
        public MutableTimeProvider Time { get; } = new(Start);
        public FakeSessionRepository Sessions { get; } = new();

        // Kimlik ikizi baştan kurulur ve bütün use case'ler **aynı** örneği
        // paylaşır: doğrulanmış e-posta ve yazılan parola gibi durum, akışlar
        // arasında taşınmak zorunda.
        public FakeIdentity Identity { get; }

        public Harness(bool emailConfirmed = false)
        {
            Identity = new FakeIdentity { EmailConfirmed = emailConfirmed };
        }

        public SendEmailVerificationUseCase SendVerification() => new(
            new FakeCurrentUser(UserId),
            Identity,
            CodeService,
            Codes,
            Sender,
            Time);

        public ConfirmEmailUseCase ConfirmEmail() => new(
            new FakeCurrentUser(UserId),
            Identity,
            CodeService,
            Codes,
            Time);

        public RequestPasswordResetUseCase RequestReset() => new(
            Identity,
            CodeService,
            Codes,
            Sender,
            Time);

        public ResetPasswordUseCase ResetPassword() => new(
            Identity,
            CodeService,
            Codes,
            Sessions,
            Time);

    }

    private sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeIdentity : IIdentityAccountService
    {
        public bool EmailConfirmed { get; init; }
        public List<Guid> ConfirmedUserIds { get; } = [];
        public string? LastPasswordSet { get; private set; }

        public PasswordChangeStatus PasswordChangeOutcome { get; set; } =
            PasswordChangeStatus.Succeeded;

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
            Task.FromResult<AuthenticatedIdentity?>(new AuthenticatedIdentity(userId, Email));

        public Task<UserAccountProfile?> FindAccountAsync(
            Guid userId,
            CancellationToken cancellationToken) =>
            Task.FromResult<UserAccountProfile?>(new UserAccountProfile(
                userId,
                Email,
                ConfirmedUserIds.Contains(userId) || EmailConfirmed,
                Start.AddDays(-30)));

        public Task<bool> VerifyPasswordAsync(
            Guid userId,
            string password,
            CancellationToken cancellationToken) =>
            Task.FromResult(true);

        public Task<PasswordChangeStatus> ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Guid?> FindActiveUserIdByEmailAsync(
            string email,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                string.Equals(email, Email, StringComparison.OrdinalIgnoreCase)
                    ? UserId
                    : (Guid?)null);

        public Task MarkEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken)
        {
            ConfirmedUserIds.Add(userId);
            return Task.CompletedTask;
        }

        public Task<PasswordChangeStatus> SetPasswordAsync(
            Guid userId,
            string newPassword,
            CancellationToken cancellationToken)
        {
            if (PasswordChangeOutcome == PasswordChangeStatus.Succeeded)
            {
                LastPasswordSet = newPassword;
            }

            return Task.FromResult(PasswordChangeOutcome);
        }

        public Task DeleteAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class FakeSessionRepository : IRefreshSessionRepository
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
            throw new NotSupportedException();

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
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<RefreshSession?> FindOwnedByIdAsync(
            Guid sessionId,
            Guid userId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

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
