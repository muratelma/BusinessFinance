using System.Net;

namespace BusinessFinance.Api.Tests.Security;

public sealed class CorsPolicyTests
{
    private const string AllowedOrigin = "http://localhost:65087";

    [Fact]
    public async Task Preflight_FromConfiguredLocalWebOrigin_IsAllowed()
    {
        await using var factory = new BusinessFinanceApiFactory(environment: "Development");
        using var client = factory.CreateClient();
        using var request = Preflight(AllowedOrigin);

        using var response = await client.SendAsync(request, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            AllowedOrigin,
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
    }

    [Fact]
    public async Task Preflight_FromUnconfiguredOrigin_ReceivesNoCorsPermission()
    {
        await using var factory = new BusinessFinanceApiFactory(environment: "Development");
        using var client = factory.CreateClient();
        using var request = Preflight("https://untrusted.example");

        using var response = await client.SendAsync(request, CancellationToken.None);

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    private static HttpRequestMessage Preflight(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/login");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "content-type");
        return request;
    }
}
