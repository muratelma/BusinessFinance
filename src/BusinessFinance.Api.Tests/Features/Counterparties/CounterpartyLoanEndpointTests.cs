using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessFinance.Api.Tests.Features.Counterparties;

/// <summary>
/// Kişinin hesabı borç parayı da tutar ve kapsamı kişi taşır (ADR 0022 İ3, İ5).
/// </summary>
public sealed class CounterpartyLoanEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Senaryo 3: verilen borç para alacağı artırır, hesaptan çıkar ve gelir ya
    /// da gider yazmaz. Geri ödeme sıradan bir tahsilattır.
    /// </summary>
    [Fact]
    public async Task MoneyLent_RaisesTheReceivableAndMovesCash_WithoutIncomeOrExpense()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-lent@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "10000");
        var mother = await CreateCounterpartyAsync(owner, "Annem", "personal");

        using var lent = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{mother.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "5000.0000", "TRY", account.Id, "2026-08-05"));
        Assert.Equal(HttpStatusCode.Created, lent.StatusCode);
        var loan = (await lent.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!;
        Assert.Equal("receivable", loan.Direction);
        Assert.Equal(account.Id, loan.AccountId);

        var balance = await GetAsync(owner, mother.Id);
        Assert.Equal("5000.0000", balance.OwedToYou);
        Assert.Equal("0.0000", balance.OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("0.0000", report.TotalIncome);
        Assert.Equal("0.0000", report.TotalExpense);
        Assert.Equal("5000.0000", Assert.Single(report.AccountBalances).Balance);

        using var repaid = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{mother.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "2000.0000", "TRY", account.Id, "2026-08-10"));
        Assert.Equal(HttpStatusCode.Created, repaid.StatusCode);

        balance = await GetAsync(owner, mother.Id);
        Assert.Equal("3000.0000", balance.OwedToYou);
        report = await ReportAsync(owner);
        Assert.Equal("0.0000", report.TotalIncome);
        Assert.Equal("7000.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>Alınan borç para borcu artırır ve hesaba girer.</summary>
    [Fact]
    public async Task MoneyBorrowed_RaisesThePayableAndEntersTheAccount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-borrowed@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var friend = await CreateCounterpartyAsync(owner, "Ali", "personal");

        using var borrowed = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("payable", "3000.0000", "TRY", account.Id, "2026-08-05"));
        Assert.Equal(HttpStatusCode.Created, borrowed.StatusCode);

        var balance = await GetAsync(owner, friend.Id);
        Assert.Equal("3000.0000", balance.OwedByYou);
        Assert.Equal("0.0000", balance.OwedToYou);
        var report = await ReportAsync(owner);
        Assert.Equal("0.0000", report.TotalExpense);
        Assert.Equal("4000.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>
    /// Vadeli borç para vadesi geçince gecikir; iptal edilince bakiye ve hesap
    /// eski hâline döner, başka kayıt değişmez. İptal idempotenttir.
    /// </summary>
    [Fact]
    public async Task LoanWithADueDate_FallsOverdue_AndCancellationRestoresEverything()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-due@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var friend = await CreateCounterpartyAsync(owner, "Veli", "personal");

        using var lent = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest(
                "receivable", "400.0000", "TRY", account.Id, "2026-08-05", "2026-08-20"));
        var loan = (await lent.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!;
        Assert.Equal("2026-08-20", loan.DueDate);

        var overdue = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{friend.Id}?asOfDate=2026-08-25");
        Assert.Equal("400.0000", overdue!.OverdueReceivable);
        var notYet = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{friend.Id}?asOfDate=2026-08-15");
        Assert.Equal("0.0000", notYet!.OverdueReceivable);

        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-loans/{loan.Id}");
        using var cancelAgain = await owner.DeleteAsync($"/api/v1/counterparty-loans/{loan.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cancelAgain.StatusCode);
        Assert.True((await cancel.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!.IsCancelled);

        var balance = await GetAsync(owner, friend.Id);
        Assert.Equal("0.0000", balance.OwedToYou);
        Assert.Equal("1000.0000", Assert.Single((await ReportAsync(owner)).AccountBalances).Balance);
    }

    /// <summary>Senaryo 10: aynı istek kimliği tek kayıt yazar; borçlandırmada da.</summary>
    [Fact]
    public async Task LoanAndCharge_RetriedWithTheSameRequestId_AreWrittenOnce()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-retry@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var customer = await CreateCounterpartyAsync(owner, "Ayşe Terzi", "business");
        var other = await CreateCounterpartyAsync(owner, "Deniz Market", "business");
        var income = await FirstCategoryAsync(owner, "income", "business");

        var loanRequest = new CreateCounterpartyLoanRequest(
            "receivable", "300.0000", "TRY", account.Id, "2026-08-05", ClientRequestId: Guid.NewGuid());
        using var firstLoan = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/loans", loanRequest);
        using var retryLoan = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/loans", loanRequest);
        using var reusedLoan = await owner.PostAsJsonAsync($"/api/v1/counterparties/{other.Id}/loans", loanRequest);
        Assert.Equal(
            (await firstLoan.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!.Id,
            (await retryLoan.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!.Id);
        Assert.Equal(HttpStatusCode.BadRequest, reusedLoan.StatusCode);

        var chargeRequest = new CreateCounterpartyChargeRequest(
            "receivable", "200.0000", "TRY", income.Id, "2026-08-06", ClientRequestId: Guid.NewGuid());
        using var firstCharge = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/charges", chargeRequest);
        using var retryCharge = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/charges", chargeRequest);
        using var reusedCharge = await owner.PostAsJsonAsync($"/api/v1/counterparties/{other.Id}/charges", chargeRequest);
        Assert.Equal(HttpStatusCode.Created, firstCharge.StatusCode);
        Assert.Equal(
            (await firstCharge.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!.Id,
            (await retryCharge.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!.Id);
        Assert.Equal(HttpStatusCode.BadRequest, reusedCharge.StatusCode);

        var balance = await GetAsync(owner, customer.Id);
        Assert.Equal("500.0000", balance.OwedToYou);
        var report = await ReportAsync(owner);
        Assert.Equal("200.0000", report.TotalIncome);
        Assert.Equal("700.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>
    /// Senaryo 22: avans ayrı bir kayıt değildir. Alacak yokken yazılan tahsilat
    /// bizim borcumuz olarak görünür ve sonraki satıştan düşer.
    /// </summary>
    [Fact]
    public async Task AdvanceFromACustomer_IsACollection_AndIsNettedAgainstTheLaterSale()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-loan@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "0");
        var customer = await CreateCounterpartyAsync(owner, "Kaan Yapı", "business");
        var income = await FirstCategoryAsync(owner, "income", "business");

        using var loan = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "2000.0000", "TRY", account.Id, "2026-08-05"));
        Assert.Equal(HttpStatusCode.Created, loan.StatusCode);
        var afterAdvance = await GetAsync(owner, customer.Id);
        Assert.Equal("2000.0000", afterAdvance.OwedByYou);
        Assert.Equal("0.0000", afterAdvance.OwedToYou);

        using var sale = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/charges",
            new CreateCounterpartyChargeRequest("receivable", "5000.0000", "TRY", income.Id, "2026-08-12"));
        Assert.Equal(HttpStatusCode.Created, sale.StatusCode);

        var afterSale = await GetAsync(owner, customer.Id);
        Assert.Equal("3000.0000", afterSale.OwedToYou);
        Assert.Equal("0.0000", afterSale.OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("5000.0000", report.TotalIncome);
        Assert.Equal("2000.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>
    /// Kapsamı listede kişi, raporda kategori taşır (senaryo 13, 25, 26): şahsi
    /// kategoriyle yazılan alım şahsi giderdir; kişinin türünü değiştirmek
    /// hiçbir tutarı değiştirmez.
    /// </summary>
    [Fact]
    public async Task ChargeScope_ComesFromTheCategory_AndThePersonsKindOnlyBreaksTheTie()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-scope@example.test");
        var supplier = await CreateCounterpartyAsync(owner, "Demir Toptancı", "business");
        var relative = await CreateCounterpartyAsync(owner, "Kayınço", "personal");
        var expenses = await owner.GetFromJsonAsync<CategoryListResponse>("/api/v1/categories?type=expense");
        var businessCategory = expenses!.Items.First(item => item.DefaultScope == "business");
        var personalCategory = expenses.Items.First(item => item.DefaultScope == "personal");
        var openCategory = expenses.Items.First(item => item.DefaultScope is null);

        // İşletme kişisinden şahsi kategoriyle alım: gider şahsi, borç kişide.
        var home = await ChargeAsync(owner, supplier.Id, personalCategory.Id, "100.0000");
        Assert.Equal("personal", home.Scope);
        // İki kapsama açık kategoride kişinin türü belirler.
        Assert.Equal("business", (await ChargeAsync(owner, supplier.Id, openCategory.Id, "40.0000")).Scope);
        Assert.Equal("personal", (await ChargeAsync(owner, relative.Id, openCategory.Id, "30.0000")).Scope);
        // Açık seçim iki kapsama açık kategoride kişinin türünü ezer.
        Assert.Equal(
            "business",
            (await ChargeAsync(owner, relative.Id, openCategory.Id, "20.0000", "business")).Scope);

        // Kategoriyle çelişen seçim reddedilir, sessizce düzeltilmez.
        using var conflict = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "10.0000", "TRY", businessCategory.Id, "2026-08-05", "personal"));
        Assert.Equal(HttpStatusCode.BadRequest, conflict.StatusCode);
        Assert.Contains("counterparties.scope_conflict", await conflict.Content.ReadAsStringAsync());

        Assert.Equal("business", (await GetAsync(owner, supplier.Id)).Scope);
        var before = await GetAsync(owner, supplier.Id);
        var reportBefore = await ReportAsync(owner);

        using var retyped = await owner.PutAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}",
            new UpdateCounterpartyRequest("Demir Toptancı", true, Scope: "personal"));
        Assert.Equal(HttpStatusCode.OK, retyped.StatusCode);
        var after = await GetAsync(owner, supplier.Id);
        Assert.Equal("personal", after.Scope);
        Assert.Equal(before.OwedByYou, after.OwedByYou);
        Assert.Equal(reportBefore.TotalExpense, (await ReportAsync(owner)).TotalExpense);

        // Tür gönderilmeyen güncelleme türü silmez.
        using var renamed = await owner.PutAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}",
            new UpdateCounterpartyRequest("Demir Toptan", true));
        Assert.Equal("personal", (await renamed.Content.ReadFromJsonAsync<CounterpartyResponse>())!.Scope);
    }

    /// <summary>
    /// İşletmesi olmayan kullanıcının kişisi sorulmadan şahsidir; işletmesi
    /// olanda tür gönderilmezse boş kalır ve uydurulmaz.
    /// </summary>
    [Fact]
    public async Task NewPerson_IsPersonalWithoutABusiness_AndUnknownWithOneUnlessTold()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var personal = await CreateAuthenticatedClientAsync(
            factory, "loan-nobusiness@example.test", hasBusiness: false);
        using var business = await CreateAuthenticatedClientAsync(factory, "loan-hasbusiness@example.test");

        Assert.Equal("personal", (await CreateCounterpartyAsync(personal, "Arkadaş", null)).Scope);
        Assert.Null((await CreateCounterpartyAsync(business, "Eski Kişi", null)).Scope);
        Assert.Equal("business", (await CreateCounterpartyAsync(business, "Müşteri", "business")).Scope);

        using var invalid = await business.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest("Bozuk", Scope: "both"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Contains("counterparties.invalid_scope", await invalid.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Senaryo 16 ve 17: başka kullanıcının kişisi ve kaydı yoktur; pasif kişiye
    /// yeni borç para yazılmaz; geçersiz istekler reddedilir.
    /// </summary>
    [Fact]
    public async Task Loan_IsOwnerScoped_AndRejectsInactivePeopleFutureDaysAndBadInput()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "loan-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(factory, "loan-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "1000");
        var strangerAccount = await CreateAccountAsync(stranger, "Kasa", "1000");
        var friend = await CreateCounterpartyAsync(owner, "Ali", "personal");

        using var lent = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "100.0000", "TRY", account.Id, "2026-08-05"));
        var loan = (await lent.Content.ReadFromJsonAsync<CounterpartyLoanResponse>())!;

        using var foreignPerson = await stranger.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "100.0000", "TRY", strangerAccount.Id, "2026-08-05"));
        using var foreignCancel = await stranger.DeleteAsync($"/api/v1/counterparty-loans/{loan.Id}");
        using var foreignAccount = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "100.0000", "TRY", strangerAccount.Id, "2026-08-05"));
        Assert.Equal(HttpStatusCode.NotFound, foreignPerson.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, foreignCancel.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, foreignAccount.StatusCode);
        Assert.False((await GetLoanViaCancelProbeAsync(owner, friend.Id)).IsSettled);

        using var future = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "100.0000", "TRY", account.Id, "2999-01-01"));
        using var badDirection = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("sideways", "100.0000", "TRY", account.Id, "2026-08-05"));
        using var dueBeforeStart = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest(
                "receivable", "100.0000", "TRY", account.Id, "2026-08-05", "2026-08-01"));
        Assert.Equal(HttpStatusCode.BadRequest, future.StatusCode);
        Assert.Contains("counterparties.date_in_future", await future.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, badDirection.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, dueBeforeStart.StatusCode);

        using var deactivate = await owner.PutAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}", new UpdateCounterpartyRequest("Ali", false));
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        using var toInactive = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("receivable", "100.0000", "TRY", account.Id, "2026-08-05"));
        Assert.Equal(HttpStatusCode.Conflict, toInactive.StatusCode);
        // Pasif kişi kalan borcunu ödeyebilir.
        using var repay = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "100.0000", "TRY", account.Id, "2026-08-06"));
        Assert.Equal(HttpStatusCode.Created, repay.StatusCode);
        Assert.Equal("0.0000", (await GetAsync(owner, friend.Id)).OwedToYou);

        // Borç parası olan kişi silinemez; geçmişi vardır.
        using var delete = await owner.DeleteAsync($"/api/v1/counterparties/{friend.Id}");
        Assert.Equal(HttpStatusCode.Conflict, delete.StatusCode);
    }

    /// <summary>
    /// "10.000'e anlaştık, vade 45 gün, bugün 4.000 avans": satış ve avans tek
    /// istekte yazılır. Gelir 10.000, hesap +4.000, alacak 6.000; vade geçince
    /// gecikmiş tutar kalan 6.000'dir. Aynı istek kimliği ikinci kez yazmaz.
    /// </summary>
    [Fact]
    public async Task SaleWithAnAdvance_WritesBothOnce_AndLeavesTheRestDue()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "paidnow-sale@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "0");
        var customer = await CreateCounterpartyAsync(owner, "Kaan Yapı", "business");
        var income = await FirstCategoryAsync(owner, "income", "business");

        var request = new CreateCounterpartyChargeRequest(
            "receivable", "10000.0000", "TRY", income.Id, "2026-08-05",
            DueDate: "2026-09-19", ClientRequestId: Guid.NewGuid(),
            PaidNow: new CounterpartyPaidNowRequest("4000.0000", account.Id));
        using var first = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/charges", request);
        using var retry = await owner.PostAsJsonAsync($"/api/v1/counterparties/{customer.Id}/charges", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var sale = (await first.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
        var replayed = (await retry.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
        Assert.NotNull(sale.PaidNowPaymentId);
        Assert.Equal(sale.Id, replayed.Id);
        Assert.Equal(sale.PaidNowPaymentId, replayed.PaidNowPaymentId);

        var balance = await GetAsync(owner, customer.Id);
        Assert.Equal("6000.0000", balance.OwedToYou);
        Assert.Equal("0.0000", balance.OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("10000.0000", report.TotalIncome);
        Assert.Equal("4000.0000", Assert.Single(report.AccountBalances).Balance);

        var overdue = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{customer.Id}?asOfDate=2026-09-20");
        Assert.Equal("6000.0000", overdue!.OverdueReceivable);
        var notYet = await owner.GetFromJsonAsync<CounterpartyResponse>(
            $"/api/v1/counterparties/{customer.Id}?asOfDate=2026-09-19");
        Assert.Equal("0.0000", notYet!.OverdueReceivable);
    }

    /// <summary>
    /// Senaryo 19: 10.000 alım, 3.000 peşin ödenen. Gider 10.000, hesap −3.000,
    /// borç 7.000. Peşin tutar alımı aşamaz; başkasının hesabı kullanılamaz.
    /// </summary>
    [Fact]
    public async Task PurchaseWithAPartPaidNow_KeepsTheRestOwed_AndRejectsBadPaidNow()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "paidnow-buy@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(factory, "paidnow-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Banka", "5000");
        var strangerAccount = await CreateAccountAsync(stranger, "Banka", "5000");
        var supplier = await CreateCounterpartyAsync(owner, "Demir Toptancı", "business");
        var expense = await FirstCategoryAsync(owner, "expense", "business");

        using var bought = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "10000.0000", "TRY", expense.Id, "2026-08-05",
                PaidNow: new CounterpartyPaidNowRequest("3000.0000", account.Id)));
        Assert.Equal(HttpStatusCode.Created, bought.StatusCode);
        Assert.Equal("7000.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("10000.0000", report.TotalExpense);
        Assert.Equal("2000.0000", Assert.Single(report.AccountBalances).Balance);

        using var tooMuch = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "100.0000", "TRY", expense.Id, "2026-08-05",
                PaidNow: new CounterpartyPaidNowRequest("100.0001", account.Id)));
        Assert.Equal(HttpStatusCode.BadRequest, tooMuch.StatusCode);
        Assert.Contains("counterparties.paid_now_exceeds_charge", await tooMuch.Content.ReadAsStringAsync());

        using var foreignAccount = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "payable", "100.0000", "TRY", expense.Id, "2026-08-05",
                PaidNow: new CounterpartyPaidNowRequest("50.0000", strangerAccount.Id)));
        Assert.Equal(HttpStatusCode.BadRequest, foreignAccount.StatusCode);
        Assert.Contains("counterparties.account_unavailable", await foreignAccount.Content.ReadAsStringAsync());

        // Reddedilen istekler hiçbir şey yazmadı.
        Assert.Equal("7000.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        Assert.Equal("10000.0000", (await ReportAsync(owner)).TotalExpense);
    }

    /// <summary>
    /// Satış iptal edilince avans kendiliğinden gitmez: para alınmıştı, kişinin
    /// hesabında borcumuz olarak kalır. Yanlış girilmiş kayıtta ikisi birlikte
    /// iptal edilir.
    /// </summary>
    [Fact]
    public async Task CancellingTheSale_KeepsTheAdvanceUnlessAskedToCancelItToo()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "paidnow-cancel@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "0");
        var customer = await CreateCounterpartyAsync(owner, "Kaan Yapı", "business");
        var income = await FirstCategoryAsync(owner, "income", "business");

        async Task<CounterpartyChargeResponse> SaleAsync()
        {
            using var response = await owner.PostAsJsonAsync(
                $"/api/v1/counterparties/{customer.Id}/charges",
                new CreateCounterpartyChargeRequest(
                    "receivable", "10000.0000", "TRY", income.Id, "2026-08-05",
                    PaidNow: new CounterpartyPaidNowRequest("4000.0000", account.Id)));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
        }

        // İş iptal oldu: satış gider, avans kalır.
        var dealCancelled = await SaleAsync();
        using var keep = await owner.DeleteAsync($"/api/v1/counterparty-charges/{dealCancelled.Id}");
        Assert.Equal(HttpStatusCode.OK, keep.StatusCode);
        var afterKeep = await GetAsync(owner, customer.Id);
        Assert.Equal("4000.0000", afterKeep.OwedByYou);
        Assert.Equal("0.0000", afterKeep.OwedToYou);
        Assert.Equal("4000.0000", Assert.Single((await ReportAsync(owner)).AccountBalances).Balance);

        // Yanlış girildi: ikisi birlikte gider.
        var mistake = await SaleAsync();
        using var both = await owner.DeleteAsync(
            $"/api/v1/counterparty-charges/{mistake.Id}?withPaidNow=true");
        Assert.Equal(HttpStatusCode.OK, both.StatusCode);
        var afterBoth = await GetAsync(owner, customer.Id);
        Assert.Equal("4000.0000", afterBoth.OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("0.0000", report.TotalIncome);
        Assert.Equal("4000.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>
    /// Senaryo 6: 950'lik vadeli alım kendi kredi kartıyla ödenir, sonra kart
    /// bankadan ödenir. Gider bir kez 950; kart borcu +950; hesap yalnız kart
    /// ödemesinde −950. Kartla ödeme kart sayfasında ve akışta kartın adıyla
    /// görünür; iptali kart borcunu geri alır.
    /// </summary>
    [Fact]
    public async Task PurchasePaidByCreditCard_RaisesTheCardDebt_AndCountsTheExpenseOnce()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cardpay-owner@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "5000");
        var card = await CreateCardAsync(owner, "İş kartı", "10000.0000");
        var supplier = await CreateCounterpartyAsync(owner, "Demir Toptancı", "business");
        var expense = await FirstCategoryAsync(owner, "expense", "business");
        await ChargeAsync(owner, supplier.Id, expense.Id, "950.0000");

        using var paid = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "payable", "950.0000", "TRY", null, "2026-08-06", CreditCardId: card.Id));
        Assert.Equal(HttpStatusCode.Created, paid.StatusCode);
        var payment = (await paid.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!;
        Assert.Null(payment.AccountId);
        Assert.Equal(card.Id, payment.CreditCardId);

        Assert.Equal("0.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        Assert.Equal("950.0000", (await CardAsync(owner, card.Id)).CurrentDebt);
        Assert.Equal("9050.0000", (await CardAsync(owner, card.Id)).AvailableLimit);
        var report = await ReportAsync(owner);
        Assert.Equal("950.0000", report.TotalExpense);
        Assert.Equal("5000.0000", Assert.Single(report.AccountBalances).Balance);

        // Akışta kaynak kartın adıdır; işlem sonrası bakiye kart borcunu döner.
        var feed = await owner.GetStringAsync("/api/v1/financial-activities?all=true");
        Assert.Contains("İş kartı", feed);
        var balances = await owner.GetStringAsync(
            $"/api/v1/financial-activities/counterparty-settlement/{payment.Id}/balances");
        Assert.Contains("credit-card", balances);
        Assert.Contains(card.Id.ToString(), balances);

        using var cardPayment = await owner.PostAsJsonAsync(
            $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(bank.Id, "950.0000", "TRY", "2026-08-20", null));
        Assert.Equal(HttpStatusCode.Created, cardPayment.StatusCode);
        Assert.Equal("0.0000", (await CardAsync(owner, card.Id)).CurrentDebt);
        report = await ReportAsync(owner);
        Assert.Equal("950.0000", report.TotalExpense);
        Assert.Equal("4050.0000", Assert.Single(report.AccountBalances).Balance);

        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-payments/{payment.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal("950.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        Assert.Equal("-950.0000", (await CardAsync(owner, card.Id)).CurrentDebt);
    }

    /// <summary>
    /// Kart yalnız ödemede ve tek başına; limit kart harcamasıyla aynı kuralla
    /// denetlenir; başkasının kartı kullanılamaz. Reddedilen istek yazmaz.
    /// </summary>
    [Fact]
    public async Task CardPayment_IsPayableOnly_RespectsTheLimit_AndIsOwnerScoped()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "cardpay-rules@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(factory, "cardpay-stranger@example.test");
        var bank = await CreateAccountAsync(owner, "Banka", "5000");
        var card = await CreateCardAsync(owner, "İş kartı", "1000.0000");
        var strangerCard = await CreateCardAsync(stranger, "Kart", "1000.0000");
        var supplier = await CreateCounterpartyAsync(owner, "Demir Toptancı", "business");

        async Task<HttpResponseMessage> PayAsync(string direction, string amount, Guid? accountId, Guid cardId) =>
            await owner.PostAsJsonAsync(
                $"/api/v1/counterparties/{supplier.Id}/payments",
                new CreateCounterpartyPaymentRequest(
                    direction, amount, "TRY", accountId, "2026-08-06", CreditCardId: cardId));

        using var collection = await PayAsync("receivable", "10.0000", null, card.Id);
        using var withAccount = await PayAsync("payable", "10.0000", bank.Id, card.Id);
        using var foreign = await PayAsync("payable", "10.0000", null, strangerCard.Id);
        using var overLimit = await PayAsync("payable", "1000.0001", null, card.Id);
        Assert.Equal(HttpStatusCode.BadRequest, collection.StatusCode);
        Assert.Contains("counterparties.card_requires_payment", await collection.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, withAccount.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, foreign.StatusCode);
        Assert.Contains("counterparties.card_unavailable", await foreign.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Conflict, overLimit.StatusCode);
        Assert.Contains("credit_cards.limit_exceeded", await overLimit.Content.ReadAsStringAsync());

        using var atLimit = await PayAsync("payable", "1000.0000", null, card.Id);
        Assert.Equal(HttpStatusCode.Created, atLimit.StatusCode);
        Assert.Equal("1000.0000", (await CardAsync(owner, card.Id)).CurrentDebt);
        Assert.Equal("0.0000", (await CardAsync(stranger, strangerCard.Id)).CurrentDebt);
        // Fazla ödeme kırpılmaz: borç yokken ödenen tutar kişide alacak olur.
        Assert.Equal("1000.0000", (await GetAsync(owner, supplier.Id)).OwedToYou);
    }

    /// <summary>
    /// Senaryo 4: arkadaştan 3.000 borç aldım, 2.000 ödedim, 1.000'inden
    /// vazgeçti. Borç 0; hesap +3.000 −2.000; vazgeçme günü 1.000 gelir. İptal
    /// borcu ve geliri geri getirir; iptal idempotenttir.
    /// </summary>
    [Fact]
    public async Task ForgivenDebt_ClosesTheRest_AndWritesIncomeOnThatDay()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "writeoff-forgiven@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "0");
        var friend = await CreateCounterpartyAsync(owner, "Ali", "personal");
        var income = await FirstCategoryAsync(owner, "income", "personal");

        using var borrowed = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/loans",
            new CreateCounterpartyLoanRequest("payable", "3000.0000", "TRY", account.Id, "2026-08-05"));
        using var paid = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/payments",
            new CreateCounterpartyPaymentRequest("payable", "2000.0000", "TRY", account.Id, "2026-08-10"));
        Assert.Equal(HttpStatusCode.Created, paid.StatusCode);

        using var tooMuch = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/write-offs",
            new CreateCounterpartyWriteOffRequest("payable", "1000.0001", "TRY", income.Id, "2026-08-20"));
        Assert.Equal(HttpStatusCode.BadRequest, tooMuch.StatusCode);
        Assert.Contains("counterparties.write_off_exceeds_balance", await tooMuch.Content.ReadAsStringAsync());

        using var forgiven = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{friend.Id}/write-offs",
            new CreateCounterpartyWriteOffRequest("payable", "1000.0000", "TRY", income.Id, "2026-08-20"));
        Assert.Equal(HttpStatusCode.Created, forgiven.StatusCode);
        var writeOff = (await forgiven.Content.ReadFromJsonAsync<CounterpartyWriteOffResponse>())!;
        Assert.Equal("personal", writeOff.Scope);

        Assert.Equal("0.0000", (await GetAsync(owner, friend.Id)).OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("1000.0000", report.TotalIncome);
        Assert.Equal("0.0000", report.TotalExpense);
        Assert.Equal("1000.0000", Assert.Single(report.AccountBalances).Balance);
        Assert.Contains("counterparty-write-off", await owner.GetStringAsync("/api/v1/financial-activities?all=true"));

        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-write-offs/{writeOff.Id}");
        using var cancelAgain = await owner.DeleteAsync($"/api/v1/counterparty-write-offs/{writeOff.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cancelAgain.StatusCode);
        Assert.Equal("1000.0000", (await GetAsync(owner, friend.Id)).OwedByYou);
        Assert.Equal("0.0000", (await ReportAsync(owner)).TotalIncome);
    }

    /// <summary>
    /// Alınmayacak alacak gider kategorisiyle o gün gider olur; kategori türü
    /// yanlışsa ve başkasının kişisiyse reddedilir.
    /// </summary>
    [Fact]
    public async Task ReceivableWrittenOff_IsAnExpense_AndIsOwnerScoped()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "writeoff-receivable@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(factory, "writeoff-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Kasa", "0");
        var customer = await CreateCounterpartyAsync(owner, "Kaan Yapı", "business");
        var income = await FirstCategoryAsync(owner, "income", "business");
        var expense = await FirstCategoryAsync(owner, "expense", "business");
        await ChargeAsync(owner, customer.Id, income.Id, "1000.0000", direction: "receivable");
        using var collected = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/payments",
            new CreateCounterpartyPaymentRequest("receivable", "700.0000", "TRY", account.Id, "2026-08-10"));

        using var wrongCategory = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/write-offs",
            new CreateCounterpartyWriteOffRequest("receivable", "300.0000", "TRY", income.Id, "2026-08-20"));
        Assert.Equal(HttpStatusCode.BadRequest, wrongCategory.StatusCode);
        using var foreign = await stranger.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/write-offs",
            new CreateCounterpartyWriteOffRequest("receivable", "300.0000", "TRY", expense.Id, "2026-08-20"));
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);

        using var writtenOff = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{customer.Id}/write-offs",
            new CreateCounterpartyWriteOffRequest("receivable", "300.0000", "TRY", expense.Id, "2026-08-20"));
        Assert.Equal(HttpStatusCode.Created, writtenOff.StatusCode);
        var writeOff = (await writtenOff.Content.ReadFromJsonAsync<CounterpartyWriteOffResponse>())!;
        using var foreignCancel = await stranger.DeleteAsync($"/api/v1/counterparty-write-offs/{writeOff.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignCancel.StatusCode);

        Assert.Equal("0.0000", (await GetAsync(owner, customer.Id)).OwedToYou);
        var report = await ReportAsync(owner);
        Assert.Equal("1000.0000", report.TotalIncome);
        Assert.Equal("300.0000", report.TotalExpense);
        Assert.Equal("700.0000", Assert.Single(report.AccountBalances).Balance);
    }

    /// <summary>
    /// Senaryo 20: 4.200 borca 4.000 ödeme + `Kalanı indirim say`. Borç 0;
    /// hesap −4.000; gider alım gününde 200 azalır. İndirim ödemeye bağlıdır:
    /// tek başına geri alınamaz, ödemenin iptali onu da geri alır. Kalan borç
    /// paradan geliyorsa indirimle kapanmaz ve hiçbir şey yazılmaz.
    /// </summary>
    [Fact]
    public async Task PaymentClosingTheRemainderAsDiscount_LowersTheExpense_AndUndoesTogether()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "remainder-owner@example.test");
        var account = await CreateAccountAsync(owner, "Banka", "10000");
        var supplier = await CreateCounterpartyAsync(owner, "Demir Toptancı", "business");
        var expense = await FirstCategoryAsync(owner, "expense", "business");
        var purchase = await ChargeAsync(owner, supplier.Id, expense.Id, "4200.0000");

        using var paid = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "payable", "4000.0000", "TRY", account.Id, "2026-08-10", CloseRemainderAsDiscount: true));
        Assert.Equal(HttpStatusCode.Created, paid.StatusCode);
        var payment = (await paid.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!;
        Assert.Equal("200.0000", payment.RemainderDiscount);
        Assert.Equal("0.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        var report = await ReportAsync(owner);
        Assert.Equal("4000.0000", report.TotalExpense);
        Assert.Equal("6000.0000", Assert.Single(report.AccountBalances).Balance);

        Guid discountId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
            var discount = await db.CounterpartyChargeDiscounts.SingleAsync(item => item.ChargeId == purchase.Id);
            Assert.Equal(payment.Id, discount.PaymentId);
            Assert.Equal(200m, discount.Amount);
            discountId = discount.Id;
        }

        using var alone = await owner.DeleteAsync(
            $"/api/v1/counterparty-charges/{purchase.Id}/discounts/{discountId}");
        Assert.Equal(HttpStatusCode.Conflict, alone.StatusCode);
        Assert.Contains("counterparty_charges.discount_belongs_to_payment", await alone.Content.ReadAsStringAsync());

        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-payments/{payment.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        Assert.Equal("4200.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
        report = await ReportAsync(owner);
        Assert.Equal("4200.0000", report.TotalExpense);
        Assert.Equal("10000.0000", Assert.Single(report.AccountBalances).Balance);

        // Kalan borç paradan geliyor: indirimle kapanmaz, vazgeçme gerekir.
        using var borrowed = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/loans",
            new CreateCounterpartyLoanRequest("payable", "500.0000", "TRY", account.Id, "2026-08-11"));
        using var notCoverable = await owner.PostAsJsonAsync(
            $"/api/v1/counterparties/{supplier.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "payable", "100.0000", "TRY", account.Id, "2026-08-12", CloseRemainderAsDiscount: true));
        Assert.Equal(HttpStatusCode.BadRequest, notCoverable.StatusCode);
        Assert.Contains("counterparties.discount_exceeds_charges", await notCoverable.Content.ReadAsStringAsync());
        Assert.Equal("4700.0000", (await GetAsync(owner, supplier.Id)).OwedByYou);
    }

    private static async Task<CreditCardResponse> CreateCardAsync(HttpClient client, string name, string limit)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/credit-cards", new CreateCreditCardRequest(name, limit, "TRY", 10, 20));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CreditCardResponse>())!;
    }

    private static async Task<CreditCardResponse> CardAsync(HttpClient client, Guid id) =>
        (await client.GetFromJsonAsync<CreditCardResponse>($"/api/v1/credit-cards/{id}"))!;

    private static Task<CounterpartyResponse> GetLoanViaCancelProbeAsync(HttpClient client, Guid id) =>
        GetAsync(client, id);

    private static async Task<CounterpartyResponse> GetAsync(HttpClient client, Guid id) =>
        (await client.GetFromJsonAsync<CounterpartyResponse>($"/api/v1/counterparties/{id}"))!;

    private static async Task<MonthlyReportResponse> ReportAsync(HttpClient client) =>
        (await client.GetFromJsonAsync<MonthlyReportResponse>("/api/v1/reports/monthly?year=2026&month=8"))!;

    private static async Task<CounterpartyChargeResponse> ChargeAsync(
        HttpClient client, Guid counterpartyId, Guid categoryId, string amount, string? scope = null,
        string direction = "payable")
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/counterparties/{counterpartyId}/charges",
            new CreateCounterpartyChargeRequest(direction, amount, "TRY", categoryId, "2026-08-05", scope));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!;
    }

    private static async Task<CounterpartyResponse> CreateCounterpartyAsync(
        HttpClient client, string name, string? scope)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest(name, Scope: scope));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyResponse>())!;
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type, string scope)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>($"/api/v1/categories?type={type}");
        return categories!.Items.First(item => item.DefaultScope == scope);
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client, string name, string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "cash", "TRY", openingBalance));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory, string email, bool hasBusiness = true)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: hasBusiness));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
