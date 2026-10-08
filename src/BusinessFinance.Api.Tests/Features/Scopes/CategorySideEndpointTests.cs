using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Scopes;

/// <summary>
/// Kategorinin tarafı (ADR 0020 İ6, İ8, İ12): varsayılan setin hangi kalemi
/// hangi tarafa açtığı, kullanıcının açtığı kategorinin tarafı ve tarafın
/// yalnız genişleyebilmesi.
/// </summary>
public sealed class CategorySideEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Adı iki tarafta aynı anlama gelen üç kalem iki tarafa açık kurulur;
    /// çift hâlindeki kalemler adlarıyla taraflarını söyler.
    /// </summary>
    [Fact]
    public async Task TradeSet_OpensTheSharedItemsToBothSidesAndNamesThePairs()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "set-sides@example.test");

        var expenses = await ExpensesAsync(client);

        Assert.Null(Named(expenses, "SGK ve vergi ödemesi").DefaultScope);
        Assert.Null(Named(expenses, "Faiz ve finansman gideri").DefaultScope);
        Assert.Null(Named(expenses, "Sigorta").DefaultScope);
        Assert.Equal("business", Named(expenses, "İşyeri faturaları").DefaultScope);
        Assert.Equal("business", Named(expenses, "Personel giderleri").DefaultScope);
        Assert.Equal("personal", Named(expenses, "Ev faturaları").DefaultScope);
        Assert.Equal("personal", Named(expenses, "Ev kirası ve aidat").DefaultScope);
        Assert.DoesNotContain(expenses, item => item.Name is "Konut" or "Faturalar");
    }

    /// <summary>
    /// İşletmesi olmayan kullanıcıya taraf sorulmaz; açtığı kategori şahsi
    /// yazılır. İşletmesi olan kullanıcıda boş taraf "iki tarafa açık"tır.
    /// </summary>
    [Theory]
    [InlineData(false, "personal")]
    [InlineData(true, null)]
    public async Task ANewCategoryWithoutASide_IsPersonalOnlyForAUserWithoutABusiness(
        bool hasBusiness,
        string? expectedSide)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(
            factory, $"new-category-{hasBusiness}@example.test", hasBusiness);

        var created = await CreateCategoryAsync(client, "Kendi kalemim", defaultScope: null);

        Assert.Equal(expectedSide, created.DefaultScope);
    }

    /// <summary>
    /// Tek taraflı kategori her zaman iki tarafa açılabilir: yazılmış kayıt
    /// yerinde kalır, yenisinde taraf sorulur.
    /// </summary>
    [Fact]
    public async Task ACategory_CanAlwaysBeOpenedToBothSides()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "category-widen@example.test");
        var account = await CreateAccountAsync(client);
        var category = Named(await ExpensesAsync(client), "Ulaşım");
        await PostExpenseAsync(client, account.Id, category.Id, scope: null);

        using var widen = await UpdateAsync(client, category, defaultScope: null);
        using var business = await PostExpenseResponseAsync(
            client, account.Id, category.Id, scope: "business");

        Assert.Equal(HttpStatusCode.OK, widen.StatusCode);
        Assert.Null((await widen.Content.ReadFromJsonAsync<CategoryResponse>())!.DefaultScope);
        Assert.Equal(HttpStatusCode.Created, business.StatusCode);
    }

    /// <summary>
    /// Daraltmak ve çevirmek yalnız öbür tarafta kayıt yoksa mümkündür; aksi
    /// hâlde yazılmış kayıt kategorisinin izin vermediği bir tarafta dururdu.
    /// </summary>
    [Fact]
    public async Task ACategory_CannotBeNarrowedOrFlippedAwayFromItsRecords()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "category-narrow@example.test");
        var account = await CreateAccountAsync(client);
        var open = await CreateCategoryAsync(client, "Araç bakımı", defaultScope: null);
        await PostExpenseAsync(client, account.Id, open.Id, scope: "business");

        using var toPersonal = await UpdateAsync(client, open, defaultScope: "personal");
        using var toBusiness = await UpdateAsync(client, open, defaultScope: "business");
        using var flip = await UpdateAsync(client, open, defaultScope: "personal");

        Assert.Equal(HttpStatusCode.Conflict, toPersonal.StatusCode);
        Assert.Contains("categories.scope_in_use", await toPersonal.Content.ReadAsStringAsync());

        // Kayıtların hepsi işletme tarafında: işletmeye daraltmak serbest,
        // sonra şahsiye çevirmek yine yasak.
        Assert.Equal(HttpStatusCode.OK, toBusiness.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, flip.StatusCode);
    }

    /// <summary>
    /// İptal edilmiş kayıt tarafı tutmaz: düzeltme "iptal et, doğrusunu yaz"
    /// ile yapıldığı için yanlış taraftaki kayıt iptal edilince kategori
    /// daraltılabilmelidir.
    /// </summary>
    [Fact]
    public async Task ACancelledRecord_DoesNotHoldTheCategoryOpen()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "category-cancelled@example.test");
        var account = await CreateAccountAsync(client);
        var open = await CreateCategoryAsync(client, "Kırtasiye", defaultScope: null);
        var wrong = await PostExpenseAsync(client, account.Id, open.Id, scope: "business");

        using var blocked = await UpdateAsync(client, open, defaultScope: "personal");
        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{wrong.Id}");
        using var narrowed = await UpdateAsync(client, open, defaultScope: "personal");

        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        Assert.True(cancel.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, narrowed.StatusCode);
    }

    private static CategoryResponse Named(IReadOnlyList<CategoryResponse> items, string name) =>
        Assert.Single(items, item => item.Name == name);

    private static async Task<IReadOnlyList<CategoryResponse>> ExpensesAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items;
    }

    private static async Task<CategoryResponse> CreateCategoryAsync(
        HttpClient client,
        string name,
        string? defaultScope)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest(name, "expense", defaultScope));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    private static Task<HttpResponseMessage> UpdateAsync(
        HttpClient client,
        CategoryResponse category,
        string? defaultScope) => client.PutAsJsonAsync(
            $"/api/v1/categories/{category.Id}",
            new UpdateCategoryRequest(category.Name, IsActive: true, defaultScope));

    private static async Task<TransactionResponse> PostExpenseAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string? scope)
    {
        using var response = await PostExpenseResponseAsync(client, accountId, categoryId, scope);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static Task<HttpResponseMessage> PostExpenseResponseAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string? scope) => client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, "40.0000", "TRY", "expense", scope, "2026-08-09", null));

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "0"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email,
        bool hasBusiness = true)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, hasBusiness));
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
