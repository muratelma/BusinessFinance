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

    public static bool TryParseDate(string? value, out DateOnly date)
    {
        return DateOnly.TryParseExact(
            value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    public static string Money(decimal value) => value.ToString("0.0000", CultureInfo.InvariantCulture);
    public static string Date(DateOnly value) => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    public static string AccountTypeValue(AccountType value) => value.ToString().ToLowerInvariant();
    public static string CategoryTypeValue(CategoryType value) => value.ToString().ToLowerInvariant();
    public static string TransactionTypeValue(TransactionType value) => value.ToString().ToLowerInvariant();
}
