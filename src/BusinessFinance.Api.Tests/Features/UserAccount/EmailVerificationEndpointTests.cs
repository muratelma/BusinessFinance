using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.UserAccount;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Tests.Features.UserAccount;

public sealed class EmailVerificationEndpointTests
{
    private const string Email = "verify@example.com";
    private const string Password = "Valid-Password-123!";
    private const string NewPassword = "Another-Password-456!";

    [Fact]
    public async Task Register_SendsAVerificationCodeWithoutBlockingTheAccount()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();

        var tokens = await RegisterAndLoginAsync(client);

        // Kod gitti ama hesap kilitlenmedi: kullanıcı giriş yapabildi.
        var sent = Assert.Single(mail.Sent);
        Assert.Equal(Email, sent.Email);
        Assert.Equal(VerificationPurpose.EmailConfirmation, sent.Purpose);
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));

        Authenticate(client, tokens.AccessToken);
        using var accountResponse = await client.GetAsync(
            "/api/v1/account",
            CancellationToken.None);
        var account = await accountResponse.Content
            .ReadFromJsonAsync<UserAccountResponse>(CancellationToken.None);
        Assert.NotNull(account);
        Assert.False(account.EmailConfirmed);
    }

    [Fact]
    public async Task ConfirmEmail_WithTheEmailedCode_MarksTheAddressConfirmed()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/account/email-verification/confirm",
            new ConfirmEmailRequest(mail.Sent[0].Code),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var accountResponse = await client.GetAsync(
            "/api/v1/account",
            CancellationToken.None);
        var account = await accountResponse.Content
            .ReadFromJsonAsync<UserAccountResponse>(CancellationToken.None);
        Assert.NotNull(account);
        Assert.True(account.EmailConfirmed);
    }

    [Fact]
    public async Task ConfirmEmail_WithAWrongCode_IsRejected()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/account/email-verification/confirm",
            new ConfirmEmailRequest("000000"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.invalid_verification_code");
    }

    [Fact]
    public async Task SendVerification_TwiceInARow_IsRejectedWithoutASecondMail()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.PostAsync(
            "/api/v1/account/email-verification",
            content: null,
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.verification_code_too_soon");
        Assert.Single(mail.Sent);
    }

    [Fact]
    public async Task RequestPasswordReset_AnswersTheSameForKnownAndUnknownAddresses()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        await RegisterAndLoginAsync(client);
        mail.Sent.Clear();

        using var knownResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest(Email),
            CancellationToken.None);
        using var unknownResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest("nobody@example.com"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Accepted, knownResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, unknownResponse.StatusCode);
        Assert.Equal(
            await knownResponse.Content.ReadAsStringAsync(CancellationToken.None),
            await unknownResponse.Content.ReadAsStringAsync(CancellationToken.None));

        // Fark yalnız gerçekten giden postada.
        var sent = Assert.Single(mail.Sent);
        Assert.Equal(Email, sent.Email);
        Assert.Equal(VerificationPurpose.PasswordReset, sent.Purpose);
    }

    [Fact]
    public async Task ResetPassword_WithTheEmailedCode_SetsThePasswordAndClosesEverySession()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        mail.Sent.Clear();

        using var requestResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest(Email),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.Accepted, requestResponse.StatusCode);

        using var resetResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset/confirm",
            new ResetPasswordRequest(Email, mail.Sent[0].Code, NewPassword),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.NoContent, resetResponse.StatusCode);

        // Sıfırlama hesabı geri alıyor: eski oturumun refresh token'ı ölü.
        using var oldSession = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(tokens.RefreshToken),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.Unauthorized, oldSession.StatusCode);

        using var oldPasswordLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);
        using var newPasswordLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, NewPassword),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, oldPasswordLogin.StatusCode);
        Assert.Equal(HttpStatusCode.OK, newPasswordLogin.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithTheSameCodeTwice_FailsTheSecondTime()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();
        await RegisterAndLoginAsync(client);
        mail.Sent.Clear();

        await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest(Email),
            CancellationToken.None);
        var code = mail.Sent[0].Code;

        using var first = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset/confirm",
            new ResetPasswordRequest(Email, code, NewPassword),
            CancellationToken.None);
        using var second = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset/confirm",
            new ResetPasswordRequest(Email, code, "Third-Password-789!"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        await AssertProblemCodeAsync(second, "authentication.invalid_reset_code");
    }

    [Fact]
    public async Task ResetPassword_ForAnUnknownAddress_AnswersLikeAWrongCode()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset/confirm",
            new ResetPasswordRequest("nobody@example.com", "123456", NewPassword),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "authentication.invalid_reset_code");
    }

    [Fact]
    public async Task PasswordReset_WhenTheRateLimitIsExceeded_ReturnsTooManyRequests()
    {
        var mail = new RecordingEmailSender();
        await using var factory = CreateFactory(mail);
        using var client = factory.CreateClient();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/v1/auth/password-reset",
                new RequestPasswordResetRequest("nobody@example.com"),
                CancellationToken.None);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        using var rejected = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest("nobody@example.com"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        await AssertProblemCodeAsync(rejected, "rate_limit.exceeded");
    }

    /// <summary>
    /// Gerçek gönderici hiçbir testte kurulmaz: test koşusu ağa çıkmaz.
    /// </summary>
    private static BusinessFinanceApiFactory CreateFactory(RecordingEmailSender mail) =>
        new(configureServices: services =>
        {
            services.RemoveAll<IVerificationEmailSender>();
            services.AddSingleton<IVerificationEmailSender>(mail);
        });

    private static void Authenticate(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private static async Task<TokenPairResponse> RegisterAndLoginAsync(HttpClient client)
    {
        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password),
            CancellationToken.None);
        registerResponse.EnsureSuccessStatusCode();

        using var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);
        loginResponse.EnsureSuccessStatusCode();

        return await loginResponse.Content.ReadFromJsonAsync<TokenPairResponse>(
                   CancellationToken.None) ??
               throw new InvalidOperationException("Token response was empty.");
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
        using var document = JsonDocument.Parse(body);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private sealed class RecordingEmailSender : IVerificationEmailSender
    {
        public List<(string Email, VerificationPurpose Purpose, string Code)> Sent { get; } = [];

        public Task<EmailDeliveryStatus> SendCodeAsync(
            string email,
            VerificationPurpose purpose,
            string code,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken)
        {
            Sent.Add((email, purpose, code));
            return Task.FromResult(EmailDeliveryStatus.Sent);
        }
    }
}
