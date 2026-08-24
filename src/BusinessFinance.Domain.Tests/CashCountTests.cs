namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 04 Grup 2: gün sonu kasa sayımı, farkın türetilmesi ve onayla üretilen
/// tek düzeltme kaydı.
/// </summary>
public sealed class CashCountTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 8, 24, 18, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly CountDate = new(2026, 8, 24);

    [Fact]
    public void CashCount_IsAnObservation_NotAMovement()
    {
        var count = NewCount(countedAmount: 1250m);

        // Sayım bir gözlemdir: taşıdığı tek tutar sayılandır. Kayıtta bir
        // "beklenen" alanı olsaydı, sonradan iptal edilen bir hareket
        // bakiyeyi değiştirdiği anda o alan sessizce yanlışa dönerdi.
        var propertyNames = typeof(CashCount).GetProperties()
            .Select(property => property.Name)
            .ToArray();
        Assert.DoesNotContain("ExpectedBalance", propertyNames);
        Assert.DoesNotContain("Difference", propertyNames);
        Assert.False(count.IsAdjusted);
        Assert.Null(count.AdjustmentTransactionId);
        Assert.Equal(1250m, count.CountedAmount);
        Assert.Equal(CurrencyCode.TRY, count.Currency);
    }

    [Theory]
    [InlineData(1250, 1200, 50, true, false, false)]
    [InlineData(1150, 1200, -50, false, true, false)]
    [InlineData(1200, 1200, 0, false, false, true)]
    public void Difference_IsDerivedFromTheBalanceItIsReadAgainst(
        decimal counted,
        decimal expected,
        decimal difference,
        bool isSurplus,
        bool isShortage,
        bool isBalanced)
    {
        var count = NewCount(countedAmount: counted);

        var result = count.DifferenceFrom(expected);

        Assert.Equal(difference, result.Amount);
        Assert.Equal(isSurplus, result.IsSurplus);
        Assert.Equal(isShortage, result.IsShortage);
        Assert.Equal(isBalanced, result.IsBalanced);
    }

    /// <summary>
    /// Aynı sayım, beklenen bakiye değiştiğinde farklı bir fark verir. Fark
    /// saklansaydı bu mümkün olmazdı ve ekran eskimiş bir sayı gösterirdi.
    /// </summary>
    [Fact]
    public void Difference_FollowsTheBalanceWhenAMovementIsLaterCancelled()
    {
        var count = NewCount(countedAmount: 1000m);

        var beforeCancellation = count.DifferenceFrom(1200m);
        var afterCancellation = count.DifferenceFrom(1000m);

        Assert.Equal(-200m, beforeCancellation.Amount);
        Assert.True(beforeCancellation.IsShortage);
        Assert.Equal(0m, afterCancellation.Amount);
        Assert.True(afterCancellation.IsBalanced);
    }

    [Theory]
    [InlineData(1250, 1200, TransactionType.Income, 50)]
    [InlineData(1150, 1200, TransactionType.Expense, 50)]
    public void Difference_CarriesTheTypeAndAPositiveAmountForItsAdjustment(
        decimal counted,
        decimal expected,
        TransactionType recognizedType,
        decimal adjustmentAmount)
    {
        var difference = NewCount(countedAmount: counted).DifferenceFrom(expected);

        Assert.Equal(recognizedType, difference.RecognizedType);
        // Yönü tür taşır; tutar her zaman pozitiftir (Money sözleşmesi).
        Assert.Equal(adjustmentAmount, difference.ToAdjustmentAmount().Amount);
    }

    [Fact]
    public void BalancedDifference_HasNoTypeAndNoAdjustmentAmount()
    {
        var difference = NewCount(countedAmount: 1200m).DifferenceFrom(1200m);

        Assert.Throws<InvalidOperationException>(() => difference.RecognizedType);
        // Sıfır tutarlı bir düzeltme kaydı yazılamaz; yazacak bir şey yok.
        Assert.Throws<ArgumentOutOfRangeException>(difference.ToAdjustmentAmount);
    }

    /// <summary>
    /// Ölçütün kendisi: sayım tek başına hiçbir finansal kayıt üretmez.
    /// </summary>
    [Fact]
    public void CashCount_ProducesNoFinancialRecordWithoutAnExplicitConfirmation()
    {
        var count = NewCount(countedAmount: 900m);

        var difference = count.DifferenceFrom(1200m);

        Assert.Equal(-300m, difference.Amount);
        Assert.False(count.IsAdjusted);
        Assert.Null(count.AdjustmentTransactionId);
        Assert.Null(count.AdjustedAtUtc);
    }

    [Fact]
    public void RecordAdjustment_IsIdempotentAndRefusesASecondDifferentRecord()
    {
        var count = NewCount(countedAmount: 900m);
        var transactionId = Guid.NewGuid();

        count.RecordAdjustment(transactionId, CreatedAtUtc);
        count.RecordAdjustment(transactionId, CreatedAtUtc.AddMinutes(5));

        Assert.True(count.IsAdjusted);
        Assert.Equal(transactionId, count.AdjustmentTransactionId);
        // İlk damga korunur: ikinci onay yeni bir olay değildir.
        Assert.Equal(CreatedAtUtc, count.AdjustedAtUtc);
        Assert.Throws<InvalidOperationException>(
            () => count.RecordAdjustment(Guid.NewGuid(), CreatedAtUtc));
    }

    [Fact]
    public void SecondCountOfTheSameDay_CancelsTheFirstInsteadOfOverwritingIt()
    {
        var userId = Guid.NewGuid();
        var account = NewCashAccount(userId);
        var first = NewCount(userId: userId, account: account, countedAmount: 1000m);
        var second = NewCount(userId: userId, account: account, countedAmount: 1150m);

        first.SupersedeWith(second, CreatedAtUtc);

        Assert.True(first.IsCancelled);
        Assert.Equal(CreatedAtUtc, first.CancelledAtUtc);
        // Eski gözlem silinmez: o sayım gerçekten yapılmıştı.
        Assert.Equal(1000m, first.CountedAmount);
        Assert.False(second.IsCancelled);
        Assert.Equal(1150m, second.CountedAmount);
    }

    [Fact]
    public void SupersedeWith_RefusesAnotherDayAnotherAccountOrItself()
    {
        var userId = Guid.NewGuid();
        var account = NewCashAccount(userId);
        var otherAccount = NewCashAccount(userId, "İkinci kasa");
        var count = NewCount(userId: userId, account: account);

        Assert.Throws<InvalidOperationException>(
            () => count.SupersedeWith(count, CreatedAtUtc));
        Assert.Throws<InvalidOperationException>(
            () => count.SupersedeWith(
                NewCount(userId: userId, account: otherAccount), CreatedAtUtc));
        Assert.Throws<InvalidOperationException>(
            () => count.SupersedeWith(
                NewCount(userId: userId, account: account, countDate: new DateOnly(2026, 8, 23)),
                CreatedAtUtc));
        Assert.False(count.IsCancelled);
    }

    [Fact]
    public void CancelledCount_IsIdempotentAndCannotRecordAnAdjustment()
    {
        var count = NewCount();

        count.Cancel(CreatedAtUtc);
        count.Cancel(CreatedAtUtc.AddHours(1));

        Assert.True(count.IsCancelled);
        Assert.Equal(CreatedAtUtc, count.CancelledAtUtc);
        Assert.Throws<InvalidOperationException>(
            () => count.RecordAdjustment(Guid.NewGuid(), CreatedAtUtc));
    }

    [Fact]
    public void EmptyTillIsALegitimateCountButNegativeCashIsNot()
    {
        var empty = NewCount(countedAmount: 0m);

        Assert.Equal(0m, empty.CountedAmount);
        Assert.Equal(-1200m, empty.DifferenceFrom(1200m).Amount);
        Assert.Throws<ArgumentOutOfRangeException>(() => NewCount(countedAmount: -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewCount(countedAmount: 10.00001m));
    }

    /// <summary>
    /// Banka bakiyesi elle sayılmaz; sayım fiziksel bir gözlemdir.
    /// </summary>
    [Fact]
    public void OnlyAnOwnedActiveCashAccountCanBeCounted()
    {
        var userId = Guid.NewGuid();
        var bank = new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY);
        var closed = NewCashAccount(userId, "Kapanan kasa");
        closed.Deactivate();
        var stranger = NewCashAccount(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => NewCount(userId: userId, account: bank));
        Assert.Throws<InvalidOperationException>(() => NewCount(userId: userId, account: closed));
        Assert.Throws<ArgumentException>(() => NewCount(userId: userId, account: stranger));
    }

    [Fact]
    public void CountDateCannotBeInTheFutureAndScopeIsRequired()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewCount(countDate: new DateOnly(2026, 8, 25)));
        Assert.Throws<ArgumentOutOfRangeException>(() => NewCount(scope: (TransactionScope)9));

        // Geçmiş bir günün sayımı meşrudur: kullanıcı dün akşamki sayımı
        // bugün girebilir.
        var yesterday = NewCount(countDate: new DateOnly(2026, 8, 23));
        Assert.Equal(new DateOnly(2026, 8, 23), yesterday.CountDate);
    }

    [Fact]
    public void Note_IsOptionalTrimmedAndBounded()
    {
        Assert.Null(NewCount(note: "   ").Note);
        Assert.Equal("Bozukluk kutusu dâhil", NewCount(note: "  Bozukluk kutusu dâhil  ").Note);
        Assert.Throws<ArgumentException>(
            () => NewCount(note: new string('a', CashCount.MaximumNoteLength + 1)));
    }

    private static Account NewCashAccount(Guid userId, string name = "Kasa") =>
        new(Guid.NewGuid(), userId, name, AccountType.Cash, CurrencyCode.TRY, 500m);

    private static CashCount NewCount(
        Guid? userId = null,
        Account? account = null,
        decimal countedAmount = 1000m,
        TransactionScope scope = TransactionScope.Business,
        DateOnly? countDate = null,
        string? note = null)
    {
        var owner = userId ?? account?.UserId ?? Guid.NewGuid();
        return new CashCount(
            Guid.NewGuid(),
            owner,
            account ?? NewCashAccount(owner),
            countedAmount,
            scope,
            countDate ?? CountDate,
            CreatedAtUtc,
            note);
    }
}
