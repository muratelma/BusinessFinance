using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Pos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests.Features.Debts;

/// <summary>
/// Yedek v13 (ADR 0021 dalga 2): kısmi ödeme, kartla ödeme, indirim, geri
/// alınmış ödeme, POS'la tahsil (duran ve geri alınmış), plan değişikliği, devir,
/// kişisiz ve planı olmayan borç, cari indirim ve devreden bakiye yedekten aynı
/// okumalarla döner.
/// </summary>
public sealed partial class DebtBackupV13Tests
{
    private const string Password = "Valid-Password-123!";
    private const string Today = "2026-10-28";
    private static readonly DateTimeOffset Noon = new(2026, 10, 28, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task BackupV13_RestoresEveryDebtStateIntoTheSameReads()
    {
        await using var factory = FixedClockApi.Create(Noon);
        await RoundTripAsync(factory);
    }

    [SqlServerFact]
    public async Task BackupV13_RestoresEveryDebtStateIntoTheSameReads_OnSqlServer()
    {
        await using var factory = new BusinessFinanceApiFactory(
            useConfiguredSqlServer: true,
            configureServices: services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(new TestClock(Noon));
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options => options.TokenValidationParameters.ValidateLifetime = false);
            });
        await RoundTripAsync(factory);
    }

    private static async Task RoundTripAsync(BusinessFinanceApiFactory factory)
    {
        using var source = await ClientAsync(factory, $"backup-v13-source-{Guid.NewGuid():N}@example.test");
        await SeedAsync(source);
        var expected = await ReadWithoutIdentitiesAsync(source);

        using var download = await source.GetAsync("/api/v1/backups/download");
        download.EnsureSuccessStatusCode();
        var backup = await download.Content.ReadAsByteArrayAsync();

        using var target = await ClientAsync(factory, $"backup-v13-target-{Guid.NewGuid():N}@example.test");
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(backup);
        file.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.business-finance.backup+json");
        form.Add(file, "file", "v13.bfbackup.json");
        using var restore = await target.PostAsync("/api/v1/backups/restore", form);
        Assert.True(restore.IsSuccessStatusCode, await restore.Content.ReadAsStringAsync());
        var actual = await ReadWithoutIdentitiesAsync(target);

        // Karşılaştırma boş listede geçmesin: yedi borcun beşi açık listede, kapanan
        // nakliye faturası kapananlarda; iptal edilen ikisinde de yok.
        Assert.Equal(5, actual[$"/api/v1/debt-book?asOfDate={Today}"]!["items"]!.AsArray()
            .Count(item => item!["kind"]!.GetValue<string>() != "ledger"));
        Assert.Single(actual["/api/v1/debt-book/closed"]!["items"]!.AsArray());
        var difference = DebtMergeBaselineTests.FirstDifference("$", expected, actual);
        Assert.True(difference is null, difference);
    }

    private static async Task SeedAsync(HttpClient owner)
    {
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");
        var expense = await CategoryAsync(owner, "expense");
        var income = await CategoryAsync(owner, "income");
        using var cardResponse = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest("İş kartı", "50000.0000", "TRY", 10, 20));
        var card = (await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>())!;

        // Fatura: kısmi ödeme, geri alınmış ödeme, indirim.
        var invoice = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Test Toptancı", "payable", "business", "2000.0000", "2000.0000", null, "TRY", "expense",
            null, expense, "2026-10-01", "2026-10-20", 1, "Boya malzemesi", Today, PlanKind: "single"));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{invoice.Id}/payments",
            new RecordDebtPaymentRequest("500.0000", "2026-10-05", Today, AccountId: bank));
        var mistaken = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{invoice.Id}/payments",
            new RecordDebtPaymentRequest("100.0000", "2026-10-06", Today, AccountId: bank));
        var mistakenId = mistaken.Closings.Single(item => item.Amount == "100.0000").Id;
        await SendAsync(owner, HttpMethod.Delete, $"/api/v1/debts/{invoice.Id}/payments/{mistakenId}?asOfDate={Today}", null);
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{invoice.Id}/discounts",
            new ApplyDebtDiscountRequest("200.0000", "2026-10-10", Today));

        // Kartla ödenmiş fatura.
        var freight = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Test Toptancı", "payable", "business", "950.0000", "950.0000", null, "TRY", "expense",
            null, expense, "2026-10-20", "2026-10-25", 1, "Nakliye faturası", Today, PlanKind: "single"));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{freight.Id}/payments",
            new RecordDebtPaymentRequest("950.0000", Today, Today, CreditCardId: card.Id));

        // Taksitli satış: bir tahsilat, sonra plan değişikliği.
        var sale = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Ayşe Terzi", "receivable", "business", "4500.0000", "4500.0000", null, "TRY", "income",
            null, income, "2026-09-15", "2026-10-15", 3, "Overlok makinesi", Today));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{sale.Id}/payments",
            new RecordDebtPaymentRequest("1500.0000", "2026-10-15", Today, AccountId: bank));

        // POS'la kısmi tahsil yolda kalır; ikincisi geri alınır ve tahsilatı iptal olur.
        using var posResponse = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Ana POS", bank, income, "0.0150", 1, false, expense));
        posResponse.EnsureSuccessStatusCode();
        var pos = (await posResponse.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{sale.Id}/payments",
            new RecordDebtPaymentRequest("400.0000", "2026-10-20", Today, Card: new CardCollectionRequest(pos.Id)));
        var reversedByCard = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{sale.Id}/payments",
            new RecordDebtPaymentRequest("250.0000", "2026-10-21", Today, Card: new CardCollectionRequest(pos.Id)));
        var reversedByCardId = reversedByCard.Closings.Single(item => item.Amount == "250.0000").Id;
        await SendAsync(owner, HttpMethod.Delete, $"/api/v1/debts/{sale.Id}/payments/{reversedByCardId}?asOfDate={Today}", null);
        await SendAsync(owner, HttpMethod.Put, $"/api/v1/debts/{sale.Id}/plan",
            new ChangeDebtPlanRequest("installments", "2026-12-15", 2, Today));

        // Devreden kredi, bir taksidi ödenmiş; kişisiz, planı olmayan borç.
        var carried = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Ziraat Bankası", "payable", "personal", "18000.0000", "18000.0000", null, "TRY", "cash",
            null, null, "2026-10-01", "2026-10-05", 12, "İhtiyaç kredisi", Today, IsCarriedOver: true));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{carried.Id}/payments",
            new RecordDebtPaymentRequest("1500.0000", "2026-10-05", Today, AccountId: bank));
        var loose = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            null, "payable", "personal", "500.0000", "500.0000", null, "TRY", "cash",
            bank, null, Today, null, 1, "Komşudan borç", Today, PlanKind: "none"));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{loose.Id}/payments",
            new RecordDebtPaymentRequest("200.0000", Today, Today, AccountId: bank));

        // Devredilmiş fatura borcu, kısmen ödenmiş: kategorisi yok, tanınmaz.
        var bill = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Elektrik idaresi", "payable", "personal", "700.0000", "700.0000", null, "TRY", "expense",
            null, null, "2026-10-01", "2026-11-10", 1, "Eylül faturası", Today, PlanKind: "single", IsCarriedOver: true));
        await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{bill.Id}/payments",
            new RecordDebtPaymentRequest("100.0000", "2026-10-20", Today, AccountId: bank));

        // İptal edilmiş borç da döner.
        var cancelled = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Test Toptancı", "payable", "business", "300.0000", "300.0000", null, "TRY", "expense",
            null, expense, "2026-10-02", "2026-10-30", 1, "Yanlış fatura", Today, PlanKind: "single"));
        await SendAsync(owner, HttpMethod.Delete, $"/api/v1/debts/{cancelled.Id}?asOfDate={Today}", null);

        // Cari: indirimli alım, geri alınmış indirim, devreden borç ve ödeme.
        var supplier = (await owner.GetFromJsonAsync<JsonNode>("/api/v1/counterparties"))!["items"]!.AsArray()
            .Single(item => item!["name"]!.GetValue<string>() == "Test Toptancı")!["id"]!.GetValue<Guid>();
        using var charged = await owner.PostAsJsonAsync($"/api/v1/counterparties/{supplier}/charges",
            new CreateCounterpartyChargeRequest("payable", "1000.0000", "TRY", expense, "2026-10-01", DueDate: "2026-10-10"));
        var charge = (await charged.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
        using (var kept = await owner.PostAsJsonAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts",
                   new CounterpartyChargeDiscountRequest("200.0000", "2026-10-12")))
            kept.EnsureSuccessStatusCode();
        using var undone = await owner.PostAsJsonAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts",
            new CounterpartyChargeDiscountRequest("50.0000", "2026-10-13"));
        var undoneId = (await undone.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!
            .Discounts.Single(item => item.Amount == "50.0000").Id;
        using (var reversed = await owner.DeleteAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts/{undoneId}"))
            reversed.EnsureSuccessStatusCode();
        using (var opening = await owner.PutAsJsonAsync($"/api/v1/counterparties/{supplier}/opening-balance",
                   new CounterpartyOpeningBalanceRequest("0.0000", "300.0000")))
            opening.EnsureSuccessStatusCode();
        using (var paid = await owner.PostAsJsonAsync($"/api/v1/counterparties/{supplier}/payments",
                   new CreateCounterpartyPaymentRequest("payable", "300.0000", "TRY", bank, "2026-10-25")))
            paid.EnsureSuccessStatusCode();
    }

    private static async Task<JsonObject> ReadWithoutIdentitiesAsync(HttpClient client)
    {
        var reads = new JsonObject();
        foreach (var path in new[]
                 {
                     $"/api/v1/debt-book?asOfDate={Today}",
                     "/api/v1/debt-book/closed",
                     $"/api/v1/obligations?asOfDate={Today}",
                     $"/api/v1/debts?asOfDate={Today}",
                     $"/api/v1/counterparties?asOfDate={Today}",
                     "/api/v1/financial-activities?pageSize=100",
                     $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=90",
                     $"/api/v1/upcoming-payments?asOfDate={Today}&daysAhead=90",
                     "/api/v1/reports/monthly?year=2026&month=9",
                     "/api/v1/reports/monthly?year=2026&month=10",
                     "/api/v1/category-summary?year=2026&month=10",
                     $"/api/v1/reports/advanced?year=2026&month=10&asOfDate={Today}&daysAhead=90",
                     "/api/v1/accounts",
                     "/api/v1/credit-cards",
                     "/api/v1/pos-settlements",
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

    [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
    private static partial Regex GuidInText();

    private static async Task<DebtResponse> SendAsync(HttpClient client, HttpMethod method, string path, object? body)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body is not null) request.Content = JsonContent.Create(body, body.GetType());
        using var response = await client.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{method} {path}: {(int)response.StatusCode} {text}");
        return System.Text.Json.JsonSerializer.Deserialize<DebtResponse>(
            text, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web))!;
    }

    private static async Task<Guid> CategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<JsonNode>("/api/v1/categories");
        return categories!["items"]!.AsArray()
            .First(item => item!["type"]!.GetValue<string>() == type &&
                           item["defaultScope"]?.GetValue<string>() == "business" &&
                           item["role"] is null && !item["isTax"]!.GetValue<bool>())!["id"]!.GetValue<Guid>();
    }

    private static async Task<Guid> AccountAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "bank", "TRY", "10000.0000", "business"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!.Id;
    }

    private static async Task<HttpClient> ClientAsync(BusinessFinanceApiFactory factory, string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        return client;
    }
}
