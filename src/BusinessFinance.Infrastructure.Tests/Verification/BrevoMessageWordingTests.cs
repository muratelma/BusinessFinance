using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Verification;

namespace BusinessFinance.Infrastructure.Tests.Verification;

/// <summary>
/// Postanın konusu yapılan işi söyler (9 Kasım 2026, kullanıcı bulgusu):
/// Google ile açılmış, hiç parolası olmayan hesaba "Parola sıfırlama kodunuz"
/// gidiyordu. Ağa çıkmaz; istek sahte bir işleyicide yakalanır.
/// </summary>
public sealed class BrevoMessageWordingTests
{
    [Fact]
    public async Task EmailConfirmation_SaysVerification()
    {
        var (sender, handler) = Create();

        await sender.SendCodeAsync(
            "user@example.test",
            VerificationPurpose.EmailConfirmation,
            "123456",
            DateTimeOffset.UtcNow,
            CancellationToken.None);

        Assert.Equal("E-posta doğrulama kodunuz", handler.Subject);
        Assert.Contains("E-posta adresinizi doğrulamak için", handler.Text);
    }

    [Fact]
    public async Task PasswordReset_SaysReset()
    {
        var (sender, handler) = Create();

        await sender.SendCodeAsync(
            "user@example.test",
            VerificationPurpose.PasswordReset,
            "123456",
            DateTimeOffset.UtcNow,
            CancellationToken.None);

        Assert.Equal("Parola sıfırlama kodunuz", handler.Subject);
        Assert.Contains("Parolanızı sıfırlamak için", handler.Text);
    }

    [Fact]
    public async Task PasswordSetup_SaysSetNotReset()
    {
        var (sender, handler) = Create();

        await sender.SendPasswordSetupCodeAsync(
            "user@example.test",
            "123456",
            DateTimeOffset.UtcNow,
            CancellationToken.None);

        Assert.Equal("Parola belirleme kodunuz", handler.Subject);
        Assert.Contains("Hesabınıza parola belirlemek için", handler.Text);
        Assert.DoesNotContain("sıfırla", handler.Text);
        Assert.Contains("123456", handler.Text);
    }

    private static (BrevoVerificationEmailSender Sender, CapturingHandler Handler) Create()
    {
        var options = new BrevoOptions
        {
            ApiKey = "test-key",
            SenderEmail = "sender@example.test"
        };
        var handler = new CapturingHandler();
        var sender = new BrevoVerificationEmailSender(
            new HttpClient(handler) { BaseAddress = new Uri(options.BaseUrl) },
            Options.Create(options),
            NullLogger<BrevoVerificationEmailSender>.Instance);
        return (sender, handler);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public string? Subject { get; private set; }

        public string Text { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using var document = JsonDocument.Parse(
                await request.Content!.ReadAsStringAsync(cancellationToken));
            Subject = document.RootElement.GetProperty("subject").GetString();
            Text = document.RootElement.GetProperty("textContent").GetString() ?? string.Empty;
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
