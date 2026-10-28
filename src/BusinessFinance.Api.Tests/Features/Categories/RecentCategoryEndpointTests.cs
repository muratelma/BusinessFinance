using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.Transfers;

namespace BusinessFinance.Api.Tests.Features.Categories;

/// <summary>
/// Kategori seçicinin `Son kullanılanlar` bölümü
/// (`GET /api/v1/categories/recent`): kategori + kapsam çifti, en yeni önce,
/// en çok dört; yalnız kullanıcının seçtiği kategoriler.
/// </summary>
public sealed class RecentCategoryEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// İki kapsama açık kategori iki ayrı satırdır; liste en yeni kayıtla
    /// başlar, dörtte kesilir ve öbür türü içermez.
    /// </summary>
    [Fact]
    public async Task Recent_ListsCategoryAndScopePairsNewestFirst()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "recent-pairs@example.test");
        var account = await CreateAccountAsync(client, "Kasa");
        var expenses = await CategoriesAsync(client, "expense");
        var interest = Named(expenses, "Faiz ve finansman gideri");
        var bills = Named(expenses, "İşyeri faturaları");
        var market = Named(expenses, "Yeme-içme");
        var fuel = Named(expenses, "Araç ve yakıt");
        var income = Named(await CategoriesAsync(client, "income"), "Satış geliri");

        await PostAsync(client, account.Id, fuel.Id, "expense", scope: null);
        await PostAsync(client, account.Id, interest.Id, "expense", "business");
        await PostAsync(client, account.Id, bills.Id, "expense", scope: null);
        await PostAsync(client, account.Id, interest.Id, "expense", "personal");
        await PostAsync(client, account.Id, market.Id, "expense", scope: null);
        await PostAsync(client, account.Id, income.Id, "income", scope: null);

        var recent = await RecentAsync(client, "expense");

        Assert.Equal(
            [
                (market.Id, "personal"),
                (interest.Id, "personal"),
                (bills.Id, "business"),
                (interest.Id, "business"),
            ],
            recent.Select(item => (item.CategoryId, item.Scope)).ToArray());
    }

    /// <summary>
    /// Bir kaydın parçası olan kalem (transfer masrafı) kullanıcının seçtiği
    /// bir kategori değildir; iptal edilen kayıt ve pasif kategori de sayılmaz.
    /// </summary>
    [Fact]
    public async Task Recent_SkipsRecordPartsCancelledRecordsAndInactiveCategories()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "recent-skips@example.test");
        var cash = await CreateAccountAsync(client, "Kasa");
        var bank = await CreateAccountAsync(client, "Banka");
        var expenses = await CategoriesAsync(client, "expense");
        var bills = Named(expenses, "İşyeri faturaları");
        var fuel = Named(expenses, "Araç ve yakıt");
        var rent = Named(expenses, "İşyeri kirası");

        await PostAsync(client, cash.Id, bills.Id, "expense", scope: null);
        var cancelled = await PostAsync(client, cash.Id, fuel.Id, "expense", scope: null);
        (await client.DeleteAsync($"/api/v1/transactions/{cancelled.Id}")).EnsureSuccessStatusCode();
        await PostAsync(client, cash.Id, rent.Id, "expense", scope: null);
        (await client.PutAsJsonAsync(
            $"/api/v1/categories/{rent.Id}",
            new UpdateCategoryRequest(rent.Name, IsActive: false, rent.DefaultScope)))
            .EnsureSuccessStatusCode();
        using var transfer = await client.PostAsJsonAsync(
            "/api/v1/transfers",
            new CreateTransferRequest(
                cash.Id, bank.Id, "100.0000", "TRY", "2026-08-10", null,
                FeeAmount: "5.0000", FeeScope: "business"));
        transfer.EnsureSuccessStatusCode();

        var recent = await RecentAsync(client, "expense");

        Assert.Equal([(bills.Id, "business")], recent.Select(item => (item.CategoryId, item.Scope)).ToArray());
    }

    /// <summary>Başka kullanıcının kayıtları hiç görünmez.</summary>
    [Fact]
    public async Task Recent_ReadsOnlyTheCurrentUsersRecords()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await AuthenticateAsync(factory, "recent-owner@example.test");
        using var other = await AuthenticateAsync(factory, "recent-other@example.test");
        var account = await CreateAccountAsync(owner, "Kasa");
        var bills = Named(await CategoriesAsync(owner, "expense"), "İşyeri faturaları");
        await PostAsync(owner, account.Id, bills.Id, "expense", scope: null);

        Assert.Single(await RecentAsync(owner, "expense"));
        Assert.Empty(await RecentAsync(other, "expense"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("?type=transfer")]
    public async Task Recent_RequiresAValidType(string query)
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, $"recent-type{query.Length}@example.test");

        using var response = await client.GetAsync("/api/v1/categories/recent" + query);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Recent_RequiresAuthentication()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/categories/recent?type=expense");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static CategoryResponse Named(IReadOnlyList<CategoryResponse> items, string name) =>
        Assert.Single(items, item => item.Name == name);

    private static async Task<IReadOnlyList<CategoryResponse>> CategoriesAsync(
        HttpClient client,
        string type) =>
        (await client.GetFromJsonAsync<CategoryListResponse>($"/api/v1/categories?type={type}"))!.Items;

    private static async Task<IReadOnlyList<RecentCategoryResponse>> RecentAsync(
        HttpClient client,
        string type) =>
        (await client.GetFromJsonAsync<RecentCategoryListResponse>(
            $"/api/v1/categories/recent?type={type}"))!.Items;

    private static async Task<TransactionResponse> PostAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string type,
        string? scope)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, "40.0000", "TRY", type, scope, "2026-08-09", null));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, "cash", "TRY", "1000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, true));
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
