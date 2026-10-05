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
/// bir tutar yazardı. Kural tek yerdedir: yazılan nakit = nakit − işaretli
/// nakit kayıtlar; yazılan kart = kart − işaretli kartlı kayıtlar
/// (ADR 0019 T2).
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
    public decimal TotalComputed { get; init; }
    public decimal? TotalDifference { get; init; }

    /// <summary>Bu girdiyle kayıt reddedilecekse nedeni; boşsa yazılabilir.</summary>
    public ApplicationError? Blocker { get; init; }

    internal sealed class CashLine
    {
        public bool Stated { get; set; }
        public decimal Entered { get; set; }
        public bool IsComputed { get; set; }
        public decimal Deducted { get; set; }
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
        public bool IsComputed { get; set; }
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
        DateOnly today,
        IDayCloseRepository repository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        if (input.Date == default || input.Date > today ||
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
        ResolveAmounts(input, cash, lines, mainLine, Block);

        var existing = await repository.ListExistingRecordsAsync(
            userId, firstDay, input.Date, cancellationToken);
        // Ek gün sonunda (ikinci cihaz) hiçbir kayıt işaretli gelmez: tek tek
        // girilmiş kayıtlar günün ilk gün sonunda zaten düşüldü; yeniden
        // düşmek aynı satışı iki kez eksiltirdi.
        var records = existing
            .Select(record => record with
            {
                IncludedByDefault = record.IncludedByDefault && !input.IsAdditional,
            })
            .Select(record => record with
            {
                Included = input.RecordOverrides
                    .LastOrDefault(item => item.Kind == record.Kind && item.Id == record.Id)
                    ?.Included ?? record.IncludedByDefault,
            })
            .ToArray();
        var counted = Deduct(records, cash, lines, Block);

        await ResolveCashTargetAsync(
            userId, input, cash, mainLine, repository, accountRepository,
            categoryRepository, Block, cancellationToken);
        await ResolvePosTargetsAsync(
            userId, lines, accountRepository, categoryRepository, Block, cancellationToken);

        var totalComputed = (cash.Stated ? cash.Entered : 0m) +
            lines.Where(line => line.Stated).Sum(line => line.Entered);
        return (new DayClosePlan
        {
            Input = input,
            ClosedBy = closedBy,
            Cash = cash,
            PosLines = lines,
            Records = records,
            Counted = counted,
            TotalComputed = totalComputed,
            TotalDifference = input.TotalAmount is decimal total && total != totalComputed
                ? total - totalComputed
                : null,
            Blocker = blocker,
        }, null);
    }

    /// <summary>
    /// Nakit, kart ve toplamdan ikisi yeter; eksik olan hesaplanır. Toplamdan
    /// hesaplanan kart tutarı ana POS'a yazılır.
    /// </summary>
    private static void ResolveAmounts(
        DayCloseInput input,
        CashLine cash,
        List<PosLine> lines,
        PosLine? mainLine,
        Action<ApplicationError> block)
    {
        var cardStated = lines.Any(line => line.Stated);
        if (input.CashAmount is decimal cashAmount)
        {
            cash.Stated = true;
            cash.Entered = cashAmount;
            if (cardStated || input.TotalAmount is not decimal total)
            {
                return;
            }

            var card = total - cashAmount;
            if (card < 0m)
            {
                block(DayCloseErrors.TotalBelowParts);
            }
            else if (card > 0m)
            {
                if (mainLine is null)
                {
                    block(DayCloseErrors.PosRequired);
                    return;
                }

                mainLine.Stated = true;
                mainLine.Entered = card;
                mainLine.IsComputed = true;
            }

            return;
        }

        if (!cardStated)
        {
            block(DayCloseErrors.AmountsRequired);
            return;
        }

        if (input.TotalAmount is decimal totalAmount)
        {
            var computedCash = totalAmount - lines.Sum(line => line.Entered);
            if (computedCash < 0m)
            {
                block(DayCloseErrors.TotalBelowParts);
                return;
            }

            cash.Stated = true;
            cash.Entered = computedCash;
            cash.IsComputed = true;
        }
    }

    /// <summary>
    /// İşaretli kayıtları kendi taraflarından düşer. Kartlı kayıt kendi POS
    /// satırından düşer; o satıra tutar yazılmadıysa ana POS'un satırından.
    /// Tutarı verilmemiş taraftan hiçbir şey düşülmez.
    /// </summary>
    private static List<DayCloseExistingRecordDto> Deduct(
        IReadOnlyList<DayCloseExistingRecordDto> records,
        CashLine cash,
        List<PosLine> lines,
        Action<ApplicationError> block)
    {
        var counted = new List<DayCloseExistingRecordDto>();
        if (cash.Stated)
        {
            counted.AddRange(records.Where(
                record => record.Included && record.Side == DayCloseSide.Cash));
            cash.Deducted = counted.Sum(record => record.Amount);
            if (cash.ToWrite < 0m)
            {
                block(DayCloseErrors.ExistingExceedsCash);
            }
        }

        var stated = lines.Where(line => line.Stated).ToArray();
        var fallback = stated.FirstOrDefault(line => line.Definition.IsDefault)
            ?? stated.FirstOrDefault();
        foreach (var record in records.Where(
                     record => record.Included && record.Side == DayCloseSide.Card))
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

        return counted;
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

        if (cash.Account is not null && cash.Category is not null)
        {
            cash.Scope = TransactionScopeResolution.Resolve(
                null, cash.Account.DefaultScope, cash.Category.DefaultScope);
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
            block(DayCloseErrors.ScopeUnresolved);
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

            line.Scope = TransactionScopeResolution.Resolve(
                null, line.Account.DefaultScope, line.SalesCategory!.DefaultScope);
            if (line.Scope is null)
            {
                block(DayCloseErrors.ScopeUnresolved);
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
            Cash.IsComputed,
            Cash.Deducted,
            Math.Max(Cash.ToWrite, 0m),
            Cash.Account?.Id,
            Cash.Account?.Name,
            Cash.Category?.Id,
            Cash.Category?.Name),
        PosLines
            .Select(line => new DayClosePosLineDto(
                line.Definition.Id,
                line.Definition.Name,
                line.Definition.IsDefault,
                line.Account?.Name ?? string.Empty,
                line.Stated,
                line.Entered,
                line.IsComputed,
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
        Blocker);
}
