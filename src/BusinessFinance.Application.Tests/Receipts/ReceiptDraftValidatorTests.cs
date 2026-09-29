using BusinessFinance.Application.Receipts;

namespace BusinessFinance.Application.Tests.Receipts;

public sealed class ReceiptDraftValidatorTests
{
    private static readonly DateOnly Today = new(2026, 8, 18);
    private static readonly Guid MarketId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TransportId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly ReceiptCategoryOption[] Categories =
    [
        new(MarketId, "Market"),
        new(TransportId, "Ulaşım")
    ];

    // ---- Pass 2: amounts -----------------------------------------------------

    /// <summary>
    /// The model is asked for "1234.56" and answers however it likes. Each of
    /// these is a real way a Turkish total gets written, and none of them may be
    /// silently misread by a factor of a hundred or a thousand.
    /// </summary>
    [Theory]
    [InlineData("847.50", 847.50)]
    [InlineData("847,50", 847.50)]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("1,234.56", 1234.56)]
    [InlineData("1234", 1234)]
    [InlineData("1.234", 1234)]      // one separator, three digits after: a group
    [InlineData("1.234.567", 1234567)]
    [InlineData("847,5000", 847.5)]  // four digits cannot be a group
    [InlineData(" 847,50 TL ", 847.50)]
    [InlineData("₺847,50", 847.50)]
    public void Validate_AmountWrittenAnyOfTheUsualWays_ReadsTheSameNumber(
        string written,
        decimal expected)
    {
        var draft = Validate(Reading(total: written));

        Assert.Equal(expected, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_AmountThatIsNotANumber_IsLeftBlankRatherThanGuessed(string written)
    {
        var draft = Validate(Reading(total: written));

        Assert.Null(draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Missing, draft.TotalAmountState);
    }

    [Theory]
    [InlineData("-50")]
    [InlineData("0")]
    [InlineData("99999999")]
    public void Validate_AmountOutsideWhatAReceiptCanSay_IsDropped(string written)
    {
        var draft = Validate(Reading(total: written));

        Assert.Null(draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Missing, draft.TotalAmountState);
        Assert.Contains(draft.Warnings, warning =>
            warning.Code is ReceiptWarnings.AmountOutOfRange or ReceiptWarnings.AmountUnparsed);
    }

    // ---- Pass 2: dates -------------------------------------------------------

    /// <summary>
    /// A live call showed the model answering dd.MM.yyyy even though the prompt
    /// asks for ISO, so both are read. Day comes first: the receipts are Turkish.
    /// </summary>
    [Theory]
    [InlineData("2026-08-17", 2026, 8, 17)]
    [InlineData("17.08.2026", 2026, 8, 17)]
    [InlineData("17/08/2026", 2026, 8, 17)]
    [InlineData("01.02.2026", 2026, 2, 1)]
    public void Validate_DateInEitherNotation_ReadsTheSameDay(
        string written, int year, int month, int day)
    {
        var draft = Validate(Reading(date: written));

        Assert.Equal(new DateOnly(year, month, day), draft.PurchasedAt);
        Assert.Equal(ReceiptFieldState.Read, draft.PurchasedAtState);
    }

    [Fact]
    public void Validate_DateInTheFuture_IsDroppedInsteadOfPostingToAMonthThatHasNotHappened()
    {
        var draft = Validate(Reading(date: "19.08.2026"));

        Assert.Null(draft.PurchasedAt);
        Assert.Equal(ReceiptFieldState.Missing, draft.PurchasedAtState);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.DateInFuture);
    }

    [Fact]
    public void Validate_TodayItself_IsAccepted()
    {
        var draft = Validate(Reading(date: "18.08.2026"));

        Assert.Equal(Today, draft.PurchasedAt);
    }

    [Fact]
    public void Validate_DateOlderThanADecade_IsDoubtedNotDropped()
    {
        // 19 Ağustos 2026 saha koşumunda 2016 tarihli gerçek bir fiş çıktı:
        // doğru okunan tarih siliniyor ve kullanıcıya yeniden yazdırılıyordu.
        var draft = Validate(Reading(date: "17.08.2016"));

        Assert.Equal(new DateOnly(2016, 8, 17), draft.PurchasedAt);
        Assert.Equal(ReceiptFieldState.Suspect, draft.PurchasedAtState);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.DateTooOld);
    }

    [Fact]
    public void Validate_DateThatIsNotADate_IsDropped()
    {
        var draft = Validate(Reading(date: "dün"));

        Assert.Null(draft.PurchasedAt);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.DateUnparsed);
    }

    // ---- KDV: yalnız toplamı denetler (ADR 0018) -----------------------------

    /// <summary>
    /// Belgede yazan KDV taslağa taşınmaz: ürün KDV tutmaz. Okunan tutar yalnız
    /// ara toplam + KDV = toplam denetiminde kullanılır; tek başına ne bir uyarı
    /// üretir ne de tutarı değiştirir.
    /// </summary>
    [Fact]
    public void Validate_ReceiptPrintsVat_LeavesTheTotalAloneAndWarnsNothing()
    {
        var draft = Validate(Reading(total: "280,00", tax: "46,67"));

        Assert.Equal(280.00m, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
        Assert.Empty(draft.Warnings);
    }

    // ---- Pass 4: consistency -------------------------------------------------

    /// <summary>
    /// The free accuracy signal: the receipt printed three numbers that must add
    /// up. When they do not, one of them was misread and the total is flagged —
    /// not corrected, because which one is wrong is unknowable.
    /// </summary>
    [Fact]
    public void Validate_SubtotalPlusTaxMissesTheTotal_FlagsTheTotalWithoutChangingIt()
    {
        var draft = Validate(Reading(total: "847,50", subtotal: "705,92", tax: "41,58"));

        Assert.Equal(847.50m, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Suspect, draft.TotalAmountState);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.TotalsDoNotAddUp);
    }

    [Fact]
    public void Validate_SubtotalPlusTaxReachesTheTotal_LeavesItTrusted()
    {
        var draft = Validate(Reading(total: "847,50", subtotal: "705,92", tax: "141,58"));

        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
        Assert.DoesNotContain(draft.Warnings, w => w.Code == ReceiptWarnings.TotalsDoNotAddUp);
    }

    [Fact]
    public void Validate_TotalsOffByRounding_IsNotTreatedAsAMisread()
    {
        var draft = Validate(Reading(total: "847,50", subtotal: "705,90", tax: "141,58"));

        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
    }

    [Fact]
    public void Validate_ReceiptWithoutASubtotal_SkipsTheCheckInsteadOfFailingIt()
    {
        var draft = Validate(Reading(total: "847,50", subtotal: null, tax: null));

        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
        Assert.Empty(draft.Warnings);
    }

    [Fact]
    public void Validate_ForeignCurrency_KeepsTheNumberButFlagsIt()
    {
        var draft = Validate(Reading(total: "84,50", currency: "EUR"));

        Assert.Equal(84.50m, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Suspect, draft.TotalAmountState);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.CurrencyUnexpected);
    }

    // ---- Pass 5: ownership ---------------------------------------------------

    [Fact]
    public void Validate_CategoryTheUserOwns_ResolvesToItsIdentity()
    {
        var draft = Validate(Reading(category: "Market"));

        Assert.Equal(MarketId, draft.CategoryId);
        Assert.Equal("Market", draft.CategoryName);
        Assert.Equal(ReceiptFieldState.Read, draft.CategoryState);
    }

    [Fact]
    public void Validate_CategoryDifferingOnlyByCase_StillResolves()
    {
        var draft = Validate(Reading(category: "  market "));

        Assert.Equal(MarketId, draft.CategoryId);
    }

    /// <summary>
    /// The guard that matters: a name that is not in this budget resolves to
    /// nothing. It must never reach for a similar category, and it can never
    /// reach another user's.
    /// </summary>
    [Fact]
    public void Validate_CategoryThatIsNotTheUsersOwn_ResolvesToNothing()
    {
        var draft = Validate(Reading(category: "Kozmetik"));

        Assert.Null(draft.CategoryId);
        Assert.Null(draft.CategoryName);
        Assert.Equal(ReceiptFieldState.Missing, draft.CategoryState);
        Assert.Contains(draft.Warnings, warning => warning.Code == ReceiptWarnings.CategoryUnknown);
    }

    [Fact]
    public void Validate_UserWithNoCategoriesAtAll_ResolvesToNothingWithoutThrowing()
    {
        var draft = ReceiptDraftValidator.Validate(Reading(category: "Market"), [], Today);

        Assert.Null(draft.CategoryId);
        Assert.Equal(ReceiptFieldState.Missing, draft.CategoryState);
    }

    // ---- Pass 1 and 6: shape and verdict -------------------------------------

    [Fact]
    public void Validate_FieldTheModelReportedUnreadable_IsNotParsedAtAll()
    {
        var reading = Reading(total: "847,50", date: "17.08.2026") with
        {
            UnreadableFields = ["totalAmount", "purchasedAt"]
        };

        var draft = ReceiptDraftValidator.Validate(reading, Categories, Today);

        Assert.Null(draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Missing, draft.TotalAmountState);
        Assert.Null(draft.PurchasedAt);
        Assert.Equal(2, draft.Warnings.Count(w => w.Code == ReceiptWarnings.FieldUnreadable));
    }

    [Theory]
    [InlineData("cash", ReceiptPaymentHint.Cash)]
    [InlineData("credit_card", ReceiptPaymentHint.CreditCard)]
    [InlineData("debit_card", ReceiptPaymentHint.DebitCard)]
    // Türü yazmayan kart ödemesi kendi değerinde kalır; krediye yuvarlanmaz.
    [InlineData("card", ReceiptPaymentHint.Card)]
    [InlineData("unknown", ReceiptPaymentHint.Unknown)]
    [InlineData("kredi karti", ReceiptPaymentHint.Unknown)]
    [InlineData("creditcard", ReceiptPaymentHint.Unknown)]
    [InlineData(null, ReceiptPaymentHint.Unknown)]
    public void Validate_PaymentHint_OnlyEverNarrowsThePicker(
        string? written,
        ReceiptPaymentHint expected)
    {
        var draft = Validate(Reading(paymentHint: written));

        Assert.Equal(expected, draft.PaymentHint);
    }

    [Fact]
    public void Validate_CompleteReceipt_ComesOutFullyTrustedAndWithoutWarnings()
    {
        var draft = Validate(Reading(
            merchant: "Test Market",
            date: "18.08.2026",
            total: "847,50",
            subtotal: "705,92",
            tax: "141,58",
            currency: "TRY",
            paymentHint: "card",
            category: "Market"));

        Assert.Equal("Test Market", draft.CounterpartyName);
        Assert.Equal(ReceiptFieldState.Read, draft.CounterpartyState);
        Assert.Equal(new DateOnly(2026, 8, 18), draft.PurchasedAt);
        Assert.Equal(847.50m, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
        Assert.Equal("TRY", draft.CurrencyCode);
        Assert.Equal(ReceiptPaymentHint.Card, draft.PaymentHint);
        Assert.Equal(MarketId, draft.CategoryId);
        Assert.Empty(draft.Warnings);
    }

    /// <summary>
    /// A ten-year-old receipt is unusual but real — the field set has one from
    /// 2016. Dropping a date the model read correctly made the user retype it;
    /// the doubt belongs in the state, not in a deleted value.
    /// </summary>
    [Fact]
    public void Validate_DateOlderThanTheWindow_IsKeptAsSuspectInsteadOfDropped()
    {
        var draft = Validate(Reading(date: "31.03.2016"));

        Assert.Equal(new DateOnly(2016, 3, 31), draft.PurchasedAt);
        Assert.Equal(ReceiptFieldState.Suspect, draft.PurchasedAtState);
        Assert.Contains(draft.Warnings, warning => warning.Code == "receipt.date_too_old");
    }

    [Fact]
    public void Validate_DateInTheFuture_IsStillDropped()
    {
        // Gelecekteki bir tarih yanlış okumadır: olmamış bir ay'a gider yazamayız.
        var draft = Validate(Reading(date: "17.08.2027"));

        Assert.Null(draft.PurchasedAt);
        Assert.Equal(ReceiptFieldState.Missing, draft.PurchasedAtState);
    }

    /// <summary>
    /// Dekonttaki 5.000 ve 4,50 birleştirilince hiç harcanmamış bir 5.004,50
    /// gideri üretilmişti. Transfer parayı taşır; harcanan yalnız ücrettir, ve
    /// ikisi ayrı alanlarda kalır.
    /// </summary>
    [Fact]
    public void Validate_TransferFee_StaysApartFromTheAmountMoved()
    {
        var draft = Validate(Reading(total: "5000,00", fee: "4,50"));

        Assert.Equal(5000.00m, draft.TotalAmount);
        Assert.Equal(ReceiptFieldState.Read, draft.TotalAmountState);
        Assert.Equal(4.50m, draft.FeeAmount);
        Assert.Equal(ReceiptFieldState.Read, draft.FeeAmountState);
    }

    [Fact]
    public void Validate_NoFeePrinted_LeavesTheFeeMissingInsteadOfZero()
    {
        var draft = Validate(Reading(total: "5000,00"));

        Assert.Null(draft.FeeAmount);
        Assert.Equal(ReceiptFieldState.Missing, draft.FeeAmountState);
    }

    [Fact]
    public void Validate_ReceiptThatCouldNotBeReadAtAll_ProducesAnEmptyDraftNotAFabricatedOne()
    {
        var draft = ReceiptDraftValidator.Validate(
            new RawReceiptReading(
                ReceiptDocumentKind.PurchaseReceipt,
                null, null, null, null, null, null, null, null, null, null, null, [], NoUsage),
            Categories,
            Today);

        Assert.Null(draft.CounterpartyName);
        Assert.Null(draft.PurchasedAt);
        Assert.Null(draft.TotalAmount);
        Assert.Null(draft.CategoryId);
        Assert.Equal(ReceiptPaymentHint.Unknown, draft.PaymentHint);
        Assert.All(
            new[]
            {
                draft.CounterpartyState, draft.PurchasedAtState,
                draft.TotalAmountState, draft.CategoryState
            },
            state => Assert.Equal(ReceiptFieldState.Missing, state));
    }

    /// <summary>
    /// Son ödeme tarihi belgenin kendi tarihi değildir ve kuralları da aynı
    /// olamaz: bir fiş yarından olamaz ama bir faturanın vadesi tam da
    /// gelecektedir. Aynı okuyucuyu kullanmak her ödenmemiş faturanın vadesini
    /// düşürürdü.
    /// </summary>
    [Fact]
    public void Validate_DueDateInTheFuture_IsKeptUnlikeTheDocumentDate()
    {
        var draft = Validate(Reading(dueDate: "30.09.2026"));

        Assert.Equal(new DateOnly(2026, 9, 30), draft.DueDate);
        Assert.Equal(ReceiptFieldState.Read, draft.DueDateState);
        // Belgenin kendi tarihi yerine geçmedi.
        Assert.Equal(new DateOnly(2026, 8, 17), draft.PurchasedAt);
    }

    /// <summary>
    /// Gecikmiş bir fatura hâlâ ödenmemiştir; geçmiş vade düşürülmez.
    /// </summary>
    [Fact]
    public void Validate_DueDateInThePast_IsAlsoKept()
    {
        var draft = Validate(Reading(dueDate: "01.08.2026"));

        Assert.Equal(new DateOnly(2026, 8, 1), draft.DueDate);
        Assert.Equal(ReceiptFieldState.Read, draft.DueDateState);
    }

    [Fact]
    public void Validate_NoDueDateOnTheDocument_LeavesItMissing()
    {
        var draft = Validate(Reading());

        Assert.Null(draft.DueDate);
        Assert.Equal(ReceiptFieldState.Missing, draft.DueDateState);
    }

    /// <summary>
    /// Taksitli satış tek seferlik tam tutar gideri değildir: o ay bütçeden
    /// çıkan yalnız bir taksittir.
    /// </summary>
    [Theory]
    [InlineData("3", 3)]
    [InlineData("12", 12)]
    public void Validate_InstallmentCountOnTheReceipt_IsRead(
        string raw,
        int expected)
    {
        var draft = Validate(Reading(installments: raw));

        Assert.Equal(expected, draft.InstallmentCount);
    }

    /// <summary>
    /// "1 taksit" tek çekimdir ve plan üretmez; anlaşılmayan bir değer de
    /// sessizce bir sayıya düşmez. Düşseydi taksitli bir alışveriş tek
    /// seferlik tam tutar gideri olurdu — bu kontrolün önlediği şey.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("üç")]
    [InlineData("3 TAKSİT")]
    [InlineData("400")]
    public void Validate_UnusableInstallmentCount_LeavesItNull(string? raw)
    {
        var draft = Validate(Reading(installments: raw));

        Assert.Null(draft.InstallmentCount);
    }

    private static ReceiptDraft Validate(RawReceiptReading reading) =>
        ReceiptDraftValidator.Validate(reading, Categories, Today);

    private static readonly ReceiptAnalysisUsage NoUsage = new(0, 0, 0, TimeSpan.Zero);

    private static RawReceiptReading Reading(
        string? merchant = "Test Market",
        string? date = "17.08.2026",
        string? subtotal = null,
        string? tax = null,
        string? total = "100,00",
        string? fee = null,
        string? currency = "TRY",
        string? paymentHint = "unknown",
        string? category = "Market",
        string? dueDate = null,
        string? installments = null) =>
        new(
          ReceiptDocumentKind.PurchaseReceipt,
          merchant, date, dueDate, subtotal, tax, total, fee, installments,
          currency, paymentHint, category, [], NoUsage);
}
