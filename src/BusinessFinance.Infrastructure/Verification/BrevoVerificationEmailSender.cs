using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Infrastructure.Verification;

public sealed class BrevoOptions
{
    public const string SectionName = "Brevo";

    /// <summary>Local user-secrets only. Never appsettings, logs or Git.</summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gönderici adresi servisin kendi tarafında doğrulanmış olmak zorundadır;
    /// doğrulanmamış adresten çıkan posta ya reddedilir ya spam'e düşer.
    /// </summary>
    public string? SenderEmail { get; init; }

    public string SenderName { get; init; } = "BusinessFinance";

    public string BaseUrl { get; init; } = "https://api.brevo.com/";

    public int TimeoutSeconds { get; init; } = 15;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(SenderEmail);
}

/// <summary>
/// Doğrulama kodunu Brevo'nun transactional e-posta ucundan gönderir.
/// </summary>
/// <remarks>
/// SDK yok, tek uç için ham <see cref="HttpClient"/> — fiş okumadaki
/// Gemini istemcisiyle aynı gerekçe.
///
/// <b>Kod hiçbir yere loglanmaz.</b> Başarısızlık durum koduyla kaydedilir,
/// gövdesiyle değil: hata gövdesi alıcının adresini taşıyabilir.
///
/// Anahtar yoksa çağrı hiç yapılmaz ve <see cref="EmailDeliveryStatus.NotConfigured"/>
/// döner. Bu bir hata değil, kapalı bir kapıdır: yerelde posta göndermeden
/// çalışmak mümkün olmalı, kayıt olan kullanıcı yüzünden uygulama patlamamalı.
/// </remarks>
internal sealed class BrevoVerificationEmailSender : IVerificationEmailSender
{
    private const string ApiKeyHeader = "api-key";
    private const string SendPath = "v3/smtp/email";

    private readonly HttpClient _client;
    private readonly BrevoOptions _options;
    private readonly ILogger<BrevoVerificationEmailSender> _logger;

    public BrevoVerificationEmailSender(
        HttpClient client,
        IOptions<BrevoOptions> options,
        ILogger<BrevoVerificationEmailSender> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailDeliveryStatus> SendCodeAsync(
        string email,
        VerificationPurpose purpose,
        string code,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken)
    {
        if (!_options.IsConfigured)
        {
            _logger.LogInformation(
                "Verification email was not sent: the mail sender is not configured.");
            return EmailDeliveryStatus.NotConfigured;
        }

        var payload = new JsonObject
        {
            ["sender"] = new JsonObject
            {
                ["name"] = _options.SenderName,
                ["email"] = _options.SenderEmail
            },
            ["to"] = new JsonArray(new JsonObject { ["email"] = email }),
            ["subject"] = Subject(purpose),
            ["textContent"] = TextBody(purpose, code)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, SendPath)
        {
            Content = new StringContent(
                payload.ToJsonString(),
                Encoding.UTF8,
                "application/json")
        };
        request.Headers.Add(ApiKeyHeader, _options.ApiKey);

        try
        {
            using var response = await _client.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return EmailDeliveryStatus.Sent;
            }

            _logger.LogWarning(
                "The mail provider rejected a verification message with status {StatusCode}.",
                (int)response.StatusCode);
            return EmailDeliveryStatus.Failed;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(
                "The mail provider could not be reached: {Reason}.",
                exception.Message);
            return EmailDeliveryStatus.Failed;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("The mail provider timed out.");
            return EmailDeliveryStatus.Failed;
        }
    }

    private static string Subject(VerificationPurpose purpose) => purpose switch
    {
        VerificationPurpose.PasswordReset => "Parola sıfırlama kodunuz",
        _ => "E-posta doğrulama kodunuz"
    };

    /// <summary>
    /// Düz metin: HTML şablonu, okunmayacak bir tasarım işi getirir ve kodu
    /// kopyalamayı zorlaştıran istemcilerde bozulur. Mesaj tek şey söyler.
    /// </summary>
    private static string TextBody(VerificationPurpose purpose, string code)
    {
        var minutes = VerificationPolicy.CodeLifetime.TotalMinutes
            .ToString("0", CultureInfo.InvariantCulture);
        var reason = purpose == VerificationPurpose.PasswordReset
            ? "Parolanızı sıfırlamak için"
            : "E-posta adresinizi doğrulamak için";

        return $"""
            {reason} aşağıdaki kodu uygulamaya girin:

            {code}

            Kod {minutes} dakika geçerlidir ve yalnız bir kez kullanılabilir.
            Bu isteği siz yapmadıysanız bu iletiyi yok sayabilirsiniz.
            """;
    }
}
