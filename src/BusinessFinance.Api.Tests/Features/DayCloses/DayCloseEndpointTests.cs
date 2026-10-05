using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.DayCloses;
using BusinessFinance.Api.Features.FinancialActivities;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.DayCloses;

/// <summary>
/// Gün sonu (ADR 0019 T1–T2): nakit satış kasaya bir gelir, kartlı satış bir
/// POS tahsilatı olur; o gün zaten girilmiş kayıtlar düşülür ve gün sonu bir
/// bütün olarak geri alınır.
/// </summary>
public sealed class DayCloseEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Path = "/api/v1/day-closes";

    /// <summary>
    /// Gün sonunun bütün etkisi tek testte: Z'deki tutarlar olduğu gibi
    /// yazılır, tek tek girilmiş satışlar düşülür ve günün geliri tam olarak
    /// Z'nin toplamı kadar olur — aynı satış iki kez sayılmaz (İ2).
    /// </summary>
    [Fact]
    public async Task DayClose_WritesTheRemainder_AfterDeductingRecordsAlreadyEntered()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-owner@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);

        // Gün içinde tek tek girilmişler: 1250 nakit satış, 800 kartlı satış
        // ve 300 nakit cari tahsilat (yazar kasadan geçmez).
        var sale = await CreateIncomeAsync(owner, f.TillId, f.SalesCategoryId, "1250.0000", today);
        var posSale = await CreateSettlementAsync(owner, f, "800.0000", today);
        var collection = await CreateCollectionAsync(owner, f.TillId, "300.0000", today);

        var request = new DayCloseRequest(
            Date(today),
            CashAmount: "3350.0000",
            PosAmounts: [new(f.PosDefinitionId, "2680.0000")]);
        var preview = await PreviewAsync(owner, request);

        Assert.Null(preview.BlockerCode);
        Assert.Empty(preview.ClosedBy);
        Assert.True(preview.Cash.Stated);
        Assert.False(preview.Cash.IsComputed);
        Assert.Equal("1250.0000", preview.Cash.DeductedAmount);
        Assert.Equal("2100.0000", preview.Cash.AmountToWrite);
        // Kasa ve satış kategorisi seçili gelir: tek nakit hesap ve ana
        // POS'un satış kategorisi.
        Assert.Equal(f.TillId, preview.Cash.AccountId);
        Assert.Equal(f.SalesCategoryId, preview.Cash.CategoryId);
        var line = Assert.Single(preview.PosLines);
        Assert.Equal(f.PosDefinitionId, line.PosDefinitionId);
        Assert.True(line.IsDefault);
        Assert.Equal("800.0000", line.DeductedAmount);
        Assert.Equal("1880.0000", line.AmountToWrite);
        Assert.Equal("37.6000", line.CommissionAmount);
        Assert.Equal("1842.4000", line.NetAmount);
        Assert.Equal(Date(today.AddDays(1)), line.ExpectedTransferDate);
        Assert.Equal("6030.0000", preview.TotalComputed);
        Assert.Null(preview.TotalDifference);

        // Satışlar işaretli, nakit cari tahsilat işaretsiz gelir.
        Assert.Equal(3, preview.ExistingRecords.Count);
        var existingSale = Assert.Single(preview.ExistingRecords, r => r.Id == sale.Id);
        Assert.Equal(("income", "cash", true, true),
            (existingSale.Kind, existingSale.Side, existingSale.IncludedByDefault, existingSale.Included));
        var existingPos = Assert.Single(preview.ExistingRecords, r => r.Id == posSale.Id);
        Assert.Equal(("pos-settlement", "card", true),
            (existingPos.Kind, existingPos.Side, existingPos.Included));
        Assert.Equal(f.PosDefinitionId, existingPos.PosDefinitionId);
        var existingCollection = Assert.Single(preview.ExistingRecords, r => r.Id == collection);
        Assert.Equal(("counterparty-payment", "cash", false, false),
            (existingCollection.Kind, existingCollection.Side,
                existingCollection.IncludedByDefault, existingCollection.Included));

        // Önizleme hiçbir şey yazmaz.
        Assert.Equal("1550.0000", await BalanceAsync(owner, f.TillId));

        var close = await CloseAsync(owner, request);
        Assert.Equal(Date(today), close.ClosedOn);
        Assert.False(close.IsAdditional);
        Assert.False(close.IsCancelled);
        Assert.Null(close.ZNumber);
        var income = Assert.Single(close.Incomes);
        Assert.Equal("2100.0000", income.Amount);
        Assert.Equal(f.TillId, income.AccountId);
        Assert.Equal("business", income.Scope);
        var settlement = Assert.Single(close.Settlements);
        Assert.Equal("1880.0000", settlement.GrossAmount);
        Assert.Equal("37.6000", settlement.CommissionAmount);
        Assert.Equal(close.Id, settlement.DayCloseId);
        Assert.Equal(f.PosDefinitionId, settlement.PosDefinitionId);
        Assert.True(settlement.IsInTransit);
        Assert.Equal("2100.0000", close.CashAmount);
        Assert.Equal("1880.0000", close.CardGrossAmount);
        Assert.Equal("37.6000", close.CommissionAmount);

        // Nakit kasaya girdi; kart parası yolda, bankaya dokunmadı.
        Assert.Equal("3650.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("0.0000", await BalanceAsync(owner, f.BankId));

        // Günün geliri tam olarak Z'nin toplamı: 3350 + 2680.
        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={today.Year}&month={today.Month}");
        Assert.Equal("6030.0000", report!.TotalIncome);

        // İşlemler: gün sonu ayrı bir satır değildir (İ3); ürettiği gelir ve
        // POS satışı kendi satırlarıdır ve tek başına iptal edilemez.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        var produced = feed!.Items.Where(item => item.Origin == "day-close").ToArray();
        Assert.Equal(
            [income.TransactionId, settlement.Id],
            produced.Select(item => item.ActivityId).OrderBy(id => id == settlement.Id));
        Assert.All(produced, item => Assert.False(item.CanCancel));
        Assert.Equal(5, feed.Items.Count);

        using var cancelIncome = await owner.DeleteAsync(
            $"/api/v1/transactions/{income.TransactionId}");
        Assert.Equal(HttpStatusCode.Conflict, cancelIncome.StatusCode);
        Assert.Equal("transactions.cancel_origin_locked", await CodeAsync(cancelIncome));
        using var cancelSettlement = await owner.DeleteAsync(
            $"/api/v1/pos-settlements/{settlement.Id}");
        Assert.Equal(HttpStatusCode.Conflict, cancelSettlement.StatusCode);
        Assert.Equal("pos_settlements.day_close_locked", await CodeAsync(cancelSettlement));

        // Gün sonu düştüğü kayıtları sahiplenir: satış ve POS tahsilatı
        // sayıldı, işaretsiz cari tahsilat sayılmadı.
        Assert.Equal(
            new[] { sale.Id, posSale.Id }.Order(),
            close.CountedRecords.Select(record => record.Id).Order());
        Assert.Equal("1250.0000", close.CountedCashAmount);
        Assert.Equal("800.0000", close.CountedCardAmount);

        // Sayılan kayıt tek başına iptal edilemez: iptal edilseydi günün
        // geliri sessizce eksilirdi.
        using var cancelCounted = await owner.DeleteAsync($"/api/v1/transactions/{sale.Id}");
        Assert.Equal(HttpStatusCode.Conflict, cancelCounted.StatusCode);
        Assert.Equal("transactions.day_close_counted", await CodeAsync(cancelCounted));
        using var cancelCountedPos = await owner.DeleteAsync(
            $"/api/v1/pos-settlements/{posSale.Id}");
        Assert.Equal(HttpStatusCode.Conflict, cancelCountedPos.StatusCode);
        Assert.Equal("pos_settlements.day_close_counted", await CodeAsync(cancelCountedPos));

        // Tahsilat listesi de aynı bağı taşır: istemci iptali sunmadan önce
        // bilir. Gün sonunun yazdığı tahsilat sayılmış değildir, üretilmiştir.
        var settlements = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            $"/api/v1/pos-settlements?from={Date(today)}&to={Date(today)}");
        var countedPos = Assert.Single(settlements!.Items, item => item.Id == posSale.Id);
        Assert.Equal(close.Id, countedPos.CountedInDayCloseId);
        Assert.Null(countedPos.DayCloseId);
        var producedPos = Assert.Single(settlements.Items, item => item.Id == settlement.Id);
        Assert.Null(producedPos.CountedInDayCloseId);
        Assert.Equal(close.Id, producedPos.DayCloseId);

        // Akış satırı gün sonunun kimliğini taşır: yazdığı kayıtta da saydığı
        // kayıtta da. Sayılmayan kayıt bağsızdır ve iptal edilebilir.
        var saleRow = Assert.Single(feed.Items, item => item.ActivityId == sale.Id);
        Assert.Equal(close.Id, saleRow.DayCloseId);
        Assert.Equal("manual", saleRow.Origin);
        Assert.False(saleRow.CanCancel);
        Assert.All(produced, item => Assert.Equal(close.Id, item.DayCloseId));
        var collectionRow = Assert.Single(feed.Items, item => item.ActivityId == collection);
        Assert.Null(collectionRow.DayCloseId);
        Assert.True(collectionRow.CanCancel);

        // Günün bütünü: toplam, yazılan ile sayılanın toplamıdır — yani
        // kullanıcının yazdığı tutar. Sayılmayan kayıt dışarıda durur.
        var day = await owner.GetFromJsonAsync<DayCloseDayResponse>(
            $"{Path}/day?date={Date(today)}");
        Assert.True(day!.IsClosed);
        Assert.Equal("3350.0000", day.CashTotal);
        Assert.Equal("2680.0000", day.CardTotal);
        Assert.Equal(close.Id, Assert.Single(day.Closes).Id);
        Assert.Equal(collection, Assert.Single(day.OutsideRecords).Id);

        // Okuma ve liste aynı kaydı döner.
        var read = await owner.GetFromJsonAsync<DayCloseResponse>($"{Path}/{close.Id}");
        Assert.Equal("2100.0000", read!.CashAmount);
        var list = await owner.GetFromJsonAsync<DayCloseListResponse>(
            $"{Path}?from={Date(today)}&to={Date(today)}");
        Assert.Equal(close.Id, Assert.Single(list!.Items).Id);
    }

    /// <summary>
    /// Nakit, kart ve toplamdan ikisi yeter; toplamdan hesaplanan kart ana
    /// POS'a yazılır. Üçü de verilip tutmuyorsa fark gösterilir, kayıt
    /// engellenmez: toplam kayıt üretmez, açıklar (T3).
    /// </summary>
    [Fact]
    public async Task TwoOfCashCardAndTotal_AreEnough()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-amounts@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);

        var cardFromTotal = await PreviewAsync(
            owner, new DayCloseRequest(Date(today), CashAmount: "1000.0000", TotalAmount: "1500.0000"));
        var computedLine = Assert.Single(cardFromTotal.PosLines);
        Assert.True(computedLine.Stated);
        Assert.True(computedLine.IsComputed);
        Assert.Equal("500.0000", computedLine.EnteredAmount);
        Assert.Equal("1500.0000", cardFromTotal.TotalComputed);
        Assert.Null(cardFromTotal.TotalDifference);
        Assert.Null(cardFromTotal.BlockerCode);

        var cashFromTotal = await PreviewAsync(
            owner,
            new DayCloseRequest(
                Date(today), PosAmounts: [new(f.PosDefinitionId, "400.0000")],
                TotalAmount: "1500.0000"));
        Assert.True(cashFromTotal.Cash.Stated);
        Assert.True(cashFromTotal.Cash.IsComputed);
        Assert.Equal("1100.0000", cashFromTotal.Cash.EnteredAmount);

        // Yalnız nakit: kart tarafına dokunulmaz.
        var cashOnly = await PreviewAsync(
            owner, new DayCloseRequest(Date(today), CashAmount: "700.0000"));
        Assert.False(Assert.Single(cashOnly.PosLines).Stated);
        Assert.Null(cashOnly.BlockerCode);

        // Yalnız toplam yetmez; panel boş açıldığında da durum budur.
        var totalOnly = await PreviewAsync(
            owner, new DayCloseRequest(Date(today), TotalAmount: "1500.0000"));
        Assert.Equal("day_closes.amounts_required", totalOnly.BlockerCode);
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.amounts_required",
            new DayCloseRequest(Date(today), TotalAmount: "1500.0000", ClientRequestId: Guid.NewGuid()));

        var below = await PreviewAsync(
            owner, new DayCloseRequest(Date(today), CashAmount: "1000.0000", TotalAmount: "900.0000"));
        Assert.Equal("day_closes.total_below_parts", below.BlockerCode);

        // Üçü de verildi ve tutmuyor: fark gösterilir, kayıt nakit ve karttan
        // yazılır.
        var mismatch = new DayCloseRequest(
            Date(today),
            CashAmount: "1000.0000",
            PosAmounts: [new(f.PosDefinitionId, "400.0000")],
            TotalAmount: "1850.0000");
        var mismatchPreview = await PreviewAsync(owner, mismatch);
        Assert.Equal("450.0000", mismatchPreview.TotalDifference);
        Assert.Null(mismatchPreview.BlockerCode);
        var close = await CloseAsync(owner, mismatch);
        Assert.Equal("1000.0000", close.CashAmount);
        Assert.Equal("400.0000", close.CardGrossAmount);
    }

    /// <summary>
    /// Her satış tek tek girildiyse yazılacak tutar kalmaz; gün yine de
    /// kapanır ve ikinci bir gün sonu ancak açıkça "ek" olarak yazılır.
    /// </summary>
    [Fact]
    public async Task ADayWithEverythingAlreadyEntered_ClosesWithoutProducingRecords()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-empty@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        await CreateIncomeAsync(owner, f.TillId, f.SalesCategoryId, "1250.0000", today);

        var request = new DayCloseRequest(Date(today), CashAmount: "1250.0000");
        var close = await CloseAsync(owner, request);
        Assert.Empty(close.Incomes);
        Assert.Empty(close.Settlements);
        Assert.Equal("0.0000", close.CashAmount);
        Assert.Equal("1250.0000", await BalanceAsync(owner, f.TillId));

        // Gün kapalı: aynı güne ikinci gün sonu reddedilir.
        var again = await PreviewAsync(owner, request);
        Assert.Equal("day_closes.already_closed", again.BlockerCode);
        Assert.Equal(close.Id, Assert.Single(again.ClosedBy).Id);
        await AssertRejectedAsync(
            owner, HttpStatusCode.Conflict, "day_closes.already_closed",
            request with { ClientRequestId = Guid.NewGuid() });

        // İkinci cihazın gün sonu açıkça "ek" olarak yazılır; tek tek girilmiş
        // kayıtlar bu kez işaretsiz gelir (ilk gün sonunda düşüldüler).
        var additional = new DayCloseRequest(Date(today), CashAmount: "400.0000", IsAdditional: true);
        var additionalPreview = await PreviewAsync(owner, additional);
        Assert.Null(additionalPreview.BlockerCode);
        // İlk gün sonunun saydığı satış yeniden listelenmez: ikinci kez
        // düşülemez.
        Assert.Empty(additionalPreview.ExistingRecords);
        Assert.Equal("400.0000", additionalPreview.Cash.AmountToWrite);
        var second = await CloseAsync(owner, additional);
        Assert.True(second.IsAdditional);
        Assert.Equal("1650.0000", await BalanceAsync(owner, f.TillId));

        // Kapatılmamış bir güne "ek" gün sonu yazılamaz.
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.not_closed_yet",
            new DayCloseRequest(
                Date(today.AddDays(-1)), CashAmount: "100.0000", IsAdditional: true,
                ClientRequestId: Guid.NewGuid()));
    }

    /// <summary>
    /// İşaretli kayıtlar gün sonu tutarını aşarsa kayıt reddedilir; kullanıcı
    /// işareti kaldırınca (kayıt gün sonunun içinde değilmiş) yazılır.
    /// </summary>
    [Fact]
    public async Task RecordsExceedingTheDayClose_AreRejectedUntilUnchecked()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-exceed@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var sale = await CreateIncomeAsync(owner, f.TillId, f.SalesCategoryId, "1250.0000", today);
        var posSale = await CreateSettlementAsync(owner, f, "800.0000", today);
        var collection = await CreateCollectionAsync(owner, f.TillId, "300.0000", today);

        var request = new DayCloseRequest(
            Date(today), CashAmount: "1000.0000", PosAmounts: [new(f.PosDefinitionId, "500.0000")]);
        var preview = await PreviewAsync(owner, request);
        Assert.Equal("day_closes.existing_exceeds_cash", preview.BlockerCode);
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.existing_exceeds_cash",
            request with { ClientRequestId = Guid.NewGuid() });

        var cardOnly = request with
        {
            RecordOverrides = [new("income", sale.Id, false)],
        };
        Assert.Equal(
            "day_closes.existing_exceeds_card", (await PreviewAsync(owner, cardOnly)).BlockerCode);

        // Satış ve POS tahsilatı gün sonunun içinde değil; cari tahsilat ise
        // yazar kasadan geçmiş.
        var corrected = request with
        {
            RecordOverrides =
            [
                new("income", sale.Id, false),
                new("pos-settlement", posSale.Id, false),
                new("counterparty-payment", collection, true),
            ],
        };
        var correctedPreview = await PreviewAsync(owner, corrected);
        Assert.Null(correctedPreview.BlockerCode);
        Assert.Equal("300.0000", correctedPreview.Cash.DeductedAmount);
        Assert.Equal("700.0000", correctedPreview.Cash.AmountToWrite);
        var close = await CloseAsync(owner, corrected);
        Assert.Equal("700.0000", close.CashAmount);
        Assert.Equal("500.0000", close.CardGrossAmount);

        // Reddedilen istekler hiçbir şey yazmadı: 1250 + 300 + 700.
        Assert.Equal("2250.0000", await BalanceAsync(owner, f.TillId));
    }

    /// <summary>
    /// Geri alma bir bütündür: ürettiği kayıtlar birlikte iptal olur, gün
    /// yeniden açılır ve gün sonu kaydı kalır. Ürettiği tahsilat hesaba
    /// geçtiyse önce yatış geri alınır.
    /// </summary>
    [Fact]
    public async Task RevertedDayClose_CancelsItsRecordsAndReopensTheDay()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-revert@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var request = new DayCloseRequest(
            Date(today), CashAmount: "2000.0000", PosAmounts: [new(f.PosDefinitionId, "1000.0000")]);
        var close = await CloseAsync(owner, request);
        var settlement = Assert.Single(close.Settlements);

        // Para hesaba geçtiyse gün sonu geri alınamaz.
        using var deposit = await owner.PostAsJsonAsync(
            "/api/v1/pos-deposits",
            new CreatePosDepositRequest(Guid.NewGuid(), [settlement.Id], "980.0000", Date(today)));
        Assert.Equal(HttpStatusCode.Created, deposit.StatusCode);
        var depositId = (await deposit.Content.ReadFromJsonAsync<PosDepositResponse>())!.Id;
        using var locked = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.Conflict, locked.StatusCode);
        Assert.Equal("day_closes.deposit_locked", await CodeAsync(locked));
        Assert.Equal("2000.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("980.0000", await BalanceAsync(owner, f.BankId));

        using var revertDeposit = await owner.DeleteAsync($"/api/v1/pos-deposits/{depositId}");
        Assert.Equal(HttpStatusCode.OK, revertDeposit.StatusCode);

        using var revert = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        var reverted = (await revert.Content.ReadFromJsonAsync<DayCloseResponse>())!;
        Assert.True(reverted.IsCancelled);
        Assert.NotNull(reverted.CancelledAtUtc);
        // Kayıt ne yazdığını hatırlar; kayıtları iptal edilmiştir.
        Assert.True(Assert.Single(reverted.Incomes).IsCancelled);
        Assert.True(Assert.Single(reverted.Settlements).IsCancelled);
        Assert.Equal("2000.0000", reverted.CashAmount);

        Assert.Equal("0.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("0.0000", await BalanceAsync(owner, f.BankId));
        var report = await owner.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={today.Year}&month={today.Month}");
        Assert.Equal("0.0000", report!.TotalIncome);
        Assert.Equal("0.0000", report.TotalExpense);
        var transit = await owner.GetFromJsonAsync<PosSettlementListResponse>(
            "/api/v1/pos-settlements?inTransitOnly=true");
        Assert.Equal(0, transit!.InTransitCount);

        // Geri alma idempotenttir.
        using var again = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.OK, again.StatusCode);

        // Gün yeniden açıldı: liste boş, yeni gün sonu yazılabilir.
        var list = await owner.GetFromJsonAsync<DayCloseListResponse>(
            $"{Path}?from={Date(today)}&to={Date(today)}");
        Assert.Empty(list!.Items);
        var corrected = await CloseAsync(owner, new DayCloseRequest(Date(today), CashAmount: "1900.0000"));
        Assert.NotEqual(close.Id, corrected.Id);
        Assert.Equal("1900.0000", await BalanceAsync(owner, f.TillId));
    }

    /// <summary>
    /// Sayılan kayıt ikinci kez sayılamaz; gün sonu geri alınınca serbest
    /// kalır. Ana gün sonu, ekleri geri alınmadan geri alınamaz.
    /// </summary>
    [Fact]
    public async Task CountedRecords_AreOwnedUntilTheDayCloseIsReverted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-counted@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var sale = await CreateIncomeAsync(owner, f.TillId, f.SalesCategoryId, "1250.0000", today);
        var collection = await CreateCollectionAsync(owner, f.TillId, "300.0000", today);

        // Cari tahsilat yazar kasadan geçmiş: kullanıcı işaretler.
        var main = await CloseAsync(
            owner,
            new DayCloseRequest(
                Date(today), CashAmount: "3350.0000",
                RecordOverrides: [new("counterparty-payment", collection, true)]));
        Assert.Equal("1800.0000", main.CashAmount);
        Assert.Equal("1550.0000", main.CountedCashAmount);

        // Sayılan cari tahsilat da tek başına iptal edilemez.
        using var cancelCollection = await owner.DeleteAsync(
            $"/api/v1/counterparty-payments/{collection}");
        Assert.Equal(HttpStatusCode.Conflict, cancelCollection.StatusCode);
        Assert.Equal("counterparty_payments.day_close_counted", await CodeAsync(cancelCollection));

        // Ek gün sonunda sayılmış kayıtlar listede yok; override ile de
        // yeniden sayılamazlar.
        var additional = await CloseAsync(
            owner,
            new DayCloseRequest(
                Date(today), CashAmount: "400.0000", IsAdditional: true,
                RecordOverrides: [new("income", sale.Id, true)]));
        Assert.Equal("400.0000", additional.CashAmount);
        Assert.Empty(additional.CountedRecords);

        var day = await owner.GetFromJsonAsync<DayCloseDayResponse>(
            $"{Path}/day?date={Date(today)}");
        Assert.Equal([main.Id, additional.Id], day!.Closes.Select(item => item.Id));
        Assert.Equal("3750.0000", day.CashTotal);
        Assert.Empty(day.OutsideRecords);

        // Ek duruyorken ana gün sonu geri alınamaz.
        using var blocked = await owner.DeleteAsync($"{Path}/{main.Id}");
        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        Assert.Equal("day_closes.additional_exists", await CodeAsync(blocked));

        using var revertAdditional = await owner.DeleteAsync($"{Path}/{additional.Id}");
        Assert.Equal(HttpStatusCode.OK, revertAdditional.StatusCode);
        using var revertMain = await owner.DeleteAsync($"{Path}/{main.Id}");
        Assert.Equal(HttpStatusCode.OK, revertMain.StatusCode);
        var reverted = (await revertMain.Content.ReadFromJsonAsync<DayCloseResponse>())!;
        Assert.Empty(reverted.CountedRecords);

        // Bağ kalktı: kayıtlar yeniden listede ve tek başına iptal edilebilir.
        var preview = await PreviewAsync(
            owner, new DayCloseRequest(Date(today), CashAmount: "3350.0000"));
        Assert.Equal(2, preview.ExistingRecords.Count);
        Assert.Equal("2100.0000", preview.Cash.AmountToWrite);
        using var cancelSale = await owner.DeleteAsync($"/api/v1/transactions/{sale.Id}");
        Assert.Equal(HttpStatusCode.OK, cancelSale.StatusCode);
        Assert.Equal("300.0000", await BalanceAsync(owner, f.TillId));
    }

    /// <summary>Aynı istek kimliği ikinci bir gün sonu ya da kayıt yazmaz.</summary>
    [Fact]
    public async Task DayClose_IsIdempotentPerClientRequest()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-idempotent@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var request = new DayCloseRequest(
            Date(today), CashAmount: "500.0000", ClientRequestId: Guid.NewGuid());

        using var first = await owner.PostAsJsonAsync(Path, request);
        using var second = await owner.PostAsJsonAsync(Path, request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var firstClose = (await first.Content.ReadFromJsonAsync<DayCloseResponse>())!;
        var secondClose = (await second.Content.ReadFromJsonAsync<DayCloseResponse>())!;
        Assert.Equal(firstClose.Id, secondClose.Id);
        Assert.Equal("500.0000", await BalanceAsync(owner, f.TillId));

        // İstek kimliği olmadan kayıt yazılmaz.
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.validation",
            new DayCloseRequest(Date(today.AddDays(-1)), CashAmount: "500.0000"));
    }

    /// <summary>
    /// Sunucu Z numarasını ve birkaç günlük Z'nin aralığını kabul eder
    /// (arayüzü Z okumayla gelir): aynı Z ikinci kez yazılamaz ve aralıktaki
    /// bütün günler kapalı sayılır.
    /// </summary>
    [Fact]
    public async Task ZNumberAndRange_GuardAgainstEnteringTheSameReportTwice()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "close-z@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);

        var close = await CloseAsync(
            owner,
            new DayCloseRequest(
                Date(today), CashAmount: "900.0000",
                RangeStart: Date(today.AddDays(-2)), ZNumber: 3143));
        Assert.Equal(3143, close.ZNumber);
        Assert.Equal(Date(today.AddDays(-2)), close.RangeStart);
        // Toplam aralığın son gününe yazılır.
        Assert.Equal(Date(today), Assert.Single(close.Incomes).Date);

        // Aralığın ortasındaki gün kapalıdır.
        await AssertRejectedAsync(
            owner, HttpStatusCode.Conflict, "day_closes.already_closed",
            new DayCloseRequest(
                Date(today.AddDays(-1)), CashAmount: "100.0000", ClientRequestId: Guid.NewGuid()));
        var middle = await owner.GetFromJsonAsync<DayCloseListResponse>(
            $"{Path}?from={Date(today.AddDays(-1))}&to={Date(today.AddDays(-1))}");
        Assert.Equal(close.Id, Assert.Single(middle!.Items).Id);

        // Aynı Z numarası başka bir güne de yazılamaz.
        await AssertRejectedAsync(
            owner, HttpStatusCode.Conflict, "day_closes.z_number_exists",
            new DayCloseRequest(
                Date(today.AddDays(-5)), CashAmount: "100.0000", ZNumber: 3143,
                ClientRequestId: Guid.NewGuid()));

        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.invalid_date",
            new DayCloseRequest(
                Date(today.AddDays(1)), CashAmount: "100.0000", ClientRequestId: Guid.NewGuid()));
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.invalid_date",
            new DayCloseRequest(
                Date(today.AddDays(-9)), CashAmount: "100.0000", RangeStart: Date(today.AddDays(-9)),
                ClientRequestId: Guid.NewGuid()));
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.invalid_z_number",
            new DayCloseRequest(
                Date(today.AddDays(-9)), CashAmount: "100.0000", ZNumber: 0,
                ClientRequestId: Guid.NewGuid()));
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.invalid_amount",
            new DayCloseRequest(
                Date(today.AddDays(-9)), CashAmount: "-1.0000", ClientRequestId: Guid.NewGuid()));

        Assert.Equal("900.0000", await BalanceAsync(owner, f.TillId));
    }

    /// <summary>
    /// Başkasının gün sonu, POS'u ve kasası hiç var olmamış bir kayıtla aynı
    /// cevabı verir; yabancı kullanıcı hiçbir şey okuyamaz ve değiştiremez.
    /// </summary>
    [Fact]
    public async Task AnotherUsersDayCloses_AnswerLikeMissingOnes()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(
            factory, "close-isolation-owner@example.test");
        using var stranger = await CreateAuthenticatedClientAsync(
            factory, "close-isolation-stranger@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var strangerSeed = await SeedAsync(stranger);
        var close = await CloseAsync(
            owner,
            new DayCloseRequest(
                Date(today), CashAmount: "2000.0000",
                PosAmounts: [new(f.PosDefinitionId, "1000.0000")]));

        foreach (var id in new[] { close.Id, Guid.NewGuid() })
        {
            using var read = await stranger.GetAsync($"{Path}/{id}");
            Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
            using var revert = await stranger.DeleteAsync($"{Path}/{id}");
            Assert.Equal(HttpStatusCode.NotFound, revert.StatusCode);
        }

        // Sahibin POS'u ve kasası yabancı için yoktur.
        foreach (var posId in new[] { f.PosDefinitionId, Guid.NewGuid() })
        {
            await AssertRejectedAsync(
                stranger, HttpStatusCode.NotFound, "day_closes.pos_unavailable",
                new DayCloseRequest(
                    Date(today), PosAmounts: [new(posId, "100.0000")],
                    ClientRequestId: Guid.NewGuid()));
        }

        foreach (var accountId in new[] { f.TillId, Guid.NewGuid() })
        {
            await AssertRejectedAsync(
                stranger, HttpStatusCode.NotFound, "day_closes.cash_account_unavailable",
                new DayCloseRequest(
                    Date(today), CashAmount: "100.0000", CashAccountId: accountId,
                    ClientRequestId: Guid.NewGuid()));
        }

        // Yabancı sahibin gününü kapalı görmez, kayıtlarını da listede görmez.
        var strangerPreview = await PreviewAsync(
            stranger, new DayCloseRequest(Date(today), CashAmount: "100.0000"));
        Assert.Empty(strangerPreview.ClosedBy);
        Assert.Empty(strangerPreview.ExistingRecords);
        Assert.Null(strangerPreview.BlockerCode);
        Assert.Equal(strangerSeed.TillId, strangerPreview.Cash.AccountId);
        var strangerList = await stranger.GetFromJsonAsync<DayCloseListResponse>(
            $"{Path}?from={Date(today)}&to={Date(today)}");
        Assert.Empty(strangerList!.Items);

        var read2 = await owner.GetFromJsonAsync<DayCloseResponse>($"{Path}/{close.Id}");
        Assert.False(read2!.IsCancelled);
        Assert.Equal("2000.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("0.0000", await BalanceAsync(stranger, strangerSeed.TillId));
    }

    private sealed record Seed(
        Guid TillId,
        Guid BankId,
        Guid SalesCategoryId,
        Guid CommissionCategoryId,
        Guid PosDefinitionId);

    /// <summary>
    /// İşletme etiketli bir kasa ve banka hesabı (ikisi de boş) ile %2
    /// komisyonlu, ertesi gün geçen bir POS.
    /// </summary>
    private static async Task<Seed> SeedAsync(HttpClient client)
    {
        var till = await CreateAccountAsync(client, "Sentetik kasa", "cash");
        var bank = await CreateAccountAsync(client, "Sentetik banka", "bank");
        var sales = await FirstCategoryAsync(client, "income");
        var commission = await FirstCategoryAsync(client, "expense");
        using var response = await client.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest(
                "Sentetik POS", bank.Id, sales.Id, "0.0200", 1, false, commission.Id));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var definition = (await response.Content.ReadFromJsonAsync<PosDefinitionResponse>())!;
        return new Seed(till.Id, bank.Id, sales.Id, commission.Id, definition.Id);
    }

    private static async Task<DayClosePreviewResponse> PreviewAsync(
        HttpClient client,
        DayCloseRequest request)
    {
        using var response = await client.PostAsJsonAsync($"{Path}/preview", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<DayClosePreviewResponse>())!;
    }

    private static async Task<DayCloseResponse> CloseAsync(HttpClient client, DayCloseRequest request)
    {
        using var response = await client.PostAsJsonAsync(
            Path, request with { ClientRequestId = request.ClientRequestId ?? Guid.NewGuid() });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<DayCloseResponse>())!;
    }

    private static async Task AssertRejectedAsync(
        HttpClient client,
        HttpStatusCode status,
        string code,
        DayCloseRequest request)
    {
        using var response = await client.PostAsJsonAsync(Path, request);
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(code, await CodeAsync(response));
    }

    private static async Task<TransactionResponse> CreateIncomeAsync(
        HttpClient client,
        Guid accountId,
        Guid categoryId,
        string amount,
        DateOnly date)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                accountId, categoryId, amount, "TRY", "income", "business", Date(date), null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<TransactionResponse>())!;
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

    /// <summary>Nakit hesaba cari tahsilat; kimliğini döner.</summary>
    private static async Task<Guid> CreateCollectionAsync(
        HttpClient client,
        Guid accountId,
        string amount,
        DateOnly date)
    {
        using var counterparty = await client.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest("Sentetik müşteri"));
        Assert.Equal(HttpStatusCode.Created, counterparty.StatusCode);
        var counterpartyId =
            (await counterparty.Content.ReadFromJsonAsync<CounterpartyResponse>())!.Id;
        using var payment = await client.PostAsJsonAsync(
            $"/api/v1/counterparties/{counterpartyId}/payments",
            new CreateCounterpartyPaymentRequest("receivable", amount, "TRY", accountId, Date(date)));
        Assert.Equal(HttpStatusCode.Created, payment.StatusCode);
        return (await payment.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!.Id;
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

    private static async Task<CategoryResponse> FirstCategoryAsync(HttpClient client, string type)
    {
        var categories = await client.GetFromJsonAsync<CategoryListResponse>(
            $"/api/v1/categories?type={type}");
        return categories!.Items[0];
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string name,
        string type)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/accounts",
            new CreateAccountRequest(name, type, "TRY", "0.0000", "business"));
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
