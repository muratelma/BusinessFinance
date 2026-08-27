namespace BusinessFinance.Domain;

public sealed record CreditCardStatementPeriod(
    DateOnly PeriodStart,
    DateOnly ClosingDate,
    DateOnly DueDate)
{
    public static CreditCardStatementPeriod ForClosingMonth(
        CreditCard card,
        int year,
        int month)
    {
        ArgumentNullException.ThrowIfNull(card);
        if (year is < 2000 or > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var closingDate = new DateOnly(year, month, card.StatementClosingDay);
        var periodStart = closingDate.AddMonths(-1).AddDays(1);
        var dueMonth = card.PaymentDueDay > card.StatementClosingDay
            ? new DateOnly(year, month, 1)
            : new DateOnly(year, month, 1).AddMonths(1);
        var dueDate = dueMonth.AddDays(card.PaymentDueDay - 1);
        return new CreditCardStatementPeriod(periodStart, closingDate, dueDate);
    }
}

public sealed record CreditCardStatement(
    Guid CreditCardId,
    int Year,
    int Month,
    DateOnly PeriodStart,
    DateOnly ClosingDate,
    DateOnly DueDate,
    decimal PreviousBalance,
    decimal PeriodCharges,
    decimal PaymentsThroughClosing,
    decimal StatementBalance,
    decimal PaymentsAfterClosing,
    decimal RemainingBalance,
    decimal MinimumPayment,
    decimal RemainingMinimumPayment,
    decimal MinimumPaymentRate,
    CurrencyCode Currency,
    StatementPaymentStatus PaymentStatus)
{
    public static CreditCardStatement Create(
        CreditCard card,
        int year,
        int month,
        DateOnly asOfDate,
        decimal previousBalance,
        decimal periodCharges,
        decimal paymentsThroughClosing,
        decimal paymentsAfterClosing)
    {
        ArgumentNullException.ThrowIfNull(card);
        var period = CreditCardStatementPeriod.ForClosingMonth(card, year, month);
        if (asOfDate < period.ClosingDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(asOfDate),
                "Statement cannot be calculated before its closing date.");
        }

        // Devir **negatif olabilir**: geçen dönemden kalan alacaklı bakiye bu
        // dönemin ekstresinden düşer. Kırpmak, kartında parası olan kullanıcıya
        // borçlu olmadığı bir tutarı ödetirdi.
        ValidateNonNegative(periodCharges, nameof(periodCharges));
        ValidateNonNegative(paymentsThroughClosing, nameof(paymentsThroughClosing));
        ValidateNonNegative(paymentsAfterClosing, nameof(paymentsAfterClosing));

        // Ekstre borcunun tabanı **duruyor**: bu "ne kadar ödemeliyim"in
        // cevabıdır ve negatif olamaz. Kartın alacaklı bakiyesi ayrı bir
        // sorunun cevabıdır (`CurrentDebt`) ve orada kırpılmaz.
        var statementBalance = Math.Max(
            0m,
            previousBalance + periodCharges - paymentsThroughClosing);
        var remaining = Math.Max(0m, statementBalance - paymentsAfterClosing);

        // Asgari de ödemelerle birlikte eriyor: kesim sonrası yapılan ödeme
        // önce asgariden düşer. Aksi hâlde asgarinin yarısını ödeyen kullanıcı
        // ekranda hâlâ tam asgariyi borçluymuş gibi görünürdü.
        //
        // Ayrıca bir üst sınıra gerek yok: asgari zaten ekstre borcunu
        // aşamadığı için kalan asgari, kalan borcu hiçbir girdide geçemez.
        var minimumPayment = card.CalculateMinimumPayment(statementBalance);
        var remainingMinimum = Math.Max(0m, minimumPayment - paymentsAfterClosing);
        var status = remaining == 0m
            ? StatementPaymentStatus.Paid
            : asOfDate > period.DueDate
                ? StatementPaymentStatus.Overdue
                : StatementPaymentStatus.Open;

        return new CreditCardStatement(
            card.Id,
            year,
            month,
            period.PeriodStart,
            period.ClosingDate,
            period.DueDate,
            previousBalance,
            periodCharges,
            paymentsThroughClosing,
            statementBalance,
            paymentsAfterClosing,
            remaining,
            minimumPayment,
            remainingMinimum,
            card.MinimumPaymentRate,
            card.Limit.Currency,
            status);
    }

    private static void ValidateNonNegative(decimal value, string parameterName)
    {
        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
