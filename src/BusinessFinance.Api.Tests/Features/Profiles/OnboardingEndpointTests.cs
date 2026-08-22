using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Profiles;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Profiles;

/// <summary>
/// Onboarding'in tek sorusu ve sonuçları: hangi kategori seti kuruluyor,
/// kapsam boyutu görünmeli mi, kayıtlar kapsamsız girilebiliyor mu.
/// </summary>
public sealed class OnboardingEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task RegisteringWithABusiness_InstallsTheTradeSetWithBusinessScopes()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "trader@example.test", hasBusiness: true);

        var profile = await client.GetFromJsonAsync<UserProfileResponse>("/api/v1/profile");
        var categories = await client.GetFromJsonAsync<CategoryListResponse>("/api/v1/categories");

        Assert.True(profile?.HasBusiness);
        Assert.Contains(categories!.Items, item =>
            item.Name == "Satış geliri" && item.DefaultScope == "business");
        Assert.Contains(categories.Items, item =>
            item.Name == "Ticari mal alımı" && item.DefaultScope == "business");

        // Patronun gündelik hayatı da aynı uygulamaya giriyor.
        Assert.Contains(categories.Items, item =>
            item.Name == "Market Alışverişi" && item.DefaultScope == "personal");
    }

    [Fact]
    public async Task RegisteringWithoutABusiness_InstallsThePersonalSetAndEveryCategoryIsPersonal()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "household@example.test", hasBusiness: false);

        var profile = await client.GetFromJsonAsync<UserProfileResponse>("/api/v1/profile");
        var categories = await client.GetFromJsonAsync<CategoryListResponse>("/api/v1/categories");

        Assert.False(profile?.HasBusiness);
        Assert.Contains(categories!.Items, item => item.Name == "Maaş");
        Assert.DoesNotContain(categories.Items, item => item.Name == "Satış geliri");
        Assert.All(categories.Items, item => Assert.Equal("personal", item.DefaultScope));
    }

    /// <summary>
    /// Kategori seti kapsam taşıdığı için, istemci kapsam göndermeden kayıt
    /// oluşturabiliyor: türetme zincirinin son halkası artık dolu.
    /// </summary>
    [Theory]
    [InlineData(true, "business")]
    [InlineData(false, "personal")]
    public async Task ATransactionWithoutAScope_ResolvesFromTheInstalledCategorySet(
        bool hasBusiness,
        string expectedScope)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(
            factory, $"resolve-{expectedScope}@example.test", hasBusiness);
        var account = await CreateAccountAsync(client);
        var category = await FirstExpenseCategoryAsync(client, expectedScope);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, category.Id, "40.0000", "TRY", "expense", null, "2026-08-09", null));
        var created = await response.Content.ReadFromJsonAsync<TransactionResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(expectedScope, created?.Scope);
    }

    /// <summary>
    /// Cevap sonradan değiştirilebilir ama kategori seti değişmez: o noktada
    /// liste artık kullanıcınındır ve silinmiş bir kategoriyi geri getirmek
    /// silme eylemini anlamsız kılardı.
    /// </summary>
    [Fact]
    public async Task ChangingTheAnswerLater_MovesTheProfileAndLeavesTheCategoriesAlone()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "switcher@example.test", hasBusiness: false);
        var before = await client.GetFromJsonAsync<CategoryListResponse>("/api/v1/categories");

        using var update = await client.PutAsJsonAsync(
            "/api/v1/profile",
            new UpdateUserProfileRequest(HasBusiness: true));
        update.EnsureSuccessStatusCode();
        var profile = await update.Content.ReadFromJsonAsync<UserProfileResponse>();
        var after = await client.GetFromJsonAsync<CategoryListResponse>("/api/v1/categories");

        Assert.True(profile?.HasBusiness);
        Assert.Equal(before!.Items.Count, after!.Items.Count);
        Assert.DoesNotContain(after.Items, item => item.Name == "Satış geliri");
    }

    [Fact]
    public async Task Profile_RequiresAuthentication()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "0"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> FirstExpenseCategoryAsync(
        HttpClient client,
        string scope)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        return categories!.Items.First(item => item.DefaultScope == scope);
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email,
        bool hasBusiness)
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
