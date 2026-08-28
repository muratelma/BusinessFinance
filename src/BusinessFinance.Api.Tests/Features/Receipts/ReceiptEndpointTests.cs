using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Receipts;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Api.Tests.Features.Receipts;

public sealed class ReceiptEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private static readonly byte[] MinimalPng = [137, 80, 78, 71, 13, 10, 26, 10];

    [Fact]
    public async Task Analyze_UsesOnlyOwnersCategories_ReturnsSuggestionAndWritesNothing()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-owner@example.test");
        using var other = await AuthenticateAsync(factory, "receipt-other@example.test");
        var ownerCategory = await CreateCategoryAsync(owner, "Sahibin özel marketi");
        var otherCategory = await CreateCategoryAsync(other, "Başkasının özel marketi");
        analyzer.CategoryName = ownerCategory.Name;

        var rowsBefore = FinancialRowCount(factory);
        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        Assert.NotNull(draft);
        Assert.Equal("purchase_receipt", draft.DocumentKind);
        Assert.Equal("Sentetik Market", draft.CounterpartyName);
        Assert.Equal("847.5000", draft.TotalAmount);
        Assert.Equal("read", draft.TotalAmountState);
        Assert.Equal("card", draft.PaymentHint);
        Assert.Equal(ownerCategory.Id, draft.CategoryId);
        Assert.Equal(ownerCategory.Name, draft.CategoryName);

        var request = Assert.Single(analyzer.Requests);
        Assert.Contains(ownerCategory.Name, request.CategoryNames);
        Assert.DoesNotContain(otherCategory.Name, request.CategoryNames);
        Assert.Equal(rowsBefore, FinancialRowCount(factory));
    }

    /// <summary>
    /// The wire value is part of the contract the Flutter client parses, and the
    /// card kinds are the reason it matters: a debit payment routed to the credit
    /// cards would put a bank-card expense on a card balance.
    /// </summary>
    [Theory]
    [InlineData("cash", "cash")]
    [InlineData("credit_card", "credit_card")]
    [InlineData("debit_card", "debit_card")]
    [InlineData("card", "card")]
    [InlineData("unknown", "unknown")]
    [InlineData("kredi karti", "unknown")]
    public async Task Analyze_PaymentHint_ReachesTheClientAsAStableWireValue(
        string written,
        string expected)
    {
        var analyzer = new RecordingAnalyzer { PaymentHint = written };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, $"receipt-hint-{expected}@example.test");

        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        Assert.NotNull(draft);
        Assert.Equal(expected, draft.PaymentHint);
    }

    /// <summary>
    /// The direction is a form field, not something the server works out. A till
    /// receipt sent as income is a contradiction the user needs to see; silently
    /// posting income the user never earned would be the expensive failure.
    /// </summary>
    [Fact]
    public async Task Analyze_TillReceiptSentAsIncome_IsRefusedWithTheMismatchCode()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-intent-mismatch@example.test");

        using var response = await AnalyzeAsync(owner, intent: "income");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("receipt.intent_mismatch", await ProblemCodeAsync(response));
    }

    /// <summary>
    /// An invoice names both parties, so income is a legitimate reading of it —
    /// and the buckets offered to the model must then be the income ones.
    /// </summary>
    [Fact]
    public async Task Analyze_InvoiceSentAsIncome_IsReadWithTheUsersIncomeBuckets()
    {
        var analyzer = new RecordingAnalyzer { DocumentType = "invoice_or_voucher" };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-income@example.test");
        var income = await CreateCategoryAsync(owner, "Sahibin özel geliri", "income");
        var expense = await CreateCategoryAsync(owner, "Sahibin özel kovası");

        var rowsBefore = FinancialRowCount(factory);
        using var response = await AnalyzeAsync(owner, intent: "income");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var request = Assert.Single(analyzer.Requests);
        Assert.Equal(ReceiptCaptureIntent.Income, request.Intent);
        // Gelir niyetinde yalnız gelir kovaları gidiyor; gider kovası gitmiyor.
        Assert.Contains(income.Name, request.CategoryNames);
        Assert.DoesNotContain(expense.Name, request.CategoryNames);
        Assert.Equal(rowsBefore, FinancialRowCount(factory));
    }

    /// <summary>
    /// Missing means expense — the direction the product assumed before 12.11.
    /// An unrecognised value is refused instead of guessed.
    /// </summary>
    [Theory]
    [InlineData(null, HttpStatusCode.OK)]
    [InlineData("expense", HttpStatusCode.OK)]
    [InlineData("EXPENSE", HttpStatusCode.OK)]
    [InlineData("gider", HttpStatusCode.BadRequest)]
    [InlineData("transfer", HttpStatusCode.BadRequest)] // fiş, dekont değil
    [InlineData("bank_slip", HttpStatusCode.BadRequest)] // fiş, dekont değil
    [InlineData("dekont", HttpStatusCode.BadRequest)] // tanınmayan değer
    public async Task Analyze_IntentField_DefaultsToExpenseAndRefusesUnknownValues(
        string? intent,
        HttpStatusCode expected)
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(
            factory, $"receipt-intent-{intent ?? "missing"}@example.test");

        using var response = await AnalyzeAsync(owner, intent: intent);

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Analyze_WithoutAuthentication_Returns401BeforeProvider()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var client = factory.CreateClient();

        using var response = await AnalyzeAsync(client);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Empty(analyzer.Requests);
    }

    [Fact]
    public async Task Analyze_RejectsPdfSignatureMismatchAndOversizeWithStableCodes()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var client = await AuthenticateAsync(factory, "receipt-files@example.test");

        using var pdf = await AnalyzeAsync(
            client, "%PDF-1.7 synthetic"u8.ToArray(), "receipt.pdf", "application/pdf");
        Assert.Equal(HttpStatusCode.BadRequest, pdf.StatusCode);
        Assert.Equal("receipt.unsupported_file", await ProblemCodeAsync(pdf));

        using var mismatch = await AnalyzeAsync(
            client, "%PDF-fake"u8.ToArray(), "receipt.png", "image/png");
        Assert.Equal(HttpStatusCode.BadRequest, mismatch.StatusCode);
        Assert.Equal("receipt.unsupported_file", await ProblemCodeAsync(mismatch));

        var file = new ByteArrayContent([1]);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        using var multipart = new MultipartFormDataContent();
        multipart.Add(file, "file", "large.png");
        multipart.Headers.ContentLength = AnalyzeReceiptUseCase.MaximumFileSizeBytes + 65_537;
        using var oversize = await client.PostAsync("/api/v1/receipts/analyze", multipart);
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, oversize.StatusCode);
        Assert.Equal("receipt.file_too_large", await ProblemCodeAsync(oversize));
        Assert.Empty(analyzer.Requests);
    }

    [Theory]
    [InlineData("receipt.unreadable", HttpStatusCode.BadRequest)]
    [InlineData("receipt.bank_document", HttpStatusCode.BadRequest)]
    [InlineData("receipt.not_a_receipt", HttpStatusCode.BadRequest)]
    [InlineData("receipt.refund_document", HttpStatusCode.BadRequest)]
    [InlineData("receipt.provider_unavailable", HttpStatusCode.ServiceUnavailable)]
    [InlineData("receipt.provider_rate_limited", HttpStatusCode.TooManyRequests)]
    [InlineData("receipt.disabled", HttpStatusCode.ServiceUnavailable)]
    public async Task Analyze_ProviderError_ReachesClientWithStableStatusAndWithoutAWrite(
        string errorCode,
        HttpStatusCode expectedStatus)
    {
        var error = errorCode switch
        {
            "receipt.unreadable" => ReceiptAnalysisErrors.Unreadable,
            "receipt.bank_document" => ReceiptAnalysisErrors.BankDocument,
            "receipt.not_a_receipt" => ReceiptAnalysisErrors.NotAReceipt,
            "receipt.refund_document" => ReceiptAnalysisErrors.RefundDocument,
            "receipt.provider_unavailable" => ReceiptAnalysisErrors.ProviderUnavailable,
            "receipt.provider_rate_limited" => ReceiptAnalysisErrors.ProviderRateLimited,
            "receipt.disabled" => ReceiptAnalysisErrors.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode))
        };
        var analyzer = new RecordingAnalyzer(error);
        await using var factory = CreateFactory(analyzer);
        using var client = await AuthenticateAsync(
            factory, $"{errorCode.Replace('.', '-')}@example.test");
        var rowsBefore = FinancialRowCount(factory);

        using var response = await AnalyzeAsync(client);

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(errorCode, await ProblemCodeAsync(response));
        Assert.Equal(rowsBefore, FinancialRowCount(factory));
    }

    [Fact]
    public async Task Analyze_RateLimitHasOneBucketPerAuthenticatedUser()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer);
        using var first = await AuthenticateAsync(factory, "receipt-rate-a@example.test");
        using var second = await AuthenticateAsync(factory, "receipt-rate-b@example.test");

        for (var attempt = 0; attempt < RateLimitingExtensions.ReceiptAnalysisPermitLimit; attempt++)
        {
            using var accepted = await AnalyzeAsync(first);
            Assert.Equal(HttpStatusCode.OK, accepted.StatusCode);
        }

        using var limited = await AnalyzeAsync(first);
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        Assert.Equal("rate_limit.exceeded", await ProblemCodeAsync(limited));

        using var otherUsersFirstRequest = await AnalyzeAsync(second);
        Assert.Equal(HttpStatusCode.OK, otherUsersFirstRequest.StatusCode);
        Assert.Equal(RateLimitingExtensions.ReceiptAnalysisPermitLimit + 1, analyzer.Requests.Count);
    }

    /// <summary>
    /// Reading the same receipt twice used to write two expenses in silence.
    /// The warning is owner-scoped: another user's identical record is not this
    /// user's duplicate, and saying so would leak that it exists.
    /// </summary>
    [Fact]
    public async Task Analyze_DocumentAlreadyRecorded_WarnsOnlyItsOwner()
    {
        var analyzer = new RecordingAnalyzer
        {
            PurchasedAt = "2026-08-11",
            CategoryName = null
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-dup-owner@example.test");
        using var other = await AuthenticateAsync(factory, "receipt-dup-other@example.test");
        // Aynı gün, aynı tutar, aynı ad — ama başka kullanıcının defterinde.
        await CreateExpenseAsync(other, "847.5000", "2026-08-11", "Sentetik Market");

        using var beforeOwn = await AnalyzeAsync(owner);
        Assert.Equal(HttpStatusCode.OK, beforeOwn.StatusCode);
        Assert.DoesNotContain(
            "receipt.possible_duplicate",
            await WarningCodesAsync(beforeOwn));

        await CreateExpenseAsync(owner, "847.5000", "2026-08-11", "Sentetik Market");

        using var afterOwn = await AnalyzeAsync(owner);
        Assert.Equal(HttpStatusCode.OK, afterOwn.StatusCode);
        Assert.Contains("receipt.possible_duplicate", await WarningCodesAsync(afterOwn));
    }

    /// <summary>
    /// A different amount on the same day is a different purchase; the match is
    /// strict on purpose, because a warning nobody trusts costs every later one.
    /// </summary>
    [Fact]
    public async Task Analyze_SimilarButNotIdenticalRecord_DoesNotWarn()
    {
        var analyzer = new RecordingAnalyzer
        {
            PurchasedAt = "2026-08-11",
            CategoryName = null
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-dup-near@example.test");
        await CreateExpenseAsync(owner, "847.6000", "2026-08-11", "Sentetik Market");

        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(
            "receipt.possible_duplicate",
            await WarningCodesAsync(response));
    }

    /// <summary>
    /// İade fişi artık reddedilmiyor: okunuyor ve geri verdiği harcama
    /// aranıyor. Eşleşme **sahibine kapsamlı** — başka kullanıcının aynı
    /// harcaması bu iadenin karşılığı değildir ve olduğunu söylemek onun
    /// varlığını sızdırırdı.
    /// </summary>
    [Fact]
    public async Task Analyze_RefundSlip_MatchesOnlyItsOwnersExpense()
    {
        var analyzer = new RecordingAnalyzer
        {
            DocumentType = "refund_receipt",
            PurchasedAt = "2026-08-12",
            CategoryName = null,
            Subtotal = null,
            Tax = null,
            TotalAmount = "847,50"
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-refund-owner@example.test");
        using var other = await AuthenticateAsync(factory, "receipt-refund-other@example.test");
        await CreateExpenseAsync(other, "847.5000", "2026-08-11", "Sentetik Market");

        using var beforeOwn = await AnalyzeAsync(owner);
        Assert.Equal(HttpStatusCode.OK, beforeOwn.StatusCode);
        Assert.Null(await RefundMatchAsync(beforeOwn));

        await CreateExpenseAsync(owner, "847.5000", "2026-08-11", "Sentetik Market");

        using var afterOwn = await AnalyzeAsync(owner);
        Assert.Equal(HttpStatusCode.OK, afterOwn.StatusCode);
        var match = await RefundMatchAsync(afterOwn);
        Assert.NotNull(match);
        Assert.Equal("847.5000", match.Amount);
        // Tam iade: iptal tek başına yeterli, yazılacak kalan yok.
        Assert.Null(match.RemainingAmount);
    }

    /// <summary>
    /// Kısmi iadede kalan tutar **sunucuda** hesaplanır: istemci finansal
    /// toplamı ikinci kez hesaplamaz.
    /// </summary>
    [Fact]
    public async Task Analyze_PartialRefund_ReportsWhatTheExpenseShouldBecome()
    {
        var analyzer = new RecordingAnalyzer
        {
            DocumentType = "refund_receipt",
            PurchasedAt = "2026-08-12",
            CategoryName = null,
            Subtotal = null,
            Tax = null,
            TotalAmount = "200,00"
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-refund-partial@example.test");
        await CreateExpenseAsync(owner, "847.5000", "2026-08-11", "Sentetik Market");

        using var response = await AnalyzeAsync(owner);

        var match = await RefundMatchAsync(response);
        Assert.NotNull(match);
        Assert.Equal("847.5000", match.Amount);
        Assert.Equal("647.5000", match.RemainingAmount);
    }

    /// <summary>
    /// Harcamadan büyük bir iade o harcamanın karşılığı olamaz; aritmetiği
    /// tutmayan bir aday sunmak, kullanıcıya yanlış kaydı iptal ettirmeye
    /// davettir.
    /// </summary>
    [Fact]
    public async Task Analyze_RefundLargerThanAnyExpense_FindsNothing()
    {
        var analyzer = new RecordingAnalyzer
        {
            DocumentType = "refund_receipt",
            PurchasedAt = "2026-08-12",
            CategoryName = null,
            Subtotal = null,
            Tax = null,
            TotalAmount = "900,00"
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-refund-large@example.test");
        await CreateExpenseAsync(owner, "847.5000", "2026-08-11", "Sentetik Market");

        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(await RefundMatchAsync(response));
    }

    /// <summary>
    /// A dekont reaches the client with the moved amount and the fee in separate
    /// fields. Adding them produced a 5.004,50 expense that was never spent.
    /// </summary>
    [Fact]
    public async Task Analyze_TransferSlip_ReportsTheFeeSeparatelyFromTheAmount()
    {
        var analyzer = new RecordingAnalyzer
        {
            DocumentType = "bank_document",
            FeeAmount = "4,50"
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-transfer@example.test");

        var rowsBefore = FinancialRowCount(factory);
        using var response = await AnalyzeAsync(owner, intent: "transfer");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        Assert.Equal("bank_document", draft!.DocumentKind);
        Assert.Equal("847.5000", draft.TotalAmount);
        Assert.Equal("4.5000", draft.FeeAmount);
        Assert.Equal("read", draft.FeeAmountState);
        // Transferin kategorisi yok: modele hiç kova gönderilmiyor.
        Assert.Empty(Assert.Single(analyzer.Requests).CategoryNames);
        Assert.Equal(rowsBefore, FinancialRowCount(factory));
    }

    /// <summary>
    /// Belge türü taslakta taşınıyor çünkü istemcinin bir sonraki adımı buna
    /// bağlı: banka belgesinde ana tutarın ne olduğu kullanıcıya sorulur,
    /// alışveriş fişinde sorulmaz.
    /// </summary>
    [Fact]
    public async Task Analyze_PaymentSlip_TellsTheClientItIsABankDocument()
    {
        var analyzer = new RecordingAnalyzer
        {
            DocumentType = "bank_payment",
            FeeAmount = "4,50",
            CategoryName = null
        };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-kind@example.test");

        using var response = await AnalyzeAsync(owner, intent: "expense");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        Assert.Equal("bank_payment", draft!.DocumentKind);
        Assert.Equal("4.5000", draft.FeeAmount);
    }

    [Fact]
    public async Task Analyze_TransferSlipWithoutTransferIntent_IsRefused()
    {
        var analyzer = new RecordingAnalyzer { DocumentType = "bank_document" };
        await using var factory = CreateFactory(analyzer);
        using var owner = await AuthenticateAsync(factory, "receipt-transfer-no@example.test");

        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("receipt.bank_document", await ProblemCodeAsync(response));
    }

    [SqlServerFact]
    public async Task Analyze_RealSql_KeepsCategoryOwnershipAndFinancialTablesReadOnly()
    {
        var analyzer = new RecordingAnalyzer();
        await using var factory = CreateFactory(analyzer, useConfiguredSqlServer: true);
        Guid ownerId = default;
        using var owner = await AuthenticateAsync(
            factory,
            $"receipt-sql-a-{Guid.NewGuid():N}@example.test",
            tokens => ownerId = tokens.UserId);
        using var other = await AuthenticateAsync(
            factory, $"receipt-sql-b-{Guid.NewGuid():N}@example.test");
        var ownerCategory = await CreateCategoryAsync(
            owner, $"SQL sahibi {Guid.NewGuid():N}");
        var otherCategory = await CreateCategoryAsync(
            other, $"SQL yabancı {Guid.NewGuid():N}");
        analyzer.CategoryName = ownerCategory.Name;
        var rowsBefore = OwnerFinancialRowCount(factory, ownerId);

        using var response = await AnalyzeAsync(owner);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var request = Assert.Single(analyzer.Requests);
        Assert.Contains(ownerCategory.Name, request.CategoryNames);
        Assert.DoesNotContain(otherCategory.Name, request.CategoryNames);
        Assert.Equal(rowsBefore, OwnerFinancialRowCount(factory, ownerId));
    }

    private static BusinessFinanceApiFactory CreateFactory(
        RecordingAnalyzer analyzer,
        bool useConfiguredSqlServer = false) =>
        new(useConfiguredSqlServer: useConfiguredSqlServer, configureServices: services =>
        {
            services.RemoveAll<IReceiptAnalyzer>();
            services.RemoveAll<IReceiptImagePreprocessor>();
            services.AddSingleton<IReceiptAnalyzer>(analyzer);
            services.AddSingleton<IReceiptImagePreprocessor, PassThroughPreprocessor>();
        });

    private static async Task<HttpResponseMessage> AnalyzeAsync(
        HttpClient client,
        byte[]? content = null,
        string fileName = "receipt.png",
        string contentType = "image/png",
        string? intent = null)
    {
        var file = new ByteArrayContent(content ?? MinimalPng);
        file.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var multipart = new MultipartFormDataContent();
        multipart.Add(file, "file", fileName);
        if (intent is not null)
            multipart.Add(new StringContent(intent), "intent");
        return await client.PostAsync("/api/v1/receipts/analyze", multipart);
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email,
        Action<TokenPairResponse>? onAuthenticated = null)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        onAuthenticated?.Invoke(tokens!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<CategoryResponse> CreateCategoryAsync(
        HttpClient client,
        string name,
        string type = "expense")
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/categories", new CreateCategoryRequest(name, type));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    private static async Task<IReadOnlyList<string>> WarningCodesAsync(
        HttpResponseMessage response)
    {
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        return draft!.Warnings.Select(warning => warning.Code).ToArray();
    }

    private static async Task<ReceiptRefundMatchResponse?> RefundMatchAsync(
        HttpResponseMessage response)
    {
        var draft = await response.Content.ReadFromJsonAsync<ReceiptAnalysisResponse>();
        return draft!.RefundMatch;
    }

    private static async Task CreateExpenseAsync(
        HttpClient client,
        string amount,
        string date,
        string description)
    {
        using var accountResponse = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest($"Fiş hesabı {Guid.NewGuid():N}", "bank", "TRY"));
        accountResponse.EnsureSuccessStatusCode();
        var account = (await accountResponse.Content.ReadFromJsonAsync<AccountResponse>())!;
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense");

        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, categories!.Items[0].Id, amount, "TRY",
                "expense", "business", date, description));
        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> ProblemCodeAsync(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        return problem.GetProperty("code").GetString()!;
    }

    private static int FinancialRowCount(BusinessFinanceApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        return db.Accounts.Count() +
               db.Categories.Count() +
               db.Transactions.Count() +
               db.MonthlyBudgets.Count() +
               db.Transfers.Count() +
               db.CreditCards.Count() +
               db.CreditCardCharges.Count() +
               db.CreditCardPayments.Count() +
               db.InstallmentPlans.Count() +
               db.InstallmentItems.Count() +
               db.RecurringTransactions.Count() +
               db.RecurringTransactionOccurrences.Count() +
               db.ImportBatches.Count() +
               db.ImportRows.Count() +
               db.DebtAgreements.Count() +
               db.DebtInstallments.Count() +
               db.SavingsGoals.Count() +
               db.SavingsGoalContributions.Count() +
               db.FinancialAttachments.Count();
    }

    private static int OwnerFinancialRowCount(
        BusinessFinanceApiFactory factory,
        Guid userId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        return db.Accounts.Count(item => item.UserId == userId) +
               db.Categories.Count(item => item.UserId == userId) +
               db.Transactions.Count(item => item.UserId == userId) +
               db.MonthlyBudgets.Count(item => item.UserId == userId) +
               db.Transfers.Count(item => item.UserId == userId) +
               db.CreditCards.Count(item => item.UserId == userId) +
               db.CreditCardCharges.Count(item => item.UserId == userId) +
               db.CreditCardPayments.Count(item => item.UserId == userId) +
               db.InstallmentPlans.Count(item => item.UserId == userId) +
               db.InstallmentItems.Count(item => item.UserId == userId) +
               db.RecurringTransactions.Count(item => item.UserId == userId) +
               db.RecurringTransactionOccurrences.Count(item => item.UserId == userId) +
               db.ImportBatches.Count(item => item.UserId == userId) +
               db.ImportRows.Count(item => item.UserId == userId) +
               db.DebtAgreements.Count(item => item.UserId == userId) +
               db.DebtInstallments.Count(item => item.UserId == userId) +
               db.SavingsGoals.Count(item => item.UserId == userId) +
               db.SavingsGoalContributions.Count(item => item.UserId == userId) +
               db.FinancialAttachments.Count(item => item.UserId == userId);
    }

    private sealed class PassThroughPreprocessor : IReceiptImagePreprocessor
    {
        public ReceiptImageNormalization Normalize(ReceiptImage image) => new(
            true, null, image, 1, 1, 1, 1, false, false, false);
    }

    private sealed class RecordingAnalyzer : IReceiptAnalyzer
    {
        private readonly ApplicationError? _error;

        public RecordingAnalyzer(ApplicationError? error = null)
        {
            _error = error;
        }

        public string? CategoryName { get; set; }

        public string PaymentHint { get; set; } = "card";

        public string DocumentType { get; set; } = "purchase_receipt";

        public string? PurchasedAt { get; set; }

        public string? FeeAmount { get; set; }

        public string? DueDate { get; set; }

        public string? InstallmentCount { get; set; }

        public string? Subtotal { get; set; } = "700,00";
        public string? Tax { get; set; } = "147,50";
        public string? TaxRate { get; set; }
        public string TotalAmount { get; set; } = "847,50";
        public List<ReceiptAnalysisRequest> Requests { get; } = [];

        public Task<ApplicationResult<RawReceiptReading>> AnalyzeAsync(
            ReceiptAnalysisRequest request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            if (_error is not null)
            {
                return Task.FromResult(
                    ApplicationResult<RawReceiptReading>.Failure(_error));
            }

            var reading = new RawReceiptReading(
                DocumentType switch
                {
                    "invoice_or_voucher" => ReceiptDocumentKind.InvoiceOrVoucher,
                    "bank_document" => ReceiptDocumentKind.BankDocument,
                    "bank_payment" => ReceiptDocumentKind.BankPayment,
                    "bank_card_payment" => ReceiptDocumentKind.CardPaymentSlip,
                    "refund_receipt" => ReceiptDocumentKind.RefundReceipt,
                    _ => ReceiptDocumentKind.PurchaseReceipt
                },
                "Sentetik Market",
                PurchasedAt,
                DueDate,
                Subtotal,
                Tax,
                TaxRate,
                TotalAmount,
                FeeAmount,
                InstallmentCount,
                "TRY",
                PaymentHint,
                CategoryName,
                [],
                new ReceiptAnalysisUsage(1200, 50, 1250, TimeSpan.FromMilliseconds(500)));
            return Task.FromResult(ApplicationResult<RawReceiptReading>.Success(reading));
        }
    }
}
