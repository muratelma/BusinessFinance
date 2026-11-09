using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.UserAccount;
using BusinessFinance.Application.Authentication.Google;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Tests.Features.UserAccount;

/// <summary>
/// Google ile açılmış (parolasız) hesapta Hesabım (9 Kasım 2026): silme
/// Google'a yeniden girişle onaylanır, parola e-postaya giden kodla konur.
/// </summary>
public sealed class PasswordlessAccountEndpointTests
{
    private const string Email = "ayse@gmail.com";
    private const string NewPassword = "Valid-Password-123!";

    [Fact]
    public async Task Account_SaysWhetherItHasAPassword()
    {
        var (factory, _, _) = Create();
        await using var _factory = factory;
        using var client = factory.CreateClient();
        var session = await SignUpWithGoogleAsync(client);
        Authenticate(client, session.AccessToken);

        var account = await client.GetFromJsonAsync<UserAccountResponse>("/api/v1/account");

        Assert.False(account!.HasPassword);
    }

    [Fact]
    public async Task Delete_IsConfirmedWithTheLinkedGoogleAccountOnly()
    {
        var (factory, google, _) = Create();
        await using var _factory = factory;
        using var client = factory.CreateClient();
        var session = await SignUpWithGoogleAsync(client);
        Authenticate(client, session.AccessToken);
        google.Add("token-other", "sub-other", "baska@gmail.com");

        using var withoutGoogle = await DeleteAsync(
            client,
            new DeleteUserAccountRequest(null, Confirmed: true));
        using var otherAccount = await DeleteAsync(
            client,
            new DeleteUserAccountRequest(null, Confirmed: true, GoogleIdToken: "token-other"));

        Assert.Equal(HttpStatusCode.Unauthorized, withoutGoogle.StatusCode);
        await AssertCodeAsync(otherAccount, "account.reauthentication_failed");

        using var deleted = await DeleteAsync(
            client,
            new DeleteUserAccountRequest(null, Confirmed: true, GoogleIdToken: "token-ayse"));
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
    }

    [Fact]
    public async Task SetPassword_UsesTheEmailCode_AndKeepsThisDeviceSignedIn()
    {
        var (factory, _, mail) = Create();
        await using var _factory = factory;
        using var client = factory.CreateClient();
        var session = await SignUpWithGoogleAsync(client);
        Authenticate(client, session.AccessToken);

        using var wrongCode = await client.PostAsJsonAsync(
            "/api/v1/account/password",
            new ChangePasswordRequest(null, NewPassword, ResetCode: "000000"));
        Assert.Equal(HttpStatusCode.BadRequest, wrongCode.StatusCode);

        using var request = await client.PostAsJsonAsync(
            "/api/v1/auth/password-reset",
            new RequestPasswordResetRequest(Email));
        request.EnsureSuccessStatusCode();
        var code = Assert.Single(mail.Sent).Code;

        using var set = await client.PostAsJsonAsync(
            "/api/v1/account/password",
            new ChangePasswordRequest(null, NewPassword, ResetCode: code));
        Assert.Equal(HttpStatusCode.OK, set.StatusCode);
        var rotated = await set.Content.ReadFromJsonAsync<ChangePasswordApiResponse>();

        // Bu cihaz açık kalır; parolayla da girilebilir.
        using var refresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(rotated!.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("ayse", NewPassword));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    private static (BusinessFinanceApiFactory Factory, FakeGoogleTokens Google, RecordingEmailSender Mail)
        Create()
    {
        var google = new FakeGoogleTokens();
        google.Add("token-ayse", "sub-ayse", Email);
        var mail = new RecordingEmailSender();
        var factory = new BusinessFinanceApiFactory(configureServices: services =>
        {
            services.RemoveAll<IGoogleIdTokenValidator>();
            services.AddSingleton<IGoogleIdTokenValidator>(google);
            services.RemoveAll<IVerificationEmailSender>();
            services.AddSingleton<IVerificationEmailSender>(mail);
            // Gerçek silici toplu silme kullanır; bellek içi veritabanı onu
            // desteklemez. Silmenin kendisi SQL testlerinde korunur.
            services.RemoveAll<IUserAccountEraser>();
            services.AddSingleton<IUserAccountEraser, NoopEraser>();
        });
        return (factory, google, mail);
    }

    private static async Task<TokenPairResponse> SignUpWithGoogleAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/google",
            new GoogleSignInRequest("token-ayse", "ayse"));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<GoogleSignInResponse>();
        return body!.Session!;
    }

    private static Task<HttpResponseMessage> DeleteAsync(
        HttpClient client,
        DeleteUserAccountRequest body) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/v1/account")
        {
            Content = JsonContent.Create(body)
        });

    private static void Authenticate(HttpClient client, string accessToken) =>
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

    private static async Task AssertCodeAsync(HttpResponseMessage response, string code)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(code, document.RootElement.GetProperty("code").GetString());
    }

    private sealed class NoopEraser : IUserAccountEraser
    {
        public Task EraseAsync(Guid userId, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class FakeGoogleTokens : IGoogleIdTokenValidator
    {
        private readonly Dictionary<string, GoogleIdentity> _tokens = new(StringComparer.Ordinal);

        public void Add(string token, string subject, string email) =>
            _tokens[token] = new GoogleIdentity(subject, email, EmailVerified: true);

        public Task<GoogleIdentity?> ValidateAsync(string idToken, CancellationToken cancellationToken) =>
            Task.FromResult(_tokens.GetValueOrDefault(idToken));
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
