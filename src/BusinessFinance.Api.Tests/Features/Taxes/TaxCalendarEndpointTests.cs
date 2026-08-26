using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Taxes;
using BusinessFinance.Api.Features.UpcomingPayments;

namespace BusinessFinance.Api.Tests.Features.Taxes;

/// <summary>
/// Aşama 05 Grup 4: vergi/SGK takvimi hazır kalemlerden kuruluyor, mevcut
/// tekrarlayan plan altyapısının üstünde yaşıyor ve yaklaşanlar listesine
/// düşüyor. Tarih ve tutar kullanıcınındır (ADR 0016).
/// </summary>
public sealed class TaxCalendarEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Öneriler <b>tutar taşımaz</b>: bir sayı önermek, hesaplanmış bir vergi
    /// tutarı iddia etmek olurdu.
    /// </summary>
    [Fact]
    public async Task Suggestions_AreOfferedWithoutAnyAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        using var response = await client.GetAsync(
            "/api/v1/tax-calendar/suggestions",
            CancellationToken.None);
        var payload = await response.Content.ReadAsStringAsync(CancellationToken.None);
        using var document = JsonDocument.Parse(payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = document.RootElement.GetProperty("items").EnumerateArray().ToArray();
        Assert.Equal(4, items.Length);
        Assert.DoesNotContain("amount", payload, StringComparison.OrdinalIgnoreCase);

        var advanceTax = items.Single(item => item.GetProperty("key").GetString() == "advance-tax");
        Assert.Equal("quarterly", advanceTax.GetProperty("frequency").GetString());
        Assert.Equal("business", advanceTax.GetProperty("scope").GetString());
        Assert.Equal(
            "SGK ve vergi ödemesi",
            advanceTax.GetProperty("suggestedCategoryName").GetString());
    }

    [Fact]
    public async Task Suggestions_RequireAuthentication()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/api/v1/tax-calendar/suggestions",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Grubun çıkış ölçütü: önerinin doldurduğu formla kurulan kalem yaklaşanlar
    /// listesine düşüyor ve kullanıcı onu silebiliyor. Kurulum ikinci bir yazma
    /// yolu açmıyor — mevcut tekrarlayan plan ucu kullanılıyor.
    /// </summary>
    [Fact]
    public async Task ACalendarItem_LandsInTheUpcomingListAndCanBeDeleted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetExpenseCategoryAsync(client);

        using var create = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "1000.0000",
                "TRY",
                "bill-payment",
                "business",
                "quarterly",
                "2026-08-17",
                null,
                "clamp-to-last-day",
                "Geçici vergi"),
            CancellationToken.None);
        create.EnsureSuccessStatusCode();
        var plan = await create.Content.ReadFromJsonAsync<RecurringTransactionResponse>(
            CancellationToken.None);

        using var generate = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-17"),
            CancellationToken.None);
        generate.EnsureSuccessStatusCode();

        var upcoming = await client.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-17&daysAhead=30",
            CancellationToken.None);

        Assert.Equal("quarterly", plan!.Frequency);
        var payment = Assert.Single(upcoming!.Items);
        Assert.Equal("recurring-occurrence", payment.SourceType);
        Assert.Equal("2026-08-17", payment.DueDate);

        using var delete = await client.DeleteAsync(
            $"/api/v1/recurring-transactions/{plan.Id}",
            CancellationToken.None);
        var afterDelete = await client.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-17&daysAhead=30",
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Empty(afterDelete!.Items);
    }

    /// <summary>
    /// Plandaki tutar bir beklentidir: gerçekleşme sırasında gönderilen tutar
    /// kayda geçer ve <b>planın tutarı değişmez</b>.
    /// </summary>
    [Fact]
    public async Task Realizing_WritesTheAmountTheUserActuallyOwes_WithoutChangingThePlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetExpenseCategoryAsync(client);

        using var create = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "1000.0000",
                "TRY",
                "bill-payment",
                "business",
                "monthly",
                "2026-08-01",
                null,
                "clamp-to-last-day",
                "KDV beyanı"),
            CancellationToken.None);
        create.EnsureSuccessStatusCode();
        var plan = await create.Content.ReadFromJsonAsync<RecurringTransactionResponse>(
            CancellationToken.None);

        using var realize = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan!.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-01", "2450.7500"),
            CancellationToken.None);
        realize.EnsureSuccessStatusCode();
        using var realized = JsonDocument.Parse(
            await realize.Content.ReadAsStringAsync(CancellationToken.None));

        var plans = await client.GetFromJsonAsync<RecurringTransactionListResponse>(
            "/api/v1/recurring-transactions",
            CancellationToken.None);

        Assert.Equal(
            "2450.7500",
            realized.RootElement.GetProperty("transaction").GetProperty("amount").GetString());
        Assert.Equal("1000.0000", Assert.Single(plans!.Items).Amount);
    }

    /// <summary>
    /// Gönderilen tutar okunamıyorsa sunucu bir değer uydurmaz.
    /// </summary>
    [Fact]
    public async Task Realizing_WithAnUnreadableAmount_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetExpenseCategoryAsync(client);

        using var create = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id,
                category.Id,
                "1000.0000",
                "TRY",
                "bill-payment",
                "business",
                "monthly",
                "2026-08-01",
                null,
                "clamp-to-last-day",
                "KDV beyanı"),
            CancellationToken.None);
        create.EnsureSuccessStatusCode();
        var plan = await create.Content.ReadFromJsonAsync<RecurringTransactionResponse>(
            CancellationToken.None);

        using var realize = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan!.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-01", "iki bin"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, realize.StatusCode);
        using var document = JsonDocument.Parse(
            await realize.Content.ReadAsStringAsync(CancellationToken.None));
        Assert.Equal(
            "recurring.invalid_amount",
            document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Banka", "bank", "TRY", "10000"),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>(CancellationToken.None))!;
    }

    private static async Task<CategoryResponse> GetExpenseCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);
        return categories!.Items[0];
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory)
    {
        var client = factory.CreateClient();
        var email = $"tax-calendar-{Guid.NewGuid():N}@example.test";
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password),
            CancellationToken.None);
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password),
            CancellationToken.None);
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokens!.AccessToken);

        return client;
    }
}
