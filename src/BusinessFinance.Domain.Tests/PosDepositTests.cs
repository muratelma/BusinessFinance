namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 06.3 Grup 5: yatış — yoldaki POS parasının hesaba geçişi, kesinti ve
/// geri alma (ADR 0019 T5, ADR 0014).
/// </summary>
public sealed class PosDepositTests
{
    private static readonly DateTimeOffset NowUtc = new(2026, 10, 1, 18, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly SaleDay = new(2026, 9, 29);
    private static readonly DateOnly DepositDay = new(2026, 10, 1);

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Account _bank;
    private readonly Category _sales;
    private readonly Category _commission;

    public PosDepositTests()
    {
        _bank = new Account(Guid.NewGuid(), _userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 1000m);
        _sales = new Category(Guid.NewGuid(), _userId, "Satış", CategoryType.Income);
        _commission = new Category(Guid.NewGuid(), _userId, "POS komisyonu", CategoryType.Expense);
    }

    /// <summary>
    /// Yatış birkaç tahsilatı tek para hareketiyle kapatır ve hiçbir gelir
    /// yazmaz: satışlar tahsilat gününde tanındı.
    /// </summary>
    [Fact]
    public void Deposit_ClosesSeveralSettlementsWithOneMovementAndNoDeduction()
    {
        var first = NewSettlement(gross: 1000m, commission: 20m);
        var second = NewSettlement(gross: 500m, commission: 10m);

        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [first, second], Money(1470m), DepositDay, NowUtc);

        Assert.Equal(1470m, deposit.DepositedAmount.Amount);
        Assert.Equal(1470m, deposit.ExpectedAmount);
        Assert.Equal(0m, deposit.DeductionAmount);
        Assert.Null(deposit.DeductionTransactionId);
        Assert.False(deposit.IsCancelled);

        Assert.All(new[] { first, second }, settlement =>
        {
            Assert.Equal(deposit.Id, settlement.PosDepositId);
            Assert.Equal(DepositDay, settlement.TransferredOn);
            Assert.Equal(NowUtc, settlement.TransferredAtUtc);
            Assert.False(settlement.IsInTransit);
        });
        // Hesaba giren, tahsilatların neti; brüt ve komisyon değişmedi.
        Assert.Equal(980m, first.SignedAccountEffect);
        Assert.Equal(490m, second.SignedAccountEffect);
        Assert.Equal(1000m, first.GrossAmount.Amount);
        Assert.Equal(20m, first.CommissionAmount);
    }

    /// <summary>
    /// Kesinti = beklenen − yatan. Hesaba tahsilatların neti girer, kesinti
    /// gideri çıkar; farkları tam olarak bankanın yatırdığı tutardır.
    /// </summary>
    [Fact]
    public void Deposit_WritesTheShortfallAsADeductionCarriedByAnExpense()
    {
        var first = NewSettlement(gross: 1000m, commission: 20m);
        var second = NewSettlement(gross: 500m, commission: 10m);
        var deposited = Money(1450m);

        var deduction = PosDeposit.DeductionFor([first, second], deposited);
        var expense = NewDeductionExpense(deduction);
        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [first, second], deposited, DepositDay, NowUtc, expense);

        Assert.Equal(20m, deduction);
        Assert.Equal(20m, deposit.DeductionAmount);
        Assert.Equal(expense.Id, deposit.DeductionTransactionId);
        Assert.Equal(1470m, deposit.ExpectedAmount);
        Assert.Equal(
            deposit.DepositedAmount.Amount,
            first.SignedAccountEffect + second.SignedAccountEffect - expense.Amount.Amount);
    }

    /// <summary>
    /// Fazla yatan gelir değildir, fazla yazılmış komisyondur; gelir yazmak
    /// satışı şişirirdi (Grup 5 açılış kararı 2).
    /// </summary>
    [Fact]
    public void Deposit_RefusesAnAmountAboveTheExpectedNet()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 20m);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => PosDeposit.DeductionFor([settlement], Money(980.01m)));
        Assert.Throws<ArgumentOutOfRangeException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(1000m), DepositDay, NowUtc));
        Assert.True(settlement.IsInTransit);
    }

    [Fact]
    public void Deduction_AndItsExpenseAppearTogetherAndMustMatch()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 20m);

        // Kesinti var, gider yok.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(970m), DepositDay, NowUtc));
        // Kesinti yok, gider var.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(980m), DepositDay, NowUtc,
            NewDeductionExpense(10m)));
        // Gider farkla aynı tutarda değil.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(970m), DepositDay, NowUtc,
            NewDeductionExpense(9m)));
        // Gider başka bir güne yazılmış.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(970m), DepositDay, NowUtc,
            NewDeductionExpense(10m, DepositDay.AddDays(-1))));
        // Gider iptal edilmiş.
        var cancelled = NewDeductionExpense(10m);
        cancelled.Cancel(NowUtc);
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(970m), DepositDay, NowUtc, cancelled));

        // Reddedilen hiçbir deneme tahsilatı bağlamadı.
        Assert.True(settlement.IsInTransit);
        Assert.Null(settlement.PosDepositId);
    }

    [Fact]
    public void Deposit_ClosesOnlyInTransitSettlementsOfOneAccount()
    {
        var otherBank = new Account(
            Guid.NewGuid(), _userId, "Diğer banka", AccountType.Bank, CurrencyCode.TRY);
        var elsewhere = NewSettlement(account: otherBank);
        var cancelled = NewSettlement();
        cancelled.Cancel(NowUtc);
        var deposited = NewSettlement();
        PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [deposited], deposited.NetAmount, DepositDay, NowUtc);
        var waiting = NewSettlement();

        // Boş seçim.
        Assert.Throws<ArgumentException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [], Money(1m), DepositDay, NowUtc));
        // Başka hesaba geçecek tahsilat.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [waiting, elsewhere], Money(100m), DepositDay, NowUtc));
        // İptal edilmiş tahsilat.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [waiting, cancelled], Money(100m), DepositDay, NowUtc));
        // Zaten yatmış tahsilat: para bir kez geçer.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [waiting, deposited], Money(100m), DepositDay, NowUtc));
        // Aynı tahsilat iki kez.
        Assert.Throws<ArgumentException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [waiting, waiting], Money(100m), DepositDay, NowUtc));
        // Başka kullanıcının hesabı: tahsilat o hesaba ait değildir.
        var foreignBank = new Account(
            Guid.NewGuid(), Guid.NewGuid(), "Yabancı", AccountType.Bank, CurrencyCode.TRY);
        Assert.Throws<InvalidOperationException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, foreignBank, [waiting], waiting.NetAmount, DepositDay, NowUtc));

        Assert.True(waiting.IsInTransit);
    }

    [Fact]
    public void DepositDate_CannotPrecedeASettlementOrSitInTheFuture()
    {
        var early = NewSettlement(settlementDate: SaleDay);
        var late = NewSettlement(settlementDate: SaleDay.AddDays(1));

        // İkinci tahsilattan önceki gün: para satıştan önce yatamaz.
        Assert.Throws<ArgumentOutOfRangeException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [early, late], Money(1964.2m), SaleDay, NowUtc));
        // Kullanıcının günü sunucunun UTC gününden bir gün ileride olabilir
        // (`LocalDay`): gece yarısından sonra bugünün tarihi kabul edilir, iki
        // gün sonrası reddedilir.
        Assert.Throws<ArgumentOutOfRangeException>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [early, late], Money(1964.2m),
            DepositDay.AddDays(2), NowUtc));
        // Reddedilen deneme ilk tahsilatı da bağlamadı.
        Assert.True(early.IsInTransit);
        Assert.True(late.IsInTransit);
    }

    /// <summary>
    /// Geri alma kapattıklarını yola döndürür ve kesinti giderini iptal eder;
    /// satış ve komisyon tanınmış olarak kalır.
    /// </summary>
    [Fact]
    public void Revert_PutsEverySettlementBackOnTheRoadAndCancelsTheDeduction()
    {
        var first = NewSettlement(gross: 1000m, commission: 20m);
        var second = NewSettlement(gross: 500m, commission: 10m);
        var expense = NewDeductionExpense(20m);
        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [first, second], Money(1450m), DepositDay, NowUtc, expense);
        var revertedAt = NowUtc.AddHours(1);

        deposit.Revert([first, second], expense, revertedAt);

        Assert.True(deposit.IsCancelled);
        Assert.Equal(revertedAt, deposit.CancelledAtUtc);
        Assert.True(expense.IsCancelled);
        Assert.All(new[] { first, second }, settlement =>
        {
            Assert.True(settlement.IsInTransit);
            Assert.Null(settlement.PosDepositId);
            Assert.Null(settlement.TransferredOn);
            Assert.Null(settlement.TransferredAtUtc);
            Assert.Equal(0m, settlement.SignedAccountEffect);
        });
        Assert.Equal(1000m, first.GrossAmount.Amount);
        Assert.Equal(20m, first.CommissionAmount);

        // İdempotent: ikinci çağrı ilk damgayı korur.
        deposit.Revert([], null, revertedAt.AddHours(1));
        Assert.Equal(revertedAt, deposit.CancelledAtUtc);

        // Yola dönen tahsilat doğru tutarla yeniden yatırılabilir.
        var again = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [first, second], Money(1470m), DepositDay, NowUtc);
        Assert.Equal(again.Id, first.PosDepositId);
    }

    [Fact]
    public void Revert_NeedsEverySettlementAndTheDeductionExpense()
    {
        var first = NewSettlement(gross: 1000m, commission: 20m);
        var second = NewSettlement(gross: 500m, commission: 10m);
        var stranger = NewSettlement();
        var expense = NewDeductionExpense(20m);
        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [first, second], Money(1450m), DepositDay, NowUtc, expense);

        // Eksik liste, yabancı tahsilat, eksik ve yanlış gider.
        Assert.Throws<InvalidOperationException>(
            () => deposit.Revert([first], expense, NowUtc));
        Assert.Throws<InvalidOperationException>(
            () => deposit.Revert([first, second, stranger], expense, NowUtc));
        Assert.Throws<InvalidOperationException>(
            () => deposit.Revert([first, second], null, NowUtc));
        Assert.Throws<InvalidOperationException>(
            () => deposit.Revert([first, second], NewDeductionExpense(20m), NowUtc));

        Assert.False(deposit.IsCancelled);
        Assert.False(expense.IsCancelled);
        Assert.Equal(deposit.Id, first.PosDepositId);
    }

    /// <summary>
    /// Yatışa bağlı tahsilat, yatış geri alınmadan iptal edilemez: yatış o
    /// tahsilatın netini hesaba taşımıştır (ADR 0019 T5).
    /// </summary>
    [Fact]
    public void DepositedSettlement_CannotBeCancelledUntilTheDepositIsReverted()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 20m);
        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(980m), DepositDay, NowUtc);

        Assert.Throws<InvalidOperationException>(() => settlement.Cancel(NowUtc));
        Assert.False(settlement.IsCancelled);

        deposit.Revert([settlement], null, NowUtc);
        settlement.Cancel(NowUtc);

        Assert.True(settlement.IsCancelled);
        Assert.Equal(0m, settlement.SignedAccountEffect);
    }

    [Fact]
    public void RestoreCancelled_RebuildsARevertedDepositWithoutSettlements()
    {
        var expense = NewDeductionExpense(20m);
        expense.Cancel(NowUtc);

        var deposit = PosDeposit.RestoreCancelled(
            Guid.NewGuid(), _userId, _bank, Money(1450m), 20m, expense, DepositDay, NowUtc,
            NowUtc.AddHours(1));

        Assert.True(deposit.IsCancelled);
        Assert.Equal(NowUtc.AddHours(1), deposit.CancelledAtUtc);
        Assert.Equal(1470m, deposit.ExpectedAmount);
        Assert.Equal(expense.Id, deposit.DeductionTransactionId);

        // Geri alınmış yatış canlı bir kesinti gideri taşıyamaz.
        Assert.Throws<InvalidOperationException>(() => PosDeposit.RestoreCancelled(
            Guid.NewGuid(), _userId, _bank, Money(1450m), 20m, NewDeductionExpense(20m),
            DepositDay, NowUtc, NowUtc));
    }

    [Fact]
    public void Deposit_ArrivesInABankAccount()
    {
        var till = new Account(Guid.NewGuid(), _userId, "Kasa", AccountType.Cash, CurrencyCode.TRY);
        var settlement = NewSettlement();

        // Kasa POS parası alamaz; tahsilat da zaten o hesaba ait değildir.
        Assert.ThrowsAny<Exception>(() => PosDeposit.Record(
            Guid.NewGuid(), _userId, till, [settlement], settlement.NetAmount, DepositDay, NowUtc));
        Assert.Throws<InvalidOperationException>(() => PosDeposit.RestoreCancelled(
            Guid.NewGuid(), _userId, till, Money(100m), 0m, null, DepositDay, NowUtc, NowUtc));
    }

    private static Money Money(decimal amount) => new(amount, CurrencyCode.TRY);

    private BudgetTransaction NewDeductionExpense(decimal amount, DateOnly? date = null) =>
        new(
            Guid.NewGuid(), _userId, _bank, _commission, Money(amount), TransactionType.Expense,
            TransactionScope.Business, date ?? DepositDay);

    private PosSettlement NewSettlement(
        decimal gross = 1000m,
        decimal commission = 17.9m,
        Account? account = null,
        DateOnly? settlementDate = null) =>
        new(
            Guid.NewGuid(),
            _userId,
            account ?? _bank,
            _sales,
            Money(gross),
            commission,
            TransactionScope.Business,
            settlementDate ?? SaleDay,
            settlementDate ?? SaleDay,
            NowUtc,
            commission > 0m ? _commission : null);
}
