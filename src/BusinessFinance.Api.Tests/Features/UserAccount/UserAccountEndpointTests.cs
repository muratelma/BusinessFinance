using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.UserAccount;

namespace BusinessFinance.Api.Tests.Features.UserAccount;

public sealed class UserAccountEndpointTests
{
    private const string Email = "owner@example.com";
    private const string Password = "Valid-Password-123!";
    private const string NewPassword = "Another-Password-456!";

    [Fact]
    public async Task GetAccount_ReturnsTheCallersOwnEmail()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.GetAsync("/api/v1/account", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var account = await response.Content
            .ReadFromJsonAsync<UserAccountResponse>(CancellationToken.None);
        Assert.NotNull(account);
        Assert.Equal(Email, account.Email);
        Assert.Equal(tokens.UserId, account.UserId);
        Assert.Equal(1, account.ActiveSessionCount);
    }

    [Fact]
    public async Task GetAccount_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/account", CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListSessions_ReturnsOnlyTheCallersSessionsAndNoSecret()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        await LoginAsync(client, Email, Password);
        var stranger = await RegisterAndLoginAsync(client, "stranger@example.com");

        Authenticate(client, tokens.AccessToken);
        using var response = await client.GetAsync(
            "/api/v1/account/sessions",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var sessions = JsonSerializer.Deserialize<UserSessionListResponse>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(sessions);
        Assert.Equal(2, sessions.Items.Count);
        Assert.DoesNotContain(stranger.SessionId, sessions.Items.Select(item => item.SessionId));
        Assert.DoesNotContain("hash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(tokens.RefreshToken, body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RevokeSession_ClosesTheSessionAndItsRefreshTokenStopsWorking()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var first = await RegisterAndLoginAsync(client);
        var second = await LoginAsync(client, Email, Password);

        Authenticate(client, first.AccessToken);
        using var revokeResponse = await client.DeleteAsync(
            $"/api/v1/account/sessions/{second.SessionId}",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;
        using var refreshResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(second.RefreshToken),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task RevokeSession_WithAnotherUsersSession_ReturnsNotFoundAndLeavesItUsable()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var owner = await RegisterAndLoginAsync(client);
        var stranger = await RegisterAndLoginAsync(client, "stranger@example.com");

        Authenticate(client, owner.AccessToken);
        using var response = await client.DeleteAsync(
            $"/api/v1/account/sessions/{stranger.SessionId}",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.session_not_found");

        client.DefaultRequestHeaders.Authorization = null;
        using var refreshResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(stranger.RefreshToken),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ClosesEverySessionAndKeepsTheCallingDeviceSignedIn()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var first = await RegisterAndLoginAsync(client);
        var second = await LoginAsync(client, Email, Password);

        Authenticate(client, first.AccessToken);
        using var changeResponse = await client.PostAsJsonAsync(
            "/api/v1/account/password",
            new ChangePasswordRequest(Password, NewPassword),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, changeResponse.StatusCode);
        var issued = await changeResponse.Content
            .ReadFromJsonAsync<ChangePasswordApiResponse>(CancellationToken.None);
        Assert.NotNull(issued);
        Assert.NotEqual(Guid.Empty, issued.SessionId);

        client.DefaultRequestHeaders.Authorization = null;

        // Önce taze token: cevapla gelen çift çalışmaya devam eder, yani parola
        // değiştiren cihaz uygulamadan atılmaz.
        using var freshTokenResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(issued.RefreshToken),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, freshTokenResponse.StatusCode);

        // Eski refresh token'lar — isteği yapan cihazınki dâhil — reddedilir.
        // Sıra önemlidir: iptal edilmiş bir token'ı denemek yeniden kullanım
        // tespitini tetikler ve kullanıcının bütün oturumlarını kapatır.
        using var oldTokenResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(first.RefreshToken),
            CancellationToken.None);
        using var otherDeviceResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(second.RefreshToken),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, oldTokenResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, otherDeviceResponse.StatusCode);

        using var newPasswordLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, NewPassword),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, newPasswordLogin.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/account/password",
            new ChangePasswordRequest("Wrong-Password-123!", NewPassword),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.invalid_password");
    }

    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/account/password",
            new ChangePasswordRequest(Password, "weak"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "authentication.password_policy");
    }

    [Fact]
    public async Task DeleteAccount_WithoutConfirmation_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await SendDeleteAsync(
            client,
            new DeleteUserAccountRequest(Password, Confirmed: false));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.delete_not_confirmed");

        using var accountResponse = await client.GetAsync(
            "/api/v1/account",
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.OK, accountResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_WithWrongPassword_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var tokens = await RegisterAndLoginAsync(client);
        Authenticate(client, tokens.AccessToken);

        using var response = await SendDeleteAsync(
            client,
            new DeleteUserAccountRequest("Wrong-Password-123!", Confirmed: true));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertProblemCodeAsync(response, "account.invalid_password");

        using var accountResponse = await client.GetAsync(
            "/api/v1/account",
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.OK, accountResponse.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendDeleteAsync(
        HttpClient client,
        DeleteUserAccountRequest request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/account")
        {
            Content = JsonContent.Create(request)
        };

        return await client.SendAsync(message, CancellationToken.None);
    }

    private static void Authenticate(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private static async Task<TokenPairResponse> RegisterAndLoginAsync(
        HttpClient client,
        string email = Email)
    {
        var previousAuthorization = client.DefaultRequestHeaders.Authorization;
        client.DefaultRequestHeaders.Authorization = null;

        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password),
            CancellationToken.None);
        registerResponse.EnsureSuccessStatusCode();

        var tokens = await LoginAsync(client, email, Password);
        client.DefaultRequestHeaders.Authorization = previousAuthorization;
        return tokens;
    }

    private static async Task<TokenPairResponse> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        var previousAuthorization = client.DefaultRequestHeaders.Authorization;
        client.DefaultRequestHeaders.Authorization = null;

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, password),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<TokenPairResponse>(
                         CancellationToken.None) ??
                     throw new InvalidOperationException("Token response was empty.");

        client.DefaultRequestHeaders.Authorization = previousAuthorization;
        return tokens;
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
}
