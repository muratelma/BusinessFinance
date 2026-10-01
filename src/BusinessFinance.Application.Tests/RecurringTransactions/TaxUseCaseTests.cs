using BusinessFinance.Application.Abstractions.Identifiers;
using BusinessFinance.Application.FinancialActivities;
using BusinessFinance.Application.RecurringTransactions;
using BusinessFinance.Application.Taxes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Tests.RecurringTransactions;

/// <summary>
/// Vergi bir nakit planıdır (ADR 0018): toplu ödeme, "Ödedim", geri alma ve
/// ritim değişikliği kullanım senaryoları.
/// </summary>
public sealed class TaxUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("cccccccc-cccc-4ccc-8ccc-cccccccccccc");
    private static readonly DateTimeOffset Now = new(2026, 9, 30, 9, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Aynı istek iki kez gelirse ikinci gider yazılmaz; ilk ödeme döner.
    /// </summary>
    [Fact]
    public async Task ATaxPaymentSentTwice_IsWrittenOnce()
    {
        var world = new World();
        var command = world.Payment(Guid.NewGuid(), []);

        var first = await world.CreatePayment().ExecuteAsync(command);
        var second = await world.CreatePayment().ExecuteAsync(command);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(first.Value.PaymentId, second.Value.PaymentId);
        Assert.Equal(1, world.Payments.AddCount);
        var expense = Assert.Single(world.Transactions.Items);
        Assert.Equal(new DateOnly(2026, 9, 28), expense.TransactionDate);
        Assert.Equal(TransactionScope.Personal, expense.Scope);
    }

    /// <summary>
    /// İki kullanıcının aynı istek kimliği iki ayrı ödeme kimliği üretir.
    /// </summary>
    [Fact]
    public void TheSameRequestIdOfTwoUsers_GivesTwoPaymentIds()
    {
        var requestId = Guid.NewGuid();

        Assert.NotEqual(
            RequestScopedId.Create("tax-payment", UserId, requestId),
            RequestScopedId.Create("tax-payment", Guid.NewGuid(), requestId));
        Assert.Equal(
            RequestScopedId.Create("tax-payment", UserId, requestId),
            RequestScopedId.Create("tax-payment", UserId, requestId));
    }

    /// <summary>
    /// Ödenmemiş vergi hiçbir toplamı etkilemez (İ3): ileri tarihli ödeme reddedilir.
    /// </summary>
    [Fact]
    public async Task ATaxPaymentInTheFuture_IsRefused()
    {
        var world = new World();
        var command = world.Payment(Guid.NewGuid(), []) with { PaidOn = new DateOnly(2026, 10, 5) };

        var result = await world.CreatePayment().ExecuteAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("tax_payments.paid_on_in_future", result.Error.Code);
        Assert.Empty(world.Transactions.Items);
    }

    /// <summary>
    /// Toplu ödeme seçilen kalemleri, henüz üretilmemiş olsalar bile, kapatır.
    /// </summary>
    [Fact]
    public async Task ATaxPayment_ClosesTheChosenItems()
    {
        var world = new World();
        var plan = world.AddTaxPlan(new DateOnly(2026, 8, 31));

        var result = await world.CreatePayment().ExecuteAsync(world.Payment(
            Guid.NewGuid(),
            [new TaxItemReference(plan.Id, new DateOnly(2026, 8, 31)),
             new TaxItemReference(plan.Id, new DateOnly(2026, 9, 30))]));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.ClosedItems.Count);
        Assert.All(world.Recurring.Occurrences, item =>
        {
            Assert.Equal(RecurringOccurrenceStatus.Closed, item.Status);
            Assert.Equal(result.Value.PaymentId, item.ClosedByTransactionId);
        });
    }

    /// <summary>
    /// "Ödedim" kartla ödenince kart harcaması, ödeme gününe yazılır (T4);
    /// vadesi gelmemiş kalem ödeme günü verildiğinde ödenebilir.
    /// </summary>
    [Fact]
    public async Task PayingWithACard_WritesACardChargeOnThePaymentDay()
    {
        var world = new World();
        var plan = world.AddTaxPlan(new DateOnly(2026, 10, 31));

        var result = await world.RealizeDue().ExecuteAsync(new RealizeDueRecurringCommand(
            plan.Id,
            new DateOnly(2026, 10, 31),
            3240m,
            new RecurringPaymentDetails(new DateOnly(2026, 9, 29), CreditCardId: world.Card.Id)));

        Assert.True(result.IsSuccess);
        Assert.Equal(RecurringSourceType.CreditCard, result.Value.SourceType);
        var charge = Assert.Single(world.Charges.Items);
        Assert.Equal(new DateOnly(2026, 9, 29), charge.ChargeDate);
        Assert.Equal(3240m, charge.Amount.Amount);
        Assert.Empty(world.Transactions.Items);
    }

    /// <summary>
    /// Kapatılmış kalem kendi ekranından geri alınmaz; geri alınacak şey onu
    /// kapatan ödemedir.
    /// </summary>
    [Fact]
    public async Task UndoingAClosedItem_PointsAtItsPayment()
    {
        var world = new World();
        var plan = world.AddTaxPlan(new DateOnly(2026, 8, 31));
        var payment = await world.CreatePayment().ExecuteAsync(world.Payment(
            Guid.NewGuid(), [new TaxItemReference(plan.Id, new DateOnly(2026, 8, 31))]));

        var result = await world.Undo().ExecuteAsync(
            new UndoRecurringOccurrenceCommand(world.Recurring.Occurrences.Single().Id));
        var undonePayment = await world.UndoPayment().ExecuteAsync(payment.Value.PaymentId);

        Assert.Equal("recurring.closed_by_payment", result.Error.Code);
        Assert.True(undonePayment.IsSuccess);
        Assert.True(undonePayment.Value.IsCancelled);
        Assert.Equal(RecurringOccurrenceStatus.Planned, world.Recurring.Occurrences.Single().Status);
    }

    /// <summary>
    /// Ritim değişince bekleyen kalemler yeniden kurulur; ödenmiş kalem geçmiştir
    /// ve yerinde kalır.
    /// </summary>
    [Fact]
    public async Task ChangingTheRhythm_RebuildsPendingItemsAndKeepsPaidOnes()
    {
        var world = new World();
        var plan = world.AddTaxPlan(new DateOnly(2026, 8, 31));
        await world.RealizeDue().ExecuteAsync(new RealizeDueRecurringCommand(
            plan.Id, new DateOnly(2026, 8, 31), 100m,
            new RecurringPaymentDetails(new DateOnly(2026, 8, 31), world.Account.Id)));
        await world.SetAmount().ExecuteAsync(new SetOccurrenceAmountCommand(plan.Id, new DateOnly(2026, 10, 31), 200m));

        var result = await world.Update().ExecuteAsync(new UpdateRecurringTransactionCommand(
            plan.Id, null, null, null, world.TaxCategory.Id, null, null, "Emlak",
            RecurrenceFrequency.SelectedMonths, new DateOnly(2026, 11, 30), null,
            MonthEndBehavior.ClampToLastDay, 31, 1 << 4 | 1 << 10));

        Assert.True(result.IsSuccess);
        var kept = Assert.Single(world.Recurring.Occurrences);
        Assert.Equal(new DateOnly(2026, 8, 31), kept.ScheduledDate);
        Assert.Equal(RecurringOccurrenceStatus.Realized, kept.Status);
        Assert.Equal(2, world.Recurring.Removed.Count);
        Assert.Equal(new DateOnly(2026, 11, 30), result.Value.NextOccurrenceDate);
        Assert.Equal(1, result.Value.GeneratedOccurrenceCount);
    }

    /// <summary>
    /// Ödeme kaynağının etiketi vergide kapsamı belirlemez: işletme vergisi
    /// şahsi hesapla ödenebilir (ADR 0018 İ9, 30 Eylül 2026).
    /// </summary>
    [Fact]
    public async Task ATaxPaymentFromAPersonalAccount_StaysOnTheProfileSide()
    {
        var world = new World(hasBusiness: true);
        world.Account.SetDefaultScope(TransactionScope.Personal);

        var result = await world.CreatePayment().ExecuteAsync(world.Payment(Guid.NewGuid(), []));

        Assert.True(result.IsSuccess);
        Assert.Equal(TransactionScope.Business, Assert.Single(world.Transactions.Items).Scope);
    }

    /// <summary>
    /// Vergi tanımında da kaynağın etiketine bakılmaz: seçim yoksa profilin
    /// tarafı, varsa seçim.
    /// </summary>
    [Fact]
    public async Task ATaxPlanOnAPersonalCard_TakesTheChoiceOrTheProfileSide()
    {
        var world = new World(hasBusiness: true);
        world.Card.SetDefaultScope(TransactionScope.Personal);

        var implicitScope = await world.CreatePlan().ExecuteAsync(world.PlanCommand() with
        {
            SourceType = RecurringSourceType.CreditCard,
            CreditCardId = world.Card.Id
        });
        var chosen = await world.CreatePlan().ExecuteAsync(world.PlanCommand() with
        {
            Scope = TransactionScope.Personal
        });

        Assert.True(implicitScope.IsSuccess);
        Assert.Equal(TransactionScope.Business, implicitScope.Value.Scope);
        Assert.Equal(TransactionScope.Personal, chosen.Value.Scope);
    }

    /// <summary>"Vergilerimi tanımla": hepsi tek yazmayla kurulur.</summary>
    [Fact]
    public async Task CreatingSeveralTaxPlans_WritesThemTogether()
    {
        var world = new World(hasBusiness: true);

        var result = await world.CreatePlans().ExecuteAsync(
        [
            world.PlanCommand(),
            world.PlanCommand() with
            {
                TaxKind = TaxKind.VatReturn,
                Description = "KDV",
                DayOfMonth = 28,
                StartDate = new DateOnly(2026, 10, 28)
            }
        ]);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(1, world.Recurring.AddRangeCalls);
        Assert.Equal(2, world.Recurring.Recurring.Count);
        Assert.All(result.Value, plan => Assert.Equal(TransactionScope.Business, plan.Scope));
    }

    /// <summary>Biri geçersizse hiçbiri yazılmaz; yarım kalmış bir liste olmaz.</summary>
    [Fact]
    public async Task CreatingSeveralTaxPlans_WithOneInvalid_WritesNone()
    {
        var world = new World(hasBusiness: true);

        var result = await world.CreatePlans().ExecuteAsync(
            [world.PlanCommand(), world.PlanCommand() with { CategoryId = Guid.NewGuid() }]);

        Assert.False(result.IsSuccess);
        Assert.Empty(world.Recurring.Recurring);
        Assert.Equal(0, world.Recurring.AddRangeCalls);
    }

    /// <summary>Toplu tanımlama yalnız vergi kurar.</summary>
    [Fact]
    public async Task CreatingSeveralTaxPlans_RefusesAnOrdinaryPlan()
    {
        var world = new World(hasBusiness: true);

        var result = await world.CreatePlans().ExecuteAsync(
            [world.PlanCommand() with { TaxKind = null, Amount = 10m }]);

        Assert.Equal("taxes.plan_not_tax", result.Error.Code);
        Assert.Empty(world.Recurring.Recurring);
    }

    /// <summary>
    /// Vergi ekranının toplamı gecikenleri de içerir ("Gecikenler ve 30 gün");
    /// tutarı belli olmayanlar toplama girmez, sayıları ayrıca döner. Vergi
    /// olmayan planlanan satır listeye girmez.
    /// </summary>
    [Fact]
    public async Task TheTaxOverview_TotalsPendingItemsIncludingOverdue()
    {
        var world = new World();
        var planned = new StubPlanned(
            Pending(new DateOnly(2026, 9, 28), PlannedActivityTiming.Overdue, null),
            Pending(new DateOnly(2026, 9, 29), PlannedActivityTiming.Overdue, 1_000m),
            Pending(new DateOnly(2026, 9, 30), PlannedActivityTiming.Today, 8_950m),
            Pending(new DateOnly(2026, 10, 28), PlannedActivityTiming.Upcoming, null),
            Pending(new DateOnly(2026, 10, 1), PlannedActivityTiming.Upcoming, 500m) with { TaxKind = null });

        var result = await new GetTaxOverviewUseCase(world.User, world.Recurring, planned, world.Payments)
            .ExecuteAsync(new DateOnly(2026, 9, 30), 30);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value.Pending.Count);
        Assert.Equal(9_950m, result.Value.PendingTotal);
        Assert.Equal(2, result.Value.PendingUnknownAmountCount);
    }

    /// <summary>
    /// Tanım ayrıntısı pencere değil sayı gösterir: gecikmişlerin hepsi ve
    /// sıradaki üç kalem; seyrek ritim için ufuk üç yıldır.
    /// </summary>
    [Fact]
    public async Task ThePlanDetail_ShowsOverdueAndTheNextThree()
    {
        var world = new World();
        var plan = world.AddTaxPlan(new DateOnly(2026, 8, 31));
        var planned = new StubPlanned(
            Pending(new DateOnly(2026, 8, 31), PlannedActivityTiming.Overdue, null, plan.Id),
            Pending(new DateOnly(2026, 9, 30), PlannedActivityTiming.Today, null, plan.Id),
            Pending(new DateOnly(2026, 10, 31), PlannedActivityTiming.Upcoming, null, plan.Id),
            Pending(new DateOnly(2026, 11, 30), PlannedActivityTiming.Upcoming, null, plan.Id),
            Pending(new DateOnly(2026, 12, 31), PlannedActivityTiming.Upcoming, null, plan.Id));

        var result = await new GetTaxPlanDetailUseCase(world.User, world.Recurring, planned, world.Payments)
            .ExecuteAsync(plan.Id, new DateOnly(2026, 9, 30));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            [new DateOnly(2026, 8, 31), new DateOnly(2026, 9, 30), new DateOnly(2026, 10, 31), new DateOnly(2026, 11, 30)],
            result.Value.Upcoming.Select(item => item.DueDate));
        Assert.Equal(new DateOnly(2029, 9, 30), planned.LastHorizon);
    }

    private static PlannedActivityDto Pending(
        DateOnly dueDate,
        PlannedActivityTiming timing,
        decimal? amount,
        Guid? planId = null) => new(
        Guid.NewGuid(),
        PlannedActivityKind.RecurringOccurrence,
        FinancialActivityEffect.Expense,
        timing,
        PlannedActivityReadiness.Ready,
        null,
        PlannedActivityAction.Realize,
        dueDate,
        amount,
        CurrencyCode.TRY,
        "Vergi",
        null,
        null,
        null,
        null,
        null,
        IsProjected: false,
        ActionTargetId: Guid.NewGuid(),
        ActionSequence: null,
        RecurringTransactionId: planId ?? Guid.NewGuid(),
        TaxKind: TaxKind.SocialSecurityPremium);

    private sealed class StubPlanned(params PlannedActivityDto[] items) : IPlannedActivityRepository
    {
        public DateOnly LastHorizon { get; private set; }

        public Task<IReadOnlyList<PlannedActivityDto>> ListAsync(
            Guid userId,
            DateOnly asOfDate,
            DateOnly horizonDate,
            TransactionScope? scope,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<PlannedActivityDto>>(items);

        public Task<IReadOnlyList<PlannedActivityDto>> ListForRecurringPlanAsync(
            Guid userId,
            Guid recurringTransactionId,
            DateOnly asOfDate,
            DateOnly horizonDate,
            CancellationToken cancellationToken)
        {
            LastHorizon = horizonDate;
            return Task.FromResult<IReadOnlyList<PlannedActivityDto>>(
                [.. items.Where(item => item.RecurringTransactionId == recurringTransactionId)]);
        }
    }

    private sealed class World
    {
        public Account Account { get; } = new(Guid.NewGuid(), UserId, "Banka", AccountType.Bank, CurrencyCode.TRY);
        public CreditCard Card { get; } = new(
            Guid.NewGuid(), UserId, "Kart", new Money(50_000m, CurrencyCode.TRY), 15, 25);
        public Category TaxCategory { get; } = new(
            Guid.NewGuid(), UserId, "Vergi ve harç", CategoryType.Expense, TransactionScope.Personal, isTax: true);
        public FakeTransactionRepository Transactions { get; } = new();
        public FakeCardChargeRepository Charges { get; } = new();
        public FakeRecurringRepository Recurring { get; }
        public FakeTaxPaymentRepository Payments { get; }

        private readonly FakeCurrentUser user = new(UserId);
        private readonly FixedTimeProvider time = new(Now);
        private readonly bool hasBusiness;

        public World(bool hasBusiness = false)
        {
            this.hasBusiness = hasBusiness;
            Recurring = new FakeRecurringRepository(null, Transactions, Charges);
            Payments = new FakeTaxPaymentRepository(Transactions, Charges, Recurring);
        }

        public FakeCurrentUser User => user;

        public CreateRecurringTransactionCommand PlanCommand() => new(
            null, null, null, TaxCategory.Id, null, CurrencyCode.TRY, RecurringTransactionKind.Expense, null,
            RecurrenceFrequency.Monthly, new DateOnly(2026, 10, 31), null, MonthEndBehavior.ClampToLastDay,
            "Bağkur", TaxKind: TaxKind.SocialSecurityPremium, DayOfMonth: 31);

        public CreateRecurringTransactionUseCase CreatePlan() => new(
            user, new FakeAccountRepository(Account), new FakeCreditCardRepository(Card),
            new FakeCategoryRepository(TaxCategory), new FakeUserProfileRepository(hasBusiness), Recurring);

        public CreateTaxPlansUseCase CreatePlans() => new(user, CreatePlan(), Recurring);

        public RecurringTransaction AddTaxPlan(DateOnly startDate)
        {
            var plan = new RecurringTransaction(
                Guid.NewGuid(), UserId, TaxCategory, null, TaxKind.SocialSecurityPremium, TransactionScope.Personal,
                RecurrenceFrequency.Monthly, startDate, description: "Bağkur", dayOfMonth: 31);
            Recurring.Recurring.Add(plan);
            return plan;
        }

        public CreateTaxPaymentCommand Payment(Guid requestId, IReadOnlyList<TaxItemReference> closes) =>
            new(requestId, 12_500m, new DateOnly(2026, 9, 28), Account.Id, null, TaxCategory.Id, null, null, closes);

        public CreateTaxPaymentUseCase CreatePayment() => new(
            user, new FakeAccountRepository(Account), new FakeCreditCardRepository(Card),
            new FakeCategoryRepository(TaxCategory), new FakeUserProfileRepository(hasBusiness), Recurring,
            new RecurringOccurrenceMaterializer(Recurring), Payments, time);

        public RealizeDueRecurringUseCase RealizeDue() => new(
            user, new RecurringOccurrenceMaterializer(Recurring),
            new RealizeRecurringOccurrenceUseCase(
                user, Recurring, new FakeAccountRepository(Account), new FakeCreditCardRepository(Card),
                new FakeCategoryRepository(TaxCategory), Transactions, Charges, time),
            time);

        public UndoRecurringOccurrenceUseCase Undo() => new(user, Recurring, Transactions, Charges, time);

        public UndoTaxPaymentUseCase UndoPayment() => new(
            user, Transactions, Charges, Recurring, new ManualOrigin(), Undo(), Payments, time);

        public SetOccurrenceAmountUseCase SetAmount() => new(user, new RecurringOccurrenceMaterializer(Recurring), Recurring);

        public UpdateRecurringTransactionUseCase Update() => new(
            user, new FakeAccountRepository(Account), new FakeCreditCardRepository(Card),
            new FakeCategoryRepository(TaxCategory), new FakeUserProfileRepository(false), Recurring);
    }

    private sealed class ManualOrigin : IActivityOriginReader
    {
        public Task<FinancialActivityOrigin> GetTransactionOriginAsync(
            Guid userId, Guid transactionId, CancellationToken cancellationToken) =>
            Task.FromResult(FinancialActivityOrigin.Manual);

        public Task<FinancialActivityOrigin> GetCardChargeOriginAsync(
            Guid userId, Guid creditCardChargeId, CancellationToken cancellationToken) =>
            Task.FromResult(FinancialActivityOrigin.Manual);
    }
}
