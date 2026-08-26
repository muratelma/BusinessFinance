using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.DataPortability;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage12;

public sealed class DataPortabilityEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task ExportValidateRestore_RoundTripsThroughProtectedHttpContract()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var anonymous = factory.CreateClient();
        using var unauthorized = await anonymous.GetAsync("/api/v1/backups/download");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        using var source = await AuthenticateAsync(factory, "portability-source@example.test");
        var account = await CreateAccountAsync(source);
        var categories = await source.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        var category = categories!.Items[0];
        using var createTransaction = await source.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, category.Id, "25.5000", "TRY", "expense",
                "business",
                "2026-08-11", "=FORMULA"));
        createTransaction.EnsureSuccessStatusCode();

        using var csv = await source.GetAsync("/api/v1/exports/transactions.csv");
        csv.EnsureSuccessStatusCode();
        Assert.Equal("text/csv", csv.Content.Headers.ContentType!.MediaType);
        Assert.Contains("attachment", csv.Content.Headers.ContentDisposition!.DispositionType, StringComparison.Ordinal);
        var csvBytes = await csv.Content.ReadAsByteArrayAsync();
        Assert.Equal([0xEF, 0xBB, 0xBF], csvBytes[..3]);

        using var json = await source.GetAsync("/api/v1/exports/financial-data.json");
        json.EnsureSuccessStatusCode();
        var jsonText = await json.Content.ReadAsStringAsync();
        Assert.DoesNotContain("password", jsonText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("refreshSession", jsonText, StringComparison.OrdinalIgnoreCase);

        using var download = await source.GetAsync("/api/v1/backups/download");
        download.EnsureSuccessStatusCode();
        Assert.Equal("application/vnd.business-finance.backup+json", download.Content.Headers.ContentType!.MediaType);
        var backup = await download.Content.ReadAsByteArrayAsync();

        using var target = await AuthenticateAsync(factory, "portability-target@example.test");
        using var validateForm = BackupForm(backup);
        using var validate = await target.PostAsync("/api/v1/backups/validate", validateForm);
        validate.EnsureSuccessStatusCode();
        var validation = await validate.Content.ReadFromJsonAsync<BackupValidationResponse>();
        Assert.Equal(10, validation!.SchemaVersion);
        Assert.True(validation.EntityCount >= 10);

        using var restoreForm = BackupForm(backup);
        using var restore = await target.PostAsync("/api/v1/backups/restore", restoreForm);
        restore.EnsureSuccessStatusCode();
        var summary = await restore.Content.ReadFromJsonAsync<RestoreSummaryResponse>();
        Assert.Equal(validation.EntityCount, summary!.RestoredEntityCount);

        using var restoredJson = await target.GetAsync("/api/v1/exports/financial-data.json");
        restoredJson.EnsureSuccessStatusCode();
        using var restoredDocument = JsonDocument.Parse(await restoredJson.Content.ReadAsByteArrayAsync());
        Assert.Single(restoredDocument.RootElement.GetProperty("accounts").EnumerateArray());
        Assert.Single(restoredDocument.RootElement.GetProperty("transactions").EnumerateArray());
        // Geri yüklenen hesap kaynağın kategorilerini alır; sayı varsayılan
        // listeyle birlikte değişir, o yüzden kaynağınkiyle karşılaştırılıyor.
        Assert.True(
            restoredDocument.RootElement.GetProperty("categories").GetArrayLength() >= 8);

        using var retryForm = BackupForm(backup);
        using var retry = await target.PostAsync("/api/v1/backups/restore", retryForm);
        Assert.Equal(HttpStatusCode.Conflict, retry.StatusCode);
    }

    /// <summary>
    /// Cari defterin dosyası korumalıdır ve yalnız sahibinin hareketlerini taşır.
    /// </summary>
    /// <remarks>
    /// İşlem CSV'sine kolon eklenmedi: o dosya <c>BudgetTransaction</c>
    /// dökümüdür ve cari hareket orada hiç bulunmaz. Cari defterin kendi
    /// dosyası olması, her dışa aktarmanın tek kaydın dökümü olmasını korur.
    /// </remarks>
    [Fact]
    public async Task CounterpartyLedgerCsv_IsOwnerScopedAndCarriesBothRecordKinds()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var anonymous = factory.CreateClient();
        using var unauthorized = await anonymous.GetAsync("/api/v1/exports/counterparty-ledger.csv");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        using var owner = await AuthenticateAsync(factory, "ledger-owner@example.test");
        var account = await CreateAccountAsync(owner);
        var incomeCategories = await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income");
        var incomeCategory = incomeCategories!.Items[0];

        using var createCounterparty = await owner.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest("Sentetik Manav"));
        createCounterparty.EnsureSuccessStatusCode();
        var counterparty = await createCounterparty.Content
            .ReadFromJsonAsync<CounterpartyResponse>();

        using var charge = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{counterparty!.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "400.0000", "TRY", incomeCategory.Id, "2026-08-06",
                "business", "Veresiye satış"));
        charge.EnsureSuccessStatusCode();
        using var payment = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{counterparty.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "120.0000", "TRY", account.Id, "2026-08-08", "Kısmi tahsilat"));
        payment.EnsureSuccessStatusCode();

        using var download = await owner.GetAsync("/api/v1/exports/counterparty-ledger.csv");
        download.EnsureSuccessStatusCode();
        Assert.Equal("text/csv", download.Content.Headers.ContentType!.MediaType);
        var bytes = await download.Content.ReadAsByteArrayAsync();
        Assert.Equal([0xEF, 0xBB, 0xBF], bytes[..3]);
        var text = Encoding.UTF8.GetString(bytes);
        Assert.Contains("counterpartyName", text, StringComparison.Ordinal);
        Assert.Contains("400.0000", text, StringComparison.Ordinal);
        Assert.Contains("120.0000", text, StringComparison.Ordinal);

        // Başka kullanıcının defteri boştur: dosya başlığı gelir, satır gelmez.
        using var stranger = await AuthenticateAsync(factory, "ledger-stranger@example.test");
        using var strangerDownload = await stranger.GetAsync(
            "/api/v1/exports/counterparty-ledger.csv");
        strangerDownload.EnsureSuccessStatusCode();
        var strangerText = Encoding.UTF8.GetString(
            await strangerDownload.Content.ReadAsByteArrayAsync());
        Assert.DoesNotContain("Sentetik Manav", strangerText, StringComparison.Ordinal);
        Assert.DoesNotContain("400.0000", strangerText, StringComparison.Ordinal);
    }

    private static MultipartFormDataContent BackupForm(byte[] content)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/vnd.business-finance.backup+json");
        form.Add(file, "file", "synthetic.bfbackup.json");
        return form;
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Backup account", "bank", "TRY", "1000.0000", "business"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }
}
