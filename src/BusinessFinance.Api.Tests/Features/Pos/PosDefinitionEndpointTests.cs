using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Pos;

namespace BusinessFinance.Api.Tests.Features.Pos;

/// <summary>
/// Aşama 06.3 Grup 4: POS tanımı (ADR 0019 T4). Tanım bir kez girilir; POS
/// tahsilatı ondan dolar ve akşamki giriş "tutar + kaydet"e iner.
/// </summary>
public sealed class PosDefinitionEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Grubun çıkış ölçütü: tanımla yazılan tahsilat yalnız tutar ve gün
    /// gönderir; hesap, kategori, komisyon ve beklenen gün tanımdan gelir.
    /// Kayıt tutarı saklar, oranı saklamaz.
    /// </summary>
    [Fact]
    public async Task Settlement_WrittenWithADefinition_NeedsOnlyTheAmountAndTheDay()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-owner@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");

        var definition = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest(
                "Ziraat POS", account.Id, income.Id, "0.0179", 1, true, expense.Id));
        Assert.Equal("0.0179", definition.CommissionRate);
        Assert.Equal(account.Name, definition.AccountName);
        Assert.True(definition.IsActive);

        // 2026-09-25 cuma: bir iş günü sonrası pazartesi.
        var friday = new DateOnly(2026, 9, 25);
        var preview = await owner.GetFromJsonAsync<PosSettlementPreviewResponse>(
            $"/api/v1/pos-definitions/{definition.Id}/preview" +
            $"?grossAmount=1000.0000&settlementDate={Date(friday)}");
        Assert.Equal("17.9000", preview!.CommissionAmount);
        Assert.Equal("982.1000", preview.NetAmount);
        Assert.Equal("2026-09-28", preview.ExpectedTransferDate);

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "1000.0000", "TRY", Date(friday),
                Scope: "business", PosDefinitionId: definition.Id));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var settlement = (await create.Content.ReadFromJsonAsync<PosSettlementResponse>())!;

        // Önizleme ile yazılan aynı sayıları söyler: tek hesaplama yolu.
        Assert.Equal(preview.CommissionAmount, settlement.CommissionAmount);
        Assert.Equal(preview.NetAmount, settlement.NetAmount);
        Assert.Equal(preview.ExpectedTransferDate, settlement.ExpectedTransferDate);
        Assert.Equal(account.Id, settlement.AccountId);
        Assert.Equal(income.Id, settlement.CategoryId);
        Assert.Equal(expense.Id, settlement.CommissionCategoryId);
        Assert.Equal(definition.Id, settlement.PosDefinitionId);
        Assert.Equal("Ziraat POS", settlement.PosDefinitionName);

        var list = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            $"/api/v1/pos-settlements?from={Date(friday)}&to={Date(friday)}");
        Assert.Equal("Ziraat POS", Assert.Single(list!.Items).PosDefinitionName);
    }

    /// <summary>
    /// Açıkça gönderilen alan tanımı ezer: o akşam banka farklı kestiyse
    /// kullanıcı tutarı yazar; komisyonsuz girilen tahsilat kategori taşımaz.
    /// </summary>
    [Fact]
    public async Task ExplicitFields_OverrideTheDefinition()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-override@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");
        var definition = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest(
                "Ziraat POS", account.Id, income.Id, "0.0179", 1, false, expense.Id));
        var day = new DateOnly(2026, 9, 21);

        using var withAmount = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "1000.0000", "TRY", Date(day), Date(day.AddDays(4)),
                CommissionAmount: "25.0000", Scope: "business",
                PosDefinitionId: definition.Id));
        var settled = (await withAmount.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        Assert.Equal("25.0000", settled.CommissionAmount);
        Assert.Equal(Date(day.AddDays(4)), settled.ExpectedTransferDate);

        using var withoutCommission = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "400.0000", "TRY", Date(day),
                CommissionRate: "0.0000", Scope: "business",
                PosDefinitionId: definition.Id));
        Assert.Equal(HttpStatusCode.Created, withoutCommission.StatusCode);
        var free = (await withoutCommission.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        Assert.Equal("0.0000", free.CommissionAmount);
        Assert.Null(free.CommissionCategoryId);
        Assert.Equal(Date(day.AddDays(1)), free.ExpectedTransferDate);
    }

    /// <summary>
    /// Tanımsız tahsilat eskisi gibi çalışır ama kendi alanlarını kendisi
    /// taşımak zorundadır; sunucu hesap ya da kategori uydurmaz.
    /// </summary>
    [Fact]
    public async Task Settlement_WithoutADefinition_MustCarryItsOwnDetails()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-none@example.test");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "100.0000", "TRY", "2026-09-21", Scope: "business"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "pos_settlements.details_required",
            await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Silme yerine pasifleştirme: tahsilatı olan tanım silinemez (409), pasife
    /// alınır ve pasif tanımla yeni tahsilat yazılamaz. Hiç kullanılmamış tanım
    /// silinebilir.
    /// </summary>
    [Fact]
    public async Task Definition_WithSettlements_IsDeactivatedNotDeleted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-delete@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");
        var used = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest("Kullanılan", account.Id, income.Id, "0.0000", 0, false));
        var unused = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest("Kullanılmayan", account.Id, income.Id, "0.0000", 0, false));

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "100.0000", "TRY", "2026-09-21",
                Scope: "business", PosDefinitionId: used.Id));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        using var deleteUsed = await owner.DeleteAsync($"/api/v1/pos-definitions/{used.Id}");
        Assert.Equal(HttpStatusCode.Conflict, deleteUsed.StatusCode);
        Assert.Contains(
            "pos_definitions.has_settlements",
            await deleteUsed.Content.ReadAsStringAsync());

        using var deleteUnused = await owner.DeleteAsync($"/api/v1/pos-definitions/{unused.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteUnused.StatusCode);

        using var deactivate = await owner.PatchAsJsonAsync(
            $"/api/v1/pos-definitions/{used.Id}/active",
            new SetPosDefinitionActiveRequest(false));
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        Assert.False((await deactivate.Content.ReadFromJsonAsync<PosDefinitionResponse>())!.IsActive);

        using var afterDeactivation = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "100.0000", "TRY", "2026-09-22",
                Scope: "business", PosDefinitionId: used.Id));
        Assert.Equal(HttpStatusCode.Conflict, afterDeactivation.StatusCode);
        Assert.Contains(
            "pos_definitions.inactive",
            await afterDeactivation.Content.ReadAsStringAsync());

        var definitions = await owner.GetFromJsonAsync<PosDefinitionListResponse>(
            "/api/v1/pos-definitions");
        var remaining = Assert.Single(definitions!.Items);
        Assert.Equal(used.Id, remaining.Id);
        Assert.False(remaining.IsActive);
    }

    /// <summary>
    /// Düzenleme tanımı değiştirir; yazılmış tahsilat yazıldığı tutarla kalır.
    /// </summary>
    [Fact]
    public async Task Update_ChangesTheDefinitionAndLeavesWrittenSettlementsAlone()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-update@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");
        var definition = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest(
                "Ziraat POS", account.Id, income.Id, "0.0100", 1, false, expense.Id));

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "1000.0000", "TRY", "2026-09-21",
                Scope: "business", PosDefinitionId: definition.Id));
        var written = (await create.Content.ReadFromJsonAsync<PosSettlementResponse>())!;

        using var update = await owner.PutAsJsonAsync(
            $"/api/v1/pos-definitions/{definition.Id}",
            new SavePosDefinitionRequest(
                "Ziraat POS (yeni oran)", account.Id, income.Id, "0.0250", 2, true, expense.Id));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var updated = (await update.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        Assert.Equal("0.0250", updated.CommissionRate);
        Assert.Equal(2, updated.TransferDays);
        Assert.True(updated.BusinessDaysOnly);

        var list = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?from=2026-09-21&to=2026-09-21");
        var unchanged = Assert.Single(list!.Items);
        Assert.Equal(written.CommissionAmount, unchanged.CommissionAmount);
        Assert.Equal("10.0000", unchanged.CommissionAmount);
        Assert.Equal("Ziraat POS (yeni oran)", unchanged.PosDefinitionName);
    }

    /// <summary>
    /// Ana POS: ilk eklenen kendiliğinden seçilir; kullanıcı başkasını seçince
    /// öncekinin işareti kalkar (kullanıcı başına en çok bir tane) ve liste
    /// onunla başlar. Pasife alınan POS ana POS olmaktan çıkar; pasif POS
    /// seçilemez.
    /// </summary>
    [Fact]
    public async Task DefaultPos_IsOnePerUserAndLeadsTheList()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-default@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-default-b@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");

        var first = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest("Ziraat POS", account.Id, income.Id, "0.0000", 1, true));
        var second = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest("Akbank POS", account.Id, income.Id, "0.0000", 1, true));
        Assert.True(first.IsDefault);
        Assert.False(second.IsDefault);

        using var choose = await owner.PutAsync(
            $"/api/v1/pos-definitions/{second.Id}/default", null);
        Assert.Equal(HttpStatusCode.OK, choose.StatusCode);
        Assert.True((await choose.Content.ReadFromJsonAsync<PosDefinitionResponse>())!.IsDefault);

        var list = await owner.GetFromJsonAsync<PosDefinitionListResponse>(
            "/api/v1/pos-definitions");
        Assert.Equal(second.Id, list!.Items[0].Id);
        Assert.Single(list.Items, item => item.IsDefault);

        // Yabancı kullanıcı başkasının POS'unu ana POS yapamaz.
        using var foreign = await stranger.PutAsync(
            $"/api/v1/pos-definitions/{first.Id}/default", null);
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);

        using var deactivate = await owner.PatchAsJsonAsync(
            $"/api/v1/pos-definitions/{second.Id}/active",
            new SetPosDefinitionActiveRequest(false));
        Assert.False((await deactivate.Content.ReadFromJsonAsync<PosDefinitionResponse>())!.IsDefault);

        using var inactive = await owner.PutAsync(
            $"/api/v1/pos-definitions/{second.Id}/default", null);
        Assert.Equal(HttpStatusCode.Conflict, inactive.StatusCode);

        var after = await owner.GetFromJsonAsync<PosDefinitionListResponse>(
            "/api/v1/pos-definitions");
        Assert.DoesNotContain(after!.Items, item => item.IsDefault);
    }

    /// <summary>
    /// Oran varsa komisyon kategorisi zorunludur; kart parası kasaya geçmez.
    /// Sunucu eksik kategoriyi kendisi kurmaz.
    /// </summary>
    [Fact]
    public async Task Definition_RejectsWhatASettlementCouldNotBeWrittenWith()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-invalid@example.test");
        var bank = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var till = await CreateAccountAsync(owner, "Sentetik kasa", "cash");
        var income = await FirstCategoryAsync(owner, "income");

        using var noCategory = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Oranlı", bank.Id, income.Id, "0.0179", 1, true));
        Assert.Equal(HttpStatusCode.Conflict, noCategory.StatusCode);

        using var intoTill = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Kasaya", till.Id, income.Id, "0.0000", 1, true));
        Assert.Equal(HttpStatusCode.NotFound, intoTill.StatusCode);

        using var badRate = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Bozuk", bank.Id, income.Id, "yüzde iki", 1, true));
        Assert.Equal(HttpStatusCode.BadRequest, badRate.StatusCode);
    }

    /// <summary>
    /// Sahiplik: yabancı kullanıcı tanımı ne görür, ne değiştirir, ne siler, ne
    /// onunla tahsilat yazar. Var olmayan kayıtla aynı cevabı alır.
    /// </summary>
    [Fact]
    public async Task Definitions_AreInvisibleAndUnusableForAnotherUser()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-a@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "pos-definition-b@example.test");
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank");
        var income = await FirstCategoryAsync(owner, "income");
        var definition = await CreateDefinitionAsync(
            owner, new SavePosDefinitionRequest("Ziraat POS", account.Id, income.Id, "0.0000", 1, true));

        var strangerAccount = await CreateAccountAsync(stranger, "Yabancı banka", "bank");
        var strangerIncome = await FirstCategoryAsync(stranger, "income");

        var strangerList = await stranger.GetFromJsonAsync<PosDefinitionListResponse>(
            "/api/v1/pos-definitions");
        Assert.Empty(strangerList!.Items);

        using var foreignUpdate = await stranger.PutAsJsonAsync(
            $"/api/v1/pos-definitions/{definition.Id}",
            new SavePosDefinitionRequest(
                "Ele geçirilen", strangerAccount.Id, strangerIncome.Id, "0.0000", 1, true));
        Assert.Equal(HttpStatusCode.NotFound, foreignUpdate.StatusCode);

        using var foreignDelete = await stranger.DeleteAsync(
            $"/api/v1/pos-definitions/{definition.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignDelete.StatusCode);

        using var foreignPreview = await stranger.GetAsync(
            $"/api/v1/pos-definitions/{definition.Id}/preview" +
            "?grossAmount=100.0000&settlementDate=2026-09-21");
        Assert.Equal(HttpStatusCode.NotFound, foreignPreview.StatusCode);

        using var foreignSettlement = await stranger.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                strangerAccount.Id, strangerIncome.Id, "100.0000", "TRY", "2026-09-21",
                "2026-09-22", Scope: "business", PosDefinitionId: definition.Id));
        Assert.Equal(HttpStatusCode.NotFound, foreignSettlement.StatusCode);

        // Kendi tanımını yabancı hesaba bağlayamaz.
        using var foreignAccount = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest("Yabancı hesap", strangerAccount.Id, income.Id, "0.0000", 1, true));
        Assert.Equal(HttpStatusCode.NotFound, foreignAccount.StatusCode);

        var ownerList = await owner.GetFromJsonAsync<PosDefinitionListResponse>(
            "/api/v1/pos-definitions");
        Assert.Equal("Ziraat POS", Assert.Single(ownerList!.Items).Name);
    }

    private static string Date(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static async Task<PosDefinitionResponse> CreateDefinitionAsync(
        HttpClient client,
        SavePosDefinitionRequest request)
    {
        using var response = await client.PostAsJsonAsync("/api/v1/pos-definitions", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", "1000.0000"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
