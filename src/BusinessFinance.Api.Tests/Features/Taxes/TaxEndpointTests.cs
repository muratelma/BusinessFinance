using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Taxes;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.UpcomingPayments;

namespace BusinessFinance.Api.Tests.Features.Taxes;

/// <summary>
/// Vergi bir nakit planıdır (ADR 0018, Aşama 06.3 Grup 3). Tanımlı vergi bir
/// tekrarlayan plandır ve Yaklaşanlar'a planlanan projection'dan düşer; ödenen
/// vergi vergi işaretli kategorideki bir giderdir. Tutar kullanıcınındır ve
/// bilinmeyebilir; ödeme ödeme gününe yazılır ve geri alınabilir.
/// </summary>
public sealed class TaxEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Today = "2026-08-28";

    /// <summary>
    /// Hazır türler yalnız ritim ve gün önerir, <b>tutar önermez</b>: bir sayı
    /// önermek, hesaplanmış bir vergi tutarı iddia etmek olurdu (İ1).
    /// </summary>
    [Fact]
    public async Task Suggestions_OfferRhythmAndDayButNoAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        using var response = await client.GetAsync("/api/v1/tax-calendar/suggestions", CancellationToken.None);
        var payload = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var suggestions = JsonSerializer.Deserialize<TaxCalendarSuggestionListResponse>(payload, Json)!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(8, suggestions.Items.Count);
        Assert.DoesNotContain("amount", payload, StringComparison.OrdinalIgnoreCase);

        var advanceTax = suggestions.Items.Single(item => item.TaxKind == "advance-tax");
        Assert.Equal("selected-months", advanceTax.Frequency);
        Assert.Equal([2, 5, 8, 11], advanceTax.Months);
        Assert.Equal(17, advanceTax.DayOfMonth);

        var socialSecurity = suggestions.Items.Single(item => item.TaxKind == "social-security-premium");
        Assert.Equal("monthly", socialSecurity.Frequency);
        Assert.Equal(31, socialSecurity.DayOfMonth);
        Assert.DoesNotContain("personalScopeAllowed", payload, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Suggestions_RequireAuthentication()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/tax-calendar/suggestions", CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tutarı ve kaynağı olmayan vergi meşrudur (İ5, T4): Yaklaşanlar'da görünür,
    /// "7 günde çıkacak" onu tahminle saymaz, kaç tane olduğunu ayrıca söyler.
    /// </summary>
    [Fact]
    public async Task ATaxWithoutAmountOrSource_IsPendingButCountedNeverEstimated()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);

        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "Bağkur", "2026-08-31");
        var planned = await client.GetFromJsonAsync<PlannedActivityListResponse>(
            $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=7", CancellationToken.None);
        var overview = await GetOverviewAsync(client);

        Assert.Equal("social-security-premium", plan.TaxKind);
        Assert.Null(plan.Amount);
        Assert.Null(plan.SourceType);
        Assert.Equal(31, plan.DayOfMonth);
        Assert.Equal("personal", plan.Scope);

        var item = Assert.Single(planned!.Items);
        Assert.Null(item.Amount);
        Assert.Equal("social-security-premium", item.TaxKind);
        Assert.Equal(plan.Id, item.RecurringTransactionId);
        Assert.Equal("0.0000", planned.UpcomingOutgoingTotal);
        Assert.Equal(1, planned.UnknownAmountCount);

        Assert.Equal(plan.Id, Assert.Single(overview.Plans).Id);
        Assert.Equal("2026-08-31", Assert.Single(overview.Pending).DueDate);
        Assert.Equal("0.0000", overview.PendingTotal);
        Assert.Equal(1, overview.PendingUnknownAmountCount);
    }

    /// <summary>
    /// Vergi ekranının kartı "Gecikenler ve 30 gün"dür: toplamı gecikeni de
    /// sayar. Planlanan görünüm gecikmişleri kendi toplamında taşır; "7 günde
    /// çıkacak"a eklemez.
    /// </summary>
    [Fact]
    public async Task OverdueTaxItems_CountInTheTaxTotal_AndInTheirOwnPlannedTotal()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);
        await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            TaxPlanRequest(taxCategory.Id, "Bağkur", "2026-07-31") with { Amount = "8950.0000" });

        var overview = await GetOverviewAsync(client);
        var planned = await client.GetFromJsonAsync<PlannedActivityListResponse>(
            $"/api/v1/financial-activities/planned?asOfDate={Today}&daysAhead=7", CancellationToken.None);

        Assert.Equal(["2026-07-31", "2026-08-31"], overview.Pending.Select(item => item.DueDate));
        Assert.Equal("17900.0000", overview.PendingTotal);
        Assert.Equal(0, overview.PendingUnknownAmountCount);
        Assert.Equal("8950.0000", planned!.OverdueOutgoingTotal);
        Assert.Equal(0, planned.OverdueUnknownAmountCount);
        Assert.Equal("8950.0000", planned.UpcomingOutgoingTotal);
    }

    /// <summary>
    /// "Vergilerimi tanımla": seçilen vergiler tek istekte kurulur. Biri
    /// geçersizse hiçbiri yazılmaz; başka kullanıcının kategorisi kullanılamaz.
    /// </summary>
    [Fact]
    public async Task SeveralTaxes_AreDefinedTogether_OrNotAtAll()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, hasBusiness: true);
        using var other = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);
        var foreignCategory = await GetTaxCategoryAsync(other);

        using var refused = await client.PostAsJsonAsync("/api/v1/taxes/plans",
            new CreateTaxPlansRequest(
            [
                TaxPlanRequest(taxCategory.Id, "Bağkur", "2026-08-31"),
                TaxPlanRequest(foreignCategory.Id, "KDV", "2026-08-31")
            ]),
            CancellationToken.None);
        await AssertProblemAsync(refused, HttpStatusCode.BadRequest, "recurring.category_unavailable");
        Assert.Empty((await GetOverviewAsync(client)).Plans);

        var created = await CreateAsync<TaxPlanListResponse>(client, "/api/v1/taxes/plans",
            new CreateTaxPlansRequest(
            [
                TaxPlanRequest(taxCategory.Id, "Bağkur", "2026-08-31"),
                new CreateRecurringTransactionRequest(
                    null, taxCategory.Id, null, "TRY", "expense", null, "selected-months", "2027-01-31",
                    null, "clamp-to-last-day", "Motorlu taşıtlar", TaxKind: "motor-vehicle-tax",
                    DayOfMonth: 31, Months: [1, 7])
            ]));

        Assert.Equal(2, created.Items.Count);
        Assert.All(created.Items, plan => Assert.Equal("business", plan.Scope));
        Assert.Equal(2, (await GetOverviewAsync(client)).Plans.Count);
        Assert.Empty((await GetOverviewAsync(other)).Plans);
    }

    [Fact]
    public async Task DefiningTaxesTogether_RefusesAnOrdinaryPlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);

        using var response = await client.PostAsJsonAsync("/api/v1/taxes/plans",
            new CreateTaxPlansRequest(
                [TaxPlanRequest(taxCategory.Id, "Kira", "2026-08-31") with { TaxKind = null, Amount = "100.0000" }]),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "taxes.plan_not_tax");
    }

    /// <summary>
    /// Tanım ayrıntısı pencere değil sayı gösterir: yılda iki kez ödenen bir
    /// verginin sıradaki üç kalemi de görünür.
    /// </summary>
    [Fact]
    public async Task ThePlanDetail_ShowsTheNextThreeEvenForASparseRhythm()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                null, taxCategory.Id, null, "TRY", "expense", null, "selected-months", "2027-01-31",
                null, "clamp-to-last-day", "Motorlu taşıtlar", TaxKind: "motor-vehicle-tax",
                DayOfMonth: 31, Months: [1, 7]));

        var detail = await client.GetFromJsonAsync<TaxPlanDetailResponse>(
            $"/api/v1/taxes/plans/{plan.Id}?asOfDate={Today}", CancellationToken.None);

        Assert.Equal(["2027-01-31", "2027-07-31", "2028-01-31"], detail!.Upcoming.Select(item => item.DueDate));
    }

    /// <summary>
    /// İşletmesi olan kullanıcının vergisi seçim yoksa işletmenindir; tanımda
    /// seçilen kapsam geçerlidir (ADR 0018 İ9).
    /// </summary>
    [Fact]
    public async Task TaxScope_FollowsTheProfileUnlessChosenExplicitly()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, hasBusiness: true);
        var taxCategory = await GetTaxCategoryAsync(client);

        var socialSecurity = await CreateTaxPlanAsync(client, taxCategory.Id, "Bağkur", "2026-08-31");
        var vehicle = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                null, taxCategory.Id, null, "TRY", "expense", "personal", "selected-months", "2027-01-31",
                null, "clamp-to-last-day", "MTV", TaxKind: "motor-vehicle-tax", DayOfMonth: 31, Months: [1, 7]));

        Assert.Equal("SGK ve vergi ödemesi", taxCategory.Name);
        Assert.Equal("business", socialSecurity.Scope);
        Assert.Equal("personal", vehicle.Scope);
        Assert.Equal([1, 7], vehicle.Months);
    }

    [Fact]
    public async Task ATaxPlan_OnACategoryNotMarkedAsTax_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense", CancellationToken.None);
        var ordinary = categories!.Items.First(item => !item.IsTax);

        using var response = await client.PostAsJsonAsync("/api/v1/recurring-transactions",
            TaxPlanRequest(ordinary.Id, "Bağkur", "2026-08-31"), CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "recurring.category_not_tax");
    }

    /// <summary>
    /// Kaynaksız plan yalnız vergidir; sıradan bir planın kaynağı hâlâ tam
    /// olarak biridir (ADR 0005).
    /// </summary>
    [Fact]
    public async Task AnOrdinaryPlanWithoutSource_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var category = await GetTaxCategoryAsync(client);

        using var response = await client.PostAsJsonAsync("/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                null, category.Id, "100.0000", "TRY", "expense", null, "monthly", "2026-08-31",
                null, "clamp-to-last-day", "Kira"),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "recurring.invalid_source");
    }

    /// <summary>
    /// "Ödedim" ödeme gününe, seçilen hesaptan yazar (T4); vadesi gelmemiş
    /// kalem de ödenebilir. Geri alma gideri iptal eder, kalem bekleyene döner
    /// ve yazılan tutar kalemde kalır (İ7).
    /// </summary>
    [Fact]
    public async Task PayingAnItem_WritesOnThePaymentDay_AndUndoReturnsItToPending()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "Bağkur", "2026-08-31");

        using var pay = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-31", "8950.0000", Today, account.Id),
            CancellationToken.None);
        pay.EnsureSuccessStatusCode();
        var paid = await pay.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>(CancellationToken.None);
        var afterPay = await GetOverviewAsync(client);

        Assert.Equal(Today, paid!.Transaction!.TransactionDate);
        Assert.Equal("8950.0000", paid.Transaction.Amount);
        Assert.Equal(account.Id, paid.Transaction.AccountId);
        Assert.Empty(afterPay.Pending);
        var payment = Assert.Single(afterPay.RecentPayments);
        Assert.Equal(paid.Transaction.Id, payment.PaymentId);
        Assert.Equal("2026-08-31", payment.RealizedItem!.ScheduledDate);

        using var undo = await client.PostAsync(
            $"/api/v1/tax-payments/{payment.PaymentId}/undo", null, CancellationToken.None);
        undo.EnsureSuccessStatusCode();
        var undone = await undo.Content.ReadFromJsonAsync<TaxPaymentResponse>(CancellationToken.None);
        var afterUndo = await GetOverviewAsync(client);

        Assert.True(undone!.IsCancelled);
        Assert.Null(undone.RealizedItem);
        Assert.Empty(afterUndo.RecentPayments);
        var pending = Assert.Single(afterUndo.Pending);
        Assert.Equal("8950.0000", pending.Amount);
    }

    [Fact]
    public async Task PayingAnItemWithoutAmount_RequiresTheAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "KDV", "2026-08-31");

        using var response = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-31", null, Today, account.Id),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "recurring.amount_required");
    }

    [Fact]
    public async Task PayingAnUnsourcedItem_RequiresAnAccountOrCard()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "KDV", "2026-08-31");

        using var response = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-31", "100.0000", Today),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "recurring.source_required");
    }

    /// <summary>
    /// Hiçbir vergi tanımlamadan tek tutarla ödeme (İ4). Aynı istek iki kez
    /// gelirse ikinci gider yazılmaz.
    /// </summary>
    [Fact]
    public async Task ATaxPaymentWithoutDefinitions_IsOneExpense_AndARetryWritesNothingNew()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var request = new CreateTaxPaymentRequest(
            Guid.NewGuid(), "12500.0000", Today, taxCategory.Id, AccountId: account.Id,
            Note: "Temmuz–Ağustos Bağkur");

        var first = await CreateAsync<TaxPaymentResponse>(client, "/api/v1/tax-payments", request);
        var retry = await CreateAsync<TaxPaymentResponse>(client, "/api/v1/tax-payments", request);
        var transactions = await client.GetFromJsonAsync<TransactionListResponse>(
            "/api/v1/transactions", CancellationToken.None);

        Assert.Equal(first.PaymentId, retry.PaymentId);
        var transaction = Assert.Single(transactions!.Items);
        Assert.Equal(first.PaymentId, transaction.Id);
        Assert.Equal("expense", transaction.Type);
        Assert.Equal(Today, transaction.TransactionDate);
        Assert.Equal("Temmuz–Ağustos Bağkur", transaction.Description);
        Assert.Empty(first.ClosedItems);
    }

    /// <summary>
    /// Ödeme kaynağının etiketi vergide kapsamı belirlemez (ADR 0018 İ9, 30 Eylül
    /// 2026): işletme vergisi şahsi hesaptan ödenebilir.
    /// </summary>
    [Fact]
    public async Task ATaxPaymentFromAPersonalAccount_StaysBusiness()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory, hasBusiness: true);
        var account = await CreateAsync<AccountResponse>(client, "/api/v1/accounts",
            new CreateAccountRequest("Şahsi hesap", "bank", "TRY", "50000", "personal"));
        var taxCategory = await GetTaxCategoryAsync(client);

        var payment = await CreateAsync<TaxPaymentResponse>(client, "/api/v1/tax-payments",
            new CreateTaxPaymentRequest(Guid.NewGuid(), "1000.0000", Today, taxCategory.Id, AccountId: account.Id));

        Assert.Equal("business", payment.Scope);
    }

    /// <summary>
    /// Toplu ödeme seçilen kalemleri kapatır (T5); kapatılan kalem bekleyenlerden
    /// düşer. Ödeme İşlemler'den iptal edilince kapattığı kalemler kendiliğinden
    /// bekleyene döner — tek gerçek, iki kapı.
    /// </summary>
    [Fact]
    public async Task ATaxPayment_ClosesItems_AndCancellingItAnywhereReopensThem()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "Bağkur", "2026-08-31");

        var payment = await CreateAsync<TaxPaymentResponse>(client, "/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "17900.0000", Today, taxCategory.Id, AccountId: account.Id,
                Closes: [new TaxItemReferenceRequest(plan.Id, "2026-08-31"), new TaxItemReferenceRequest(plan.Id, "2026-09-30")]));
        var afterPayment = await GetOverviewAsync(client, daysAhead: 90);

        Assert.Equal(["2026-08-31", "2026-09-30"], payment.ClosedItems.Select(item => item.ScheduledDate));
        Assert.DoesNotContain(afterPayment.Pending, item => item.DueDate is "2026-08-31" or "2026-09-30");
        Assert.Contains(afterPayment.Pending, item => item.DueDate == "2026-10-31");

        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{payment.PaymentId}", CancellationToken.None);
        cancel.EnsureSuccessStatusCode();
        var afterCancel = await GetOverviewAsync(client, daysAhead: 90);

        Assert.Contains(afterCancel.Pending, item => item.DueDate == "2026-08-31");
        Assert.Contains(afterCancel.Pending, item => item.DueDate == "2026-09-30");
        Assert.Empty(afterCancel.RecentPayments);
    }

    /// <summary>
    /// Kapatılmış kalem ikinci bir ödemeyle "Ödedim" olamaz: bir kalem tam
    /// olarak tek sonuç taşır (İ7).
    /// </summary>
    [Fact]
    public async Task AClosedItem_CannotBePaidAgain()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "Bağkur", "2026-08-31");
        await CreateAsync<TaxPaymentResponse>(client, "/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "8950.0000", Today, taxCategory.Id, AccountId: account.Id,
                Closes: [new TaxItemReferenceRequest(plan.Id, "2026-08-31")]));

        using var pay = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-31", "8950.0000", Today, account.Id),
            CancellationToken.None);
        using var closeAgain = await client.PostAsJsonAsync("/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "8950.0000", Today, taxCategory.Id, AccountId: account.Id,
                Closes: [new TaxItemReferenceRequest(plan.Id, "2026-08-31")]),
            CancellationToken.None);

        await AssertProblemAsync(pay, HttpStatusCode.Conflict, "recurring.already_settled");
        await AssertProblemAsync(closeAgain, HttpStatusCode.Conflict, "tax_payments.item_not_pending");
    }

    /// <summary>
    /// Toplu ödeme yalnız tanımlı vergileri kapatır; vergi türü boş bir planın
    /// kalemi kapatılamaz.
    /// </summary>
    [Fact]
    public async Task ATaxPayment_CannotCloseAnOrdinaryPlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var ordinary = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, taxCategory.Id, "100.0000", "TRY", "expense", null, "monthly", "2026-08-31",
                null, "clamp-to-last-day", "Eski plan"));

        using var response = await client.PostAsJsonAsync("/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "100.0000", Today, taxCategory.Id, AccountId: account.Id,
                Closes: [new TaxItemReferenceRequest(ordinary.Id, "2026-08-31")]),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "tax_payments.item_not_tax");
    }

    [Fact]
    public async Task ATaxPayment_ToACategoryNotMarkedAsTax_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense", CancellationToken.None);

        using var response = await client.PostAsJsonAsync("/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "100.0000", Today, categories!.Items.First(item => !item.IsTax).Id,
                AccountId: account.Id),
            CancellationToken.None);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "tax_payments.category_not_tax");
    }

    /// <summary>
    /// "Tutar belli oldu" bekleyen kaleme yazar, ileri bir tarih için de; plan
    /// değişmez (T3).
    /// </summary>
    [Fact]
    public async Task KnowingTheAmountEarly_WritesTheItemNotThePlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "KDV", "2026-08-31");

        using var response = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/amount",
            new SetOccurrenceAmountRequest("2026-09-30", "4321.5000"),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var overview = await GetOverviewAsync(client, daysAhead: 90);

        Assert.Equal("4321.5000", overview.Pending.Single(item => item.DueDate == "2026-09-30").Amount);
        Assert.Null(overview.Pending.Single(item => item.DueDate == "2026-08-31").Amount);
        Assert.Null(Assert.Single(overview.Plans).Amount);
    }

    /// <summary>
    /// Ritim değişince plan yeni ritimle yeniden başlar; yeni başlangıç son
    /// ödenen kalemden sonra olmalıdır (kullanıcı kararı, 30 Eylül 2026).
    /// </summary>
    [Fact]
    public async Task ChangingTheRhythm_RestartsThePlanAfterItsHistory()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var taxCategory = await GetTaxCategoryAsync(client);
        var plan = await CreateTaxPlanAsync(client, taxCategory.Id, "Emlak", "2026-08-31");
        using var pay = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-31", "700.0000", Today, account.Id),
            CancellationToken.None);
        pay.EnsureSuccessStatusCode();

        using var tooEarly = await client.PutAsJsonAsync($"/api/v1/recurring-transactions/{plan.Id}",
            new UpdateRecurringTransactionRequest(
                null, taxCategory.Id, null, null, "selected-months", "2026-05-31", null,
                "clamp-to-last-day", "Emlak", DayOfMonth: 31, Months: [5, 11]),
            CancellationToken.None);
        using var update = await client.PutAsJsonAsync($"/api/v1/recurring-transactions/{plan.Id}",
            new UpdateRecurringTransactionRequest(
                null, taxCategory.Id, null, null, "selected-months", "2026-11-30", null,
                "clamp-to-last-day", "Emlak vergisi", DayOfMonth: 31, Months: [5, 11]),
            CancellationToken.None);
        update.EnsureSuccessStatusCode();
        var updated = await update.Content.ReadFromJsonAsync<RecurringTransactionResponse>(CancellationToken.None);

        await AssertProblemAsync(tooEarly, HttpStatusCode.BadRequest, "recurring.reschedule_before_history");
        Assert.Equal("selected-months", updated!.Frequency);
        Assert.Equal([5, 11], updated.Months);
        Assert.Equal("2026-11-30", updated.NextOccurrenceDate);
        Assert.Equal("Emlak vergisi", updated.Description);
        Assert.Equal(1, updated.GeneratedOccurrenceCount);
    }

    /// <summary>
    /// Vergi işareti yalnız gider kategorisine konur; sonradan kaldırılabilir
    /// ve alanı göndermeyen güncelleme onu değiştirmez.
    /// </summary>
    [Fact]
    public async Task TheTaxMark_BelongsToExpenseCategoriesAndSurvivesARename()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);

        var marked = await CreateAsync<CategoryResponse>(client, "/api/v1/categories",
            new CreateCategoryRequest("Belediye harçları", "expense", null, IsTax: true));
        using var income = await client.PostAsJsonAsync("/api/v1/categories",
            new CreateCategoryRequest("Vergi iadesi", "income", null, IsTax: true), CancellationToken.None);
        using var rename = await client.PutAsJsonAsync($"/api/v1/categories/{marked.Id}",
            new UpdateCategoryRequest("Harçlar", true), CancellationToken.None);
        var renamed = await rename.Content.ReadFromJsonAsync<CategoryResponse>(CancellationToken.None);
        using var unmark = await client.PutAsJsonAsync($"/api/v1/categories/{marked.Id}",
            new UpdateCategoryRequest("Harçlar", true, null, IsTax: false), CancellationToken.None);
        var unmarked = await unmark.Content.ReadFromJsonAsync<CategoryResponse>(CancellationToken.None);

        Assert.True(marked.IsTax);
        Assert.Equal(HttpStatusCode.BadRequest, income.StatusCode);
        Assert.True(renamed!.IsTax);
        Assert.False(unmarked!.IsTax);
    }

    /// <summary>
    /// Sıradan plan eskisi gibi çalışır: Yaklaşanlar'a düşer ve silinebilir.
    /// </summary>
    [Fact]
    public async Task AnOrdinaryPlan_LandsInTheUpcomingListAndCanBeDeleted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetTaxCategoryAsync(client);

        var plan = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "1000.0000", "TRY", "bill-payment", null, "quarterly", "2026-08-17",
                null, "clamp-to-last-day", "Geçici vergi"));
        using var generate = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-17"),
            CancellationToken.None);
        generate.EnsureSuccessStatusCode();
        var upcoming = await client.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-17&daysAhead=30", CancellationToken.None);

        Assert.Equal("quarterly", plan.Frequency);
        Assert.Null(plan.TaxKind);
        var payment = Assert.Single(upcoming!.Items);
        Assert.Equal("recurring-occurrence", payment.SourceType);
        Assert.Equal("1000.0000", payment.Amount);

        using var delete = await client.DeleteAsync($"/api/v1/recurring-transactions/{plan.Id}", CancellationToken.None);
        var afterDelete = await client.GetFromJsonAsync<UpcomingPaymentListResponse>(
            "/api/v1/upcoming-payments?asOfDate=2026-08-17&daysAhead=30", CancellationToken.None);

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Empty(afterDelete!.Items);
    }

    /// <summary>
    /// Plandaki tutar bir beklentidir: gerçekleşme sırasında gönderilen tutar
    /// kayda geçer ve <b>planın tutarı değişmez</b>.
    /// </summary>
    [Fact]
    public async Task Realizing_WritesTheAmountTheUserActuallyOwes_WithoutChangingThePlan()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetTaxCategoryAsync(client);
        var plan = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "1000.0000", "TRY", "bill-payment", null, "monthly", "2026-08-01",
                null, "clamp-to-last-day", "KDV beyanı"));

        using var realize = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-01", "2450.7500"),
            CancellationToken.None);
        realize.EnsureSuccessStatusCode();
        var realized = await realize.Content.ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>(
            CancellationToken.None);
        var plans = await client.GetFromJsonAsync<RecurringTransactionListResponse>(
            "/api/v1/recurring-transactions", CancellationToken.None);

        Assert.Equal("2450.7500", realized!.Transaction!.Amount);
        Assert.Equal("2026-08-01", realized.Transaction.TransactionDate);
        Assert.Equal("1000.0000", Assert.Single(plans!.Items).Amount);
    }

    /// <summary>
    /// Gönderilen tutar okunamıyorsa sunucu bir değer uydurmaz.
    /// </summary>
    [Fact]
    public async Task Realizing_WithAnUnreadableAmount_IsRefused()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateAuthenticatedClientAsync(factory);
        var account = await CreateAccountAsync(client);
        var category = await GetTaxCategoryAsync(client);
        var plan = await CreateAsync<RecurringTransactionResponse>(client, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "1000.0000", "TRY", "bill-payment", null, "monthly", "2026-08-01",
                null, "clamp-to-last-day", "KDV beyanı"));

        using var realize = await client.PostAsJsonAsync(
            $"/api/v1/recurring-transactions/{plan.Id}/occurrences/realize",
            new RealizeDueRecurringRequest("2026-08-01", "iki bin"),
            CancellationToken.None);

        await AssertProblemAsync(realize, HttpStatusCode.BadRequest, "recurring.invalid_amount");
    }

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static CreateRecurringTransactionRequest TaxPlanRequest(Guid categoryId, string name, string startDate) =>
        new(null, categoryId, null, "TRY", "expense", null, "monthly", startDate, null,
            "clamp-to-last-day", name, TaxKind: "social-security-premium", DayOfMonth: 31);

    private static Task<RecurringTransactionResponse> CreateTaxPlanAsync(
        HttpClient client, Guid categoryId, string name, string startDate) =>
        CreateAsync<RecurringTransactionResponse>(
            client, "/api/v1/recurring-transactions", TaxPlanRequest(categoryId, name, startDate));

    private static async Task<TaxOverviewResponse> GetOverviewAsync(HttpClient client, int daysAhead = 30) =>
        (await client.GetFromJsonAsync<TaxOverviewResponse>(
            $"/api/v1/taxes?asOfDate={Today}&daysAhead={daysAhead}", CancellationToken.None))!;

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
        Assert.True(response.StatusCode == status, $"{(int)response.StatusCode}: {body}");
        using var document = JsonDocument.Parse(body);
        Assert.Equal(code, document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<TResponse> CreateAsync<TResponse>(HttpClient client, string path, object request)
    {
        using var response = await client.PostAsync(
            path, JsonContent.Create(request, request.GetType()), CancellationToken.None);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"{path}: {(int)response.StatusCode} " +
                await response.Content.ReadAsStringAsync(CancellationToken.None));
        }

        return (await response.Content.ReadFromJsonAsync<TResponse>(CancellationToken.None))!;
    }

    private static Task<AccountResponse> CreateAccountAsync(HttpClient client) =>
        CreateAsync<AccountResponse>(client, "/api/v1/accounts",
            new CreateAccountRequest("Banka", "bank", "TRY", "50000"));

    private static async Task<CategoryResponse> GetTaxCategoryAsync(HttpClient client)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense", CancellationToken.None);
        return categories!.Items.Single(item => item.IsTax);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        bool hasBusiness = false)
    {
        var client = factory.CreateClient();
        var email = $"taxes-{Guid.NewGuid():N}@example.test";
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, hasBusiness),
            CancellationToken.None);
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password),
            CancellationToken.None);
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>(CancellationToken.None);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        return client;
    }
}
