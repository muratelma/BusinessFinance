using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Infrastructure.Receipts;

namespace BusinessFinance.Infrastructure.Tests.Receipts;

public sealed class GeminiReceiptAnalyzerTests
{
    [Fact]
    public async Task Analyze_WithoutAnApiKey_ReportsTheFeatureOffWithoutCallingOut()
    {
        var handler = new RecordingHandler(_ => throw new InvalidOperationException(
            "A request must not leave the process when no key is configured."));
        var analyzer = Build(handler, new GeminiOptions { ApiKey = null });

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.disabled", result.Error.Code);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Analyze_CompletedReading_IsReturnedFieldByField()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {
              "counterpartyName": "Migros",
              "purchasedAt": "2026-08-18",
              "subtotalAmount": "705.92",
              "taxAmount": "141.58",
              "totalAmount": "847.50",
              "currencyCode": "TRY",
              "paymentMethodHint": "card",
              "categoryName": "Market",
              "unreadableFields": []
            }
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var reading = result.Value;
        Assert.Equal("Migros", reading.CounterpartyName);
        Assert.Equal("2026-08-18", reading.PurchasedAt);
        Assert.Equal("847.50", reading.TotalAmount);
        Assert.Equal("card", reading.PaymentMethodHint);
        Assert.Equal("Market", reading.CategoryName);
        Assert.Empty(reading.UnreadableFields);
    }

    /// <summary>
    /// The amount stays a string all the way through. A model that answers with a
    /// JSON number must not be quietly dropped either — it is carried as written
    /// so the validator, not the transport, decides whether to believe it.
    /// </summary>
    [Fact]
    public async Task Analyze_AmountReturnedAsANumber_IsCarriedAsWrittenNotDiscarded()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"counterpartyName": "A", "totalAmount": 847.50, "unreadableFields": []}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("847.50", result.Value.TotalAmount);
    }

    [Fact]
    public async Task Analyze_UnknownCategorySentinel_BecomesNoCategoryAtAll()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"categoryName": "__bilinmiyor__", "unreadableFields": ["totalAmount"]}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.CategoryName);
        Assert.Equal(["totalAmount"], result.Value.UnreadableFields);
    }

    [Fact]
    public async Task Analyze_FieldTheModelLeftBlank_ArrivesAsNullRatherThanEmptyText()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"counterpartyName": "", "totalAmount": "  ", "unreadableFields": []}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.CounterpartyName);
        Assert.Null(result.Value.TotalAmount);
    }

    [Fact]
    public async Task Analyze_TextSplitAcrossSteps_IsJoinedBeforeParsing()
    {
        var envelope = new JsonObject
        {
            ["status"] = "completed",
            ["steps"] = new JsonArray
            {
                new JsonObject
                {
                    ["type"] = "user_input",
                    ["content"] = new JsonArray
                    {
                        new JsonObject { ["type"] = "text", ["text"] = "yok sayılmalı" }
                    }
                },
                Step("""{"counterpartyName": "Mig"""),
                Step("""ros", "unreadableFields": []}""")
            }
        };
        var analyzer = Build(new RecordingHandler(_ => Ok(envelope.ToJsonString())));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Migros", result.Value.CounterpartyName);
    }

    [Theory]
    [InlineData("failed")]
    [InlineData("incomplete")]
    [InlineData("budget_exceeded")]
    [InlineData("requires_action")]
    public async Task Analyze_AnyStatusOtherThanCompleted_CountsAsUnread(string status)
    {
        var envelope = new JsonObject
        {
            ["status"] = status,
            ["steps"] = new JsonArray { Step("""{"counterpartyName": "Migros"}""") }
        };
        var analyzer = Build(new RecordingHandler(_ => Ok(envelope.ToJsonString())));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unreadable", result.Error.Code);
    }

    [Fact]
    public async Task Analyze_ModelAnsweredProseInsteadOfJson_CountsAsUnread()
    {
        var analyzer = Build(new RecordingHandler(
            _ => Ok(Envelope("Bu fotoğrafta bir fiş göremedim."))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unreadable", result.Error.Code);
    }

    [Fact]
    public async Task Analyze_ResponseThatIsNotJsonAtAll_CountsAsUnread()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok("<html>gateway</html>")));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unreadable", result.Error.Code);
    }

    /// <summary>
    /// A 429 is never retried. The free tier allows 15 requests a minute, and
    /// knocking again immediately spends the quota the next receipt needs.
    /// </summary>
    [Fact]
    public async Task Analyze_ProviderRateLimit_IsReportedOnceWithoutRetrying()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests));
        var analyzer = Build(handler);

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.provider_rate_limited", result.Error.Code);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Analyze_ServerErrorThenSuccess_IsRetriedExactlyOnce()
    {
        var attempts = 0;
        var handler = new RecordingHandler(_ =>
        {
            attempts++;
            return attempts == 1
                ? new HttpResponseMessage(HttpStatusCode.BadGateway)
                : Ok(Envelope("""{"counterpartyName": "Migros", "unreadableFields": []}"""));
        });
        var analyzer = Build(handler);

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task Analyze_ServerErrorTwice_GivesUpAsUnavailable()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.BadGateway));
        var analyzer = Build(handler);

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.provider_unavailable", result.Error.Code);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task Analyze_TimeoutOnBothAttempts_IsReportedAsUnavailable()
    {
        var handler = new RecordingHandler(_ => throw new TaskCanceledException("timed out"));
        var analyzer = Build(handler);

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.provider_unavailable", result.Error.Code);
    }

    [Fact]
    public async Task Analyze_SendsTheKeyInTheHeaderAndNeverInTheUrl()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler, new GeminiOptions { ApiKey = "gizli-anahtar" });

        await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        var request = handler.Requests[0];
        Assert.Equal("gizli-anahtar", request.Headers.GetValues("x-goog-api-key").Single());
        Assert.DoesNotContain("gizli-anahtar", request.RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Analyze_DisablesProviderSideInteractionStorage()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        var payload = JsonNode.Parse(handler.Bodies[0])!;
        Assert.False(payload["store"]!.GetValue<bool>());
    }

    /// <summary>
    /// The closed category set is the guard that stops the model inventing a
    /// bucket this budget does not have, so it is asserted on the wire, not
    /// assumed from the prompt.
    /// </summary>
    [Fact]
    public async Task Analyze_ConstrainsTheCategoryToTheUsersOwnListPlusUnknown()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(
            new ReceiptAnalysisRequest(
                Image,
                ["Market", "Ulaşım", "market"],
                ReceiptCaptureIntent.Expense),
            CancellationToken.None);

        var schema = JsonNode.Parse(handler.Bodies[0])!["response_format"]!["schema"]!;
        var allowed = schema["properties"]!["categoryName"]!["enum"]!.AsArray()
            .Select(node => node!.GetValue<string>())
            .ToArray();

        Assert.Equal(["Market", "Ulaşım", "__bilinmiyor__"], allowed);
    }

    [Fact]
    public async Task Analyze_SendsTheImageInlineWithItsOwnMediaType()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        var input = JsonNode.Parse(handler.Bodies[0])!["input"]!.AsArray();
        var image = input.Single(node => node!["type"]!.GetValue<string>() == "image")!;
        Assert.Equal("image/jpeg", image["mime_type"]!.GetValue<string>());
        Assert.Equal(Convert.ToBase64String("fiş"u8.ToArray()), image["data"]!.GetValue<string>());
    }

    /// <summary>
    /// The direction reaches the model as a told fact, not as a question. It is
    /// needed only so the model knows which printed party is the counterparty on
    /// a document that names both.
    /// </summary>
    [Theory]
    [InlineData(ReceiptCaptureIntent.Expense, "ÖDEYEN taraftır")]
    [InlineData(ReceiptCaptureIntent.Income, "ALAN taraftır")]
    public async Task Analyze_TellsTheModelWhichSideTheUserIsOn(
        ReceiptCaptureIntent intent,
        string expected)
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(Request(intent), CancellationToken.None);

        var input = JsonNode.Parse(handler.Bodies[0])!["input"]!.AsArray();
        var prompt = input.Single(node => node!["type"]!.GetValue<string>() == "text")!;
        Assert.Contains(expected, prompt["text"]!.GetValue<string>(), StringComparison.Ordinal);
    }

    /// <summary>
    /// An invoice names both parties and is therefore readable in either
    /// direction; the schema must offer it or the model cannot say so.
    /// </summary>
    [Fact]
    public async Task Analyze_DocumentTypeSchema_OffersTheTwoPartyDocumentKind()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""{"unreadableFields": []}""")));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        var schema = JsonNode.Parse(handler.Bodies[0])!["response_format"]!["schema"]!;
        var kinds = schema["properties"]!["documentType"]!["enum"]!.AsArray()
            .Select(node => node!.GetValue<string>())
            .ToArray();
        Assert.Contains("invoice_or_voucher", kinds);
        Assert.Contains("purchase_receipt", kinds);
    }

    [Fact]
    public async Task Analyze_ReportsTokenCostSoModelsCanBeComparedLater()
    {
        var envelope = new JsonObject
        {
            ["status"] = "completed",
            ["steps"] = new JsonArray { Step("""{"unreadableFields": []}""") },
            ["usage"] = new JsonObject
            {
                ["total_input_tokens"] = 1290,
                ["total_output_tokens"] = 64,
                ["total_tokens"] = 1354
            }
        };
        var analyzer = Build(new RecordingHandler(_ => Ok(envelope.ToJsonString())));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.Equal(1290, result.Value.Usage.InputTokens);
        Assert.Equal(64, result.Value.Usage.OutputTokens);
        Assert.Equal(1354, result.Value.Usage.TotalTokens);
    }

    private static readonly ReceiptImage Image = new("fiş"u8.ToArray(), "image/jpeg");

    private static ReceiptAnalysisRequest Request(
        ReceiptCaptureIntent intent = ReceiptCaptureIntent.Expense) =>
        new(Image, ["Market"], intent);

    private static GeminiReceiptAnalyzer Build(
        RecordingHandler handler,
        GeminiOptions? options = null)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/")
        };
        return new GeminiReceiptAnalyzer(
            client,
            Options.Create(options ?? new GeminiOptions { ApiKey = "test-anahtari" }),
            TimeProvider.System);
    }

    private static HttpResponseMessage Ok(string body) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json")
    };

    [Fact]
    public async Task Analyze_DocumentTypeIsCarriedThrough()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"documentType": "bank_document", "counterpartyName": "", "unreadableFields": []}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReceiptDocumentKind.BankDocument, result.Value.DocumentKind);
    }

    [Fact]
    public async Task Analyze_InvoiceOrVoucherKind_IsCarriedThrough()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"documentType": "invoice_or_voucher", "counterpartyName": "A", "unreadableFields": []}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReceiptDocumentKind.InvoiceOrVoucher, result.Value.DocumentKind);
    }

    /// <summary>
    /// A missing or renamed value must not land on "purchase receipt": that would
    /// let a provider change silently reopen the hole where a transfer slip is
    /// read as an expense.
    /// </summary>
    [Theory]
    [InlineData("""{"counterpartyName": "A", "unreadableFields": []}""")]
    [InlineData("""{"documentType": "receipt", "counterpartyName": "A", "unreadableFields": []}""")]
    public async Task Analyze_MissingOrUnknownDocumentType_FallsToTheRefusingSide(
        string output)
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope(output))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReceiptDocumentKind.Unknown, result.Value.DocumentKind);
    }

    [Fact]
    public async Task Analyze_RefundSlipIsItsOwnKind()
    {
        var analyzer = Build(new RecordingHandler(_ => Ok(Envelope("""
            {"documentType": "refund_receipt", "counterpartyName": "", "unreadableFields": []}
            """))));

        var result = await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReceiptDocumentKind.RefundReceipt, result.Value.DocumentKind);
    }

    [Fact]
    public async Task Analyze_AsksTheModelToClassifyBeforeReading()
    {
        var handler = new RecordingHandler(_ => Ok(Envelope("""
            {"documentType": "purchase_receipt", "counterpartyName": "A", "unreadableFields": []}
            """)));
        var analyzer = Build(handler);

        await analyzer.AnalyzeAsync(Request(), CancellationToken.None);

        var body = handler.Bodies.Single();
        // Prompt artık "sana bir fiş veriliyor" diye iddia etmiyor; sınıflandırma
        // ve toplama yasağı sözleşmenin parçası.
        Assert.Contains("bank_document", body, StringComparison.Ordinal);
        Assert.Contains("refund_receipt", body, StringComparison.Ordinal);
        Assert.Contains("ASLA toplama", body, StringComparison.Ordinal);
        // Yakın kategoriye yerleştirme yasağı: döner satın alması market değildir.
        // Gövde JSON olduğu için Türkçe harfler kaçışlıdır; ASCII bir parça
        // aranıyor.
        Assert.Contains("restoran, kafe", body, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Sana bir alışveriş fişinin fotoğrafı veriliyor",
            body,
            StringComparison.Ordinal);
    }

    private static string Envelope(string outputText) => new JsonObject
    {
        ["status"] = "completed",
        ["steps"] = new JsonArray { Step(outputText) }
    }.ToJsonString();

    private static JsonObject Step(string text) => new()
    {
        ["type"] = "model_output",
        ["content"] = new JsonArray
        {
            new JsonObject { ["type"] = "text", ["text"] = text }
        }
    };

    private sealed class RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];
        public List<string> Bodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            Bodies.Add(request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken));
            return respond(request);
        }
    }
}
