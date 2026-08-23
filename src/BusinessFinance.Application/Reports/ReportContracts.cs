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

/// <summary>
/// Bir ayın tek bir kapsamdaki gelir/gider tablosu.
/// </summary>
public sealed record ScopeTotalsDto(decimal Income, decimal Expense, decimal Net);

/// <summary>
/// Ayın iki tarafı: işletme ve şahsi. İkisinin toplamı raporun kendi
/// toplamıdır — her gelir/gider kaydı tam olarak bir kapsam taşır ve üçüncü
/// bir "bilinmiyor" değeri yoktur.
/// </summary>
/// <remarks>
/// Yalnız **filtresiz** okumada dolar. Kapsam filtresi verilmişse rapor zaten
/// tek tarafı anlatıyor demektir; kırılım döndürmek, dışlanmış tarafı sıfır
/// olarak gösterip "o tarafta hiç hareket yok" dedirtirdi.
///
/// Kırılım sunucudan gelir çünkü istemci finansal toplamı ikinci kez
/// hesaplamaz: "şahsi çekim" ile "işletme neti" bir çıkarma değil, ayrı ayrı
/// toplanmış iki tablodur.
/// </remarks>
public sealed record MonthlyScopeBreakdownDto(ScopeTotalsDto Business, ScopeTotalsDto Personal);

public sealed record MonthlyReportDto(
    int Year,
    int Month,
    TransactionScope? Scope,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Net,
    CurrencyCode Currency,
    IReadOnlyList<CategoryExpenseDto> CategoryExpenses,
    IReadOnlyList<CategoryExpenseSliceDto> CategoryExpenseSlices,
    IReadOnlyList<AccountBalanceDto> AccountBalances,
    MonthlyScopeBreakdownDto? ScopeBreakdown = null);

public partial interface IFinancialReportRepository
{
    /// <summary>
    /// Bir ayın gelir/gider tablosu. <paramref name="scope"/> boşsa toplam.
    /// </summary>
    /// <remarks>
    /// Kapsam yalnız gelir/gider tarafını böler; hesap bakiyeleri filtreden
    /// etkilenmez (ADR 0013).
    /// </remarks>
    Task<MonthlyReportDto> GetMonthlyAsync(
        Guid userId,
        int year,
        int month,
        TransactionScope? scope,
        CancellationToken cancellationToken);
}
