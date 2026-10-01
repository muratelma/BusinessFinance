namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Vergi bir nakit planıdır (ADR 0018): tekrarlayan planın vergi biçimi, "seçilen
/// aylarda" ritmi, tutarsız kalem, ödeme anında seçilen kaynak, toplu ödemeyle
/// kapatma ve geri alma.
/// </summary>
public sealed class TaxPlanTests
{
    private static readonly Guid UserId = Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb");
    private static readonly DateTimeOffset Now = new(2026, 9, 30, 10, 0, 0, TimeSpan.Zero);

    private static readonly int MayAndNovember = Mask(5, 11);

    [Fact]
    public void SelectedMonths_FallOnlyInTheChosenMonths_OnTheLastDay()
    {
        var plan = TaxPlan(RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 5, 31), 31, MayAndNovember);

        var dates = Walk(plan, 4);

        Assert.Equal(
            [
                new DateOnly(2026, 5, 31),
                new DateOnly(2026, 11, 30),
                new DateOnly(2027, 5, 31),
                new DateOnly(2027, 11, 30)
            ],
            dates);
    }

    /// <summary>
    /// Geçici vergi: Şubat, Mayıs, Ağustos, Kasım, ayın 17'si; yıl dönümünü aşar.
    /// </summary>
    [Fact]
    public void SelectedMonths_CrossTheYearBoundary()
    {
        var plan = TaxPlan(
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 8, 17), 17, Mask(2, 5, 8, 11));

        Assert.Equal(
            [
                new DateOnly(2026, 8, 17),
                new DateOnly(2026, 11, 17),
                new DateOnly(2027, 2, 17),
                new DateOnly(2027, 5, 17)
            ],
            Walk(plan, 4));
    }

    /// <summary>
    /// "Atla" davranışında ayın gününe sığmayan seçili ay geçilir.
    /// </summary>
    [Fact]
    public void SelectedMonths_WithSkip_PassOverAMonthTooShort()
    {
        var plan = new RecurringTransaction(
            Guid.NewGuid(), UserId, TaxCategory(), null, TaxKind.Custom, TransactionScope.Business,
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 1, 31),
            monthEndBehavior: MonthEndBehavior.SkipInvalidPeriod, description: "Özel",
            dayOfMonth: 31, selectedMonths: Mask(1, 2, 3));

        Assert.Equal(
            [new DateOnly(2026, 1, 31), new DateOnly(2026, 3, 31), new DateOnly(2027, 1, 31)],
            Walk(plan, 3));
    }

    /// <summary>
    /// "Ay sonu" başlangıç günü 30 olsa bile ayın son günüdür: Bağkur 30 Eylül'de
    /// başlarsa sonraki kalem 30 Ekim değil 31 Ekim'dir.
    /// </summary>
    [Fact]
    public void MonthEnd_IsTheLastDayEvenWhenTheStartMonthIsShort()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);

        Assert.Equal(
            [
                new DateOnly(2026, 9, 30),
                new DateOnly(2026, 10, 31),
                new DateOnly(2026, 11, 30),
                new DateOnly(2027, 2, 28)
            ],
            Walk(plan, 6).Where((_, index) => index is 0 or 1 or 2 or 5));
    }

    [Theory]
    [InlineData(2026, 6, 30)]  // Haziran seçili değil
    [InlineData(2026, 5, 17)]  // Mayıs seçili ama gün 31
    public void AStartOffTheRhythm_IsRejected(int year, int month, int day)
    {
        Assert.Throws<ArgumentException>(() => TaxPlan(
            RecurrenceFrequency.SelectedMonths, new DateOnly(year, month, day), 31, MayAndNovember));
    }

    [Fact]
    public void SelectedMonths_RequireAtLeastOneMonth_AndBelongOnlyToThatRhythm()
    {
        Assert.Throws<ArgumentException>(() => TaxPlan(
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 5, 31), 31, 0));
        Assert.Throws<ArgumentException>(() => TaxPlan(
            RecurrenceFrequency.Monthly, new DateOnly(2026, 5, 31), 31, MayAndNovember));
    }

    /// <summary>
    /// Kaynaksız ve tutarsız plan yalnız vergidir (ADR 0018 T3, T4); sıradan bir
    /// planın kaynağı ve tutarı hâlâ zorunludur.
    /// </summary>
    [Fact]
    public void OnlyATaxPlan_MayLackAnAmount()
    {
        var account = new Account(Guid.NewGuid(), UserId, "Banka", AccountType.Bank, CurrencyCode.TRY);

        var tax = new RecurringTransaction(
            Guid.NewGuid(), UserId, account, TaxCategory(), null, RecurringTransactionKind.Expense,
            TransactionScope.Business, RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 28),
            description: "KDV", taxKind: TaxKind.VatReturn);

        Assert.Null(tax.Amount);
        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(), UserId, account, TaxCategory(), null, RecurringTransactionKind.Expense,
            TransactionScope.Business, RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 28),
            description: "Kira"));
    }

    [Fact]
    public void ATaxPlan_NeedsAName_AndMustBeAnExpense()
    {
        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(), UserId, TaxCategory(), null, TaxKind.VatReturn, TransactionScope.Business,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 28)));

        var account = new Account(Guid.NewGuid(), UserId, "Banka", AccountType.Bank, CurrencyCode.TRY);
        var income = new Category(Guid.NewGuid(), UserId, "İade", CategoryType.Income);
        Assert.Throws<ArgumentException>(() => new RecurringTransaction(
            Guid.NewGuid(), UserId, account, income, new Money(10m, CurrencyCode.TRY),
            RecurringTransactionKind.Income, TransactionScope.Business, RecurrenceFrequency.Monthly,
            new DateOnly(2026, 9, 28), description: "İade", taxKind: TaxKind.Custom));
    }

    [Fact]
    public void OnlyAnExpenseCategory_CanBeMarkedAsTax()
    {
        var expense = new Category(Guid.NewGuid(), UserId, "Harçlar", CategoryType.Expense, isTax: true);
        var income = new Category(Guid.NewGuid(), UserId, "İade", CategoryType.Income);

        Assert.True(expense.IsTax);
        Assert.Throws<ArgumentException>(() => income.SetTax(true));
        expense.SetTax(false);
        Assert.False(expense.IsTax);
    }

    /// <summary>
    /// Tutarı bilinmeyen kalem tutarsız gerçekleşemez (İ1, İ5); tutar yazıldıktan
    /// sonra ödeme anında seçilen kaynaktan gerçekleşir.
    /// </summary>
    [Fact]
    public void AnItemWithoutAmount_CannotBeRealizedUntilTheAmountIsKnown()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);
        var item = RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, new DateOnly(2026, 9, 30));
        var accountId = Guid.NewGuid();

        Assert.Null(item.SourceType);
        item.UseAccount(accountId);
        Assert.Throws<InvalidOperationException>(() => item.RealizeWithTransaction(Guid.NewGuid(), Now));

        item.CorrectAmount(new Money(8950m, CurrencyCode.TRY));
        var transactionId = Guid.NewGuid();
        item.RealizeWithTransaction(transactionId, Now);

        Assert.Equal(RecurringOccurrenceStatus.Realized, item.Status);
        Assert.Equal(accountId, item.AccountId);
        Assert.Equal(transactionId, item.BudgetTransactionId);
    }

    /// <summary>
    /// Toplu ödemeyle kapatılan kalemin kendi sonucu yoktur ve ikinci bir sonuç
    /// alamaz (İ7); aynı ödemeyle tekrar kapatmak etkisizdir.
    /// </summary>
    [Fact]
    public void AClosedItem_HasNoResultOfItsOwn_AndTakesNoSecondOne()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);
        var item = RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, new DateOnly(2026, 9, 30));
        var paymentId = Guid.NewGuid();

        item.CloseWithTransaction(paymentId, Now);
        item.CloseWithTransaction(paymentId, Now);

        Assert.Equal(RecurringOccurrenceStatus.Closed, item.Status);
        Assert.Equal(paymentId, item.ClosedByTransactionId);
        Assert.Null(item.BudgetTransactionId);
        Assert.Throws<InvalidOperationException>(() => item.CloseWithTransaction(Guid.NewGuid(), Now));
        Assert.Throws<InvalidOperationException>(() => item.CorrectAmount(new Money(1m, CurrencyCode.TRY)));
        Assert.Throws<InvalidOperationException>(() => item.UseAccount(Guid.NewGuid()));
    }

    /// <summary>
    /// Geri alma kalemi bekleyene döndürür; bağlar temizlenir, yazılan tutar kalır.
    /// </summary>
    [Fact]
    public void Reopen_ReturnsASettledItemToPending()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);
        var realized = RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, new DateOnly(2026, 9, 30));
        realized.CorrectAmount(new Money(500m, CurrencyCode.TRY));
        realized.UseCreditCard(Guid.NewGuid());
        realized.RealizeWithCharge(Guid.NewGuid(), Now);

        realized.Reopen();
        realized.Reopen();

        Assert.Equal(RecurringOccurrenceStatus.Planned, realized.Status);
        Assert.Null(realized.CreditCardChargeId);
        Assert.Null(realized.RealizedAtUtc);
        Assert.Equal(500m, realized.Amount!.Amount);
    }

    /// <summary>
    /// Düzenleme bekleyen kalemi plana uydurur; kullanıcının o dönem için ayrıca
    /// yazdığı tutar korunur.
    /// </summary>
    [Fact]
    public void FollowPlan_KeepsAnAmountWrittenForThatPeriod()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);
        var fromPlan = RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, new DateOnly(2026, 9, 30));
        plan.AdvanceAfter(new DateOnly(2026, 9, 30));
        var written = RecurringTransactionOccurrence.Create(Guid.NewGuid(), plan, new DateOnly(2026, 10, 31));
        written.CorrectAmount(new Money(4000m, CurrencyCode.TRY));

        var previous = plan.AmountValue;
        plan.Update(TaxCategory(plan), new Money(9000m, CurrencyCode.TRY), TransactionScope.Personal,
            "Bağkur (yeni)", null, null);
        fromPlan.FollowPlan(plan, previous);
        written.FollowPlan(plan, previous);

        Assert.Equal(9000m, fromPlan.Amount!.Amount);
        Assert.Equal(4000m, written.Amount!.Amount);
        Assert.Equal(TransactionScope.Personal, written.Scope);
        Assert.Equal("Bağkur (yeni)", written.Description);
    }

    [Fact]
    public void Reschedule_MustStartAfterTheLastSettledItem()
    {
        var plan = TaxPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 9, 30), 31, null);

        Assert.Throws<ArgumentOutOfRangeException>(() => plan.Reschedule(
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 5, 31), null,
            MonthEndBehavior.ClampToLastDay, 31, MayAndNovember, new DateOnly(2026, 9, 30), 1));

        plan.Reschedule(
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 11, 30), null,
            MonthEndBehavior.ClampToLastDay, 31, MayAndNovember, new DateOnly(2026, 9, 30), 1);

        Assert.Equal(new DateOnly(2026, 11, 30), plan.NextOccurrenceDate);
        Assert.Equal(1, plan.GeneratedOccurrenceCount);
        Assert.Equal(new DateOnly(2027, 5, 31), plan.GetFollowingDate(new DateOnly(2026, 11, 30)));
    }

    private static int Mask(params int[] months) => months.Sum(month => 1 << (month - 1));

    private static Category TaxCategory() =>
        new(Guid.NewGuid(), UserId, "Vergi ve harç", CategoryType.Expense, TransactionScope.Personal, isTax: true);

    private static Category TaxCategory(RecurringTransaction plan) =>
        new(plan.CategoryId, UserId, "Vergi ve harç", CategoryType.Expense, TransactionScope.Personal, isTax: true);

    private static RecurringTransaction TaxPlan(
        RecurrenceFrequency frequency,
        DateOnly startDate,
        int? dayOfMonth,
        int? selectedMonths) =>
        new(Guid.NewGuid(), UserId, TaxCategory(), null, TaxKind.SocialSecurityPremium, TransactionScope.Business,
            frequency, startDate, description: "Bağkur", dayOfMonth: dayOfMonth, selectedMonths: selectedMonths);

    private static List<DateOnly> Walk(RecurringTransaction plan, int count)
    {
        var dates = new List<DateOnly>();
        var date = plan.NextOccurrenceDate;
        while (date is DateOnly current && dates.Count < count)
        {
            dates.Add(current);
            date = plan.GetFollowingDate(current);
        }

        return dates;
    }
}

