using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Verification;

namespace BusinessFinance.Infrastructure.Tests.Verification;

/// <summary>
/// Brevo'ya gerçekten posta gönderen tek test.
/// </summary>
/// <remarks>
/// Gemini'nin canlı sözleşme testiyle aynı gerekçe: istemci elle yazıldı, bu
/// yüzden sessiz bir sözleşme değişikliği ancak burada kırmızıya döner. Diğer
/// bütün testler <see cref="IVerificationEmailSender"/> ikizini kullanır ve
/// koşu ağa çıkmaz.
///
/// Çalıştığında <b>gerçek bir adrese gerçek bir posta</b> gider; hedef
/// kullanıcının kendi adresidir ve ortam değişkeninden verilir. Kod da gerçek
/// üretilir ama teste yazılmaz.
/// </remarks>
public sealed class BrevoLiveContractTests
{
    [BrevoFact]
    public async Task SendCode_WithRealCredentials_IsAcceptedByTheProvider()
    {
        var options = new BrevoOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_BREVO_TEST_KEY"),
            SenderEmail = Environment.GetEnvironmentVariable("BUSINESS_FINANCE_BREVO_TEST_SENDER")
        };
        var recipient =
            Environment.GetEnvironmentVariable("BUSINESS_FINANCE_BREVO_TEST_RECIPIENT") ??
            options.SenderEmail!;

        var sender = new BrevoVerificationEmailSender(
            new HttpClient
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            },
            Options.Create(options),
            NullLogger<BrevoVerificationEmailSender>.Instance);

        var status = await sender.SendCodeAsync(
            recipient,
            VerificationPurpose.EmailConfirmation,
            new VerificationCodeService().CreateCode(),
            DateTimeOffset.UtcNow.AddMinutes(15),
            CancellationToken.None);

        Assert.Equal(EmailDeliveryStatus.Sent, status);
    }

    [Fact]
    public async Task SendCode_WithoutCredentials_ReportsNotConfiguredInsteadOfFailing()
    {
        // Anahtarsız yerel kurulum: posta gitmez ama hiçbir şey patlamaz ve
        // hiçbir HTTP çağrısı yapılmaz — HttpClient'ın adresi bile yok.
        var sender = new BrevoVerificationEmailSender(
            new HttpClient(),
            Options.Create(new BrevoOptions()),
            NullLogger<BrevoVerificationEmailSender>.Instance);

        var status = await sender.SendCodeAsync(
            "user@example.test",
            VerificationPurpose.PasswordReset,
            "123456",
            DateTimeOffset.UtcNow.AddMinutes(15),
            CancellationToken.None);

        Assert.Equal(EmailDeliveryStatus.NotConfigured, status);
    }
}

public sealed class BrevoFactAttribute : FactAttribute
{
    public BrevoFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(
                "BUSINESS_FINANCE_BREVO_TEST_KEY")) ||
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(
                "BUSINESS_FINANCE_BREVO_TEST_SENDER")))
        {
            Skip = "BUSINESS_FINANCE_BREVO_TEST_KEY/SENDER is not configured.";
        }
    }
}
