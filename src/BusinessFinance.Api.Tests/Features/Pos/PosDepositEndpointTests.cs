using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.Pos;

/// <summary>
/// "Hesaba geçenleri işaretle" (ADR 0019 T5): bir yatış bir ya da birkaç
/// yoldaki tahsilatı bankanın gerçekten yatırdığı tutarla kapatır.
/// </summary>
public sealed class PosDepositEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// Yatışın bütün etkisi tek testte: hesaba tam olarak yatan tutar girer,
    /// eksik kalan kısım kesinti gideri olur ve yatış hiçbir gelir yazmaz.
    /// </summary>
    /// <remarks>
    /// Hesaba tahsilatların neti girer, kesinti gideri çıkar; farkı tam olarak
    /// yatan tutardır. Kesinti sıradan bir giderdir (rapora ve İşlemler'e
    /// öyle girer) ama tek başına iptal edilemez: iptal edilseydi yatış,
    /// hesaba gerçekte geçmemiş bir tutarı geçmiş gösterirdi.
    /// </remarks>
    [Fact]
    public async Task Deposit_ClosesSeveralSettlements_AndWritesTheShortfallAsADeduction()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-owner@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);

        // 1000 @ %1,5 → net 985; 500 @ %1,5 → net 492,5. Beklenen 1477,5.
        var first = await CreateSettlementAsync(owner, f, "1000.0000", today.AddDays(-2));
        var second = await CreateSettlementAsync(owner, f, "500.0000", today.AddDays(-1));
        var ids = $"settlementIds={first.Id}&settlementIds={second.Id}";

        // Formun ilk açılışı: tutar gönderilmedi, beklenen yatmış sayılır.
        var initial = await owner.GetFromJsonAsync<PosDepositPreviewResponse>(
            $"/api/v1/pos-deposits/preview?{ids}");
        Assert.Equal(f.AccountId, initial!.AccountId);
        Assert.Equal(2, initial.SettlementCount);
        Assert.Equal("1477.5000", initial.ExpectedAmount);
        Assert.Equal("1477.5000", initial.DepositedAmount);
        Assert.Equal("0.0000", initial.DeductionAmount);
        Assert.False(initial.ExceedsExpected);
        // Kesinti kategorisi POS'un komisyon kategorisinden dolu gelir.
        Assert.Equal(f.CommissionCategoryId, initial.DeductionCategoryId);
        // Yatış, kapattığı en geç tahsilattan önce olamaz.
        Assert.Equal(Date(today.AddDays(-1)), initial.EarliestDepositDate);

        var shortfall = await owner.GetFromJsonAsync<PosDepositPreviewResponse>(
            $"/api/v1/pos-deposits/preview?{ids}&depositedAmount=1470.0000");
        Assert.Equal("7.5000", shortfall!.DeductionAmount);
        Assert.False(shortfall.ExceedsExpected);

        var excess = await owner.GetFromJsonAsync<PosDepositPreviewResponse>(
            $"/api/v1/pos-deposits/preview?{ids}&depositedAmount=1500.0000");
        Assert.True(excess!.ExceedsExpected);
        Assert.Equal("0.0000", excess.DeductionAmount);

        // Önizleme hiçbir şey yazmaz.
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [first.Id, second.Id], "1470.0000", Date(today)));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var deposit = (await create.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.Equal("1477.5000", deposit.ExpectedAmount);
        // Brüt − komisyon = beklenen: yatan tutarın nereden geldiği.
        Assert.Equal("1500.0000", deposit.GrossAmount);
        Assert.Equal("22.5000", deposit.CommissionAmount);
        Assert.Equal("1470.0000", deposit.DepositedAmount);
        Assert.Equal("7.5000", deposit.DeductionAmount);
        Assert.NotNull(deposit.DeductionTransactionId);
        Assert.Equal(f.CommissionCategoryId, deposit.DeductionCategoryId);
        Assert.Equal(Date(today), deposit.DepositDate);
        Assert.False(deposit.IsCancelled);
        Assert.Equal(2, deposit.Settlements.Count);
        Assert.All(deposit.Settlements, settlement =>
        {
            Assert.False(settlement.IsInTransit);
            Assert.Equal(deposit.Id, settlement.PosDepositId);
            Assert.Equal(Date(today), settlement.TransferredOn);
        });

        // Hesaba tam olarak bankanın yatırdığı tutar girdi.
        Assert.Equal("2470.0000", await BalanceAsync(owner, f.AccountId));

        // Yatış gelir yazmadı; kesinti gider olarak eklendi (22,5 komisyon +
        // 7,5 kesinti).
        var report = await ReportAsync(owner, today.AddDays(-2), today);
        Assert.Equal(1500m, report.Income);
        Assert.Equal(30m, report.Expense);

        var advanced = await AdvancedAsync(owner, today);
        Assert.Equal("0.0000", advanced.NetWorth.MoneyInTransit);
        Assert.Equal("2470.0000", advanced.NetWorth.LiquidAssets);
        Assert.Equal("2470.0000", advanced.NetWorth.NetWorth);

        // Okuma aynı kaydı döner; yatıştan hemen sonraki hesap bakiyesini de
        // taşır (kesinti gideri dahil).
        var read = await owner.GetFromJsonAsync<PosDepositResponse>(
            $"/api/v1/pos-deposits/{deposit.Id}");
        Assert.Equal(deposit.DeductionTransactionId, read!.DeductionTransactionId);
        Assert.Equal(2, read.Settlements.Count);
        Assert.Equal("2470.0000", read.BalanceAfter);
        var balances = await owner.GetFromJsonAsync<ActivityBalanceListResponse>(
            $"/api/v1/financial-activities/pos-deposit/{deposit.Id}/balances");
        var afterDeposit = Assert.Single(balances!.Items);
        Assert.Equal("2470.0000", afterDeposit.Balance);
        Assert.Equal("increased", afterDeposit.Change);

        // İşlemler: yatış tek satırdır ve gerçekten yatan tutarı gösterir.
        // Kesinti ayrı satır değildir; yatışın parçası olarak taşınır
        // (fikir kaydı F06, KP11: "yatışın detayında görünsün").
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var depositRow = Assert.Single(feed!.Items, item => item.ActivityKind == "pos-deposit");
        Assert.Equal(deposit.Id, depositRow.ActivityId);
        Assert.Equal("1470.0000", depositRow.Amount);
        Assert.Equal("7.5000", depositRow.FeeAmount);
        Assert.Equal(2, depositRow.SettlementCount);
        Assert.Equal("Sentetik POS", depositRow.ChannelName);
        Assert.Equal("neutral", depositRow.Effect);
        Assert.Equal("realized", depositRow.Status);
        Assert.Equal(Date(today), depositRow.ActivityDate);
        Assert.False(depositRow.CanCancel);
        Assert.DoesNotContain(
            feed.Items, item => item.ActivityId == deposit.DeductionTransactionId);
        // Satışların komisyonu da kendi satışlarının parçasıdır.
        Assert.DoesNotContain(feed.Items, item => item.Effect == "expense");
        Assert.Equal(
            ["15.0000", "7.5000"],
            feed.Items.Where(item => item.ActivityKind == "pos-sale")
                .OrderBy(item => item.ActivityDate)
                .Select(item => item.FeeAmount));

        // Kesinti yine sıradan bir giderdir ve kapattığı satışın kapsamını
        // taşır; yalnız akışta ayrı satır olmaz.
        var deduction = await owner.GetFromJsonAsync<TransactionResponse>(
            $"/api/v1/transactions/{deposit.DeductionTransactionId}");
        Assert.Equal("7.5000", deduction!.Amount);
        Assert.Equal("expense", deduction.Type);
        Assert.Equal("business", deduction.Scope);
        Assert.Equal(f.CommissionCategoryId, deduction.CategoryId);

        // Tek başına iptal edilemez: doğrudan çağrı reddedilir.
        using var cancelDeduction = await owner.DeleteAsync(
            $"/api/v1/transactions/{deposit.DeductionTransactionId}");
        Assert.Equal(HttpStatusCode.Conflict, cancelDeduction.StatusCode);
        Assert.Equal("transactions.cancel_origin_locked", await CodeAsync(cancelDeduction));
        Assert.Equal("2470.0000", await BalanceAsync(owner, f.AccountId));
    }

    /// <summary>
    /// Geri alma kapattıklarını yola döndürür ve kesinti giderini iptal eder;
    /// yatış kaydı kalır (silme yerine iptal).
    /// </summary>
    [Fact]
    public async Task RevertedDeposit_ReturnsSettlementsToTransit_AndCancelsTheDeduction()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-revert@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var first = await CreateSettlementAsync(owner, f, "1000.0000", today);
        var second = await CreateSettlementAsync(owner, f, "500.0000", today);
        var deposit = await DepositAsync(owner, [first.Id, second.Id], "1470.0000", today);

        using var revert = await owner.DeleteAsync($"/api/v1/pos-deposits/{deposit.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        var reverted = (await revert.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.True(reverted.IsCancelled);
        Assert.NotNull(reverted.CancelledAtUtc);
        Assert.Empty(reverted.Settlements);
        // Kayıt ne yazdığını hatırlar.
        Assert.Equal("1470.0000", reverted.DepositedAmount);
        Assert.Equal("7.5000", reverted.DeductionAmount);

        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));
        var transit = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Equal(2, transit!.InTransitCount);
        Assert.Equal("1477.5000", transit.MoneyInTransit);
        Assert.All(transit.Items, settlement => Assert.Null(settlement.PosDepositId));

        // Satış ve komisyon yerinde, kesinti gideri düştü.
        var report = await ReportAsync(owner, today, today);
        Assert.Equal(1500m, report.Income);
        Assert.Equal(22.5m, report.Expense);

        // İşlemler geri alınan yatışı iptal edilmiş gösterir; kesinti gideri
        // de iptal edilmiştir.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var depositRow = Assert.Single(feed!.Items, item => item.ActivityKind == "pos-deposit");
        Assert.Equal(deposit.Id, depositRow.ActivityId);
        Assert.Equal("cancelled", depositRow.Status);
        Assert.NotNull(depositRow.CancelledAtUtc);
        Assert.Equal(0, depositRow.SettlementCount);
        var deduction = await owner.GetFromJsonAsync<TransactionResponse>(
            $"/api/v1/transactions/{deposit.DeductionTransactionId}");
        Assert.True(deduction!.IsCancelled);
        // Geri alınmış yatışın "sonrası" yoktur.
        Assert.Null(reverted.BalanceAfter);
        var balances = await owner.GetFromJsonAsync<ActivityBalanceListResponse>(
            $"/api/v1/financial-activities/pos-deposit/{deposit.Id}/balances");
        Assert.Empty(balances!.Items);

        // Geri alma idempotenttir.
        using var again = await owner.DeleteAsync($"/api/v1/pos-deposits/{deposit.Id}");
        Assert.Equal(HttpStatusCode.OK, again.StatusCode);
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));

        // Yola dönen tahsilatlar yeniden, bu kez beklenen tutarla kapatılabilir.
        var corrected = await DepositAsync(owner, [first.Id, second.Id], "1477.5000", today);
        Assert.NotEqual(deposit.Id, corrected.Id);
        Assert.Equal("0.0000", corrected.DeductionAmount);
        Assert.Null(corrected.DeductionTransactionId);
        Assert.Equal("2477.5000", await BalanceAsync(owner, f.AccountId));
    }

    /// <summary>
    /// Aynı istek kimliği ikinci bir yatış ya da kesinti gideri yazmaz.
    /// </summary>
    [Fact]
    public async Task Deposit_IsIdempotentPerClientRequest()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-idempotent@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var settlement = await CreateSettlementAsync(owner, f, "1000.0000", today);
        var request = new CreatePosDepositRequest(
            Guid.NewGuid(), [settlement.Id], "980.0000", Date(today));

        using var first = await owner.PostAsJsonAsync("/api/v1/pos-deposits", request);
        using var second = await owner.PostAsJsonAsync("/api/v1/pos-deposits", request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var firstDeposit = (await first.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        var secondDeposit = (await second.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.Equal(firstDeposit.Id, secondDeposit.Id);
        Assert.Equal(firstDeposit.DeductionTransactionId, secondDeposit.DeductionTransactionId);
        Assert.Equal("1980.0000", await BalanceAsync(owner, f.AccountId));
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var row = Assert.Single(feed!.Items, item => item.ActivityKind == "pos-deposit");
        Assert.Equal("5.0000", row.FeeAmount);
        var transactions = await owner.GetFromJsonAsync<TransactionListResponse>(
            "/api/v1/transactions");
        Assert.Single(transactions!.Items);
    }

    /// <summary>
    /// Beklenenden fazla yatan tutar reddedilir: fazlası gelir değil, fazla
    /// yazılmış komisyondur. Reddedilen istek hiçbir şey yazmaz.
    /// </summary>
    [Fact]
    public async Task Deposit_RejectsInvalidRequests_AndWritesNothing()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-invalid@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var settlement = await CreateSettlementAsync(owner, f, "1000.0000", today.AddDays(-1));
        // Başka bir hesaba geçen tahsilat: tek yatış tek hesaba düşer.
        var otherAccount = await CreateAccountAsync(owner, "İkinci banka", "bank", "0.0000");
        using var otherCreate = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                otherAccount.Id, f.SalesCategoryId, "200.0000", "TRY",
                Date(today.AddDays(-1)), Date(today), Scope: "business"));
        Assert.Equal(HttpStatusCode.Created, otherCreate.StatusCode);
        var other = (await otherCreate.Content.ReadFromJsonAsync<PosSettlementResponse>())!;

        async Task AssertRejectedAsync(
            HttpStatusCode status, string code, CreatePosDepositRequest request)
        {
            using var response = await owner.PostAsJsonAsync("/api/v1/pos-deposits", request);
            Assert.Equal(status, response.StatusCode);
            Assert.Equal(code, await CodeAsync(response));
        }

        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.amount_exceeds_expected",
            new(Guid.NewGuid(), [settlement.Id], "985.0001", Date(today)));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.invalid_amount",
            new(Guid.NewGuid(), [settlement.Id], "0.0000", Date(today)));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.invalid_amount",
            new(Guid.NewGuid(), [settlement.Id], "bin lira", Date(today)));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.settlements_required",
            new(Guid.NewGuid(), [], "985.0000", Date(today)));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.settlements_required",
            new(Guid.NewGuid(), [settlement.Id, settlement.Id], "985.0000", Date(today)));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.mixed_accounts",
            new(Guid.NewGuid(), [settlement.Id, other.Id], "1185.0000", Date(today)));
        // Para satıştan önce yatamaz; gelecekte de yatmış olamaz.
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.invalid_deposit_date",
            new(Guid.NewGuid(), [settlement.Id], "985.0000", Date(today.AddDays(-2))));
        await AssertRejectedAsync(
            HttpStatusCode.BadRequest, "pos_deposits.invalid_deposit_date",
            new(Guid.NewGuid(), [settlement.Id], "985.0000", Date(today.AddDays(1))));
        await AssertRejectedAsync(
            HttpStatusCode.NotFound, "pos_deposits.settlement_not_found",
            new(Guid.NewGuid(), [settlement.Id, Guid.NewGuid()], "985.0000", Date(today)));

        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));
        var transit = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Equal(2, transit!.InTransitCount);
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.DoesNotContain(feed!.Items, item => item.ActivityKind == "pos-deposit");

        // Hesaba geçmiş tahsilat ikinci bir yatışla kapatılamaz.
        await DepositAsync(owner, [settlement.Id], "985.0000", today);
        await AssertRejectedAsync(
            HttpStatusCode.Conflict, "pos_deposits.settlement_not_in_transit",
            new(Guid.NewGuid(), [settlement.Id], "985.0000", Date(today)));
        Assert.Equal("1985.0000", await BalanceAsync(owner, f.AccountId));
    }

    /// <summary>
    /// Kesinti varsa yazılacağı kategori bilinmelidir; sunucu kategori uydurmaz.
    /// POS'suz ve komisyonsuz girilmiş tahsilatın önerecek bir kategorisi yoktur.
    /// </summary>
    [Fact]
    public async Task Deduction_NeedsACategory_WhenTheSettlementsOfferNone()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-category@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                f.AccountId, f.SalesCategoryId, "400.0000", "TRY",
                Date(today), Date(today), Scope: "personal"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var settlement = (await create.Content.ReadFromJsonAsync<PosSettlementResponse>())!;

        var preview = await owner.GetFromJsonAsync<PosDepositPreviewResponse>(
            $"/api/v1/pos-deposits/preview?settlementIds={settlement.Id}&depositedAmount=390.0000");
        Assert.Equal("10.0000", preview!.DeductionAmount);
        Assert.Null(preview.DeductionCategoryId);

        using var missing = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [settlement.Id], "390.0000", Date(today)));
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        Assert.Equal("pos_deposits.deduction_category_required", await CodeAsync(missing));

        // Gelir kategorisi kesinti taşıyamaz.
        using var wrongType = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [settlement.Id], "390.0000", Date(today), f.SalesCategoryId));
        Assert.Equal(HttpStatusCode.BadRequest, wrongType.StatusCode);
        Assert.Equal("pos_deposits.deduction_category_unavailable", await CodeAsync(wrongType));
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.AccountId));

        // Kesinti yoksa kategori de sorulmaz.
        var exact = await owner.GetFromJsonAsync<PosDepositPreviewResponse>(
            $"/api/v1/pos-deposits/preview?settlementIds={settlement.Id}");
        Assert.Equal("0.0000", exact!.DeductionAmount);

        using var withCategory = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [settlement.Id], "390.0000", Date(today),
                f.CommissionCategoryId));
        Assert.Equal(HttpStatusCode.Created, withCategory.StatusCode);
        var deposit = (await withCategory.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.Equal(f.CommissionCategoryId, deposit.DeductionCategoryId);

        // Kesinti, kapattığı satışın tarafındadır: tahsilat şahsi yazıldıysa
        // kesinti de şahsidir, hesabın ya da kategorinin etiketine bakılmaz.
        var deduction = await owner.GetFromJsonAsync<TransactionResponse>(
            $"/api/v1/transactions/{deposit.DeductionTransactionId}");
        Assert.Equal("personal", deduction!.Scope);
    }

    /// <summary>
    /// Başkasının tahsilatı ve yatışı, hiç var olmamış bir kayıtla aynı cevabı
    /// verir; yabancı kullanıcı hiçbir şey değiştiremez.
    /// </summary>
    [Fact]
    public async Task AnotherUsersSettlementsAndDeposits_AnswerLikeMissingOnes()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "deposit-isolation-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "deposit-isolation-stranger@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var inTransit = await CreateSettlementAsync(owner, f, "1000.0000", today);
        var closed = await CreateSettlementAsync(owner, f, "500.0000", today);
        var deposit = await DepositAsync(owner, [closed.Id], "492.5000", today);

        foreach (var settlementId in new[] { inTransit.Id, Guid.NewGuid() })
        {
            using var create = await stranger.PostAsJsonAsync(
                "/api/v1/pos-deposits",
                new CreatePosDepositRequest(
                    Guid.NewGuid(), [settlementId], "985.0000", Date(today)));
            Assert.Equal(HttpStatusCode.NotFound, create.StatusCode);
            Assert.Equal("pos_deposits.settlement_not_found", await CodeAsync(create));

            using var preview = await stranger.GetAsync(
                $"/api/v1/pos-deposits/preview?settlementIds={settlementId}");
            Assert.Equal(HttpStatusCode.NotFound, preview.StatusCode);
            Assert.Equal("pos_deposits.settlement_not_found", await CodeAsync(preview));
        }

        foreach (var depositId in new[] { deposit.Id, Guid.NewGuid() })
        {
            using var read = await stranger.GetAsync($"/api/v1/pos-deposits/{depositId}");
            Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
            using var revert = await stranger.DeleteAsync($"/api/v1/pos-deposits/{depositId}");
            Assert.Equal(HttpStatusCode.NotFound, revert.StatusCode);
        }

        // Sahibin kayıtları yerinde.
        var read2 = await owner.GetFromJsonAsync<PosDepositResponse>(
            $"/api/v1/pos-deposits/{deposit.Id}");
        Assert.False(read2!.IsCancelled);
        Assert.Equal("1492.5000", await BalanceAsync(owner, f.AccountId));
        var transit = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Equal(inTransit.Id, Assert.Single(transit!.Items).Id);

        // Yabancının akışında sahibin yatışı görünmez.
        var strangerFeed = await stranger.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.Empty(strangerFeed!.Items);
    }

    private sealed record Seed(
        Guid AccountId,
        Guid SalesCategoryId,
        Guid CommissionCategoryId,
        Guid PosDefinitionId);

    /// <summary>
    /// Banka hesabı (1000) ve %1,5 komisyonlu, komisyon kategorisi olan bir POS.
    /// </summary>
    private static async Task<Seed> SeedAsync(HttpClient client)
    {
        var account = await CreateAccountAsync(client, "Sentetik banka", "bank", "1000.0000");
        var sales = await FirstCategoryAsync(client, "income");
        var commission = await FirstCategoryAsync(client, "expense");
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest(
                "Sentetik POS", account.Id, sales.Id, "0.0150", 1, false, commission.Id));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var definition = (await response.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        return new Seed(account.Id, sales.Id, commission.Id, definition.Id);
    }

    private static async Task<PosSettlementResponse> CreateSettlementAsync(
        HttpClient client,
        Seed seed,
        string grossAmount,
        DateOnly settlementDate)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, grossAmount, "TRY", Date(settlementDate),
                Scope: "business", PosDefinitionId: seed.PosDefinitionId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
    }

    private static async Task<PosDepositResponse> DepositAsync(
        HttpClient client,
        IReadOnlyList<Guid> settlementIds,
        string depositedAmount,
        DateOnly depositDate)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), settlementIds, depositedAmount, Date(depositDate)));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<PosDepositResponse>())!;
    }

    private static async Task<string> BalanceAsync(HttpClient client, Guid accountId)
    {
        var account = await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{accountId}");
        return account!.Balance;
    }

    private static async Task<string?> CodeAsync(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        return problem?["code"]?.GetValue<string>();
    }

    private static string Date(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    /// <summary>
    /// Verilen günleri kapsayan ayların gelir ve gider toplamı. Tahsilatlar
    /// "bugün"e göre yazıldığı için ay dönümünde iki aya düşebilir; test hangi
    /// gün koşarsa koşsun aynı toplamı okur.
    /// </summary>
    private static async Task<(decimal Income, decimal Expense)> ReportAsync(
        HttpClient client,
        DateOnly from,
        DateOnly to)
    {
        var months = new[] { (from.Year, from.Month), (to.Year, to.Month) }.Distinct();
        var income = 0m;
        var expense = 0m;
        foreach (var (year, month) in months)
        {
            var report = (await client.GetFromJsonAsync<MonthlyReportResponse>(
                $"/api/v1/reports/monthly?year={year}&month={month}"))!;
            income += decimal.Parse(report.TotalIncome, CultureInfo.InvariantCulture);
            expense += decimal.Parse(report.TotalExpense, CultureInfo.InvariantCulture);
        }

        return (income, expense);
    }

    private static async Task<AdvancedFinancialReportResponse> AdvancedAsync(
        HttpClient client,
        DateOnly today)
    {
        return (await client.GetFromJsonAsync<AdvancedFinancialReportResponse>(
            $"/api/v1/reports/advanced?year={today.Year}&month={today.Month}"
            + $"&asOfDate={Date(today)}&trendMonths=2&daysAhead=30"))!;
    }

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type,
        string openingBalance)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", openingBalance));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AccountResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register", new RegisterRequest(email, Password));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
