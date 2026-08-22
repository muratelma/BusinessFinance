using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Attachments;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Attachments;

public sealed class AttachmentEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private static readonly byte[] MinimalPng = [137, 80, 78, 71, 13, 10, 26, 10];

    [Fact]
    public async Task UploadListAndDownload_AreOwnerScopedAndPreserveSafeMetadata()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "attachment-owner@example.test");
        using var other = await CreateAuthenticatedClientAsync(factory, "attachment-other@example.test");
        var transaction = await CreateTransactionAsync(owner);

        using var upload = await UploadAsync(
            owner, transaction.Id, MinimalPng, "../../receipt.png", "image/png");
        Assert.Equal(HttpStatusCode.Created, upload.StatusCode);
        var attachment = (await upload.Content.ReadFromJsonAsync<AttachmentResponse>())!;
        Assert.Equal("receipt.png", attachment.FileName);
        Assert.Equal("image/png", attachment.ContentType);
        Assert.Equal(MinimalPng.Length, attachment.SizeBytes);
        Assert.Equal(64, attachment.Sha256.Length);

        var list = await owner.GetFromJsonAsync<AttachmentListResponse>(
            $"/api/v1/transactions/{transaction.Id}/attachments");
        Assert.Equal(attachment.Id, Assert.Single(list!.Items).Id);
        using var download = await owner.GetAsync($"/api/v1/attachments/{attachment.Id}/content");
        download.EnsureSuccessStatusCode();
        Assert.Equal("image/png", download.Content.Headers.ContentType!.MediaType);
        Assert.Equal(MinimalPng, await download.Content.ReadAsByteArrayAsync());

        using var foreignDownload = await other.GetAsync(
            $"/api/v1/attachments/{attachment.Id}/content");
        Assert.Equal(HttpStatusCode.NotFound, foreignDownload.StatusCode);
        using var foreignList = await other.GetAsync(
            $"/api/v1/transactions/{transaction.Id}/attachments");
        Assert.Equal(HttpStatusCode.NotFound, foreignList.StatusCode);
    }

    [Fact]
    public async Task Upload_RejectsSignatureMismatchActivePdfAndOversizeMetadata()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "attachment-reject@example.test");
        var transaction = await CreateTransactionAsync(owner);

        using var mismatch = await UploadAsync(
            owner, transaction.Id, "%PDF-fake"u8.ToArray(), "fake.png", "image/png");
        Assert.Equal(HttpStatusCode.BadRequest, mismatch.StatusCode);
        using var activePdf = await UploadAsync(
            owner, transaction.Id, "%PDF-1.7 /JavaScript"u8.ToArray(), "active.pdf", "application/pdf");
        Assert.Equal(HttpStatusCode.BadRequest, activePdf.StatusCode);
        var activeProblem = await activePdf.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(
            "Aktif veya gömülü içerik barındıran PDF dosyaları yüklenemez.",
            activeProblem.GetProperty("detail").GetString());
        var oversize = new ByteArrayContent([1]);
        oversize.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        using var multipart = new MultipartFormDataContent();
        multipart.Add(oversize, "file", "large.png");
        multipart.Headers.ContentLength = 5 * 1024 * 1024 + 65_537;
        using var tooLarge = await owner.PostAsync(
            $"/api/v1/transactions/{transaction.Id}/attachments", multipart);
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, tooLarge.StatusCode);
    }

    private static async Task<HttpResponseMessage> UploadAsync(
        HttpClient client,
        Guid transactionId,
        byte[] content,
        string fileName,
        string contentType)
    {
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var multipart = new MultipartFormDataContent();
        multipart.Add(file, "file", fileName);
        return await client.PostAsync(
            $"/api/v1/transactions/{transactionId}/attachments", multipart);
    }

    private static async Task<TransactionResponse> CreateTransactionAsync(HttpClient client)
    {
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest("Attachment Account", "bank", "TRY"));
        accountResponse.EnsureSuccessStatusCode();
        var account = (await accountResponse.Content.ReadFromJsonAsync<AccountResponse>())!;
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, categories!.Items[0].Id, "10.0000", "TRY",
                "expense", "business", "2026-08-11", "Synthetic receipt"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
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
}
