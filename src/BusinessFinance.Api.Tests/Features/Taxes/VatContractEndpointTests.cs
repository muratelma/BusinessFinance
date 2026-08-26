using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Taxes;

/// <summary>
/// Aşama 05 Grup 2: KDV sözleşmede taşınan bir bilgidir. Sunucu ne oranı
/// tutardan ne tutarı orandan türetir ve KDV hiçbir toplamı değiştirmez
/// (ADR 0016).
/// </summary>
public sealed class VatContractEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Grubun çıkış ölçütü: KDV alanı boş bırakılabilir ve cevapta içi boş bir
    /// nesne değil, <c>null</c> döner.
    /// </summary>
    [Fact]
    public async Task Transaction_WithoutVat_CarriesNoVatAtAll()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "120.0000",
                "TRY",
                "expense",
                "business",
                "2026-08-26",
                "Vergisiz gider"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        Assert.Equal(
            JsonValueKind.Null,
            document.RootElement.GetProperty("vat").ValueKind);
    }

    /// <summary>
    /// Yalnız oran girilirse tutar boş kalır: sunucu bölme yapmaz.
    /// </summary>
    [Fact]
    public async Task Transaction_WithOnlyARate_DoesNotFillTheAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "120.0000",
                "TRY",
                "expense",
                "business",
                "2026-08-26",
                "Orani bilinen belge",
                VatRate: "0.2000"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        var vat = document.RootElement.GetProperty("vat");
        Assert.Equal("0.2000", vat.GetProperty("rate").GetString());
        Assert.Equal(JsonValueKind.Null, vat.GetProperty("amount").ValueKind);
    }

    /// <summary>
    /// Oran ile tutar birbirini tutmasa bile kayıt olduğu gibi durur ve gider
    /// toplamı brüt tutar kadar kalır: KDV hiçbir toplamı bölmez.
    /// </summary>
    [Fact]
    public async Task MismatchedVat_IsStoredAsWritten_AndTheReportStaysGross()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "120.0000",
                "TRY",
                "expense",
                "business",
                "2026-08-26",
                "Karisik oranli fis",
                VatRate: "0.2000",
                VatAmount: "10.0000"),
            CancellationToken.None);
        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        var vat = document.RootElement.GetProperty("vat");
        Assert.Equal("0.2000", vat.GetProperty("rate").GetString());
        Assert.Equal("10.0000", vat.GetProperty("amount").GetString());

        // Gider brüt tutar kadar; KDV ondan düşülmedi.
        Assert.Equal("120.0000", report?.TotalExpense);
    }

    [Theory]
    [InlineData("0.20001", null)]
    [InlineData(null, "1.00001")]
    [InlineData("ceyrek", null)]
    public async Task VatThatCannotBeRead_IsRejectedWithoutGuessing(
        string? rate,
        string? amount)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "120.0000",
                "TRY",
                "expense",
                "business",
                "2026-08-26",
                null,
                VatRate: rate,
                VatAmount: amount),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        Assert.Equal(
            "transactions.invalid_vat",
            document.RootElement.GetProperty("code").GetString());
    }

    /// <summary>
    /// KDV tutarı kaydın tutarını aşamaz; sunucu bunu bir sınır olarak uygular,
    /// değeri kırpmaz.
    /// </summary>
    [Fact]
    public async Task VatAmountAboveTheRecordAmount_IsRejectedNotClamped()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var (accountId, categoryId) = await CreateAccountAndCategoryAsync(client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "120.0000",
                "TRY",
                "expense",
                "business",
                "2026-08-26",
                null,
                VatAmount: "120.0001"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

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
        var email = $"vat-{Guid.NewGuid():N}@example.test";
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
