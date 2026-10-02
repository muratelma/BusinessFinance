using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;

namespace BusinessFinance.Api.Tests.Features.Pos;

public sealed class PosSettlementEndpointTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>
    /// ADR 0015'in iki anı tek testte: tahsilat günü satışı tanır ve hesabı
    /// kıpırdatmaz, yatış günü hesabı artırır ve <b>hiçbir gelir/gider
    /// yazmaz</b>. İkincisi yazsaydı aynı satış iki kez sayılırdı. Tahsilat
    /// hesaba yalnız bir yatışla geçer (ADR 0019 T5).
    /// </summary>
    [Fact]
    public async Task PosSale_RecognizesOnTheSaleDay_AndOnlyMovesCashWhenItIsDeposited()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "pos-stranger@example.test");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank", "1000.0000");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                income.Id,
                "1000.0000",
                "TRY",
                Date(today),
                Date(today.AddDays(2)),
                CommissionRate: "0.0150",
                CommissionCategoryId: expense.Id,
                Scope: "business"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var settlement = (await create.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        Assert.Equal("1000.0000", settlement.GrossAmount);
        Assert.Equal("15.0000", settlement.CommissionAmount);
        Assert.Equal("985.0000", settlement.NetAmount);
        Assert.True(settlement.IsInTransit);
        Assert.False(settlement.IsLate);

        // Para yolda: kullanılabilir bakiye kıpırdamadı.
        var accountBefore = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1000.0000", accountBefore!.Balance);

        var listBefore = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Single(listBefore!.Items);
        Assert.Equal("985.0000", listBefore.MoneyInTransit);
        Assert.Equal(1, listBefore.InTransitCount);

        // Net varlık yoldaki parayı içerir; ikisinin farkı tam olarak o tutar.
        var advancedBefore = await AdvancedAsync(owner, today);
        Assert.Equal("985.0000", advancedBefore.NetWorth.MoneyInTransit);
        Assert.Equal("1000.0000", advancedBefore.NetWorth.LiquidAssets);
        Assert.Equal("1985.0000", advancedBefore.NetWorth.NetWorth);

        // Satış tahsil edildiği gün tanınır: brüt gelir, komisyon ayrı gider.
        var monthlyBefore = await MonthlyAsync(owner, today);
        Assert.Equal("1000.0000", monthlyBefore.TotalIncome);
        Assert.Equal("15.0000", monthlyBefore.TotalExpense);

        var depositRequest = new CreatePosDepositRequest(
            Guid.NewGuid(), [settlement.Id], "985.0000", Date(today));
        using var deposit = await owner.PostAsJsonAsync("/api/v1/pos-deposits", depositRequest);
        Assert.Equal(HttpStatusCode.Created, deposit.StatusCode);
        var deposited = (await deposit.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.Equal("0.0000", deposited.DeductionAmount);
        Assert.Null(deposited.DeductionTransactionId);
        var transferred = Assert.Single(deposited.Settlements);
        Assert.False(transferred.IsInTransit);
        Assert.Equal(Date(today), transferred.TransferredOn);
        Assert.Equal(deposited.Id, transferred.PosDepositId);

        var accountAfter = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1985.0000", accountAfter!.Balance);

        // Yatış günü rapora hiçbir şey eklemedi.
        var monthlyAfter = await MonthlyAsync(owner, today);
        Assert.Equal("1000.0000", monthlyAfter.TotalIncome);
        Assert.Equal("15.0000", monthlyAfter.TotalExpense);

        var advancedAfter = await AdvancedAsync(owner, today);
        Assert.Equal("0.0000", advancedAfter.NetWorth.MoneyInTransit);
        Assert.Equal("1985.0000", advancedAfter.NetWorth.LiquidAssets);
        Assert.Equal("1985.0000", advancedAfter.NetWorth.NetWorth);

        // Aynı isteği tekrarlamak ikinci bir bakiye etkisi üretmez.
        using var depositAgain = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits", depositRequest);
        Assert.Equal(HttpStatusCode.Created, depositAgain.StatusCode);
        var accountAgain = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1985.0000", accountAgain!.Balance);

        // Yabancı kullanıcı ne okuyabilir ne yazabilir.
        var strangerList = await stranger.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements");
        Assert.Empty(strangerList!.Items);
        Assert.Equal("0.0000", strangerList.MoneyInTransit);
        using var foreignRevert = await stranger.DeleteAsync(
            $"/api/v1/pos-deposits/{deposited.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignRevert.StatusCode);
        using var foreignCreate = await stranger.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                income.Id,
                "10.0000",
                "TRY",
                Date(today),
                Date(today),
                Scope: "business"));
        Assert.Equal(HttpStatusCode.NotFound, foreignCreate.StatusCode);
    }

    /// <summary>
    /// 28 Eylül denetimi U12: yanlışlıkla "hesaba geçti" denen tahsilat ve
    /// yanlış girilen POS kaydı düzeltilemiyordu. Yatışı geri almak yalnız
    /// hesaptaki parayı geri çeker; iptal satışı, komisyonu ve yoldaki parayı
    /// birlikte kaldırır. Yatışa bağlı tahsilat önce yatış geri alınmadan
    /// iptal edilemez (ADR 0019 T5).
    /// </summary>
    [Fact]
    public async Task WrongDepositCanBeReverted_AndAWrongSaleCanBeCancelledOnlyAfterwards()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-undo-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "pos-undo-stranger@example.test");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank", "1000.0000");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");

        using var create = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                income.Id,
                "1000.0000",
                "TRY",
                Date(today),
                Date(today.AddDays(2)),
                CommissionRate: "0.0150",
                CommissionCategoryId: expense.Id,
                Scope: "business"));
        var settlement = (await create.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        using var deposit = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [settlement.Id], "985.0000", Date(today)));
        Assert.Equal(HttpStatusCode.Created, deposit.StatusCode);
        var deposited = (await deposit.Content.ReadFromJsonAsync<PosDepositResponse>())!;

        // Yabancı kullanıcı geri alamaz ve iptal edemez; cevap var olmayan
        // kayıtla aynıdır.
        using var foreignRevert = await stranger.DeleteAsync(
            $"/api/v1/pos-deposits/{deposited.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignRevert.StatusCode);
        using var foreignCancel = await stranger.DeleteAsync(
            $"/api/v1/pos-settlements/{settlement.Id}");
        Assert.Equal(HttpStatusCode.NotFound, foreignCancel.StatusCode);

        // Yatışa bağlı tahsilat iptal edilemez: yatış o tahsilatın netini
        // hesaba taşıdı. Cevap kendi kodunu taşır ki istemci "önce yatışı
        // geri alın" diyebilsin; hiçbir şey değişmez.
        using var lockedCancel = await owner.DeleteAsync(
            $"/api/v1/pos-settlements/{settlement.Id}");
        Assert.Equal(HttpStatusCode.Conflict, lockedCancel.StatusCode);
        var lockedProblem = await lockedCancel.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "pos_settlements.deposit_locked", lockedProblem!["code"]!.GetValue<string>());
        var accountWhileLocked = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1985.0000", accountWhileLocked!.Balance);

        // Yatış geri alındı: para yeniden yolda, hesap bakiyesi eski hâlinde,
        // satış ve komisyon tanınmış olarak kalır.
        using var revert = await owner.DeleteAsync($"/api/v1/pos-deposits/{deposited.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        var revertedDeposit = (await revert.Content.ReadFromJsonAsync<PosDepositResponse>())!;
        Assert.True(revertedDeposit.IsCancelled);
        Assert.Empty(revertedDeposit.Settlements);
        var listAfterRevert = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        var reverted = Assert.Single(listAfterRevert!.Items);
        Assert.True(reverted.IsInTransit);
        Assert.Null(reverted.TransferredOn);
        Assert.Null(reverted.PosDepositId);
        var accountAfterRevert = await owner.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{account.Id}");
        Assert.Equal("1000.0000", accountAfterRevert!.Balance);
        var advancedAfterRevert = await AdvancedAsync(owner, today);
        Assert.Equal("985.0000", advancedAfterRevert.NetWorth.MoneyInTransit);
        var monthlyAfterRevert = await MonthlyAsync(owner, today);
        Assert.Equal("1000.0000", monthlyAfterRevert.TotalIncome);
        Assert.Equal("15.0000", monthlyAfterRevert.TotalExpense);

        // Geri almak idempotenttir.
        using var revertAgain = await owner.DeleteAsync(
            $"/api/v1/pos-deposits/{deposited.Id}");
        Assert.Equal(HttpStatusCode.OK, revertAgain.StatusCode);

        // İptal: satış, komisyon ve yoldaki para birlikte düşer.
        using var cancel = await owner.DeleteAsync($"/api/v1/pos-settlements/{settlement.Id}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        var cancelled = (await cancel.Content.ReadFromJsonAsync<PosSettlementResponse>())!;
        Assert.True(cancelled.IsCancelled);
        Assert.False(cancelled.IsInTransit);
        var monthlyAfterCancel = await MonthlyAsync(owner, today);
        Assert.Equal("0.0000", monthlyAfterCancel.TotalIncome);
        Assert.Equal("0.0000", monthlyAfterCancel.TotalExpense);
        var advancedAfterCancel = await AdvancedAsync(owner, today);
        Assert.Equal("0.0000", advancedAfterCancel.NetWorth.MoneyInTransit);

        // İptal idempotenttir; iptal edilmiş tahsilat bir yatışla kapatılamaz.
        using var cancelAgain = await owner.DeleteAsync(
            $"/api/v1/pos-settlements/{settlement.Id}");
        Assert.Equal(HttpStatusCode.OK, cancelAgain.StatusCode);
        using var depositCancelled = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [settlement.Id], "985.0000", Date(today)));
        Assert.Equal(HttpStatusCode.Conflict, depositCancelled.StatusCode);
        var cancelledProblem = await depositCancelled.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal(
            "pos_deposits.settlement_not_in_transit",
            cancelledProblem!["code"]!.GetValue<string>());
    }

    [Fact]
    public async Task Create_RejectsCommissionSentAsBothAmountAndRate()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-commission@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik banka", "bank", "0.0000");
        var income = await FirstCategoryAsync(owner, "income");
        var expense = await FirstCategoryAsync(owner, "expense");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                income.Id,
                "100.0000",
                "TRY",
                Date(today),
                Date(today),
                CommissionAmount: "1.5000",
                CommissionRate: "0.0150",
                CommissionCategoryId: expense.Id,
                Scope: "business"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Kasa POS parası alamaz: para bankaya geçer, tezgâhın çekmecesine değil.
    /// </summary>
    [Fact]
    public async Task Create_RejectsACashAccountAsTheDestination()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "pos-cash-account@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var account = await CreateAccountAsync(owner, "Sentetik kasa", "cash", "0.0000");
        var income = await FirstCategoryAsync(owner, "income");

        using var response = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                account.Id,
                income.Id,
                "100.0000",
                "TRY",
                Date(today),
                Date(today),
                Scope: "business"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static string Date(DateOnly value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static async Task<MonthlyReportResponse> MonthlyAsync(
        HttpClient client,
        DateOnly today)
    {
        return (await client.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={today.Year}&month={today.Month}"))!;
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
