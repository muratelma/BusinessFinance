using System.Net;
using System.Text;
using System.Text.Json;

namespace BusinessFinance.Api.Tests.Errors;

public sealed class BadRequestContractTests
{
    [Fact]
    public async Task Register_WithMalformedJson_ReturnsInvalidFormatProblemDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();
        using var content = new StringContent(
            "{ invalid json",
            Encoding.UTF8,
            "application/json");

        using var response = await client.PostAsync(
            "/api/v1/auth/register",
            content,
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        await using var stream = await response.Content.ReadAsStreamAsync(
            CancellationToken.None);
        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: CancellationToken.None);
        Assert.Equal(
            "request.invalid_format",
            document.RootElement.GetProperty("code").GetString());
    }
}
