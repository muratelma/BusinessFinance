using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Taxes;

/// <summary>
/// Vergi türünün, sıklığın ve ay kümesinin sözleşme değerleri. Sunucu kararlı
/// makine değerleri gönderir; türün adını ve ipucunu istemci kurar.
/// </summary>
public static class TaxContractValues
{
    private static readonly Dictionary<TaxKind, string> TaxKindValues = new()
    {
        [TaxKind.SocialSecurityPremium] = "social-security-premium",
        [TaxKind.VatReturn] = "vat-return",
        [TaxKind.WithholdingReturn] = "withholding-return",
        [TaxKind.AdvanceTax] = "advance-tax",
        [TaxKind.AnnualIncomeTax] = "annual-income-tax",
        [TaxKind.PropertyTax] = "property-tax",
        [TaxKind.MotorVehicleTax] = "motor-vehicle-tax",
        [TaxKind.AdvertisingTax] = "advertising-tax",
        [TaxKind.Custom] = "custom"
    };

    private static readonly Dictionary<RecurrenceFrequency, string> FrequencyValues = new()
    {
        [RecurrenceFrequency.Daily] = "daily",
        [RecurrenceFrequency.Weekly] = "weekly",
        [RecurrenceFrequency.Monthly] = "monthly",
        [RecurrenceFrequency.Quarterly] = "quarterly",
        [RecurrenceFrequency.Yearly] = "yearly",
        [RecurrenceFrequency.SelectedMonths] = "selected-months"
    };

    public static string TaxKindValue(TaxKind taxKind) => TaxKindValues[taxKind];

    public static string? OptionalTaxKindValue(TaxKind? taxKind) =>
        taxKind is TaxKind value ? TaxKindValues[value] : null;

    public static bool TryParseTaxKind(string? value, out TaxKind? taxKind)
    {
        taxKind = null;
        if (value is null) return true;
        foreach (var pair in TaxKindValues)
        {
            if (string.Equals(pair.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                taxKind = pair.Key;
                return true;
            }
        }

        return false;
    }

    public static string FrequencyValue(RecurrenceFrequency frequency) => FrequencyValues[frequency];

    public static bool TryParseFrequency(string? value, out RecurrenceFrequency frequency)
    {
        foreach (var pair in FrequencyValues)
        {
            if (string.Equals(pair.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                frequency = pair.Key;
                return true;
            }
        }

        frequency = default;
        return false;
    }

    /// <summary>
    /// Ay listesini (1–12) bit kümesine çevirir; boş liste boş küme demektir.
    /// Aralık dışı ya da tekrarlanan ay geçersizdir.
    /// </summary>
    public static bool TryParseMonths(IReadOnlyList<int>? months, out int? mask)
    {
        mask = null;
        if (months is null || months.Count == 0) return true;
        var value = 0;
        foreach (var month in months)
        {
            if (month is < 1 or > 12) return false;
            var bit = 1 << (month - 1);
            if ((value & bit) != 0) return false;
            value |= bit;
        }

        mask = value;
        return true;
    }

    public static IReadOnlyList<int>? MonthsValue(int? mask) =>
        mask is int value
            ? [.. Enumerable.Range(1, 12).Where(month => RecurringTransaction.IsMonthSelected(value, month))]
            : null;
}
