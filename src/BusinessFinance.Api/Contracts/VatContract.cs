using BusinessFinance.Application.Taxes;

namespace BusinessFinance.Api.Contracts;

/// <summary>
/// Bir kaydın üstünde taşınan KDV'nin sözleşme biçimi.
/// </summary>
/// <remarks>
/// ADR 0016: sunucu hiçbir vergi tutarını hesaplamaz. İki alan da isteğe
/// bağlıdır ve ikisi de boşsa kayıt KDV taşımaz — cevapta alan <c>null</c>
/// döner, içi boş bir nesne dönmez.
///
/// Oran da tutar gibi dört ondalıklı <b>string</b>'dir (0,20 → <c>"0.2000"</c>):
/// para sözleşmesinin hassasiyet kuralı burada da geçerlidir.
/// </remarks>
public sealed record VatContract(string? Rate, string? Amount);

internal static class VatContractMapper
{
    public static VatContract? ToContract(VatDto? vat) => vat is null
        ? null
        : new VatContract(
            FinanceContract.OptionalMoney(vat.Rate),
            FinanceContract.OptionalMoney(vat.Amount));

    /// <summary>
    /// İstekteki iki isteğe bağlı alanı okur. Ayrıştırılamayan bir değer
    /// <c>false</c> döner; ikisi de boşsa sonuç <c>null</c>'dır.
    /// </summary>
    public static bool TryParse(string? rate, string? amount, out VatDto? vat)
    {
        vat = null;
        if (!FinanceContract.TryParseOptionalAmount(rate, out var parsedRate) ||
            !FinanceContract.TryParseOptionalAmount(amount, out var parsedAmount))
        {
            return false;
        }

        vat = parsedRate is null && parsedAmount is null
            ? null
            : new VatDto(parsedRate, parsedAmount);
        return true;
    }
}
