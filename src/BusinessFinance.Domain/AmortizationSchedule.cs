namespace BusinessFinance.Domain;

/// <summary>
/// Anüite (azalan bakiye) taksit hesabı.
/// </summary>
/// <remarks>
/// <para>
/// Basit faiz yerine anüite seçildi çünkü basit faiz ödenmiş anaparaya da
/// faiz işletir — gerçek kredilerde olmayan bir şey — ve bir taksiti anapara
/// ile faize bölemez. O bölme, faizin gider olarak yazılmasının önkoşuludur.
/// </para>
/// <para>
/// <b>Para kanoniktir, oran türetilmiştir.</b> Oran kanonik olsaydı yuvarlama
/// kullanıcının yazdığı toplamı değiştirirdi: 400,00 girip 399,99 görmek
/// kabul edilemez. Bu yüzden taksit planı daima toplamdan bölünür ve oran
/// yalnız gösterilen/çevrilen bilgidir.
/// </para>
/// <para>
/// Oran <b>nominal yıllıktır</b>: aylık oran = yıllık / 12. Türk banka
/// pratiği böyle ilan eder ("aylık %2,5, yıllık %30"). Bileşik etkiyi içeren
/// efektif yıllık oran (APR) ayrı bir büyüklüktür ve burada hesaplanmaz.
/// </para>
/// </remarks>
public static class AmortizationSchedule
{
    /// <summary>Yıllık oranın yüzde cinsinden üst sınırı.</summary>
    public const decimal MaximumAnnualInterestRate = 1000m;

    /// <summary>Para dört ondalık basamakta tutulur.</summary>
    public const int MoneyDecimals = 4;

    /// <summary>Oran da dört ondalık basamakta tutulur (yüzde cinsinden).</summary>
    public const int RateDecimals = 4;

    // Bisection'ın durma eşiği, oranın yuvarlandığı basamaktan bir kademe
    // küçük. Daha ilerisini kovalamak yuvarlamada kaybolur.
    private const decimal RateSearchTolerance = 0.00001m;

    // Aralık her adımda yarılandığı için 1000 → 0,00001 yaklaşık 27 adım
    // sürer. Sınır, yakınsamayan bir girdinin sessizce yanlış sonuç yerine
    // sonsuz döngü de üretmemesi için var.
    private const int MaximumIterations = 200;

    /// <summary>Bir taksitin anapara ve faiz payı.</summary>
    public readonly record struct InstallmentSplit(decimal Principal, decimal Interest);

    /// <summary>
    /// Anapara, yıllık oran ve taksit sayısından toplam geri ödemeyi hesaplar.
    /// </summary>
    public static decimal TotalRepaymentFor(
        decimal principal,
        decimal annualInterestRate,
        int installmentCount)
    {
        ValidatePrincipal(principal);
        ValidateInstallmentCount(installmentCount);
        ValidateRate(annualInterestRate);

        var monthlyRate = MonthlyRateOf(annualInterestRate);
        if (monthlyRate == 0m)
        {
            return decimal.Round(principal, MoneyDecimals, MidpointRounding.AwayFromZero);
        }

        var payment = PaymentFor(principal, monthlyRate, installmentCount);
        return decimal.Round(payment * installmentCount, MoneyDecimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Anapara, toplam geri ödeme ve taksit sayısından yıllık oranı çözer.
    /// </summary>
    /// <remarks>
    /// Bu yönün kapalı formülü yoktur. Taksit tutarı orana göre kesintisiz
    /// artan bir fonksiyon olduğu için ikiye bölme (bisection) tek köke
    /// yakınsar ve Newton'ın aksine türev veya başlangıç tahmini istemez.
    /// </remarks>
    public static decimal AnnualInterestRateFor(
        decimal principal,
        decimal totalRepayment,
        int installmentCount)
    {
        ValidatePrincipal(principal);
        ValidateInstallmentCount(installmentCount);
        if (totalRepayment < principal)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalRepayment),
                totalRepayment,
                "Total repayment cannot be below principal.");
        }

        if (totalRepayment == principal)
        {
            return 0m;
        }

        var target = totalRepayment / installmentCount;
        var upperBound = MaximumAnnualInterestRate;
        if (PaymentFor(principal, MonthlyRateOf(upperBound), installmentCount) < target)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalRepayment),
                totalRepayment,
                $"Total repayment implies an annual rate above {MaximumAnnualInterestRate}%.");
        }

        var lowerBound = 0m;
        for (var iteration = 0; iteration < MaximumIterations; iteration++)
        {
            if (upperBound - lowerBound < RateSearchTolerance)
            {
                break;
            }

            var middle = (lowerBound + upperBound) / 2m;
            if (PaymentFor(principal, MonthlyRateOf(middle), installmentCount) < target)
            {
                lowerBound = middle;
            }
            else
            {
                upperBound = middle;
            }
        }

        return decimal.Round((lowerBound + upperBound) / 2m, RateDecimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Gerçekleşmiş taksit tutarlarını anapara ve faiz paylarına böler.
    /// </summary>
    /// <remarks>
    /// Taksit tutarları girdi olarak alınır, yeniden hesaplanmaz: plan
    /// toplamdan bölündüğü için tek gerçek kaynak odur. Son taksit kalan
    /// anaparanın tamamını üstlenir; böylece anapara payları toplamı anaparaya,
    /// faiz payları toplamı da toplam faize <b>birebir</b> eşit kalır.
    /// </remarks>
    public static IReadOnlyList<InstallmentSplit> Split(
        decimal principal,
        decimal annualInterestRate,
        IReadOnlyList<decimal> installmentAmounts)
    {
        ValidatePrincipal(principal);
        ArgumentNullException.ThrowIfNull(installmentAmounts);
        ValidateInstallmentCount(installmentAmounts.Count);
        ValidateRate(annualInterestRate);
        if (installmentAmounts.Sum() < principal)
        {
            throw new ArgumentException(
                "Installment amounts cannot sum below the principal.",
                nameof(installmentAmounts));
        }

        var monthlyRate = MonthlyRateOf(annualInterestRate);
        var splits = new InstallmentSplit[installmentAmounts.Count];
        var outstanding = principal;
        for (var index = 0; index < installmentAmounts.Count; index++)
        {
            var amount = installmentAmounts[index];
            if (index == installmentAmounts.Count - 1)
            {
                splits[index] = new InstallmentSplit(outstanding, amount - outstanding);
                outstanding = 0m;
                continue;
            }

            var interest = decimal.Round(
                outstanding * monthlyRate, MoneyDecimals, MidpointRounding.AwayFromZero);

            // Faiz taksitin tamamını yerse anapara hiç azalmaz ve borç büyür.
            // Toplam anaparadan çözülen oranla bu doğmaz; yine de sessizce
            // negatif anapara payı üretmek yerine payı sıfırda tutuyoruz.
            var principalPart = amount - interest;
            if (principalPart < 0m)
            {
                principalPart = 0m;
                interest = amount;
            }

            if (principalPart > outstanding)
            {
                principalPart = outstanding;
                interest = amount - principalPart;
            }

            splits[index] = new InstallmentSplit(principalPart, interest);
            outstanding -= principalPart;
        }

        return splits;
    }

    /// <summary>Yıllık yüzdeden aylık ondalık orana.</summary>
    public static decimal MonthlyRateOf(decimal annualInterestRate) =>
        annualInterestRate / 100m / 12m;

    private static decimal PaymentFor(decimal principal, decimal monthlyRate, int installmentCount)
    {
        if (monthlyRate == 0m)
        {
            return principal / installmentCount;
        }

        var discount = DiscountFactor(monthlyRate, installmentCount);
        return principal * monthlyRate / (1m - discount);
    }

    // (1+i)^-n. Doğrudan (1+i)^n hesaplanamaz: i=0,8333 ve n=360 iken sonuç
    // 10^94 civarıdır ve decimal taşar. Bunun yerine 1/(1+i) çarpanı
    // tekrarlanır — değer daima (0,1] aralığında kalır, taşma imkânsızdır ve
    // çok küçük sonuçlar sıfıra doğru sorunsuz erir.
    private static decimal DiscountFactor(decimal monthlyRate, int installmentCount)
    {
        var step = 1m / (1m + monthlyRate);
        var discount = 1m;
        for (var index = 0; index < installmentCount; index++)
        {
            discount *= step;
        }

        return discount;
    }

    private static void ValidatePrincipal(decimal principal)
    {
        if (principal <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(principal), principal, "Principal must be greater than zero.");
        }
    }

    private static void ValidateInstallmentCount(int installmentCount)
    {
        if (installmentCount is < 1 or > DebtAgreement.MaximumInstallments)
        {
            throw new ArgumentOutOfRangeException(
                nameof(installmentCount),
                installmentCount,
                $"Installment count must be between 1 and {DebtAgreement.MaximumInstallments}.");
        }
    }

    private static void ValidateRate(decimal annualInterestRate)
    {
        if (annualInterestRate is < 0m or > MaximumAnnualInterestRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(annualInterestRate),
                annualInterestRate,
                $"Annual interest rate must be between 0 and {MaximumAnnualInterestRate}.");
        }
    }
}
