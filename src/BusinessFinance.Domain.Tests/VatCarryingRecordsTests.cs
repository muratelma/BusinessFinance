using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 05 Grup 2: gelir/gider tanıyan beş kaydın KDV taşıması. KDV taşınan
/// bir bilgidir; kaydın tutarını ve rapora yaptığı etkiyi <b>değiştirmez</b>
/// (ADR 0016).
/// </summary>
public sealed class VatCarryingRecordsTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 8, 26, 9, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly Today = new(2026, 8, 26);

    /// <summary>
    /// Beş kayıt türünde de KDV boş bırakılabilir: her harekette KDV yoktur.
    /// </summary>
    [Fact]
    public void EveryRecognizingRecord_CanBeWrittenWithoutVat()
    {
        var userId = Guid.NewGuid();

        Assert.Null(NewTransaction(userId).Vat);
        Assert.Null(NewCardCharge(userId).Vat);
        Assert.Null(NewCounterpartyCharge(userId).Vat);
        Assert.Null(NewObligation(userId).Vat);
        Assert.Null(NewSettlement(userId).Vat);
    }

    /// <summary>
    /// KDV girildiğinde olduğu gibi durur ve kaydın tutarına dokunmaz: kayıt
    /// tutarı brüttür ve brüt kalır.
    /// </summary>
    [Fact]
    public void VatIsCarried_WithoutChangingTheAmountOfTheRecord()
    {
        var userId = Guid.NewGuid();
        var vat = new VatDetails(rate: 0.20m, amount: 20m);

        var transaction = NewTransaction(userId, vat: vat);
        Assert.Equal(vat, transaction.Vat);
        Assert.Equal(120m, transaction.Amount.Amount);

        var cardCharge = NewCardCharge(userId, vat: vat);
        Assert.Equal(vat, cardCharge.Vat);
        Assert.Equal(120m, cardCharge.Amount.Amount);

        var counterpartyCharge = NewCounterpartyCharge(userId, vat: vat);
        Assert.Equal(vat, counterpartyCharge.Vat);
        Assert.Equal(120m, counterpartyCharge.Amount.Amount);

        var obligation = NewObligation(userId, vat: vat);
        Assert.Equal(vat, obligation.Vat);
        Assert.Equal(120m, obligation.Amount.Amount);
    }

    /// <summary>
    /// POS tahsilatında KDV brütün üstünde durur: net tutar, komisyon ve
    /// hesaba geçen para KDV'den etkilenmez.
    /// </summary>
    [Fact]
    public void PosSettlement_CarriesVatWithoutTouchingTheMoneyThatMoves()
    {
        var userId = Guid.NewGuid();
        var withoutVat = NewSettlement(userId);
        var withVat = NewSettlement(userId, vat: new VatDetails(0.20m, 20m));

        Assert.Equal(withoutVat.NetAmount.Amount, withVat.NetAmount.Amount);
        Assert.Equal(withoutVat.CommissionAmount, withVat.CommissionAmount);

        withVat.MarkTransferred(Today, CreatedAtUtc);
        Assert.Equal(withVat.NetAmount.Amount, withVat.SignedAccountEffect);
    }

    /// <summary>
    /// KDV tutarı kaydın tutarını aşamaz; beş kayıt türünde de aynı sınır.
    /// </summary>
    [Fact]
    public void VatAmountAboveTheRecordAmount_IsRejectedEverywhere()
    {
        var userId = Guid.NewGuid();
        var tooMuch = new VatDetails(rate: null, amount: 120.01m);

        Assert.Throws<ArgumentOutOfRangeException>(() => NewTransaction(userId, vat: tooMuch));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewCardCharge(userId, vat: tooMuch));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewCounterpartyCharge(userId, vat: tooMuch));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewObligation(userId, vat: tooMuch));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewSettlement(userId, vat: tooMuch));
    }

    private static BudgetTransaction NewTransaction(Guid userId, VatDetails? vat = null) => new(
        Guid.NewGuid(),
        userId,
        new Account(Guid.NewGuid(), userId, "Kasa", AccountType.Cash, CurrencyCode.TRY),
        new Category(Guid.NewGuid(), userId, "Ticari mal", CategoryType.Expense),
        new Money(120m, CurrencyCode.TRY),
        TransactionType.Expense,
        TransactionScope.Business,
        Today,
        description: null,
        vat: vat);

    private static CreditCardCharge NewCardCharge(Guid userId, VatDetails? vat = null) => new(
        Guid.NewGuid(),
        userId,
        new CreditCard(Guid.NewGuid(), userId, "Kart", new Money(5000m, CurrencyCode.TRY), 10, 20),
        new Category(Guid.NewGuid(), userId, "Yakıt", CategoryType.Expense),
        new Money(120m, CurrencyCode.TRY),
        TransactionScope.Business,
        Today,
        description: null,
        vat: vat);

    private static CounterpartyCharge NewCounterpartyCharge(
        Guid userId,
        VatDetails? vat = null) => new(
        Guid.NewGuid(),
        userId,
        new Counterparty(Guid.NewGuid(), userId, "Ahmet Manav"),
        new Category(Guid.NewGuid(), userId, "Veresiye satış", CategoryType.Income),
        DebtDirection.Receivable,
        new Money(120m, CurrencyCode.TRY),
        TransactionScope.Business,
        Today,
        description: null,
        dueDate: null,
        vat: vat);

    private static Obligation NewObligation(Guid userId, VatDetails? vat = null) => new(
        Guid.NewGuid(),
        userId,
        new Category(Guid.NewGuid(), userId, "Fatura", CategoryType.Expense),
        DebtDirection.Payable,
        new Money(120m, CurrencyCode.TRY),
        TransactionScope.Business,
        Today,
        Today.AddDays(10),
        CreatedAtUtc,
        counterparty: null,
        description: null,
        vat: vat);

    private static PosSettlement NewSettlement(Guid userId, VatDetails? vat = null) => new(
        Guid.NewGuid(),
        userId,
        new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 1000m),
        new Category(Guid.NewGuid(), userId, "Satış", CategoryType.Income),
        new Money(120m, CurrencyCode.TRY),
        3m,
        TransactionScope.Business,
        Today,
        Today,
        CreatedAtUtc,
        new Category(Guid.NewGuid(), userId, "POS komisyonu", CategoryType.Expense),
        description: null,
        vat: vat);
}
