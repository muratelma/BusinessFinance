using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Scopes;

/// <summary>
/// Kapsam türetme zincirinin HTTP sözleşmesindeki hâli: istek kapsamı
/// göndermek zorunda değil, ama sunucu da uydurmuyor.
/// </summary>
public sealed class TransactionScopeEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task CreateTransaction_WithoutScope_TakesTheAccountLabelAndReportsIt()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-account@example.test");
        var account = await CreateAccountAsync(client, "Dükkân kasası", "business");
        var category = await FirstExpenseCategoryAsync(client);

        using var response = await PostTransactionAsync(client, account.Id, category.Id, scope: null);
        var created = await response.Content.ReadFromJsonAsync<TransactionResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("business", created?.Scope);
    }

    [Fact]
    public async Task CreateTransaction_WithAnExplicitScope_OverridesTheAccountLabel()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-explicit@example.test");
        var account = await CreateAccountAsync(client, "Dükkân kasası", "business");
        var category = await FirstExpenseCategoryAsync(client);

        using var response = await PostTransactionAsync(
            client, account.Id, category.Id, scope: "personal");
        var created = await response.Content.ReadFromJsonAsync<TransactionResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("personal", created?.Scope);
    }

    /// <summary>
    /// Hiçbir halka dolmadığında istek reddedilir ve hareket oluşmaz. Sunucunun
    /// bir kapsam seçmesi, kullanıcının işletme netini sessizce bozardı.
    /// </summary>
    [Fact]
    public async Task CreateTransaction_WithNothingToGoOn_IsRejectedAndNothingIsWritten()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-missing@example.test");
        var account = await CreateAccountAsync(client, "Etiketsiz kasa", defaultScope: null);

        // Varsayılan kategori setinin tamamı kapsam taşıyor, yani zincir normalde
        // her zaman çözülür. Reddi görebilmek için üç halkanın da boş olduğu tek
        // durumu kurmak gerekiyor: etiketsiz hesap ve kullanıcının kendi açtığı,
        // kapsam vermediği bir kategori.
        var category = await CreateCategoryAsync(client, "Etiketsiz kalem");

        using var response = await PostTransactionAsync(client, account.Id, category.Id, scope: null);
        await AssertProblemCodeAsync(response, "transactions.scope_unresolved");
        var listed = await client.GetFromJsonAsync<TransactionListResponse>("/api/v1/transactions");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(listed!.Items);
    }

    [Fact]
    public async Task CreateTransaction_WithAnUnknownScope_IsRejectedBeforeTheUseCase()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-unknown@example.test");
        var account = await CreateAccountAsync(client, "Dükkân kasası", "business");
        var category = await FirstExpenseCategoryAsync(client);

        using var response = await PostTransactionAsync(
            client, account.Id, category.Id, scope: "household");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "transactions.invalid_scope");
    }

    /// <summary>
    /// Varsayılan kapsam yetkilidir: güncellemede boş göndermek onu kaldırır.
    /// </summary>
    [Fact]
    public async Task UpdateAccount_WithoutDefaultScope_ClearsTheLabel()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-clear@example.test");
        var account = await CreateAccountAsync(client, "Dükkân kasası", "business");

        using var update = await client.PutAsJsonAsync(
            $"/api/v1/accounts/{account.Id}",
            new UpdateAccountRequest(account.Name, IsActive: true, DefaultScope: null));
        update.EnsureSuccessStatusCode();
        var cleared = await update.Content.ReadFromJsonAsync<AccountResponse>();

        Assert.Equal("business", account.DefaultScope);
        Assert.Null(cleared?.DefaultScope);
    }

    /// <summary>
    /// Özet ekranının hero metriği tek istekte kurulabilmeli: işletme neti,
    /// şahsi taraf ve ayın toplamı aynı cevapta gelir. İstemci finansal
    /// toplamı ikinci kez hesaplamaz — çıkarmayı o yapsaydı ekrandaki sayı
    /// sunucununkiyle tutmayabilirdi.
    /// </summary>
    [Fact]
    public async Task MonthlyReport_ReportsBothSidesAndTheirSum()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-hero@example.test");
        var shop = await CreateAccountAsync(client, "Dükkân kasası", "business");
        var wallet = await CreateAccountAsync(client, "Cüzdan", "personal");
        var expenseCategory = await FirstExpenseCategoryAsync(client);
        var incomeCategory = await FirstIncomeCategoryAsync(client);

        await PostAmountAsync(client, shop.Id, incomeCategory.Id, "600.0000", "income");
        await PostAmountAsync(client, shop.Id, expenseCategory.Id, "200.0000", "expense");
        await PostAmountAsync(client, wallet.Id, expenseCategory.Id, "50.0000", "expense");

        var all = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/dashboard?year=2026&month=8");
        var breakdown = all!.ScopeBreakdown;

        Assert.NotNull(breakdown);
        Assert.Equal("600.0000", breakdown.Business.Income);
        Assert.Equal("200.0000", breakdown.Business.Expense);
        Assert.Equal("400.0000", breakdown.Business.Net);
        Assert.Equal("0.0000", breakdown.Personal.Income);
        Assert.Equal("50.0000", breakdown.Personal.Expense);
        Assert.Equal("-50.0000", breakdown.Personal.Net);
        Assert.Equal("350.0000", all.Net);
    }

    /// <summary>
    /// Filtreli okuma kırılım taşımaz: dışlanan taraf sıfır görünürdü ve
    /// ekranda "o tarafta hiç hareket yok" diye okunurdu.
    /// </summary>
    [Fact]
    public async Task MonthlyReport_WithAScopeFilter_CarriesNoBreakdown()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "scope-hero-filtered@example.test");
        var shop = await CreateAccountAsync(client, "Dükkân kasası", "business");
        var expenseCategory = await FirstExpenseCategoryAsync(client);
        await PostAmountAsync(client, shop.Id, expenseCategory.Id, "200.0000", "expense");

        var business = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/dashboard?year=2026&month=8&scope=business");

        Assert.Equal("business", business!.Scope);
        Assert.Equal("200.0000", business.TotalExpense);
        Assert.Null(business.ScopeBreakdown);
    }

    private static async Task PostAmountAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                amount,
                "TRY",
                type,
                null,
                "2026-08-09",
                null));
        response.EnsureSuccessStatusCode();
    }

    private static async Task<CategoryResponse> FirstIncomeCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income");
        return categories!.Items[0];
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);
        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: CancellationToken.None);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private static Task<HttpResponseMessage> PostTransactionAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string? scope) => client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId,
                categoryId,
                "125.5000",
                "TRY",
                "expense",
                scope,
                "2026-08-09",
                "Sentetik gider"));

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string? defaultScope)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, "cash", "TRY", "0", defaultScope));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> CreateCategoryAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest(name, "expense"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    private static async Task<CategoryResponse> FirstExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items[0];
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokens!.AccessToken);
        return client;
    }
}
