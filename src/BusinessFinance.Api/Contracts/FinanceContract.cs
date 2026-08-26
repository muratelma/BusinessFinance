using System.Globalization;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Contracts;

internal static class FinanceContract
{
    private const NumberStyles MoneyStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public static bool TryParseAmount(string? value, out decimal amount)
    {
        return decimal.TryParse(
                   value,
                   MoneyStyles,
                   CultureInfo.InvariantCulture,
                   out amount) &&
               decimal.Round(amount, 4) == amount;
    }

    /// <summary>
    /// İsteğe bağlı bir para/oran alanı. Boş değer meşrudur ve "yok" demektir.
    /// </summary>
    public static bool TryParseOptionalAmount(string? value, out decimal? amount)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            amount = null;
            return true;
        }

        if (TryParseAmount(value, out var parsed))
        {
            amount = parsed;
            return true;
        }

        amount = null;
        return false;
    }

    public static bool TryParseDate(string? value, out DateOnly date)
    {
        return DateOnly.TryParseExact(
            value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    public static string? OptionalMoney(decimal? value) =>
        value is decimal amount ? Money(amount) : null;

    public static string Money(decimal value) => value.ToString("0.0000", CultureInfo.InvariantCulture);
    public static string Date(DateOnly value) => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    public static string AccountTypeValue(AccountType value) => value.ToString().ToLowerInvariant();
    public static string CategoryTypeValue(CategoryType value) => value.ToString().ToLowerInvariant();
    public static string TransactionTypeValue(TransactionType value) => value.ToString().ToLowerInvariant();

    /// <summary>
    /// Kapsamin kararli makine degeri: <c>business</c> veya <c>personal</c>.
    /// Kullaniciya gosterilecek cumleyi istemci uretir.
    /// </summary>
    public static string ScopeValue(TransactionScope value) => value.ToString().ToLowerInvariant();

    /// <summary>
    /// İsteğe bağlı varsayılan kapsam. Boş değer meşrudur ve "bu kaynak kapsam
    /// belirlemiyor" demektir; tanınmayan bir metin ise reddedilir.
    /// </summary>
    public static bool TryParseOptionalScope(string? value, out TransactionScope? scope)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            scope = null;
            return true;
        }

        if (TryParseScope(value, out var parsed))
        {
            scope = parsed;
            return true;
        }

        scope = null;
        return false;
    }

    public static string? OptionalScopeValue(TransactionScope? value) =>
        value is TransactionScope scope ? ScopeValue(scope) : null;

    public static bool TryParseScope(string? value, out TransactionScope scope) =>
        Enum.TryParse(value, true, out scope) &&
        scope is TransactionScope.Business or TransactionScope.Personal;
}
