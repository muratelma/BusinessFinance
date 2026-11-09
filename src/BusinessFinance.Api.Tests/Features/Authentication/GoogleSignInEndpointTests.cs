using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Application.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests.Features.Authentication;

/// <summary>
/// Google ile giriş (9 Kasım 2026 kararları): hesap Devam et'te açılır, aynı
/// e-postalı hesap bağlanır, doğrulanmamış hesapta parola düşer.
/// </summary>
public sealed class GoogleSignInEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task FirstSignIn_WritesNothingAndSuggestsAUsername()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        await RegisterAsync(client, "ali@outlook.com", "ali");
        google.Add("token-ali", "sub-ali", "ali@gmail.com", verified: true);

        var first = await PostAsync(client, new GoogleSignInRequest("token-ali"));

        Assert.Equal("signup-required", first.Status);
        Assert.Null(first.Session);
        Assert.Equal("ali@gmail.com", first.Signup!.Email);
        Assert.Equal("ali2", first.Signup.SuggestedUsername);

        // Hiçbir şey yazılmadı: ikinci istek de aynı cevabı verir.
        var again = await PostAsync(client, new GoogleSignInRequest("token-ali"));
        Assert.Equal("signup-required", again.Status);
    }

    [Fact]
    public async Task Continue_CreatesThePasswordlessAccount_AndNextSignInIsDirect()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        google.Add("token-ayse", "sub-ayse", "ayse@gmail.com", verified: true);

        var created = await PostAsync(
            client,
            new GoogleSignInRequest("token-ayse", "Ayse.K", HasBusiness: true));

        Assert.Equal("signed-in", created.Status);
        Assert.Equal("ayse@gmail.com", created.Session!.Email);
        Assert.Null(created.Signup);

        var next = await PostAsync(client, new GoogleSignInRequest("token-ayse"));
        Assert.Equal("signed-in", next.Status);
        Assert.Equal(created.Session.UserId, next.Session!.UserId);

        // Hesap parolasızdır: parolayla girilemez ama kullanıcı adı ona aittir.
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("ayse.k", Password));
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        var availability = await client.GetFromJsonAsync<UsernameAvailabilityResponse>(
            "/api/v1/auth/username-availability?username=ayse.k");
        Assert.False(availability!.Available);
    }

    [Fact]
    public async Task Continue_WithTakenUsername_ReturnsUsernameTaken()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        await RegisterAsync(client, "veli@outlook.com", "veli");
        google.Add("token-veli", "sub-veli", "veli@gmail.com", verified: true);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/google",
            new GoogleSignInRequest("token-veli", "veli"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertCodeAsync(response, "authentication.username_taken");
    }

    [Fact]
    public async Task ExistingVerifiedAccount_IsLinkedAndKeepsItsPassword()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        var userId = await RegisterAsync(client, "can@gmail.com", "can");
        await ConfirmEmailAsync(factory, userId);
        google.Add("token-can", "sub-can", "can@gmail.com", verified: true);

        var linked = await PostAsync(client, new GoogleSignInRequest("token-can"));

        Assert.Equal("signed-in", linked.Status);
        Assert.Equal(userId, linked.Session!.UserId);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("can", Password));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task ExistingUnverifiedAccount_GoesToTheGoogleOwner_PasswordAndSessionsDrop()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        var userId = await RegisterAsync(client, "deniz@gmail.com", "deniz");
        using var oldLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("deniz", Password));
        var oldSession = await oldLogin.Content.ReadFromJsonAsync<TokenPairResponse>();
        google.Add("token-deniz", "sub-deniz", "deniz@gmail.com", verified: true);

        var linked = await PostAsync(client, new GoogleSignInRequest("token-deniz"));

        Assert.Equal("signed-in", linked.Status);
        Assert.Equal(userId, linked.Session!.UserId);

        using var passwordLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("deniz", Password));
        Assert.Equal(HttpStatusCode.Unauthorized, passwordLogin.StatusCode);

        using var newRefresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(linked.Session.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, newRefresh.StatusCode);

        // Son sırada: kapatılmış oturumun belirteci "yeniden kullanım" sayılır
        // ve kullanıcının bütün oturumlarını kapatır.
        using var oldRefresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(oldSession!.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, oldRefresh.StatusCode);
    }

    [Fact]
    public async Task ExistingEmail_WithoutGoogleVerification_IsNotLinked()
    {
        var google = new FakeGoogleTokens();
        await using var factory = CreateFactory(google);
        using var client = factory.CreateClient();
        await RegisterAsync(client, "ece@firma.com", "ece");
        google.Add("token-ece", "sub-ece", "ece@firma.com", verified: false);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/google",
            new GoogleSignInRequest("token-ece"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertCodeAsync(response, "authentication.google_email_unverified");
    }

    [Fact]
    public async Task InvalidToken_ReturnsUnauthorized()
    {
        await using var factory = CreateFactory(new FakeGoogleTokens());
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/google",
            new GoogleSignInRequest("forged"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertCodeAsync(response, "authentication.google_invalid_token");
    }

    private static BusinessFinanceApiFactory CreateFactory(FakeGoogleTokens google) =>
        new(configureServices: services =>
        {
            services.RemoveAll<IGoogleIdTokenValidator>();
            services.AddSingleton<IGoogleIdTokenValidator>(google);
        });

    private static async Task<GoogleSignInResponse> PostAsync(
        HttpClient client,
        GoogleSignInRequest request)
    {
        using var response = await client.PostAsJsonAsync("/api/v1/auth/google", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<GoogleSignInResponse>())!;
    }

    private static async Task<Guid> RegisterAsync(HttpClient client, string email, string username)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, Username: username));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!.UserId;
    }

    private static async Task ConfirmEmailAsync(BusinessFinanceApiFactory factory, Guid userId)
    {
        using var scope = factory.Services.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<BusinessFinance.Application.Authentication.IIdentityAccountService>()
            .MarkEmailConfirmedAsync(userId, CancellationToken.None);
    }

    private static async Task AssertCodeAsync(HttpResponseMessage response, string code)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(code, document.RootElement.GetProperty("code").GetString());
    }

    private sealed class FakeGoogleTokens : IGoogleIdTokenValidator
    {
        private readonly Dictionary<string, GoogleIdentity> _tokens = new(StringComparer.Ordinal);

        public void Add(string token, string subject, string email, bool verified) =>
            _tokens[token] = new GoogleIdentity(subject, email, verified);

        public Task<GoogleIdentity?> ValidateAsync(string idToken, CancellationToken cancellationToken) =>
            Task.FromResult(_tokens.GetValueOrDefault(idToken));
    }
}
