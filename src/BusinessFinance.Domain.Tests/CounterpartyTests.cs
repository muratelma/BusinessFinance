namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Cari hesabın domain kuralları: ADR 0014'ün "tanır / taşır" ayrımı.
/// </summary>
public sealed class CounterpartyTests
{
    [Fact]
    public void Counterparty_TrimsNameAndNoteAndStartsActive()
    {
        var userId = Guid.NewGuid();

        var counterparty = new Counterparty(
            Guid.NewGuid(), userId, "  Ahmet Manav  ", "  Çarşı girişi  ");

        Assert.Equal("Ahmet Manav", counterparty.Name);
        Assert.Equal("Çarşı girişi", counterparty.Note);
        Assert.True(counterparty.IsActive);
    }

    [Fact]
    public void Counterparty_RejectsEmptyIdentityAndOversizedText()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            new Counterparty(Guid.Empty, userId, "Ahmet"));
        Assert.Throws<ArgumentException>(() =>
            new Counterparty(Guid.NewGuid(), Guid.Empty, "Ahmet"));
        Assert.Throws<ArgumentException>(() =>
            new Counterparty(Guid.NewGuid(), userId, "   "));
        Assert.Throws<ArgumentException>(() => new Counterparty(
            Guid.NewGuid(),
            userId,
            new string('a', Counterparty.MaximumNameLength + 1)));
        Assert.Throws<ArgumentException>(() => new Counterparty(
            Guid.NewGuid(),
            userId,
            "Ahmet",
            new string('a', Counterparty.MaximumNoteLength + 1)));
    }

    [Fact]
    public void Counterparty_NoteCanBeCleared()
    {
        var counterparty = NewCounterparty(Guid.NewGuid(), note: "Eski not");

        counterparty.SetNote(null);

        Assert.Null(counterparty.Note);
    }

    [Fact]
    public void Charge_RecognisesIncomeOrExpenseAccordingToDirection()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var expense = new Category(Guid.NewGuid(), userId, "Ticari mal alımı", CategoryType.Expense);

        var sale = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(600m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10), "  Veresiye satış  ");
        var purchase = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, expense, DebtDirection.Payable,
            new Money(400m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 11));

        Assert.Equal(counterparty.Id, sale.CounterpartyId);
        Assert.Equal(income.Id, sale.CategoryId);
        Assert.Equal("Veresiye satış", sale.Description);
        Assert.Equal(expense.Id, purchase.CategoryId);
        Assert.Equal(CategoryType.Income, CounterpartyCharge.RequiredCategoryType(DebtDirection.Receivable));
        Assert.Equal(CategoryType.Expense, CounterpartyCharge.RequiredCategoryType(DebtDirection.Payable));
    }

    [Fact]
    public void Charge_WithACategoryThatContradictsTheDirection_IsRejected()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var expense = new Category(Guid.NewGuid(), userId, "Ticari mal alımı", CategoryType.Expense);

        // Veresiye satış gelirdir; gider kategorisiyle yazılırsa rapor
        // yanlış tarafa düşerdi.
        Assert.Throws<InvalidOperationException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, expense, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));
        Assert.Throws<InvalidOperationException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Payable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void Charge_RequiresOwnedActiveCounterpartyAndCategory()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var foreignCounterparty = NewCounterparty(Guid.NewGuid());
        var foreignIncome = new Category(
            Guid.NewGuid(), Guid.NewGuid(), "Satış geliri", CategoryType.Income);

        Assert.Throws<ArgumentException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, foreignCounterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));
        Assert.Throws<ArgumentException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, foreignIncome, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));

        income.Deactivate();
        Assert.Throws<InvalidOperationException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void Charge_CarriesScopeAndRejectsAnUndefinedOne()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);

        var charge = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Personal,
            new DateOnly(2026, 8, 10));

        Assert.Equal(TransactionScope.Personal, charge.Scope);
        Assert.Throws<ArgumentOutOfRangeException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), (TransactionScope)7,
            new DateOnly(2026, 8, 10)));
    }

    [Fact]
    public void Charge_CarriesAnOptionalDueDateThatCannotPrecedeTheCharge()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var chargeDate = new DateOnly(2026, 8, 10);
        var dueDate = new DateOnly(2026, 8, 25);

        var dated = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            chargeDate, dueDate: dueDate);
        var undated = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            chargeDate);

        Assert.Equal(dueDate, dated.DueDate);
        Assert.Null(undated.DueDate);
        Assert.Throws<ArgumentOutOfRangeException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            chargeDate, dueDate: chargeDate.AddDays(-1)));
    }

    /// <summary>
    /// Tahsilat gelir/gider raporuna girmediği için bölünecek bir tarafı yok;
    /// kategori ve kapsam alanı taşımaması bilinçli (ADR 0013, ADR 0014).
    /// Alan sonradan eklenirse bu test kırılır.
    /// </summary>
    [Fact]
    public void Payment_CarriesNeitherCategoryNorScope()
    {
        var properties = typeof(CounterpartyPayment)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain("CategoryId", properties);
        Assert.DoesNotContain("Scope", properties);
    }

    [Fact]
    public void Payment_MovesTheAccountAccordingToDirection()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);

        var collection = new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
            new Money(250m, CurrencyCode.TRY), new DateOnly(2026, 8, 12));
        var settlement = new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Payable,
            new Money(100m, CurrencyCode.TRY), new DateOnly(2026, 8, 12));

        // Tahsilat kasaya para koyar, ödeme kasadan alır.
        Assert.Equal(250m, collection.SignedAccountEffect);
        Assert.Equal(-100m, settlement.SignedAccountEffect);
    }

    [Fact]
    public void Payment_RequiresAnOwnedActiveAccount()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var foreignAccount = NewAccount(Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, foreignAccount, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 12)));

        account.Deactivate();
        Assert.Throws<InvalidOperationException>(() => new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 12)));
    }

    /// <summary>
    /// Artık iş yapılmayan bir müşteri kalan borcunu ödeyebilmeli; engellemek
    /// açık bakiyeyi kapatılamaz hâle getirirdi.
    /// </summary>
    [Fact]
    public void AnInactiveCounterparty_TakesNoNewChargeButCanStillSettle()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        counterparty.Deactivate();

        Assert.Throws<InvalidOperationException>(() => new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10)));

        var settlement = new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 12));

        Assert.Equal(counterparty.Id, settlement.CounterpartyId);
    }

    [Fact]
    public void CancellingIsIdempotentAndUtcOnly()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var charge = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10));
        var payment = new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
            new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 12));
        var cancelledAt = new DateTimeOffset(2026, 8, 13, 9, 0, 0, TimeSpan.Zero);

        charge.Cancel(cancelledAt);
        charge.Cancel(cancelledAt.AddHours(2));
        payment.Cancel(cancelledAt);

        Assert.True(charge.IsCancelled);
        Assert.Equal(cancelledAt, charge.CancelledAtUtc);
        Assert.True(payment.IsCancelled);
        Assert.Throws<ArgumentException>(() => new CounterpartyPayment(
                Guid.NewGuid(), userId, counterparty, account, DebtDirection.Payable,
                new Money(10m, CurrencyCode.TRY), new DateOnly(2026, 8, 12))
            .Cancel(new DateTimeOffset(2026, 8, 13, 9, 0, 0, TimeSpan.FromHours(3))));
    }

    /// <summary>
    /// Aşamanın çıkış senaryosunun domain hâli: üç satış, iki kısmi tahsilat.
    /// </summary>
    [Fact]
    public void ThreeSalesAndTwoPartialCollections_LeaveTheRemainderOpen()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);

        var sales = new[] { 300m, 450m, 250m }
            .Select(amount => new CounterpartyCharge(
                Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
                new Money(amount, CurrencyCode.TRY), TransactionScope.Business,
                new DateOnly(2026, 8, 10)))
            .ToArray();
        var collections = new[] { 400m, 200m }
            .Select(amount => new CounterpartyPayment(
                Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
                new Money(amount, CurrencyCode.TRY), new DateOnly(2026, 8, 12)))
            .ToArray();

        var balance = CounterpartyBalance.Calculate(sales, collections);

        // 1.000 satıldı, 600 tahsil edildi: 400 açık kaldı. Gelir üç satışın
        // toplamıdır ve tahsilat onu ikinci kez artırmaz.
        Assert.Equal(400m, balance.Receivable);
        Assert.Equal(0m, balance.Payable);
        Assert.Equal(400m, balance.Net);
    }

    [Fact]
    public void BothSidesOfTheSamePersonAreKeptApart()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var expense = new Category(Guid.NewGuid(), userId, "Ticari mal alımı", CategoryType.Expense);

        var balance = CounterpartyBalance.Calculate(
            [
                new CounterpartyCharge(
                    Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
                    new Money(500m, CurrencyCode.TRY), TransactionScope.Business,
                    new DateOnly(2026, 8, 10)),
                new CounterpartyCharge(
                    Guid.NewGuid(), userId, counterparty, expense, DebtDirection.Payable,
                    new Money(300m, CurrencyCode.TRY), TransactionScope.Business,
                    new DateOnly(2026, 8, 11))
            ],
            [
                new CounterpartyPayment(
                    Guid.NewGuid(), userId, counterparty, account, DebtDirection.Payable,
                    new Money(100m, CurrencyCode.TRY), new DateOnly(2026, 8, 12))
            ]);

        // Aynı kişiyle iki taraf birden yürüyor; net tek cümle veriyor.
        Assert.Equal(500m, balance.Receivable);
        Assert.Equal(200m, balance.Payable);
        Assert.Equal(300m, balance.Net);
    }

    [Fact]
    public void CancelledMovementsLeaveTheBalanceUntouched()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);
        var sale = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(500m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10));
        var cancelledSale = new CounterpartyCharge(
            Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
            new Money(999m, CurrencyCode.TRY), TransactionScope.Business,
            new DateOnly(2026, 8, 10));
        var cancelledCollection = new CounterpartyPayment(
            Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
            new Money(123m, CurrencyCode.TRY), new DateOnly(2026, 8, 12));
        var cancelledAt = new DateTimeOffset(2026, 8, 13, 9, 0, 0, TimeSpan.Zero);
        cancelledSale.Cancel(cancelledAt);
        cancelledCollection.Cancel(cancelledAt);

        var balance = CounterpartyBalance.Calculate(
            [sale, cancelledSale],
            [cancelledCollection]);

        Assert.Equal(500m, balance.Receivable);
        Assert.Equal(500m, balance.Net);
    }

    /// <summary>
    /// Fazla tahsilat kırpılmaz: kalan, karşı tarafın bizdeki alacağıdır ve
    /// gerçektir. Sıfıra çekmek kullanıcının parasını ekranda yok ederdi.
    /// </summary>
    [Fact]
    public void OverCollectingTurnsTheSideNegativeInsteadOfClampingToZero()
    {
        var userId = Guid.NewGuid();
        var counterparty = NewCounterparty(userId);
        var account = NewAccount(userId);
        var income = new Category(Guid.NewGuid(), userId, "Satış geliri", CategoryType.Income);

        var balance = CounterpartyBalance.Calculate(
            [
                new CounterpartyCharge(
                    Guid.NewGuid(), userId, counterparty, income, DebtDirection.Receivable,
                    new Money(100m, CurrencyCode.TRY), TransactionScope.Business,
                    new DateOnly(2026, 8, 10))
            ],
            [
                new CounterpartyPayment(
                    Guid.NewGuid(), userId, counterparty, account, DebtDirection.Receivable,
                    new Money(150m, CurrencyCode.TRY), new DateOnly(2026, 8, 12))
            ]);

        Assert.Equal(-50m, balance.Receivable);
        Assert.Equal(-50m, balance.Net);
    }

    [Fact]
    public void AnEmptyLedgerIsZeroOnBothSides()
    {
        var balance = CounterpartyBalance.Calculate([], []);

        Assert.Equal(0m, balance.Receivable);
        Assert.Equal(0m, balance.Payable);
        Assert.Equal(0m, balance.Net);
    }

    /// <summary>
    /// "Bu ad zaten var mı?" sorusunun ölçüsü: harf büyüklüğü ve boşluk farkı
    /// ad farkı değildir; dört i harfi tek harftir.
    /// </summary>
    [Theory]
    [InlineData("ÖRNEK ELEKTRİK DAĞITIM A.Ş.", "Örnek Elektrik Dağıtım A.Ş.")]
    [InlineData("IŞIK MARKET", "ışık market")]
    [InlineData("IKEA", "ikea")]
    [InlineData("İkea", "IKEA")]
    [InlineData("  Ahmet   Bakkal ", "ahmet bakkal")]
    [InlineData("Çağrı Şükrü ÖZTÜRK", "çağrı şükrü öztürk")]
    [InlineData("i\u0307stanbul Toptan", "İSTANBUL TOPTAN")]
    public void NameKey_TreatsCasingAndSpacingAsTheSameName(string first, string second)
    {
        Assert.Equal(Counterparty.NameKeyOf(first), Counterparty.NameKeyOf(second));
    }

    /// <summary>
    /// Teklik bir yasaktır; yakın ama farklı adları birleştirmez. Yanlış yasak
    /// kullanıcıyı ikinci bir gerçek kişiyi açamaz bırakırdı.
    /// </summary>
    [Theory]
    [InlineData("Örnek Elektrik", "Ornek Elektrik")]
    [InlineData("Ahmet Usta", "Ahmed Usta")]
    [InlineData("Ali Kaya", "Ali Kara")]
    [InlineData("Örnek Elektrik A.Ş.", "Örnek Elektrik")]
    public void NameKey_KeepsDifferentNamesApart(string first, string second)
    {
        Assert.NotEqual(Counterparty.NameKeyOf(first), Counterparty.NameKeyOf(second));
    }

    [Fact]
    public void Counterparty_CarriesTheKeyOfItsNameAndFollowsARename()
    {
        var counterparty = new Counterparty(Guid.NewGuid(), Guid.NewGuid(), " IŞIK  Market ");

        Assert.Equal("IŞIK  Market", counterparty.Name);
        Assert.Equal("işikmarket", counterparty.NameKey);

        counterparty.Rename("Güneş Market");

        Assert.Equal("güneşmarket", counterparty.NameKey);
    }

    /// <summary>
    /// Kural sıkılaşmadan önce açılmış aynı adlı ikinci kişi ayrı bir anahtar
    /// taşır. Yalnız yazımını düzeltmek onu ilkiyle çakıştırmaz; adı gerçekten
    /// değişince sıradan bir kişi olur.
    /// </summary>
    [Fact]
    public void KeptApartCounterparty_KeepsItsKeyUntilTheNameReallyChanges()
    {
        var counterparty = new Counterparty(Guid.NewGuid(), Guid.NewGuid(), "ÖRNEK ELEKTRİK");
        counterparty.KeepApartFromSameName();
        var apart = $"örnekelektrik#{counterparty.Id:D}";

        Assert.Equal(apart, counterparty.NameKey);

        counterparty.Rename("Örnek Elektrik");

        Assert.Equal("Örnek Elektrik", counterparty.Name);
        Assert.Equal(apart, counterparty.NameKey);

        counterparty.Rename("Örnek Elektrik Şube");

        Assert.Equal("örnekelektrikşube", counterparty.NameKey);
    }

    private static Counterparty NewCounterparty(Guid userId, string? note = null) =>
        new(Guid.NewGuid(), userId, "Ahmet Manav", note);

    private static Account NewAccount(Guid userId) =>
        new(Guid.NewGuid(), userId, "Dükkân kasası", AccountType.Cash, CurrencyCode.TRY, 0m);
}
