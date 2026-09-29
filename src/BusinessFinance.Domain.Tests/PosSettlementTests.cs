namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 04 Grup 3: POS tahsilatının tanıma/taşıma ayrımı, komisyon ve yoldaki
/// paranın projection olması (ADR 0014, ADR 0015).
/// </summary>
public sealed class PosSettlementTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 8, 24, 18, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly SettlementDate = new(2026, 8, 24);
    private static readonly DateOnly TransferDate = new(2026, 8, 24);

    /// <summary>
    /// Grubun çıkış ölçütü: tahsilat + geçiş senaryosunda gelir bir kez,
    /// komisyon bir kez sayılır.
    /// </summary>
    [Fact]
    public void Settlement_RecognizesOnCollectionDayAndCarriesOnlyOnTransferDay()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 17.9m);

        // Tahsilat günü: gelir brüt tutar kadar tanındı, komisyon ayrı gider,
        // hesap bakiyesi kıpırdamadı — para henüz bankada değil.
        Assert.Equal(1000m, settlement.GrossAmount.Amount);
        Assert.Equal(17.9m, settlement.CommissionAmount);
        Assert.Equal(982.1m, settlement.NetAmount.Amount);
        Assert.Equal(0m, settlement.SignedAccountEffect);
        Assert.True(settlement.IsInTransit);

        settlement.MarkTransferred(TransferDate, CreatedAtUtc);

        // Geçiş günü: hesap net tutar kadar arttı ve hiçbir gelir/gider
        // yeniden yazılmadı. Brüt ve komisyon değişmedi — ikinci kez sayılan
        // bir şey yok.
        Assert.Equal(982.1m, settlement.SignedAccountEffect);
        Assert.Equal(1000m, settlement.GrossAmount.Amount);
        Assert.Equal(17.9m, settlement.CommissionAmount);
        Assert.False(settlement.IsInTransit);
        Assert.True(settlement.IsTransferred);
        Assert.Equal(TransferDate, settlement.TransferredOn);
    }

    /// <summary>
    /// Komisyon brüt tutara eklenmez ve ondan düşülerek gizlenmez: ikisi ayrı
    /// okunur. Net tutarı gelir yazmak, kesilen faturayı küçültürdü.
    /// </summary>
    [Fact]
    public void Commission_IsReadBesideTheGrossAmountNotFoldedIntoIt()
    {
        var settlement = NewSettlement(gross: 500m, commission: 12.5m);

        Assert.Equal(500m, settlement.GrossAmount.Amount);
        Assert.Equal(487.5m, settlement.NetAmount.Amount);
        Assert.NotEqual(settlement.GrossAmount.Amount, settlement.NetAmount.Amount);
        Assert.NotNull(settlement.CommissionCategoryId);
    }

    [Theory]
    [InlineData(1000, 0.0179, 17.9)]
    [InlineData(250, 0.02, 5)]
    // 99,99 × 0,015 = 1,49985 → paranın dört basamağına yuvarlanır. Kuruşa
    // yuvarlamak, hesaba geçen tutarla dosyadaki tutarı ayrıştırırdı.
    [InlineData(99.99, 0.015, 1.4999)]
    public void CommissionFromRate_ResolvesTheAmountAndTheRateComesBackFromMoney(
        decimal gross,
        decimal rate,
        decimal expectedCommission)
    {
        var grossAmount = new Money(gross, CurrencyCode.TRY);

        var commission = PosSettlement.CommissionFromRate(grossAmount, rate);
        var settlement = NewSettlement(gross: gross, commission: commission);

        Assert.Equal(expectedCommission, commission);
        // Oran saklanmıyor, paradan çözülüyor (ADR 0009'un aynı kararı).
        Assert.DoesNotContain(
            "CommissionRateStored",
            typeof(PosSettlement).GetProperties().Select(property => property.Name));
        Assert.Equal(
            decimal.Round(commission / gross, 4, MidpointRounding.AwayFromZero),
            settlement.CommissionRate);
    }

    [Fact]
    public void CommissionFromRate_RefusesAnImpossibleRate()
    {
        var gross = new Money(1000m, CurrencyCode.TRY);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => PosSettlement.CommissionFromRate(gross, -0.01m));
        // Oranın tamamı komisyon olamaz: hesaba hiçbir şey geçmezdi.
        Assert.Throws<ArgumentOutOfRangeException>(
            () => PosSettlement.CommissionFromRate(gross, 1m));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => PosSettlement.CommissionFromRate(gross, 0.00001m));
    }

    [Fact]
    public void ZeroCommission_IsLegitimateAndCarriesNoExpenseCategory()
    {
        var settlement = NewSettlement(gross: 300m, commission: 0m);

        Assert.Equal(0m, settlement.CommissionAmount);
        Assert.Equal(0m, settlement.CommissionRate);
        Assert.Null(settlement.CommissionCategoryId);
        Assert.Equal(300m, settlement.NetAmount.Amount);
    }

    [Fact]
    public void CommissionAndItsCategoryAppearTogetherOrNotAtAll()
    {
        var userId = Guid.NewGuid();

        // Komisyon var, kategori yok.
        Assert.Throws<InvalidOperationException>(
            () => NewSettlement(userId: userId, commission: 10m, withCommissionCategory: false));
        // Komisyon yok, kategori var.
        Assert.Throws<InvalidOperationException>(
            () => NewSettlement(userId: userId, commission: 0m, withCommissionCategory: true));
    }

    [Fact]
    public void Commission_CannotBeNegativeOrConsumeTheWholeSettlement()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(gross: 100m, commission: -1m));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(gross: 100m, commission: 100m));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(gross: 100m, commission: 0.00001m));
    }

    [Fact]
    public void MarkTransferred_IsIdempotentAndRefusesASecondDifferentDay()
    {
        // Tahsilat iki gün önce yapıldı ki geçiş için geçerli iki ayrı gün
        // kalsın; aksi hâlde ikinci gün zaten tarih kapısına takılırdı.
        var settlement = NewSettlement(settlementDate: new DateOnly(2026, 8, 22));
        var transferredOn = new DateOnly(2026, 8, 23);

        settlement.MarkTransferred(transferredOn, CreatedAtUtc);
        settlement.MarkTransferred(transferredOn, CreatedAtUtc.AddHours(3));

        // İlk damga korunur: ikinci işaretleme yeni bir olay değildir.
        Assert.Equal(CreatedAtUtc, settlement.TransferredAtUtc);
        Assert.Equal(transferredOn, settlement.TransferredOn);
        // Para bir kez geçer: başka bir günle işaretlemek reddedilir.
        Assert.Throws<InvalidOperationException>(
            () => settlement.MarkTransferred(new DateOnly(2026, 8, 24), CreatedAtUtc));
    }

    /// <summary>
    /// 28 Eylül denetimi U12: yanlışlıkla "hesaba geçti" denen tahsilat
    /// düzeltilemiyordu; bankada olmayan para uygulamada banka bakiyesindeydi.
    /// </summary>
    [Fact]
    public void RevertTransfer_PutsTheMoneyBackOnTheRoadWithoutTouchingTheSale()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 17.9m);
        settlement.MarkTransferred(TransferDate, CreatedAtUtc);

        settlement.RevertTransfer();

        Assert.True(settlement.IsInTransit);
        Assert.Null(settlement.TransferredOn);
        Assert.Null(settlement.TransferredAtUtc);
        Assert.Equal(0m, settlement.SignedAccountEffect);
        // Satış ve komisyon tahsilat gününde tanındı; geri alma onlara dokunmaz.
        Assert.Equal(1000m, settlement.GrossAmount.Amount);
        Assert.Equal(17.9m, settlement.CommissionAmount);

        // İdempotent: yoldaki tahsilatta ikinci çağrı bir şey değiştirmez.
        settlement.RevertTransfer();
        Assert.True(settlement.IsInTransit);

        // Doğru günle yeniden işaretlenebilir.
        settlement.MarkTransferred(TransferDate, CreatedAtUtc);
        Assert.Equal(982.1m, settlement.SignedAccountEffect);
    }

    [Fact]
    public void RevertTransfer_IsRefusedOnACancelledSettlement()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 0m);
        settlement.MarkTransferred(TransferDate, CreatedAtUtc);
        settlement.Cancel(CreatedAtUtc);

        Assert.Throws<InvalidOperationException>(settlement.RevertTransfer);
    }

    [Fact]
    public void TransferDate_CannotPrecedeTheSettlementOrSitInTheFuture()
    {
        var settlement = NewSettlement();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => settlement.MarkTransferred(new DateOnly(2026, 8, 23), CreatedAtUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => settlement.MarkTransferred(new DateOnly(2026, 8, 25), CreatedAtUtc));
        Assert.False(settlement.IsTransferred);
    }

    [Fact]
    public void CancelledSettlement_LosesBothItsRecognitionAndItsCashEffect()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 20m);
        settlement.MarkTransferred(TransferDate, CreatedAtUtc);

        settlement.Cancel(CreatedAtUtc);
        settlement.Cancel(CreatedAtUtc.AddHours(1));

        Assert.True(settlement.IsCancelled);
        Assert.Equal(CreatedAtUtc, settlement.CancelledAtUtc);
        Assert.Equal(0m, settlement.SignedAccountEffect);
        Assert.False(settlement.IsInTransit);
        Assert.Throws<InvalidOperationException>(
            () => settlement.MarkTransferred(TransferDate, CreatedAtUtc));
    }

    /// <summary>
    /// Yoldaki para bir hesap değil, bir toplamdır (ADR 0015).
    /// </summary>
    [Fact]
    public void TransitBalance_SumsOnlyTheNetOfWhatHasNotArrivedYet()
    {
        var userId = Guid.NewGuid();
        var account = NewBankAccount(userId);
        var waiting = NewSettlement(userId: userId, account: account, gross: 1000m, commission: 20m);
        var alsoWaiting = NewSettlement(userId: userId, account: account, gross: 500m, commission: 0m);
        var arrived = NewSettlement(userId: userId, account: account, gross: 800m, commission: 10m);
        arrived.MarkTransferred(TransferDate, CreatedAtUtc);
        var cancelled = NewSettlement(userId: userId, account: account, gross: 400m, commission: 0m);
        cancelled.Cancel(CreatedAtUtc);

        var transit = PosTransitBalance.Calculate([waiting, alsoWaiting, arrived, cancelled]);

        // 980 + 500. Geçmiş olan hesabın kendi bakiyesinde, iptal olan hiç
        // olmadı; ikisini de saymak aynı parayı iki yerde göstermek olurdu.
        Assert.Equal(1480m, transit.Amount);
        Assert.Equal(2, transit.Count);
        Assert.True(transit.HasMoneyInTransit);
    }

    [Fact]
    public void TransitBalance_IsEmptyWhenNothingIsWaiting()
    {
        var transit = PosTransitBalance.Calculate([]);

        Assert.Equal(0m, transit.Amount);
        Assert.Equal(0, transit.Count);
        Assert.False(transit.HasMoneyInTransit);
    }

    [Fact]
    public void PosMoneyArrivesInABankAccountAndTheSaleNeedsAnIncomeCategory()
    {
        var userId = Guid.NewGuid();
        var till = new Account(Guid.NewGuid(), userId, "Kasa", AccountType.Cash, CurrencyCode.TRY);
        var closed = NewBankAccount(userId, "Kapanan banka");
        closed.Deactivate();
        var expenseCategory = new Category(
            Guid.NewGuid(), userId, "Market", CategoryType.Expense);

        Assert.Throws<InvalidOperationException>(
            () => NewSettlement(userId: userId, account: till));
        Assert.Throws<InvalidOperationException>(
            () => NewSettlement(userId: userId, account: closed));
        Assert.Throws<InvalidOperationException>(
            () => NewSettlement(userId: userId, category: expenseCategory));
        Assert.Throws<ArgumentException>(
            () => NewSettlement(userId: userId, account: NewBankAccount(Guid.NewGuid())));
    }

    [Fact]
    public void ExpectedTransferDateMayBeInTheFutureButNotBeforeTheSale()
    {
        // Beklenen geçiş günü gelecektedir; olması gereken de budur.
        var settlement = NewSettlement(expectedTransferDate: new DateOnly(2026, 8, 27));
        Assert.Equal(new DateOnly(2026, 8, 27), settlement.ExpectedTransferDate);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(expectedTransferDate: new DateOnly(2026, 8, 23)));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(settlementDate: new DateOnly(2026, 8, 25)));
    }

    [Fact]
    public void Scope_IsRequiredAndDescriptionIsOptionalTrimmedAndBounded()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NewSettlement(scope: (TransactionScope)9));
        Assert.Null(NewSettlement(description: "  ").Description);
        Assert.Equal("Öğle servisi", NewSettlement(description: " Öğle servisi ").Description);
        Assert.Throws<ArgumentException>(
            () => NewSettlement(
                description: new string('a', PosSettlement.MaximumDescriptionLength + 1)));
    }

    private static Account NewBankAccount(Guid userId, string name = "Banka") =>
        new(Guid.NewGuid(), userId, name, AccountType.Bank, CurrencyCode.TRY, 1000m);

    private static PosSettlement NewSettlement(
        Guid? userId = null,
        Account? account = null,
        Category? category = null,
        decimal gross = 1000m,
        decimal commission = 17.9m,
        bool? withCommissionCategory = null,
        TransactionScope scope = TransactionScope.Business,
        DateOnly? settlementDate = null,
        DateOnly? expectedTransferDate = null,
        string? description = null)
    {
        var owner = userId ?? account?.UserId ?? Guid.NewGuid();
        var carriesCommissionCategory = withCommissionCategory ?? commission > 0m;
        return new PosSettlement(
            Guid.NewGuid(),
            owner,
            account ?? NewBankAccount(owner),
            category ?? new Category(Guid.NewGuid(), owner, "Satış", CategoryType.Income),
            new Money(gross, CurrencyCode.TRY),
            commission,
            scope,
            settlementDate ?? SettlementDate,
            expectedTransferDate ?? SettlementDate,
            CreatedAtUtc,
            carriesCommissionCategory
                ? new Category(Guid.NewGuid(), owner, "POS komisyonu", CategoryType.Expense)
                : null,
            description);
    }
}
