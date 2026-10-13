using BusinessFinance.Domain;

namespace BusinessFinance.Application.Categories;

public static class CategoryRoles
{
    public static string? Value(CategoryRole? role) => role switch
    {
        CategoryRole.InterestExpense => "interest-expense",
        CategoryRole.InterestIncome => "interest-income",
        CategoryRole.CashDifference => "cash-difference",
        CategoryRole.BankFee => "bank-fee",
        _ => null
    };

    public static (string Name, CategoryType Type) Default(CategoryRole role) => role switch
    {
        CategoryRole.InterestExpense => ("Faiz ve finansman gideri", CategoryType.Expense),
        CategoryRole.InterestIncome => ("Faiz geliri", CategoryType.Income),
        CategoryRole.CashDifference => ("Kasa farkı", CategoryType.Expense),
        CategoryRole.BankFee => ("Banka masrafı", CategoryType.Expense),
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };

    // Yalnız eski veri ve rolsüz yedeklerin yükseltme eşlemesi.
    public static CategoryRole? FromLegacy(string name, CategoryType type) =>
        (NameKeys.Of(name), type) switch
        {
            ("faizvefinansmangideri", CategoryType.Expense) => CategoryRole.InterestExpense,
            ("faizgeliri", CategoryType.Income) => CategoryRole.InterestIncome,
            ("kasafarki", CategoryType.Expense) => CategoryRole.CashDifference,
            ("bankaveposkomisyonu" or "bankamasrafi", CategoryType.Expense) => CategoryRole.BankFee,
            _ => null
        };

    public static async Task<Category> FindOrOpenAsync(
        this ICategoryRepository repository, Guid userId, CategoryRole role,
        CancellationToken cancellationToken)
        => await repository.FindOrOpenRoleAsync(userId, role, cancellationToken);

    public static async Task<Category> FindOrOpenCoreAsync(
        ICategoryRepository repository, Guid userId, CategoryRole role,
        CancellationToken cancellationToken)
    {
        var (name, type) = Default(role);
        var categories = await repository.ListAsync(userId, type, null, cancellationToken);
        var existing = categories.SingleOrDefault(category => category.Role == role);
        if (existing is not null) return existing;
        // Aynı adlı özel kategori korunur; sistem kategorisi onun yerine geçmez.
        var candidate = name;
        for (var suffix = 2; categories.Any(category => category.NameKey == NameKeys.Of(candidate)); suffix++)
            candidate = $"{name} ({suffix})";
        var opened = new Category(Guid.NewGuid(), userId, candidate, type, role: role);
        await repository.AddAsync(opened, cancellationToken);
        return opened;
    }
}
