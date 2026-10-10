using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;
using BusinessFinance.Application.Categories;
using BusinessFinance.Application.Scopes;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.DayCloses;

/// <summary>
/// Gün sonunun hesabı: girilen tutarlardan, o gün zaten girilmiş kayıtlar
/// düşülerek, hangi kaydın hangi tutarla yazılacağı.
/// </summary>
/// <remarks>
/// Önizleme ve kayıt <b>aynı planı</b> kurar. İkisi ayrı hesaplasaydı panel
/// kaydedilemeyecek bir girdiyi geçerli gösterir ya da gösterdiğinden farklı
/// bir tutar yazardı. Kural tek yerdedir: yazılan nakit = nakit − dahil
/// edilen nakit kayıtlar + ortak tutarlar; yazılan kart = kart − işaretli
/// kartlı kayıtlar (ADR 0019 T2).
///
/// Vadeli satış ve nakit tahsilat hazır cevapla gelmez; satışı da tahsilatı da
/// dahil edilen grubun ortak tutarı sorulur. Sunucu ikisinde de cevap
/// uydurmaz: cevap yoksa plan engellidir.
/// </remarks>
internal sealed class DayClosePlan
{
    public required DayCloseInput Input { get; init; }
    public required IReadOnlyList<DayCloseSummaryDto> ClosedBy { get; init; }
    public required CashLine Cash { get; init; }
    public required IReadOnlyList<PosLine> PosLines { get; init; }
    public required IReadOnlyList<DayCloseExistingRecordDto> Records { get; init; }

    /// <summary>
    /// Tutardan gerçekten düşülen kayıtlar: gün sonu bunları sahiplenir.
    /// İşaretli ama tarafına tutar yazılmamış kayıt burada yoktur.
    /// </summary>
    public required IReadOnlyList<DayCloseExistingRecordDto> Counted { get; init; }

    /// <summary>Ortak tutarı sorulan gruplar; cevaplanmış ya da cevapsız.</summary>
    public required IReadOnlyList<DayCloseOverlapGroupDto> OverlapGroups { get; init; }
    public decimal TotalComputed { get; init; }
    public decimal? TotalDifference { get; init; }

    /// <summary>Bu girdiyle kayıt reddedilecekse nedeni; boşsa yazılabilir.</summary>
    public ApplicationError? Blocker { get; init; }

    internal sealed class CashLine
    {
        public bool Stated { get; set; }
        public decimal Entered { get; set; }
        public decimal Deducted { get; set; }

        /// <summary>Düşülenin dökümü; nakit yazılmadıysa hepsi sıfırdır.</summary>
        public DayCloseCashDeductionsDto Deductions { get; set; } = new(0m, 0m, 0m, 0m, 0m);
        public decimal ToWrite => Stated ? Entered - Deducted : 0m;
        public Account? Account { get; set; }
        public Category? Category { get; set; }
        public TransactionScope? Scope { get; set; }
    }

    internal sealed class PosLine(PosDefinition definition)
    {
        public PosDefinition Definition { get; } = definition;
        public Account? Account { get; set; }
        public Category? SalesCategory { get; set; }
        public Category? CommissionCategory { get; set; }
        public bool Stated { get; set; }
        public decimal Entered { get; set; }
        public decimal Deducted { get; set; }
        public decimal ToWrite => Stated ? Entered - Deducted : 0m;
        public decimal Commission { get; set; }
        public TransactionScope? Scope { get; set; }
    }

    /// <summary>
    /// Planı kurar. Girdi baştan anlamsızsa (geçersiz gün, eksi tutar, yabancı
    /// POS) plan yerine hata döner; panelin gösterip düzeltebileceği her şey
    /// planın <see cref="Blocker"/> alanında kalır.
    /// </summary>
    public static async Task<(DayClosePlan? Plan, ApplicationError? Error)> BuildAsync(
        Guid userId,
        DayCloseInput input,
        DateOnly latestAllowedDay,
        IDayCloseRepository repository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        // Gün kullanıcının takvimiyle gelir ve sunucunun UTC gününden bir gün
        // ileride olabilir (`LocalDay`).
        if (input.Date == default || input.Date > latestAllowedDay ||
            input.RangeStart is DateOnly rangeStart && rangeStart >= input.Date)
        {
            return (null, DayCloseErrors.InvalidDate);
        }

        if (input.ZNumber is <= 0)
        {
            return (null, DayCloseErrors.InvalidZNumber);
        }

        if (input.CashAmount is < 0m || input.TotalAmount is < 0m ||
            input.PosAmounts.Any(amount => amount.Amount < 0m) ||
            input.PosAmounts.Select(amount => amount.PosDefinitionId).Distinct().Count()
                != input.PosAmounts.Count)
        {
            return (null, DayCloseErrors.InvalidAmount);
        }

        var overlaps = input.Overlaps ?? [];
        if (overlaps.Select(overlap => overlap.GroupId).Distinct().Count() != overlaps.Count)
        {
            return (null, DayCloseErrors.InvalidOverlap);
        }

        var definitions = await repository.ListActivePosDefinitionsAsync(userId, cancellationToken);
        var lines = definitions.Select(definition => new PosLine(definition)).ToList();
        foreach (var amount in input.PosAmounts)
        {
            if (lines.FirstOrDefault(line => line.Definition.Id == amount.PosDefinitionId)
                is not PosLine line)
            {
                return (null, DayCloseErrors.PosUnavailable);
            }

            line.Stated = true;
            line.Entered = amount.Amount;
        }

        ApplicationError? blocker = null;
        void Block(ApplicationError error) => blocker ??= error;

        var firstDay = input.RangeStart ?? input.Date;
        var closedBy = await repository.ListCoveringAsync(
            userId, firstDay, input.Date, cancellationToken);
        if (!input.IsAdditional && closedBy.Count > 0)
        {
            Block(DayCloseErrors.AlreadyClosed);
        }
        else if (input.IsAdditional && closedBy.Count == 0)
        {
            Block(DayCloseErrors.NotClosedYet);
        }

        if (input.ZNumber is int zNumber &&
            await repository.ZNumberExistsAsync(userId, zNumber, cancellationToken))
        {
            Block(DayCloseErrors.ZNumberExists);
        }

        var mainLine = lines.FirstOrDefault(line => line.Definition.IsDefault)
            ?? lines.FirstOrDefault();
        var cash = new CashLine();
        ResolveAmounts(input, cash, lines, Block);

        var existing = await repository.ListExistingRecordsAsync(
            userId, firstDay, input.Date, cancellationToken);
        // Cevap yalnız görülen kayıt içindir: listede artık olmayan bir kayıt
        // için cevap taşıyan istek eski bir listeye bakıyordur.
        if (input.RecordOverrides.Any(item =>
                !existing.Any(record => record.Kind == item.Kind && record.Id == item.Id)))
        {
            Block(DayCloseErrors.RecordsChanged);
        }

        // Ek gün sonunda (ikinci cihaz) hiçbir kayıt işaretli gelmez: tek tek
        // girilmiş kayıtlar günün ilk gün sonunda zaten düşüldü; yeniden
        // düşmek aynı satışı iki kez eksiltirdi. Cevap isteyen kayıtlar hiçbir
        // gün sonunda hazır cevapla gelmez.
        var records = existing
            .Select(record =>
            {
                var answer = input.RecordOverrides
                    .LastOrDefault(item => item.Kind == record.Kind && item.Id == record.Id)
                    ?.Included;
                if (record.RequiresAnswer)
                {
                    return record with { IncludedByDefault = false, Included = answer };
                }

                var byDefault = record.IncludedByDefault && !input.IsAdditional;
                return record with { IncludedByDefault = byDefault, Included = answer ?? byDefault };
            })
            .ToArray();
        var (counted, overlapGroups) = Deduct(records, overlaps, cash, lines, Block);

        await ResolveCashTargetAsync(
            userId, input, cash, mainLine, repository, accountRepository,
            categoryRepository, Block, cancellationToken);
        await ResolvePosTargetsAsync(
            userId, lines, accountRepository, categoryRepository, Block, cancellationToken);

        var totalComputed = (cash.Stated ? cash.Entered : 0m) +
            lines.Where(line => line.Stated).Sum(line => line.Entered);
        if (input.TotalAmount is decimal stated && stated < totalComputed)
        {
            Block(DayCloseErrors.TotalBelowParts);
        }

        return (new DayClosePlan
        {
            Input = input,
            ClosedBy = closedBy,
            Cash = cash,
            PosLines = lines,
            Records = records,
            Counted = counted,
            OverlapGroups = overlapGroups,
            TotalComputed = totalComputed,
            TotalDifference = input.TotalAmount is decimal total && total != totalComputed
                ? total - totalComputed
                : null,
            Blocker = blocker,
        }, null);
    }

    /// <summary>
    /// Bir taraf yalnız kendi tutarı yazıldıysa kayıt üretir. Toplamdan eksik
    /// taraf <b>hesaplanmaz</b>: raporun toplamı kredili satış ya da yemek
    /// kartı gibi başka ödeme türlerini içerebilir ve farkı nakde ya da karta
    /// yazmak olmayan bir parayı kaydederdi.
    /// </summary>
    private static void ResolveAmounts(
        DayCloseInput input,
        CashLine cash,
        List<PosLine> lines,
        Action<ApplicationError> block)
    {
        if (input.CashAmount is decimal cashAmount)
        {
            cash.Stated = true;
            cash.Entered = cashAmount;
        }
        else if (!lines.Any(line => line.Stated))
        {
            block(DayCloseErrors.AmountsRequired);
        }
    }

    /// <summary>
    /// Dahil edilen kayıtları kendi taraflarından düşer. Kartlı kayıt kendi POS
    /// satırından düşer; o satıra tutar yazılmadıysa ana POS'un satırından.
    /// Tutarı verilmemiş taraftan hiçbir şey düşülmez ve o tarafın kayıtları
    /// için cevap da istenmez.
    /// </summary>
    /// <remarks>
    /// Nakit tarafında satışı da tahsilatı da dahil edilen grubun (kişi ya da
    /// fatura) ortak tutarı bir kez çıkarılır: düşülen = satışlar + tahsilatlar
    /// − ortak tutar. Ortak tutar girilen toplamda ikisinin nasıl sayıldığını
    /// söyler ve cevabı kullanıcı verir.
    /// </remarks>
    private static (List<DayCloseExistingRecordDto> Counted, List<DayCloseOverlapGroupDto> Groups)
        Deduct(
            IReadOnlyList<DayCloseExistingRecordDto> records,
            IReadOnlyList<DayCloseOverlap> overlaps,
            CashLine cash,
            List<PosLine> lines,
            Action<ApplicationError> block)
    {
        var counted = new List<DayCloseExistingRecordDto>();
        var groups = new List<DayCloseOverlapGroupDto>();
        if (cash.Stated)
        {
            var cashRecords = records
                .Where(record => record.Side == DayCloseSide.Cash)
                .ToArray();
            if (cashRecords.Any(record => record.RequiresAnswer && record.Included is null))
            {
                block(DayCloseErrors.RecordsUnanswered);
            }

            counted.AddRange(cashRecords.Where(record => record.Included == true));
            var shared = 0m;
            foreach (var group in counted
                         .Where(record => record.GroupId is not null)
                         .GroupBy(record => record.GroupId!.Value))
            {
                var sales = group.Where(record => record.IsDeferredSale).Sum(record => record.Amount);
                var collections = group.Where(record => record.IsCollection).Sum(record => record.Amount);
                if (sales <= 0m || collections <= 0m)
                {
                    continue;
                }

                var maximum = Math.Min(sales, collections);
                var answer = overlaps.FirstOrDefault(overlap => overlap.GroupId == group.Key)?.Amount;
                if (answer is null)
                {
                    block(DayCloseErrors.OverlapUnanswered);
                }
                else if (answer < 0m || answer > maximum)
                {
                    block(DayCloseErrors.InvalidOverlap);
                    answer = null;
                }

                shared += answer ?? 0m;
                groups.Add(DayCloseOverlapGroupDto.From(group.Key, group.ToArray(), answer));
            }

            decimal Sum(Func<DayCloseExistingRecordDto, bool> filter) =>
                counted.Where(filter).Sum(record => record.Amount);
            cash.Deductions = new DayCloseCashDeductionsDto(
                Sum(record => record.Kind == DayCloseRecordKind.Income),
                Sum(record => record.IsCollection),
                Sum(record => record.Kind == DayCloseRecordKind.CounterpartyCharge),
                Sum(record => record.Kind == DayCloseRecordKind.Obligation),
                shared);
            cash.Deducted = counted.Sum(record => record.Amount) - shared;
            if (cash.ToWrite < 0m)
            {
                block(DayCloseErrors.ExistingExceedsCash);
            }
        }

        // Ortak tutar yalnız sorulan grup için verilir.
        if (overlaps.Any(overlap => groups.All(group => group.GroupId != overlap.GroupId)))
        {
            block(DayCloseErrors.InvalidOverlap);
        }

        var stated = lines.Where(line => line.Stated).ToArray();
        var fallback = stated.FirstOrDefault(line => line.Definition.IsDefault)
            ?? stated.FirstOrDefault();
        foreach (var record in records.Where(
                     record => record.Included == true && record.Side == DayCloseSide.Card))
        {
            var target = stated.FirstOrDefault(
                line => line.Definition.Id == record.PosDefinitionId) ?? fallback;
            if (target is not null)
            {
                target.Deducted += record.Amount;
                counted.Add(record);
            }
        }

        if (stated.Any(line => line.ToWrite < 0m))
        {
            block(DayCloseErrors.ExistingExceedsCard);
        }

        return (counted, groups);
    }

    /// <summary>
    /// Nakit satışın yazılacağı kasa ve kategori: açık seçim → son gün
    /// sonununki → tek nakit hesap / ana POS'un satış kategorisi. Sunucu
    /// birden çok aday arasından seçmez.
    /// </summary>
    private static async Task ResolveCashTargetAsync(
        Guid userId,
        DayCloseInput input,
        CashLine cash,
        PosLine? mainLine,
        IDayCloseRepository repository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        Action<ApplicationError> block,
        CancellationToken cancellationToken)
    {
        var defaults = await repository.GetDefaultsAsync(userId, cancellationToken);

        if (input.CashAccountId is Guid requestedAccountId)
        {
            var account = await accountRepository.FindOwnedByIdAsync(
                requestedAccountId, userId, cancellationToken);
            if (IsCashAccount(account))
            {
                cash.Account = account;
            }
            else
            {
                block(DayCloseErrors.CashAccountUnavailable);
            }
        }
        else
        {
            var candidateId = defaults.LastCashAccountId is Guid last &&
                              defaults.CashAccountIds.Contains(last)
                ? last
                : defaults.CashAccountIds.Count == 1 ? defaults.CashAccountIds[0] : (Guid?)null;
            if (candidateId is Guid id)
            {
                var account = await accountRepository.FindOwnedByIdAsync(
                    id, userId, cancellationToken);
                cash.Account = IsCashAccount(account) ? account : null;
            }
        }

        if (input.CashCategoryId is Guid requestedCategoryId)
        {
            var category = await categoryRepository.FindOwnedByIdAsync(
                requestedCategoryId, userId, cancellationToken);
            if (IsIncomeCategory(category))
            {
                cash.Category = category;
            }
            else
            {
                block(DayCloseErrors.CashCategoryUnavailable);
            }
        }
        else
        {
            foreach (var candidateId in new[]
                     {
                         defaults.LastCashCategoryId, mainLine?.Definition.SalesCategoryId,
                     })
            {
                if (candidateId is not Guid id)
                {
                    continue;
                }

                var category = await categoryRepository.FindOwnedByIdAsync(
                    id, userId, cancellationToken);
                if (IsIncomeCategory(category))
                {
                    cash.Category = category;
                    break;
                }
            }
        }

        // Gün sonu işletme satışıdır (ADR 0020 T2): kasanın etiketi sayılmaz;
        // şahsi kategoride taraf boş kalır ve aşağıda çelişki olarak engellenir.
        if (cash.Account is not null && cash.Category is not null)
        {
            cash.Scope = TransactionScopeResolution.ResolveInContext(
                TransactionScope.Business, null, cash.Category.DefaultScope).Scope;
        }

        if (cash.ToWrite <= 0m)
        {
            return;
        }

        if (cash.Account is null)
        {
            block(DayCloseErrors.CashAccountRequired);
        }
        else if (cash.Category is null)
        {
            block(DayCloseErrors.CashCategoryRequired);
        }
        else if (cash.Scope is null)
        {
            block(DayCloseErrors.ScopeConflict);
        }
    }

    /// <summary>
    /// Her POS satırının hesabını, kategorilerini, komisyonunu ve kapsamını
    /// POS'tan çözer; tahsilatı tek tek yazan yolla aynı kurallar.
    /// </summary>
    private static async Task ResolvePosTargetsAsync(
        Guid userId,
        List<PosLine> lines,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        Action<ApplicationError> block,
        CancellationToken cancellationToken)
    {
        foreach (var line in lines)
        {
            var definition = line.Definition;
            line.Account = await accountRepository.FindOwnedByIdAsync(
                definition.AccountId, userId, cancellationToken);
            line.SalesCategory = await categoryRepository.FindOwnedByIdAsync(
                definition.SalesCategoryId, userId, cancellationToken);
            if (line.ToWrite <= 0m)
            {
                continue;
            }

            if (line.Account is not { IsActive: true, Type: AccountType.Bank } ||
                !IsIncomeCategory(line.SalesCategory))
            {
                block(DayCloseErrors.PosUnusable);
                continue;
            }

            line.Commission = PosSettlement.CommissionFromRate(
                new Money(line.ToWrite, line.Account.Currency), definition.CommissionRate);
            if (line.Commission > 0m)
            {
                line.CommissionCategory = definition.CommissionCategoryId is Guid categoryId
                    ? await categoryRepository.FindOwnedByIdAsync(
                        categoryId, userId, cancellationToken)
                    : null;
                if (line.CommissionCategory is not { IsActive: true, Type: CategoryType.Expense })
                {
                    block(DayCloseErrors.PosUnusable);
                    continue;
                }
            }

            var resolution = TransactionScopeResolution.ResolveInContext(
                TransactionScope.Business, null, line.SalesCategory!.DefaultScope);
            if (resolution.Scope is TransactionScope && line.CommissionCategory is not null)
            {
                resolution = TransactionScopeResolution.ResolveInContext(
                    TransactionScope.Business, null, line.CommissionCategory.DefaultScope);
            }

            line.Scope = resolution.Scope;
            if (line.Scope is null)
            {
                block(DayCloseErrors.ScopeConflict);
            }
        }
    }

    private static bool IsCashAccount(Account? account) =>
        account is { IsActive: true, Type: AccountType.Cash };

    private static bool IsIncomeCategory(Category? category) =>
        category is { IsActive: true, Type: CategoryType.Income };

    public DayClosePreviewDto ToPreview() => new(
        Input.Date,
        Input.RangeStart,
        Cash.Account?.Currency ?? CurrencyCode.TRY,
        ClosedBy,
        new DayCloseCashLineDto(
            Cash.Stated,
            Cash.Entered,
            Cash.Deducted,
            Math.Max(Cash.ToWrite, 0m),
            Cash.Account?.Id,
            Cash.Account?.Name,
            Cash.Category?.Id,
            Cash.Category?.Name,
            Cash.Deductions),
        PosLines
            .Select(line => new DayClosePosLineDto(
                line.Definition.Id,
                line.Definition.Name,
                line.Definition.IsDefault,
                line.Account?.Name ?? string.Empty,
                line.Stated,
                line.Entered,
                line.Deducted,
                Math.Max(line.ToWrite, 0m),
                line.Commission,
                Math.Max(line.ToWrite, 0m) - line.Commission,
                line.Definition.ExpectedTransferDate(Input.Date)))
            .ToArray(),
        Input.TotalAmount,
        TotalComputed,
        TotalDifference,
        Records,
        Blocker,
        OverlapGroups);
}
