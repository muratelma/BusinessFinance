using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Profiles;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Taxes;

/// <summary>
/// Aşama 05 Grup 3: indirilebilirlik kapsamdan ayrı bir alandır, yalnız işletme
/// kapsamlı giderde sorulur ve <b>işletme netini değiştirmez</b> (ADR 0016).
/// </summary>
public sealed class TaxDeductibilityEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Grubun çıkış ölçütü: indirilemeyen bir gider de gider olarak sayılır.
    /// İndirilebilirlik matrahı ilgilendirir ve matrah bu üründe hesaplanmaz.
    /// </summary>
    [Fact]
    public async Task Deductibility_DoesNotChangeTheBusinessNet()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var deductible = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(accountId, categoryId, "100.0000", isTaxDeductible: true),
            CancellationToken.None);
        using var notDeductible = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(accountId, categoryId, "40.0000", isTaxDeductible: false),
            CancellationToken.None);
        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, deductible.StatusCode);
        Assert.Equal(HttpStatusCode.Created, notDeductible.StatusCode);

        // İki kayıt da toplama girdi; indirilemeyen olan elenmedi.
        Assert.Equal("140.0000", report?.TotalExpense);
    }

    [Fact]
    public async Task Deductibility_IsCarriedOnTheResponse()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(accountId, categoryId, "100.0000", isTaxDeductible: false),
            CancellationToken.None);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        Assert.False(document.RootElement.GetProperty("isTaxDeductible").GetBoolean());
    }

    /// <summary>
    /// Şahsi kayda soru sorulmaz; cevap sessizce saklanmaz, istek reddedilir.
    /// </summary>
    [Fact]
    public async Task AnsweringForAPersonalRecord_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(
                accountId,
                categoryId,
                "100.0000",
                isTaxDeductible: true,
                scope: "personal"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Kategorinin varsayılanı işletme giderine iner; aynı kategoriyle yazılan
    /// şahsi kayda inmez.
    /// </summary>
    [Fact]
    public async Task CategoryDefault_ReachesTheBusinessRecordAndNotThePersonalOne()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "1000"),
            CancellationToken.None);
        var account = await accountResponse.Content.ReadFromJsonAsync<AccountResponse>(
            CancellationToken.None);
        using var categoryResponse = await client.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest(
                "Ticari mal alimi",
                "expense",
                DefaultIsTaxDeductible: true),
            CancellationToken.None);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>(
            CancellationToken.None);

        using var business = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(account!.Id, category!.Id, "100.0000"),
            CancellationToken.None);
        using var personal = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            Request(account.Id, category.Id, "50.0000", scope: "personal"),
            CancellationToken.None);

        Assert.True(category.DefaultIsTaxDeductible);
        using var businessDocument = JsonDocument.Parse(
            await business.Content.ReadAsStringAsync(CancellationToken.None));
        using var personalDocument = JsonDocument.Parse(
            await personal.Content.ReadAsStringAsync(CancellationToken.None));
        Assert.True(businessDocument.RootElement.GetProperty("isTaxDeductible").GetBoolean());
        Assert.Equal(
            JsonValueKind.Null,
            personalDocument.RootElement.GetProperty("isTaxDeductible").ValueKind);
    }

    /// <summary>
    /// İşletme ön ayarıyla açılan kategori seti indirilebilirlik önerisi taşır;
    /// cevabı gerçekten muhasebecinin takdirinde olan kalem boş açılır.
    /// </summary>
    [Fact]
    public async Task BusinessCategorySet_OpensWithASuggestionAndLeavesTheJudgementCallBlank()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        using var profile = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new UpdateUserProfileRequest(true),
            CancellationToken.None);
        profile.EnsureSuccessStatusCode();

        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);

        var goods = categories!.Items.Single(item => item.Name == "Ticari mal alımı");
        var taxes = categories.Items.Single(item => item.Name == "SGK ve vergi ödemesi");
        var personal = categories.Items.Single(item => item.Name == "Market Alışverişi");

        Assert.True(goods.DefaultIsTaxDeductible);
        Assert.Null(taxes.DefaultIsTaxDeductible);
        Assert.Null(personal.DefaultIsTaxDeductible);
    }

    private static CreateTransactionRequest Request(
        Guid accountId,
        Guid categoryId,
        string amount,
        bool? isTaxDeductible = null,
        string scope = "business") => new(
        accountId,
        categoryId,
        amount,
        "TRY",
        "expense",
        scope,
        "2026-08-26",
        null,
        IsTaxDeductible: isTaxDeductible);

    private static async Task<(Guid AccountId, Guid CategoryId)>
        CreateAccountAndCategoryAsync(HttpClient client)
    {
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "1000"),
            CancellationToken.None);
        accountResponse.EnsureSuccessStatusCode();
        var account = await accountResponse.Content.ReadFromJsonAsync<AccountResponse>(
            CancellationToken.None);
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);

        return (account!.Id, categories!.Items[0].Id);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory)
    {
        var client = factory.CreateClient();
        var email = $"deductible-{Guid.NewGuid():N}@example.test";
        using var registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password),
            CancellationToken.None);
        registerResponse.EnsureSuccessStatusCode();
        using var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password),
            CancellationToken.None);
        loginResponse.EnsureSuccessStatusCode();
        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None) ??
            throw new InvalidOperationException("Token response was empty.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokens.AccessToken);

        return client;
    }
}
