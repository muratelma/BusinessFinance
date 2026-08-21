using BusinessFinance.CSharpSandbox;

decimal monthlyIncome = 52_500.75m;
decimal monthlyExpense = 18_250.40m;
decimal remainingBudget = monthlyIncome - monthlyExpense;

string budgetStatus = GetBudgetStatus(monthlyIncome, monthlyExpense);

DateOnly transactionDate = new(2026, 8, 6);

DateTimeOffset createdAt = new(
    2026,
    8,
    6,
    15,
    30,
    0,
    TimeSpan.FromHours(3));

string? description = "Market alışverişi";
DateOnly? plannedPaymentDate = new DateOnly(2026, 8, 10);

string descriptionText = description ?? "Açıklama girilmedi";

string plannedDateText =
    plannedPaymentDate?.ToString("dd.MM.yyyy") ?? "Planlanmadı";

Console.WriteLine($"Aylık gelir: {monthlyIncome:N2} TRY");
Console.WriteLine($"Aylık gider: {monthlyExpense:N2} TRY");
Console.WriteLine($"Kalan bütçe: {remainingBudget:N2} TRY");
Console.WriteLine($"İşlem günü: {transactionDate:dd.MM.yyyy}");
Console.WriteLine($"Kayıt anı: {createdAt:dd.MM.yyyy HH:mm zzz}");
Console.WriteLine($"Açıklama: {descriptionText}");
Console.WriteLine($"Planlanan ödeme: {plannedDateText}");
Console.WriteLine($"Bütçe durumu: {budgetStatus}");
Console.WriteLine(
    $"Tam kullanım testi: {GetBudgetStatus(1_000m, 1_000m)}");

Console.WriteLine(
    $"Aşım testi: {GetBudgetStatus(1_000m, 1_250m)}");

ExpenseDraft draft = new(
    275.90m,
    new DateOnly(2026, 8, 7),
    "  Market alışverişi  ");

Okul okul = new("ERCİYES", 1970);

Console.WriteLine($"Taslak tutarı: {draft.Amount:N2} TRY");
Console.WriteLine($"Taslak günü: {draft.TransactionDate:dd.MM.yyyy}");
Console.WriteLine($"Taslak açıklaması: {draft.Description}");
Console.WriteLine($"Okul ismi: {okul.Name}");
Console.WriteLine($"Okul kuruluş yılı: {okul.KurulusYili}");

try
{
    _ = new ExpenseDraft(
        0m,
        new DateOnly(2026, 8, 7),
        null);
}
catch (ArgumentOutOfRangeException exception)
{
    Console.WriteLine(
        $"Geçersiz taslak parametresi: {exception.ParamName}");
}

try
{
    _ = GetBudgetStatus(1_000m, -1m);
}
catch (ArgumentOutOfRangeException exception)
{
    Console.WriteLine(
        $"Geçersiz parametre yakalandı: {exception.ParamName}");
}

List<ExpenseDraft> expenses = new()
{
    draft,
    new ExpenseDraft(
        120.50m,
        new DateOnly(2026, 8, 8),
        "Ulaşım"),
    new ExpenseDraft(
        850m,
        new DateOnly(2026, 8, 9),
        "Elektrik faturası")
};

Console.WriteLine($"Gider sayısı: {expenses.Count}");

foreach (ExpenseDraft expense in expenses)
{
    Console.WriteLine(
        $"{expense.TransactionDate:dd.MM.yyyy} - " +
        $"{expense.Description} - {expense.Amount:N2} TRY");
}
decimal minimumAmount = 500m;

List<ExpenseDraft> highAmountExpenses = expenses
    .Where(expense => expense.Amount >= minimumAmount)
    .ToList();

Console.WriteLine(
    $"En az {minimumAmount:N2} TRY olan gider sayısı: " +
    $"{highAmountExpenses.Count}");

foreach (ExpenseDraft expense in highAmountExpenses)
{
    Console.WriteLine(
        $"Yüksek tutarlı gider: {expense.Description}");
}

decimal highAmountTotal = highAmountExpenses
    .Sum(expense => expense.Amount);

Console.WriteLine(
    $"Yüksek tutarlı giderlerin toplamı: {highAmountTotal:N2} TRY");

string searchText = "fatura";

List<ExpenseDraft> matchingExpenses = expenses
    .Where(expense =>
        expense.Description?.Contains(
            searchText,
            StringComparison.OrdinalIgnoreCase) == true)
    .ToList();

Console.WriteLine(
    $"'{searchText}' içeren gider sayısı: {matchingExpenses.Count}");


expenses.Add(
    new ExpenseDraft(
        200m,
        new DateOnly(2026, 8, 8),
        "Kitap"));

var expensesByDate = expenses
    .GroupBy(expense => expense.TransactionDate);

foreach (var dateGroup in expensesByDate)
{
    decimal dateTotal = dateGroup
        .Sum(expense => expense.Amount);

    Console.WriteLine(
        $"{dateGroup.Key:dd.MM.yyyy}: " +
        $"{dateGroup.Count()} gider, {dateTotal:N2} TRY");
}

IExpenseDraftSource expenseSource = new FakeExpenseDraftSource();
List<ExpenseDraft> sourceExpenses =
    await expenseSource.GetExpensesAsync();

Console.WriteLine($"Veri kaynagından gelen gider sayısı: {sourceExpenses.Count}");

ExpenseKind expenseKind = ExpenseKind.Essential;
Console.WriteLine($"gider Türü {expenseKind}");

ExpenseSummary firstSummary = new(
      sourceExpenses.Count,
      sourceExpenses.Sum(expense => expense.Amount));

ExpenseSummary secondSummary = new(
    sourceExpenses.Count,
    sourceExpenses.Sum(expense => expense.Amount));

Console.WriteLine(
    $"Record değerleri eşit mi: {firstSummary == secondSummary}");

IExpenseDraftSource failingSource =
    new FailingExpenseDraftSource();

try
{
    _ = await failingSource.GetExpensesAsync();
}
catch (InvalidOperationException exception)
{
    Console.WriteLine(
        $"Async hata yakalandı: {exception.Message}");
}

static string GetBudgetStatus(decimal income, decimal expense)
{
    if (income < 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(income),
            "Gelir negatif olamaz.");
    }

    if (expense < 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(expense),
            "Gider negatif olamaz.");
    }

    decimal remaining = income - expense;

    if (remaining < 0)
    {
        return "Bütçe aşıldı";
    }

    if (remaining == 0)
    {
        return "Bütçe tam kullanıldı";
    }

    return "Bütçe uygun";
}
