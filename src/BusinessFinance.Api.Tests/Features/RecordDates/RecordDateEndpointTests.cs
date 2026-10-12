using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Imports;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.Transfers;

namespace BusinessFinance.Api.Tests.Features.RecordDates;

/// <summary>
/// Kayıt olmuş olanı yazar; ileride yapılacak ödeme plandır (kullanıcı kararı,
/// 12 Ekim 2026).
/// </summary>
/// <remarks>
/// O güne kadar gelir/gider, kart harcaması ve ödemesi, transfer, cari kayıt,
/// borç başlangıcı ve taksit ödemesi ileri tarihle yazılabiliyordu; ileri
/// tarihli kayıt bakiyeyi, kart borcunu ve cari bakiyeyi yazıldığı an
/// değiştiriyordu. Testler kayıt üreten her yolu ayrı ayrı dener: yarının
/// tarihi reddedilir ve geride hiçbir iz kalmaz, bugünün tarihi kabul edilir.
/// Saat sabittir: 12 Ekim 2026, Türkiye'de öğlen.
/// </remarks>
public sealed class RecordDateEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Yesterday = "2026-10-11";
    private const string Today = "2026-10-12";
    private const string Tomorrow = "2026-10-13";
    private static readonly DateTimeOffset Noon = new(2026, 10, 12, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task ARecordDatedTomorrow_IsRejectedOnEveryManualPath_AndLeavesNoTrace()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-manual@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var cash = await CreateAccountAsync(owner, "Kasa", "cash", "1000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");
        var income = await FirstCategoryAsync(owner, "income");
        var card = await PostAsync<CreditCardResponse>(
            owner, "/api/v1/credit-cards", new CreateCreditCardRequest("Kart", "5000.0000", "TRY", 10, 20));
        var person = await PostAsync<CounterpartyResponse>(
            owner, "/api/v1/counterparties", new CreateCounterpartyRequest("Ahmet Bakkal"));

        await AssertRejectedAsync(
            owner, "/api/v1/transactions",
            new CreateTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "expense", "business", Tomorrow, "Yarın"),
            "transactions.date_in_future");
        await AssertRejectedAsync(
            owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "20.0000", "TRY", Tomorrow, null),
            "transfers.date_in_future");
        await AssertRejectedAsync(
            owner, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(expense.Id, "50.0000", "TRY", "business", Tomorrow, null),
            "credit_cards.date_in_future");
        await AssertRejectedAsync(
            owner, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(bank.Id, "30.0000", "TRY", Tomorrow, null),
            "credit_cards.date_in_future");
        await AssertRejectedAsync(
            owner, $"/api/v1/counterparties/{person.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "700.0000", "TRY", income.Id, Tomorrow, "business"),
            "counterparties.date_in_future");
        await AssertRejectedAsync(
            owner, $"/api/v1/counterparties/{person.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "200.0000", "TRY", bank.Id, Tomorrow),
            "counterparties.date_in_future");
        await AssertRejectedAsync(
            owner, "/api/v1/debts",
            new CreateDebtRequest(
                "Sentetik Banka", "payable", "business", "1000.0000", "1200.0000", null, "TRY",
                "cash", bank.Id, null, Tomorrow, "2026-11-12", 2, "Kredi", Today),
            "debt.date_in_future");

        // Kasa, POS ve yükümlülük de kendi koduyla reddeder; ekran ne olduğunu
        // söyleyebilsin diye genel doğrulama koduna düşmezler.
        await AssertRejectedAsync(
            owner, "/api/v1/obligations",
            new
            {
                direction = "payable",
                amount = "300.0000",
                currency = "TRY",
                categoryId = expense.Id,
                issueDate = Tomorrow,
                dueDate = "2026-11-12",
                scope = "business",
            },
            "obligations.date_in_future");
        await AssertRejectedAsync(
            owner, "/api/v1/pos-settlements",
            new
            {
                accountId = bank.Id,
                categoryId = income.Id,
                grossAmount = "400.0000",
                currency = "TRY",
                settlementDate = Tomorrow,
            },
            "pos_settlements.date_in_future");
        await AssertRejectedAsync(
            owner, "/api/v1/cash-counts",
            new { accountId = cash.Id, countedAmount = "1000.0000", countDate = Tomorrow },
            "cash_counts.date_in_future");

        // Reddedilen isteklerin hiçbiri iz bırakmaz.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Empty(feed!.Items);
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank.Id));
        Assert.Equal("1000.0000", await BalanceAsync(owner, cash.Id));
        var cards = await owner.GetFromJsonAsync<CreditCardListResponse>("/api/v1/credit-cards");
        Assert.Equal("0.0000", Assert.Single(cards!.Items).CurrentDebt);

        // Aynı kayıtlar bugünün tarihiyle yazılır.
        await PostAsync<TransactionResponse>(
            owner, "/api/v1/transactions",
            new CreateTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "expense", "business", Today, "Bugün"));
        await PostAsync<TransferResponse>(
            owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "20.0000", "TRY", Today, null));
        await PostAsync<CardChargeResponse>(
            owner, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(expense.Id, "50.0000", "TRY", "business", Today, null));
        await PostAsync<CardPaymentResponse>(
            owner, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(bank.Id, "30.0000", "TRY", Today, null));
        await PostAsync<CounterpartyChargeResponse>(
            owner, $"/api/v1/counterparties/{person.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "700.0000", "TRY", income.Id, Today, "business"));
        await PostAsync<CounterpartyPaymentResponse>(
            owner, $"/api/v1/counterparties/{person.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "200.0000", "TRY", bank.Id, Today));

        // Vadesi ileride olan yükümlülük bugün yazılır; ödeme günü ileride olamaz.
        var obligation = await PostAsync<JsonObject>(
            owner, "/api/v1/obligations",
            new
            {
                direction = "payable",
                amount = "300.0000",
                currency = "TRY",
                categoryId = expense.Id,
                issueDate = Today,
                dueDate = "2026-11-12",
                scope = "business",
            });
        await AssertRejectedAsync(
            owner, $"/api/v1/obligations/{obligation["id"]!.GetValue<string>()}/settlement",
            new { accountId = bank.Id, settlementDate = Tomorrow },
            "obligations.date_in_future");
    }

    /// <summary>
    /// Vade ile ödeme günü ayrı şeylerdir: vadesi gelmemiş taksit bugün
    /// ödenebilir, ödeme günü ileride olamaz.
    /// </summary>
    [Fact]
    public async Task ADebtInstallmentDueLater_IsPaidToday_ButNotWithAFuturePaymentDate()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-debt@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");

        var debt = await PostAsync<DebtResponse>(
            owner, "/api/v1/debts",
            new CreateDebtRequest(
                "Sentetik Banka", "payable", "business", "1000.0000", "1200.0000", null, "TRY",
                "cash", bank.Id, null, Today, "2026-11-12", 2, "Kredi", Today));

        await AssertRejectedAsync(
            owner, $"/api/v1/debts/{debt.Id}/installments/1/pay",
            new PayDebtInstallmentRequest(bank.Id, Tomorrow, Today),
            "debt.date_in_future");
        Assert.Equal("11000.0000", await BalanceAsync(owner, bank.Id));

        var paid = await PostAsync<DebtResponse>(
            owner, $"/api/v1/debts/{debt.Id}/installments/1/pay",
            new PayDebtInstallmentRequest(bank.Id, Today, Today));
        var first = paid.Installments.Single(item => item.Sequence == 1);
        Assert.Equal("2026-11-12", first.DueDate);
        Assert.Equal(Today, first.PaymentDate);
        Assert.Equal("10400.0000", await BalanceAsync(owner, bank.Id));
    }

    /// <summary>
    /// Tekrarlayan planın iki gerçekleştirme ucu aynı kapıdan geçer: gün
    /// verilmezse vadesi gelmemiş kalem reddedilir, gün verilirse kalem o güne
    /// yazılır ve ikinci istek ikinci kayıt üretmez.
    /// </summary>
    [Fact]
    public async Task ARecurringItemDueLater_IsRealizedOnThePaymentDay_ThroughBothEndpoints()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-recurring@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");
        var plan = await PostAsync<RecurringTransactionResponse>(
            owner, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "bill-payment", "business", "monthly",
                "2026-10-20", null, "clamp-to-last-day", "Kira"));

        // Plan ve tarihle gelen uç: gün verilmezse vade günü ileridedir.
        var byPlan = $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize";
        await AssertRejectedAsync(
            owner, byPlan, new RealizeDueRecurringRequest("2026-10-20"), "recurring.not_due_yet");

        // Kalem kimliğiyle gelen uç: bu denetim 12 Ekim 2026'ya kadar yoktu.
        using var generate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-10-20"));
        generate.EnsureSuccessStatusCode();
        var occurrence = Assert.Single(
            (await generate.Content.ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>())!
            .GeneratedOccurrences);
        var byOccurrence = $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize";
        using var bare = await owner.PostAsync(byOccurrence, null);
        Assert.Equal(HttpStatusCode.BadRequest, bare.StatusCode);
        Assert.Equal("recurring.not_due_yet", await CodeAsync(bare));
        await AssertRejectedAsync(
            owner, byOccurrence, new RealizeRecurringOccurrenceRequest(PaidOn: Tomorrow),
            "recurring.paid_on_in_future");
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank.Id));

        // Kira 11 Ekim'de ödendi, uygulamaya 12 Ekim'de girildi.
        var early = await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byOccurrence, new RealizeRecurringOccurrenceRequest(PaidOn: Yesterday));
        Assert.Equal(Yesterday, early.Transaction!.TransactionDate);
        var repeated = await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byOccurrence, new RealizeRecurringOccurrenceRequest(PaidOn: Today));
        Assert.Equal(early.Transaction.Id, repeated.Transaction!.Id);
        Assert.Equal(Yesterday, repeated.Transaction.TransactionDate);

        // Sonraki ayın kalemi henüz üretilmedi; plan ve tarihle bugün ödenir.
        var next = await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byPlan, new RealizeDueRecurringRequest("2026-11-20", PaidOn: Today));
        Assert.Equal(Today, next.Transaction!.TransactionDate);

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Equal(2, feed!.Items.Count);
        Assert.Equal("9800.0000", await BalanceAsync(owner, bank.Id));
    }

    /// <summary>
    /// Sıra atlama yasak değil, onaydır: vadesi gelmemiş kalem, planın daha
    /// önceki bir kalemi beklerken yazılmak istenirse sunucu açık onay ister.
    /// Onayla yalnız seçilen kalem kapanır; önceki bekliyor olarak kalır.
    /// </summary>
    [Fact]
    public async Task SkippingAnEarlierPendingItem_NeedsAnExplicitConfirmation()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-order@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");
        var plan = await PostAsync<RecurringTransactionResponse>(
            owner, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "bill-payment", "business", "monthly",
                "2026-10-20", null, "clamp-to-last-day", "Kira"));
        var byPlan = $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize";

        // Aralık kirası, Ekim ve Kasım beklerken: onaysız yazılmaz.
        using var skipped = await owner.PostAsJsonAsync(
            byPlan, new RealizeDueRecurringRequest("2026-12-20", PaidOn: Today));
        Assert.Equal(HttpStatusCode.Conflict, skipped.StatusCode);
        // Cevap hangi kalemin beklediğini taşır: onay kutusu onu yazar.
        var asked = await skipped.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal("recurring.earlier_item_pending", asked!["code"]!.GetValue<string>());
        Assert.Equal("2026-10-20", asked["pendingScheduledDate"]!.GetValue<string>());
        Assert.Equal("2", asked["pendingCount"]!.GetValue<string>());
        Assert.Equal("10000.0000", await BalanceAsync(owner, bank.Id));

        // Ödeme günü onaydan önce denetlenir: kullanıcı sırayı onaylayıp
        // ardından gün yüzünden reddedilmez.
        await AssertRejectedAsync(
            owner, byPlan, new RealizeDueRecurringRequest("2026-12-20", PaidOn: Tomorrow),
            "recurring.paid_on_in_future");

        // Onayla yazılır ve bugüne yazılır; yalnız Aralık kapanır.
        var december = await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byPlan,
            new RealizeDueRecurringRequest("2026-12-20", PaidOn: Today, AllowEarlierPending: true));
        Assert.Equal(Today, december.Transaction!.TransactionDate);
        var occurrences = await owner.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences");
        Assert.Equal(
            [("2026-10-20", "planned"), ("2026-11-20", "planned"), ("2026-12-20", "realized")],
            occurrences!.Items
                .OrderBy(item => item.ScheduledDate, StringComparer.Ordinal)
                .Select(item => (item.ScheduledDate, item.Status)));

        // Kasım'dan önce Ekim bekliyor: yine onay. Sıradaki kalem (Ekim)
        // onaysız yazılır; ondan sonra Kasım da onaysız yazılır.
        using var november = await owner.PostAsJsonAsync(
            byPlan, new RealizeDueRecurringRequest("2026-11-20", PaidOn: Today));
        Assert.Equal("recurring.earlier_item_pending", await CodeAsync(november));
        await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byPlan, new RealizeDueRecurringRequest("2026-10-20", PaidOn: Today));
        await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, byPlan, new RealizeDueRecurringRequest("2026-11-20", PaidOn: Today));
        Assert.Equal("9700.0000", await BalanceAsync(owner, bank.Id));
    }

    /// <summary>
    /// Sınırına ya da bitiş tarihine ulaşan plan duraklatılmış değildir: son
    /// kalemi listelerde kalır ve gerçekleşir. Tek seferlik ileri ödeme tekrar
    /// sınırı 1 olan planla yazıldığı için bu yol erken ödemenin de yoludur.
    /// Kullanıcının duraklattığı plan para üretmez.
    /// </summary>
    /// <remarks>
    /// 13 Ekim 2026'ya kadar son kalem üretildiği an plan pasif sayılıyor,
    /// kalem Planlananlar'dan düşüyor ve <c>recurring.schedule_inactive</c>
    /// ile reddediliyordu (gerçek API'de görüldü).
    /// </remarks>
    [Fact]
    public async Task TheLastItemOfAPlanThatRanOut_StaysListed_AndIsRealized()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-last-item@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");
        async Task<RecurringTransactionResponse> PlanAsync(
            string name, string start, string? end = null, int? limit = null) =>
            await PostAsync<RecurringTransactionResponse>(
                owner, "/api/v1/recurring-transactions",
                new CreateRecurringTransactionRequest(
                    bank.Id, expense.Id, "100.0000", "TRY", "bill-payment", "business", "monthly",
                    start, end, "clamp-to-last-day", name, OccurrenceLimit: limit));
        static string Realize(RecurringTransactionResponse plan) =>
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize";

        // Tek seferlik ileri ödeme: vadesi 20 Ekim, bugün ödenir.
        var oneOff = await PlanAsync("Tek seferlik", "2026-10-20", limit: 1);
        var early = await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, Realize(oneOff), new RealizeDueRecurringRequest("2026-10-20", PaidOn: Today));
        Assert.Equal(Today, early.Transaction!.TransactionDate);

        // Bitiş tarihli planın son kalemi üretildi ama ödenmedi: plan artık
        // aktif değil, kalem ise hâlâ bekliyor.
        var ending = await PlanAsync("Bitişli", "2026-09-05", end: "2026-10-05");
        using var generate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest(Today));
        generate.EnsureSuccessStatusCode();
        var plans = await owner.GetFromJsonAsync<RecurringTransactionListResponse>(
            "/api/v1/recurring-transactions");
        var ended = plans!.Items.Single(item => item.Id == ending.Id);
        Assert.False(ended.IsActive);
        Assert.Null(ended.NextOccurrenceDate);

        var occurrences = await owner.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences");
        Assert.Equal(
            [("2026-09-05", "planned"), ("2026-10-05", "planned")],
            occurrences!.Items
                .Where(item => item.RecurringTransactionId == ending.Id)
                .OrderBy(item => item.ScheduledDate, StringComparer.Ordinal)
                .Select(item => (item.ScheduledDate, item.Status)));
        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=30");
        Assert.Equal(
            ["2026-09-05", "2026-10-05"],
            planned!.Items
                .Where(item => item.RecurringTransactionId == ending.Id)
                .Select(item => item.DueDate)
                .Order(StringComparer.Ordinal));

        await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, Realize(ending), new RealizeDueRecurringRequest("2026-09-05", PaidOn: Today));
        await PostAsync<RealizeRecurringOccurrenceResponse>(
            owner, Realize(ending), new RealizeDueRecurringRequest("2026-10-05", PaidOn: Today));
        Assert.Equal("9700.0000", await BalanceAsync(owner, bank.Id));

        // Biten planın takviminde olmayan gün "pasif plan" değil, bulunamadı.
        using var beyond = await owner.PostAsJsonAsync(
            Realize(ending), new RealizeDueRecurringRequest("2026-11-05", PaidOn: Today));
        Assert.Equal(HttpStatusCode.NotFound, beyond.StatusCode);

        // Duraklatılan plan: bekleyen kalemi listeden düşer ve gerçekleşmez.
        var stopped = await PlanAsync("Duraklatılan", "2026-10-05");
        using var pause = await owner.PatchAsJsonAsync(
            $"/api/v1/recurring-transactions/{stopped.Id}/active",
            new SetRecurringActiveRequest(false));
        pause.EnsureSuccessStatusCode();
        using var refused = await owner.PostAsJsonAsync(
            Realize(stopped), new RealizeDueRecurringRequest("2026-10-05", PaidOn: Today));
        Assert.Equal(HttpStatusCode.Conflict, refused.StatusCode);
        Assert.Equal("recurring.schedule_inactive", await CodeAsync(refused));
        Assert.Equal("9700.0000", await BalanceAsync(owner, bank.Id));
    }

    /// <summary>
    /// Kart taksidi kendi gününe yazılır; günü gelmemiş taksit reddedilir ve
    /// bugüne taşınmaz.
    /// </summary>
    [Fact]
    public async Task AnInstallmentDueLater_IsNotRealized()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-installment@example.test");
        var expense = await FirstCategoryAsync(owner, "expense");
        var card = await PostAsync<CreditCardResponse>(
            owner, "/api/v1/credit-cards", new CreateCreditCardRequest("Kart", "5000.0000", "TRY", 10, 20));
        var plan = await PostAsync<InstallmentPlanResponse>(
            owner, "/api/v1/installment-plans",
            new CreateInstallmentPlanRequest(
                card.Id, expense.Id, Guid.NewGuid(), "900.0000", "TRY", "business", 3,
                "2026-10-01", "Telefon"));

        using var due = await owner.PostAsync(
            $"/api/v1/installment-plans/{plan.Id}/items/1/realize", null);
        due.EnsureSuccessStatusCode();
        Assert.Equal(
            "2026-10-01", (await due.Content.ReadFromJsonAsync<CardChargeResponse>())!.ChargeDate);

        using var later = await owner.PostAsync(
            $"/api/v1/installment-plans/{plan.Id}/items/2/realize", null);
        Assert.Equal(HttpStatusCode.BadRequest, later.StatusCode);
        Assert.Equal("installments.not_due_yet", await CodeAsync(later));

        var cards = await owner.GetFromJsonAsync<CreditCardListResponse>("/api/v1/credit-cards");
        Assert.Equal("300.0000", Assert.Single(cards!.Items).CurrentDebt);
    }

    /// <summary>
    /// Banka ekstresi de kayıt üretir: ileri tarihli satır hatalı satır olarak
    /// gelir, ileri tarihe düzeltilemez.
    /// </summary>
    [Fact]
    public async Task AStatementRowDatedTomorrow_IsInvalid_AndCannotBeCorrectedToTheFuture()
    {
        await using var factory = FixedClockApi.Create(Noon);
        using var owner = await CreateClientAsync(factory, "record-date-import@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");
        using var form = CreateCsvForm(
            $"Tarih;Tutar;Açıklama\n{Tomorrow};-25,5000;Yarın\n{Today};-10,0000;Bugün");
        using var stage = await owner.PostAsync("/api/v1/imports/csv/stage", form);
        stage.EnsureSuccessStatusCode();
        var staged = (await stage.Content.ReadFromJsonAsync<ImportBatchResponse>())!;

        Assert.Equal("invalid", staged.Rows[0].Status);
        Assert.Contains("future", staged.Rows[0].ErrorMessage);
        Assert.Equal("valid", staged.Rows[1].Status);

        using var toFuture = await owner.PatchAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/rows/{staged.Rows[1].Id}",
            new UpdateImportCandidateRequest(
                Tomorrow, "-10.0000", "Bugün", null, bank.Id, expense.Id));
        Assert.Equal(HttpStatusCode.BadRequest, toFuture.StatusCode);
        Assert.Equal("imports.date_in_future", await CodeAsync(toFuture));

        using var mapped = await owner.PatchAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/rows/{staged.Rows[1].Id}",
            new UpdateImportCandidateRequest(
                Today, "-10.0000", "Bugün", null, bank.Id, expense.Id));
        mapped.EnsureSuccessStatusCode();
        using var confirm = await owner.PostAsJsonAsync(
            $"/api/v1/imports/{staged.Id}/confirm",
            new ConfirmImportBatchRequest([staged.Rows[1].Id]));
        confirm.EnsureSuccessStatusCode();
        Assert.Equal("9990.0000", await BalanceAsync(owner, bank.Id));
    }

    /// <summary>
    /// Gün Türkiye günüdür: gece yarısından sonra UTC günü hâlâ dündür, ama
    /// bugünün tarihi kabul edilir ve yarınınki reddedilir.
    /// </summary>
    [Fact]
    public async Task AfterMidnightInTurkey_TodayIsAccepted_AndTomorrowIsNot()
    {
        // Türkiye'de 13 Ekim 01:30; UTC'de 12 Ekim 22:30.
        await using var factory = FixedClockApi.Create(
            new DateTimeOffset(2026, 10, 12, 22, 30, 0, TimeSpan.Zero));
        using var owner = await CreateClientAsync(factory, "record-date-night@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(owner, "expense");

        var written = await PostAsync<TransactionResponse>(
            owner, "/api/v1/transactions",
            new CreateTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "expense", "business", "2026-10-13", null));
        Assert.Equal("2026-10-13", written.TransactionDate);
        await AssertRejectedAsync(
            owner, "/api/v1/transactions",
            new CreateTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "expense", "business", "2026-10-14", null),
            "transactions.date_in_future");
    }

    /// <summary>
    /// Kural yeni kaydı durdurur; daha önce ileri tarihe yazılmış kayıt
    /// silinmez, taşınmaz ve yedekle birlikte geri yüklenir.
    /// </summary>
    [Fact]
    public async Task ARecordAlreadyDatedInTheFuture_StaysReachable_AndSurvivesBackupAndRestore()
    {
        // Kayıt 20 Ekim'de yazılır; saat geri alınınca "eski ileri tarihli
        // kayıt" olur.
        var clock = new TestClock(new DateTimeOffset(2026, 10, 20, 9, 0, 0, TimeSpan.Zero));
        await using var factory = FixedClockApi.Create(clock);
        using var source = await CreateClientAsync(factory, "record-date-backup-source@example.test");
        var bank = await CreateAccountAsync(source, "Banka", "bank", "10000.0000");
        var expense = await FirstCategoryAsync(source, "expense");
        await PostAsync<TransactionResponse>(
            source, "/api/v1/transactions",
            new CreateTransactionRequest(
                bank.Id, expense.Id, "100.0000", "TRY", "expense", "business", "2026-10-20", "Eski"));
        clock.UtcNow = Noon;

        var feed = await source.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Equal("2026-10-20", Assert.Single(feed!.Items).ActivityDate);
        // Bilerek kabul edilen geçiş etkisi: kayıt günü gelene kadar da bakiyededir.
        Assert.Equal("9900.0000", await BalanceAsync(source, bank.Id));

        using var download = await source.GetAsync("/api/v1/backups/download");
        download.EnsureSuccessStatusCode();
        var backup = await download.Content.ReadAsByteArrayAsync();
        using var target = await CreateClientAsync(factory, "record-date-backup-target@example.test");
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(backup);
        file.Headers.ContentType = MediaTypeHeaderValue.Parse(
            "application/vnd.business-finance.backup+json");
        form.Add(file, "file", "synthetic.bfbackup.json");
        using var restore = await target.PostAsync("/api/v1/backups/restore", form);
        Assert.True(restore.IsSuccessStatusCode, await restore.Content.ReadAsStringAsync());

        var restored = await target.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Equal("2026-10-20", Assert.Single(restored!.Items).ActivityDate);
    }

    private static async Task AssertRejectedAsync<TRequest>(
        HttpClient client,
        string path,
        TRequest request,
        string expectedCode)
    {
        using var response = await client.PostAsJsonAsync(path, request);
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest,
            $"{path}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        Assert.Equal(expectedCode, await CodeAsync(response));
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string path, object request)
    {
        using var response = await client.PostAsJsonAsync(path, request);
        Assert.True(
            response.IsSuccessStatusCode,
            $"{path}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task<string?> CodeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonObject>();
        return body?["code"]?.GetValue<string>();
    }

    private static async Task<string> BalanceAsync(HttpClient client, Guid accountId)
    {
        var accounts = await client.GetFromJsonAsync<AccountListResponse>("/api/v1/accounts");
        return accounts!.Items.Single(item => item.Id == accountId).Balance;
    }

    private static MultipartFormDataContent CreateCsvForm(string csv)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(csv));
        file.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        form.Add(file, "file", "statement.csv");
        form.Add(new StringContent("Tarih"), "dateColumn");
        form.Add(new StringContent("Tutar"), "amountColumn");
        form.Add(new StringContent("Açıklama"), "descriptionColumn");
        form.Add(new StringContent("semicolon"), "delimiter");
        form.Add(new StringContent(","), "decimalSeparator");
        return form;
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type,
        string openingBalance) =>
        await PostAsync<AccountResponse>(
            client, "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", openingBalance, "business"));

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items.First(item => item.DefaultScope == "business");
    }

    private static async Task<HttpClient> CreateClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        return client;
    }
}
