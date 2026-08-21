using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using BusinessFinance.Api.Errors;

namespace BusinessFinance.Api.Tests.Errors;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ReturnsSafeProblemDetails()
    {
        const string sensitiveMessage = "Synthetic card value 4111111111111111";
        var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = services,
            TraceIdentifier = "test-trace-id"
        };
        context.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(
            context,
            new InvalidOperationException(sensitiveMessage),
            CancellationToken.None);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(
            context.Response.Body,
            cancellationToken: CancellationToken.None);
        var root = document.RootElement;

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(500, root.GetProperty("status").GetInt32());
        Assert.Equal(
            "An unexpected server error occurred.",
            root.GetProperty("title").GetString());
        Assert.Equal(
            "server.unexpected_error",
            root.GetProperty("code").GetString());
        Assert.Equal("test-trace-id", root.GetProperty("traceId").GetString());
        Assert.DoesNotContain(sensitiveMessage, root.GetRawText(), StringComparison.Ordinal);
    }
}
