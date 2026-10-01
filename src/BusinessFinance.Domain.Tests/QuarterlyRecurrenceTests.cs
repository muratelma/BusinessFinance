using BusinessFinance.Domain;

namespace BusinessFinance.Domain.Tests;

/// <summary>
/// Aşama 05 Grup 4: takvim kalemleri mevcut tekrarlayan plan altyapısının
/// üstüne kuruluyor. Üç aylık ritim (geçici vergi) aynı yolun üç adımlık hâli;
/// dönemin tutarı gerçekleşmeden önce düzeltilebiliyor (ADR 0016).
/// </summary>
public sealed class QuarterlyRecurrenceTests
{
    private static readonly DateTimeOffset RealizedAtUtc =
        new(2026, 8, 26, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void QuarterlySchedule_StepsThreeMonthsAtATime()
    {
        var plan = NewPlan(RecurrenceFrequency.Quarterly, new DateOnly(2026, 2, 17));

        Assert.Equal(new DateOnly(2026, 2, 17), plan.NextOccurrenceDate);

        var second = plan.GetFollowingDate(new DateOnly(2026, 2, 17));
        var third = plan.GetFollowingDate(second!.Value);

        Assert.Equal(new DateOnly(2026, 5, 17), second);
        Assert.Equal(new DateOnly(2026, 8, 17), third);
    }

    /// <summary>
    /// Ay sonu davranışı aylık planla aynı kuralı izler: kısa ayda son güne
    /// çekilir.
    /// </summary>
    [Fact]
    public void QuarterlySchedule_ClampsToTheLastDayOfAShortMonth()
    {
        var plan = NewPlan(RecurrenceFrequency.Quarterly, new DateOnly(2026, 11, 30));

        var next = plan.GetFollowingDate(new DateOnly(2026, 11, 30));

        Assert.Equal(new DateOnly(2027, 2, 28), next);
    }

    /// <summary>
    /// Plandaki tutar bir beklentidir: bekleyen occurrence gerçek tutarla
    /// düzeltilebilir ve <b>plan değişmez</b>.
    /// </summary>
    [Fact]
    public void PlannedOccurrence_CanBeCorrectedToTheAmountTheUserActuallyOwes()
    {
        var plan = NewPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 28));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), plan, new DateOnly(2026, 8, 28));

        occurrence.CorrectAmount(new Money(2450.75m, CurrencyCode.TRY));

        Assert.Equal(2450.75m, occurrence.Amount!.Amount);
        Assert.Equal(1000m, plan.Amount!.Amount);
    }

    /// <summary>
    /// Gerçekleşmiş occurrence geçmiştir: düzeltmesi iptal + yeni kayıttır,
    /// tutarın üstüne yazmak değil.
    /// </summary>
    [Fact]
    public void RealizedOccurrence_CannotHaveItsAmountRewritten()
    {
        var plan = NewPlan(RecurrenceFrequency.Monthly, new DateOnly(2026, 8, 28));
        var occurrence = RecurringTransactionOccurrence.Create(
            Guid.NewGuid(), plan, new DateOnly(2026, 8, 28));
        occurrence.RealizeWithTransaction(Guid.NewGuid(), RealizedAtUtc);

        Assert.Throws<InvalidOperationException>(
            () => occurrence.CorrectAmount(new Money(2450.75m, CurrencyCode.TRY)));
        Assert.Equal(1000m, occurrence.Amount!.Amount);
    }

    private static RecurringTransaction NewPlan(
        RecurrenceFrequency frequency,
        DateOnly startDate)
    {
        var userId = Guid.NewGuid();
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            new Account(Guid.NewGuid(), userId, "Banka", AccountType.Bank, CurrencyCode.TRY, 10000m),
            new Category(Guid.NewGuid(), userId, "SGK ve vergi ödemesi", CategoryType.Expense),
            new Money(1000m, CurrencyCode.TRY),
            RecurringTransactionKind.Expense,
            TransactionScope.Business,
            frequency,
            startDate);
    }
}
