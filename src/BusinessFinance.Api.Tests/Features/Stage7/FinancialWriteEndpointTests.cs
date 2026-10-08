using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Api.Tests.Features.Stage7;

public sealed class FinancialWriteEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task AccountOpeningBalanceAndMovements_ProduceCalculatedBalance()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, "balance@example.test");
        var account = await CreateAccountAsync(client, "Main", "1000.2500");
        var category = await GetCategoryAsync(client, "income");

        var transaction = await CreateTransactionAsync(
            client,
            account.Id,
            category.Id,
            "250.5000",
            "income");

        var withIncome = await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1000.2500", withIncome?.OpeningBalance);
        Assert.Equal("1250.7500", withIncome?.Balance);

        using var cancelResponse = await client.DeleteAsync(
            $"/api/v1/transactions/{transaction.Id}");
        cancelResponse.EnsureSuccessStatusCode();
        var cancelled = await cancelResponse.Content.ReadFromJsonAsync<TransactionResponse>();
        Assert.True(cancelled?.IsCancelled);

        var afterCancellation = await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1000.2500", afterCancellation?.Balance);
    }

    [Fact]
    public async Task Categories_AreOwnerScopedSeededAndDuplicateTypeNameIsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var first = await CreateAuthenticatedClientAsync(
            factory, "categories-a@example.test", hasBusiness: false);
        using var second = await CreateAuthenticatedClientAsync(
            factory, "categories-b@example.test", hasBusiness: false);

        var firstDefaults = await first.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories");
        var secondDefaults = await second.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories");
        // Sayı listeyle birlikte değişir; sabitlemek kategori eklenince
        // düşen ama hiçbir hata göstermeyen bir test bırakırdı. Sabit olan
        // her iki kullanıcının aynı seti alması ve kimliklerin ayrı olması.
        Assert.Equal(firstDefaults?.Items.Count, secondDefaults?.Items.Count);
        Assert.True(firstDefaults!.Items.Count >= 8);

        // Kişisel bütçede ayrı izlenmesi beklenen birkaç kalem örnekleme
        // olarak aranıyor; hepsini saymak listeyi ikinci kez yazmak olurdu.
        var names = firstDefaults.Items.Select(item => item.Name).ToArray();
        Assert.Contains("Maaş", names);
        Assert.Contains("Market Alışverişi", names);
        Assert.Contains("Faiz ve finansman gideri", names);
        Assert.Contains("Yatırım getirisi", names);

        // Aynı ad iki tipte birden bulunabilir: hediye hem alınır hem verilir.
        Assert.Equal(2, names.Count(name => name == "Hediye"));
        Assert.Empty(firstDefaults!.Items.Select(item => item.Id)
            .Intersect(secondDefaults!.Items.Select(item => item.Id)));

        using var create = await first.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest("Education", "expense"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        using var duplicate = await first.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest("education", "expense"));
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [Fact]
    public async Task Categories_TranslateOnlyLegacyDefaultNamesForExistingUser()
    {
        const string email = "category-translation@example.test";
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, email);
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
            var userId = await dbContext.Users
                .Where(user => user.Email == email)
                .Select(user => user.Id)
                .SingleAsync();
            dbContext.Categories.AddRange(
                new Category(Guid.NewGuid(), userId, "Salary", CategoryType.Income),
                new Category(Guid.NewGuid(), userId, "Groceries", CategoryType.Expense),
                new Category(Guid.NewGuid(), userId, "Evcil Hayvan", CategoryType.Expense));
            await dbContext.SaveChangesAsync();
        }

        var response = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories");

        Assert.Equal(
            ["Evcil Hayvan", "Maaş", "Market Alışverişi"],
            response!.Items.Select(item => item.Name).Order(StringComparer.Ordinal));
    }

    [Fact]
    public async Task TransactionWrite_RejectsOtherUsersResourcesInactiveResourcesAndTypeMismatch()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "write-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "write-other@example.test");
        var account = await CreateAccountAsync(owner, "Owner Account", "0");
        var expense = await GetCategoryAsync(owner, "expense");
        var otherAccount = await CreateAccountAsync(other, "Other Account", "0");
        var otherExpense = await GetCategoryAsync(other, "expense");

        using var foreignAccount = await CreateTransactionResponseAsync(
            other,
            account.Id,
            otherExpense.Id,
            "10",
            "expense");
        Assert.Equal(HttpStatusCode.BadRequest, foreignAccount.StatusCode);

        using var foreignCategory = await CreateTransactionResponseAsync(
            other,
            otherAccount.Id,
            expense.Id,
            "10",
            "expense");
        Assert.Equal(HttpStatusCode.BadRequest, foreignCategory.StatusCode);

        using var mismatch = await CreateTransactionResponseAsync(
            owner,
            account.Id,
            expense.Id,
            "10",
            "income");
        Assert.Equal(HttpStatusCode.BadRequest, mismatch.StatusCode);

        using var deactivate = await owner.PutAsJsonAsync(
            $"/api/v1/accounts/{account.Id}",
            new UpdateAccountRequest(account.Name, false));
        deactivate.EnsureSuccessStatusCode();
        using var inactive = await CreateTransactionResponseAsync(
            owner,
            account.Id,
            expense.Id,
            "10",
            "expense");
        Assert.Equal(HttpStatusCode.BadRequest, inactive.StatusCode);

        var activeAccount = await CreateAccountAsync(owner, "Active Account", "0");
        using var deactivateCategory = await owner.PutAsJsonAsync(
            $"/api/v1/categories/{expense.Id}",
            new UpdateCategoryRequest(expense.Name, false));
        deactivateCategory.EnsureSuccessStatusCode();
        using var inactiveCategory = await CreateTransactionResponseAsync(
            owner,
            activeAccount.Id,
            expense.Id,
            "10",
            "expense");
        Assert.Equal(HttpStatusCode.BadRequest, inactiveCategory.StatusCode);
    }

    [Fact]
    public async Task AccountDeletion_DeletesOnlyOwnedAccountsWithoutFinancialHistory()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "delete-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "delete-other@example.test");
        var unused = await CreateAccountAsync(owner, "Unused Account", "500.0000");

        using var foreignDelete = await other.DeleteAsync($"/api/v1/accounts/{unused.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignDelete.StatusCode);

        using var unusedDelete = await owner.DeleteAsync($"/api/v1/accounts/{unused.Id}");
        Assert.Equal(HttpStatusCode.NoContent, unusedDelete.StatusCode);
        using var deletedGet = await owner.GetAsync($"/api/v1/accounts/{unused.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deletedGet.StatusCode);

        var used = await CreateAccountAsync(owner, "Used Account", "0");
        var expense = await GetCategoryAsync(owner, "expense");
        await CreateTransactionAsync(owner, used.Id, expense.Id, "10", "expense");

        using var usedDelete = await owner.DeleteAsync($"/api/v1/accounts/{used.Id}");
        Assert.Equal(HttpStatusCode.Conflict, usedDelete.StatusCode);
        using var existingGet = await owner.GetAsync($"/api/v1/accounts/{used.Id}");
        Assert.Equal(HttpStatusCode.OK, existingGet.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
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

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, "cash", "TRY", openingBalance));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetCategoryAsync(HttpClient client, string type)
    {
        var response = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return response!.Items.First(item => item.DefaultScope == "business");
    }

    private static async Task<TransactionResponse> CreateTransactionAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type)
    {
        using var response = await CreateTransactionResponseAsync(
            client,
            accountId,
            categoryId,
            amount,
            type);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static Task<HttpResponseMessage> CreateTransactionResponseAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type) => client.PostAsJsonAsync(
        "/api/v1/transactions",
        new CreateTransactionRequest(
            accountId,
            categoryId,
            amount,
            "TRY",
            type,
            "business",
            "2026-08-09",
            "Synthetic transaction"));
}
