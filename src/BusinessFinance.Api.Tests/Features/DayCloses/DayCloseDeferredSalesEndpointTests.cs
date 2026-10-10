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
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Reports;
using BusinessFinance.Api.Features.Transactions;

namespace BusinessFinance.Api.Tests.Features.DayCloses;

/// <summary>
/// Gün sonunda vadeli satış ve tahsilat (Aşama 06.3 Grup 5, karar tablosu
/// G1–G4): <c>stages/06.3-butunsel-duzenleme.md</c> içindeki Tablo 1–8'in her
/// satırı. Her tabloda günün uygulamaya yazılmamış nakit satışı 1.000'dir.
/// </summary>
/// <remarks>
/// Satırların "girilen → düşülen → yeni satış" kısmı önizlemeyle, tablonun
/// doğru satırı gerçek kayıtla doğrulanır: kasa girişi kasanın bakiyesidir
/// (kasa boş başlar), günün geliri ayın gelir toplamıdır (kullanıcının başka
/// kaydı yoktur).
/// </remarks>
public sealed class DayCloseDeferredSalesEndpointTests
{
    private const string Password = "Valid-Password-123!";
    private const string Path = "/api/v1/day-closes";
    private const string Charge = "counterparty-charge";
    private const string Payment = "counterparty-payment";
    private const string Invoice = "obligation";
    private const string InvoiceSettlement = "obligation-settlement";

    /// <summary>Tablo 1 — eski borcun tahsilatı (kayıtlı: 300 tahsilat).</summary>
    [Fact]
    public async Task Table1_CollectionOfAnOldDebt_IsDeductedOnlyWhenItIsInsideTheCashAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-1@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var collection = await CollectAsync(owner, person, f.TillId, "300.0000", today);

        // Hazır cevap yok: tahsilat cevaplanmadan gün sonu yazılmaz.
        var unanswered = await PreviewAsync(owner, Cash(today, "1000.0000"));
        Assert.Equal("day_closes.records_unanswered", unanswered.BlockerCode);
        var listed = Assert.Single(unanswered.ExistingRecords);
        Assert.Equal((Payment, "cash", true, false, (bool?)null),
            (listed.Kind, listed.Side, listed.RequiresAnswer, listed.IncludedByDefault,
                listed.Included));
        Assert.Equal(person, listed.GroupId);
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.records_unanswered",
            Cash(today, "1000.0000") with { ClientRequestId = Guid.NewGuid() });

        await AssertRowAsync(owner, Cash(today, "1000.0000", Out(Payment, collection)), "0.0000", "1000.0000");
        await AssertRowAsync(owner, Cash(today, "1300.0000", In(Payment, collection)), "300.0000", "1000.0000");
        // Yanlış satırlar yalnız kullanıcının açık cevabıyla oluşur.
        await AssertRowAsync(owner, Cash(today, "1000.0000", In(Payment, collection)), "300.0000", "700.0000");
        await AssertRowAsync(owner, Cash(today, "1300.0000", Out(Payment, collection)), "0.0000", "1300.0000");

        var close = await CloseAsync(owner, Cash(today, "1300.0000", In(Payment, collection)));
        Assert.Equal("1000.0000", close.CashAmount);
        Assert.Equal("300.0000", close.CountedCashAmount);
        Assert.Empty(close.Overlaps);
        Assert.Equal("1300.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1000.0000", await MonthIncomeAsync(owner, today));
    }

    /// <summary>Tablo 2 — bugünkü veresiye satış, para alınmadı (kayıtlı: 300 satış).</summary>
    [Fact]
    public async Task Table2_SaleOnCreditRungAsCash_IsDeductedSoItIsNotCountedTwice()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-2@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var sale = await SellOnCreditAsync(owner, person, f.SalesCategoryId, "300.0000", today);

        var unanswered = await PreviewAsync(owner, Cash(today, "1300.0000"));
        Assert.Equal("day_closes.records_unanswered", unanswered.BlockerCode);
        var listed = Assert.Single(unanswered.ExistingRecords);
        Assert.Equal((Charge, "cash", true, (bool?)null, "300.0000"),
            (listed.Kind, listed.Side, listed.RequiresAnswer, listed.Included, listed.Amount));
        Assert.Equal((person, "Sentetik müşteri"), (listed.GroupId, listed.GroupName));

        await AssertRowAsync(owner, Cash(today, "1000.0000", Out(Charge, sale)), "0.0000", "1000.0000");
        await AssertRowAsync(owner, Cash(today, "1300.0000", In(Charge, sale)), "300.0000", "1000.0000");
        await AssertRowAsync(owner, Cash(today, "1300.0000", Out(Charge, sale)), "0.0000", "1300.0000");
        await AssertRowAsync(owner, Cash(today, "1000.0000", In(Charge, sale)), "300.0000", "700.0000");

        var close = await CloseAsync(owner, Cash(today, "1300.0000", In(Charge, sale)));
        Assert.Equal("1000.0000", close.CashAmount);
        var counted = Assert.Single(close.CountedRecords);
        Assert.Equal((Charge, sale, (bool?)true), (counted.Kind, counted.Id, counted.Included));
        // Veresiye satış gelir yazdı, kasaya girmedi.
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1300.0000", await MonthIncomeAsync(owner, today));

        // Sayılan vadeli satış tek başına iptal edilemez; akış da bunu söyler.
        var feed = await owner.GetFromJsonAsync<FinancialActivityListResponse>(
            "/api/v1/financial-activities");
        Assert.False(Assert.Single(feed!.Items, item => item.ActivityId == sale).CanCancel);
        using var locked = await owner.DeleteAsync($"/api/v1/counterparty-charges/{sale}");
        Assert.Equal(HttpStatusCode.Conflict, locked.StatusCode);
        Assert.Equal("counterparty_charges.day_close_counted", await CodeAsync(locked));

        // Geri almada satış serbest kalır: yeniden listelenir ve iptal edilebilir.
        using var revert = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        var reopened = await PreviewAsync(owner, Cash(today, "1300.0000"));
        Assert.Equal(sale, Assert.Single(reopened.ExistingRecords).Id);
        using var cancelled = await owner.DeleteAsync($"/api/v1/counterparty-charges/{sale}");
        Assert.Equal(HttpStatusCode.OK, cancelled.StatusCode);
    }

    /// <summary>
    /// Tablo 3 — satış anında kısmi ödeme (kayıtlı: 500 satış + 200 tahsilat).
    /// </summary>
    [Fact]
    public async Task Table3_PartialPaymentAtTheSale_NeedsTheSharedAmountWhenBothAreInside()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-3@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var sale = await SellOnCreditAsync(owner, person, f.SalesCategoryId, "500.0000", today);
        var paid = await CollectAsync(owner, person, f.TillId, "200.0000", today);

        await AssertRowAsync(
            owner, Cash(today, "1000.0000", Out(Charge, sale), Out(Payment, paid)), "0.0000", "1000.0000");
        await AssertRowAsync(
            owner, Cash(today, "1200.0000", Out(Charge, sale), In(Payment, paid)), "200.0000", "1000.0000");
        await AssertRowAsync(
            owner, Cash(today, "1500.0000", In(Charge, sale), Out(Payment, paid)), "500.0000", "1000.0000");
        await AssertRowAsync(
            owner, Cash(today, "1200.0000", Out(Charge, sale), Out(Payment, paid)), "0.0000", "1200.0000");

        // İkisi de dahil: ortak tutar verilmeden yazılmaz ve sunucu sorar.
        var both = Cash(today, "1500.0000", In(Charge, sale), In(Payment, paid));
        var asked = await PreviewAsync(owner, both);
        Assert.Equal("day_closes.overlap_unanswered", asked.BlockerCode);
        var group = Assert.Single(asked.OverlapGroups);
        Assert.Equal(
            (person, "Sentetik müşteri", "500.0000", "200.0000", "200.0000", (string?)null, (string?)null),
            (group.GroupId, group.Name, group.SalesAmount, group.CollectionsAmount,
                group.MaximumOverlap, group.OverlapAmount, group.DeductedAmount));
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.overlap_unanswered",
            both with { ClientRequestId = Guid.NewGuid() });

        // Tahsilat satışın içinde bir kez sayıldıysa ortak tutar tahsilat kadardır.
        var once = await AssertRowAsync(
            owner, both with { Overlaps = [new(person, "200.0000")] }, "500.0000", "1000.0000");
        Assert.Equal(
            ("200.0000", "500.0000"),
            (Assert.Single(once.OverlapGroups).OverlapAmount,
                Assert.Single(once.OverlapGroups).DeductedAmount));
        // Aynı satışın ödemesi olsa da tahsilat girilen toplamda satışa ek olarak
        // yer alıyorsa (1.700) ortak tutar sıfırdır: ortak tutar ödemenin hangi
        // satışa ait olduğunu değil, toplamın nasıl oluştuğunu söyler.
        await AssertRowAsync(
            owner,
            Cash(today, "1700.0000", In(Charge, sale), In(Payment, paid)) with
            {
                Overlaps = [new(person, "0.0000")],
            },
            "700.0000", "1000.0000");
        // Yanlış satırlar: ayrı tutarlar diye ikisi birden düşülürse.
        await AssertRowAsync(
            owner,
            Cash(today, "1200.0000", In(Charge, sale), In(Payment, paid)) with
            {
                Overlaps = [new(person, "0.0000")],
            },
            "700.0000", "500.0000");
        await AssertRowAsync(
            owner, both with { Overlaps = [new(person, "0.0000")] }, "700.0000", "800.0000");

        // Ortak tutar iki toplamdan küçük olanı aşamaz ve sorulmayan grup için verilemez.
        Assert.Equal(
            "day_closes.invalid_overlap",
            (await PreviewAsync(owner, both with { Overlaps = [new(person, "200.0001")] })).BlockerCode);
        Assert.Equal(
            "day_closes.invalid_overlap",
            (await PreviewAsync(
                owner,
                Cash(today, "1500.0000", In(Charge, sale), Out(Payment, paid)) with
                {
                    Overlaps = [new(person, "200.0000")],
                })).BlockerCode);

        var close = await CloseAsync(owner, both with { Overlaps = [new(person, "200.0000")] });
        Assert.Equal("1000.0000", close.CashAmount);
        Assert.Equal(2, close.CountedRecords.Count);
        // Düşülen 500: sayılan 700, ortak 200.
        Assert.Equal("500.0000", close.CountedCashAmount);
        var stored = Assert.Single(close.Overlaps);
        Assert.Equal((person, "200.0000", "500.0000"),
            (stored.GroupId, stored.OverlapAmount, stored.DeductedAmount));
        Assert.Equal("1200.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1500.0000", await MonthIncomeAsync(owner, today));

        // Günün toplamı girilen tutardır: yazılan 1.000 + düşülen 500.
        var day = await owner.GetFromJsonAsync<DayCloseDayResponse>(
            $"{Path}/day?date={Date(today)}");
        Assert.Equal("1500.0000", day!.CashTotal);

        // Geri alma ortak tutarı da bırakır.
        using var revert = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        var reverted = await owner.GetFromJsonAsync<DayCloseResponse>($"{Path}/{close.Id}");
        Assert.Empty(reverted!.Overlaps);
        Assert.Empty(reverted.CountedRecords);
        Assert.Equal("200.0000", await BalanceAsync(owner, f.TillId));
    }

    /// <summary>
    /// Tablo 4 — aynı kişi, aynı gün, ayrı iki olay: eski borçtan 200 tahsilat
    /// ve yeni 300 veresiye, ikisi de tutarın içinde.
    /// </summary>
    [Fact]
    public async Task Table4_SeparateSaleAndCollectionOfOnePerson_AreBothDeducted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-4@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var sale = await SellOnCreditAsync(owner, person, f.SalesCategoryId, "300.0000", today);
        var paid = await CollectAsync(owner, person, f.TillId, "200.0000", today);
        var both = Cash(today, "1500.0000", In(Charge, sale), In(Payment, paid));

        await AssertRowAsync(
            owner, both with { Overlaps = [new(person, "0.0000")] }, "500.0000", "1000.0000");
        await AssertRowAsync(
            owner, both with { Overlaps = [new(person, "200.0000")] }, "300.0000", "1200.0000");

        var close = await CloseAsync(owner, both with { Overlaps = [new(person, "0.0000")] });
        Assert.Equal("500.0000", close.CountedCashAmount);
        // "Ayrı ayrı" saklanacak bir ortak tutar bırakmaz.
        Assert.Empty(close.Overlaps);
        Assert.Equal("1200.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1300.0000", await MonthIncomeAsync(owner, today));
    }

    /// <summary>
    /// Tablo 5 — aynı gün yazılıp nakit kapatılan alacak faturası (kayıtlı: 400
    /// fatura + kendi 400 kapanışı). Bağ bilinir ama ortak tutar yine girilen
    /// toplamda nasıl sayıldıklarına göre sorulur.
    /// </summary>
    [Fact]
    public async Task Table5_InvoiceSettledTheSameDay_FollowsTheSameSharedAmountRule()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-5@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var invoice = await InvoiceAsync(owner, f.SalesCategoryId, "400.0000", today);
        var settlement = await SettleAsync(owner, invoice, f.TillId, today);

        var none = Cash(today, "1000.0000", Out(Invoice, invoice), Out(InvoiceSettlement, settlement));
        var listed = (await AssertRowAsync(owner, none, "0.0000", "1000.0000")).ExistingRecords;
        Assert.Equal(2, listed.Count);
        // Fatura ile kendi kapanışı aynı gruptadır.
        Assert.All(listed, record => Assert.Equal(invoice, record.GroupId));
        Assert.All(listed, record => Assert.True(record.RequiresAnswer));

        var both = Cash(today, "1400.0000", In(Invoice, invoice), In(InvoiceSettlement, settlement));
        Assert.Equal("day_closes.overlap_unanswered", (await PreviewAsync(owner, both)).BlockerCode);
        await AssertRowAsync(
            owner, both with { Overlaps = [new(invoice, "400.0000")] }, "400.0000", "1000.0000");
        // Girilen toplamda ikisi ayrı ayrı sayıldıysa (1.800) ikisi de düşülür.
        await AssertRowAsync(
            owner,
            Cash(today, "1800.0000", In(Invoice, invoice), In(InvoiceSettlement, settlement)) with
            {
                Overlaps = [new(invoice, "0.0000")],
            },
            "800.0000", "1000.0000");
        await AssertRowAsync(
            owner, both with { Overlaps = [new(invoice, "0.0000")] }, "800.0000", "600.0000");

        var close = await CloseAsync(owner, both with { Overlaps = [new(invoice, "400.0000")] });
        Assert.Equal("400.0000", close.CountedCashAmount);
        Assert.Equal("400.0000", Assert.Single(close.Overlaps).OverlapAmount);
        Assert.Equal("1400.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1400.0000", await MonthIncomeAsync(owner, today));

        // Sayılan fatura ve kapanışı tek başına iptal edilemez.
        using var locked = await owner.DeleteAsync($"/api/v1/obligations/{invoice}");
        Assert.Equal(HttpStatusCode.Conflict, locked.StatusCode);
        Assert.Equal("obligations.day_close_counted", await CodeAsync(locked));
    }

    /// <summary>
    /// Yalnız faturası sayılan (kapanışı sayılmayan ya da hiç kapanmamış)
    /// alacak da tek başına iptal edilemez.
    /// </summary>
    [Fact]
    public async Task CountedInvoice_IsLockedEvenWhenItsSettlementIsNotCounted()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-5b@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var invoice = await InvoiceAsync(owner, f.SalesCategoryId, "400.0000", today);

        var close = await CloseAsync(owner, Cash(today, "1400.0000", In(Invoice, invoice)));
        Assert.Equal("1000.0000", close.CashAmount);
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1400.0000", await MonthIncomeAsync(owner, today));

        using var locked = await owner.DeleteAsync($"/api/v1/obligations/{invoice}");
        Assert.Equal(HttpStatusCode.Conflict, locked.StatusCode);
        Assert.Equal("obligations.day_close_counted", await CodeAsync(locked));

        using var revert = await owner.DeleteAsync($"{Path}/{close.Id}");
        Assert.Equal(HttpStatusCode.OK, revert.StatusCode);
        using var cancelled = await owner.DeleteAsync($"/api/v1/obligations/{invoice}");
        Assert.Equal(HttpStatusCode.OK, cancelled.StatusCode);
    }

    /// <summary>
    /// Tablo 6 — kısmi örtüşme: 500 satış + 300 tahsilat (200'ü bugünkü satış
    /// için, 100'ü eski borç için). Doğru düşüm 600'dür ve tam kayıt seçerek
    /// ulaşılamaz.
    /// </summary>
    [Fact]
    public async Task Table6_PartlySharedCollection_IsDeductedOnceThroughTheSharedAmount()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-6@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var sale = await SellOnCreditAsync(owner, person, f.SalesCategoryId, "500.0000", today);
        var paid = await CollectAsync(owner, person, f.TillId, "300.0000", today);

        await AssertRowAsync(
            owner, Cash(today, "1600.0000", Out(Charge, sale), In(Payment, paid)), "300.0000", "1300.0000");
        await AssertRowAsync(
            owner, Cash(today, "1600.0000", In(Charge, sale), Out(Payment, paid)), "500.0000", "1100.0000");
        var both = Cash(today, "1600.0000", In(Charge, sale), In(Payment, paid));
        await AssertRowAsync(
            owner, both with { Overlaps = [new(person, "0.0000")] }, "800.0000", "800.0000");
        var right = both with { Overlaps = [new(person, "200.0000")] };
        var preview = await AssertRowAsync(owner, right, "600.0000", "1000.0000");
        Assert.Equal("300.0000", Assert.Single(preview.OverlapGroups).MaximumOverlap);

        var close = await CloseAsync(owner, right);
        Assert.Equal("600.0000", close.CountedCashAmount);
        Assert.Equal("1300.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1500.0000", await MonthIncomeAsync(owner, today));
        var day = await owner.GetFromJsonAsync<DayCloseDayResponse>(
            $"{Path}/day?date={Date(today)}");
        Assert.Equal("1600.0000", day!.CashTotal);
    }

    /// <summary>
    /// Tablo 7 — toplamdan türetme yok, gelir zaten yazılmışken: 1.000 nakit,
    /// 500 kart, 300 kredili veresiye (kayıtlı); raporun toplamı 1.800.
    /// </summary>
    [Fact]
    public async Task Table7_TotalNeverProducesTheMissingSide_WhenASaleOnCreditIsRecorded()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-7@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var sale = await SellOnCreditAsync(owner, person, f.SalesCategoryId, "300.0000", today);

        var both = await PreviewAsync(
            owner,
            Cash(today, "1000.0000", Out(Charge, sale)) with
            {
                PosAmounts = [new(f.PosDefinitionId, "500.0000")],
            });
        Assert.Null(both.BlockerCode);
        Assert.Equal("1000.0000", both.Cash.AmountToWrite);
        Assert.Equal("500.0000", Assert.Single(both.PosLines).AmountToWrite);

        // Toplam ve nakit: kart hesaplanmaz, POS satırına dokunulmaz.
        var totalAndCash = await PreviewAsync(
            owner, Cash(today, "1000.0000", Out(Charge, sale)) with { TotalAmount = "1800.0000" });
        Assert.Null(totalAndCash.BlockerCode);
        Assert.False(Assert.Single(totalAndCash.PosLines).Stated);
        Assert.Equal("0.0000", Assert.Single(totalAndCash.PosLines).AmountToWrite);
        Assert.Equal("800.0000", totalAndCash.TotalDifference);

        // Toplam ve kart: nakit hesaplanmaz. Nakit yazılmadığı için nakit
        // tarafının soruları da sorulmaz; kayıt engellenmez.
        var totalAndCard = new DayCloseRequest(
            Date(today),
            PosAmounts: [new(f.PosDefinitionId, "500.0000")],
            TotalAmount: "1800.0000");
        var preview = await PreviewAsync(owner, totalAndCard);
        Assert.Null(preview.BlockerCode);
        Assert.False(preview.Cash.Stated);
        Assert.Equal("0.0000", preview.Cash.AmountToWrite);
        Assert.Equal("500.0000", preview.TotalComputed);
        Assert.Equal("1300.0000", preview.TotalDifference);

        var close = await CloseAsync(owner, totalAndCard);
        Assert.Empty(close.Incomes);
        Assert.Equal("500.0000", close.CardGrossAmount);
        Assert.Empty(close.CountedRecords);
        Assert.Equal("0.0000", await BalanceAsync(owner, f.TillId));
        // 500 kartlı satış + 300 veresiye; olmayan bir nakit yazılmadı.
        Assert.Equal("800.0000", await MonthIncomeAsync(owner, today));
    }

    /// <summary>
    /// Tablo 8 — toplamdan türetme yok, başka ödeme türü kayıtsızken: 1.000
    /// nakit, 500 kart (ana POS), 300 yemek kartı (kendi POS'u); toplam 1.800.
    /// </summary>
    [Fact]
    public async Task Table8_TotalNeverProducesTheMissingSide_WhenAnotherPaymentTypeExists()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-8@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        using var response = await owner.PostAsJsonAsync(
            "/api/v1/pos-definitions",
            new SavePosDefinitionRequest(
                "Sentetik yemek kartı", f.BankId, f.SalesCategoryId, "0.0500", 7, false,
                f.CommissionCategoryId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var mealCard = (await response.Content.ReadFromJsonAsync<PosDefinitionResponse>())!.Id;

        // Toplam ve ana POS: nakit hesaplanmaz, yemek kartı POS'una dokunulmaz.
        var totalAndCard = await PreviewAsync(
            owner,
            new DayCloseRequest(
                Date(today),
                PosAmounts: [new(f.PosDefinitionId, "500.0000")],
                TotalAmount: "1800.0000"));
        Assert.False(totalAndCard.Cash.Stated);
        Assert.False(totalAndCard.PosLines.Single(line => line.PosDefinitionId == mealCard).Stated);
        Assert.Equal("1300.0000", totalAndCard.TotalDifference);

        // Toplam ve nakit: 800 ana POS'a yazılmaz.
        var totalAndCash = await PreviewAsync(
            owner, Cash(today, "1000.0000") with { TotalAmount = "1800.0000" });
        Assert.All(totalAndCash.PosLines, line => Assert.False(line.Stated));
        Assert.Equal("800.0000", totalAndCash.TotalDifference);

        // Yalnız toplam hiçbir şey yazmaz.
        Assert.Equal(
            "day_closes.amounts_required",
            (await PreviewAsync(owner, new DayCloseRequest(Date(today), TotalAmount: "1800.0000")))
            .BlockerCode);

        var close = await CloseAsync(
            owner,
            Cash(today, "1000.0000") with
            {
                PosAmounts = [new(f.PosDefinitionId, "500.0000"), new(mealCard, "300.0000")],
                TotalAmount = "1800.0000",
            });
        Assert.Equal("1000.0000", close.CashAmount);
        Assert.Equal(
            ["300.0000", "500.0000"],
            close.Settlements.Select(settlement => settlement.GrossAmount).Order());
        Assert.Equal(
            mealCard,
            close.Settlements.Single(settlement => settlement.GrossAmount == "300.0000")
                .PosDefinitionId);
        Assert.Equal("1000.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("1800.0000", await MonthIncomeAsync(owner, today));
    }

    /// <summary>
    /// Cevap görülen kayıtlar içindir: önizlemeden sonra gelen kayıt cevapsızdır,
    /// listede artık olmayan kayıt için cevap taşıyan istek reddedilir; ek gün
    /// sonunda da aynı soru sorulur.
    /// </summary>
    [Fact]
    public async Task Answers_CoverOnlyTheRecordsTheUserSaw()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-guard@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        var person = await CreatePersonAsync(owner, "Sentetik müşteri");
        var first = await CollectAsync(owner, person, f.TillId, "300.0000", today);
        var answered = Cash(today, "1300.0000", In(Payment, first));
        Assert.Null((await PreviewAsync(owner, answered)).BlockerCode);

        // Panel açıkken ikinci bir tahsilat girildi: "hepsi" onu içine almaz.
        var second = await CollectAsync(owner, person, f.TillId, "50.0000", today);
        await AssertRejectedAsync(
            owner, HttpStatusCode.BadRequest, "day_closes.records_unanswered",
            answered with { ClientRequestId = Guid.NewGuid() });

        // Cevaplanan kayıt bu sırada iptal edildi: istek eski listeye bakıyor.
        using var cancel = await owner.DeleteAsync($"/api/v1/counterparty-payments/{second}");
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);
        var stale = Cash(today, "1300.0000", In(Payment, first), Out(Payment, second));
        Assert.Equal("day_closes.records_changed", (await PreviewAsync(owner, stale)).BlockerCode);
        await AssertRejectedAsync(
            owner, HttpStatusCode.Conflict, "day_closes.records_changed",
            stale with { ClientRequestId = Guid.NewGuid() });

        // Dışarıda bırakılan tahsilat sayılmaz; ek gün sonunda yeniden sorulur.
        var third = await CollectAsync(owner, person, f.TillId, "70.0000", today);
        await CloseAsync(owner, Cash(today, "1300.0000", In(Payment, first), Out(Payment, third)));
        var additional = Cash(today, "400.0000") with { IsAdditional = true };
        var asked = await PreviewAsync(owner, additional);
        Assert.Equal("day_closes.records_unanswered", asked.BlockerCode);
        Assert.Equal(third, Assert.Single(asked.ExistingRecords).Id);
    }

    /// <summary>
    /// Panel hiçbir tutarı toplamaz: "zaten kayıtlı" tutarın dökümü
    /// önizlemeden gelir. Gün: 250 tek tek girilmiş nakit
    /// satış, 300 ve 120 tahsilat, 450 veresiye satış; yazılan nakit 1.670.
    /// </summary>
    [Fact]
    public async Task Preview_BreaksDownWhatIsAlreadyRecorded()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-panel@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        using var income = await owner.PostAsJsonAsync(
            "/api/v1/transactions",
            new CreateTransactionRequest(
                f.TillId, f.SalesCategoryId, "250.0000", "TRY", "income", "business", Date(today), null));
        Assert.Equal(HttpStatusCode.Created, income.StatusCode);
        using var cardSale = await owner.PostAsJsonAsync(
            "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                null, null, "800.0000", "TRY", Date(today),
                Scope: "business", PosDefinitionId: f.PosDefinitionId));
        Assert.Equal(HttpStatusCode.Created, cardSale.StatusCode);
        var first = await CollectAsync(
            owner, await CreatePersonAsync(owner, "Sentetik bakkal"), f.TillId, "300.0000", today);
        var second = await CollectAsync(
            owner, await CreatePersonAsync(owner, "Sentetik usta"), f.TillId, "120.0000", today);
        var sale = await SellOnCreditAsync(
            owner, await CreatePersonAsync(owner, "Sentetik terzi"), f.SalesCategoryId, "450.0000", today);

        // Tahsilatların ikisi de içinde, veresiye satış değil: 1.670 − 670 = 1.000.
        var all = await AssertRowAsync(
            owner,
            Cash(today, "1670.0000", In(Payment, first), In(Payment, second), Out(Charge, sale)),
            "670.0000", "1000.0000");
        Assert.Equal(
            new DayCloseCashDeductionsResponse("250.0000", "420.0000", "0.0000", "0.0000", "0.0000"),
            all.Cash.Deductions);
        Assert.Equal(5, all.ExistingRecords.Count);

        // Yalnız biri içinde: 1.670 − 550 = 1.120.
        var some = await AssertRowAsync(
            owner,
            Cash(today, "1670.0000", In(Payment, first), Out(Payment, second), Out(Charge, sale)),
            "550.0000", "1120.0000");
        Assert.Equal(
            new DayCloseCashDeductionsResponse("250.0000", "300.0000", "0.0000", "0.0000", "0.0000"),
            some.Cash.Deductions);

        // Veresiye satış da içinde: kendi alanında gelir.
        var withSale = await AssertRowAsync(
            owner,
            Cash(today, "1670.0000", In(Payment, first), Out(Payment, second), In(Charge, sale)),
            "1000.0000", "670.0000");
        Assert.Equal(
            new DayCloseCashDeductionsResponse("250.0000", "300.0000", "450.0000", "0.0000", "0.0000"),
            withSale.Cash.Deductions);

        // Nakit yazılmadıysa nakitten hiçbir şey düşülmez; kartlı kayıt kendi
        // POS'undan düşer: 1.300 − 800 = 500.
        var cardOnly = await PreviewAsync(
            owner,
            new DayCloseRequest(Date(today), PosAmounts: [new(f.PosDefinitionId, "1300.0000")]));
        Assert.Null(cardOnly.BlockerCode);
        Assert.Equal(
            new DayCloseCashDeductionsResponse("0.0000", "0.0000", "0.0000", "0.0000", "0.0000"),
            cardOnly.Cash.Deductions);
        var line = Assert.Single(cardOnly.PosLines);
        Assert.Equal(("800.0000", "500.0000"), (line.DeductedAmount, line.AmountToWrite));
    }

    /// <summary>
    /// Ortak tutarı sorulan grup üç cevabın sonucunu hazır taşır: ayrı ayrı,
    /// biri öbürünün içinde, bir kısmı. Grup bir kişi ya da bir faturadır ve
    /// hangisinin büyük olduğu sunucudan gelir.
    /// </summary>
    [Fact]
    public async Task OverlapGroup_CarriesTheResultOfEachAnswer()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await CreateAuthenticatedClientAsync(factory, "defer-options@example.test");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f = await SeedAsync(owner);
        // Satış tahsilattan büyük: 500 veresiye + 300 tahsilat.
        var usta = await CreatePersonAsync(owner, "Sentetik usta");
        var ustaSale = await SellOnCreditAsync(owner, usta, f.SalesCategoryId, "500.0000", today);
        var ustaPaid = await CollectAsync(owner, usta, f.TillId, "300.0000", today);
        var one = Cash(today, "1600.0000", In(Charge, ustaSale), In(Payment, ustaPaid));

        var asked = await PreviewAsync(owner, one);
        Assert.Equal("day_closes.overlap_unanswered", asked.BlockerCode);
        Assert.Equal(
            new DayCloseOverlapGroupResponse(
                usta, "Sentetik usta", "500.0000", "300.0000", "300.0000", null, null,
                "counterparty", "800.0000", "500.0000", "sales"),
            Assert.Single(asked.OverlapGroups));

        // Üç cevap: ayrı ayrı 800, tahsilat satışın içinde 500, bir kısmı (200) 600.
        await AssertRowAsync(owner, one with { Overlaps = [new(usta, "0.0000")] }, "800.0000", "800.0000");
        await AssertRowAsync(owner, one with { Overlaps = [new(usta, "300.0000")] }, "500.0000", "1100.0000");
        var partly = await AssertRowAsync(
            owner, one with { Overlaps = [new(usta, "200.0000")] }, "600.0000", "1000.0000");
        Assert.Equal(
            ("200.0000", "600.0000"),
            (Assert.Single(partly.OverlapGroups).OverlapAmount,
                Assert.Single(partly.OverlapGroups).DeductedAmount));
        Assert.Equal(
            new DayCloseCashDeductionsResponse("0.0000", "300.0000", "500.0000", "0.0000", "200.0000"),
            partly.Cash.Deductions);

        // Tahsilat satıştan büyük (200 veresiye + 350 tahsilat) ve aynı gün
        // kapatılan 400'lük alacak faturası.
        var komsu = await CreatePersonAsync(owner, "Sentetik komşu");
        var komsuSale = await SellOnCreditAsync(owner, komsu, f.SalesCategoryId, "200.0000", today);
        var komsuPaid = await CollectAsync(owner, komsu, f.TillId, "350.0000", today);
        var invoice = await InvoiceAsync(owner, f.SalesCategoryId, "400.0000", today);
        var settlement = await SettleAsync(owner, invoice, f.TillId, today);
        // Düşülen: usta 600 + komşu 350 + fatura 400 = 1.350; yeni satış 1.200.
        var three = Cash(
            today, "2550.0000",
            In(Charge, ustaSale), In(Payment, ustaPaid),
            In(Charge, komsuSale), In(Payment, komsuPaid),
            In(Invoice, invoice), In(InvoiceSettlement, settlement)) with
        {
            Overlaps = [new(usta, "200.0000"), new(komsu, "200.0000"), new(invoice, "400.0000")],
        };
        var preview = await AssertRowAsync(owner, three, "1350.0000", "1200.0000");
        Assert.Equal(
            new DayCloseOverlapGroupResponse(
                komsu, "Sentetik komşu", "200.0000", "350.0000", "200.0000", "200.0000", "350.0000",
                "counterparty", "550.0000", "350.0000", "collections"),
            preview.OverlapGroups.Single(group => group.GroupId == komsu));
        // Eşit tutarda satış büyük sayılır: "tahsilat faturanın içinde".
        Assert.Equal(
            new DayCloseOverlapGroupResponse(
                invoice, "Sentetik fatura", "400.0000", "400.0000", "400.0000", "400.0000", "400.0000",
                "obligation", "800.0000", "400.0000", "sales"),
            preview.OverlapGroups.Single(group => group.GroupId == invoice));
        // 700 veresiye + 400 fatura + 1.050 tahsilat − 800 ikisinde de = 1.350.
        Assert.Equal(
            new DayCloseCashDeductionsResponse("0.0000", "1050.0000", "700.0000", "400.0000", "800.0000"),
            preview.Cash.Deductions);

        // Günün ekranı saklanan ortak tutarları aynı alanlarla verir.
        var close = await CloseAsync(owner, three);
        Assert.Equal(("1200.0000", "1350.0000"), (close.CashAmount, close.CountedCashAmount));
        Assert.Equal(
            preview.OverlapGroups.OrderBy(group => group.GroupId),
            close.Overlaps.OrderBy(group => group.GroupId));
        Assert.Equal("2250.0000", await BalanceAsync(owner, f.TillId));
        Assert.Equal("2300.0000", await MonthIncomeAsync(owner, today));
    }

    private static DayCloseRecordOverrideRequest In(string kind, Guid id) => new(kind, id, true);

    private static DayCloseRecordOverrideRequest Out(string kind, Guid id) => new(kind, id, false);

    private static DayCloseRequest Cash(
        DateOnly day,
        string amount,
        params DayCloseRecordOverrideRequest[] answers) =>
        new(Date(day), CashAmount: amount, RecordOverrides: answers);

    /// <summary>
    /// Tablonun bir satırı: bu girdiyle nakitten düşülen ve yazılacak yeni
    /// satış. Önizleme hiçbir şey yazmaz.
    /// </summary>
    private static async Task<DayClosePreviewResponse> AssertRowAsync(
        HttpClient client,
        DayCloseRequest request,
        string deducted,
        string newSale)
    {
        var preview = await PreviewAsync(client, request);
        Assert.Null(preview.BlockerCode);
        Assert.Equal((deducted, newSale), (preview.Cash.DeductedAmount, preview.Cash.AmountToWrite));
        return preview;
    }

    private sealed record Seed(
        Guid TillId,
        Guid BankId,
        Guid SalesCategoryId,
        Guid CommissionCategoryId,
        Guid PosDefinitionId);

    /// <summary>
    /// İşletme etiketli boş bir kasa ve banka hesabı ile %2 komisyonlu, ertesi
    /// gün geçen bir POS.
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

    private static async Task<Guid> CreatePersonAsync(HttpClient client, string name)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/counterparties", new CreateCounterpartyRequest(name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyResponse>())!.Id;
    }

    /// <summary>Veresiye satış: gelir yazar, kasaya girmez.</summary>
    private static async Task<Guid> SellOnCreditAsync(
        HttpClient client,
        Guid personId,
        Guid categoryId,
        string amount,
        DateOnly date)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/counterparties/{personId}/charges",
            new CreateCounterpartyChargeRequest("receivable", amount, "TRY", categoryId, Date(date)));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyChargeResponse>())!.Id;
    }

    /// <summary>Nakit tahsilat: kasaya girer, gelir yazmaz.</summary>
    private static async Task<Guid> CollectAsync(
        HttpClient client,
        Guid personId,
        Guid accountId,
        string amount,
        DateOnly date)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/counterparties/{personId}/payments",
            new CreateCounterpartyPaymentRequest("receivable", amount, "TRY", accountId, Date(date)));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CounterpartyPaymentResponse>())!.Id;
    }

    /// <summary>Alacak faturası (alacak yönlü yükümlülük): doğduğu gün gelir yazar.</summary>
    private static async Task<Guid> InvoiceAsync(
        HttpClient client,
        Guid categoryId,
        string amount,
        DateOnly date)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/obligations",
            new CreateObligationRequest(
                "receivable", amount, "TRY", categoryId, Date(date), Date(date.AddDays(30)),
                "business", Description: "Sentetik fatura"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ObligationResponse>())!.Id;
    }

    /// <summary>Faturanın nakit kapanışı; kapanışın kimliğini gün sonu listesinden okur.</summary>
    private static async Task<Guid> SettleAsync(
        HttpClient client,
        Guid obligationId,
        Guid accountId,
        DateOnly date)
    {
        using var response = await client.PostAsJsonAsync(
            $"/api/v1/obligations/{obligationId}/settlement",
            new SettleObligationRequest(accountId, Date(date)));
        Assert.True(response.IsSuccessStatusCode);
        var listed = await PreviewAsync(client, new DayCloseRequest(Date(date), CashAmount: "0.0000"));
        return listed.ExistingRecords.Single(record => record.Kind == InvoiceSettlement).Id;
    }

    private static async Task<string> BalanceAsync(HttpClient client, Guid accountId)
    {
        var account = await client.GetFromJsonAsync<AccountResponse>(
            $"/api/v1/accounts/{accountId}");
        return account!.Balance;
    }

    private static async Task<string> MonthIncomeAsync(HttpClient client, DateOnly day)
    {
        var report = await client.GetFromJsonAsync<MonthlyReportResponse>(
            $"/api/v1/reports/monthly?year={day.Year}&month={day.Month}");
        return report!.TotalIncome;
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
        return categories!.Items.First(item => item.DefaultScope == "business");
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
            "/api/v1/auth/register", new RegisterRequest(email, Password, HasBusiness: true));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new LoginRequest(email, Password));
        var tokens = (await login.Content.ReadFromJsonAsync<TokenPairResponse>())!;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
