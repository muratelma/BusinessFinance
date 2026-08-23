using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Attachments;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Application.Receipts;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.Receipts;

public sealed class AnalyzeReceiptUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid CategoryId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly DateTimeOffset Now =
        new(2026, 8, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly ReceiptAnalysisUsage Usage =
        new(1200, 50, 1250, TimeSpan.FromMilliseconds(700));

    [Fact]
    public async Task Execute_UnauthenticatedUser_IsRejectedBeforeReadingOrCallingProvider()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer, unauthenticated: true);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("auth.authentication_required", result.Error.Code);
        Assert.Empty(analyzer.Requests);
    }

    [Theory]
    [InlineData(0, "receipt.unsupported_file")]
    [InlineData(AnalyzeReceiptUseCase.MaximumFileSizeBytes + 1, "receipt.file_too_large")]
    public async Task Execute_EmptyOrOversizeFile_IsRejectedBeforeProvider(
        long declaredLength,
        string expectedCode)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            new AnalyzeReceiptCommand(
                "receipt.png", "image/png", new MemoryStream([1]), declaredLength));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Empty(analyzer.Requests);
    }

    [Fact]
    public async Task Execute_ActualBytesExceedDeclaredLength_IsRejected()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            new AnalyzeReceiptCommand(
                "receipt.png", "image/png", new MemoryStream([1, 2]), 1));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unsupported_file", result.Error.Code);
        Assert.Empty(analyzer.Requests);
    }

    [Fact]
    public async Task Execute_FileRejectedBySharedInspector_NeverReachesPreprocessorOrProvider()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var preprocessor = new StubPreprocessor();
        var useCase = Build(
            analyzer,
            inspector: new StubInspector(
                new AttachmentInspection(false, "signature mismatch", null, null, null)),
            preprocessor: preprocessor);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unsupported_file", result.Error.Code);
        Assert.Equal("signature mismatch", result.Error.Message);
        Assert.Equal(0, preprocessor.CallCount);
        Assert.Empty(analyzer.Requests);
    }

    [Fact]
    public async Task Execute_PdfThatPassesAttachmentInspection_IsStillNotAReceiptImage()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(
            analyzer,
            inspector: new StubInspector(
                new AttachmentInspection(true, null, "application/pdf", ".pdf", "hash")));

        var result = await useCase.ExecuteAsync(Command("receipt.pdf", "application/pdf"));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unsupported_file", result.Error.Code);
        Assert.Empty(analyzer.Requests);
    }

    [Fact]
    public async Task Execute_PreprocessorCannotDecodeImage_ReturnsUnreadable()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(
            analyzer,
            preprocessor: new StubPreprocessor(accepted: false));

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unreadable", result.Error.Code);
        Assert.Empty(analyzer.Requests);
    }

    [Theory]
    [InlineData("receipt.provider_unavailable")]
    [InlineData("receipt.provider_rate_limited")]
    [InlineData("receipt.unreadable")]
    [InlineData("receipt.disabled")]
    public async Task Execute_ProviderFailure_IsPreservedForTheApi(string errorCode)
    {
        var error = errorCode switch
        {
            "receipt.provider_unavailable" => ReceiptAnalysisErrors.ProviderUnavailable,
            "receipt.provider_rate_limited" => ReceiptAnalysisErrors.ProviderRateLimited,
            "receipt.unreadable" => ReceiptAnalysisErrors.Unreadable,
            "receipt.disabled" => ReceiptAnalysisErrors.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode))
        };
        var analyzer = new StubAnalyzer(error);
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal(errorCode, result.Error.Code);
    }

    [Fact]
    public async Task Execute_UsesOnlyActiveExpenseCategories_AndValidatesBeforeReturningDraft()
    {
        var category = new Category(CategoryId, UserId, "Market Özel", CategoryType.Expense);
        var repository = new StubCategoryRepository([category]);
        var analyzer = new StubAnalyzer(SuccessfulReading(
            category: category.Name,
            total: "847,50",
            subtotal: "700,00",
            tax: "100,00"));
        var useCase = Build(analyzer, categoryRepository: repository);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Equal(UserId, repository.RequestedUserId);
        Assert.Equal(CategoryType.Expense, repository.RequestedType);
        Assert.True(repository.RequestedIsActive);
        Assert.Equal([category.Name], Assert.Single(analyzer.Requests).CategoryNames);
        Assert.Equal(CategoryId, result.Value.Draft.CategoryId);
        Assert.Equal(847.50m, result.Value.Draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Suspect, result.Value.Draft.TotalAmountState);
        Assert.Contains(result.Value.Draft.Warnings, warning =>
            warning.Code == ReceiptWarnings.TotalsDoNotAddUp);
        Assert.Same(Usage, result.Value.Usage);
    }

    private static AnalyzeReceiptUseCase Build(
        StubAnalyzer analyzer,
        bool unauthenticated = false,
        IAttachmentFileInspector? inspector = null,
        IReceiptImagePreprocessor? preprocessor = null,
        StubCategoryRepository? categoryRepository = null,
        StubDuplicateLookup? duplicateLookup = null,
        StubRefundLookup? refundLookup = null,
        StubCounterpartyRepository? counterpartyRepository = null) => new(
            new StubCurrentUser(unauthenticated ? null : UserId),
            inspector ?? new StubInspector(),
            preprocessor ?? new StubPreprocessor(),
            categoryRepository ?? new StubCategoryRepository([]),
            counterpartyRepository ?? new StubCounterpartyRepository([]),
            analyzer,
            duplicateLookup ?? new StubDuplicateLookup(null),
            refundLookup ?? new StubRefundLookup(null),
            new FixedTimeProvider(Now));

    private static AnalyzeReceiptCommand Command(
        string fileName = "receipt.png",
        string contentType = "image/png",
        ReceiptCaptureIntent intent = ReceiptCaptureIntent.Expense) =>
        new(fileName, contentType, new MemoryStream([1]), 1, intent);


    /// <summary>
    /// A wire transfer slip read as a receipt produced a confident, wrong expense:
    /// the 5.000 TL that moved between the user's own accounts became spending,
    /// while the only amount actually spent — the 4,50 TL fee — was buried in it.
    /// The flow refuses the document instead of reading it.
    /// </summary>
    [Fact]
    public async Task Execute_BankTransferSlip_IsRefusedInsteadOfReadAsAnExpense()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.bank_document", result.Error.Code);
        Assert.Contains("transfer", result.Error.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Fişte okunan ad kullanıcının kendi karşı taraflarında aranır ve bulunan
    /// kayıt <b>öneri olarak</b> iliştirilir.
    /// </summary>
    /// <remarks>
    /// Model karşı tarafı seçmez (ADR 0011): burada olan tek şey, okunan adın
    /// kullanıcının kayıtlarında tam olarak bulunması. Ad da yerinde kalıyor —
    /// öneriyi reddeden kullanıcı adsız bir taslakla baş başa kalmamalı.
    /// </remarks>
    [Fact]
    public async Task Execute_WhenTheNameMatchesAKnownCounterparty_SuggestsIt()
    {
        var known = new Counterparty(Guid.NewGuid(), UserId, "Sentetik Market");
        var counterparties = new StubCounterpartyRepository([known]);
        var useCase = Build(new StubAnalyzer(SuccessfulReading()), counterpartyRepository: counterparties);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Equal(known.Id, result.Value.Draft.CounterpartyId);
        Assert.Equal("Sentetik Market", result.Value.Draft.CounterpartyName);
    }

    /// <summary>
    /// Benzeyen ad eşleşme değildir: "Sentetik Manav" ile "Sentetik Market"
    /// aynı kişi sayılsaydı, uygulama yanlış bir bakiyeyi doğru gibi gösterirdi.
    /// Eşleşme yoksa taslak yalnız adı taşır ve karşı taraf kayıt onaylanırken
    /// kurulur.
    /// </summary>
    [Fact]
    public async Task Execute_WhenOnlyASimilarNameExists_SuggestsNothing()
    {
        var other = new Counterparty(Guid.NewGuid(), UserId, "Sentetik Manav");
        var counterparties = new StubCounterpartyRepository([other]);
        var useCase = Build(new StubAnalyzer(SuccessfulReading()), counterpartyRepository: counterparties);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Draft.CounterpartyId);
        Assert.Equal("Sentetik Market", result.Value.Draft.CounterpartyName);
    }

    /// <summary>
    /// Başka kullanıcının aynı adlı karşı tarafı hiçbir koşulda önerilmez.
    /// </summary>
    [Fact]
    public async Task Execute_WhenTheMatchingNameBelongsToSomeoneElse_SuggestsNothing()
    {
        var strangers = new StubCounterpartyRepository(
            [new Counterparty(Guid.NewGuid(), Guid.NewGuid(), "Sentetik Market")]);
        var useCase = Build(new StubAnalyzer(SuccessfulReading()), counterpartyRepository: strangers);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Draft.CounterpartyId);
    }

    /// <summary>
    /// Ad okunamadıysa arama hiç yapılmaz: boş adla arama, ilk karşı tarafı
    /// rastgele önermeye açık kapı bırakırdı.
    /// </summary>
    [Fact]
    public async Task Execute_WhenTheNameIsUnreadable_DoesNotAskForAMatch()
    {
        var counterparties = new StubCounterpartyRepository(
            [new Counterparty(Guid.NewGuid(), UserId, "Sentetik Market")]);
        var reading = SuccessfulReading() with { UnreadableFields = ["counterpartyName"] };
        var useCase = Build(new StubAnalyzer(reading), counterpartyRepository: counterparties);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Draft.CounterpartyName);
        Assert.Null(result.Value.Draft.CounterpartyId);
        Assert.False(counterparties.WasAsked);
    }

    /// <summary>
    /// A refund slip used to be refused outright, because reading it produced an
    /// expense with the sign reversed: the 19 August 2026 field run turned a
    /// 320 TL pharmacy refund into 320 TL of spending on health.
    ///
    /// <para>
    /// It is now read in the expense direction and the expense it gives back is
    /// looked up. Nothing is cancelled here — the candidate is a suggestion the
    /// user confirms.
    /// </para>
    /// </summary>
    [Fact]
    public async Task Execute_RefundSlip_IsReadAndCarriesTheExpenseItReverses()
    {
        var match = new ReceiptRefundMatch(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 10),
            320m,
            "ECZANE",
            null);
        var refundLookup = new StubRefundLookup(match);
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.RefundReceipt));
        var useCase = Build(analyzer, refundLookup: refundLookup);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.True(refundLookup.WasAsked);
        Assert.Same(match, result.Value.Draft.RefundMatch);
    }

    /// <summary>
    /// No candidate means no candidate. Inventing one would undo a record the
    /// user never pointed at, and the refusal path is gone — the draft simply
    /// carries nothing and the client says so.
    /// </summary>
    [Fact]
    public async Task Execute_RefundSlipWithoutAMatch_StillSucceedsWithNoMatch()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.RefundReceipt));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Draft.RefundMatch);
    }

    /// <summary>
    /// The other directions still refuse it with guidance: recording a refund as
    /// income would show money coming back as money earned.
    /// </summary>
    [Theory]
    [InlineData(ReceiptCaptureIntent.Income)]
    [InlineData(ReceiptCaptureIntent.Transfer)]
    [InlineData(ReceiptCaptureIntent.BankSlip)]
    public async Task Execute_RefundSlipInAnotherDirection_IsRefused(
        ReceiptCaptureIntent intent)
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.RefundReceipt));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command(intent: intent));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.refund_document", result.Error.Code);
    }

    /// <summary>
    /// Only a refund slip triggers the lookup: every other document would be
    /// asking the database a question that cannot apply to it.
    /// </summary>
    [Fact]
    public async Task Execute_OrdinaryReceipt_DoesNotAskForARefundMatch()
    {
        var refundLookup = new StubRefundLookup(null);
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer, refundLookup: refundLookup);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.False(refundLookup.WasAsked);
    }

    [Theory]
    [InlineData(ReceiptDocumentKind.OtherDocument)]
    [InlineData(ReceiptDocumentKind.NotADocument)]
    [InlineData(ReceiptDocumentKind.Unknown)]
    public async Task Execute_AnythingButAPurchaseDocument_IsRefused(
        ReceiptDocumentKind kind)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.not_a_receipt", result.Error.Code);
    }

    /// <summary>
    /// The bucket list follows the declared direction. Offering expense buckets
    /// for a payslip would have the model choose a category the app can never
    /// post an income record to.
    /// </summary>
    [Theory]
    [InlineData(ReceiptCaptureIntent.Expense, CategoryType.Expense)]
    [InlineData(ReceiptCaptureIntent.Income, CategoryType.Income)]
    public async Task Execute_CategoryBuckets_FollowTheDeclaredDirection(
        ReceiptCaptureIntent intent,
        CategoryType expected)
    {
        var categories = new StubCategoryRepository([]);
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.InvoiceOrVoucher));
        var useCase = Build(analyzer, categoryRepository: categories);

        await useCase.ExecuteAsync(Command(intent: intent));

        Assert.Equal(expected, categories.RequestedType);
        Assert.Equal(UserId, categories.RequestedUserId);
        Assert.True(categories.RequestedIsActive);
    }

    /// <summary>
    /// The model is told the direction so it knows which printed party to read as
    /// the counterparty; it is never asked to work the direction out.
    /// </summary>
    [Fact]
    public async Task Execute_DeclaredDirection_ReachesTheAnalyzer()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.InvoiceOrVoucher));
        var useCase = Build(analyzer);

        await useCase.ExecuteAsync(Command(intent: ReceiptCaptureIntent.Income));

        Assert.Equal(ReceiptCaptureIntent.Income, Assert.Single(analyzer.Requests).Intent);
    }

    /// <summary>
    /// Whoever holds a till receipt is the buyer, so capturing one as income is a
    /// contradiction. It is reported rather than silently turned into income:
    /// the sign is the expensive half of a financial record.
    /// </summary>
    [Fact]
    public async Task Execute_TillReceiptCapturedAsIncome_IsRefusedNotConverted()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.PurchaseReceipt));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Income));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.intent_mismatch", result.Error.Code);
    }

    /// <summary>
    /// An invoice or voucher names both parties, so it is valid in either
    /// direction — which one it is was decided by the user, not by the gate.
    /// </summary>
    [Theory]
    [InlineData(ReceiptCaptureIntent.Expense)]
    [InlineData(ReceiptCaptureIntent.Income)]
    public async Task Execute_InvoiceOrVoucher_IsAcceptedInBothDirections(
        ReceiptCaptureIntent intent)
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.InvoiceOrVoucher));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command(intent: intent));

        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Refusals that are about the paper itself do not soften because the user
    /// declared a different direction: a bank slip is a transfer either way.
    /// </summary>
    [Theory]
    [InlineData(ReceiptDocumentKind.BankDocument, "receipt.bank_document")]
    [InlineData(ReceiptDocumentKind.RefundReceipt, "receipt.refund_document")]
    [InlineData(ReceiptDocumentKind.NotADocument, "receipt.not_a_receipt")]
    [InlineData(ReceiptDocumentKind.Unknown, "receipt.not_a_receipt")]
    public async Task Execute_PaperLevelRefusals_HoldInTheIncomeDirectionToo(
        ReceiptDocumentKind kind,
        string expectedCode)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Income));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    /// <summary>
    /// The bank-slip capture accepts all three bank kinds, because the direction
    /// is not declared on this path: it is asked after the read, on the decision
    /// page. Most real slips are `bank_payment` and used to be reachable only
    /// through the expense option.
    /// </summary>
    [Theory]
    [InlineData(ReceiptDocumentKind.BankDocument)]
    [InlineData(ReceiptDocumentKind.BankPayment)]
    [InlineData(ReceiptDocumentKind.CardPaymentSlip)]
    public async Task Execute_BankSlipCapture_ReadsEveryBankKind(
        ReceiptDocumentKind kind)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.BankSlip));

        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// A shopping document is still refused: the bank-slip path ends in the
    /// "what is this amount?" question, which is meaningless for a till receipt.
    /// </summary>
    [Theory]
    [InlineData(ReceiptDocumentKind.PurchaseReceipt, "receipt.not_a_bank_slip")]
    [InlineData(ReceiptDocumentKind.InvoiceOrVoucher, "receipt.not_a_bank_slip")]
    [InlineData(ReceiptDocumentKind.RefundReceipt, "receipt.refund_document")]
    [InlineData(ReceiptDocumentKind.NotADocument, "receipt.not_a_receipt")]
    public async Task Execute_BankSlipCapture_RefusesWhatIsNotASlip(
        ReceiptDocumentKind kind,
        string expectedCode)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.BankSlip));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    /// <summary>
    /// The same invariant in the transfer direction, which used to be reported
    /// wrongly: the transfer arm was matched before the paper-level arms, so an
    /// iade fişi captured as a transfer was told it is a purchase document. The
    /// refusal was right, the reason was not — and the reason is what the user
    /// acts on.
    /// </summary>
    [Theory]
    [InlineData(ReceiptDocumentKind.RefundReceipt, "receipt.refund_document")]
    [InlineData(ReceiptDocumentKind.NotADocument, "receipt.not_a_receipt")]
    [InlineData(ReceiptDocumentKind.Unknown, "receipt.not_a_receipt")]
    public async Task Execute_PaperLevelRefusals_HoldInTheTransferDirectionToo(
        ReceiptDocumentKind kind,
        string expectedCode)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    /// <summary>
    /// Reading the same receipt twice used to produce two expenses in silence.
    /// The user is told, not blocked: two identical purchases in a day happen.
    /// </summary>
    [Fact]
    public async Task Execute_DocumentAlreadyRecorded_WarnsInsteadOfRefusing()
    {
        var lookup = new StubDuplicateLookup(new ReceiptDuplicateMatch(
            Guid.NewGuid(), new DateOnly(2026, 8, 18), 100m, "Sentetik Market"));
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer, duplicateLookup: lookup);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        var warning = Assert.Single(
            result.Value.Draft.Warnings,
            item => item.Code == ReceiptWarnings.PossibleDuplicate);
        Assert.Contains("18.08.2026", warning.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The lookup is asked with the fields the user is about to confirm, and in
    /// the direction they declared — an income document cannot duplicate an
    /// expense.
    /// </summary>
    [Fact]
    public async Task Execute_DuplicateLookup_IsAskedWithTheReadFieldsAndDirection()
    {
        var lookup = new StubDuplicateLookup(null);
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.InvoiceOrVoucher));
        var useCase = Build(analyzer, duplicateLookup: lookup);

        await useCase.ExecuteAsync(Command(intent: ReceiptCaptureIntent.Income));

        Assert.Equal(new DateOnly(2026, 8, 18), lookup.RequestedDate);
        Assert.Equal(100m, lookup.RequestedAmount);
        Assert.Equal("Sentetik Market", lookup.RequestedCounterparty);
        Assert.Equal(ReceiptCaptureIntent.Income, lookup.RequestedIntent);
    }

    /// <summary>
    /// A bank slip is checked too, and the warning says <b>where</b> the match
    /// is.
    ///
    /// <para>
    /// Until this test the slip paths had no protection worth the name: the
    /// only shelf searched was the expense list, and a slip becomes a transfer,
    /// a card payment or a receivable at least as often. A warning that does
    /// not name the kind is nearly as bad — the user goes looking for a card
    /// payment among their expenses and concludes the warning was wrong.
    /// </para>
    /// </summary>
    [Theory]
    [InlineData(ReceiptDuplicateKind.Transfer, "aktarma")]
    [InlineData(ReceiptDuplicateKind.CardPayment, "kart ödemesi")]
    [InlineData(ReceiptDuplicateKind.Receivable, "alacak kaydı")]
    public async Task Execute_BankSlipAlreadyRecorded_NamesTheKindInTheWarning(
        ReceiptDuplicateKind kind,
        string expectedLabel)
    {
        var lookup = new StubDuplicateLookup(new ReceiptDuplicateMatch(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 18),
            100m,
            "Sentetik Market",
            kind));
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer, duplicateLookup: lookup);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.BankSlip));

        Assert.True(result.IsSuccess);
        var warning = Assert.Single(
            result.Value.Draft.Warnings,
            item => item.Code == ReceiptWarnings.PossibleDuplicate);
        Assert.Contains(expectedLabel, warning.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The transfer intent is asked as well.
    ///
    /// <para>
    /// It used to return before the lookup ran, on the grounds that a transfer
    /// has no counterparty to match on. That stopped being true once every slip
    /// path started carrying the read name into its description: the third leg
    /// exists, so the strict rule applies here like anywhere else.
    /// </para>
    /// </summary>
    [Fact]
    public async Task Execute_TransferIntent_IsCheckedForDuplicatesToo()
    {
        var lookup = new StubDuplicateLookup(null);
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer, duplicateLookup: lookup);

        await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.Equal(ReceiptCaptureIntent.Transfer, lookup.RequestedIntent);
        Assert.Equal(new DateOnly(2026, 8, 18), lookup.RequestedDate);
    }

    [Fact]
    public async Task Execute_NoMatchingRecord_AddsNoDuplicateWarning()
    {
        var analyzer = new StubAnalyzer(SuccessfulReading());
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(
            result.Value.Draft.Warnings,
            item => item.Code == ReceiptWarnings.PossibleDuplicate);
    }

    /// <summary>
    /// A bank slip is only readable when the user said "transfer". Read as an
    /// expense it invents spending that never happened, and the cash it produced
    /// gets spent again on receipts the user also scans.
    /// </summary>
    [Theory]
    [InlineData(ReceiptCaptureIntent.Expense, "receipt.bank_document")]
    [InlineData(ReceiptCaptureIntent.Income, "receipt.bank_document")]
    public async Task Execute_BankSlipWithoutTransferIntent_IsStillRefused(
        ReceiptCaptureIntent intent,
        string expectedCode)
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command(intent: intent));

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    [Fact]
    public async Task Execute_BankSlipDeclaredAsTransfer_IsRead()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// A bank slip that pays someone else is spending, not moving. The 19 August
    /// 2026 field run found all three real dekont samples were this kind: two
    /// havale to other people and one utility bill payment. Refusing them with
    /// "record it as a transfer" told the user to name a destination account that
    /// does not exist.
    /// </summary>
    [Fact]
    public async Task Execute_PaymentToSomeoneElse_IsReadAsSpending()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankPayment));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Expense));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Execute_PaymentSlipDeclaredAsTransfer_SaysItIsAPaymentNotAMove()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankPayment));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.bank_payment_not_transfer", result.Error.Code);
        Assert.Contains("Harcama", result.Error.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A card payment slip is read like any bank payment — the refusal would be
    /// wrong, the user still has to record it — but it carries its own kind so
    /// the app can put the right answer first. Recording one as spending
    /// double-counts: the purchases were already counted as card charges.
    /// </summary>
    [Fact]
    public async Task Execute_CardPaymentSlip_IsReadAndKeepsItsOwnKind()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.CardPaymentSlip));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Expense));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            ReceiptDocumentKind.CardPaymentSlip,
            result.Value.Draft.DocumentKind);
    }

    [Fact]
    public async Task Execute_CardPaymentSlipDeclaredAsTransfer_IsRefused()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.CardPaymentSlip));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.bank_payment_not_transfer", result.Error.Code);
    }

    /// <summary>
    /// The two bank kinds are opposites and neither refusal may point at the
    /// other's flow: an own-account move is not spending, a payment is.
    /// </summary>
    [Fact]
    public async Task Execute_OwnAccountMoveAsExpense_StillPointsAtTheTransferFlow()
    {
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Expense));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.bank_document", result.Error.Code);
        Assert.Contains("Transfer", result.Error.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The mismatch works in both directions: a purchase document declared as a
    /// transfer would move money without recording that it was spent.
    /// </summary>
    [Theory]
    [InlineData(ReceiptDocumentKind.PurchaseReceipt)]
    [InlineData(ReceiptDocumentKind.InvoiceOrVoucher)]
    public async Task Execute_PurchaseDocumentDeclaredAsTransfer_IsRefused(
        ReceiptDocumentKind kind)
    {
        var analyzer = new StubAnalyzer(SuccessfulReading(kind: kind));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(
            Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.not_a_transfer_document", result.Error.Code);
    }

    /// <summary>
    /// A transfer has no category: the money was not spent, it moved. Offering
    /// buckets would have the model invent a choice the app never posts.
    /// </summary>
    [Fact]
    public async Task Execute_TransferIntent_OffersNoCategoriesToTheModel()
    {
        var categories = new StubCategoryRepository([
            new Category(CategoryId, UserId, "Market", CategoryType.Expense)
        ]);
        var analyzer = new StubAnalyzer(
            SuccessfulReading(kind: ReceiptDocumentKind.BankDocument));
        var useCase = Build(analyzer, categoryRepository: categories);

        await useCase.ExecuteAsync(Command(intent: ReceiptCaptureIntent.Transfer));

        Assert.Empty(Assert.Single(analyzer.Requests).CategoryNames);
    }

    /// <summary>
    /// An unrelated photo used to come back as 200 with an empty draft and one
    /// warning per field: five boxes saying the same thing. Nothing readable is a
    /// failed reading.
    /// </summary>
    [Fact]
    public async Task Execute_NothingReadable_FailsOnceInsteadOfWarningPerField()
    {
        var analyzer = new StubAnalyzer(EmptyReading());
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.False(result.IsSuccess);
        Assert.Equal("receipt.unreadable", result.Error.Code);
    }

    [Fact]
    public async Task Execute_OnlySomeFieldsUnreadable_StillReturnsTheDraft()
    {
        // Yarım okunan fiş hâlâ işe yarar: kullanıcı eksik alanı kendisi doldurur.
        var analyzer = new StubAnalyzer(SuccessfulReading(total: "100,00"));
        var useCase = Build(analyzer);

        var result = await useCase.ExecuteAsync(Command());

        Assert.True(result.IsSuccess);
    }

    private static RawReceiptReading EmptyReading() => new(
        ReceiptDocumentKind.PurchaseReceipt,
        null, null, null, null, null, null, null, null, null, null, null,
        ["counterpartyName", "purchasedAt", "totalAmount", "categoryName"],
        new ReceiptAnalysisUsage(0, 0, 0, TimeSpan.Zero));

    private static RawReceiptReading SuccessfulReading(
        string? category = null,
        string total = "100,00",
        string? subtotal = null,
        string? tax = null,
        string? dueDate = null,
        string? installments = null,
        ReceiptDocumentKind kind = ReceiptDocumentKind.PurchaseReceipt) => new(
            kind,
            "Sentetik Market",
            "18.08.2026",
            dueDate,
            subtotal,
            tax,
            total,
            null,
            installments,
            "TRY",
            "card",
            category,
            [],
            Usage);

    /// <summary>
    /// Yalnız ada göre arama gerçek: fiş okuma karşı tarafı <b>önerir</b>,
    /// kurmaz. Kalan port yüzeyi bu use case'in yolunda değil ve çağrılırsa
    /// test sessizce yanlış şeyi doğrulamak yerine düşer.
    /// </summary>
    private sealed class StubCounterpartyRepository(IReadOnlyList<Counterparty> known)
        : ICounterpartyRepository
    {
        public bool WasAsked { get; private set; }

        public Task<Counterparty?> FindOwnedByNameAsync(
            Guid userId, string name, CancellationToken cancellationToken)
        {
            WasAsked = true;
            return Task.FromResult(
                known.FirstOrDefault(item =>
                    item.UserId == userId &&
                    string.Equals(item.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task<Counterparty?> FindOwnedByIdAsync(
            Guid counterpartyId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<CounterpartyBalanceSummary>> ListBalancesAsync(
            Guid userId,
            CounterpartyBalanceFilter filter,
            bool? isActive,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyBalanceSummary?> FindBalanceAsync(
            Guid counterpartyId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Counterparty> FindOrCreateByNameAsync(
            Guid userId, string name, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyDictionary<Guid, string>> ListNamesAsync(
            Guid userId,
            IReadOnlyCollection<Guid> counterpartyIds,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAsync(
            Guid userId,
            string normalizedName,
            Guid? exceptCounterpartyId,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddAsync(Counterparty counterparty, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task UpdateOwnedAsync(
            Counterparty counterparty, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> DeleteIfWithoutHistoryAsync(
            Guid counterpartyId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddPaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyCharge?> FindOwnedChargeAsync(
            Guid chargeId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<CounterpartyPayment?> FindOwnedPaymentAsync(
            Guid paymentId, Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SaveChargeAsync(CounterpartyCharge charge, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task SavePaymentAsync(CounterpartyPayment payment, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class StubRefundLookup(ReceiptRefundMatch? match)
        : IReceiptRefundLookup
    {
        public bool WasAsked { get; private set; }

        public Task<ReceiptRefundMatch?> FindAsync(
            Guid userId,
            DateOnly? refundDate,
            decimal? refundAmount,
            string? counterpartyName,
            CancellationToken cancellationToken)
        {
            WasAsked = true;
            return Task.FromResult(match);
        }
    }

    private sealed class StubDuplicateLookup(ReceiptDuplicateMatch? match)
        : IReceiptDuplicateLookup
    {
        public DateOnly? RequestedDate { get; private set; }
        public decimal? RequestedAmount { get; private set; }
        public string? RequestedCounterparty { get; private set; }
        public ReceiptCaptureIntent? RequestedIntent { get; private set; }

        public Task<ReceiptDuplicateMatch?> FindAsync(
            Guid userId,
            DateOnly? transactionDate,
            decimal? amount,
            string? counterpartyName,
            ReceiptCaptureIntent intent,
            CancellationToken cancellationToken)
        {
            RequestedDate = transactionDate;
            RequestedAmount = amount;
            RequestedCounterparty = counterpartyName;
            RequestedIntent = intent;
            return Task.FromResult(match);
        }
    }

    private sealed class StubCurrentUser(Guid? userId) : ICurrentUser
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class StubInspector : IAttachmentFileInspector
    {
        private readonly AttachmentInspection _inspection;

        public StubInspector(AttachmentInspection? inspection = null)
        {
            _inspection = inspection ?? new AttachmentInspection(
                true, null, "image/png", ".png", "hash");
        }

        public AttachmentInspection Inspect(
            string fileName,
            string contentType,
            ReadOnlySpan<byte> content) => _inspection;
    }

    private sealed class StubPreprocessor(bool accepted = true) : IReceiptImagePreprocessor
    {
        public int CallCount { get; private set; }

        public ReceiptImageNormalization Normalize(ReceiptImage image)
        {
            CallCount++;
            return accepted
                ? new ReceiptImageNormalization(
                    true, null, image, 1, 1, 1, 1, false, false, false)
                : ReceiptImageNormalization.Reject("cannot decode");
        }
    }

    private sealed class StubAnalyzer : IReceiptAnalyzer
    {
        private readonly ApplicationResult<RawReceiptReading> _result;

        public StubAnalyzer(RawReceiptReading reading)
        {
            _result = ApplicationResult<RawReceiptReading>.Success(reading);
        }

        public StubAnalyzer(ApplicationError error)
        {
            _result = ApplicationResult<RawReceiptReading>.Failure(error);
        }

        public List<ReceiptAnalysisRequest> Requests { get; } = [];

        public Task<ApplicationResult<RawReceiptReading>> AnalyzeAsync(
            ReceiptAnalysisRequest request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(_result);
        }
    }

    private sealed class StubCategoryRepository(IReadOnlyList<Category> categories)
        : ICategoryRepository
    {
        public Guid? RequestedUserId { get; private set; }
        public CategoryType? RequestedType { get; private set; }
        public bool? RequestedIsActive { get; private set; }

        public Task<IReadOnlyList<Category>> ListAsync(
            Guid userId,
            CategoryType? type,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            RequestedUserId = userId;
            RequestedType = type;
            RequestedIsActive = isActive;
            return Task.FromResult(categories);
        }

        public Task EnsureDefaultsAsync(Guid userId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task AddAsync(Category category, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByNameAndTypeAsync(
            Guid userId,
            string name,
            CategoryType type,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<Category?> FindOwnedByIdAsync(
            Guid categoryId,
            Guid userId,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task UpdateOwnedAsync(
            Category category,
            Guid userId,
            CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
