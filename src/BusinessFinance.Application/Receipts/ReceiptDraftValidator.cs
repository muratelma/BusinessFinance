using System.Globalization;

namespace BusinessFinance.Application.Receipts;

/// <summary>
/// Turns what the model said into what the form may show. Six passes, in order:
/// shape, type, meaning, consistency, ownership, and a per-field verdict.
///
/// A plain class rather than a port: it has no outside dependency, so the use
/// case reads the categories and the clock and hands them in. That keeps every
/// rule here testable as a function of its inputs.
///
/// One rule runs through all six passes: a field that fails becomes null and
/// raises a warning. It is never repaired, never rounded into shape, never
/// guessed. A blank field tells the user where to look; an invented one does
/// the opposite.
/// </summary>
public static class ReceiptDraftValidator
{
    /// <summary>
    /// Above this a reading is a misread, not a purchase. Chosen high enough not
    /// to argue with a real receipt and low enough to catch a decimal point that
    /// went missing.
    /// </summary>
    private const decimal MaximumPlausibleTotal = 10_000_000m;

    /// <inheritdoc cref="DebtAgreement.MaximumInstallments" />
    private const int MaximumInstallmentCount = 360;

    private const int MaximumAgeInYears = 10;

    /// <summary>
    /// Rounding slack for the subtotal + tax check. Receipts round per VAT rate,
    /// so a couple of kuruş of drift is arithmetic, not a misread.
    /// </summary>
    private const decimal TotalsTolerance = 0.05m;

    private const string ExpectedCurrency = "TRY";

    /// <summary>
    /// Day first, because the receipts are Turkish: 01.02.2026 is 1 February.
    /// ISO is accepted as well and listed first, since that is what the prompt
    /// asks for — but asking is not enforcing, and a live call showed the model
    /// answering in dd.MM.yyyy regardless.
    /// </summary>
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd", "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy"
    ];

    public static ReceiptDraft Validate(
        RawReceiptReading reading,
        IReadOnlyList<ReceiptCategoryOption> categories,
        DateOnly today)
    {
        var warnings = new List<ReceiptWarning>();

        // Pass 1 — shape. The model reports what it could not read; those fields
        // are treated as absent before anything tries to parse them.
        var unreadable = new HashSet<string>(reading.UnreadableFields, StringComparer.OrdinalIgnoreCase);
        foreach (var field in reading.UnreadableFields)
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.FieldUnreadable,
                $"Fişte '{FriendlyName(field)}' okunamadı."));
        }

        var counterparty = unreadable.Contains("counterpartyName")
            ? null
            : reading.CounterpartyName;

        // Pass 2 — type.
        var (purchasedAt, purchasedAtState) = ReadDate(
            unreadable.Contains("purchasedAt") ? null : reading.PurchasedAt, today, warnings);
        // Son ödeme tarihi belgenin kendi tarihi DEĞİLDİR ve kuralları da
        // aynı olamaz: bir fiş yarından olamaz ama bir faturanın vadesi tam da
        // gelecektedir. `ReadDate`'i kullanmak, her ödenmemiş faturanın
        // vadesini "gelecekte" diye düşürürdü.
        var (dueDate, dueDateState) = ReadDueDate(
            unreadable.Contains("dueDate") ? null : reading.DueDate, warnings);
        var (total, totalState) = ReadAmount(
            unreadable.Contains("totalAmount") ? null : reading.TotalAmount, warnings);
        var subtotal = TryAmount(reading.SubtotalAmount);
        // Masraf, taşınan tutardan ayrı okunur ve ona ASLA eklenmez: dekonttaki
        // 5.000 ile 4,50 birleştirilince hiç harcanmamış bir 5.004,50 gideri
        // üretilmişti. Transfer parayı taşır, harcayan yalnız bu ücrettir.
        var (fee, feeState) = reading.FeeAmount is null
            ? (null, ReceiptFieldState.Missing)
            : ReadAmount(
                unreadable.Contains("feeAmount") ? null : reading.FeeAmount,
                warnings);
        var tax = TryAmount(reading.TaxAmount);
        // Taksit sayısı para değil sayıdır ve sınırı domain'den gelir: bir
        // plan en fazla 360 taksit taşıyabilir. Anlaşılmayan değer sessizce
        // 1'e düşmez — düşseydi taksitli bir alışveriş tek seferlik tam tutar
        // gideri olarak yazılırdı, bu grubun tam olarak önlediği şey.
        var installments = ReadInstallmentCount(
            unreadable.Contains("installmentCount") ? null : reading.InstallmentCount);

        // Pass 3 — meaning is folded into the readers above: a negative total or a
        // date in the future is dropped there rather than carried and re-checked.

        // Pass 4 — consistency. Free evidence: if the receipt printed a subtotal
        // and a tax and they do not reach the total, one of the three was misread.
        // Which one is unknowable, so the total is flagged rather than replaced.
        if (total is { } amount && subtotal is { } sub && tax is { } vat &&
            Math.Abs(sub + vat - amount) > TotalsTolerance)
        {
            totalState = ReceiptFieldState.Suspect;
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.TotalsDoNotAddUp,
                "Ara toplam ve KDV, genel toplamı tutmuyor. Tutarı kontrol edin."));
        }

        var currency = Normalize(reading.CurrencyCode)?.ToUpperInvariant();
        if (currency is not null && currency != ExpectedCurrency)
        {
            // The budget is kept in one currency. A foreign total is a real number
            // for the wrong unit, which is exactly what "suspect" is for.
            if (totalState == ReceiptFieldState.Read)
                totalState = ReceiptFieldState.Suspect;
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.CurrencyUnexpected,
                $"Fiş {currency} para biriminde görünüyor. Tutarı kontrol edin."));
        }

        // Pass 5 — ownership. The model picks from a closed set, but the answer is
        // still resolved against the user's own categories: a name that does not
        // belong to this budget resolves to nothing rather than to someone else's
        // category.
        var (categoryId, categoryName, categoryState) = ResolveCategory(
            unreadable.Contains("categoryName") ? null : reading.CategoryName,
            categories,
            warnings);

        // Pass 6 — verdict per field.
        return new ReceiptDraft(
            reading.DocumentKind,
            counterparty,
            counterparty is null ? ReceiptFieldState.Missing : ReceiptFieldState.Read,
            purchasedAt,
            purchasedAtState,
            installments,
            dueDate,
            dueDateState,
            total,
            totalState,
            fee,
            feeState,
            currency,
            ReadPaymentHint(reading.PaymentMethodHint),
            categoryId,
            categoryName,
            categoryState,
            warnings);
    }

    private static (decimal? Value, ReceiptFieldState State) ReadAmount(
        string? raw,
        List<ReceiptWarning> warnings)
    {
        if (Normalize(raw) is null)
            return (null, ReceiptFieldState.Missing);

        var parsed = TryAmount(raw);
        if (parsed is null)
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.AmountUnparsed,
                "Fişteki toplam tutar sayıya çevrilemedi."));
            return (null, ReceiptFieldState.Missing);
        }

        if (parsed <= 0 || parsed > MaximumPlausibleTotal)
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.AmountOutOfRange,
                "Fişten okunan tutar makul aralıkta değil."));
            return (null, ReceiptFieldState.Missing);
        }

        return (parsed, ReceiptFieldState.Read);
    }

    /// <summary>
    /// Reads an amount written by a model that was asked for "1234.56" but may
    /// answer "1.234,56", "1,234.56" or "847,50".
    ///
    /// Separator rules, in order:
    /// both kinds present  -> the last one is the decimal point, the other groups;
    /// one kind, repeated  -> grouping;
    /// one kind, exactly 3 digits after -> grouping (money with three decimals is
    ///                        far rarer than a thousands group);
    /// otherwise           -> decimal point.
    /// </summary>
    private static decimal? TryAmount(string? raw)
    {
        var value = Normalize(raw);
        if (value is null)
            return null;

        value = new string(value.Where(c => char.IsDigit(c) || c is '.' or ',' or '-').ToArray());
        if (value.Length == 0)
            return null;

        var lastDot = value.LastIndexOf('.');
        var lastComma = value.LastIndexOf(',');

        if (lastDot >= 0 && lastComma >= 0)
        {
            var decimalSeparator = lastDot > lastComma ? '.' : ',';
            var groupSeparator = decimalSeparator == '.' ? ',' : '.';
            value = value.Replace(groupSeparator.ToString(), string.Empty)
                .Replace(decimalSeparator, '.');
        }
        else if (lastDot >= 0 || lastComma >= 0)
        {
            var separator = lastDot >= 0 ? '.' : ',';
            var position = lastDot >= 0 ? lastDot : lastComma;
            var occurrences = value.Count(c => c == separator);
            var digitsAfter = value.Length - position - 1;

            value = occurrences > 1 || digitsAfter == 3
                ? value.Replace(separator.ToString(), string.Empty)
                : value.Replace(separator, '.');
        }

        return decimal.TryParse(
            value,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out var amount)
            ? amount
            : null;
    }

    /// <summary>
    /// Fişte yazan taksit sayısı. İki ve üstü anlamlıdır; "1 taksit" tek
    /// çekimdir ve plan üretmez.
    /// </summary>
    private static int? ReadInstallmentCount(string? raw)
    {
        var value = Normalize(raw);
        if (value is null) return null;
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var count))
            return null;
        return count is >= 2 and <= MaximumInstallmentCount ? count : null;
    }

    /// <summary>
    /// Vade tarihi. Geçmişte olması da meşrudur — gecikmiş bir fatura hâlâ
    /// ödenmemiştir — bu yüzden yalnız okunamayan değer düşer.
    /// </summary>
    private static (DateOnly? Value, ReceiptFieldState State) ReadDueDate(
        string? raw,
        List<ReceiptWarning> warnings)
    {
        var value = Normalize(raw);
        if (value is null)
            return (null, ReceiptFieldState.Missing);

        if (!DateOnly.TryParseExact(
                value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.DateUnparsed,
                "Faturadaki son ödeme tarihi anlaşılamadı."));
            return (null, ReceiptFieldState.Missing);
        }

        return (date, ReceiptFieldState.Read);
    }

    private static (DateOnly? Value, ReceiptFieldState State) ReadDate(
        string? raw,
        DateOnly today,
        List<ReceiptWarning> warnings)
    {
        var value = Normalize(raw);
        if (value is null)
            return (null, ReceiptFieldState.Missing);

        if (!DateOnly.TryParseExact(
                value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.DateUnparsed,
                "Fişteki tarih anlaşılamadı."));
            return (null, ReceiptFieldState.Missing);
        }

        if (date > today)
        {
            // A receipt cannot be from tomorrow. Keeping it would post an expense
            // into a month that has not happened.
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.DateInFuture,
                "Fişten okunan tarih gelecekte görünüyor."));
            return (null, ReceiptFieldState.Missing);
        }

        if (date < today.AddYears(-MaximumAgeInYears))
        {
            // Kept, not dropped: a ten-year-old receipt is unusual but real, and
            // a correctly read date that disappears makes the user retype what
            // the model already got right. `suspect` says exactly this much —
            // the value is there and it needs a look.
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.DateTooOld,
                "Fişten okunan tarih çok eski görünüyor. Kontrol edin."));
            return (date, ReceiptFieldState.Suspect);
        }

        return (date, ReceiptFieldState.Read);
    }

    private static (Guid? Id, string? Name, ReceiptFieldState State) ResolveCategory(
        string? raw,
        IReadOnlyList<ReceiptCategoryOption> categories,
        List<ReceiptWarning> warnings)
    {
        var value = Normalize(raw);
        if (value is null)
            return (null, null, ReceiptFieldState.Missing);

        var match = categories.FirstOrDefault(
            option => string.Equals(option.Name.Trim(), value, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            warnings.Add(new ReceiptWarning(
                ReceiptWarnings.CategoryUnknown,
                "Önerilen kategori sizin listenizde yok; kategoriyi kendiniz seçin."));
            return (null, null, ReceiptFieldState.Missing);
        }

        return (match.Id, match.Name, ReceiptFieldState.Read);
    }

    private static ReceiptPaymentHint ReadPaymentHint(string? raw) => Normalize(raw) switch
    {
        "cash" => ReceiptPaymentHint.Cash,
        "credit_card" => ReceiptPaymentHint.CreditCard,
        "debit_card" => ReceiptPaymentHint.DebitCard,
        "card" => ReceiptPaymentHint.Card,
        _ => ReceiptPaymentHint.Unknown
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string FriendlyName(string field) => field switch
    {
        "counterpartyName" => "işletme adı",
        "purchasedAt" => "tarih",
        "totalAmount" => "toplam tutar",
        "subtotalAmount" => "ara toplam",
        "feeAmount" => "işlem ücreti",
        "taxAmount" => "KDV",
        "currencyCode" => "para birimi",
        "categoryName" => "kategori",
        _ => field
    };
}
