using System.Net;

namespace BusinessFinance.Api.Tests.Health;

public sealed class HealthEndpointTests
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthEndpoint_WhenDependenciesAreAvailable_ReturnsHealthy(
        string path)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(path, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "Healthy",
            await response.Content.ReadAsStringAsync(CancellationToken.None));
    }
}
