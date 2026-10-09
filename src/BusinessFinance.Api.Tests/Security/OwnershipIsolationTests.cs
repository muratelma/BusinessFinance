using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using BusinessFinance.Api.Features.Accounts;
using BusinessFinance.Api.Features.Attachments;
using BusinessFinance.Api.Features.Authentication;
using BusinessFinance.Api.Features.Budgets;
using BusinessFinance.Api.Features.Cash;
using BusinessFinance.Api.Features.Categories;
using BusinessFinance.Api.Features.Counterparties;
using BusinessFinance.Api.Features.CreditCards;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Imports;
using BusinessFinance.Api.Features.Obligations;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.DayCloses;
using BusinessFinance.Api.Features.RecurringTransactions;
using BusinessFinance.Api.Features.SavingsGoals;
using BusinessFinance.Api.Features.Taxes;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Api.Features.Transfers;
using BusinessFinance.Api.Features.UserAccount;

namespace BusinessFinance.Api.Tests.Security;

/// <summary>
/// Sahiplik izolasyonunun uçtan uca denetimi (Aşama 06.1 Grup 3).
///
/// Tek bir kural ölçülür: <b>başka kullanıcıya ait kayıt ile var olmayan kayıt
/// aynı cevaba gider.</b> Farklı cevap vermek — biri 404, öteki 403 ya da 409 —
/// kaydın var olduğunu sızdırır; saldırgan kimlik denemesiyle kimin nesi
/// olduğunu haritalayabilir.
///
/// Denetim uç uç dağıtılmış testlere bırakılmadı, çünkü bu grubun işi yeni test
/// yazmaktan çok <b>eksik test aramaktı</b>: dağıtılmış testlerde eksik olanı
/// görmek, olanı görmekten zor. Burada uçların listesi tek yerde durur ve
/// <see cref="EveryOwnerScopedRoute_IsProbed"/> listeyi uygulamanın gerçek
/// route tablosuyla karşılaştırır — yeni bir uç eklenip buraya yazılmazsa
/// takım kırmızıya döner.
/// </summary>
public sealed class OwnershipIsolationTests
{
    private const string Password = "Valid-Password-123!";

    /// <summary>Sahibin kaydı yerine konan, hiç var olmamış kimlik.</summary>
    private static readonly Guid GhostId = Guid.Parse("dddddddd-dddd-4ddd-8ddd-dddddddddddd");

    [Fact]
    public async Task AnotherUsersRecord_AnswersExactlyLikeAMissingOne()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await AuthenticateAsync(factory, "isolation-owner@example.com");
        using var intruder = await AuthenticateAsync(factory, "isolation-intruder@example.com");

        var fixture = await SeedAsync(owner);
        var failures = new List<string>();

        foreach (var probe in Probes(fixture))
        {
            using var onOwnersRecord = await SendAsync(intruder, probe, probe.OwnedPath);
            using var onMissingRecord = await SendAsync(intruder, probe, probe.GhostPath);

            if (onOwnersRecord.StatusCode != onMissingRecord.StatusCode)
            {
                failures.Add(
                    $"{probe.Name}: başkasının kaydı {(int)onOwnersRecord.StatusCode}, " +
                    $"olmayan kayıt {(int)onMissingRecord.StatusCode} döndü");
                continue;
            }

            if (onOwnersRecord.StatusCode != HttpStatusCode.NotFound)
            {
                var body = await onOwnersRecord.Content.ReadAsStringAsync(CancellationToken.None);
                failures.Add(
                    $"{probe.Name}: beklenen 404, gelen {(int)onOwnersRecord.StatusCode} {body}");
            }
        }

        Assert.Empty(failures);
    }

    /// <summary>
    /// İzolasyonun öteki yarısı: kimlik taşımayan okuma uçları.
    ///
    /// Burada kimlik denemesi yoktur; sızıntı olursa sunucunun kendi listesinden
    /// gelir. Ölçü cevabın şekline bakmadan kurulur: <b>sahibin hiçbir kimliği
    /// saldırganın gövdesinde geçmemeli.</b> Şekle bakan bir kontrol (öğe sayısı,
    /// alan adı) her uç için ayrı yazılırdı ve yeni uçta unutulurdu.
    /// </summary>
    [Fact]
    public async Task ReadEndpoints_ShowNothingThatBelongsToAnotherUser()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var owner = await AuthenticateAsync(factory, "list-owner@example.com");
        using var intruder = await AuthenticateAsync(factory, "list-intruder@example.com");

        var fixture = await SeedAsync(owner);
        var ownedIds = OwnedIds(fixture);
        var failures = new List<string>();

        foreach (var path in ReadPaths(fixture))
        {
            using var response = await intruder.GetAsync(path, CancellationToken.None);

            // Saldırgan kendi verisi olmadığı için bazı uçlarda 404 alabilir;
            // ölçülen şey cevabın başarısı değil, içeriğidir.
            var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

            foreach (var (name, id) in ownedIds)
            {
                if (body.Contains(id.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{path}: sahibin {name} kimliği gövdede görünüyor");
                }
            }
        }

        Assert.Empty(failures);

        // Hiçbir şey döndürmeyen bir uç kümesi de yeşil görünürdü. Sahibin
        // aynı uçlarda kendi kimliklerini gördüğü ölçülür: görünmeyen bir kayıt
        // için "saldırgan görmüyor" demek bir şey söylemez.
        var seenByOwner = new HashSet<Guid>();
        foreach (var path in ReadPaths(fixture))
        {
            using var response = await owner.GetAsync(path, CancellationToken.None);
            var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

            foreach (var (_, id) in ownedIds)
            {
                if (body.Contains(id.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    seenByOwner.Add(id);
                }
            }
        }

        var invisible = ownedIds
            .Where(owned => !seenByOwner.Contains(owned.Id))
            .Select(owned => owned.Name)
            .ToList();

        Assert.Empty(invisible);
    }

    private static IEnumerable<string> ReadPaths(Fixture f) =>
    [
        "/api/v1/accounts",
        "/api/v1/categories",
        "/api/v1/transactions",
        "/api/v1/transfers",
        "/api/v1/budgets?year=2026&month=8",
        "/api/v1/credit-cards",
        "/api/v1/counterparties",
        "/api/v1/debts",
        "/api/v1/goals",
        "/api/v1/installment-plans",
        "/api/v1/obligations",
        "/api/v1/pos-settlements",
        "/api/v1/pos-definitions",
        "/api/v1/day-closes?from=2026-08-01&to=2026-08-31",
        "/api/v1/day-closes/day?date=2026-08-27",
        $"/api/v1/cash-counts?accountId={f.AccountId}",
        $"/api/v1/cash-counts/today?accountId={f.AccountId}",
        "/api/v1/recurring-transactions",
        "/api/v1/recurring-transactions/occurrences",
        "/api/v1/financial-activities",
        "/api/v1/financial-activities/planned",
        "/api/v1/upcoming-payments",
        "/api/v1/reports/monthly?year=2026&month=8",
        "/api/v1/reports/advanced?year=2026&month=8",
        "/api/v1/dashboard",
        "/api/v1/tax-calendar/suggestions",
        "/api/v1/taxes?asOfDate=2026-08-28",
        "/api/v1/tax-payments",
        "/api/v1/account",
        "/api/v1/account/sessions",
        "/api/v1/profile",
        "/api/v1/exports/transactions.csv",
        "/api/v1/exports/counterparty-ledger.csv",
        "/api/v1/exports/financial-data.json",
        "/api/v1/exports/download"
    ];

    private static IEnumerable<(string Name, Guid Id)> OwnedIds(Fixture f) =>
    [
        ("hesap", f.AccountId),
        ("ikinci hesap", f.SecondAccountId),
        ("gider kategorisi", f.ExpenseCategoryId),
        ("gelir kategorisi", f.IncomeCategoryId),
        ("işlem", f.TransactionId),
        ("belge", f.AttachmentId),
        ("transfer", f.TransferId),
        ("kart", f.CreditCardId),
        ("kart harcaması", f.CardChargeId),
        ("kart ödemesi", f.CardPaymentId),
        ("cari", f.CounterpartyId),
        ("cari borçlandırma", f.CounterpartyChargeId),
        ("cari tahsilat", f.CounterpartyPaymentId),
        ("bütçe", f.BudgetId),
        ("hedef", f.GoalId),
        ("borç", f.DebtId),
        ("tekrarlayan plan", f.RecurringId),
        ("vergi planı", f.TaxPlanId),
        ("vergi ödemesi", f.TaxPaymentId),
        ("occurrence", f.OccurrenceId),
        ("taksit planı", f.InstallmentPlanId),
        ("yükümlülük", f.ObligationId),
        ("POS tahsilatı", f.PosSettlementId),
        ("POS yatışı", f.PosDepositId),
        ("POS tanımı", f.PosDefinitionId),
        ("gün sonu", f.DayCloseId),
        ("kasa sayımı", f.CashCountId),
        ("içe aktarma partisi", f.ImportBatchId),
        ("oturum", f.SessionId)
    ];

    /// <summary>
    /// Yukarıdaki liste elle tutuluyor; elle tutulan liste eskir. Bu test onu
    /// uygulamanın gerçek route tablosuyla karşılaştırır: kimlik taşıyan her uç
    /// ya denetleniyordur ya da kapsam dışı olduğu <see cref="NotOwnerScoped"/>
    /// içinde <b>gerekçesiyle</b> yazılıdır.
    /// </summary>
    [Fact]
    public async Task EveryOwnerScopedRoute_IsProbed()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        var routes = factory.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText is { } text && text.Contains('{'))
            .Select(endpoint => Signature(endpoint))
            .Distinct()
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToList();

        Assert.NotEmpty(routes);

        var probed = Probes(Fixture.Placeholder)
            .Select(probe => probe.Signature)
            .ToHashSet(StringComparer.Ordinal);
        var excused = NotOwnerScoped.Keys.ToHashSet(StringComparer.Ordinal);

        var uncovered = routes
            .Where(route => !probed.Contains(route) && !excused.Contains(route))
            .ToList();

        Assert.Empty(uncovered);

        // Kapsam dışı listesi de eskir: artık var olmayan bir uç için yazılmış
        // gerekçe, denetimi olduğundan geniş gösterir.
        var stale = excused.Where(route => !routes.Contains(route)).ToList();
        Assert.Empty(stale);
    }

    /// <summary>
    /// Muhasebeci paketi kalktı (ADR 0018): ne okuma ucu ne zip dışa aktarımı
    /// route tablosunda kalır. Kalan bir uç, kaldırılmış bir ön muhasebe
    /// özelliğinin sessizce yaşaması olurdu.
    /// </summary>
    [Fact]
    public async Task AccountantPackageRoutes_AreGone()
    {
        await using var factory = new BusinessFinanceApiFactory();
        using var client = factory.CreateClient();

        var routes = factory.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText ?? string.Empty);

        Assert.DoesNotContain(
            routes,
            route => route.Contains("accountant", StringComparison.OrdinalIgnoreCase));
    }

    // ---------------------------------------------------------------------
    // Prob tablosu
    // ---------------------------------------------------------------------

    private static IEnumerable<Probe> Probes(Fixture f)
    {
        const string Today = "2026-08-28";
        var patch = new HttpMethod("PATCH");

        yield return Json("oturum kapatma", HttpMethod.Delete,
            "api/v1/account/sessions/{sessionId:guid}", null, f.SessionId);

        yield return Json("hesap okuma", HttpMethod.Get,
            "api/v1/accounts/{accountId:guid}", null, f.AccountId);
        yield return Json("hesap güncelleme", HttpMethod.Put,
            "api/v1/accounts/{accountId:guid}",
            new UpdateAccountRequest("Ele geçirilen", true), f.AccountId);
        yield return Json("hesap silme", HttpMethod.Delete,
            "api/v1/accounts/{accountId:guid}", null, f.AccountId);

        yield return Json("ek okuma", HttpMethod.Get,
            "api/v1/attachments/{attachmentId:guid}/content", null, f.AttachmentId);
        yield return Json("işlemin ekleri", HttpMethod.Get,
            "api/v1/transactions/{transactionId:guid}/attachments", null, f.TransactionId);
        yield return new Probe("işleme ek yükleme", HttpMethod.Post,
            "api/v1/transactions/{transactionId:guid}/attachments",
            $"/api/v1/transactions/{f.TransactionId}/attachments",
            $"/api/v1/transactions/{GhostId}/attachments",
            PngUpload);

        yield return Json("bütçe güncelleme", HttpMethod.Put,
            "api/v1/budgets/{budgetId:guid}",
            new UpdateBudgetRequest("999.0000", "TRY"), f.BudgetId);
        yield return Json("bütçe silme", HttpMethod.Delete,
            "api/v1/budgets/{budgetId:guid}", null, f.BudgetId);

        yield return Json("kasa farkını onaylama", HttpMethod.Post,
            "api/v1/cash-counts/{id:guid}/adjustment",
            new ConfirmCashCountDifferenceRequest(f.ExpenseCategoryId), f.CashCountId);

        yield return Json("kategori güncelleme", HttpMethod.Put,
            "api/v1/categories/{categoryId:guid}",
            new UpdateCategoryRequest("Ele geçirilen", true), f.ExpenseCategoryId);

        yield return Json("cari okuma", HttpMethod.Get,
            "api/v1/counterparties/{counterpartyId:guid}", null, f.CounterpartyId);
        yield return Json("cari güncelleme", HttpMethod.Put,
            "api/v1/counterparties/{counterpartyId:guid}",
            new UpdateCounterpartyRequest("Ele geçirilen", true), f.CounterpartyId);
        yield return Json("cari silme", HttpMethod.Delete,
            "api/v1/counterparties/{counterpartyId:guid}", null, f.CounterpartyId);
        yield return Json("cariye borçlandırma", HttpMethod.Post,
            "api/v1/counterparties/{counterpartyId:guid}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "50.0000", "TRY", f.IncomeCategoryId, Today, "business"),
            f.CounterpartyId);
        yield return Json("cariye tahsilat", HttpMethod.Post,
            "api/v1/counterparties/{counterpartyId:guid}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "50.0000", "TRY", f.AccountId, Today),
            f.CounterpartyId);
        yield return Json("cari borçlandırma iptali", HttpMethod.Delete,
            "api/v1/counterparty-charges/{chargeId:guid}", null, f.CounterpartyChargeId);
        yield return Json("cari tahsilat iptali", HttpMethod.Delete,
            "api/v1/counterparty-payments/{paymentId:guid}", null, f.CounterpartyPaymentId);

        yield return Json("kart okuma", HttpMethod.Get,
            "api/v1/credit-cards/{creditCardId:guid}", null, f.CreditCardId);
        yield return Json("kart güncelleme", HttpMethod.Put,
            "api/v1/credit-cards/{creditCardId:guid}",
            new UpdateCreditCardRequest("Ele geçirilen", "5000.0000", "TRY", 10, 20, true),
            f.CreditCardId);
        yield return Json("kart hareketleri", HttpMethod.Get,
            "api/v1/credit-cards/{creditCardId:guid}/activity", null, f.CreditCardId);
        yield return Json("karta harcama", HttpMethod.Post,
            "api/v1/credit-cards/{creditCardId:guid}/charges",
            new CreateCardChargeRequest(
                f.ExpenseCategoryId, "40.0000", "TRY", "business", Today, null),
            f.CreditCardId);
        yield return Json("karta ödeme", HttpMethod.Post,
            "api/v1/credit-cards/{creditCardId:guid}/payments",
            new CreateCardPaymentRequest(f.AccountId, "40.0000", "TRY", Today, null),
            f.CreditCardId);
        yield return Json("güncel ekstre", HttpMethod.Get,
            "api/v1/credit-cards/{creditCardId:guid}/statements/current", null, f.CreditCardId);
        yield return Json("dönem ekstresi", HttpMethod.Get,
            "api/v1/credit-cards/{creditCardId:guid}/statements/{year:int}/{month:int}",
            null, f.CreditCardId, 2026, 8);
        yield return Json("kart harcaması iptali", HttpMethod.Delete,
            "api/v1/credit-card-charges/{chargeId:guid}", null, f.CardChargeId);
        yield return Json("kart ödemesi iptali", HttpMethod.Delete,
            "api/v1/credit-card-payments/{paymentId:guid}", null, f.CardPaymentId);

        yield return Json("borcun açılışı", HttpMethod.Post,
            "api/v1/debts/{debtId:guid}/opening",
            new RecordDebtOpeningRequest("cash", f.AccountId, null, Today), f.DebtId);
        yield return Json("borç taksidi ödeme", HttpMethod.Post,
            "api/v1/debts/{debtId:guid}/installments/{sequence:int}/pay",
            new PayDebtInstallmentRequest(f.AccountId, Today, Today), f.DebtId, 1);

        yield return Json("hedefe katkı", HttpMethod.Post,
            "api/v1/goals/{goalId:guid}/contributions",
            new AddSavingsGoalContributionRequest(
                "10.0000", "TRY", Today, Guid.NewGuid(), null, Today),
            f.GoalId);
        yield return Json("hedef silme", HttpMethod.Delete,
            "api/v1/goals/{goalId:guid}", null, f.GoalId);

        yield return Json("içe aktarma partisi", HttpMethod.Get,
            "api/v1/imports/{batchId:guid}", null, f.ImportBatchId);
        yield return Json("içe aktarmayı onaylama", HttpMethod.Post,
            "api/v1/imports/{batchId:guid}/confirm",
            new ConfirmImportBatchRequest([f.ImportRowId]), f.ImportBatchId);
        yield return Json("içe aktarma satırı", patch,
            "api/v1/imports/{batchId:guid}/rows/{rowId:guid}",
            new UpdateImportCandidateRequest(
                Today, "-10.0000", null, null, f.AccountId, f.ExpenseCategoryId),
            f.ImportBatchId, f.ImportRowId);
        yield return Json("içe aktarma çift kaydı", patch,
            "api/v1/imports/{batchId:guid}/rows/{rowId:guid}/duplicate-decision",
            new ResolveImportDuplicateRequest("skip"), f.ImportBatchId, f.ImportRowId);

        yield return Json("taksit gerçekleştirme", HttpMethod.Post,
            "api/v1/installment-plans/{installmentPlanId:guid}/items/{sequence:int}/realize",
            null, f.InstallmentPlanId, 1);

        yield return Json("yükümlülüğü kapatma", HttpMethod.Post,
            "api/v1/obligations/{id:guid}/settlement",
            new SettleObligationRequest(f.AccountId, Today), f.ObligationId);
        yield return Json("yükümlülüğü iptal", HttpMethod.Delete,
            "api/v1/obligations/{id:guid}", null, f.ObligationId);

        yield return Json("POS tahsilatını iptal", HttpMethod.Delete,
            "api/v1/pos-settlements/{id:guid}", null, f.PosSettlementId);

        // İlk yol değeri kimlik değil türdür; hayalet yol elle kurulur.
        yield return new Probe("işlem sonrası bakiye", HttpMethod.Get,
            "api/v1/financial-activities/{activityKind}/{activityId:guid}/balances",
            $"/api/v1/financial-activities/account-transaction/{f.TransactionId}/balances",
            $"/api/v1/financial-activities/account-transaction/{GhostId}/balances",
            null);

        yield return Json("POS yatışını okuma", HttpMethod.Get,
            "api/v1/pos-deposits/{id:guid}", null, f.PosDepositId);
        yield return Json("POS yatışını geri alma", HttpMethod.Delete,
            "api/v1/pos-deposits/{id:guid}", null, f.PosDepositId);
        // Önizleme kimliği yolda değil sorgu dizesinde taşır; başkasının
        // tahsilatı ile olmayan tahsilat aynı cevabı vermelidir. Yatış yazan
        // POST kimliği gövdede taşıdığı için `PosDepositEndpointTests` içinde
        // ölçülür.
        yield return new Probe("POS yatışı önizlemesi", HttpMethod.Get,
            "api/v1/pos-deposits/preview",
            $"/api/v1/pos-deposits/preview?settlementIds={f.PosSettlementId}",
            $"/api/v1/pos-deposits/preview?settlementIds={GhostId}",
            null);

        // Gün sonunu yazan POST ve önizleme kimliği gövdede taşır;
        // `DayCloseEndpointTests` içinde ölçülür.
        yield return Json("gün sonunu okuma", HttpMethod.Get,
            "api/v1/day-closes/{id:guid}", null, f.DayCloseId);
        yield return Json("gün sonunu geri alma", HttpMethod.Delete,
            "api/v1/day-closes/{id:guid}", null, f.DayCloseId);

        yield return Json("POS tanımını güncelleme", HttpMethod.Put,
            "api/v1/pos-definitions/{id:guid}",
            new SavePosDefinitionRequest(
                "Ele geçirilen", f.SecondAccountId, f.IncomeCategoryId, "0.0000", 1, true),
            f.PosDefinitionId);
        yield return Json("POS tanımını pasife alma", patch,
            "api/v1/pos-definitions/{id:guid}/active",
            new SetPosDefinitionActiveRequest(false), f.PosDefinitionId);
        yield return Json("POS'u ana POS yapma", HttpMethod.Put,
            "api/v1/pos-definitions/{id:guid}/default", null, f.PosDefinitionId);
        yield return Json("POS tanımını silme", HttpMethod.Delete,
            "api/v1/pos-definitions/{id:guid}", null, f.PosDefinitionId);
        // Sorgu dizesi imzaya girmez; doğrulamayı geçip sahiplik denetimine
        // ulaşsın diye iki yola da aynı geçerli değerler eklenir.
        yield return new Probe("POS tanımıyla önizleme", HttpMethod.Get,
            "api/v1/pos-definitions/{id:guid}/preview",
            $"/api/v1/pos-definitions/{f.PosDefinitionId}/preview" +
            "?grossAmount=100.0000&settlementDate=2026-08-28",
            $"/api/v1/pos-definitions/{GhostId}/preview" +
            "?grossAmount=100.0000&settlementDate=2026-08-28",
            null);

        yield return Json("tekrarlayan planı silme", HttpMethod.Delete,
            "api/v1/recurring-transactions/{recurringTransactionId:guid}",
            null, f.RecurringId);
        yield return Json("tekrarlayan planı pasifleştirme", patch,
            "api/v1/recurring-transactions/{recurringTransactionId:guid}/active",
            new SetRecurringActiveRequest(false), f.RecurringId);
        yield return Json("planın vadesini gerçekleştirme", HttpMethod.Post,
            "api/v1/recurring-transactions/{recurringTransactionId:guid}/occurrences/realize",
            new RealizeDueRecurringRequest(Today), f.RecurringId);
        yield return Json("tek occurrence gerçekleştirme", HttpMethod.Post,
            "api/v1/recurring-transactions/occurrences/{occurrenceId:guid}/realize",
            new RealizeRecurringOccurrenceRequest(), f.OccurrenceId);
        yield return Json("tekrarlayan planı düzenleme", HttpMethod.Put,
            "api/v1/recurring-transactions/{recurringTransactionId:guid}",
            new UpdateRecurringTransactionRequest(
                f.AccountId, f.ExpenseCategoryId, "80.0000", "business", "monthly",
                "2026-08-01", null, "clamp-to-last-day", "Ele geçirilen"),
            f.RecurringId);
        yield return Json("kalemin tutarını yazma", HttpMethod.Post,
            "api/v1/recurring-transactions/{recurringTransactionId:guid}/occurrences/amount",
            new SetOccurrenceAmountRequest("2026-09-30", "999.0000"), f.TaxPlanId);
        yield return Json("kalemin ödemesini geri alma", HttpMethod.Post,
            "api/v1/recurring-transactions/occurrences/{occurrenceId:guid}/undo",
            null, f.OccurrenceId);

        yield return new Probe("vergi tanımı ayrıntısı", HttpMethod.Get,
            "api/v1/taxes/plans/{recurringTransactionId:guid}",
            $"/api/v1/taxes/plans/{f.TaxPlanId}?asOfDate={Today}",
            $"/api/v1/taxes/plans/{GhostId}?asOfDate={Today}",
            null);
        yield return Json("vergi ödemesini geri alma", HttpMethod.Post,
            "api/v1/tax-payments/{paymentId:guid}/undo", null, f.TaxPaymentId);

        yield return Json("işlem okuma", HttpMethod.Get,
            "api/v1/transactions/{transactionId:guid}", null, f.TransactionId);
        yield return Json("işlem iptali", HttpMethod.Delete,
            "api/v1/transactions/{transactionId:guid}", null, f.TransactionId);

        yield return Json("transfer okuma", HttpMethod.Get,
            "api/v1/transfers/{transferId:guid}", null, f.TransferId);
        yield return Json("transfer iptali", HttpMethod.Delete,
            "api/v1/transfers/{transferId:guid}", null, f.TransferId);
    }

    /// <summary>
    /// Yolu şablondan kurar. İlk değer <b>sahiplik taşıyan kimliktir</b>: hayalet
    /// yolda yerine <see cref="GhostId"/> geçer, geri kalanı aynı kalır. Böylece
    /// iki istek arasındaki tek fark sahiplik olur.
    /// </summary>
    private static Probe Json(
        string name,
        HttpMethod method,
        string template,
        object? body,
        params object[] values)
    {
        var ghostValues = values.ToArray();
        ghostValues[0] = GhostId;

        return new Probe(
            name,
            method,
            template,
            Fill(template, values),
            Fill(template, ghostValues),
            body is null ? null : () => JsonContent.Create(body, body.GetType()));
    }

    private static string Fill(string template, IReadOnlyList<object> values)
    {
        var path = template;
        var index = 0;

        while (path.IndexOf('{') is var open && open >= 0)
        {
            var close = path.IndexOf('}', open);
            path = path[..open] + values[index++] + path[(close + 1)..];
        }

        return '/' + path;
    }

    private static HttpContent PngUpload()
    {
        var file = new ByteArrayContent([137, 80, 78, 71, 13, 10, 26, 10]);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        return new MultipartFormDataContent { { file, "file", "fis.png" } };
    }

    /// <summary>
    /// Kimlik taşıyan ama sahiplik denetimine girmeyen uçlar ve nedenleri.
    /// Listeye yeni satır eklemek, bir ucu denetimden çıkarmaktır: gerekçe
    /// yazılmadan eklenmez.
    /// </summary>
    private static readonly Dictionary<string, string> NotOwnerScoped = new(StringComparer.Ordinal)
    {
    };


    // ---------------------------------------------------------------------
    // Sahibin kurduğu kayıtlar
    // ---------------------------------------------------------------------

    /// <summary>
    /// Denetimin dokunduğu her kayıt türünden bir tane. Kimlikler
    /// <see cref="Probes"/> içinde yola gömülür.
    /// </summary>
    private sealed record Fixture(
        Guid AccountId,
        Guid SecondAccountId,
        Guid ExpenseCategoryId,
        Guid IncomeCategoryId,
        Guid TransactionId,
        Guid AttachmentId,
        Guid TransferId,
        Guid CreditCardId,
        Guid CardChargeId,
        Guid CardPaymentId,
        Guid CounterpartyId,
        Guid CounterpartyChargeId,
        Guid CounterpartyPaymentId,
        Guid BudgetId,
        Guid GoalId,
        Guid DebtId,
        Guid RecurringId,
        Guid OccurrenceId,
        Guid TaxPlanId,
        Guid TaxPaymentId,
        Guid InstallmentPlanId,
        Guid ObligationId,
        Guid PosSettlementId,
        Guid PosDepositId,
        Guid PosDefinitionId,
        Guid DayCloseId,
        Guid CashCountId,
        Guid ImportBatchId,
        Guid ImportRowId,
        Guid SessionId)
    {
        /// <summary>
        /// Route kapsamı testi yalnız yol şablonlarını karşılaştırır; gerçek
        /// kimliklere ihtiyacı yoktur ve sunucuya hiç istek atmaz.
        /// </summary>
        public static Fixture Placeholder { get; } = new(
            GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId,
            GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId,
            GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId, GhostId,
            GhostId, GhostId, GhostId, GhostId, GhostId, GhostId);
    }

    private sealed record Probe(
        string Name,
        HttpMethod Method,
        string Template,
        string OwnedPath,
        string GhostPath,
        Func<HttpContent>? Content)
    {
        public string Signature => $"{Method.Method} /{Template}";
    }

    private static string Signature(RouteEndpoint endpoint)
    {
        var methods = endpoint.Metadata
            .GetMetadata<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
            ?.HttpMethods ?? Array.Empty<string>();
        var method = methods.Count > 0 ? methods[0] : "ANY";
        return $"{method} /{endpoint.RoutePattern.RawText?.TrimStart('/')}";
    }

    private static Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        Probe probe,
        string path)
    {
        var request = new HttpRequestMessage(probe.Method, path)
        {
            Content = probe.Content?.Invoke()
        };

        return client.SendAsync(request, CancellationToken.None);
    }

    // ---------------------------------------------------------------------
    // Sahibin verisini kurma
    // ---------------------------------------------------------------------

    /// <summary>
    /// Denetimin dokunduğu her kayıt türünden bir tane kurar. Kurulum HTTP
    /// üzerinden yapılır: veritabanına doğrudan yazmak, uçların kabul ettiği
    /// veriyle testin ürettiği veriyi ayırır ve izolasyonu olduğundan iyi
    /// gösterebilirdi.
    /// </summary>
    private static async Task<Fixture> SeedAsync(HttpClient owner)
    {
        const string Today = "2026-08-28";

        var account = await CreateAsync<AccountResponse>(owner, "/api/v1/accounts",
            new CreateAccountRequest("Kasa", "cash", "TRY", "5000.0000", "business"));
        var secondAccount = await CreateAsync<AccountResponse>(owner, "/api/v1/accounts",
            new CreateAccountRequest("Banka", "bank", "TRY", "5000.0000", "business"));
        var expense = await CreateAsync<CategoryResponse>(owner, "/api/v1/categories",
            new CreateCategoryRequest("Gider", "expense", "business"));
        var income = await CreateAsync<CategoryResponse>(owner, "/api/v1/categories",
            new CreateCategoryRequest("Gelir", "income", "business"));

        var transaction = await CreateAsync<TransactionResponse>(owner, "/api/v1/transactions",
            new CreateTransactionRequest(
                account.Id, expense.Id, "100.0000", "TRY", "expense", "business",
                Today, "Sentetik"));

        using var upload = await owner.PostAsync(
            $"/api/v1/transactions/{transaction.Id}/attachments",
            PngUpload(),
            CancellationToken.None);
        upload.EnsureSuccessStatusCode();
        var attachment = (await upload.Content.ReadFromJsonAsync<AttachmentResponse>(
            CancellationToken.None))!;

        var transfer = await CreateAsync<TransferResponse>(owner, "/api/v1/transfers",
            new CreateTransferRequest(
                account.Id, secondAccount.Id, "50.0000", "TRY", Today, null));

        var card = await CreateAsync<CreditCardResponse>(owner, "/api/v1/credit-cards",
            new CreateCreditCardRequest(
                "Kart", "10000.0000", "TRY", 10, 20, DefaultScope: "business"));
        var cardCharge = await CreateAsync<CardChargeResponse>(
            owner, $"/api/v1/credit-cards/{card.Id}/charges",
            new CreateCardChargeRequest(
                expense.Id, "120.0000", "TRY", "business", Today, "Sentetik"));
        var cardPayment = await CreateAsync<CardPaymentResponse>(
            owner, $"/api/v1/credit-cards/{card.Id}/payments",
            new CreateCardPaymentRequest(account.Id, "60.0000", "TRY", Today, null));

        var counterparty = await CreateAsync<CounterpartyResponse>(
            owner, "/api/v1/counterparties",
            new CreateCounterpartyRequest("Sentetik Cari"));
        var counterpartyCharge = await CreateAsync<CounterpartyChargeResponse>(
            owner, $"/api/v1/counterparties/{counterparty.Id}/charges",
            new CreateCounterpartyChargeRequest(
                "receivable", "200.0000", "TRY", income.Id, Today, "business"));
        var counterpartyPayment = await CreateAsync<CounterpartyPaymentResponse>(
            owner, $"/api/v1/counterparties/{counterparty.Id}/payments",
            new CreateCounterpartyPaymentRequest(
                "receivable", "40.0000", "TRY", account.Id, Today));

        var budget = await CreateAsync<BudgetResponse>(owner, "/api/v1/budgets",
            new CreateBudgetRequest(expense.Id, "1000.0000", "TRY", "business", 2026, 8));

        var goal = await CreateAsync<SavingsGoalResponse>(owner, "/api/v1/goals",
            new CreateSavingsGoalRequest(
                "Sentetik hedef", "1000.0000", "TRY", "2026-12-31", "manual-contributions",
                null, null, Today, "business"));

        var debt = await CreateAsync<DebtResponse>(owner, "/api/v1/debts",
            new CreateDebtRequest(
                "Sentetik Alacaklı", "payable", "business", "1200.0000", "1200.0000", null,
                "TRY", "cash", account.Id, null, "2026-08-01", "2026-09-28", 3, null, Today));

        var recurring = await CreateAsync<RecurringTransactionResponse>(
            owner, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                account.Id, expense.Id, "75.0000", "TRY", "expense", "business",
                "monthly", "2026-08-01", null, "clamp-to-last-day", "Sentetik plan"));

        using var generate = await owner.PostAsJsonAsync(
            "/api/v1/recurring-transactions/occurrences/generate",
            new GenerateRecurringOccurrencesRequest(Today),
            CancellationToken.None);
        generate.EnsureSuccessStatusCode();
        var occurrences = await owner.GetFromJsonAsync<RecurringOccurrenceListResponse>(
            "/api/v1/recurring-transactions/occurrences",
            CancellationToken.None);

        // Vergi planı ve vergi ödemesi kişisel setin vergi işaretli
        // kategorisine yazılır (ADR 0018 T6).
        var categories = await owner.GetFromJsonAsync<CategoryListResponse>(
            "/api/v1/categories?type=expense",
            CancellationToken.None);
        var taxCategory = categories!.Items.First(item => item.IsTax);
        var taxPlan = await CreateAsync<RecurringTransactionResponse>(
            owner, "/api/v1/recurring-transactions",
            new CreateRecurringTransactionRequest(
                null, taxCategory.Id, null, "TRY", "expense", null,
                "monthly", "2026-08-31", null, "clamp-to-last-day", "Bağkur",
                TaxKind: "social-security-premium", DayOfMonth: 31));
        var taxPayment = await CreateAsync<TaxPaymentResponse>(
            owner, "/api/v1/tax-payments",
            new CreateTaxPaymentRequest(
                Guid.NewGuid(), "1500.0000", Today, taxCategory.Id, AccountId: account.Id));

        var installmentPlan = await CreateAsync<InstallmentPlanResponse>(
            owner, "/api/v1/installment-plans",
            new CreateInstallmentPlanRequest(
                card.Id, expense.Id, Guid.NewGuid(), "600.0000", "TRY", "business",
                3, "2026-09-01", "Sentetik taksit"));

        var obligation = await CreateAsync<ObligationResponse>(owner, "/api/v1/obligations",
            new CreateObligationRequest(
                "payable", "300.0000", "TRY", expense.Id, Today, "2026-09-15", "business"));

        var pos = await CreateAsync<PosSettlementResponse>(owner, "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                secondAccount.Id, income.Id, "500.0000", "TRY", Today, "2026-08-30",
                "10.0000", null, expense.Id, "business"));

        // Yatış kendi tahsilatını kapatır: yukarıdaki tahsilat yolda kalmalı
        // ki iptal probu sahiplikten başka bir nedenle reddedilmesin.
        var depositedPos = await CreateAsync<PosSettlementResponse>(
            owner, "/api/v1/pos-settlements",
            new CreatePosSettlementRequest(
                secondAccount.Id, income.Id, "300.0000", "TRY", Today, "2026-08-30",
                "6.0000", null, expense.Id, "business"));
        var posDeposit = await CreateAsync<PosDepositResponse>(owner, "/api/v1/pos-deposits",
            new CreatePosDepositRequest(
                Guid.NewGuid(), [depositedPos.Id], "294.0000", Today));

        var posDefinition = await CreateAsync<PosDefinitionResponse>(
            owner, "/api/v1/pos-definitions",
            new SavePosDefinitionRequest(
                "Sentetik POS", secondAccount.Id, income.Id, "0.0150", 1, true, expense.Id));

        // Gün sonu bir önceki güne yazılır: o gün tek tek girilmiş kayıt yok,
        // düşülecek bir şey de yok.
        var dayClose = await CreateAsync<DayCloseResponse>(owner, "/api/v1/day-closes",
            new DayCloseRequest(
                "2026-08-27", CashAmount: "10.0000", CashAccountId: account.Id,
                CashCategoryId: income.Id, ClientRequestId: Guid.NewGuid()));

        var cashCount = await CreateAsync<CashCountResponse>(owner, "/api/v1/cash-counts",
            new CreateCashCountRequest(account.Id, "4000.0000", Today, "business"));

        using var stage = await owner.PostAsync(
            "/api/v1/imports/csv/stage",
            CsvUpload(),
            CancellationToken.None);
        stage.EnsureSuccessStatusCode();
        var batch = (await stage.Content.ReadFromJsonAsync<ImportBatchResponse>(
            CancellationToken.None))!;

        var sessions = await owner.GetFromJsonAsync<UserSessionListResponse>(
            "/api/v1/account/sessions",
            CancellationToken.None);

        return new Fixture(
            account.Id,
            secondAccount.Id,
            expense.Id,
            income.Id,
            transaction.Id,
            attachment.Id,
            transfer.Id,
            card.Id,
            cardCharge.Id,
            cardPayment.Id,
            counterparty.Id,
            counterpartyCharge.Id,
            counterpartyPayment.Id,
            budget.Id,
            goal.Id,
            debt.Id,
            recurring.Id,
            occurrences!.Items[0].Id,
            taxPlan.Id,
            taxPayment.PaymentId,
            installmentPlan.Id,
            obligation.Id,
            pos.Id,
            posDeposit.Id,
            posDefinition.Id,
            dayClose.Id,
            cashCount.Id,
            batch.Id,
            batch.Rows[0].Id,
            sessions!.Items[0].SessionId);
    }

    private static async Task<TResponse> CreateAsync<TResponse>(
        HttpClient client,
        string path,
        object request)
    {
        using var response = await client.PostAsync(
            path,
            JsonContent.Create(request, request.GetType()),
            CancellationToken.None);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"{path} kurulamadı: {(int)response.StatusCode} " +
                await response.Content.ReadAsStringAsync(CancellationToken.None));
        }

        return (await response.Content.ReadFromJsonAsync<TResponse>(
            CancellationToken.None))!;
    }

    private static MultipartFormDataContent CsvUpload()
    {
        var csv = new ByteArrayContent(
            System.Text.Encoding.UTF8.GetBytes("Tarih;Tutar;Açıklama\n2026-08-11;-25,5000;Market"));
        csv.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");

        return new MultipartFormDataContent
        {
            { csv, "file", "ekstre.csv" },
            { new StringContent("Tarih"), "dateColumn" },
            { new StringContent("Tutar"), "amountColumn" },
            { new StringContent("Açıklama"), "descriptionColumn" },
            { new StringContent("semicolon"), "delimiter" },
            { new StringContent(","), "decimalSeparator" }
        };
    }

    private static async Task<HttpClient> AuthenticateAsync(
        BusinessFinanceApiFactory factory,
        string email)
    {
        var client = factory.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(email, Password, HasBusiness: true),
            CancellationToken.None);
        register.EnsureSuccessStatusCode();

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, Password),
            CancellationToken.None);
        login.EnsureSuccessStatusCode();

        var tokens = await login.Content.ReadFromJsonAsync<TokenPairResponse>(
            CancellationToken.None) ??
            throw new InvalidOperationException("Token response was empty.");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }
}
