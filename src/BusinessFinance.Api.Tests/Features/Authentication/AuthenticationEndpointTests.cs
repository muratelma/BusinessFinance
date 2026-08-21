using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Authentication;

namespace BusinessFinance.Api.Tests.Features.Authentication;

public sealed class AuthenticationEndpointTests
{
    private const string Email = "user@example.com";
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task RegisterAndLogin_WithValidCredentials_ReturnsTokenPair()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.NotNull(registerResponse.Headers.Location);
        var registration = await registerResponse.Content
            .ReadFromJsonAsync<RegisterResponse>(CancellationToken.None);
        Assert.NotNull(registration);
        Assert.NotEqual(Guid.Empty, registration.UserId);
        Assert.Equal(Email, registration.Email);

        using var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var tokens = await loginResponse.Content
            .ReadFromJsonAsync<TokenPairResponse>(CancellationToken.None);
        Assert.NotNull(tokens);
        Assert.Equal(registration.UserId, tokens.UserId);
        Assert.Equal(3, tokens.AccessToken.Split('.').Length);
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.Equal(TimeSpan.Zero, tokens.AccessTokenExpiresAtUtc.Offset);
        Assert.Equal(TimeSpan.Zero, tokens.RefreshTokenExpiresAtUtc.Offset);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflictProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        await RegisterAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest("USER@example.com", Password),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(
            response,
            "authentication.registration_conflict");
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsValidationProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, "weak"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "authentication.password_policy");
    }

    [Fact]
    public async Task Login_WithWrongOrUnknownCredentials_ReturnsSameUnauthorizedError()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        await RegisterAsync(client);

        using var wrongPasswordResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, "Wrong-Password-123!"),
            CancellationToken.None);
        using var unknownUserResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("unknown@example.com", Password),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPasswordResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownUserResponse.StatusCode);
        Assert.Equal("Bearer", wrongPasswordResponse.Headers.WwwAuthenticate.Single().Scheme);
        Assert.Equal("Bearer", unknownUserResponse.Headers.WwwAuthenticate.Single().Scheme);
        await AssertProblemCodeAsync(
            wrongPasswordResponse,
            "authentication.invalid_credentials");
        await AssertProblemCodeAsync(
            unknownUserResponse,
            "authentication.invalid_credentials");
    }

    [Fact]
    public async Task Refresh_WhenRotatedTokenIsReused_RevokesReplacementSession()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var login = await RegisterAndLoginAsync(client);

        using var refreshResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(login.RefreshToken),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var replacement = await refreshResponse.Content
            .ReadFromJsonAsync<RefreshTokenResponse>(CancellationToken.None);
        Assert.NotNull(replacement);
        Assert.NotEqual(login.RefreshToken, replacement.RefreshToken);

        using var reuseResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(login.RefreshToken),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);
        await AssertProblemCodeAsync(
            reuseResponse,
            "authentication.invalid_refresh_token");

        using var replacementResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new RefreshTokenRequest(replacement.RefreshToken),
            CancellationToken.None);
        Assert.Equal(HttpStatusCode.Unauthorized, replacementResponse.StatusCode);
    }

    [Fact]
    public async Task Logout_RepeatedRequest_RemainsNoContent()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        var login = await RegisterAndLoginAsync(client);

        using var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new LogoutRequest(login.RefreshToken),
            CancellationToken.None);
        using var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new LogoutRequest(login.RefreshToken),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WhenRateLimitIsExceeded_ReturnsTooManyRequests()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        for (var attempt = 0; attempt < 10; attempt++)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new LoginRequest("unknown@example.com", Password),
                CancellationToken.None);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        using var rejectedResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("unknown@example.com", Password),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        await AssertProblemCodeAsync(rejectedResponse, "rate_limit.exceeded");
    }

    private static async Task RegisterAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password),
            CancellationToken.None);

        response.EnsureSuccessStatusCode();
    }

    private static async Task<TokenPairResponse> RegisterAndLoginAsync(HttpClient client)
    {
        await RegisterAsync(client);
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenPairResponse>(
                   CancellationToken.None) ??
               throw new InvalidOperationException("Token response was empty.");
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(
            CancellationToken.None);
        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: CancellationToken.None);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
        Assert.True(document.RootElement.TryGetProperty("traceId", out _));
    }
}
