using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.DayCloses;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests.Features.Debts;

/// <summary>
/// Yükümlülük ile borç planının tek borç modelinde birleşmesinin (ADR 0021,
/// dalga 1) "önce/sonra" kanıtı.
/// </summary>
/// <remarks>
/// <para>
/// Sabit saatle zengin bir sentetik veri kurulur: açık, ödenmiş, kartla tahsil
/// edilip yatışa bağlanmış, gün sonunda sayılmış ve iptal edilmiş
/// yükümlülükler; nakit, faizli, gelir ve gider kaynaklı ve açılışı bilinmeyen
/// borç planları; aynı kişide cari hareket. Sonra yükümlülüğe ve borca dokunan
/// bütün okumalar tek bir JSON'da toplanır.
/// </para>
/// <para>
/// Referans dosya birleşmeden <b>önce</b> bugünkü kodla üretildi
/// (<c>Golden/debt-merge-baseline.json</c>). Birleşmeden sonra aynı istekler
/// aynı cevabı vermelidir: dalga 1'de ekran değişmez. Kimlikler adlarla,
/// adı olmayanlar ilk görünme sırasıyla değiştirilir; saat sabit olduğu için
/// zaman damgaları olduğu gibi karşılaştırılır.
/// </para>
/// <para>
/// Referansı yeniden üretmek için <c>BUSINESS_FINANCE_UPDATE_GOLDEN=1</c>.
/// Bunu yalnız davranışın bilerek değiştiği bir adımda yapın ve farkı
/// inceleyin; birleşme adımında yapılmaz.
/// </para>
/// </remarks>
public sealed partial class DebtMergeBaselineTests
{
    private const string Password = "Valid-Password-123!";
    private const string Today = "2026-08-20";

    /// <remarks>
    /// Gerçek SQL Server üzerinde koşar: kişiye göre süzülen akış ve birleşik
    /// okumalar bellekteki sağlayıcıda çevrilemiyor, kanıt da SQL'de anlamlı.
    /// </remarks>
    [SqlServerFact]
    public async Task EveryDebtAndObligationRead_MatchesTheBaseline()
    {
        var clock = new TestClock(new DateTimeOffset(2026, 8, 20, 9, 0, 0, TimeSpan.Zero));
        await using var factory = new BusinessFinanceApiFactory(
            useConfiguredSqlServer: true,
            configureServices: services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(clock);
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options => options.TokenValidationParameters.ValidateLifetime = false);
            });
        var email = $"debt-merge-{Guid.NewGuid():N}@example.test";
        using var client = await RegisterAsync(factory, email);
        var scenario = new Scenario(client, clock, factory, email);

        await scenario.SeedAsync();
        var snapshot = await scenario.ReadAllAsync();
        var normalized = scenario.Normalize(snapshot);

        var text = normalized.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        var goldenPath = GoldenPath("debt-merge-baseline.json");
        if (Environment.GetEnvironmentVariable("BUSINESS_FINANCE_UPDATE_GOLDEN") == "1" ||
            !File.Exists(goldenPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(goldenPath)!);
            await File.WriteAllTextAsync(goldenPath, text + "\n");
            await File.WriteAllBytesAsync(
                GoldenPath("debt-merge-backup-v12.json"), await scenario.BackupAsync());
            return;
        }

        var expected = JsonNode.Parse(await File.ReadAllTextAsync(goldenPath))!;
        var difference = FirstDifference("$", expected, normalized);
        if (difference is not null)
        {
            await File.WriteAllTextAsync(GoldenPath("debt-merge-baseline.actual.json"), text + "\n");
        }

        Assert.True(difference is null, difference);
    }

    /// <summary>
    /// Birleşmeden önceki kodla alınmış v12 yedeği yeni modele kayıpsız geri
    /// yüklenir: aynı senaryoyu bugünkü kodla kuran kullanıcıyla yedeği geri
    /// yüklenen kullanıcı, kimlikten bağımsız her okumada aynı cevabı alır.
    /// </summary>
    /// <remarks>
    /// Geri yükleme kimlikleri yeniler; bu yüzden karşılaştırmada her kimlik
    /// <c>&lt;id&gt;</c> olur ve kimliğe bağlı ayrıntı okumaları dışarıda kalır
    /// (onları birinci test karşılaştırır).
    /// </remarks>
    [SqlServerFact]
    public async Task BackupTakenBeforeTheMerge_RestoresIntoTheSameReads()
    {
        var clock = new TestClock(new DateTimeOffset(2026, 8, 20, 9, 0, 0, TimeSpan.Zero));
        await using var factory = new BusinessFinanceApiFactory(
            useConfiguredSqlServer: true,
            configureServices: services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(clock);
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options => options.TokenValidationParameters.ValidateLifetime = false);
            });
        var sourceEmail = $"debt-merge-source-{Guid.NewGuid():N}@example.test";
        using var source = await RegisterAsync(factory, sourceEmail);
        await new Scenario(source, clock, factory, sourceEmail).SeedAsync();
        var expected = await ReadWithoutIdentitiesAsync(source);

        using var target = await RegisterAsync(factory, $"debt-merge-target-{Guid.NewGuid():N}@example.test");
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(await File.ReadAllBytesAsync(GoldenPath("debt-merge-backup-v12.json")));
        file.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.business-finance.backup+json");
        form.Add(file, "file", "debt-merge-backup-v12.json");
        using var restore = await target.PostAsync("/api/v1/backups/restore", form);
        Assert.True(restore.IsSuccessStatusCode, await restore.Content.ReadAsStringAsync());
        var actual = await ReadWithoutIdentitiesAsync(target);

        // Karşılaştırma boş listelerde geçmesin: senaryonun yedi yükümlülüğü ve
        // beş borcu iki tarafta da görünür.
        Assert.Equal(7, actual[$"/api/v1/obligations?asOfDate={Today}"]!["items"]!.AsArray().Count);
        Assert.Equal(5, actual[$"/api/v1/debts?asOfDate={Today}"]!["items"]!.AsArray().Count);
        var difference = FirstDifference("$", expected, actual);
        Assert.True(difference is null, difference);
    }

    private static async Task<JsonObject> ReadWithoutIdentitiesAsync(HttpClient client)
    {
        var reads = new JsonObject();
        foreach (var path in new[]
                 {
                     $"/api/v1/obligations?asOfDate={Today}",
                     $"/api/v1/debts?asOfDate={Today}",
                     $"/api/v1/counterparties?asOfDate={Today}",
                     "/api/v1/financial-activities?pageSize=100",
                     "/api/v1/financial-activities?pageSize=100&dateFrom=2026-07-01&dateTo=2026-07-31",
                     $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=90",
                     $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=90&scope=business",
                     $"/api/v1/upcoming-payments?asOfDate={Today}&daysAhead=90",
                     "/api/v1/reports/monthly?year=2026&month=7",
                     "/api/v1/reports/monthly?year=2026&month=8",
                     "/api/v1/reports/monthly?year=2026&month=8&scope=business",
                     "/api/v1/category-summary?year=2026&month=8",
                     "/api/v1/category-summary?year=2026&month=8&direction=income",
                     $"/api/v1/reports/advanced?year=2026&month=8&asOfDate={Today}&daysAhead=90",
                     "/api/v1/dashboard?year=2026&month=8",
                     "/api/v1/accounts",
                     "/api/v1/accounts/overview",
                     "/api/v1/pos-settlements",
                     $"/api/v1/day-closes/day?date={Today}",
                     "/api/v1/exports/counterparty-ledger.csv",
                 })
        {
            using var response = await client.GetAsync(path);
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"{path}: {(int)response.StatusCode} {body}");
            var withoutIdentities = GuidInText().Replace(body, "<id>");
            reads[path] = path.EndsWith(".csv", StringComparison.Ordinal)
                ? JsonValue.Create(withoutIdentities)
                : JsonNode.Parse(withoutIdentities);
        }

        return reads;
    }

    internal static string GoldenPath(string name, [CallerFilePath] string source = "") =>
        Path.Combine(Path.GetDirectoryName(source)!, "Golden", name);

    internal static string? FirstDifference(string path, JsonNode? expected, JsonNode? actual)
    {
        switch (expected, actual)
        {
            case (null, null):
                return null;
            case (JsonObject left, JsonObject right):
                foreach (var name in left.Select(item => item.Key).Union(right.Select(item => item.Key)))
                {
                    if (!left.ContainsKey(name)) return $"{path}.{name}: unexpected in actual";
                    if (!right.ContainsKey(name)) return $"{path}.{name}: missing in actual";
                    var inner = FirstDifference($"{path}.{name}", left[name], right[name]);
                    if (inner is not null) return inner;
                }

                return null;
            case (JsonArray left, JsonArray right):
                for (var index = 0; index < Math.Max(left.Count, right.Count); index++)
                {
                    if (index >= left.Count) return $"{path}[{index}]: unexpected in actual";
                    if (index >= right.Count) return $"{path}[{index}]: missing in actual";
                    var inner = FirstDifference($"{path}[{index}]", left[index], right[index]);
                    if (inner is not null) return inner;
                }

                return null;
            default:
                var expectedText = expected?.ToJsonString();
                var actualText = actual?.ToJsonString();
                return expectedText == actualText
                    ? null
                    : $"{path}: expected {expectedText}, actual {actualText}";
        }
    }

    private static async Task<HttpClient> RegisterAsync(BusinessFinanceApiFactory factory, string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, HasBusiness: true));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }

    [GeneratedRegex("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
    private static partial Regex GuidPattern();

    [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
    private static partial Regex GuidInText();

    /// <summary>Sentetik verinin kurulumu ve okunması.</summary>
    internal sealed class Scenario(
        HttpClient client, TestClock clock, BusinessFinanceApiFactory factory, string email)
    {
        private readonly Dictionary<Guid, string> _names = [];
        private readonly Dictionary<Guid, string> _anonymous = [];

        public Guid Kasa { get; private set; }
        public Guid Banka { get; private set; }
        public Guid ExpenseCategory { get; private set; }
        public Guid IncomeCategory { get; private set; }
        public Guid Toptanci { get; private set; }
        public Guid Ayse { get; private set; }
        public Guid EskiBanka { get; private set; }
        public Guid PosDefinition { get; private set; }
        public Dictionary<string, Guid> Obligations { get; } = [];
        public Dictionary<string, Guid> ObligationSettlements { get; } = [];
        public Dictionary<string, Guid> Debts { get; } = [];
        public Dictionary<string, Guid[]> Installments { get; } = [];

        /// <summary>Her yazmadan önce saat bir dakika ilerler: giriş sırası belirlidir.</summary>
        private void Tick() => clock.UtcNow = clock.UtcNow.AddMinutes(1);

        private void Name(Guid id, string name) => _names[id] = name;

        public async Task SeedAsync()
        {
            Kasa = await AccountAsync("Kasa", "cash", "5000.0000", "business");
            Banka = await AccountAsync("Banka", "bank", "20000.0000", "business");
            Name(await AccountAsync("Şahsi banka", "bank", "3000.0000", "personal"), "account:sahsi");

            var categories = await client.GetFromJsonAsync<JsonObject>("/api/v1/categories");
            foreach (var item in categories!["items"]!.AsArray())
            {
                Name(item!["id"]!.GetValue<Guid>(), $"category:{item["name"]!.GetValue<string>()}");
            }

            ExpenseCategory = FirstCategory(categories, "expense");
            IncomeCategory = FirstCategory(categories, "income");

            Toptanci = await CounterpartyAsync("Test Toptancı", "cp:toptanci");
            Ayse = await CounterpartyAsync("Ayşe Terzi", "cp:ayse");
            EskiBanka = await CounterpartyAsync("Eski Banka", "cp:eski-banka");

            Tick();
            using (var pos = await client.PostAsJsonAsync(
                       "/api/v1/pos-definitions",
                       new SavePosDefinitionRequest(
                           "Sentetik POS", Banka, IncomeCategory, "0.0200", 1, false, ExpenseCategory)))
            {
                Assert.Equal(HttpStatusCode.Created, pos.StatusCode);
                PosDefinition = (await pos.Content.ReadFromJsonAsync<PosDefinitionResponse>())!.Id;
                Name(PosDefinition, "pos:sentetik");
            }

            // Yükümlülükler.
            await ObligationAsync("o1-nakliye", "payable", "950.0000", ExpenseCategory, "2026-07-10", "2026-07-25", Toptanci, "Nakliye faturası");
            await ObligationAsync("o2-sigorta", "payable", "2400.0000", ExpenseCategory, "2026-08-01", "2026-09-05", null, "İşyeri sigortası");
            await ObligationAsync("o3-odenmis", "payable", "1200.0000", ExpenseCategory, "2026-07-05", "2026-07-30", Toptanci, "Ambalaj faturası");
            await SettleAsync("o3-odenmis", Banka, "2026-07-28", card: null);
            await ObligationAsync("o4-kartla", "receivable", "4000.0000", IncomeCategory, "2026-08-02", "2026-08-15", Ayse, "Dikiş işi");
            await SettleAsync("o4-kartla", null, "2026-08-10", new CardCollectionRequest(PosDefinitionId: PosDefinition));
            await ObligationAsync("o5-bugun", "receivable", "600.0000", IncomeCategory, Today, "2026-08-30", Ayse, "Tadilat");
            await ObligationAsync("o6-nakit", "receivable", "300.0000", IncomeCategory, "2026-08-05", "2026-08-25", Ayse, "Paça işi");
            await SettleAsync("o6-nakit", Kasa, Today, card: null);
            await ObligationAsync("o7-iptal", "payable", "500.0000", ExpenseCategory, "2026-07-15", "2026-07-31", Toptanci, "Yanlış fatura");
            Tick();
            using (var cancel = await client.DeleteAsync($"/api/v1/obligations/{Obligations["o7-iptal"]}"))
            {
                Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
            }

            // Kartla tahsilin parası yatışla hesaba geçer.
            var transit = await client.GetFromJsonAsync<JsonObject>("/api/v1/pos-settlements?inTransitOnly=true");
            var posSettlement = Assert.Single(transit!["items"]!.AsArray())!["id"]!.GetValue<Guid>();
            Name(posSettlement, "pos-settlement:o4");
            Tick();
            using (var deposit = await client.PostAsJsonAsync(
                       "/api/v1/pos-deposits",
                       new CreatePosDepositRequest(Guid.NewGuid(), [posSettlement], "3920.0000", "2026-08-12")))
            {
                Assert.True(deposit.StatusCode == HttpStatusCode.Created, await deposit.Content.ReadAsStringAsync());
                var created = await deposit.Content.ReadFromJsonAsync<JsonObject>();
                Name(created!["id"]!.GetValue<Guid>(), "pos-deposit:o4");
            }

            // Borç planları.
            await DebtAsync("d1-kuzen", "Kuzen Ali", "payable", "personal", "5000.0000", "5000.0000", "cash", Kasa, null, "2026-07-01", "2026-07-20", 5, "Borç para");
            Name((await CounterpartyIdAsync("Kuzen Ali")), "cp:kuzen");
            await PayAsync("d1-kuzen", 1, Kasa, "2026-07-20");
            await PayAsync("d1-kuzen", 2, Kasa, "2026-08-19");
            await DebtAsync("d2-banka", "Örnek Banka", "payable", "business", "32000.0000", "36000.0000", "cash", Banka, null, "2026-07-03", "2026-07-08", 12, "İhtiyaç kredisi");
            Name((await CounterpartyIdAsync("Örnek Banka")), "cp:ornek-banka");
            await PayAsync("d2-banka", 1, Banka, "2026-07-08");
            await PayAsync("d2-banka", 2, Banka, "2026-08-08");
            await DebtAsync("d3-overlok", "Ayşe Terzi", "receivable", "business", "4500.0000", "4500.0000", "income", null, IncomeCategory, "2026-08-04", "2026-08-18", 3, "Overlok makinesi");
            await PayAsync("d3-overlok", 1, Kasa, "2026-08-18");
            await DebtAsync("d4-dolap", "Test Toptancı", "payable", "business", "6000.0000", "6000.0000", "expense", null, ExpenseCategory, "2026-07-12", "2026-08-10", 6, "Soğutucu dolap");
            await UnrecordedDebtAsync();
            await PayAsync("d5-eski", 1, Banka, "2026-07-01");

            // Aynı kişide cari hareket: yükümlülük cari bakiyeye girmez.
            Tick();
            using (var charge = await client.PostAsJsonAsync(
                       $"/api/v1/counterparties/{Toptanci}/charges",
                       new CreateCounterpartyChargeRequest(
                           "payable", "1500.0000", "TRY", ExpenseCategory, "2026-08-03", "business",
                           DueDate: "2026-08-25")))
            {
                Assert.True(charge.StatusCode == HttpStatusCode.Created, await charge.Content.ReadAsStringAsync());
                Name((await charge.Content.ReadFromJsonAsync<JsonObject>())!["id"]!.GetValue<Guid>(), "charge:toptanci");
            }

            // Gün sonu bugünün alacak faturasını ve nakit tahsilatı sayar.
            Tick();
            using (var close = await client.PostAsJsonAsync(
                       "/api/v1/day-closes",
                       new DayCloseRequest(Today, CashAmount: "900.0000",
                           RecordOverrides:
                           [
                               new DayCloseRecordOverrideRequest("obligation", Obligations["o5-bugun"], true),
                               new DayCloseRecordOverrideRequest("obligation-settlement", ObligationSettlements["o6-nakit"], true),
                           ]) with
                       { ClientRequestId = Guid.NewGuid() }))
            {
                Assert.True(close.StatusCode == HttpStatusCode.Created, await close.Content.ReadAsStringAsync());
                Name((await close.Content.ReadFromJsonAsync<JsonObject>())!["id"]!.GetValue<Guid>(), "day-close:today");
            }

            Tick();
        }

        public async Task<JsonObject> ReadAllAsync()
        {
            var reads = new JsonObject();

            async Task Read(string name, string path)
            {
                using var response = await client.GetAsync(path);
                var body = await response.Content.ReadAsStringAsync();
                Assert.True(response.IsSuccessStatusCode, $"{path}: {(int)response.StatusCode} {body}");
                reads[name] = path.EndsWith(".csv", StringComparison.Ordinal)
                    ? JsonValue.Create(body)
                    : JsonNode.Parse(body);
            }

            await Read("obligations", $"/api/v1/obligations?asOfDate={Today}");
            await Read("debts", $"/api/v1/debts?asOfDate={Today}");
            await Read("counterparties", $"/api/v1/counterparties?asOfDate={Today}");
            foreach (var (key, id) in new[] { ("toptanci", Toptanci), ("ayse", Ayse), ("eski-banka", EskiBanka) })
            {
                await Read($"counterparty:{key}", $"/api/v1/counterparties/{id}?asOfDate={Today}");
                await Read($"feed:counterparty:{key}", $"/api/v1/financial-activities?pageSize=100&counterpartyId={id}");
            }

            await Read("feed", "/api/v1/financial-activities?pageSize=100");
            await Read("feed:july", "/api/v1/financial-activities?pageSize=100&dateFrom=2026-07-01&dateTo=2026-07-31");
            await Read("planned:90", $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=90");
            await Read("planned:business", $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=90&scope=business");
            await Read("upcoming", $"/api/v1/upcoming-payments?asOfDate={Today}&daysAhead=90");
            foreach (var month in new[] { 7, 8 })
            {
                await Read($"monthly:{month}", $"/api/v1/reports/monthly?year=2026&month={month}");
                await Read($"monthly:{month}:business", $"/api/v1/reports/monthly?year=2026&month={month}&scope=business");
                await Read($"category-summary:{month}", $"/api/v1/category-summary?year=2026&month={month}");
                await Read($"category-summary:{month}:income", $"/api/v1/category-summary?year=2026&month={month}&direction=income");
                await Read($"category:{month}:expense", $"/api/v1/categories/{ExpenseCategory}/page?year=2026&month={month}");
                await Read($"category:{month}:income", $"/api/v1/categories/{IncomeCategory}/page?year=2026&month={month}&direction=income");
                foreach (var (key, id) in new[] { ("kasa", Kasa), ("banka", Banka) })
                {
                    await Read($"account:{key}:{month}", $"/api/v1/accounts/{id}/page?year=2026&month={month}");
                }
            }

            await Read("advanced", $"/api/v1/reports/advanced?year=2026&month=8&asOfDate={Today}&daysAhead=90");
            await Read("dashboard", "/api/v1/dashboard?year=2026&month=8");
            await Read("accounts", "/api/v1/accounts");
            await Read("accounts:overview", "/api/v1/accounts/overview");
            await Read("account:banka:months", $"/api/v1/accounts/{Banka}/months?year=2026");
            await Read("account:banka:in-transit", $"/api/v1/accounts/{Banka}/in-transit");
            await Read("pos-settlements", "/api/v1/pos-settlements");
            await Read("day-close:today", $"/api/v1/day-closes/day?date={Today}");
            await Read("profile", "/api/v1/profile");
            await Read("export:counterparty-ledger", "/api/v1/exports/counterparty-ledger.csv");

            foreach (var (name, kind, id) in new[]
                     {
                         ("o1", "obligation", Obligations["o1-nakliye"]),
                         ("o3-settlement", "obligation-settlement", ObligationSettlements["o3-odenmis"]),
                         ("o4-settlement", "obligation-settlement", ObligationSettlements["o4-kartla"]),
                         ("o6-settlement", "obligation-settlement", ObligationSettlements["o6-nakit"]),
                         ("d1-opening", "debt-opening", Debts["d1-kuzen"]),
                         ("d1-payment-2", "debt-payment", Installments["d1-kuzen"][1]),
                         ("d2-payment-2", "debt-payment", Installments["d2-banka"][1]),
                         ("d3-collection-1", "debt-collection", Installments["d3-overlok"][0]),
                         ("d4-opening", "debt-opening", Debts["d4-dolap"]),
                     })
            {
                await Read($"balances:{name}", $"/api/v1/financial-activities/{kind}/{id}/balances");
            }

            return reads;
        }

        public async Task<byte[]> BackupAsync()
        {
            using var response = await client.GetAsync("/api/v1/backups/download");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Kimlikleri adlarla değiştirir. Adı olmayan kimlik (sunucunun kendi
        /// ürettiği) ilk görünme sırasıyla numaralanır.
        /// </summary>
        public JsonNode Normalize(JsonNode node) => node switch
        {
            JsonObject obj => new JsonObject(obj.Select(item =>
                new KeyValuePair<string, JsonNode?>(item.Key, item.Value is null ? null : Normalize(item.Value)))),
            JsonArray array => new JsonArray([.. array.Select(item => item is null ? null : Normalize(item))]),
            JsonValue value when value.TryGetValue<string>(out var text) && text == email =>
                JsonValue.Create("<email>"),
            JsonValue value when value.TryGetValue<string>(out var text) && GuidPattern().IsMatch(text) =>
                JsonValue.Create(NameOf(Guid.Parse(text))),
            JsonValue value when value.TryGetValue<string>(out var text) && GuidInText().IsMatch(text) =>
                JsonValue.Create(NormalizeText(text)),
            _ => node.DeepClone(),
        };

        private string NormalizeText(string text) => GuidInText().Replace(
            text, match => NameOf(Guid.Parse(match.Value)));

        private string NameOf(Guid id)
        {
            if (_names.TryGetValue(id, out var name)) return name;
            if (!_anonymous.TryGetValue(id, out var anonymous))
            {
                anonymous = $"id:{_anonymous.Count + 1}";
                _anonymous[id] = anonymous;
            }

            return anonymous;
        }

        private static Guid FirstCategory(JsonObject categories, string type) => categories["items"]!.AsArray()
            .First(item => item!["type"]!.GetValue<string>() == type && item["isActive"]!.GetValue<bool>() &&
                           item["defaultScope"]?.GetValue<string>() == "business")!["id"]!
            .GetValue<Guid>();

        private async Task<Guid> AccountAsync(string name, string type, string opening, string scope)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                "/api/v1/accounts", new CreateAccountRequest(name, type, "TRY", opening, scope));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var id = (await response.Content.ReadFromJsonAsync<AccountResponse>())!.Id;
            Name(id, $"account:{name}");
            return id;
        }

        private async Task<Guid> CounterpartyAsync(string name, string key)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                "/api/v1/counterparties", new CreateCounterpartyRequest(name));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var id = (await response.Content.ReadFromJsonAsync<CounterpartyResponse>())!.Id;
            Name(id, key);
            return id;
        }

        private async Task<Guid> CounterpartyIdAsync(string name)
        {
            var list = await client.GetFromJsonAsync<JsonObject>("/api/v1/counterparties");
            return list!["items"]!.AsArray()
                .Single(item => item!["name"]!.GetValue<string>() == name)!["id"]!.GetValue<Guid>();
        }

        private async Task ObligationAsync(
            string key, string direction, string amount, Guid category, string issue, string due,
            Guid? counterparty, string description)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                "/api/v1/obligations",
                new CreateObligationRequest(direction, amount, "TRY", category, issue, due, "business", counterparty, description));
            Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
            var id = (await response.Content.ReadFromJsonAsync<ObligationResponse>())!.Id;
            Obligations[key] = id;
            Name(id, $"obligation:{key}");
        }

        private async Task SettleAsync(string key, Guid? account, string date, CardCollectionRequest? card)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                $"/api/v1/obligations/{Obligations[key]}/settlement",
                new SettleObligationRequest(account, date, card));
            Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
            var settled = (await response.Content.ReadFromJsonAsync<JsonObject>())!;
            var id = settled["settlementId"]!.GetValue<Guid>();
            ObligationSettlements[key] = id;
            Name(id, $"obligation-settlement:{key}");
        }

        private async Task DebtAsync(
            string key, string counterparty, string direction, string scope, string principal, string total,
            string source, Guid? openingAccount, Guid? category, string start, string firstDue, int count,
            string description)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                "/api/v1/debts",
                new CreateDebtRequest(counterparty, direction, scope, principal, total, null, "TRY", source,
                    openingAccount, category, start, firstDue, count, description, Today));
            Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
            Remember(key, (await response.Content.ReadFromJsonAsync<DebtResponse>())!);
        }

        private void Remember(string key, DebtResponse debt)
        {
            Debts[key] = debt.Id;
            Name(debt.Id, $"debt:{key}");
            Installments[key] = [.. debt.Installments.OrderBy(item => item.Sequence).Select(item => item.Id)];
            foreach (var installment in debt.Installments)
            {
                Name(installment.Id, $"installment:{key}:{installment.Sequence}");
            }
        }

        private async Task PayAsync(string key, int sequence, Guid account, string date)
        {
            Tick();
            using var response = await client.PostAsJsonAsync(
                $"/api/v1/debts/{Debts[key]}/installments/{sequence}/pay",
                new PayDebtInstallmentRequest(account, date, Today));
            Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Açılışı kaydedilmemiş eski borç API'den açılamaz (ADR 0009 §6);
        /// eski veriyi taklit etmek için doğrudan yazılır.
        /// </summary>
        private async Task UnrecordedDebtAsync()
        {
            Tick();
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
                var counterparty = await db.Counterparties.SingleAsync(item => item.Id == EskiBanka);
                var debt = Debt.WithUnrecordedOpening(
                    Guid.NewGuid(),
                    counterparty.UserId,
                    counterparty,
                    DebtDirection.Payable,
                    TransactionScope.Business,
                    new Money(3000m, CurrencyCode.TRY),
                    new Money(3000m, CurrencyCode.TRY),
                    new DateOnly(2026, 6, 1),
                    new DateOnly(2026, 7, 1),
                    3,
                    "Eski kredi");
                db.Debts.Add(debt);
                await db.SaveChangesAsync();
            }

            var list = await client.GetFromJsonAsync<DebtListResponse>($"/api/v1/debts?asOfDate={Today}");
            Remember("d5-eski", list!.Items.Single(item => item.Description == "Eski kredi"));
        }
    }
}
