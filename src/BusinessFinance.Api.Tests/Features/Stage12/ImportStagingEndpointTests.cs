using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Imports;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Infrastructure.Persistence;
using BusinessFinance.Domain;
using BusinessFinance.Application.Imports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Stage12;

public sealed class ImportStagingEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Stage_MixedCsv_PersistsPreviewOnlyAndIsolatesOwner()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await AuthenticateAsync(factory, "import-owner@example.test");
        using var other = await AuthenticateAsync(factory, "import-other@example.test");
        using var form = CreateCsvForm(
            "Tarih;Tutar;Açıklama\n2026-08-11;-25,5000;Market\nbad;0;Broken");

        using var response = await owner.PostAsync("/api/v1/imports/csv/stage", form);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var batch = await response.Content.ReadFromJsonAsync<ImportBatchResponse>();
        Assert.NotNull(batch);
        Assert.Equal(2, batch.TotalRowCount);
        Assert.Equal(1, batch.ValidRowCount);
        Assert.Equal(1, batch.InvalidRowCount);
        Assert.Equal("-25.5000", batch.Rows[0].SignedAmount);
        Assert.Equal("2026-08-11", batch.Rows[0].TransactionDate);

        using var ownerRead = await owner.GetAsync($"/api/v1/imports/{batch.Id}");
        ownerRead.EnsureSuccessStatusCode();
        using var foreignRead = await other.GetAsync($"/api/v1/imports/{batch.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignRead.StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        Assert.Empty(db.Transactions);
        Assert.Equal(2, db.ImportRows.Count());
    }

    [Fact]
    public async Task Stage_WrongExtensionOrContentType_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "import-type@example.test");
        using var form = CreateCsvForm("Date,Amount\n2026-08-11,10", "statement.txt", "text/plain");

        using var response = await client.PostAsync("/api/v1/imports/csv/stage", form);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CorrectAndConfirm_SelectedReadyRow_ImportsAtomicallyAndLeavesInvalidRow()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "import-confirm@example.test");
        var account = await CreateAccountAsync(client);
        var category = await GetCategoryAsync(client, "expense");
        using var form = CreateCsvForm(
            "Tarih;Tutar;Açıklama\n2026-08-11;-25,5000;Market\nbad;0;Broken");
        using var stageResponse = await client.PostAsync("/api/v1/imports/csv/stage", form);
        var staged = await stageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        var selected = staged!.Rows[0];

        using var patchResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/rows/{selected.Id}",
            new UpdateImportCandidateRequest(
                "2026-08-11", "-25.5000", "Corrected market", "bank-ref-1",
                account.Id, category.Id));
        patchResponse.EnsureSuccessStatusCode();
        var candidate = await patchResponse.Content.ReadFromJsonAsync<ImportRowResponse>();
        Assert.Equal("ready", candidate!.Status);
        Assert.Equal("expense", candidate.TransactionType);

        using var confirmResponse = await client.PostAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/confirm",
            new ConfirmImportBatchRequest([selected.Id]));
        confirmResponse.EnsureSuccessStatusCode();
        var confirmed = await confirmResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        Assert.Equal("partially-imported", confirmed!.Status);
        Assert.NotNull(confirmed.Rows[0].BudgetTransactionId);
        Assert.Equal("imported", confirmed.Rows[0].Status);
        Assert.Equal("invalid", confirmed.Rows[1].Status);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        var transaction = Assert.Single(db.Transactions);
        Assert.Equal(25.5000m, transaction.Amount.Amount);
        Assert.Equal(TransactionType.Expense, transaction.Type);
    }

    [Fact]
    public async Task Confirm_WhenAnySelectedRowIsUnmapped_CreatesNothing()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "import-atomic@example.test");
        var account = await CreateAccountAsync(client);
        var category = await GetCategoryAsync(client, "expense");
        using var form = CreateCsvForm(
            "Tarih;Tutar;Açıklama\n2026-08-11;-10,0000;One\n2026-08-12;-20,0000;Two");
        using var stageResponse = await client.PostAsync("/api/v1/imports/csv/stage", form);
        var staged = await stageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        using var patchResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{staged!.Id}/rows/{staged.Rows[0].Id}",
            new UpdateImportCandidateRequest(
                "2026-08-11", "-10.0000", "One", null, account.Id, category.Id));
        patchResponse.EnsureSuccessStatusCode();

        using var confirmResponse = await client.PostAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/confirm",
            new ConfirmImportBatchRequest(staged.Rows.Select(row => row.Id).ToArray()));
        Assert.Equal(HttpStatusCode.Conflict, confirmResponse.StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        Assert.Empty(db.Transactions);
        Assert.All(db.ImportRows, row => Assert.NotEqual(ImportRowStatus.Imported, row.Status));
    }

    [Fact]
    public async Task Confirm_MoreThanFiveThousandRows_IsRejected()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "import-limit@example.test");
        var rowIds = Enumerable.Range(0, ConfirmImportBatchUseCase.MaximumConfirmationRows + 1)
            .Select(_ => Guid.NewGuid()).ToArray();

        using var response = await client.PostAsJsonAsync(
            $"/api/v1/imports/{Guid.NewGuid()}/confirm",
            new ConfirmImportBatchRequest(rowIds));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StageRetryAndDuplicateOverride_ReturnExistingBatchAndImportOnlyOnce()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await AuthenticateAsync(factory, "import-duplicate@example.test");
        var account = await CreateAccountAsync(client);
        var category = await GetCategoryAsync(client, "expense");
        using var existingResponse = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, category.Id, "42.5000", "TRY", "expense",
                "business",
                "2026-08-11", "Grocery   SHOP"));
        existingResponse.EnsureSuccessStatusCode();
        var existing = await existingResponse.Content.ReadFromJsonAsync<TransactionResponse>();
        const string csv = "Tarih;Tutar;Açıklama\n2026-08-11;-42,5000;grocery shop";
        using var firstForm = CreateCsvForm(csv);
        using var firstStageResponse = await client.PostAsync("/api/v1/imports/csv/stage", firstForm);
        var firstBatch = await firstStageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        using var retryForm = CreateCsvForm(csv);
        using var retryStageResponse = await client.PostAsync("/api/v1/imports/csv/stage", retryForm);
        var retryBatch = await retryStageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        Assert.Equal(firstBatch!.Id, retryBatch!.Id);
        Assert.Equal(firstBatch.FileFingerprint, retryBatch.FileFingerprint);

        var row = firstBatch.Rows.Single();
        using var patchResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{firstBatch.Id}/rows/{row.Id}",
            new UpdateImportCandidateRequest(
                "2026-08-11", "-42.5000", " grocery shop ", null,
                account.Id, category.Id));
        patchResponse.EnsureSuccessStatusCode();
        var duplicate = await patchResponse.Content.ReadFromJsonAsync<ImportRowResponse>();
        Assert.Equal("pending-duplicate-review", duplicate!.Status);
        Assert.Equal(existing!.Id, duplicate.DuplicateTransactionId);
        Assert.Equal("date-amount-description", duplicate.DuplicateReason);

        using var blockedConfirm = await client.PostAsJsonAsync(
            $"/api/v1/imports/{firstBatch.Id}/confirm",
            new ConfirmImportBatchRequest([row.Id]));
        Assert.Equal(HttpStatusCode.Conflict, blockedConfirm.StatusCode);
        using var decisionResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{firstBatch.Id}/rows/{row.Id}/duplicate-decision",
            new ResolveImportDuplicateRequest("import-anyway"));
        decisionResponse.EnsureSuccessStatusCode();

        using var confirm = await client.PostAsJsonAsync(
            $"/api/v1/imports/{firstBatch.Id}/confirm",
            new ConfirmImportBatchRequest([row.Id]));
        confirm.EnsureSuccessStatusCode();
        using var confirmRetry = await client.PostAsJsonAsync(
            $"/api/v1/imports/{firstBatch.Id}/confirm",
            new ConfirmImportBatchRequest([row.Id]));
        confirmRetry.EnsureSuccessStatusCode();

        using var other = await AuthenticateAsync(factory, "import-duplicate-other@example.test");
        using var otherForm = CreateCsvForm(csv);
        using var otherStageResponse = await other.PostAsync("/api/v1/imports/csv/stage", otherForm);
        var otherBatch = await otherStageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        Assert.NotEqual(firstBatch.Id, otherBatch!.Id);
        Assert.Equal(firstBatch.FileFingerprint, otherBatch.FileFingerprint);

        using var skipForm = CreateCsvForm(
            "Tarih;Tutar;Açıklama\n2026-08-11;-42,5000; grocery   shop ");
        using var skipStageResponse = await client.PostAsync("/api/v1/imports/csv/stage", skipForm);
        var skipBatch = await skipStageResponse.Content.ReadFromJsonAsync<ImportBatchResponse>();
        var skipRow = skipBatch!.Rows.Single();
        using var skipPatchResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{skipBatch.Id}/rows/{skipRow.Id}",
            new UpdateImportCandidateRequest(
                "2026-08-11", "-42.5000", "grocery shop", null,
                account.Id, category.Id));
        skipPatchResponse.EnsureSuccessStatusCode();
        using var skipDecisionResponse = await client.PatchAsJsonAsync(
            $"/api/v1/imports/{skipBatch.Id}/rows/{skipRow.Id}/duplicate-decision",
            new ResolveImportDuplicateRequest("skip"));
        skipDecisionResponse.EnsureSuccessStatusCode();
        var skipped = await skipDecisionResponse.Content.ReadFromJsonAsync<ImportRowResponse>();
        Assert.Equal("skipped-duplicate", skipped!.Status);
        var completedSkipBatch = await client.GetFromJsonAsync<ImportBatchResponse>(
            $"/api/v1/imports/{skipBatch.Id}");
        Assert.Equal("imported", completedSkipBatch!.Status);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        Assert.Equal(2, db.Transactions.Count());
        Assert.Equal(3, db.ImportBatches.Count());
    }

    private static MultipartFormDataContent CreateCsvForm(
        string csv,
        string fileName = "statement.csv",
        string contentType = "text/csv")
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(csv));
        file.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        form.Add(file, "file", fileName);
        form.Add(new StringContent("Tarih"), "dateColumn");
        form.Add(new StringContent("Tutar"), "amountColumn");
        form.Add(new StringContent("Açıklama"), "descriptionColumn");
        form.Add(new StringContent("semicolon"), "delimiter");
        form.Add(new StringContent(","), "decimalSeparator");
        return form;
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
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
            new CreateAccountRequest("Import account", "bank", "TRY", "0", "business"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetCategoryAsync(HttpClient client, string type)
    {
        var response = await client.GetFromJsonAsync<CategoryListResponse>($"/api/v1/categories?type={type}");
        return response!.Items.First(item => item.DefaultScope == "business");
    }
}
