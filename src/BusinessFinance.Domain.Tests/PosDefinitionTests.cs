namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 06.3 Grup 4: POS tanımı (ADR 0019 T4). Tanım para taşımaz; tahsilat
/// formunu doldurur ve beklenen günü iş günü seçeneğine göre hesaplar.
/// </summary>
public sealed class PosDefinitionTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);

    // 2026-10-02 cuma, 2026-10-03 cumartesi, 2026-10-05 pazartesi.
    private static readonly DateOnly Friday = new(2026, 10, 2);
    private static readonly DateOnly Saturday = new(2026, 10, 3);
    private static readonly DateOnly Monday = new(2026, 10, 5);

    [Fact]
    public void Definition_CarriesWhatThePosFormWouldOtherwiseAskEveryEvening()
    {
        var definition = NewDefinition(name: "  Ziraat POS  ", rate: 0.0179m, transferDays: 1);

        Assert.Equal("Ziraat POS", definition.Name);
        Assert.Equal(0.0179m, definition.CommissionRate);
        Assert.NotNull(definition.CommissionCategoryId);
        Assert.Equal(1, definition.TransferDays);
        Assert.True(definition.IsActive);
    }

    /// <summary>
    /// Takvim günüyle sayılan geçiş hafta sonuna düşebilir; iş günüyle sayılan
    /// düşmez. Cuma satışının "ertesi günü" pazartesidir — aksi hâlde her hafta
    /// sonu sahte bir "Gecikti" çıkardı (28 Eylül denetimi).
    /// </summary>
    [Fact]
    public void ExpectedTransferDate_SkipsWeekendsOnlyWhenBusinessDaysAreCounted()
    {
        var calendar = NewDefinition(transferDays: 1, businessDaysOnly: false);
        var business = NewDefinition(transferDays: 1, businessDaysOnly: true);

        Assert.Equal(Saturday, calendar.ExpectedTransferDate(Friday));
        Assert.Equal(Monday, business.ExpectedTransferDate(Friday));
        // Cumartesi satışı da pazartesi geçer.
        Assert.Equal(Monday, business.ExpectedTransferDate(Saturday));
        // Hafta içi: perşembe + 1 iş günü = cuma.
        Assert.Equal(Friday, business.ExpectedTransferDate(Friday.AddDays(-1)));
    }

    [Fact]
    public void ExpectedTransferDate_CountsSeveralBusinessDaysAcrossAWeekend()
    {
        var definition = NewDefinition(transferDays: 3, businessDaysOnly: true);

        // Cuma + 3 iş günü: pazartesi, salı, çarşamba.
        Assert.Equal(new DateOnly(2026, 10, 7), definition.ExpectedTransferDate(Friday));
    }

    /// <summary>
    /// Aynı gün geçen POS (0 gün) hafta sonu satışında yine de hafta içine
    /// taşınır; hafta içi satışta aynı gün kalır.
    /// </summary>
    [Fact]
    public void ExpectedTransferDate_WithZeroDaysStaysOnTheSaleDayUnlessItIsAWeekend()
    {
        var business = NewDefinition(rate: 0m, transferDays: 0, businessDaysOnly: true);
        var calendar = NewDefinition(rate: 0m, transferDays: 0, businessDaysOnly: false);

        Assert.Equal(Friday, business.ExpectedTransferDate(Friday));
        Assert.Equal(Monday, business.ExpectedTransferDate(Saturday));
        Assert.Equal(Saturday, calendar.ExpectedTransferDate(Saturday));
    }

    /// <summary>
    /// Oran varsa komisyonun yazılacağı kategori de vardır; sıfır oranlı tanım
    /// (komisyon kesmeyen POS) kategorisiz olabilir.
    /// </summary>
    [Fact]
    public void CommissionRate_NeedsAnExpenseCategoryOnlyWhenItIsAboveZero()
    {
        Assert.Throws<InvalidOperationException>(
            () => NewDefinition(rate: 0.02m, withCommissionCategory: false));

        var free = NewDefinition(rate: 0m, withCommissionCategory: false);
        Assert.Null(free.CommissionCategoryId);
        Assert.Equal(0m, free.CommissionRate);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1)]
    [InlineData(0.01795)]
    public void CommissionRate_MustBeBelowOneAndCarryAtMostFourDecimals(double rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewDefinition(rate: (decimal)rate));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(PosDefinition.MaximumTransferDays + 1)]
    public void TransferDays_StayWithinTheSupportedRange(int days)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewDefinition(transferDays: days));
    }

    [Fact]
    public void Definition_RejectsWhatASettlementCouldNotBeWrittenWith()
    {
        var owner = Guid.NewGuid();
        var till = new Account(
            Guid.NewGuid(), owner, "Kasa", AccountType.Cash, CurrencyCode.TRY, 0m);
        var expense = new Category(Guid.NewGuid(), owner, "Gider", CategoryType.Expense);
        var income = new Category(Guid.NewGuid(), owner, "Satış", CategoryType.Income);

        // Kart parası kasaya geçmez.
        Assert.Throws<InvalidOperationException>(() => NewDefinition(owner, account: till));
        // Satış bir gelir kategorisine, komisyon bir gider kategorisine yazılır.
        Assert.Throws<InvalidOperationException>(
            () => NewDefinition(owner, salesCategory: expense));
        Assert.Throws<InvalidOperationException>(
            () => NewDefinition(owner, commissionCategory: income));
        Assert.Throws<ArgumentException>(() => NewDefinition(owner, name: "   "));
        Assert.Throws<ArgumentException>(
            () => NewDefinition(owner, name: new string('a', PosDefinition.MaximumNameLength + 1)));
    }

    [Fact]
    public void Definition_RejectsAnotherUsersAccountOrCategory()
    {
        var owner = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        var foreignAccount = new Account(
            Guid.NewGuid(), stranger, "Banka", AccountType.Bank, CurrencyCode.TRY, 0m);
        var foreignCategory = new Category(
            Guid.NewGuid(), stranger, "Satış", CategoryType.Income);

        Assert.Throws<ArgumentException>(() => NewDefinition(owner, account: foreignAccount));
        Assert.Throws<ArgumentException>(
            () => NewDefinition(owner, salesCategory: foreignCategory));
    }

    /// <summary>
    /// Düzenleme tanımı değiştirir; pasifleştirme tanımı silmez. İkisi de
    /// geçmiş tahsilatlara dokunmaz (onlar kendi tutarlarını taşır).
    /// </summary>
    [Fact]
    public void UpdateAndDeactivate_ChangeTheDefinitionOnly()
    {
        var owner = Guid.NewGuid();
        var definition = NewDefinition(owner, rate: 0.0179m, transferDays: 1);
        var otherBank = new Account(
            Guid.NewGuid(), owner, "Garanti", AccountType.Bank, CurrencyCode.TRY, 0m);
        var sales = new Category(Guid.NewGuid(), owner, "Paket satış", CategoryType.Income);
        var commission = new Category(Guid.NewGuid(), owner, "Komisyon", CategoryType.Expense);

        definition.Update("Yemek kartı", otherBank, sales, 0.065m, commission, 20, false);
        definition.SetActive(false);

        Assert.Equal("Yemek kartı", definition.Name);
        Assert.Equal(otherBank.Id, definition.AccountId);
        Assert.Equal(sales.Id, definition.SalesCategoryId);
        Assert.Equal(0.065m, definition.CommissionRate);
        Assert.Equal(20, definition.TransferDays);
        Assert.False(definition.BusinessDaysOnly);
        Assert.False(definition.IsActive);
    }

    /// <summary>
    /// Ana POS işareti pasifleştirmeyle düşer ve pasif POS'a konamaz: pasif POS
    /// yeni tahsilatta zaten kullanılamaz.
    /// </summary>
    [Fact]
    public void DefaultFlag_CannotStayOnAnInactiveDefinition()
    {
        var definition = NewDefinition();
        Assert.False(definition.IsDefault);

        definition.SetDefault(true);
        Assert.True(definition.IsDefault);

        definition.SetActive(false);
        Assert.False(definition.IsDefault);
        Assert.Throws<InvalidOperationException>(() => definition.SetDefault(true));
    }

    /// <summary>
    /// Tahsilat hangi tanımla yazıldığını taşır; tanımsız yazılanda bağ boştur.
    /// Başka kullanıcının tanımına bağlanamaz.
    /// </summary>
    [Fact]
    public void Settlement_RemembersTheDefinitionItWasWrittenWith()
    {
        var owner = Guid.NewGuid();
        var account = new Account(
            Guid.NewGuid(), owner, "Banka", AccountType.Bank, CurrencyCode.TRY, 0m);
        var sales = new Category(Guid.NewGuid(), owner, "Satış", CategoryType.Income);
        var definition = NewDefinition(owner, account: account, salesCategory: sales, rate: 0m,
            withCommissionCategory: false);

        PosSettlement Settle(PosDefinition? with, Guid? userId = null) => new(
            Guid.NewGuid(), userId ?? owner, account, sales,
            new Money(100m, CurrencyCode.TRY), 0m, TransactionScope.Business,
            Friday.AddDays(-7), Friday.AddDays(-6), CreatedAtUtc,
            definition: with);

        Assert.Equal(definition.Id, Settle(definition).PosDefinitionId);
        Assert.Null(Settle(null).PosDefinitionId);
        Assert.Throws<ArgumentException>(() => Settle(NewDefinition(Guid.NewGuid())));
    }

    private static PosDefinition NewDefinition(
        Guid? userId = null,
        string name = "Ziraat POS",
        Account? account = null,
        Category? salesCategory = null,
        Category? commissionCategory = null,
        decimal rate = 0.0179m,
        bool? withCommissionCategory = null,
        int transferDays = 1,
        bool businessDaysOnly = true)
    {
        var owner = userId ?? Guid.NewGuid();
        var carriesCategory = withCommissionCategory ?? true;
        return new PosDefinition(
            Guid.NewGuid(),
            owner,
            name,
            account ?? new Account(
                Guid.NewGuid(), owner, "Banka", AccountType.Bank, CurrencyCode.TRY, 0m),
            salesCategory ?? new Category(Guid.NewGuid(), owner, "Satış", CategoryType.Income),
            rate,
            commissionCategory ?? (carriesCategory
                ? new Category(Guid.NewGuid(), owner, "POS komisyonu", CategoryType.Expense)
                : null),
            transferDays,
            businessDaysOnly,
            CreatedAtUtc);
    }
}
