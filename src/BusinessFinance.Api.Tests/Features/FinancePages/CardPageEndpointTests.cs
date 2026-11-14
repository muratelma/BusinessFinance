using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;

namespace BusinessFinance.Api.Tests.Features.FinancePages;

/// <summary>
/// Kartın hareketler sayfası: özet, kategori toplamları ve satırlar tek
/// okumadan gelir; tür, kapsam, kategori ve dönem aynı süzgeçtir.
/// </summary>
public sealed class CardPageEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Page_SumsSpendingAndPayments_AndFiltersByKindCategoryAndPeriod()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await ClientAsync(factory, "card-page@example.test");
        var card = await PostAsync<CreditCardResponse>(client, "/api/v1/credit-cards",
            new CreateCreditCardRequest("İşletme kartı", "50000", "TRY", 15, 25));
        var categories = (await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense"))!.Items.Where(c => c.DefaultScope == "business").ToArray();
        var first = categories[0];
        var second = categories[1];
        var account = await PostAsync<AccountResponse>(client, "/api/v1/accounts",
            new CreateAccountRequest("Ziraat", "bank", "TRY", "10000", "business"));

        await PostAsync<CardChargeResponse>(client, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(first.Id, "1800.0000", "TRY", null, "2026-08-08", "Araç bakımı"));
        await PostAsync<CardChargeResponse>(client, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(second.Id, "750.0000", "TRY", null, "2026-07-20", null));
        var plan = await PostAsync<InstallmentPlanResponse>(client, "/api/v1/installment-plans",
            new CreateInstallmentPlanRequest(card.Id, first.Id, Guid.NewGuid(), "900.0000", "TRY",
                null, 3, "2026-08-12", "Soğutucu dolap"));
        using (var realize = await client.PostAsync(
            $"/api/v1/installment-plans/{plan.Id}/items/1/realize", null))
        {
            realize.EnsureSuccessStatusCode();
        }
        await PostAsync<JsonElement>(client, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(account.Id, "1000.0000", "TRY", "2026-08-01", null));

        var all = await PageAsync(client, card.Id, "all=true");
        Assert.Equal("2850.0000", all.GetProperty("spending").GetString());
        Assert.Equal("1000.0000", all.GetProperty("payments").GetString());
        Assert.Equal("all", all.GetProperty("period").GetProperty("kind").GetString());
        Assert.Equal(4, all.GetProperty("totalCount").GetInt32());
        var rows = all.GetProperty("items").EnumerateArray().ToArray();
        // En yeni üstte: taksit (12 Ağustos), harcama (8), ödeme (1), harcama (20 Temmuz).
        Assert.Equal("-300.0000", rows[0].GetProperty("amount").GetString());
        Assert.Equal(1, rows[0].GetProperty("installmentSequence").GetInt32());
        Assert.Equal(3, rows[0].GetProperty("installmentCount").GetInt32());
        Assert.Equal("1000.0000", rows[2].GetProperty("amount").GetString());
        Assert.Equal(JsonValueKind.Null, rows[1].GetProperty("installmentSequence").ValueKind);
        var options = all.GetProperty("categories").EnumerateArray().ToArray();
        Assert.Equal("2850.0000", options[0].GetProperty("amount").GetString());
        Assert.Equal($"category:{first.Id:D}", options[1].GetProperty("key").GetString());
        Assert.Equal("2100.0000", options[1].GetProperty("amount").GetString());

        var charges = await PageAsync(client, card.Id, "all=true&kind=charge");
        Assert.Equal("2550.0000", charges.GetProperty("spending").GetString());
        Assert.Equal(JsonValueKind.Null, charges.GetProperty("payments").ValueKind);

        var installments = await PageAsync(client, card.Id, "all=true&kind=installment");
        Assert.Equal(1, installments.GetProperty("totalCount").GetInt32());

        var payments = await PageAsync(client, card.Id, "all=true&kind=payment");
        Assert.Equal("0.0000", payments.GetProperty("spending").GetString());
        Assert.Equal("1000.0000", payments.GetProperty("payments").GetString());

        var category = await PageAsync(client, card.Id, $"all=true&category={second.Id}");
        Assert.Equal("750.0000", category.GetProperty("spending").GetString());
        Assert.Equal(JsonValueKind.Null, category.GetProperty("payments").ValueKind);

        // Ağustos ekstresi: 16 Temmuz – 15 Ağustos.
        var statement = await PageAsync(client, card.Id, "year=2026&month=8");
        var period = statement.GetProperty("period");
        Assert.Equal("statement", period.GetProperty("kind").GetString());
        Assert.Equal("2026-07-16", period.GetProperty("start").GetString());
        Assert.Equal("2026-08-15", period.GetProperty("end").GetString());
        Assert.Equal("2850.0000", statement.GetProperty("spending").GetString());
        Assert.Equal(24, statement.GetProperty("statements").GetArrayLength());

        var range = await PageAsync(client, card.Id, "from=2026-08-02&to=2026-08-31");
        Assert.Equal("range", range.GetProperty("period").GetProperty("kind").GetString());
        Assert.Equal(2, range.GetProperty("totalCount").GetInt32());

        var preview = await PageAsync(client, card.Id, "all=true&limit=1");
        Assert.Single(preview.GetProperty("items").EnumerateArray());
        Assert.True(preview.GetProperty("hasMore").GetBoolean());
    }

    [Theory]
    [InlineData("kind=refund")]
    [InlineData("scope=unknown")]
    [InlineData("year=2026")]
    [InlineData("from=2026-08-10&to=2026-08-01")]
    [InlineData("all=true&year=2026&month=8")]
    [InlineData("limit=51")]
    public async Task Page_RejectsAnInvalidFilter(string query)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await ClientAsync(factory, "card-page-invalid@example.test");
        var card = await PostAsync<CreditCardResponse>(client, "/api/v1/credit-cards",
            new CreateCreditCardRequest("Kart", "1000", "TRY", 10, 20));
        using var response = await client.GetAsync($"/api/v1/credit-cards/{card.Id}/page?{query}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Preview_SplitsLikeThePlanAndReturnsEachCount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await ClientAsync(factory, "installment-preview@example.test");
        var body = await client.GetFromJsonAsync<InstallmentPreviewResponse>(
            "/api/v1/installment-plans/preview?totalAmount=6500.0000&firstInstallmentDate=2026-11-12&counts=2,9");
        Assert.Equal(2, body!.Options.Count);
        var nine = body.Options[1];
        Assert.Equal(9, nine.InstallmentCount);
        Assert.Equal("722.2222", nine.InstallmentAmount);
        Assert.Equal("722.2224", nine.LastInstallmentAmount);
        Assert.Equal("2026-11-12", nine.FirstInstallmentDate);
        Assert.Equal("2027-07-12", nine.LastInstallmentDate);

        using var invalid = await client.GetAsync(
            "/api/v1/installment-plans/preview?totalAmount=6500&firstInstallmentDate=2026-11-12&counts=1");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    private static async Task<JsonElement> PageAsync(HttpClient client, Guid cardId, string query)
    {
        using var response = await client.GetAsync($"/api/v1/credit-cards/{cardId}/page?{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string path, object body)
    {
        using var response = await client.PostAsJsonAsync(path, body);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task<HttpClient> ClientAsync(BusinessFinanceApiFactory factory, string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", tokens!.AccessToken);
        return client;
    }
}
