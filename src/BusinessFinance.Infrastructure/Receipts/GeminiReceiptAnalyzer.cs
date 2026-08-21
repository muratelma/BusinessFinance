using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Receipts;

namespace BusinessFinance.Infrastructure.Receipts;

public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    /// <summary>Local user-secrets only. Never appsettings, logs or Git.</summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Free tier quotas differ by 25x between the Lite line (RPD 500) and the
    /// full Flash line (RPD 20), so the default is the one that survives a day of
    /// development. Confirmed by measurement on 18 August 2026: Lite read 264/264
    /// fields across the whole synthetic set, and 3.7 Flash showed no accuracy
    /// advantage over the six pairs that fit in its daily quota while costing 2.8x
    /// the latency. See documentation/receipt-measurement.md.
    /// </summary>
    public string Model { get; init; } = "gemini-3.5-flash-lite";

    public string BaseUrl { get; init; } = "https://generativelanguage.googleapis.com/";

    /// <summary>
    /// Set from measured latency, not from a round number. The 18 August 2026 run
    /// saw Flash Lite average 10 s with a 47 s outlier, and 3.7 Flash average
    /// 28.5 s with a 102 s outlier; the measurement tool itself needs 75 s. A 30 s
    /// budget would have cut off calls that were about to succeed and shown the
    /// user "servise ulaşılamıyor" for a receipt the model had already read.
    /// The chosen model is Flash Lite, whose slowest measured call was 46.9 s, so
    /// 60 s stands.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 60;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}

/// <summary>
/// Talks to the Gemini Interactions API over raw <see cref="HttpClient"/>.
///
/// No SDK on purpose: one endpoint is called, the official .NET SDK is young and
/// the alternatives are third party. The cost of that choice is that a contract
/// change shows up as a failing integration test rather than a compile error,
/// which is why the shape is parsed defensively and the skippable live test
/// exists.
/// </summary>
internal sealed class GeminiReceiptAnalyzer : IReceiptAnalyzer
{
    private const string ApiKeyHeader = "x-goog-api-key";
    private const string InteractionsPath = "v1beta/interactions";

    /// <summary>
    /// Chosen so a category the user does not have is impossible to express: the
    /// model picks from a closed set, and "unknown" is a member of that set
    /// rather than free text it can improvise.
    /// </summary>
    internal const string UnknownCategorySentinel = "__bilinmiyor__";

    private readonly HttpClient _client;
    private readonly GeminiOptions _options;
    private readonly TimeProvider _timeProvider;

    public GeminiReceiptAnalyzer(
        HttpClient client,
        IOptions<GeminiOptions> options,
        TimeProvider timeProvider)
    {
        _client = client;
        _options = options.Value;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<RawReceiptReading>> AnalyzeAsync(
        ReceiptAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (!_options.IsConfigured)
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Disabled);

        var payload = BuildRequest(request);
        var started = _timeProvider.GetTimestamp();

        var (response, transportFailed) = await SendAsync(payload, cancellationToken);
        using (response)
        {
            if (transportFailed)
            {
                return ApplicationResult<RawReceiptReading>.Failure(
                    ReceiptAnalysisErrors.ProviderUnavailable);
            }

            if (response!.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return ApplicationResult<RawReceiptReading>.Failure(
                    ReceiptAnalysisErrors.ProviderRateLimited);
            }

            if (!response.IsSuccessStatusCode)
            {
                // The body can carry the caller's own receipt back in an error
                // message, so it is never read into a log or an exception here.
                return ApplicationResult<RawReceiptReading>.Failure(
                    ReceiptAnalysisErrors.ProviderUnavailable);
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var latency = _timeProvider.GetElapsedTime(started);
            return Interpret(body, latency);
        }
    }

    private async Task<(HttpResponseMessage? Response, bool TransportFailed)> SendAsync(
        string payload,
        CancellationToken cancellationToken)
    {
        // One retry, and only for a transport fault or a server-side 5xx. A 429 is
        // never retried: the free tier allows 15 requests a minute, and retrying
        // into a closed door spends the quota that the next receipt needs.
        for (var attempt = 0; ; attempt++)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, InteractionsPath)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
            message.Headers.TryAddWithoutValidation(ApiKeyHeader, _options.ApiKey);

            try
            {
                var response = await _client.SendAsync(message, cancellationToken);
                if (attempt == 0 && (int)response.StatusCode >= 500)
                {
                    response.Dispose();
                    continue;
                }

                return (response, false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (
                exception is HttpRequestException or TaskCanceledException or TimeoutException)
            {
                if (attempt == 0)
                    continue;

                return (null, true);
            }
        }
    }

    private static ApplicationResult<RawReceiptReading> Interpret(string body, TimeSpan latency)
    {
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(body);
        }
        catch (JsonException)
        {
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);
        }

        if (root is not JsonObject envelope)
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);

        // "completed" is the only status that carries a usable answer. A refusal,
        // a safety block and a truncated answer all arrive as some other status,
        // and each of them means "we did not read this receipt".
        var status = envelope["status"]?.GetValue<string>();
        if (!string.Equals(status, "completed", StringComparison.Ordinal))
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);

        var text = ExtractOutputText(envelope);
        if (string.IsNullOrWhiteSpace(text))
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);

        JsonNode? reading;
        try
        {
            reading = JsonNode.Parse(text);
        }
        catch (JsonException)
        {
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);
        }

        if (reading is not JsonObject fields)
            return ApplicationResult<RawReceiptReading>.Failure(ReceiptAnalysisErrors.Unreadable);

        return ApplicationResult<RawReceiptReading>.Success(new RawReceiptReading(
            DocumentKind(fields),
            Text(fields, "counterpartyName"),
            Text(fields, "purchasedAt"),
            Text(fields, "dueDate"),
            Text(fields, "subtotalAmount"),
            Text(fields, "taxAmount"),
            Text(fields, "totalAmount"),
            Text(fields, "feeAmount"),
            Text(fields, "installmentCount"),
            Text(fields, "currencyCode"),
            Text(fields, "paymentMethodHint"),
            Category(fields),
            Strings(fields, "unreadableFields"),
            ReadUsage(envelope, latency)));
    }

    /// <summary>
    /// Prefers the flattened convenience field and falls back to walking the
    /// steps. Both are documented; reading only one of them would make the client
    /// break on a response that is perfectly valid.
    /// </summary>
    private static string? ExtractOutputText(JsonObject envelope)
    {
        if (envelope["output_text"] is JsonValue direct &&
            direct.TryGetValue<string>(out var flattened) &&
            !string.IsNullOrWhiteSpace(flattened))
        {
            return flattened;
        }

        if (envelope["steps"] is not JsonArray steps)
            return null;

        var builder = new StringBuilder();
        foreach (var step in steps)
        {
            if (step is not JsonObject item ||
                item["type"]?.GetValue<string>() != "model_output" ||
                item["content"] is not JsonArray content)
            {
                continue;
            }

            foreach (var part in content)
            {
                if (part is JsonObject piece &&
                    piece["type"]?.GetValue<string>() == "text" &&
                    piece["text"] is JsonValue value &&
                    value.TryGetValue<string>(out var chunk))
                {
                    builder.Append(chunk);
                }
            }
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    private static ReceiptAnalysisUsage ReadUsage(JsonObject envelope, TimeSpan latency)
    {
        var usage = envelope["usage"] as JsonObject;
        return new ReceiptAnalysisUsage(
            Number(usage, "total_input_tokens"),
            Number(usage, "total_output_tokens"),
            Number(usage, "total_tokens"),
            latency);
    }

    private static int Number(JsonObject? source, string name) =>
        source?[name] is JsonValue value && value.TryGetValue<int>(out var number) ? number : 0;

    private static string? Text(JsonObject source, string name)
    {
        if (source[name] is not JsonValue value)
            return null;

        // The schema asks for strings, but a model that answers 847.50 as a number
        // must not silently become an empty field: it is read as written and left
        // for the validator to reject.
        var raw = value.TryGetValue<string>(out var text)
            ? text
            : value.ToJsonString().Trim('"');

        return string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
    }

    /// <summary>
    /// An unrecognised value maps to <see cref="ReceiptDocumentKind.Unknown"/>,
    /// which the use case refuses. Defaulting to "purchase receipt" would make a
    /// provider change silently reopen exactly the hole this closes.
    /// </summary>
    private static ReceiptDocumentKind DocumentKind(JsonObject source) =>
        Text(source, "documentType") switch
        {
            "purchase_receipt" => ReceiptDocumentKind.PurchaseReceipt,
            "invoice_or_voucher" => ReceiptDocumentKind.InvoiceOrVoucher,
            "refund_receipt" => ReceiptDocumentKind.RefundReceipt,
            "bank_document" => ReceiptDocumentKind.BankDocument,
            "bank_payment" => ReceiptDocumentKind.BankPayment,
            "bank_card_payment" => ReceiptDocumentKind.CardPaymentSlip,
            "other_document" => ReceiptDocumentKind.OtherDocument,
            "not_a_document" => ReceiptDocumentKind.NotADocument,
            _ => ReceiptDocumentKind.Unknown
        };

    private static string? Category(JsonObject source)
    {
        var value = Text(source, "categoryName");
        return string.Equals(value, UnknownCategorySentinel, StringComparison.Ordinal)
            ? null
            : value;
    }

    private static IReadOnlyList<string> Strings(JsonObject source, string name)
    {
        if (source[name] is not JsonArray array)
            return [];

        return array
            .OfType<JsonValue>()
            .Select(item => item.TryGetValue<string>(out var text) ? text : null)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item!.Trim())
            .ToArray();
    }

    private string BuildRequest(ReceiptAnalysisRequest request)
    {
        var categories = request.CategoryNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Append(UnknownCategorySentinel)
            .ToArray();

        var payload = new JsonObject
        {
            ["model"] = _options.Model,
            // Interactions are stored by the provider unless this is explicit.
            // Receipt photos and extracted financial fields are single-turn input;
            // we never need server-side history and must not retain it by default.
            ["store"] = false,
            ["input"] = new JsonArray
            {
                new JsonObject { ["type"] = "text", ["text"] = PromptFor(request.Intent) },
                new JsonObject
                {
                    ["type"] = "image",
                    ["mime_type"] = request.Image.ContentType,
                    ["data"] = Convert.ToBase64String(request.Image.Content.Span)
                }
            },
            ["response_format"] = new JsonObject
            {
                ["type"] = "text",
                ["mime_type"] = "application/json",
                ["schema"] = Schema(categories)
            }
        };

        return payload.ToJsonString();
    }

    private static JsonObject Schema(IReadOnlyList<string> categories) => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["documentType"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = new JsonArray
                {
                    "purchase_receipt",
                    "invoice_or_voucher",
                    "refund_receipt",
                    "bank_document",
                    "bank_payment",
                    "bank_card_payment",
                    "other_document",
                    "not_a_document"
                }
            },
            ["counterpartyName"] = StringField(),
            ["purchasedAt"] = StringField(),
            ["dueDate"] = StringField(),
            ["subtotalAmount"] = StringField(),
            ["taxAmount"] = StringField(),
            ["totalAmount"] = StringField(),
            ["feeAmount"] = StringField(),
            ["installmentCount"] = StringField(),
            ["currencyCode"] = StringField(),
            ["paymentMethodHint"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = new JsonArray
                {
                    "cash", "credit_card", "debit_card", "card", "unknown"
                }
            },
            ["categoryName"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = new JsonArray(categories.Select(name => (JsonNode)name!).ToArray())
            },
            ["unreadableFields"] = new JsonObject
            {
                ["type"] = "array",
                ["items"] = StringField()
            }
        },
        ["required"] = new JsonArray
        {
            "documentType", "counterpartyName", "purchasedAt", "totalAmount",
            "currencyCode", "paymentMethodHint", "categoryName",
            "unreadableFields"
        }
    };

    private static JsonObject StringField() => new() { ["type"] = "string" };

    /// <summary>
    /// Turkish because the receipts are: the labels the model is told to look for
    /// are the ones actually printed on them.
    ///
    /// Three rules carry most of the weight. The first is the classification: the
    /// prompt no longer asserts that the photo is a receipt, because a prompt that
    /// asserts it leaves the model no way to disagree — handed a bank transfer
    /// slip it complied and mapped the slip onto receipt fields. The second is the
    /// ban on arithmetic: a total the model computed looks exactly like a total it
    /// read, and 5.000 + 4,50 became a 5.004,50 expense that was never spent.
    ///
    /// The third is that the category lives in its own section. Every other field
    /// is transcribed and the transcription rules say never to infer; a category
    /// is never printed on a receipt, so asking for it inside that block asked the
    /// model to obey two contradicting rules at once. Handed a receipt whose brand
    /// name said nothing ("maydonoz", with İskender and Pepsi on the item lines)
    /// it resolved the contradiction the safe way and returned "unknown" six times
    /// out of six. The section states which evidence to weigh and in what order.
    /// </summary>
    private const string BasePrompt = """
        Sana bir fotoğraf veriliyor. ÖNCE bunun ne olduğunu belirle, SONRA
        istenen JSON şemasına göre döndür.

        1) documentType alanını doldur:
        - "purchase_receipt": tezgâh üstü bir satın alma — market/mağaza fişi,
          yazar kasa fişi, restoran adisyonu. Bu fişi elinde tutan taraf daima
          ALICI'dır.
        - "invoice_or_voucher": ödeyen ve ödenen tarafın **adıyla yazılı** olduğu
          fatura, makbuz veya bordro — e-arşiv/e-fatura, kira makbuzu, serbest
          meslek makbuzu, maaş/ücret bordrosu, abonelik faturası. Bu belgede
          kullanıcının hangi taraf olduğunu SEN belirlemezsin; sana aşağıda
          söylenir.
        - "refund_receipt": para geri verilmiş — üzerinde "İADE", "IADE",
          "İPTAL", "RETURN" yazan fiş veya POS slipi. Satın alma değil.
        - "bank_payment": banka üzerinden BAŞKASINA yapılmış ödeme. Şu iki
          işaretten biri yeterlidir:
          (a) belgede gönderen/hesap sahibi ile alacaklı/alıcı adı FARKLI
              kişiler — "HESAPTAN HESABA HAVALE" veya "HESAPTAN EFT" başlığı
              taşısa bile fark ediyorsa bu bir ödemedir;
          (b) bir kuruma fatura ödemesi — "FATURA ÖDEMELERİ", "ABONE NO",
              "FATURA NO", "KURUM ADI" gibi ifadeler geçer.
        - "bank_card_payment": kredi kartı borcunun ödendiği dekont — "KREDİ
          KARTI BORÇ ÖDEMESİ", "KART BORCU", "EKSTRE BORCU" gibi ifadeler ya da
          ödenen tarafta bir kart numarası geçer. "bank_payment"tan önce bunu
          kontrol et.
        - "bank_document": para KULLANICININ KENDİ hesapları arasında kalmış —
          ATM nakit çekimi, virman, ya da gönderen ile alıcının AYNI kişi olduğu
          havale. Hesap ekstresi de buraya girer.
          Bu iki türü ayırırken YALNIZ belgede yazan adlara bak; kullanıcının kim
          olduğunu bilmiyorsun. İki ad farklıysa daima "bank_payment" seç.
        - "other_document": belge ama yukarıdakilerden değil (ör. reçete, bilet,
          sözleşme, ekran görüntüsü).
        - "not_a_document": fotoğrafta okunabilir bir belge yok.

        2) documentType "other_document" veya "not_a_document" ise diğer bütün
        alanları boş metin ("") olarak döndür: belge olmayan bir fotoğrafı
        belgeymiş gibi okuma. Bir belgede hem satış hem iade ifadesi varsa
        "refund_receipt" seç.

        3) Diğer bütün türlerde belgedeki bilgileri oku:
        - Yalnızca fişte YAZAN bilgiyi döndür. Okuyamadığın veya fişte olmayan bir
          alan için boş metin ("") döndür ve alan adını unreadableFields listesine
          ekle. ASLA tahmin etme, ASLA uydurma.
        - Tutarları ASLA toplama, çıkarma veya hesaplama. Hiçbir alanı iki sayıyı
          birleştirerek üretme.
        - totalAmount: fişte TEK BİR SATIRDA BASILI olan genel toplam (genellikle
          "TOPLAM" veya "GENEL TOPLAM"). Böyle bir satır yoksa boş bırak.
          subtotalAmount: "ARA TOPLAM". taxAmount: toplam KDV tutarı.
        - Tutarları fişte yazdığı değerle, ondalık ayırıcı olarak nokta kullanarak
          döndür. Binlik ayırıcı ve para birimi simgesi koyma. Örnek: "1234.56"
        - purchasedAt: fişin tarihi, yyyy-MM-dd biçiminde. Fişte gg.aa.yyyy
          yazıyorsa dönüştür.
        - dueDate: yalnız faturalarda, "SON ÖDEME TARİHİ" / "ÖDEME TARİHİ" /
          "VADE" yazan tarih; yyyy-MM-dd. Bu, belgenin kendi tarihi DEĞİLDİR ve
          onun yerine ASLA yazılmaz — ikisi farklı satırlardır. Böyle bir satır
          yoksa boş bırak. Yazar kasa fişinde bu alan daima boştur.
        - installmentCount: fişte taksit yazıyorsa taksit SAYISI, yalnız rakam
          ("3 TAKSİT" → "3"; "6 TAKSITLI" → "6"). Tek çekimse ya da taksit
          yazmıyorsa boş bırak. Taksit tutarını buraya YAZMA ve totalAmount'ı
          taksit sayısına BÖLME — bölme işlemini uygulama yapar.
        - currencyCode: ISO kodu. Türk lirası için "TRY".
        - paymentMethodHint: fişte YAZAN ödeme biçimi.
          "NAKİT" / "NAKIT" yazıyorsa "cash".
          "KREDİ KARTI" / "KREDI KARTI" / "CREDIT" yazıyorsa "credit_card".
          "BANKA KARTI" / "DEBİT" / "DEBIT" / "MAESTRO" yazıyorsa "debit_card".
          Yalnızca "KART" veya "POS" yazıyor, türü yazmıyorsa "card" — kart
          türünü ASLA tahmin etme, marka adından (Visa, Mastercard, Troy)
          çıkarma; bu markalar hem kredi hem banka kartında bulunur.
          Ödeme biçimi hiç yazmıyorsa "unknown".
        - counterpartyName: işlemin **karşı tarafı** — bu belgede parayı alan
          işletme. İşletmenin **markasını** yaz; fişin en üstündeki ad genellikle
          budur. Ticari ünvan ayrı satırdaysa (… LTD ŞTİ, … A.Ş., … SAN. TİC.)
          markayı tercih et. Şube veya adres bilgisini ekleme.
        - categoryName: 4. maddeye bak.
        - Ürün kalemlerini çıktıya YAZMA; yalnızca yukarıdaki alanları döndür.
          Kalemleri okumak serbesttir ve kategori seçiminde kullanılır.

        4) categoryName — bu, fişten ÇIKARIM yapılan tek alandır.
        Diğer alanlar fişte yazanı aktarır; kategori fişte yazmaz, fişten
        çıkarılır. Burada çıkarım yapmak serbesttir, ama yalnız fişteki kanıta
        dayanmak zorundadır.

        Kanıtlar, güçlüden zayıfa:
        a) Satılan ürün/hizmet kalemleri — ne alındığını en iyi bunlar söyler.
        b) İşletmenin adı ve fişteki iş türü ibareleri (ECZANE, AKARYAKIT,
           MARKET, RESTAURANT gibi).
        İşletme adı yalnız bir markaysa ve ne iş yaptığını söylemiyorsa
        kalemlere bak; kalemler okunmuyorsa işletme adına bak.

        Aynı ürün, nerede satıldığına göre farklı kovaya girer: markette satılan
        gıda bir market alışverişidir; restoran, kafe, dönerci gibi bir yerde
        hazır servis edilen yiyecek-içecek ise yeme-içmedir. Listede bu ayrımı
        karşılayan kova hangisiyse onu seç.

        Kategoriyi ödeme yöntemi, tutarın büyüklüğü veya tarih belirlemez.

        Listeden yalnız AÇIKÇA uyan birini seç. Yakın veya kısmen benzeyen
        kovayı SEÇME: kısmen uyan kategori, harcamayı yanlış kovaya yazar ve
        bunu kimse fark etmez. Uyan kova yoksa veya iki kova arasında
        kararsızsan "__bilinmiyor__" döndür. Liste dışında değer üretme.
        """;

    /// <summary>
    /// The direction is given to the model, never asked of it.
    ///
    /// <para>
    /// It is needed for exactly one reason: on a document that names both parties
    /// — a rent voucher, a payslip — "the counterparty" is whichever one is not
    /// the user, and the paper does not say which that is. Telling the model who
    /// the user is on this document lets it pick the right name. It does not let
    /// it decide the direction; that arrived already decided.
    /// </para>
    /// </summary>
    private static string PromptFor(ReceiptCaptureIntent intent) =>
        BasePrompt + intent switch
        {
            ReceiptCaptureIntent.Transfer => """


                5) YÖN: bu bir TRANSFER belgesidir (dekont, havale/EFT makbuzu,
                ATM çekim fişi). Para harcanmadı, hesaplar arasında taşındı.
                totalAmount: TAŞINAN tutar (gönderilen veya çekilen).
                feeAmount: varsa işlem ücreti/masraf/komisyon — ayrı bir satırda
                basılıdır. İkisini ASLA toplama, masrafı taşınan tutara ekleme.
                Masraf yoksa feeAmount'ı boş bırak.
                Belge masrafı İKİ satırda basmışsa ("MASRAF TUTARI" ve "MASRAF
                TOPLAMI", ya da "ÜCRET" ve "ÜCRET + BSMV") daima TOPLAM olanı
                seç: bankanın hesaptan çektiği tutar odur, vergisi dahil. Sen
                toplama YAPMA, yalnız basılı olan toplam satırını oku.
                counterpartyName: dekontta yazan karşı taraf (alıcı, gönderen
                veya banka adı); yoksa boş bırak.
                categoryName daima "__bilinmiyor__" olsun: transferin kategorisi
                yoktur.
                """,

            ReceiptCaptureIntent.BankSlip => """


                5) YÖN: kullanıcı bunun bir BANKA DEKONTU olduğunu bildirdi;
                yönü BİLDİRMEDİ ve senden de istenmiyor. Paranın harcama mı,
                kendi hesabına aktarma mı, kart borcu ödemesi mi, yoksa borç
                verme mi olduğunu kullanıcı okuma bittikten sonra seçecek.
                Sen yalnız belgede YAZANI oku.
                documentType'ı belgeye bakarak seç: "bank_document",
                "bank_payment" veya "bank_card_payment".
                totalAmount: dekonttaki ASIL tutar (gönderilen, çekilen veya
                ödenen). feeAmount: işlem ücreti/masraf/komisyon; ayrı satırda
                basılıdır. İkisini ASLA toplama.
                Masraf İKİ satırda basılmışsa ("MASRAF TUTARI" ve "MASRAF
                TOPLAMI", ya da "ÜCRET" ve "ÜCRET + BSMV") feeAmount daima
                TOPLAM olanıdır: bankanın hesaptan çektiği tutar odur, vergisi
                dahil. Sen toplama YAPMA, yalnız basılı olan toplam satırını
                oku.
                counterpartyName: paranın GİTTİĞİ taraf — alacaklı, alıcı veya
                kurum adı. Bankanın kendi adı karşı taraf değildir.
                """,

            ReceiptCaptureIntent.Income => """


                5) YÖN: kullanıcı bu belgede parayı ALAN taraftır (gelir).
                counterpartyName, parayı ÖDEYEN taraftır — kiracı, işveren veya
                müşteri. Belgede iki taraf yazılıysa ödeyeni seç.
                Bu bilgiyi kullanıcı bildirdi; sorgulama ve documentType'ı buna
                göre değiştirme. Belge tezgâh üstü bir satın alma fişiyse yine
                "purchase_receipt" de — çelişkiyi uygulama ele alır.
                """,

            _ => """


                5) YÖN: kullanıcı bu belgede ÖDEYEN taraftır (gider).
                counterpartyName, parayı ALAN taraftır — işletme, kurum veya kişi.
                Belge "bank_payment" ise (banka üzerinden yapılmış ödeme):
                totalAmount ödenen asıl tutardır (fatura tutarı veya havale
                tutarı), feeAmount ise işlem ücreti/masraf/aracılık ücretidir.
                İkisini ASLA toplama — belgede "ödenen toplam" gibi birleşik bir
                satır bassa bile totalAmount asıl tutar olarak kalsın.
                Masraf İKİ satırda basılmışsa ("MASRAF TUTARI" ve "MASRAF
                TOPLAMI", ya da "ÜCRET" ve "ÜCRET + BSMV") feeAmount daima
                TOPLAM olanıdır: bankanın hesaptan çektiği tutar odur, vergisi
                dahil. Sen toplama YAPMA, yalnız basılı olan toplam satırını oku.
                """
        };
}
