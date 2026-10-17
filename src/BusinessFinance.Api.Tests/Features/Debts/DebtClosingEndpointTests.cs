using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Reports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessFinance.Api.Tests.Features.Debts;

/// <summary>
/// ADR 0021 dalga 2: borç kendi kapanışlarıyla kapanır — her plan, kısmi
/// ödeme, istek kimliği, indirim, geri alma, plan değişikliği, kartla ödeme,
/// uygulamadan önceki borç ve iptal. Bugün 28 Ekim 2026.
/// </summary>
public sealed class DebtClosingEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Today = "2026-10-28";
    private static readonly DateTimeOffset Noon = new(2026, 10, 28, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Invoice_TakesPartialPaymentsDiscountReversalAndPlanChange()
    {
        await using var factory = FixedClockApi.Create(new TestClock(Noon));
        await InvoiceScenarioAsync(factory);
    }

    /// <summary>Aynı senaryo gerçek SQL Server'da: görünümlerin çevirisi ve sürüm belirteci.</summary>
    [SqlServerFact]
    public async Task Invoice_TakesPartialPaymentsDiscountReversalAndPlanChange_OnSqlServer()
    {
        await using var factory = SqlFactory();
        await InvoiceScenarioAsync(factory);
    }

    [SqlServerFact]
    public async Task CardPayment_RaisesTheCardDebt_OnSqlServer()
    {
        await using var factory = SqlFactory();
        await CardScenarioAsync(factory);
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

    private static async Task InvoiceScenarioAsync(BusinessFinanceApiFactory factory)
    {
        using var owner = await ClientAsync(factory, $"debt-closing-owner-{Guid.NewGuid():N}@example.test");
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");
        var category = await ExpenseCategoryAsync(owner);

        // 1 Ekim: 10.000 raf, 3.000 peşin, son gün 30 Ekim.
        var created = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Demir Mobilya", "payable", "business", "10000.0000", "10000.0000", null, "TRY", "expense",
            null, category, "2026-10-01", "2026-10-30", 1, "Raf faturası", Today,
            PlanKind: "single", UpfrontPayment: new DebtUpfrontPaymentRequest("3000.0000", bank)));
        Assert.Equal("single", created.PlanKind);
        Assert.Equal("7000.0000", created.RemainingAmount);
        var upfront = Assert.Single(created.Closings);
        Assert.Equal("payment", upfront.Kind);
        Assert.Equal("7000.0000", await BalanceAsync(owner, bank));

        // Kısmi ödeme; aynı istek kimliği ikinci bir ödeme yazmaz.
        var requestId = Guid.NewGuid();
        var payment = new RecordDebtPaymentRequest("500.0000", Today, Today, bank, ClientRequestId: requestId);
        var first = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{created.Id}/payments", payment);
        var again = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{created.Id}/payments", payment);
        Assert.Equal("6500.0000", again.RemainingAmount);
        Assert.Equal(2, again.Closings.Count);
        Assert.Equal(first.Closings.Select(item => item.Id), again.Closings.Select(item => item.Id));
        Assert.Equal("6500.0000", await BalanceAsync(owner, bank));

        // Kalanı aşan tutar reddedilir; kalan cevapta yazar.
        using var over = await owner.PostAsJsonAsync(
            $"/api/v1/debts/{created.Id}/payments",
            new RecordDebtPaymentRequest("6500.0001", Today, Today, bank));
        Assert.Equal(HttpStatusCode.BadRequest, over.StatusCode);
        Assert.Equal("debt.amount_exceeds_remaining", await CodeAsync(over));

        // Kısmen ödenmiş fatura eski yükümlülük listesinde açıktır.
        var obligations = await owner.GetFromJsonAsync<ObligationListResponse>(
            $"/api/v1/obligations?asOfDate={Today}");
        var listed = Assert.Single(obligations!.Items, item => item.Id == created.Id);
        Assert.Equal("open", listed.Status);

        // İndirim gideri fatura gününde azaltır; para hareket etmez.
        var discounted = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{created.Id}/discounts",
            new ApplyDebtDiscountRequest("1000.0000", Today, Today));
        Assert.Equal("5500.0000", discounted.RemainingAmount);
        Assert.Equal("discount", discounted.Closings[^1].Kind);
        var october = await owner.GetFromJsonAsync<MonthlyReportResponse>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("9000.0000", october!.TotalExpense);
        Assert.Equal("6500.0000", await BalanceAsync(owner, bank));

        // Akışta fatura satırı raporun saydığını yazar, indirim altında (kullanıcı kararı A).
        Assert.Equal(("obligation", "9000.0000", "1000.0000"), await InvoiceRowAsync(owner, created.Id));

        // Peşin ödemeyi geri almak yalnız onu açar: para hesaba döner, gider değişmez.
        var reversed = await SendAsync(owner, HttpMethod.Delete,
            $"/api/v1/debts/{created.Id}/payments/{upfront.Id}?asOfDate={Today}", null);
        Assert.Equal("8500.0000", reversed.RemainingAmount);
        Assert.True(reversed.Closings.Single(item => item.Id == upfront.Id).IsCancelled);
        Assert.Equal("9500.0000", await BalanceAsync(owner, bank));

        // Plan değişikliği kalanı böler; gider ve ödemeler olduğu gibi kalır.
        var replanned = await SendAsync(owner, HttpMethod.Put, $"/api/v1/debts/{created.Id}/plan",
            new ChangeDebtPlanRequest("installments", "2026-11-30", 2, Today));
        Assert.Equal("installments", replanned.PlanKind);
        Assert.Equal(["4250.0000", "4250.0000"], replanned.Installments.Select(item => item.Amount));
        Assert.Equal(["2026-11-30", "2026-12-30"], replanned.Installments.Select(item => item.DueDate));
        Assert.Equal("8500.0000", replanned.RemainingAmount);
        Assert.Equal(("debt-opening", "9000.0000", "1000.0000"), await InvoiceRowAsync(owner, created.Id));

        // İptal bir bütündür: açılış ve kalan ödemeler birlikte.
        var cancelled = await SendAsync(owner, HttpMethod.Delete, $"/api/v1/debts/{created.Id}?asOfDate={Today}", null);
        Assert.True(cancelled.IsCancelled);
        Assert.All(cancelled.Closings, item => Assert.True(item.IsCancelled));
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank));
        october = await owner.GetFromJsonAsync<MonthlyReportResponse>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("0.0000", october!.TotalExpense);
    }

    [Fact]
    public async Task CardPayment_RaisesTheCardDebtWithoutMovingTheAccountOrAddingAnExpense()
    {
        await using var factory = FixedClockApi.Create(new TestClock(Noon));
        await CardScenarioAsync(factory);
    }

    private static async Task CardScenarioAsync(BusinessFinanceApiFactory factory)
    {
        using var owner = await ClientAsync(factory, $"debt-closing-card-{Guid.NewGuid():N}@example.test");
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");
        var category = await ExpenseCategoryAsync(owner);
        using var cardResponse = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest("İş kartı", "50000.0000", "TRY", 10, 20));
        var card = (await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>())!;

        var invoice = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Test Toptancı", "payable", "business", "950.0000", "950.0000", null, "TRY", "expense",
            null, category, "2026-10-20", "2026-10-25", 1, "Nakliye faturası", Today, PlanKind: "single"));
        var paid = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{invoice.Id}/payments",
            new RecordDebtPaymentRequest("950.0000", Today, Today, CreditCardId: card.Id));

        Assert.True(paid.IsClosed);
        Assert.Equal(card.Id, paid.Closings.Single().CreditCardId);
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank));
        var cards = await owner.GetFromJsonAsync<JsonNode>("/api/v1/credit-cards");
        Assert.Equal("950.0000", cards!["items"]![0]!["currentDebt"]!.GetValue<string>());
        var october = await owner.GetFromJsonAsync<MonthlyReportResponse>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("950.0000", october!.TotalExpense);

        // Akışta kapanış satırı: kaynağı kart, sonrası kart borcu; hesap satırı yok.
        var closingId = paid.Closings.Single().Id;
        var feed = await owner.GetFromJsonAsync<JsonNode>("/api/v1/financial-activities?pageSize=100");
        var row = feed!["items"]!.AsArray().Single(item => item!["activityId"]!.GetValue<Guid>() == closingId)!;
        Assert.Equal(("obligation-settlement", "İş kartı", "950.0000"), (
            row["activityKind"]!.GetValue<string>(), row["sourceName"]!.GetValue<string>(), row["amount"]!.GetValue<string>()));
        var after = await owner.GetFromJsonAsync<JsonNode>(
            $"/api/v1/financial-activities/obligation-settlement/{closingId}/balances");
        var holders = after!["items"]!.AsArray();
        var cardAfter = Assert.Single(holders, item => item!["holder"]!.GetValue<string>() == "credit-card")!;
        Assert.Equal(("950.0000", "increased"), (cardAfter["balance"]!.GetValue<string>(), cardAfter["change"]!.GetValue<string>()));
        Assert.DoesNotContain(holders, item => item!["holder"]!.GetValue<string>() == "account");

        // Alacak kartla ödenmez.
        var income = await IncomeCategoryAsync(owner);
        var receivable = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Kaan Yapı", "receivable", "business", "4000.0000", "4000.0000", null, "TRY", "income",
            null, income, Today, "2026-11-15", 1, "Montaj işi", Today, PlanKind: "single"));
        using var wrong = await owner.PostAsJsonAsync($"/api/v1/debts/{receivable.Id}/payments",
            new RecordDebtPaymentRequest("100.0000", Today, Today, CreditCardId: card.Id));
        Assert.Equal("debt.payment_source_invalid", await CodeAsync(wrong));
    }

    [Fact]
    public async Task CarriedOverAndPlanlessDebts_WriteNoMoneyAndKeepTheirRules()
    {
        await using var factory = FixedClockApi.Create(new TestClock(Noon));
        using var owner = await ClientAsync(factory, "debt-closing-carried@example.test");
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");

        // Uygulamadan önceki kredi: kalan 18.000, 12 taksit; hesap ve gider yok.
        var carried = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Ziraat Bankası", "payable", "personal", "18000.0000", "18000.0000", null, "TRY", "cash",
            null, null, Today, "2026-11-05", 12, "İhtiyaç kredisi", Today, IsCarriedOver: true));
        Assert.True(carried.IsCarriedOver);
        Assert.Null(carried.OpeningAccountId);
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank));
        var advanced = await owner.GetFromJsonAsync<JsonNode>(
            $"/api/v1/reports/advanced?year=2026&month=10&asOfDate={Today}&daysAhead=30");
        Assert.Equal("18000.0000", advanced!["netWorth"]!["payableDebt"]!.GetValue<string>());

        // Kişisiz borç açıklama ister.
        using var anonymous = await owner.PostAsJsonAsync("/api/v1/debts", new CreateDebtRequest(
            null, "payable", "personal", "3000.0000", "3000.0000", null, "TRY", "cash",
            bank, null, Today, null, 1, null, Today, PlanKind: "none"));
        Assert.Equal("debt.description_required", await CodeAsync(anonymous));

        // Planı olmayan borç: vadesi yok, ödendikçe kapanır.
        var loose = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Kuzen Ali", "payable", "personal", "3000.0000", "3000.0000", null, "TRY", "cash",
            bank, null, Today, null, 1, null, Today, PlanKind: "none"));
        Assert.Equal("none", loose.PlanKind);
        Assert.Null(loose.FirstDueDate);
        Assert.Equal("open", Assert.Single(loose.Installments).Status);
        Assert.Equal("13000.0000", await BalanceAsync(owner, bank));

        // Kişisiz borç da akışta görünür; devreden borç açılış satırı yazmaz.
        var nobody = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            null, "payable", "personal", "500.0000", "500.0000", null, "TRY", "cash",
            bank, null, Today, null, 1, "Komşudan borç", Today, PlanKind: "none"));
        var paidBack = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{nobody.Id}/payments",
            new RecordDebtPaymentRequest("200.0000", Today, Today, AccountId: bank));
        var feed = (await owner.GetFromJsonAsync<JsonNode>("/api/v1/financial-activities?pageSize=100"))!["items"]!.AsArray();
        Assert.DoesNotContain(feed, item => item!["activityId"]!.GetValue<Guid>() == carried.Id);
        var opening = Assert.Single(feed, item => item!["activityId"]!.GetValue<Guid>() == nobody.Id)!;
        Assert.Equal(("debt-opening", "Komşudan borç"), (opening["activityKind"]!.GetValue<string>(), opening["title"]!.GetValue<string>()));
        var repayment = Assert.Single(feed, item => item!["activityId"]!.GetValue<Guid>() == paidBack.Closings.Single().Id)!;
        Assert.Equal(("debt-payment", "200.0000"), (repayment["activityKind"]!.GetValue<string>(), repayment["amount"]!.GetValue<string>()));

        // Devredilmiş fatura borcu: gider yazmaz, ama net varlıkta ve planlananlarda durur.
        string PayableDebt(JsonNode report) => report["netWorth"]!["payableDebt"]!.GetValue<string>();
        var before = (await owner.GetFromJsonAsync<JsonNode>(
            $"/api/v1/reports/advanced?year=2026&month=10&asOfDate={Today}&daysAhead=30"))!;
        var bill = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Elektrik idaresi", "payable", "personal", "700.0000", "700.0000", null, "TRY", "expense",
            null, null, Today, "2026-11-10", 1, "Eylül faturası", Today, PlanKind: "single", IsCarriedOver: true));
        var after = (await owner.GetFromJsonAsync<JsonNode>(
            $"/api/v1/reports/advanced?year=2026&month=10&asOfDate={Today}&daysAhead=30"))!;
        Assert.Equal(decimal.Parse(PayableDebt(before), System.Globalization.CultureInfo.InvariantCulture) + 700m,
            decimal.Parse(PayableDebt(after), System.Globalization.CultureInfo.InvariantCulture));
        var october = await owner.GetFromJsonAsync<MonthlyReportResponse>("/api/v1/reports/monthly?year=2026&month=10");
        Assert.Equal("0.0000", october!.TotalExpense);
        var planned = (await owner.GetFromJsonAsync<JsonNode>(
            $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=30"))!["items"]!.AsArray();
        Assert.Contains(planned, item => item!["sourceId"]?.GetValue<Guid>() == bill.Id ||
                                         item!["actionTargetId"]?.GetValue<Guid>() == bill.Id);
        feed = (await owner.GetFromJsonAsync<JsonNode>("/api/v1/financial-activities?pageSize=100"))!["items"]!.AsArray();
        Assert.DoesNotContain(feed, item => item!["activityId"]!.GetValue<Guid>() == bill.Id);
    }

    [Fact]
    public async Task InterestBearingDebt_IsPaidInWholeInstallmentsAndKeepsItsPlan()
    {
        await using var factory = FixedClockApi.Create(new TestClock(Noon));
        using var owner = await ClientAsync(factory, "debt-closing-interest@example.test");
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");
        var loan = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Örnek Banka", "payable", "business", "1000.0000", "1100.0000", null, "TRY", "cash",
            bank, null, "2026-09-01", "2026-10-01", 2, "Kredi", Today));

        using var partial = await owner.PostAsJsonAsync($"/api/v1/debts/{loan.Id}/payments",
            new RecordDebtPaymentRequest("300.0000", Today, Today, bank));
        Assert.Equal("debt.interest_bearing", await CodeAsync(partial));
        using var replan = await owner.PutAsJsonAsync($"/api/v1/debts/{loan.Id}/plan",
            new ChangeDebtPlanRequest("single", "2026-12-01", 1, Today));
        Assert.Equal("debt.interest_bearing", await CodeAsync(replan));

        var whole = await SendAsync(owner, HttpMethod.Post, $"/api/v1/debts/{loan.Id}/payments",
            new RecordDebtPaymentRequest(loan.Installments[0].Amount, Today, Today, bank));
        Assert.Equal("paid", whole.Installments[0].Status);
        Assert.NotEqual("0.0000", whole.Closings.Single().InterestAmount);
    }

    [Fact]
    public async Task AnotherUsersDebt_AnswersLikeAMissingOne()
    {
        await using var factory = FixedClockApi.Create(new TestClock(Noon));
        using var owner = await ClientAsync(factory, "debt-closing-a@example.test");
        using var stranger = await ClientAsync(factory, "debt-closing-b@example.test");
        var bank = await AccountAsync(owner, "Ziraat Vadesiz");
        var strangerBank = await AccountAsync(stranger, "Garanti");
        var debt = await SendAsync(owner, HttpMethod.Post, "/api/v1/debts", new CreateDebtRequest(
            "Kuzen Ali", "payable", "personal", "3000.0000", "3000.0000", null, "TRY", "cash",
            bank, null, Today, null, 1, null, Today, PlanKind: "none"));

        using var read = await stranger.GetAsync($"/api/v1/debts/{debt.Id}?asOfDate={Today}");
        using var pay = await stranger.PostAsJsonAsync($"/api/v1/debts/{debt.Id}/payments",
            new RecordDebtPaymentRequest("100.0000", Today, Today, strangerBank));
        using var cancel = await stranger.DeleteAsync($"/api/v1/debts/{debt.Id}?asOfDate={Today}");
        Assert.All([read, pay, cancel], response => Assert.Equal(HttpStatusCode.NotFound, response.StatusCode));
    }

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

    private static async Task<(string Kind, string Amount, string? Discount)> InvoiceRowAsync(HttpClient client, Guid debtId)
    {
        var feed = await client.GetFromJsonAsync<JsonNode>("/api/v1/financial-activities?pageSize=100");
        var row = feed!["items"]!.AsArray().Single(item => item!["activityId"]!.GetValue<Guid>() == debtId)!;
        return (row["activityKind"]!.GetValue<string>(), row["amount"]!.GetValue<string>(),
            row["discountAmount"]?.GetValue<string>());
    }

    private static async Task<string?> CodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<JsonNode>())?["code"]?.GetValue<string>();

    private static async Task<string> BalanceAsync(HttpClient client, Guid accountId) =>
        (await client.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{accountId}"))!.Balance;

    private static async Task<Guid> ExpenseCategoryAsync(HttpClient client) =>
        await CategoryAsync(client, "expense");

    private static async Task<Guid> IncomeCategoryAsync(HttpClient client) =>
        await CategoryAsync(client, "income");

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
