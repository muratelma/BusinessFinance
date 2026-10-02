using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.Transfers;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessFinance.Api.Tests.Features.Stage125;

public sealed class FinancialActivityEndpointTests
{
    private const string Password = "Valid-Password-123!";

    [Fact]
    public async Task Feed_ReturnsEveryRealizedKindWithLosslessStringsAndOwnerIsolation()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "activity-api-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "activity-api-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var second = await CreateAccountAsync(owner, "Cash");
        var category = await GetCategoryAsync(owner, "expense");
        await CreateTransactionAsync(owner, account.Id, category.Id, "625.5000", "expense");
        await CreateTransferAsync(owner, account.Id, second.Id, "250.0000");

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        Assert.Equal(2, feed!.Pagination.TotalCount);
        var expense = Assert.Single(feed.Items, item => item.ActivityKind == "account-transaction");
        Assert.Equal("expense", expense.Effect);
        Assert.Equal("account", expense.SourceGroup);
        Assert.Equal("manual", expense.Origin);
        Assert.Equal("realized", expense.Status);
        // Money stays a four-decimal string end to end.
        Assert.Equal("625.5000", expense.Amount);
        Assert.Equal("TRY", expense.Currency);
        Assert.True(expense.CanCancel);
        Assert.True(expense.SupportsAttachments);

        var transfer = Assert.Single(feed.Items, item => item.ActivityKind == "transfer");
        Assert.Equal("neutral", transfer.Effect);
        Assert.Equal("Bank", transfer.SourceName);
        Assert.Equal("Cash", transfer.DestinationName);
        // Attachments exist only for budget transactions.
        Assert.False(transfer.SupportsAttachments);

        var strangerFeed = await stranger.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Empty(strangerFeed!.Items);
        Assert.Equal(0, strangerFeed.Pagination.TotalCount);
    }

    /// <summary>
    /// Hesaba geçmiş bir POS tahsilatı feed'de iki satırdır: satış geliri ve
    /// paranın hesaba yattığı nötr hareket. Komisyon ayrı satır değildir;
    /// satışın parçasıdır.
    /// </summary>
    /// <remarks>
    /// Satış ile yatış ayrı günlerin gerçeğidir: gelir tahsilat günü, para ise
    /// yatış günü. Komisyon ise satışla aynı kaydın parçasıdır; ayrı satır
    /// olduğunda beş satışın beş komisyonu alt alta duruyor ve hangisinin
    /// hangi satışa ait olduğu okunmuyordu (2 Ekim 2026 emülatör turu). Satış
    /// tahsilatın, yatış satırı yatışın kimliğini taşır (ADR 0019 T5).
    /// </remarks>
    [Fact]
    public async Task Feed_ProjectsAPosSettlementAsSaleWithItsCommissionAndADeposit()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "pos-feed-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "pos-feed-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var income = await GetCategoryAsync(owner, "income");
        var expense = await GetCategoryAsync(owner, "expense");

        Guid settlementId;
        Guid depositId;
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
            var userId = await db.Users
                .Where(user => user.Email == "pos-feed-owner@example.test")
                .Select(user => user.Id)
                .SingleAsync();
            var bank = await db.Accounts.SingleAsync(item => item.Id == account.Id);
            var saleCategory = await db.Categories.SingleAsync(item => item.Id == income.Id);
            var commissionCategory = await db.Categories.SingleAsync(item => item.Id == expense.Id);
            var createdAt = new DateTimeOffset(2026, 8, 15, 9, 0, 0, TimeSpan.Zero);
            var settlement = new PosSettlement(
                Guid.NewGuid(), userId, bank, saleCategory,
                new Money(1000m, CurrencyCode.TRY), 17.5m, TransactionScope.Business,
                new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 13), createdAt,
                commissionCategory, "Kartlı satış");
            var deposit = PosDeposit.Record(
                Guid.NewGuid(), userId, bank, [settlement],
                new Money(982.5m, CurrencyCode.TRY), new DateOnly(2026, 8, 13), createdAt);
            settlementId = settlement.Id;
            depositId = deposit.Id;
            db.PosSettlements.Add(settlement);
            db.PosDeposits.Add(deposit);
            await db.SaveChangesAsync();
        }

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        Assert.Equal(2, feed!.Pagination.TotalCount);
        Assert.All(feed.Items, item => Assert.Equal("pos", item.SourceGroup));

        var sale = Assert.Single(feed.Items, item => item.ActivityKind == "pos-sale");
        Assert.Equal("income", sale.Effect);
        Assert.Equal(settlementId, sale.ActivityId);
        // Gelir brüt tutar kadar tanınır; komisyon ondan düşülmez.
        Assert.Equal("1000.0000", sale.Amount);
        Assert.Equal("2026-08-10", sale.ActivityDate);
        Assert.Equal("business", sale.Scope);
        Assert.False(sale.CanCancel);
        Assert.False(sale.SupportsAttachments);
        // Komisyon satışın parçasıdır: tutarı, hesaba geçen neti ve geçiş günü
        // satış satırında taşınır.
        Assert.Equal("17.5000", sale.FeeAmount);
        Assert.Equal("982.5000", sale.NetAmount);
        Assert.Equal("2026-08-13", sale.ExpectedTransferDate);
        Assert.Equal("2026-08-13", sale.TransferredOn);
        // POS seçilmeden girilmiş tahsilat POS adı taşımaz.
        Assert.Null(sale.ChannelName);
        Assert.DoesNotContain(feed.Items, item => item.Effect == "expense");

        var deposited = Assert.Single(feed.Items, item => item.ActivityKind == "pos-deposit");
        // Yatış parayı taşır: gelir/gider yeniden tanınmaz ve kapsam taşımaz.
        Assert.Equal(depositId, deposited.ActivityId);
        Assert.Equal("neutral", deposited.Effect);
        Assert.Null(deposited.Scope);
        Assert.Equal("982.5000", deposited.Amount);
        Assert.Equal("2026-08-13", deposited.ActivityDate);
        Assert.Equal("Bank", deposited.DestinationName);
        // Başlık hesabın adına düşmez: boştur, istemci türün adını yazar.
        Assert.Equal(string.Empty, deposited.Title);
        Assert.Equal(1, deposited.SettlementCount);
        Assert.Null(deposited.FeeAmount);
        // Yatış kendi ucundan geri alınır, İşlemler'den iptal edilmez.
        Assert.False(deposited.CanCancel);

        // Kapsam filtresi kapsamsız satırı eler: yatış düşer, satış kalır.
        // İkisini birden göstermek aynı parayı iki kez saydırırdı.
        var businessOnly = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?scope=business");
        Assert.Equal(1, businessOnly!.Pagination.TotalCount);
        Assert.DoesNotContain(businessOnly.Items, item => item.ActivityKind == "pos-deposit");

        // Kalkan tür artık bir süzgeç değeri de değildir.
        using var removedKind = await owner.GetAsync(
            "/api/v1/financial-activities?activityKind=pos-commission");
        Assert.Equal(HttpStatusCode.BadRequest, removedKind.StatusCode);

        var strangerFeed = await stranger.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Empty(strangerFeed!.Items);
    }

    /// <summary>
    /// Yolda olan tahsilat feed'de yalnız satış satırıdır: para henüz geçmedi.
    /// </summary>
    /// <remarks>
    /// Yatış satırı yazılsaydı, feed hesabın henüz almadığı bir parayı almış
    /// gibi gösterirdi. Komisyonsuz tahsilatın komisyon satırı da yoktur:
    /// sıfır tutarlı bir gider, olmamış bir gideri kayda geçirmek olurdu.
    /// </remarks>
    [Fact]
    public async Task Feed_LeavesMoneyStillInTransitOutOfTheDepositRow()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "pos-transit-owner@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var income = await GetCategoryAsync(owner, "income");

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
            var userId = await db.Users
                .Where(user => user.Email == "pos-transit-owner@example.test")
                .Select(user => user.Id)
                .SingleAsync();
            var bank = await db.Accounts.SingleAsync(item => item.Id == account.Id);
            var saleCategory = await db.Categories.SingleAsync(item => item.Id == income.Id);
            db.PosSettlements.Add(new PosSettlement(
                Guid.NewGuid(), userId, bank, saleCategory,
                new Money(400m, CurrencyCode.TRY), 0m, TransactionScope.Business,
                new DateOnly(2026, 8, 12), new DateOnly(2026, 8, 15),
                new DateTimeOffset(2026, 8, 15, 9, 0, 0, TimeSpan.Zero)));
            await db.SaveChangesAsync();
        }

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        var sale = Assert.Single(feed!.Items);
        Assert.Equal("pos-sale", sale.ActivityKind);
        Assert.Equal("400.0000", sale.Amount);
    }

    [Fact]
    public async Task Feed_RejectsUnknownFilterValueInsteadOfIgnoringIt()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-filter@example.test");

        using var response = await client.GetAsync(
            "/api/v1/financial-activities?sourceGroup=not-a-group");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "financial_activities.invalid_filter_value",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// A transaction created by realizing a recurring occurrence cannot be cancelled.
    /// The occurrence holds exactly one result id with no way back, so cancelling here
    /// would leave it realized and pointing at a cancelled row. The feed reports this
    /// through canCancel, and the endpoint enforces it so a direct call cannot bypass
    /// the hidden button.
    /// </summary>
    [Fact]
    public async Task CancelTransaction_ForRecurringResult_IsRejectedAndReportedByTheFeed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-recurring@example.test");
        var account = await CreateAccountAsync(client, "Bank");
        var category = await GetCategoryAsync(client, "expense");

        using var planResponse = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "100.0000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-01", null, "clamp-to-last-day", "Rent"));
        planResponse.EnsureSuccessStatusCode();

        using var generate = await client.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest("2026-08-01"));
        generate.EnsureSuccessStatusCode();
        var generated = await generate.Content
            .ReadFromJsonAsync<GenerateRecurringOccurrencesResponse>();
        var occurrence = Assert.Single(generated!.GeneratedOccurrences);

        using var realize = await client.PostAsync(
            $"/api/v1/recurring-transactions/occurrences/{occurrence.Id}/realize", null);
        realize.EnsureSuccessStatusCode();
        var realized = await realize.Content
            .ReadFromJsonAsync<RealizeRecurringOccurrenceResponse>();
        var transactionId = realized!.Transaction!.Id;

        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items, item => item.ActivityId == transactionId);
        Assert.Equal("recurring", activity.Origin);
        Assert.False(activity.CanCancel);

        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{transactionId}");

        Assert.Equal(HttpStatusCode.Conflict, cancel.StatusCode);
        var problem = await cancel.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "transactions.cancel_origin_locked",
            problem!["code"]!.GetValue<string>());
    }

    [Fact]
    public async Task CancelCardCharge_ForInstallmentResult_IsRejectedAndReportedByTheFeed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-installment@example.test");
        var category = await GetCategoryAsync(client, "expense");
        using var cardResponse = await client.PostAsJsonAsync(
            "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "10000", "TRY", 15, 25));
        cardResponse.EnsureSuccessStatusCode();
        var card = await cardResponse.Content.ReadFromJsonAsync<CreditCardResponse>();

        using var planResponse = await client.PostAsJsonAsync(
            "/api/v1/installment-plans",
            new CreateInstallmentPlanRequest(
                card!.Id, category.Id, Guid.NewGuid(), "600.0000", "TRY", "business", 2, "2026-08-01", "Phone"));
        planResponse.EnsureSuccessStatusCode();
        var plan = await planResponse.Content.ReadFromJsonAsync<InstallmentPlanResponse>();

        using var realize = await client.PostAsync(
            $"/api/v1/installment-plans/{plan!.Id}/items/1/realize", null);
        realize.EnsureSuccessStatusCode();
        var charge = await realize.Content.ReadFromJsonAsync<CardChargeResponse>();

        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items, item => item.ActivityId == charge!.Id);
        Assert.Equal("installment", activity.Origin);
        Assert.False(activity.CanCancel);

        using var cancel = await client.DeleteAsync($"/api/v1/credit-card-charges/{charge!.Id}");

        Assert.Equal(HttpStatusCode.Conflict, cancel.StatusCode);
        var problem = await cancel.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "credit_card_charges.cancel_origin_locked",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// The origin guard must not break the cancel paths that always worked.
    /// </summary>
    [Fact]
    public async Task CancelTransaction_ForManualEntry_StillWorks()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "activity-api-manual@example.test");
        var account = await CreateAccountAsync(client, "Bank");
        var category = await GetCategoryAsync(client, "expense");
        var transaction = await CreateTransactionAsync(
            client, account.Id, category.Id, "40.0000", "expense");

        using var cancel = await client.DeleteAsync($"/api/v1/transactions/{transaction.Id}");

        cancel.EnsureSuccessStatusCode();
        var feed = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var activity = Assert.Single(feed!.Items);
        Assert.Equal("cancelled", activity.Status);
        Assert.NotNull(activity.CancelledAtUtc);
        // A cancelled activity stays in the history but can no longer be acted on.
        Assert.False(activity.CanCancel);

        var withoutCancelled = await client.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities?includeCancelled=false");
        Assert.Empty(withoutCancelled!.Items);
    }

    /// <summary>
    /// The planned view reports obligations that have not happened yet, with the enum
    /// strings and lossless money the client depends on, and reports no single total
    /// because summing income, expenses and neutral obligations would mislead.
    /// </summary>
    [Fact]
    public async Task PlannedFeed_ReportsUpcomingObligationsWithoutASingleTotal()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "planned-api-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "planned-api-stranger@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var category = await GetCategoryAsync(owner, "expense");

        using var plan = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, category.Id, "125.5000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-20", null, "clamp-to-last-day", "Rent"));
        plan.EnsureSuccessStatusCode();

        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");

        Assert.Equal("2026-08-15", planned!.AsOfDate);
        Assert.Equal(30, planned.DaysAhead);
        Assert.Equal(1, planned.TotalCount);
        Assert.Equal("2026-08-20", planned.NearestDueDate);
        var item = Assert.Single(planned.Items);
        Assert.Equal("recurring-occurrence", item.PlannedKind);
        Assert.Equal("expense", item.Effect);
        Assert.Equal("upcoming", item.Timing);
        Assert.Equal("ready", item.Readiness);
        Assert.Null(item.AttentionCode);
        Assert.Equal("realize", item.ActionKind);
        Assert.Equal("125.5000", item.Amount);
        Assert.Equal("TRY", item.Currency);
        Assert.Equal(account.Id, item.SourceId);
        Assert.Equal("Bank", item.SourceName);
        // No occurrence has been generated yet, so acting on it must generate one first.
        Assert.True(item.IsProjected);

        var strangerPlanned = await stranger.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");
        Assert.Equal(0, strangerPlanned!.TotalCount);
        Assert.Null(strangerPlanned.NearestDueDate);
    }

    /// <summary>
    /// The obligation flag separates money the user owes from money coming in, so the
    /// client does not re-derive that rule and drift from the server.
    /// </summary>
    [Fact]
    public async Task PlannedFeed_MarksWhichRowsAreTheUsersObligation()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "planned-api-burden@example.test");
        var account = await CreateAccountAsync(owner, "Bank");
        var expense = await GetCategoryAsync(owner, "expense");
        var income = await GetCategoryAsync(owner, "income");

        using var bill = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, expense.Id, "125.5000", "TRY", "bill-payment", "business", "monthly",
                "2026-08-20", null, "clamp-to-last-day", "Rent"));
        bill.EnsureSuccessStatusCode();
        using var salary = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, income.Id, "5000.0000", "TRY", "income", "business", "monthly",
                "2026-08-21", null, "clamp-to-last-day", "Salary"));
        salary.EnsureSuccessStatusCode();

        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");

        var billRow = Assert.Single(planned!.Items, item => item.Effect == "expense");
        var salaryRow = Assert.Single(planned.Items, item => item.Effect == "income");
        Assert.True(billRow.IsPaymentObligation);
        Assert.False(salaryRow.IsPaymentObligation);
    }

    [Fact]
    public async Task PlannedFeed_ProjectsOpenOneTimeObligationsWithoutOwnerLeakage()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "planned-obligation-owner@example.test");
        using var stranger = await CreateClientAsync(factory, "planned-obligation-stranger@example.test");
        var expense = await GetCategoryAsync(owner, "expense");
        var income = await GetCategoryAsync(owner, "income");

        Guid payableId;
        Guid receivableId;
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
            var userId = await db.Users
                .Where(user => user.Email == "planned-obligation-owner@example.test")
                .Select(user => user.Id)
                .SingleAsync();
            var expenseCategory = await db.Categories.SingleAsync(item => item.Id == expense.Id);
            var incomeCategory = await db.Categories.SingleAsync(item => item.Id == income.Id);
            var createdAt = new DateTimeOffset(2026, 8, 15, 9, 0, 0, TimeSpan.Zero);
            var payable = new Obligation(
                Guid.NewGuid(), userId, expenseCategory, DebtDirection.Payable,
                new Money(450m, CurrencyCode.TRY), TransactionScope.Business,
                new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 10), createdAt,
                description: "Electricity");
            var receivable = new Obligation(
                Guid.NewGuid(), userId, incomeCategory, DebtDirection.Receivable,
                new Money(300m, CurrencyCode.TRY), TransactionScope.Personal,
                new DateOnly(2026, 8, 2), new DateOnly(2026, 8, 20), createdAt,
                description: "Refund");
            payableId = payable.Id;
            receivableId = receivable.Id;
            db.Obligations.AddRange(payable, receivable);
            await db.SaveChangesAsync();
        }

        var planned = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");

        Assert.Equal(2, planned!.TotalCount);
        var payableRow = Assert.Single(planned.Items, item => item.PlannedActivityId == payableId);
        Assert.Equal("payable-obligation", payableRow.PlannedKind);
        Assert.Equal("neutral", payableRow.Effect);
        Assert.Equal("overdue", payableRow.Timing);
        Assert.Equal("ready", payableRow.Readiness);
        Assert.Null(payableRow.AttentionCode);
        Assert.Equal("pay-obligation", payableRow.ActionKind);
        Assert.True(payableRow.IsPaymentObligation);
        Assert.Equal(payableId, payableRow.ActionTargetId);

        var receivableRow = Assert.Single(
            planned.Items, item => item.PlannedActivityId == receivableId);
        Assert.Equal("receivable-obligation", receivableRow.PlannedKind);
        Assert.Equal("collect-obligation", receivableRow.ActionKind);
        Assert.False(receivableRow.IsPaymentObligation);

        var businessOnly = await owner.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30&scope=business");
        Assert.Equal(payableId, Assert.Single(businessOnly!.Items).PlannedActivityId);

        var strangerPlanned = await stranger.GetFromJsonAsync<PlannedActivityListResponse>(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=30");
        Assert.Empty(strangerPlanned!.Items);
    }

    [Fact]
    public async Task PlannedFeed_RejectsAHorizonThatIsNotOffered()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = await CreateClientAsync(factory, "planned-api-horizon@example.test");

        using var response = await client.GetAsync(
            "/api/v1/financial-activities/planned?asOfDate=2026-08-15&daysAhead=45");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "planned_activities.invalid_days_ahead",
            problem!["code"]!.GetValue<string>());
    }

    /// <summary>
    /// Aynı günün kayıtları türe göre değil <b>giriş sırasına</b> göre dizilir,
    /// en yeni üstte. Türe göre dizildiğinde beş satış üst üste, beş gider alt
    /// alta geliyor ve günün akışı okunmuyordu (2 Ekim 2026 emülatör turu).
    /// </summary>
    /// <remarks>
    /// Giriş anını sunucu yazar, kullanıcıdan istenmez. Bu kolondan önce
    /// yazılmış kaydın anı bilinmez ve uydurulmaz: günün sonuna düşer.
    /// </remarks>
    [Fact]
    public async Task Feed_OrdersADayByEntryTimeAcrossKinds_NewestFirst()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "activity-order@example.test");
        var bank = await CreateAccountAsync(owner, "Bank");
        var cash = await CreateAccountAsync(owner, "Cash");
        var expense = await GetCategoryAsync(owner, "expense");
        var income = await GetCategoryAsync(owner, "income");
        const string Day = "2026-08-14";

        var legacy = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, expense.Id, "5.0000", "TRY", "expense", "business", Day, "Eski kayıt"));
        var first = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, expense.Id, "10.0000", "TRY", "expense", "business", Day, "Birinci"));
        var second = await PostAsync<TransferResponse>(owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "20.0000", "TRY", Day, "İkinci"));
        var third = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, income.Id, "30.0000", "TRY", "income", "business", Day, "Üçüncü"));
        // Dünün kaydı bugün girilse de kendi gününde durur.
        await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, expense.Id, "40.0000", "TRY", "expense", "business", "2026-08-13", "Dün"));
        await ClearEntryTimeAsync(factory, legacy.Id);

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        Assert.Equal(
            [third.Id, second.Id, first.Id, legacy.Id],
            feed!.Items.Where(item => item.ActivityDate == Day).Select(item => item.ActivityId));
        Assert.Equal("Dün", feed.Items[^1].Title);
    }

    /// <summary>
    /// Para taşıyan kaydın başlığı hesap ya da kart adına düşmez. Açıklama
    /// yoksa başlık boş gelir ve istemci türün adını yazar ("Transfer", "Kart
    /// ödemesi"); hesap alt satırdaki "kaynak → hedef"te zaten vardır ve
    /// başlıkta tekrar edilince satır "Ziraat / Ziraat" diye okunuyordu.
    /// </summary>
    [Fact]
    public async Task Feed_DoesNotNameAMoneyMovementAfterAnAccountOrCard()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "activity-title@example.test");
        var bank = await CreateAccountAsync(owner, "Bank");
        var cash = await CreateAccountAsync(owner, "Cash");
        var card = await PostAsync<CreditCardResponse>(owner, "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "5000.0000", "TRY", 10, 20));
        await PostAsync<TransferResponse>(owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "20.0000", "TRY", "2026-08-14", null));
        await PostAsync<TransferResponse>(owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "25.0000", "TRY", "2026-08-14", "Kasaya avans"));
        var expense = await GetCategoryAsync(owner, "expense");
        await PostAsync<CardChargeResponse>(owner, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(expense.Id, "50.0000", "TRY", "business", "2026-08-14", null));
        await PostAsync<CardPaymentResponse>(owner, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(bank.Id, "30.0000", "TRY", "2026-08-14", null));

        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");

        var payment = Assert.Single(feed!.Items, item => item.ActivityKind == "card-payment");
        Assert.Equal(string.Empty, payment.Title);
        Assert.Equal("Card", payment.DestinationName);
        Assert.Equal(
            ["Kasaya avans", string.Empty],
            feed.Items.Where(item => item.ActivityKind == "transfer").Select(item => item.Title));
    }

    /// <summary>
    /// "İşlem sonrası bakiye": hareketin dokunduğu hesabın bakiyesi ya da
    /// kartın borcu, o hareketten hemen sonra. Sonradan girilen kayıtlar
    /// önceki hareketin "sonrası"nı değiştirmez.
    /// </summary>
    [Fact]
    public async Task Balances_FollowEachMovementInFeedOrder()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateClientAsync(factory, "activity-balance@example.test");
        using var stranger = await CreateClientAsync(factory, "activity-balance-stranger@example.test");
        var bank = await CreateAccountAsync(owner, "Bank");
        var cash = await CreateAccountAsync(owner, "Cash");
        var expenseCategory = await GetCategoryAsync(owner, "expense");
        var incomeCategory = await GetCategoryAsync(owner, "income");
        var card = await PostAsync<CreditCardResponse>(owner, "/api/v1/credit-cards",
            new CreateCreditCardRequest("Card", "5000.0000", "TRY", 10, 20));
        const string Day = "2026-08-14";

        // Banka 1000 ile açıldı; hepsi aynı gün, bu sırayla girildi.
        var expense = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, expenseCategory.Id, "100.0000", "TRY", "expense", "business", Day, "Gider"));
        var income = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, incomeCategory.Id, "50.0000", "TRY", "income", "business", Day, "Gelir"));
        var transfer = await PostAsync<TransferResponse>(owner, "/api/v1/transfers",
            new CreateTransferRequest(bank.Id, cash.Id, "200.0000", "TRY", Day, null));
        var charge = await PostAsync<CardChargeResponse>(owner, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(expenseCategory.Id, "300.0000", "TRY", "business", Day, "Kart"));
        var payment = await PostAsync<CardPaymentResponse>(owner, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(bank.Id, "120.0000", "TRY", Day, null));
        var cancelled = await PostAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(bank.Id, expenseCategory.Id, "70.0000", "TRY", "expense", "business", Day, "İptal"));
        using (var cancel = await owner.DeleteAsync($"/api/v1/transactions/{cancelled.Id}"))
        {
            cancel.EnsureSuccessStatusCode();
        }

        async Task<IReadOnlyList<ActivityBalanceResponse>> BalancesAsync(string kind, Guid id) =>
            (await owner.GetFromJsonAsync<ActivityBalanceListResponse>(
                $"/api/v1/financial-activities/{kind}/{id}/balances"))!.Items;

        // 1000 − 100; sonraki gelir, transfer ve kart ödemesi bunu değiştirmez.
        var afterExpense = Assert.Single(await BalancesAsync("account-transaction", expense.Id));
        Assert.Equal("account", afterExpense.Holder);
        Assert.Equal(bank.Id, afterExpense.Id);
        Assert.Equal("Bank", afterExpense.Name);
        Assert.Equal("900.0000", afterExpense.Balance);
        Assert.Equal("TRY", afterExpense.Currency);
        Assert.Equal("decreased", afterExpense.Change);

        var afterIncome = Assert.Single(await BalancesAsync("account-transaction", income.Id));
        Assert.Equal("950.0000", afterIncome.Balance);
        Assert.Equal("increased", afterIncome.Change);

        // Transfer iki hesaba dokunur: ikisinin de sonrası döner.
        var afterTransfer = await BalancesAsync("transfer", transfer.Id);
        Assert.Equal(
            [("Bank", "750.0000", "decreased"), ("Cash", "1200.0000", "increased")],
            afterTransfer.Select(item => (item.Name, item.Balance, item.Change)));

        // Kart harcaması hesabı değil kartın borcunu değiştirir.
        var afterCharge = Assert.Single(await BalancesAsync("card-charge", charge.Id));
        Assert.Equal("credit-card", afterCharge.Holder);
        Assert.Equal("Card", afterCharge.Name);
        Assert.Equal("300.0000", afterCharge.Balance);
        Assert.Equal("increased", afterCharge.Change);

        // Kart ödemesi ikisine de dokunur.
        var afterPayment = await BalancesAsync("card-payment", payment.Id);
        Assert.Equal(
            [("account", "630.0000", "decreased"), ("credit-card", "180.0000", "decreased")],
            afterPayment.Select(item => (item.Holder, item.Balance, item.Change)));

        // Son hareketin sonrası hesabın güncel bakiyesidir.
        var current = await owner.GetFromJsonAsync<AccountResponse>($"/api/v1/accounts/{bank.Id}");
        Assert.Equal(current!.Balance, afterPayment[0].Balance);

        // İptal edilmiş hareketin bakiye etkisi yoktur.
        Assert.Empty(await BalancesAsync("account-transaction", cancelled.Id));

        // Giriş anı bilinmeyen eski kaydın gün içindeki yeri bilinmez: sayı
        // uydurulmaz. Sonraki kayıtların toplamına ise günün en eskisi olarak
        // girmeye devam eder.
        await ClearEntryTimeAsync(factory, expense.Id);
        Assert.Empty(await BalancesAsync("account-transaction", expense.Id));
        Assert.Equal(
            "950.0000",
            Assert.Single(await BalancesAsync("account-transaction", income.Id)).Balance);

        // Başkasının hareketi, olmayan hareket ve türü tutmayan kimlik aynı
        // cevabı verir.
        foreach (var path in new[]
                 {
                     $"/api/v1/financial-activities/account-transaction/{income.Id}/balances",
                     $"/api/v1/financial-activities/account-transaction/{Guid.NewGuid()}/balances",
                 })
        {
            using var response = await stranger.GetAsync(path);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        using var wrongKind = await owner.GetAsync(
            $"/api/v1/financial-activities/transfer/{income.Id}/balances");
        Assert.Equal(HttpStatusCode.NotFound, wrongKind.StatusCode);
        using var unknownKind = await owner.GetAsync(
            $"/api/v1/financial-activities/not-a-kind/{income.Id}/balances");
        Assert.Equal(HttpStatusCode.BadRequest, unknownKind.StatusCode);
    }

    private static async Task<TResponse> PostAsync<TResponse>(
        HttpClient client,
        string path,
        object request)
    {
        using var response = await client.PostAsync(
            path, JsonContent.Create(request, request.GetType()));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TResponse>())!;
    }

    /// <summary>
    /// Giriş anı tutulmadan önce yazılmış bir kaydı taklit eder: gölge kolonu
    /// boşaltır.
    /// </summary>
    private static async Task ClearEntryTimeAsync(BusinessFinanceApiFactory factory, Guid transactionId)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessFinanceDbContext>();
        var transaction = await db.Transactions.SingleAsync(item => item.Id == transactionId);
        db.Entry(transaction).Property<DateTimeOffset?>("CreatedAtUtc").CurrentValue = null;
        await db.SaveChangesAsync();
    }

    private static async Task<HttpClient> CreateClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        register.EnsureSuccessStatusCode();
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        login.EnsureSuccessStatusCode();
        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        return client;
    }

    private static async Task<AccountResponse> CreateAccountAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts", new CreateAccountRequest(name, "bank", "TRY", "1000"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<CategoryResponse> GetCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<TransactionResponse> CreateTransactionAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, amount, "TRY", type, "business", "2026-08-14", "Synthetic"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
    }

    private static async Task CreateTransferAsync(
        HttpClient client,
        Guid sourceAccountId,
        Guid destinationAccountId,
        string amount)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transfers",
            new CreateTransferRequest(
                sourceAccountId, destinationAccountId, amount, "TRY", "2026-08-13", null));
        response.EnsureSuccessStatusCode();
    }
}
