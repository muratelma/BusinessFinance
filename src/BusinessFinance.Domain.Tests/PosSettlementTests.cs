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
        var userId = Guid.NewGuid();
        var account = NewBankAccount(userId);
        var settlement = NewSettlement(
            userId: userId, account: account, gross: 1000m, commission: 17.9m);

        // Tahsilat günü: gelir brüt tutar kadar tanındı, komisyon ayrı gider,
        // hesap bakiyesi kıpırdamadı — para henüz bankada değil.
        Assert.Equal(1000m, settlement.GrossAmount.Amount);
        Assert.Equal(17.9m, settlement.CommissionAmount);
        Assert.Equal(982.1m, settlement.NetAmount.Amount);
        Assert.Equal(0m, settlement.SignedAccountEffect);
        Assert.True(settlement.IsInTransit);

        Deposit(account, settlement);

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

    /// <summary>
    /// Yolda iptal edilen tahsilat tanıdığı satışı ve yoldaki parayı birlikte
    /// kaybeder. Yatışa bağlı tahsilatın iptali <see cref="PosDepositTests"/>
    /// içindedir.
    /// </summary>
    [Fact]
    public void CancelledSettlement_LosesItsRecognitionAndLeavesTheRoad()
    {
        var settlement = NewSettlement(gross: 1000m, commission: 20m);

        settlement.Cancel(CreatedAtUtc);
        settlement.Cancel(CreatedAtUtc.AddHours(1));

        Assert.True(settlement.IsCancelled);
        Assert.Equal(CreatedAtUtc, settlement.CancelledAtUtc);
        Assert.Equal(0m, settlement.SignedAccountEffect);
        Assert.False(settlement.IsInTransit);
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
        Deposit(account, arrived);
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

    /// <summary>
    /// Kartla tahsil (ADR 0019 T5) satışın yolunu paylaşır ama gelir
    /// tanımaz: kategori taşımaz, kapsamı yalnız komisyonun kapsamıdır.
    /// </summary>
    [Fact]
    public void Collection_SharesTheRoadButRecognizesNoIncome()
    {
        var userId = Guid.NewGuid();
        var account = NewBankAccount(userId);
        var commissionCategory = new Category(
            Guid.NewGuid(), userId, "POS komisyonu", CategoryType.Expense);
        var collection = PosSettlement.Collect(
            Guid.NewGuid(), userId, account, new Money(1000m, CurrencyCode.TRY), 15m,
            TransactionScope.Business, SettlementDate, SettlementDate, CreatedAtUtc,
            commissionCategory);

        Assert.Equal(PosSettlementKind.Collection, collection.Kind);
        Assert.False(collection.IsSale);
        Assert.Null(collection.CategoryId);
        Assert.Equal(985m, collection.NetAmount.Amount);
        Assert.True(collection.IsInTransit);
        Assert.Equal(0m, collection.SignedAccountEffect);

        Deposit(account, collection);
        Assert.Equal(985m, collection.SignedAccountEffect);

        // Komisyonsuz tahsilin kapsamı yoktur; komisyonlunun zorunludur.
        var free = PosSettlement.Collect(
            Guid.NewGuid(), userId, account, new Money(200m, CurrencyCode.TRY), 0m,
            null, SettlementDate, SettlementDate, CreatedAtUtc);
        Assert.Null(free.Scope);
        Assert.Throws<ArgumentNullException>(() => PosSettlement.Collect(
            Guid.NewGuid(), userId, account, new Money(200m, CurrencyCode.TRY), 3m,
            null, SettlementDate, SettlementDate, CreatedAtUtc, commissionCategory));
        Assert.Throws<InvalidOperationException>(() => PosSettlement.Collect(
            Guid.NewGuid(), userId, account, new Money(200m, CurrencyCode.TRY), 0m,
            TransactionScope.Business, SettlementDate, SettlementDate, CreatedAtUtc));
    }

    /// <summary>
    /// Kartla tahsil eden cari tahsilat hesaba dokunmaz; yalnız tahsilat yönünde,
    /// aynı tutar, gün ve hesapla kurulabilir.
    /// </summary>
    [Fact]
    public void CardCollectedPayment_LeavesTheAccountToTheDeposit()
    {
        var userId = Guid.NewGuid();
        var account = NewBankAccount(userId);
        var customer = new Counterparty(Guid.NewGuid(), userId, "Ahmet");
        var amount = new Money(500m, CurrencyCode.TRY);
        PosSettlement Collect(decimal gross = 500m, Account? on = null) => PosSettlement.Collect(
            Guid.NewGuid(), userId, on ?? account, new Money(gross, CurrencyCode.TRY), 0m, null,
            SettlementDate, SettlementDate, CreatedAtUtc);

        var collection = Collect();
        var payment = new CounterpartyPayment(
            Guid.NewGuid(), userId, customer, account, DebtDirection.Receivable, amount,
            SettlementDate, cardSettlement: collection);
        Assert.Equal(collection.Id, payment.PosSettlementId);
        Assert.Equal(0m, payment.SignedAccountEffect);

        Assert.Throws<InvalidOperationException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, customer, account, DebtDirection.Payable, amount,
            SettlementDate, cardSettlement: Collect()));
        Assert.Throws<ArgumentException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, customer, account, DebtDirection.Receivable, amount,
            SettlementDate, cardSettlement: Collect(gross: 499m)));
        Assert.Throws<ArgumentException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, customer, account, DebtDirection.Receivable, amount,
            SettlementDate, cardSettlement: Collect(on: NewBankAccount(userId, "Başka banka"))));
        Assert.Throws<InvalidOperationException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, customer, account, DebtDirection.Receivable, amount,
            SettlementDate, cardSettlement: NewSettlement(
                userId: userId, account: account, gross: 500m, commission: 0m)));
    }

    /// <summary>Para hesaba yalnız bir yatışla geçer (ADR 0019 T5).</summary>
    private static PosDeposit Deposit(Account account, PosSettlement settlement) =>
        PosDeposit.Record(
            Guid.NewGuid(), settlement.UserId, account, [settlement], settlement.NetAmount,
            TransferDate, CreatedAtUtc);

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
