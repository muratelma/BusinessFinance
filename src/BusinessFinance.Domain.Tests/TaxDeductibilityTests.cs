using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 05 Grup 3: indirilebilirlik kapsamdan ayrı, iki durumlu bir alandır ve
/// yalnız işletme kapsamlı gider kaydında anlamlıdır (ADR 0016).
/// </summary>
public sealed class TaxDeductibilityTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 8, 26, 9, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly Today = new(2026, 8, 26);

    /// <summary>
    /// Grubun çıkış ölçütü: alan işletme giderinde cevaplanabilir ve boş
    /// bırakılabilir; boş olması üçüncü bir durum değil, sorunun sorulmamış
    /// olmasıdır.
    /// </summary>
    [Fact]
    public void BusinessExpense_CanAnswerTheQuestionOrLeaveItUnasked()
    {
        var userId = Guid.NewGuid();

        Assert.True(NewTransaction(userId, isTaxDeductible: true).IsTaxDeductible);
        Assert.False(NewTransaction(userId, isTaxDeductible: false).IsTaxDeductible);
        Assert.Null(NewTransaction(userId).IsTaxDeductible);
    }

    /// <summary>
    /// Kapsam ile indirilebilirlik ayrı sorulardır: şahsi kayda soru sorulmaz
    /// ve cevap sessizce saklanmaz — reddedilir.
    /// </summary>
    [Fact]
    public void PersonalRecord_IsNotAskedTheQuestion()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => NewTransaction(
            userId,
            scope: TransactionScope.Personal,
            isTaxDeductible: true));
    }

    /// <summary>
    /// Gelir kaydında da sorulmaz: indirilebilirlik gider tarafının sorusudur.
    /// </summary>
    [Fact]
    public void IncomeRecord_IsNotAskedTheQuestion()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => NewTransaction(
            userId,
            type: TransactionType.Income,
            isTaxDeductible: true));
    }

    /// <summary>
    /// Kart harcaması her zaman gider tanır; cari borçlandırma ve yükümlülük
    /// yalnız borç yönünde tanır ve alacak yönünde soru sorulmaz.
    /// </summary>
    [Fact]
    public void OtherRecognizingRecords_FollowTheSameRule()
    {
        var userId = Guid.NewGuid();

        Assert.True(NewCardCharge(userId, isTaxDeductible: true).IsTaxDeductible);
        Assert.True(NewCounterpartyCharge(userId, isTaxDeductible: true).IsTaxDeductible);
        Assert.True(NewObligation(userId, isTaxDeductible: true).IsTaxDeductible);

        Assert.Throws<ArgumentException>(() => NewCounterpartyCharge(
            userId,
            direction: DebtDirection.Receivable,
            isTaxDeductible: true));
        Assert.Throws<ArgumentException>(() => NewObligation(
            userId,
            direction: DebtDirection.Receivable,
            isTaxDeductible: true));
        Assert.Throws<ArgumentException>(() => NewCardCharge(
            userId,
            scope: TransactionScope.Personal,
            isTaxDeductible: true));
    }

    /// <summary>
    /// Kategori bir varsayılan taşıyabilir ve yalnız gider kategorisi taşır.
    /// </summary>
    [Fact]
    public void OnlyAnExpenseCategory_CarriesADeductibilityDefault()
    {
        var userId = Guid.NewGuid();
        var expense = new Category(
            Guid.NewGuid(), userId, "Ticari mal", CategoryType.Expense, null, true);

        Assert.True(expense.DefaultIsTaxDeductible);

        expense.SetDefaultTaxDeductibility(null);
        Assert.Null(expense.DefaultIsTaxDeductible);

        Assert.Throws<ArgumentException>(() => new Category(
            Guid.NewGuid(), userId, "Satış", CategoryType.Income, null, true));
    }

    private static BudgetTransaction NewTransaction(
        Guid userId,
        TransactionScope scope = TransactionScope.Business,
        TransactionType type = TransactionType.Expense,
        bool? isTaxDeductible = null) => new(
        Guid.NewGuid(),
        userId,
        new Account(Guid.NewGuid(), userId, "Kasa", AccountType.Cash, CurrencyCode.TRY),
        new Category(
            Guid.NewGuid(),
            userId,
            "Kalem",
            type == TransactionType.Income ? CategoryType.Income : CategoryType.Expense),
        new Money(120m, CurrencyCode.TRY),
        type,
        scope,
        Today,
        description: null,
        vat: null,
        isTaxDeductible: isTaxDeductible);

    private static CreditCardCharge NewCardCharge(
        Guid userId,
        TransactionScope scope = TransactionScope.Business,
        bool? isTaxDeductible = null) => new(
        Guid.NewGuid(),
        userId,
        new CreditCard(Guid.NewGuid(), userId, "Kart", new Money(5000m, CurrencyCode.TRY), 10, 20),
        new Category(Guid.NewGuid(), userId, "Yakıt", CategoryType.Expense),
        new Money(120m, CurrencyCode.TRY),
        scope,
        Today,
        description: null,
        vat: null,
        isTaxDeductible: isTaxDeductible);

    private static CounterpartyCharge NewCounterpartyCharge(
        Guid userId,
        DebtDirection direction = DebtDirection.Payable,
        bool? isTaxDeductible = null) => new(
        Guid.NewGuid(),
        userId,
        new Counterparty(Guid.NewGuid(), userId, "Tedarikçi"),
        new Category(
            Guid.NewGuid(),
            userId,
            "Kalem",
            CounterpartyCharge.RequiredCategoryType(direction)),
        direction,
        new Money(120m, CurrencyCode.TRY),
        TransactionScope.Business,
        Today,
        description: null,
        dueDate: null,
        vat: null,
        isTaxDeductible: isTaxDeductible);

    private static Obligation NewObligation(
        Guid userId,
        DebtDirection direction = DebtDirection.Payable,
        bool? isTaxDeductible = null) => new(
        Guid.NewGuid(),
        userId,
        new Category(
            Guid.NewGuid(),
            userId,
            "Kalem",
            Obligation.RequiredCategoryType(direction)),
        direction,
        new Money(120m, CurrencyCode.TRY),
        TransactionScope.Business,
        Today,
        Today.AddDays(10),
        CreatedAtUtc,
        counterparty: null,
        description: null,
        vat: null,
        isTaxDeductible: isTaxDeductible);
}
