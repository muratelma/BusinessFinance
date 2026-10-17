using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.Debts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests.Features.Debts;

/// <summary>
/// "Borç ve alacaklar" listesi (ADR 0021 T8, CB7, CB23, CB24): brif 7'nin
/// alacak örneği. Bugün 28 Ekim 2026.
/// </summary>
public sealed class DebtBookEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Today = "2026-10-28";
    private static readonly DateTimeOffset Noon = new(2026, 10, 28, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Book_ListsDebtsAndLedgersBySection_WithServerTotals()
    {
        await using var factory = FixedClockApi.Create(Noon);
        await BookScenarioAsync(factory);
    }

    /// <summary>Aynı senaryo gerçek SQL Server'da: liste ve gecikme sorgularının çevirisi.</summary>
    [SqlServerFact]
    public async Task Book_ListsDebtsAndLedgersBySection_WithServerTotals_OnSqlServer()
    {
        await using var factory = SqlFactory();
        await BookScenarioAsync(factory);
    }

    [Fact]
    public async Task LedgerDiscountAndOpeningBalance_ReduceTheBalanceAndAgeOldestFirst()
    {
        await using var factory = FixedClockApi.Create(Noon);
        await LedgerScenarioAsync(factory);
    }

    [SqlServerFact]
    public async Task LedgerDiscountAndOpeningBalance_ReduceTheBalanceAndAgeOldestFirst_OnSqlServer()
    {
        await using var factory = SqlFactory();
        await LedgerScenarioAsync(factory);
    }

    private static BusinessFinanceApiFactory SqlFactory() => new(
        useConfiguredSqlServer: true,
        configureServices: services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(new TestClock(Noon));
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options => options.TokenValidationParameters.ValidateLifetime = false);
        });

    private static async Task BookScenarioAsync(BusinessFinanceApiFactory factory)
    {
        using var owner = await ClientAsync(factory, $"debt-book-{Guid.NewGuid():N}@example.test");
        var bank = await AccountAsync(owner);
        var income = await CategoryAsync(owner, "income");
        var expense = await CategoryAsync(owner, "expense");
        var ayse = await PersonAsync(owner, "Ayşe Terzi");
        var toptanci = await PersonAsync(owner, "Test Toptancı");

        // Ayşe Terzi cari: 2.000 gecikmiş + 1.500 vadesi gelmemiş.
        await ChargeAsync(owner, ayse, "receivable", "2000.0000", income, "2026-09-01", "2026-10-01");
        await ChargeAsync(owner, ayse, "receivable", "1500.0000", income, "2026-10-20", "2026-11-30");

        // Test Toptancı cari: vadesiz eski alım 1.000, vadeli alım 500 (10 Ekim),
        // 1.000 ödeme. Ödeme önce eski alımı kapatır: 500 gecikmiş kalır.
        await ChargeAsync(owner, toptanci, "payable", "1000.0000", expense, "2026-09-01", null);
        await ChargeAsync(owner, toptanci, "payable", "500.0000", expense, "2026-09-15", "2026-10-10");
        using (var paid = await owner.PostAsJsonAsync($"/api/v1/counterparties/{toptanci}/payments",
                   new CreateCounterpartyPaymentRequest("payable", "1000.0000", "TRY", bank, "2026-10-01")))
            paid.EnsureSuccessStatusCode();

        await DebtAsync(owner, new CreateDebtRequest(
            "Kaan Yapı", "receivable", "business", "4000.0000", "4000.0000", null, "TRY", "income",
            null, income, "2026-10-15", "2026-11-15", 1, "Montaj işi", Today, PlanKind: "single"));
        await DebtAsync(owner, new CreateDebtRequest(
            null, "receivable", "business", "4500.0000", "4500.0000", null, "TRY", "income",
            null, income, "2026-10-15", "2026-11-15", 3, "Overlok makinesi", Today, CounterpartyId: ayse));
        await DebtAsync(owner, new CreateDebtRequest(
            "Elif", "receivable", "business", "4000.0000", "4000.0000", null, "TRY", "income",
            null, income, "2026-10-01", null, 1, "Telefon", Today, PlanKind: "none"));
        var paint = await DebtAsync(owner, new CreateDebtRequest(
            null, "payable", "business", "2000.0000", "2000.0000", null, "TRY", "expense",
            null, expense, "2026-09-12", "2026-09-30", 1, "Boya malzemesi", Today, PlanKind: "single",
            CounterpartyId: toptanci, UpfrontPayment: new DebtUpfrontPaymentRequest("2000.0000", bank)));
        Assert.True(paint.IsClosed);

        var book = (await owner.GetFromJsonAsync<DebtBookResponse>($"/api/v1/debt-book?asOfDate={Today}"))!;

        Assert.Equal("16000.0000", book.ReceivableTotal);
        Assert.Equal("500.0000", book.PayableTotal);
        var receivables = book.Items.Where(item => item.Direction == "receivable").ToArray();
        // Bölüm sırası; yaklaşanda yakın vade önce, aynı günde büyük kalan önce.
        Assert.Equal(
            [("overdue", "Ayşe Terzi", "3500.0000"), ("upcoming", "Overlok makinesi", "4500.0000"),
             ("upcoming", "Montaj işi", "4000.0000"), ("undated", "Telefon", "4000.0000")],
            receivables.Select(item => (item.Section, item.Title, item.Remaining)));
        var ledger = receivables[0];
        Assert.Equal("ledger", ledger.Kind);
        Assert.Equal("2000.0000", ledger.Overdue);
        Assert.Equal("2026-11-30", ledger.NextDueDate);
        var overlok = receivables[1];
        Assert.Equal("2026-11-15", overlok.NextDueDate);
        Assert.Equal("1500.0000", overlok.NextDueAmount);

        var payable = Assert.Single(book.Items, item => item.Direction == "payable");
        Assert.Equal(("ledger", "overdue", "500.0000"), (payable.Kind, payable.Section, payable.Overdue));

        // Kişinin sayfası aynı gecikmiş tutarı söyler.
        var person = await owner.GetFromJsonAsync<JsonNode>($"/api/v1/counterparties/{toptanci}?asOfDate={Today}");
        Assert.Equal("500.0000", person!["overduePayable"]!.GetValue<string>());

        var ayseRow = Assert.Single(book.People, item => item.CounterpartyId == ayse);
        Assert.Equal(("8000.0000", "3500.0000", 1), (ayseRow.OwedToYou, ayseRow.LedgerOwedToYou, ayseRow.OpenDebtCount));

        Assert.Equal(1, book.ClosedPayableCount);
        var closed = await owner.GetFromJsonAsync<DebtBookClosedResponse>("/api/v1/debt-book/closed");
        var closedPaint = Assert.Single(closed!.Items);
        Assert.Equal(("Boya malzemesi", "2000.0000", "2026-09-12"), (closedPaint.Title, closedPaint.Amount, closedPaint.ClosedOn));
    }

    /// <summary>
    /// Cari alımda indirim bedeli ve gideri alım gününde düşürür; devreden borç
    /// gider yazmaz ve ödemeyi ilk o karşılar (ADR 0021 T3, T5).
    /// </summary>
    private static async Task LedgerScenarioAsync(BusinessFinanceApiFactory factory)
    {
        using var owner = await ClientAsync(factory, $"ledger-discount-{Guid.NewGuid():N}@example.test");
        var bank = await AccountAsync(owner);
        var expense = await CategoryAsync(owner, "expense");
        var supplier = await PersonAsync(owner, "Test Toptancı");

        using var created = await owner.PostAsJsonAsync($"/api/v1/counterparties/{supplier}/charges",
            new CreateCounterpartyChargeRequest("payable", "1000.0000", "TRY", expense, "2026-10-01", DueDate: "2026-10-10"));
        var charge = (await created.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;

        var request = new CounterpartyChargeDiscountRequest("200.0000", "2026-10-20", Guid.NewGuid());
        using var discounted = await owner.PostAsJsonAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts", request);
        Assert.True(discounted.IsSuccessStatusCode, await discounted.Content.ReadAsStringAsync());
        using (var retried = await owner.PostAsJsonAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts", request))
        {
            var again = (await retried.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
            Assert.Equal("800.0000", again.NetAmount);
            Assert.Single(again.Discounts);
        }

        using (var tooMuch = await owner.PostAsJsonAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts",
                   new CounterpartyChargeDiscountRequest("900.0000", "2026-10-20")))
            Assert.Contains("counterparty_charges.discount_exceeds_amount", await tooMuch.Content.ReadAsStringAsync());

        var report = await owner.GetFromJsonAsync<JsonNode>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("800.0000", report!["totalExpense"]!.GetValue<string>());

        // Akıştaki alım satırı raporla aynı: bedel 800, indirim altında 200.
        var feed = await owner.GetFromJsonAsync<JsonNode>("/api/v1/financial-activities?pageSize=100");
        var row = feed!["items"]!.AsArray().Single(item => item!["activityId"]!.GetValue<Guid>() == charge.Id)!;
        Assert.Equal(("800.0000", "200.0000"), (row["amount"]!.GetValue<string>(), row["discountAmount"]!.GetValue<string>()));

        using (var opening = await owner.PutAsJsonAsync($"/api/v1/counterparties/{supplier}/opening-balance",
                   new CounterpartyOpeningBalanceRequest("0.0000", "300.0000")))
        {
            var person = (await opening.Content.ReadFromJsonAsync<JsonNode>())!;
            Assert.Equal(("1100.0000", "300.0000", "800.0000"), (
                person["payable"]!.GetValue<string>(),
                person["openingPayable"]!.GetValue<string>(),
                person["overduePayable"]!.GetValue<string>()));
        }

        // Ödeme önce devreden borcu kapatır: gecikmiş alım olduğu gibi kalır.
        using (var paid = await owner.PostAsJsonAsync($"/api/v1/counterparties/{supplier}/payments",
                   new CreateCounterpartyPaymentRequest("payable", "300.0000", "TRY", bank, "2026-10-25")))
            paid.EnsureSuccessStatusCode();
        var afterPayment = await owner.GetFromJsonAsync<JsonNode>($"/api/v1/counterparties/{supplier}?asOfDate={Today}");
        Assert.Equal(("800.0000", "800.0000"), (
            afterPayment!["payable"]!.GetValue<string>(), afterPayment["overduePayable"]!.GetValue<string>()));
        report = await owner.GetFromJsonAsync<JsonNode>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("800.0000", report!["totalExpense"]!.GetValue<string>());

        // Cari dökümü bakiyeyi tutar: devir boş günle başta, indirim alımın kategorisiyle.
        var csv = (await owner.GetStringAsync("/api/v1/exports/counterparty-ledger.csv")).Split('\n');
        Assert.StartsWith(",opening-balance,payable,300.0000,", csv[1].Split(',', 2)[1]);
        Assert.Contains(csv, line => line.Contains(",2026-10-20,discount,payable,200.0000,", StringComparison.Ordinal));

        var discountId = (await discounted.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!.Discounts[0].Id;
        using var reversed = await owner.DeleteAsync($"/api/v1/counterparty-charges/{charge.Id}/discounts/{discountId}");
        Assert.Equal("1000.0000", (await reversed.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!.NetAmount);
        report = await owner.GetFromJsonAsync<JsonNode>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("1000.0000", report!["totalExpense"]!.GetValue<string>());
    }

    private static async Task ChargeAsync(
        HttpClient client, Guid person, string direction, string amount, Guid category, string date, string? due)
    {
        using var response = await client.PostAsJsonAsync($"/api/v1/counterparties/{person}/charges",
            new CreateCounterpartyChargeRequest(direction, amount, "TRY", category, date, DueDate: due));
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }

    private static async Task<DebtResponse> DebtAsync(HttpClient client, CreateDebtRequest request)
    {
        using var response = await client.PostAsJsonAsync("/api/v1/debts", request);
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<DebtResponse>())!;
    }

    private static async Task<Guid> PersonAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync("/api/v1/counterparties", new CreateCounterpartyRequest(name));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonNode>())!["id"]!.GetValue<Guid>();
    }

    private static async Task<Guid> CategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<JsonNode>("/api/v1/categories");
        return categories!["items"]!.AsArray()
            .First(item => item!["type"]!.GetValue<string>() == type &&
                           item["defaultScope"]?.GetValue<string>() == "business" &&
                           item["role"] is null && !item["isTax"]!.GetValue<bool>())!["id"]!.GetValue<Guid>();
    }

    private static async Task<Guid> AccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest("Ziraat Vadesiz", "bank", "TRY", "10000.0000", "business"));
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
