namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 06.3 Grup 5: gün sonu — var olan kayıtları üreten, tutar taşımayan
/// kimlik ve bir bütün olarak geri alma (ADR 0019 T1, İ3, İ8).
/// </summary>
public sealed class DayCloseTests
{
    private static readonly DateTimeOffset NowUtc = new(2026, 10, 4, 18, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Day = new(2026, 10, 3);

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _closeId = Guid.NewGuid();
    private readonly Account _till;
    private readonly Account _bank;
    private readonly Category _sales;
    private readonly Category _commission;

    public DayCloseTests()
    {
        _till = new Account(Guid.NewGuid(), _userId, "Kasa", AccountType.Cash, CurrencyCode.TRY, 0m);
        _bank = new Account(Guid.NewGuid(), _userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 0m);
        _sales = new Category(Guid.NewGuid(), _userId, "Satış", CategoryType.Income);
        _commission = new Category(Guid.NewGuid(), _userId, "POS komisyonu", CategoryType.Expense);
    }

    /// <summary>
    /// Gün sonu sıradan bir gelir ve sıradan bir POS tahsilatı üretir; kendisi
    /// tutar taşımaz.
    /// </summary>
    [Fact]
    public void Record_LinksTheRecordsItProduced()
    {
        var income = NewIncome(2100m);
        var settlement = NewSettlement(1480m, 29.6m);

        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [income], [settlement]);

        Assert.Equal(Day, close.ClosedOn);
        Assert.Equal(Day, close.FirstDay);
        Assert.Null(close.RangeStart);
        Assert.Null(close.ZNumber);
        Assert.False(close.IsAdditional);
        Assert.False(close.IsCancelled);
        Assert.Equal(_closeId, income.DayCloseId);
        Assert.Equal(_closeId, settlement.DayCloseId);
        // Kayıtlar başka her gelir ve tahsilat gibidir.
        Assert.Equal(2100m, income.Amount.Amount);
        Assert.True(settlement.IsInTransit);
        Assert.Equal(1450.4m, settlement.NetAmount.Amount);
    }

    /// <summary>
    /// Gün içinde her satış tek tek girildiyse yazılacak tutar kalmaz; gün
    /// yine de kapatılmıştır.
    /// </summary>
    [Fact]
    public void Record_AllowsACloseThatProducesNothing()
    {
        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [], []);

        Assert.False(close.IsCancelled);
        Assert.True(close.Covers(Day));
    }

    /// <summary>Birkaç günlük Z: aralıktaki bütün günler kapalı sayılır.</summary>
    [Fact]
    public void Record_ARangeCoversEveryDayInIt()
    {
        var close = DayClose.Record(
            _closeId, _userId, Day, NowUtc, [], [],
            rangeStart: Day.AddDays(-2), zNumber: 3143);

        Assert.Equal(Day.AddDays(-2), close.FirstDay);
        Assert.Equal(3143, close.ZNumber);
        Assert.True(close.Covers(Day.AddDays(-2)));
        Assert.True(close.Covers(Day.AddDays(-1)));
        Assert.True(close.Covers(Day));
        Assert.False(close.Covers(Day.AddDays(-3)));
        Assert.False(close.Covers(Day.AddDays(1)));
    }

    [Fact]
    public void Record_RejectsAFutureDayAnInvertedRangeAndABadZNumber()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DayClose.Record(_closeId, _userId, new DateOnly(2026, 10, 5), NowUtc, [], []));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [], [], rangeStart: Day));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [], [], zNumber: 0));
        Assert.Throws<ArgumentException>(() =>
            DayClose.Record(Guid.Empty, _userId, Day, NowUtc, [], []));
    }

    /// <summary>
    /// Gün sonu yalnız kendi kimliğiyle kurulmuş, kapattığı günlerin içindeki
    /// canlı kayıtları kabul eder; aksi hâlde geri alma başka bir kaydı iptal
    /// ederdi.
    /// </summary>
    [Fact]
    public void Record_RejectsRecordsThatAreNotItsOwn()
    {
        var foreign = NewIncome(100m, dayCloseId: Guid.NewGuid());
        var manual = new BudgetTransaction(
            Guid.NewGuid(), _userId, _till, _sales, Money(100m), TransactionType.Income,
            TransactionScope.Business, Day);
        var otherDay = NewIncome(100m, date: Day.AddDays(-1));
        var cancelled = NewIncome(100m);
        cancelled.Cancel(NowUtc);

        Assert.Throws<ArgumentException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [foreign], []));
        Assert.Throws<ArgumentException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [manual], []));
        Assert.Throws<InvalidOperationException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [otherDay], []));
        Assert.Throws<InvalidOperationException>(() =>
            DayClose.Record(_closeId, _userId, Day, NowUtc, [cancelled], []));
        Assert.Throws<ArgumentException>(() =>
            DayClose.Record(
                _closeId, _userId, Day, NowUtc, [],
                [NewSettlement(100m, 0m, dayCloseId: Guid.NewGuid())]));
    }

    /// <summary>Gün sonu gelir üretir; gider üretmez.</summary>
    [Fact]
    public void AnExpenseCannotCarryADayClose()
    {
        var expenseCategory = new Category(Guid.NewGuid(), _userId, "Gider", CategoryType.Expense);

        Assert.Throws<InvalidOperationException>(() => new BudgetTransaction(
            Guid.NewGuid(), _userId, _till, expenseCategory, Money(10m), TransactionType.Expense,
            TransactionScope.Business, Day, dayCloseId: _closeId));
    }

    /// <summary>
    /// Geri alma bir bütündür: ürettiği gelir ve tahsilat birlikte iptal
    /// olur, gün sonu kaydı kalır (silme yerine iptal).
    /// </summary>
    [Fact]
    public void Revert_CancelsEveryRecordTogether_AndIsIdempotent()
    {
        var income = NewIncome(2100m);
        var settlement = NewSettlement(1480m, 29.6m);
        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [income], [settlement]);
        var revertedAt = NowUtc.AddHours(1);

        close.Revert([income], [settlement], revertedAt);

        Assert.True(close.IsCancelled);
        Assert.Equal(revertedAt, close.CancelledAtUtc);
        Assert.True(income.IsCancelled);
        Assert.True(settlement.IsCancelled);
        Assert.False(settlement.IsInTransit);
        // Kayıtlar hangi gün sonundan geldiklerini hatırlar.
        Assert.Equal(_closeId, income.DayCloseId);

        close.Revert([income], [settlement], revertedAt.AddHours(1));
        Assert.Equal(revertedAt, close.CancelledAtUtc);
    }

    /// <summary>
    /// Ürettiği tahsilat bir yatışla hesaba geçtiyse gün sonu geri alınamaz;
    /// reddedilen geri alma hiçbir kaydı değiştirmez.
    /// </summary>
    [Fact]
    public void Revert_IsRejectedWhileASettlementIsDeposited_AndChangesNothing()
    {
        var income = NewIncome(2100m);
        var settlement = NewSettlement(1000m, 20m);
        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [income], [settlement]);
        var deposit = PosDeposit.Record(
            Guid.NewGuid(), _userId, _bank, [settlement], Money(980m), Day, NowUtc);

        Assert.Throws<InvalidOperationException>(() =>
            close.Revert([income], [settlement], NowUtc));

        Assert.False(close.IsCancelled);
        Assert.False(income.IsCancelled);
        Assert.False(settlement.IsCancelled);

        // Yatış geri alınınca gün sonu da geri alınabilir.
        deposit.Revert([settlement], null, NowUtc);
        close.Revert([income], [settlement], NowUtc);
        Assert.True(close.IsCancelled);
        Assert.True(settlement.IsCancelled);
    }

    [Fact]
    public void Revert_RejectsARecordOfAnotherDayClose()
    {
        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [], []);
        var foreign = NewIncome(100m, dayCloseId: Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => close.Revert([foreign], [], NowUtc));
        Assert.False(close.IsCancelled);
        Assert.False(foreign.IsCancelled);
    }

    /// <summary>
    /// Gün sonu düştüğü kaydı sahiplenir; bağ tutar taşımaz. Geri alınmış gün
    /// sonu hiçbir kaydı sayamaz.
    /// </summary>
    [Fact]
    public void CountedRecord_LinksARecordToALiveDayClose()
    {
        var close = DayClose.Record(_closeId, _userId, Day, NowUtc, [], []);
        var recordId = Guid.NewGuid();

        var counted = new DayCloseCountedRecord(close, DayCloseRecordKind.Income, recordId);

        Assert.Equal(_userId, counted.UserId);
        Assert.Equal(_closeId, counted.DayCloseId);
        Assert.Equal(DayCloseRecordKind.Income, counted.Kind);
        Assert.Equal(recordId, counted.RecordId);

        Assert.Throws<ArgumentException>(() =>
            new DayCloseCountedRecord(close, DayCloseRecordKind.Income, Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DayCloseCountedRecord(close, (DayCloseRecordKind)9, recordId));
        close.Revert([], [], NowUtc);
        Assert.Throws<InvalidOperationException>(() =>
            new DayCloseCountedRecord(close, DayCloseRecordKind.Income, recordId));
    }

    /// <summary>Yedekten kurulan gün sonu, geri alınmışsa öyle kurulur.</summary>
    [Fact]
    public void Restore_RebuildsALiveOrRevertedClose()
    {
        var live = DayClose.Restore(_closeId, _userId, Day, null, 3143, false, NowUtc, null);
        var reverted = DayClose.Restore(
            Guid.NewGuid(), _userId, Day, Day.AddDays(-1), null, true, NowUtc, NowUtc.AddHours(2));

        Assert.False(live.IsCancelled);
        Assert.Equal(3143, live.ZNumber);
        Assert.True(reverted.IsCancelled);
        Assert.True(reverted.IsAdditional);
        Assert.Equal(NowUtc.AddHours(2), reverted.CancelledAtUtc);
    }

    private BudgetTransaction NewIncome(decimal amount, Guid? dayCloseId = null, DateOnly? date = null) =>
        new(
            Guid.NewGuid(), _userId, _till, _sales, Money(amount), TransactionType.Income,
            TransactionScope.Business, date ?? Day, dayCloseId: dayCloseId ?? _closeId);

    private PosSettlement NewSettlement(decimal gross, decimal commission, Guid? dayCloseId = null) =>
        new(
            Guid.NewGuid(), _userId, _bank, _sales, Money(gross), commission,
            TransactionScope.Business, Day, Day.AddDays(1), NowUtc,
            commission > 0m ? _commission : null,
            dayCloseId: dayCloseId ?? _closeId);

    private static Money Money(decimal amount) => new(amount, CurrencyCode.TRY);
}
