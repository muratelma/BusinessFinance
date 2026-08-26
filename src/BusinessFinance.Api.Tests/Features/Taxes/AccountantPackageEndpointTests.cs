using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Taxes;

/// <summary>
/// Aşama 05 Grup 5: ay sonu muhasebeci paketi. Toplamlar aynı ayın işletme
/// raporuyla birebir tutar ve pakete şahsi hiçbir kayıt girmez (ADR 0016).
/// </summary>
public sealed class AccountantPackageEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Month = "2026-08";
    private static readonly byte[] MinimalPng = [137, 80, 78, 71, 13, 10, 26, 10];

    /// <summary>
    /// Aşamanın en kritik testi: paket yalnız işletme kapsamını içerir,
    /// toplamları raporunkiyle birebir eşittir ve satırların toplamı da aynı
    /// sayıyı verir — iki ayrı hesaplama yolu yok.
    /// </summary>
    [Fact]
    public async Task Package_MatchesTheBusinessReport_AndLeaksNoPersonalRecord()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var context = await SeedMonthAsync(client);

        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            "/api/v1/reports/monthly?year=2026&month=8&scope=business",
            CancellationToken.None);
        using var response = await client.GetAsync(
            "/api/v1/accountant-package?year=2026&month=8",
            CancellationToken.None);
        var payload = await response.Content.ReadAsStringAsync(CancellationToken.None);
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("business", root.GetProperty("scope").GetString());
        Assert.Equal(report!.TotalIncome, root.GetProperty("totalIncome").GetString());
        Assert.Equal(report.TotalExpense, root.GetProperty("totalExpense").GetString());
        Assert.Equal(report.Net, root.GetProperty("net").GetString());

        var lines = root.GetProperty("lines").EnumerateArray().ToArray();
        Assert.Equal(
            Decimal(report.TotalIncome),
            SumOf(lines, "income"));
        Assert.Equal(
            Decimal(report.TotalExpense),
            SumOf(lines, "expense"));

        // Şahsi kayıt ne satırda ne toplamda: paket muhasebeciye gidiyor.
        Assert.DoesNotContain(
            context.PersonalTransactionId.ToString("D"),
            payload,
            StringComparison.OrdinalIgnoreCase);
        Assert.All(
            lines,
            line => Assert.NotEqual(
                context.PersonalTransactionId.ToString("D"),
                line.GetProperty("sourceId").GetString()));
    }

    /// <summary>
    /// KDV özeti taşınan alanların toplamıdır; hiçbir tutar hesaplanmaz ve KDV
    /// yazılmamış satırlar ayrıca sayılır.
    /// </summary>
    [Fact]
    public async Task Package_SummarisesTheCarriedVatAndTheNonDeductibleExpenses()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        await SeedMonthAsync(client);

        using var response = await client.GetAsync(
            "/api/v1/accountant-package?year=2026&month=8",
            CancellationToken.None);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));
        var root = document.RootElement;

        // Yazılan KDV: gelirde POS satışının 180, giderde ticari malın 20'si.
        Assert.Equal("180.0000", root.GetProperty("vatOnIncome").GetString());
        Assert.Equal("20.0000", root.GetProperty("vatOnExpense").GetString());
        Assert.True(root.GetProperty("linesWithoutVat").GetInt32() > 0);

        // İndirilemeyen tek kalem: trafik cezası.
        Assert.Equal("500.0000", root.GetProperty("nonDeductibleExpense").GetString());
        Assert.Equal(1, root.GetProperty("nonDeductibleCount").GetInt32());
        Assert.True(root.GetProperty("deductibilityUnansweredCount").GetInt32() > 0);
    }

    /// <summary>
    /// Paket tek dosyadır ve kullanıcının kendi cihazından paylaşılır; sunucu
    /// kimseye bir şey göndermez.
    /// </summary>
    [Fact]
    public async Task Package_DownloadsAsOneFile_WithoutAnyPersonalLine()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var context = await SeedMonthAsync(client);

        using var response = await client.GetAsync(
            "/api/v1/exports/accountant-package.zip",
            CancellationToken.None);
        var bytes = await response.Content.ReadAsByteArrayAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var download = await client.GetAsync(
            "/api/v1/exports/accountant-package.zip?year=2026&month=8",
            CancellationToken.None);
        bytes = await download.Content.ReadAsByteArrayAsync(CancellationToken.None);
        using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal("application/zip", download.Content.Headers.ContentType!.MediaType);
        Assert.Equal(
            "muhasebeci-paketi-2026-08.zip",
            download.Content.Headers.ContentDisposition!.FileNameStar);
        Assert.Contains(archive.Entries, entry => entry.FullName == "summary.csv");
        Assert.Contains(archive.Entries, entry => entry.FullName == "lines.csv");
        Assert.Contains(archive.Entries, entry => entry.FullName == "attachments.csv");

        var lines = ReadEntry(archive, "lines.csv");
        var summary = ReadEntry(archive, "summary.csv");
        Assert.DoesNotContain(
            context.PersonalTransactionId.ToString("D"),
            lines,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains("scope,business", summary, StringComparison.Ordinal);
    }

    /// <summary>
    /// Paket sahibine kapsamlıdır: başkasının ayı boş döner, hata değil.
    /// </summary>
    [Fact]
    public async Task Package_IsScopedToItsOwner()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory);
        using var other = await CreateAuthenticatedClientAsync(factory);
        await SeedMonthAsync(owner);

        using var response = await other.GetAsync(
            "/api/v1/accountant-package?year=2026&month=8",
            CancellationToken.None);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(document.RootElement.GetProperty("lines").EnumerateArray());
        Assert.Equal("0.0000", document.RootElement.GetProperty("totalExpense").GetString());
    }

    [Fact]
    public async Task Package_WithAnImpossiblePeriod_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        using var response = await client.GetAsync(
            "/api/v1/accountant-package?year=2026&month=13",
            CancellationToken.None);
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(CancellationToken.None));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "accountant_package.invalid_period",
            document.RootElement.GetProperty("code").GetString());
    }

    /// <summary>
    /// Kayda bağlı belge paketin içine giriyor: muhasebeci fişi ayrıca
    /// istemek zorunda kalmıyor.
    /// </summary>
    [Fact]
    public async Task Package_CarriesTheDocumentsAttachedToItsLines()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var context = await SeedMonthAsync(client);

        using var file = new ByteArrayContent(MinimalPng);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        using var multipart = new MultipartFormDataContent { { file, "file", "fis.png" } };
        using var upload = await client.PostAsync(
            $"/api/v1/transactions/{context.BusinessTransactionId}/attachments",
            multipart,
            CancellationToken.None);
        upload.EnsureSuccessStatusCode();

        using var download = await client.GetAsync(
            "/api/v1/exports/accountant-package.zip?year=2026&month=8",
            CancellationToken.None);
        var bytes = await download.Content.ReadAsByteArrayAsync(CancellationToken.None);
        using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

        var attachmentEntry = Assert.Single(
            archive.Entries,
            entry => entry.FullName.StartsWith("attachments/", StringComparison.Ordinal));
        Assert.EndsWith("fis.png", attachmentEntry.FullName, StringComparison.Ordinal);
        Assert.Contains(
            "true",
            ReadEntry(archive, "attachments.csv"),
            StringComparison.Ordinal);

        // Satır ekin varlığını da taşıyor; muhasebeci hangi kayıtta belge
        // olduğunu listeden görüyor.
        Assert.Contains(",1", ReadEntry(archive, "lines.csv"), StringComparison.Ordinal);
    }

    private static decimal SumOf(IEnumerable<JsonElement> lines, string type) => lines
        .Where(line => line.GetProperty("type").GetString() == type)
        .Sum(line => Decimal(line.GetProperty("amount").GetString()));

    private static decimal Decimal(string? value) =>
        decimal.Parse(value!, CultureInfo.InvariantCulture);

    private static string ReadEntry(ZipArchive archive, string name)
    {
        using var stream = archive.GetEntry(name)!.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private sealed record SeedContext(Guid PersonalTransactionId, Guid BusinessTransactionId);

    /// <summary>
    /// Bir ayın işletme ve şahsi kayıtları: paket yalnız ilkini görmeli.
    /// </summary>
    private static async Task<SeedContext> SeedMonthAsync(HttpClient client)
    {
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest("Banka", "bank", "TRY", "10000"),
            CancellationToken.None);
        accountResponse.EnsureSuccessStatusCode();
        var account = (await accountResponse.Content.ReadFromJsonAsync<AccountResponse>(
            CancellationToken.None))!;

        var expenseCategories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);
        var incomeCategories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=income",
            CancellationToken.None);
        var expenseCategory = expenseCategories!.Items[0];
        var incomeCategory = incomeCategories!.Items[0];

        // İşletme gideri: KDV taşıyor ve indirilebilir.
        using var deductible = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id,
                expenseCategory.Id,
                "120.0000",
                "TRY",
                "expense",
                "business",
                $"{Month}-05",
                "Ticari mal",
                VatRate: "0.2000",
                VatAmount: "20.0000",
                IsTaxDeductible: true),
            CancellationToken.None);
        deductible.EnsureSuccessStatusCode();
        var businessTransaction = (await deductible.Content.ReadFromJsonAsync<TransactionResponse>(
            CancellationToken.None))!;

        // İndirilemeyen işletme gideri: gider olarak sayılır, pakette ayrıca
        // işaretlenir.
        using var nonDeductible = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id,
                expenseCategory.Id,
                "500.0000",
                "TRY",
                "expense",
                "business",
                $"{Month}-06",
                "Trafik cezası",
                IsTaxDeductible: false),
            CancellationToken.None);
        nonDeductible.EnsureSuccessStatusCode();

        // Şahsi gider: pakete girmemeli.
        using var personal = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id,
                expenseCategory.Id,
                "300.0000",
                "TRY",
                "expense",
                "personal",
                $"{Month}-07",
                "Market alışverişi"),
            CancellationToken.None);
        personal.EnsureSuccessStatusCode();
        var personalTransaction = (await personal.Content.ReadFromJsonAsync<TransactionResponse>(
            CancellationToken.None))!;

        // Kart harcaması: işletme gideri, ayrı yazma modeli.
        using var cardResponse = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Kart", "20000", "TRY", 10, 20),
            CancellationToken.None);
        cardResponse.EnsureSuccessStatusCode();
        var card = (await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>(
            CancellationToken.None))!;
        using var charge = await client.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(
                expenseCategory.Id,
                "250.0000",
                "TRY",
                "business",
                $"{Month}-08",
                "Yakıt"),
            CancellationToken.None);
        charge.EnsureSuccessStatusCode();

        // POS tahsilatı: satış brüt gelir, komisyon ayrı gider.
        using var settlement = await client.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                incomeCategory.Id,
                "1080.0000",
                "TRY",
                $"{Month}-09",
                $"{Month}-11",
                CommissionAmount: "30.0000",
                CommissionCategoryId: expenseCategory.Id,
                Scope: "business",
                Description: "Kartlı satış",
                VatRate: "0.2000",
                VatAmount: "180.0000"),
            CancellationToken.None);
        settlement.EnsureSuccessStatusCode();

        return new SeedContext(personalTransaction.Id, businessTransaction.Id);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory)
    {
        var client = factory.CreateClient();
        var email = $"package-{Guid.NewGuid():N}@example.test";
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
