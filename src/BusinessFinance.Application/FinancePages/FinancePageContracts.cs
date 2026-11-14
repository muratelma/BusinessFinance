using BusinessFinance.Domain;
using BusinessFinance.Application.FinancialActivities;

namespace BusinessFinance.Application.FinancePages;

public sealed record CategoryPageQuery(int Year, int Month, TransactionType Type,
    TransactionScope? Scope = null, string? Source = null, string? Person = null, int Offset = 0);
public sealed record AccountPageQuery(int Year, int Month, string? Direction = null,
    string? Selection = null, int Offset = 0);
public sealed record PageOption(string Key, string Group, string? Name, decimal Amount,
    string? Kind = null, Guid? Id = null);
public sealed record CategorySlice(Guid? Id, string? Name, decimal Amount, decimal Share, decimal BarRatio);
/// <summary>Özetin altı aylık çubuğu: kaydı olmayan ay <c>HasData=false</c> taşır ve çizilmez.</summary>
public sealed record RecentMonth(int Year, int Month, decimal Amount, decimal BarRatio, bool HasData);
/// <summary>
/// <c>PreviousChange</c> geçen aya göre işaretli tam yüzdedir (60 = %60 fazla); geçen ay sıfırsa
/// boştur. İkisi de sayfanın kapsam, kaynak ve kişi seçimini izler.
/// </summary>
public sealed record CategorySummary(int Year, int Month, string Direction, string? Scope,
    decimal Total, decimal PreviousTotal, int CategoryCount, IReadOnlyList<CategorySlice> Categories,
    IReadOnlyList<PageOption> Sources, decimal? PreviousChange = null,
    IReadOnlyList<RecentMonth>? RecentMonths = null);
public sealed record PageActivity(FinancialActivityRow Activity, decimal Amount, string? Part = null,
    decimal? PartAmount = null, decimal? ParentAmount = null, string? State = null,
    DateOnly? SettledOn = null, string? SettledSource = null, bool ShowScope = false,
    DateOnly? FirstSaleDate = null, DateOnly? LastSaleDate = null, string? Scope = null,
    int? InstallmentSequence = null, int? InstallmentCount = null);
public sealed record CategoryPage(CategorySummary Summary, Guid? CategoryId, string? Name,
    decimal WholeTotal, bool ShowScope, bool HasPeople, IReadOnlyList<PageOption> People,
    IReadOnlyList<PageActivity> Items, int TotalCount, int Offset, bool HasMore);
public sealed record MonthValue(int Month, decimal Amount, decimal BarRatio, bool Enabled);
public sealed record YearMonths(int Year, IReadOnlyList<MonthValue> Months);
public sealed record TransitSummary(decimal Total, int Count, PageActivity? Nearest);
public sealed record TransitPage(Guid AccountId, string Name, decimal Total, int TotalCount,
    IReadOnlyList<PageActivity> Items, int Offset, bool HasMore);
public sealed record AccountPage(Guid AccountId, string Name, string Type, string? DefaultScope,
    bool IsActive, int Year, int Month, decimal Balance, decimal CurrentBalance, bool IsCurrentMonth,
    decimal Inflow, decimal Outflow, decimal Net, decimal SelectionTotal,
    IReadOnlyList<PageOption> Options, TransitSummary Transit, IReadOnlyList<PageActivity> Items,
    int TotalCount, int Offset, bool HasMore);
public sealed record AccountListItem(Guid Id, string Name, string Type, string? DefaultScope,
    bool IsActive, decimal Balance);
/// <summary>
/// Paylar tam yüzdedir (58 = %58) ve yalnız toplam artı, parçaların hiçbiri eksi değilken
/// doludur; eksiye düşmüş bir hesap varken "toplamın yüzde kaçı" sorusunun cevabı yoktur.
/// </summary>
public sealed record AccountsOverview(decimal Total, decimal CashTotal, decimal BankTotal,
    decimal InactiveTotal, IReadOnlyList<AccountListItem> Items,
    decimal? CashShare = null, decimal? BankShare = null, decimal? InactiveShare = null);
/// <summary>
/// Kart hareketlerinin türü: <c>charge</c> taksitsiz harcama ve kartla ödenen
/// borç, <c>installment</c> taksidin karta yansıyan harcaması, <c>payment</c>
/// kart ödemesi. Boşsa hepsi.
/// </summary>
public sealed record CardPageQuery(DateOnly? From, DateOnly? To, string? Kind = null,
    TransactionScope? Scope = null, Guid? CategoryId = null, int Offset = 0, int Limit = 50);
/// <summary>
/// Kartın sayfasındaki seçimin özeti ve satırları. <c>Payments</c> seçim ödemeyi
/// dışarıda bırakıyorsa (tür harcama, kapsam ya da kategori seçili) boştur;
/// kapsam ve kategori ödemede yoktur.
/// </summary>
public sealed record CardPageData(decimal Spending, decimal? Payments,
    IReadOnlyList<PageOption> Categories, IReadOnlyList<PageActivity> Items,
    int TotalCount, int Offset, bool HasMore);
/// <summary>Bir ekstrenin dönemi: kesim ayı, ilk gün ve kesim günü.</summary>
public sealed record CardStatementRange(int Year, int Month, DateOnly Start, DateOnly End);
/// <summary>
/// Sayfanın seçili dönemi. <c>Kind</c>: <c>statement</c>, <c>range</c> ya da
/// <c>all</c>. Ekstrede <c>Status</c>: <c>current</c> (kesilmedi), <c>open</c>
/// (kesildi, ödeme bekleniyor), <c>paid</c>, <c>overdue</c>.
/// </summary>
public sealed record CardPagePeriod(string Kind, int? Year, int? Month, DateOnly? Start,
    DateOnly? End, string? Status);
public sealed record CardPage(Guid CardId, string Name, CardPagePeriod Period,
    IReadOnlyList<CardStatementRange> Statements, decimal Spending, decimal? Payments,
    IReadOnlyList<PageOption> Categories, IReadOnlyList<PageActivity> Items,
    int TotalCount, int Offset, bool HasMore);
public sealed record ActivityPart(string Kind, decimal Amount, Guid? CategoryId, string? CategoryName);
public sealed record ActivityParts(IReadOnlyList<ActivityPart> Items);

public interface IFinancePageReader
{
    Task<CategorySummary> CategorySummaryAsync(Guid userId, CategoryPageQuery query, CancellationToken ct);
    Task<CategoryPage?> CategoryAsync(Guid userId, Guid? categoryId, CategoryPageQuery query, CancellationToken ct);
    Task<YearMonths?> CategoryMonthsAsync(Guid userId, Guid? categoryId, bool summary,
        CategoryPageQuery query, CancellationToken ct);
    Task<AccountPage?> AccountAsync(Guid userId, Guid accountId, AccountPageQuery query, CancellationToken ct);
    Task<YearMonths?> AccountMonthsAsync(Guid userId, Guid accountId, int year, CancellationToken ct);
    Task<TransitPage?> TransitAsync(Guid userId, Guid accountId, int offset, CancellationToken ct);
    Task<AccountsOverview> AccountsAsync(Guid userId, CancellationToken ct);
    Task<CardPageData?> CardAsync(Guid userId, Guid cardId, CardPageQuery query, CancellationToken ct);
    Task<ActivityParts?> PartsAsync(Guid userId, FinancialActivityKind kind, Guid activityId, CancellationToken ct);
}
