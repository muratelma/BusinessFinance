using BusinessFinance.Domain;

namespace BusinessFinance.Application.Reports;

public sealed record CategoryExpenseDto(Guid CategoryId, string CategoryName, decimal Amount);

/// <summary>
/// Kategori dağılımının halka grafiğe sığan hâli: en büyük birkaç kategori ve
/// geri kalanının toplamı.
/// </summary>
/// <remarks>
/// Bu gruplama sunucuda yapılıyor çünkü "Diğer" bir **finansal toplam**;
/// istemci parayı ikinci kez hesaplamaz. Tam liste
/// <see cref="MonthlyReportDto.CategoryExpenses"/>'te durmaya devam ediyor —
/// bu alan onun yerine geçmez, sunum için daraltılmış bir görünümüdür.
///
/// <see cref="CategoryId"/> yalnız "Diğer" satırında <c>null</c>'dır: o satır
/// bir kategori değil, birden çoğunun toplamıdır ve uydurma bir kimlik
/// verilseydi istemci onunla filtreleyip boş sonuç alırdı.
/// </remarks>
public sealed record CategoryExpenseSliceDto(
    Guid? CategoryId,
    string CategoryName,
    decimal Amount);
public sealed record AccountBalanceDto(Guid AccountId, string AccountName, decimal Balance, AccountType Type);
public sealed record MonthlyReportDto(
    int Year,
    int Month,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Net,
    CurrencyCode Currency,
    IReadOnlyList<CategoryExpenseDto> CategoryExpenses,
    IReadOnlyList<CategoryExpenseSliceDto> CategoryExpenseSlices,
    IReadOnlyList<AccountBalanceDto> AccountBalances);

public partial interface IFinancialReportRepository
{
    Task<MonthlyReportDto> GetMonthlyAsync(
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken);
}
