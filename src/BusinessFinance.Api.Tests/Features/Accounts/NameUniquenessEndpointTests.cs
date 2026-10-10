using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Pos;

namespace BusinessFinance.Api.Tests.Features.Accounts;

/// <summary>
/// Ad tekliğinin tek kuralı, uçtan uca (kullanıcı kararı, 9 Ekim 2026).
/// </summary>
/// <remarks>
/// İki ad yalnız harfleri ve rakamları aynıysa aynı addır: harf büyüklüğü,
/// boşluk ve noktalama fark sayılmaz; Türkçe harf başka harftir. Hesap, kredi
/// kartı, kategori, kişi ve POS aynı kuralı kullanır. Kullanıcı ikinci bir
/// "İş Bankası" hesabı açmak isterse ayırt edici bir ek yazar.
/// </remarks>
public sealed class NameUniquenessEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Account_RefusesTheSameNameInAnotherSpelling_AndAcceptsARealDifference()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "name-rule-account@example.test");
        using var neighbour = await CreateAuthenticatedClientAsync(
            factory, "name-rule-account-neighbour@example.test");

        var bank = await CreateAccountAsync(owner, "İş Bankası");

        foreach (var sameName in new[]
                 {
                     "İŞ BANKASI", "iş  bankası", "işbankası", "İş-Bankası", "İş Bankası.",
                     " İş Bankası ", "İş.Bankası",
                 })
        {
            using var refused = await owner.PostAsJsonAsync(
                "/api/v1/accounts", new CreateAccountRequest(sameName, "bank", "TRY", "0"));
            Assert.True(
                refused.StatusCode == HttpStatusCode.Conflict, $"{sameName}: {refused.StatusCode}");
            Assert.Equal("accounts.duplicate_name", await CodeAsync(refused));
        }

        // Türkçe harf başka harftir; ayırt edici ek başka addır.
        foreach (var different in new[]
                 {
                     "Is Bankasi", "İş Bankası 2", "İş Bankası Şahsi", "İş Bankası 4512",
                 })
        {
            using var accepted = await owner.PostAsJsonAsync(
                "/api/v1/accounts", new CreateAccountRequest(different, "bank", "TRY", "0"));
            Assert.True(
                accepted.StatusCode == HttpStatusCode.Created, $"{different}: {accepted.StatusCode}");
        }

        // Başka kullanıcı aynı adı kullanır.
        await CreateAccountAsync(neighbour, "İŞ BANKASI");

        // Yalnız yazımı düzeltmek serbesttir; başka bir hesabın adına geçmek değil.
        using var respelled = await owner.PutAsJsonAsync(
            $"/api/v1/accounts/{bank.Id}", new UpdateAccountRequest("İŞ BANKASI", true));
        Assert.Equal(HttpStatusCode.OK, respelled.StatusCode);
        var cash = await CreateAccountAsync(owner, "Kasa");
        using var taken = await owner.PutAsJsonAsync(
            $"/api/v1/accounts/{cash.Id}", new UpdateAccountRequest("iş-bankası", true));
        Assert.Equal(HttpStatusCode.Conflict, taken.StatusCode);
        Assert.Equal("accounts.duplicate_name", await CodeAsync(taken));

        // Pasif hesap adını tutar.
        using var deactivated = await owner.PutAsJsonAsync(
            $"/api/v1/accounts/{cash.Id}", new UpdateAccountRequest("Kasa", false));
        Assert.Equal(HttpStatusCode.OK, deactivated.StatusCode);
        using var reopened = await owner.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest("KASA", "cash", "TRY", "0"));
        Assert.Equal(HttpStatusCode.Conflict, reopened.StatusCode);
    }

    [Fact]
    public async Task CategoryCardPersonAndPos_FollowTheSameRule()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "name-rule-others@example.test");
        var bank = await CreateAccountAsync(owner, "Garanti");

        // Kategori: teklik türle birlikte.
        using var expense = await owner.PostAsJsonAsync(
            "/api/v1/categories", new CreateCategoryRequest("Özel Kalem", "expense", "business"));
        Assert.Equal(HttpStatusCode.Created, expense.StatusCode);
        var expenseCategory = (await expense.Content.ReadFromJsonAsync<CategoryResponse>())!;
        using var sameExpense = await owner.PostAsJsonAsync(
            "/api/v1/categories", new CreateCategoryRequest("özel-kalem", "expense", "business"));
        Assert.Equal(HttpStatusCode.Conflict, sameExpense.StatusCode);
        Assert.Equal("categories.duplicate_name", await CodeAsync(sameExpense));
        using var income = await owner.PostAsJsonAsync(
            "/api/v1/categories", new CreateCategoryRequest("Özel Kalem", "income", "business"));
        Assert.Equal(HttpStatusCode.Created, income.StatusCode);
        var incomeCategory = (await income.Content.ReadFromJsonAsync<CategoryResponse>())!;
        using var respelledCategory = await owner.PutAsJsonAsync(
            $"/api/v1/categories/{expenseCategory.Id}",
            new UpdateCategoryRequest("ÖZEL KALEM", true, "business"));
        Assert.Equal(HttpStatusCode.OK, respelledCategory.StatusCode);

        // Kredi kartı: hesapla aynı adı taşıyabilir (ayrı listeler); kendi
        // listesinde aynı ad ikinci kez açılmaz.
        using var card = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest("Garanti", "5000.0000", "TRY", 10, 20));
        Assert.Equal(HttpStatusCode.Created, card.StatusCode);
        var createdCard = (await card.Content.ReadFromJsonAsync<CreditCardResponse>())!;
        using var sameCard = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest("GARANTİ.", "5000.0000", "TRY", 10, 20));
        Assert.Equal(HttpStatusCode.Conflict, sameCard.StatusCode);
        Assert.Equal("credit_cards.duplicate_name", await CodeAsync(sameCard));
        using var otherCard = await owner.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest("Garanti Bonus", "5000.0000", "TRY", 10, 20));
        Assert.Equal(HttpStatusCode.Created, otherCard.StatusCode);
        // Adı değişmeden limiti düzenlemek serbest; yazım düzeltmesi de.
        using var editedCard = await owner.PutAsJsonAsync(
            $"/api/v1/credit-cards/{createdCard.Id}",
            new UpdateCreditCardRequest("garanti", "7000.0000", "TRY", 10, 20, true));
        Assert.Equal(HttpStatusCode.OK, editedCard.StatusCode);
        using var takenCard = await owner.PutAsJsonAsync(
            $"/api/v1/credit-cards/{createdCard.Id}",
            new UpdateCreditCardRequest("garanti-bonus", "7000.0000", "TRY", 10, 20, true));
        Assert.Equal(HttpStatusCode.Conflict, takenCard.StatusCode);

        // Kişi: boşluk artık hiç sayılmaz.
        using var person = await owner.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest("Ali Can"));
        Assert.Equal(HttpStatusCode.Created, person.StatusCode);
        foreach (var sameName in new[] { "Alican", "ALİ CAN", "Ali-Can" })
        {
            using var refused = await owner.PostAsJsonAsync(
                "/api/v1/counterparties", new CreateCounterpartyRequest(sameName));
            Assert.True(
                refused.StatusCode == HttpStatusCode.Conflict, $"{sameName}: {refused.StatusCode}");
            Assert.Equal("counterparties.duplicate_name", await CodeAsync(refused));
        }

        using var otherPerson = await owner.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest("Ali Can Yılmaz"));
        Assert.Equal(HttpStatusCode.Created, otherPerson.StatusCode);

        // POS: bugüne kadar adında hiç teklik yoktu.
        var definition = new SavePosDefinitionRequest(
            "Garanti POS", bank.Id, incomeCategory.Id, "0.0000", 1, false);
        using var pos = await owner.PostAsJsonAsync("/api/v1/pos-definitions", definition);
        Assert.True(pos.StatusCode == HttpStatusCode.Created, await pos.Content.ReadAsStringAsync());
        var createdPos = (await pos.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        foreach (var sameName in new[] { "garanti pos", "GarantiPOS", "Garanti-POS" })
        {
            using var refused = await owner.PostAsJsonAsync(
                "/api/v1/pos-definitions", definition with { Name = sameName });
            Assert.True(
                refused.StatusCode == HttpStatusCode.Conflict, $"{sameName}: {refused.StatusCode}");
            Assert.Equal("pos_definitions.duplicate_name", await CodeAsync(refused));
        }

        using var otherPos = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions", definition with { Name = "Garanti POS 2" });
        Assert.Equal(HttpStatusCode.Created, otherPos.StatusCode);
        var secondPos = (await otherPos.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        using var respelledPos = await owner.PutAsJsonAsync(
            $"/api/v1/pos-definitions/{createdPos.Id}", definition with { Name = "GARANTİ POS" });
        Assert.Equal(HttpStatusCode.OK, respelledPos.StatusCode);
        using var takenPos = await owner.PutAsJsonAsync(
            $"/api/v1/pos-definitions/{secondPos.Id}", definition with { Name = "garantipos" });
        Assert.Equal(HttpStatusCode.Conflict, takenPos.StatusCode);
        Assert.Equal("pos_definitions.duplicate_name", await CodeAsync(takenPos));
    }

    private static async Task<string?> CodeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonObject>();
        return body?["code"]?.GetValue<string>();
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "bank", "TRY", "0", "business"));
        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
