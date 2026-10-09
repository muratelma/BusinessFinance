using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Receipts;

/// <summary>
/// Reads a receipt into an editable suggestion. This use case is deliberately
/// read-only: it does not receive a transaction repository, an attachment
/// store, or a unit of work, so analysis cannot become a financial write by
/// accident.
/// </summary>
public sealed class AnalyzeReceiptUseCase(
    ICurrentUser currentUser,
    IAttachmentFileInspector fileInspector,
    IReceiptImagePreprocessor preprocessor,
    ICategoryRepository categoryRepository,
    ICounterpartyRepository counterpartyRepository,
    IReceiptAnalyzer analyzer,
    IReceiptDuplicateLookup duplicateLookup,
    IReceiptRefundLookup refundLookup,
    TimeProvider timeProvider)
{
    public const long MaximumFileSizeBytes = FinancialAttachment.MaximumSizeBytes;

    public async Task<ApplicationResult<AnalyzedReceipt>> ExecuteAsync(
        AnalyzeReceiptCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<AnalyzedReceipt>.Failure(
                ReceiptAnalysisErrors.AuthenticationRequired);
        }

        if (command.DeclaredLength is < 1 or > MaximumFileSizeBytes)
        {
            return ApplicationResult<AnalyzedReceipt>.Failure(
                command.DeclaredLength > MaximumFileSizeBytes
                    ? ReceiptAnalysisErrors.FileTooLarge
                    : ReceiptAnalysisErrors.UnsupportedFile("Boş olmayan bir fiş fotoğrafı seçin."));
        }

        var contentResult = await ReadContentAsync(command, cancellationToken);
        if (!contentResult.IsSuccess)
            return ApplicationResult<AnalyzedReceipt>.Failure(contentResult.Error);

        var content = contentResult.Value;
        var inspection = fileInspector.Inspect(command.FileName, command.ContentType, content);
        if (!inspection.IsAccepted)
        {
            return ApplicationResult<AnalyzedReceipt>.Failure(
                ReceiptAnalysisErrors.UnsupportedFile(
                    inspection.RejectionReason ?? "Fiş fotoğrafı güvenlik denetiminden geçemedi."));
        }

        if (inspection.NormalizedContentType is not ("image/jpeg" or "image/png"))
        {
            return ApplicationResult<AnalyzedReceipt>.Failure(
                ReceiptAnalysisErrors.UnsupportedFile("Fiş yalnız JPEG veya PNG fotoğrafı olabilir."));
        }

        var normalization = preprocessor.Normalize(
            new ReceiptImage(content, inspection.NormalizedContentType));
        if (!normalization.IsAccepted || normalization.Image is null)
        {
            return ApplicationResult<AnalyzedReceipt>.Failure(ReceiptAnalysisErrors.Unreadable);
        }

        // The bucket list follows the direction the user declared. Offering
        // expense buckets for an income document would make the model choose a
        // category the app would never post the record to.
        var categories = await categoryRepository.ListAsync(
            userId,
            command.Intent switch
            {
                ReceiptCaptureIntent.Income => CategoryType.Income,
                _ => CategoryType.Expense
            },
            true,
            cancellationToken);
        // Transferin kategorisi yoktur: para harcanmadı, yer değiştirdi. Modele
        // kova sunmak, uygulamanın asla yazmayacağı bir seçim uydurturdu.
        ReceiptCategoryOption[] categoryOptions =
            command.Intent is ReceiptCaptureIntent.Transfer
                ? []
                : [.. categories.Select(category =>
                    new ReceiptCategoryOption(category.Id, category.Name))];

        var analysis = await analyzer.AnalyzeAsync(
            new ReceiptAnalysisRequest(
                normalization.Image,
                categoryOptions.Select(category => category.Name).ToArray(),
                command.Intent),
            cancellationToken);
        if (!analysis.IsSuccess)
            return ApplicationResult<AnalyzedReceipt>.Failure(analysis.Error);

        // The kind is checked before the fields are believed. Reading a bank slip
        // as if it were a receipt is not a smaller error than failing to read it:
        // it produces a confident, wrong expense.
        var kindRefusal = RefuseUnsupportedKind(analysis.Value.DocumentKind, command.Intent);
        if (kindRefusal is not null)
            return ApplicationResult<AnalyzedReceipt>.Failure(kindRefusal);

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var draft = ReceiptDraftValidator.Validate(analysis.Value, categoryOptions, today);

        // Nothing readable is a failed reading, not a half-read receipt. Returning
        // an empty draft with one warning per field told the user five times that
        // the same thing went wrong.
        if (IsEmpty(draft))
            return ApplicationResult<AnalyzedReceipt>.Failure(ReceiptAnalysisErrors.Unreadable);

        draft = await WarnIfAlreadyRecordedAsync(draft, userId, command.Intent, cancellationToken);
        draft = await AttachRefundMatchAsync(draft, userId, cancellationToken);
        draft = await AttachCounterpartyMatchAsync(draft, userId, cancellationToken);

        return ApplicationResult<AnalyzedReceipt>.Success(
            new AnalyzedReceipt(draft, normalization, analysis.Value.Usage));
    }

    /// <summary>
    /// Okunan adı kullanıcının kendi karşı taraflarında arar ve bulduğunu
    /// taslağa <b>öneri olarak</b> iliştirir.
    /// </summary>
    /// <remarks>
    /// Hiçbir şey yazılmaz ve hiçbir karşı taraf kurulmaz (ADR 0011): model
    /// karşı tarafı seçmez, uygulama yalnız "bu adı zaten tanıyorum" der.
    /// Kullanıcı öneriyi reddederse alan boşalır; ad düz metin olarak kalır
    /// ve kayıt onaylanırken karşı taraf bulunur ya da kurulur.
    ///
    /// Arama <b>tam ad</b> üzerinedir (harf duyarsız): benzeyen adı
    /// eşleştirmek "Ahmet Market" ile "Ahmet Manav"ı aynı kişi saymak
    /// olurdu ve yanlış bakiyeyi doğru gibi gösterirdi. Arama kullanıcının
    /// kendi kayıtlarıyla sınırlı; başka kullanıcının karşı tarafı hiçbir
    /// koşulda önerilmez.
    /// </remarks>
    private async Task<ReceiptDraft> AttachCounterpartyMatchAsync(
        ReceiptDraft draft,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(draft.CounterpartyName))
            return draft;

        var match = await counterpartyRepository.FindOwnedByNameAsync(
            userId, draft.CounterpartyName, cancellationToken);

        // Eşleşme yoksa taslak olduğu gibi kalır: o adla ilk kez iş
        // yapılıyor olabilir ve bu bir hata değil.
        return match is null ? draft : draft with { CounterpartyId = match.Id };
    }

    /// <summary>
    /// Attaches the expense an iade fişi appears to reverse.
    ///
    /// <para>
    /// A suggestion, not an action: nothing is cancelled here and the endpoint
    /// still writes nothing. The candidate is shown to the user, who confirms it
    /// against the date, amount and name before anything happens. When no
    /// candidate is found the draft simply carries none — an invented
    /// cancellation would undo a record the user never pointed at.
    /// </para>
    /// </summary>
    private async Task<ReceiptDraft> AttachRefundMatchAsync(
        ReceiptDraft draft,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (draft.DocumentKind is not ReceiptDocumentKind.RefundReceipt)
            return draft;

        var match = await refundLookup.FindAsync(
            userId,
            draft.PurchasedAt,
            draft.TotalAmount,
            draft.CounterpartyName,
            cancellationToken);

        return match is null ? draft : draft with { RefundMatch = match };
    }

    /// <summary>
    /// The model's own claim about the document is trusted only to refuse, never
    /// to accept: an unknown value falls to the refusing side.
    ///
    /// <para>
    /// The gate now compares the paper against the direction the user declared.
    /// A retail till receipt can only be an expense — whoever holds it is the
    /// buyer — so capturing one as income is a contradiction and is reported,
    /// not converted. An invoice or voucher names both parties and is therefore
    /// valid in either direction; which one it is was already decided by the user,
    /// not here.
    /// </para>
    /// </summary>
    private static ApplicationError? RefuseUnsupportedKind(
        ReceiptDocumentKind kind,
        ReceiptCaptureIntent intent) => (kind, intent) switch
        {
            // Dekont niyeti üç banka belgesinin ÜÇÜNÜ birden kabul eder ve
            // hiçbirini yöne göre reddetmez: yön bu yolda henüz bildirilmedi,
            // okuma bittikten sonra karar sayfasında sorulacak. Bu yüzden
            // aşağıdaki `*_not_transfer` / `intent_mismatch` retlerinin hiçbiri
            // bu yolda oluşmaz — gerçek dekontların çoğu `bank_payment`'tır ve
            // eski akışta yalnız Harcama seçeneğinden geçebiliyorlardı.
            (ReceiptDocumentKind.BankDocument, ReceiptCaptureIntent.BankSlip) => null,
            (ReceiptDocumentKind.BankPayment, ReceiptCaptureIntent.BankSlip) => null,
            (ReceiptDocumentKind.CardPaymentSlip, ReceiptCaptureIntent.BankSlip) => null,

            // Kendi hesaplar arası aktarma yalnız transfer (veya dekont)
            // niyetinde okunur. Gider olarak okunsaydı hiç harcanmamış bir gider
            // yazılır, üstelik çekilen parayla yapılan alışveriş de fişlenince
            // aynı para iki kez sayılırdı.
            (ReceiptDocumentKind.BankDocument, ReceiptCaptureIntent.Transfer) => null,
            (ReceiptDocumentKind.BankDocument, _) => ReceiptAnalysisErrors.BankDocument,

            // Üçüncü tarafa yapılan ödeme bunun tersidir: para geri gelmeyecek
            // şekilde çıkmıştır, yani harcamadır. Transfer olarak kaydetmek
            // olmayan bir hedef hesap istemek olurdu.
            (ReceiptDocumentKind.BankPayment, ReceiptCaptureIntent.Expense) => null,
            (ReceiptDocumentKind.CardPaymentSlip, ReceiptCaptureIntent.Expense) => null,
            (ReceiptDocumentKind.CardPaymentSlip, ReceiptCaptureIntent.Transfer) =>
                ReceiptAnalysisErrors.BankPaymentNotTransfer,
            (ReceiptDocumentKind.CardPaymentSlip, _) => ReceiptAnalysisErrors.IntentMismatch,
            (ReceiptDocumentKind.BankPayment, ReceiptCaptureIntent.Transfer) =>
                ReceiptAnalysisErrors.BankPaymentNotTransfer,
            (ReceiptDocumentKind.BankPayment, _) => ReceiptAnalysisErrors.IntentMismatch,

            // Belgenin kendisiyle ilgili retler, kullanıcının hangi yönü
            // bildirdiğinden bağımsızdır ve yön kollarından ÖNCE gelir: iade
            // fişi hangi yönde okutulursa okutulsun iade fişidir. Sıra ters
            // olduğunda ret doğru ama gerekçe yanlış çıkıyordu ("bu bir
            // alışveriş belgesi"), oysa kullanıcının üzerine hareket ettiği şey
            // gerekçedir.
            // İade artık reddedilmiyor: okunuyor ve geri verdiği harcama
            // aranıyor. Yalnız gider yönünde — kullanıcı o alışverişi hangi
            // seçenekle yazdıysa iadesini de oradan okutur. Diğer yönlerde
            // yönlendirmeli ret duruyor; iadeyi gelir yazmak, geri gelen parayı
            // kazanılmış gibi göstermek olurdu.
            (ReceiptDocumentKind.RefundReceipt, ReceiptCaptureIntent.Expense) => null,
            (ReceiptDocumentKind.RefundReceipt, _) => ReceiptAnalysisErrors.RefundDocument,
            (ReceiptDocumentKind.InvoiceOrVoucher, ReceiptCaptureIntent.Transfer) =>
                ReceiptAnalysisErrors.NotATransferDocument,
            (ReceiptDocumentKind.PurchaseReceipt, ReceiptCaptureIntent.Transfer) =>
                ReceiptAnalysisErrors.NotATransferDocument,
            (ReceiptDocumentKind.InvoiceOrVoucher, ReceiptCaptureIntent.BankSlip) =>
                ReceiptAnalysisErrors.NotABankSlip,
            (ReceiptDocumentKind.PurchaseReceipt, ReceiptCaptureIntent.BankSlip) =>
                ReceiptAnalysisErrors.NotABankSlip,

            (ReceiptDocumentKind.InvoiceOrVoucher, _) => null,
            (ReceiptDocumentKind.PurchaseReceipt, ReceiptCaptureIntent.Expense) => null,
            (ReceiptDocumentKind.PurchaseReceipt, _) => ReceiptAnalysisErrors.IntentMismatch,
            _ => ReceiptAnalysisErrors.NotAReceipt
        };

    /// <summary>
    /// Reading the same photo twice used to produce two expenses in silence.
    ///
    /// <para>
    /// It warns instead of refusing: two identical purchases on one day are
    /// ordinary, and a block would be wrong more often than the double entry it
    /// prevents. The user sees what already exists and decides.
    /// </para>
    /// </summary>
    private async Task<ReceiptDraft> WarnIfAlreadyRecordedAsync(
        ReceiptDraft draft,
        Guid userId,
        ReceiptCaptureIntent intent,
        CancellationToken cancellationToken)
    {
        var match = await duplicateLookup.FindAsync(
            userId,
            draft.PurchasedAt,
            draft.TotalAmount,
            draft.CounterpartyName,
            intent,
            cancellationToken);
        if (match is null)
            return draft;

        return draft with
        {
            Warnings =
            [
                .. draft.Warnings,
                new ReceiptWarning(
                    ReceiptWarnings.PossibleDuplicate,
                    $"Bu belgeyi daha önce kaydetmiş olabilirsiniz: "
                        + $"{match.TransactionDate:dd.MM.yyyy} tarihli "
                        + $"{match.Amount:0.00} tutarlı {KindLabel(match.Kind)} "
                        + "zaten var.")
            ]
        };
    }

    /// <summary>
    /// Hangi rafta bulunduğunu söyler.
    ///
    /// <para>
    /// Türü söylemeyen bir uyarı kullanıcıyı yanlış listede arattırır: dekonttan
    /// yazılmış bir kart ödemesi gider listesinde hiçbir zaman bulunmaz.
    /// </para>
    /// </summary>
    private static string KindLabel(ReceiptDuplicateKind kind) => kind switch
    {
        ReceiptDuplicateKind.Transfer => "aktarma",
        ReceiptDuplicateKind.CardPayment => "kart ödemesi",
        ReceiptDuplicateKind.Receivable => "alacak kaydı",
        ReceiptDuplicateKind.Obligation => "Yükümlülükler kaydı",
        _ => "kayıt"
    };

    private static bool IsEmpty(ReceiptDraft draft) =>
        draft.CounterpartyState is ReceiptFieldState.Missing &&
        draft.PurchasedAtState is ReceiptFieldState.Missing &&
        draft.TotalAmountState is ReceiptFieldState.Missing &&
        draft.CategoryState is ReceiptFieldState.Missing;

    private static async Task<ApplicationResult<byte[]>> ReadContentAsync(
        AnalyzeReceiptCommand command,
        CancellationToken cancellationToken)
    {
        await using var buffer = new MemoryStream((int)command.DeclaredLength);
        var chunk = new byte[81920];
        long total = 0;
        int read;
        while ((read = await command.Content.ReadAsync(chunk, cancellationToken)) > 0)
        {
            total += read;
            if (total > MaximumFileSizeBytes)
            {
                return ApplicationResult<byte[]>.Failure(
                    ReceiptAnalysisErrors.FileTooLarge);
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        if (total != command.DeclaredLength)
        {
            return ApplicationResult<byte[]>.Failure(
                ReceiptAnalysisErrors.UnsupportedFile(
                    "Bildirilen dosya boyutu gerçek fotoğraf boyutuyla eşleşmiyor."));
        }

        return ApplicationResult<byte[]>.Success(buffer.ToArray());
    }
}
