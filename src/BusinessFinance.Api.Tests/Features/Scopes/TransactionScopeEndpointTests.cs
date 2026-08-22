using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
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
