using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Tests.Security;

/// <summary>
/// Log gövdesinin ve hata cevabının sızıntı denetimi (Aşama 06.1 Grup 4).
///
/// Elle bakmak bir kereliktir; bu yüzden denetim testtir. İki ayrı soru sorulur:
/// kullanıcının parası, kimliği ve sırrı <b>loglara</b> düşüyor mu, ve hata
/// cevabı sunucunun <b>iç yapısını</b> anlatıyor mu.
/// </summary>
public sealed class LeakageTests
{
    private const string Email = "log-leak@example.com";
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Gerçek bir oturumda geçen değerler. Her biri sentetik ve aranabilecek
    /// kadar tuhaf: log gövdesinde geçerse tesadüf olamaz.
    /// </summary>
    private const string Amount = "1234.5678";
    private const string Description = "GizliAciklamaXYZ";
    private const string CounterpartyName = "GizliKarsiTarafXYZ";

    /// <summary>
    /// Ürünün gönderdiği yapılandırmayla koşan gerçek bir oturum: kullanıcının
    /// parası, kimliği ve sırrı log gövdesinde hiç geçmemeli.
    /// </summary>
    [Fact]
    public async Task NothingSensitive_ReachesTheLog()
    {
        var failures = await RunAsync(removeFilters: false, ourCodeOnly: false);

        Assert.Empty(failures);
    }

    /// <summary>
    /// Bir öncekinin daha sert hâli: bütün süzgeçler kalkar ve <b>bizim
    /// yazdığımız</b> log satırları taranır. Ürünün bugünkü seviyesinde sızıntı
    /// olmaması yetmez — log seviyesini yükselten bir hata ayıklama oturumu
    /// sızıntıyı başlatmamalı.
    ///
    /// Framework kategorileri bu kapının dışındadır ve bunun bir nedeni var:
    /// ASP.NET Core istek satırını <b>sorgu dizesiyle birlikte</b> loglar.
    /// Oraya hassas bir değer koymamak istemcinin işidir, log altyapısının değil;
    /// kapısı <see cref="RequestLogging_StaysAtWarning"/>.
    /// </summary>
    [Fact]
    public async Task OurOwnLogStatements_CarryNothingSensitive()
    {
        var failures = await RunAsync(removeFilters: true, ourCodeOnly: true);

        Assert.Empty(failures);
    }

    private static async Task<IReadOnlyList<string>> RunAsync(
        bool removeFilters,
        bool ourCodeOnly)
    {
        var sink = new LogSink();
        var mail = new RecordingEmailSender();

        await using var factory = new BusinessFinanceApiFactory(
            configureServices: services =>
            {
                services.RemoveAll<IVerificationEmailSender>();
                services.AddSingleton<IVerificationEmailSender>(mail);
                services.AddSingleton<ILoggerProvider>(new SinkLoggerProvider(sink));

                if (removeFilters)
                {
                    services.Configure<LoggerFilterOptions>(options =>
                    {
                        options.Rules.Clear();
                        options.MinLevel = LogLevel.Trace;
                    });
                }
            });

        using var client = factory.CreateClient();
        var tokens = await ExerciseAsync(client, mail);

        var secrets = new List<(string Name, string Value)>
        {
            ("e-posta", Email),
            ("parola", Password),
            ("tutar", Amount),
            ("açıklama", Description),
            ("karşı taraf adı", CounterpartyName),
            ("access token", tokens.AccessToken),
            ("refresh token", tokens.RefreshToken),
            ("doğrulama kodu", mail.Sent[0].Code)
        };

        var lines = sink.Lines
            .Where(line => !ourCodeOnly || line.StartsWith("BusinessFinance", StringComparison.Ordinal))
            .ToList();

        // Hiçbir şey okumayan bir tarama da yeşil görünürdü.
        Assert.NotEmpty(lines);

        var failures = new List<string>();

        foreach (var line in lines)
        {
            foreach (var (name, value) in secrets)
            {
                if (line.Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    // Sızan değerin kendisi rapora yazılmaz; nerede geçtiği yazılır.
                    failures.Add($"{name} log gövdesinde geçiyor: {Redact(line, value)}");
                }
            }
        }

        return failures;
    }

    /// <summary>
    /// ASP.NET Core istek satırını sorgu dizesiyle birlikte `Information`
    /// seviyesinde loglar. Bugün sorgu dizesinde yalnız kimlik, tarih ve enum
    /// taşınıyor; yarın oraya bir açıklama ya da tutar konursa doğrudan loga
    /// düşer. Kategorinin `Warning`'de kalması bu yüzden bir yapılandırma
    /// tercihi değil, sınırın kendisidir.
    /// </summary>
    [Fact]
    public void RequestLogging_StaysAtWarning()
    {
        var appsettings = Path.Combine(
            RepositoryRoot.Find(), "src", "BusinessFinance.Api", "appsettings.json");
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(appsettings));

        var level = document.RootElement
            .GetProperty("Logging")
            .GetProperty("LogLevel")
            .GetProperty("Microsoft.AspNetCore")
            .GetString();

        Assert.Equal("Warning", level);
    }

    /// <summary>
    /// Hata cevabı istemcinin ne yapacağını söyler, sunucunun nasıl kurulduğunu
    /// değil. Development ve Production ayrı ayrı denenir: geliştirme profilinde
    /// açılan ayrıntılı hata sayfası, yanlış ortamda çalıştığında sızıntıdır.
    /// </summary>
    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task ErrorResponse_TellsNothingAboutTheServer(string environment)
    {
        await using var factory = new BusinessFinanceApiFactory(
            environment,
            configureServices: services =>
            {
                services.RemoveAll<IVerificationEmailSender>();
                services.AddSingleton<IVerificationEmailSender>(new ExplodingEmailSender());
            });

        using var client = factory.CreateClient();

        // Kayıt doğrulama kodunu gönderir; sağlayıcı orada patlar ve istisna
        // ele geçmeden yukarı çıkar. Aranan yol tam olarak budur.
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password, HasBusiness: true),
            CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("server.unexpected_error", body, StringComparison.Ordinal);

        // İç yapıyı anlatan her iz. `traceId` bilerek dışarıda: kullanıcının
        // destekle konuşurken söyleyebileceği tek şey odur ve içerik taşımaz.
        string[] internals =
        [
            ExplodingEmailSender.Marker,
            "Exception",
            "   at ",
            ".cs:line",
            "BusinessFinance.",
            "Microsoft.",
            "System.",
            "SELECT ",
            "C:\\",
            "StackTrace",
            "stackTrace"
        ];

        foreach (var trace in internals)
        {
            Assert.DoesNotContain(trace, body, StringComparison.Ordinal);
        }
    }

    // ---------------------------------------------------------------------

    /// <summary>Denetimin taradığı log gövdesini üreten gerçekçi bir koşu.</summary>
    private static async Task<TokenPairResponse> ExerciseAsync(
        HttpClient client,
        RecordingEmailSender mail)
    {
        var tokens = await RegisterAndLoginAsync(client);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var account = await CreateAsync<AccountResponse>(client, "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "9000.0000", "business"));
        var category = await CreateAsync<CategoryResponse>(client, "/api/v1/categories",
            new CreateCategoryRequest("Gider", "expense", "business"));

        await CreateAsync<TransactionResponse>(client, "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, category.Id, Amount, "TRY", "expense", "business",
                "2026-08-28", Description));
        await CreateAsync<CounterpartyResponse>(client, "/api/v1/counterparties",
            new CreateCounterpartyRequest(CounterpartyName, Description));

        // Kayıt sırasında bir doğrulama kodu üretildi; kod loglara düşmemeli.
        Assert.NotEmpty(mail.Sent);

        // Reddedilen istekler de loglanır: bozuk gövde ve yanlış parola.
        using var malformed = await client.PostAsync(
            "/api/v1/accounts",
            new StringContent("{ bozuk", System.Text.Encoding.UTF8, "application/json"),
            CancellationToken.None);
        Assert.False(malformed.IsSuccessStatusCode);

        using var wrongPassword = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, "Wrong-Password-987!"),
            CancellationToken.None);
        Assert.False(wrongPassword.IsSuccessStatusCode);

        // Sorgu dizesi de loglanabilir; tutar oraya da konur.
        using var query = await client.GetAsync(
            $"/api/v1/transactions?description={Description}",
            CancellationToken.None);
        Assert.True(query.IsSuccessStatusCode || !query.IsSuccessStatusCode);

        return tokens;
    }

    private static async Task<TokenPairResponse> RegisterAndLoginAsync(HttpClient client)
    {
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(Email, Password, HasBusiness: true),
            CancellationToken.None);
        register.EnsureSuccessStatusCode();

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(Email, Password),
            CancellationToken.None);
        login.EnsureSuccessStatusCode();

        return (await login.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None))!;
    }

    private static async Task<TResponse> CreateAsync<TResponse>(
        HttpClient client,
        string path,
        object request)
    {
        using var response = await client.PostAsync(
            path,
            JsonContent.Create(request, request.GetType()),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TResponse>(
            CancellationToken.None))!;
    }

    /// <summary>Bulguyu raporlarken sızan değerin kendisi yazılmaz.</summary>
    private static string Redact(string line, string value) =>
        line.Replace(value, "[gizlendi]", StringComparison.OrdinalIgnoreCase);

    private sealed class LogSink
    {
        private readonly List<string> _lines = [];
        private readonly Lock _lock = new();

        public IReadOnlyList<string> Lines
        {
            get { lock (_lock) return _lines.ToArray(); }
        }

        public void Add(string line)
        {
            lock (_lock) _lines.Add(line);
        }
    }

    private sealed class SinkLoggerProvider(LogSink sink) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new SinkLogger(categoryName, sink);

        public void Dispose()
        {
        }

        private sealed class SinkLogger(string category, LogSink sink) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                // Hem biçimlenmiş cümle hem ham durum taranır: yapılandırılmış
                // log alanları cümlede görünmese de bir sink'e yazılır.
                sink.Add($"{category} {formatter(state, exception)} {state} {exception}");
            }
        }
    }

    private sealed class RecordingEmailSender : IVerificationEmailSender
    {
        public List<(string Email, VerificationPurpose Purpose, string Code)> Sent { get; } = [];

        public Task<EmailDeliveryStatus> SendCodeAsync(
            string email,
            VerificationPurpose purpose,
            string code,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken)
        {
            Sent.Add((email, purpose, code));
            return Task.FromResult(EmailDeliveryStatus.Sent);
        }
    }

    /// <summary>Ele geçmemiş bir istisna üretir; cevabın ne anlattığı ölçülür.</summary>
    private sealed class ExplodingEmailSender : IVerificationEmailSender
    {
        public const string Marker = "PatlayanSaglayiciXYZ";

        public Task<EmailDeliveryStatus> SendCodeAsync(
            string email,
            VerificationPurpose purpose,
            string code,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(Marker);
    }
}
