using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Application.DayCloses;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.DayCloses;

public sealed record DayClosePosAmountRequest(Guid PosDefinitionId, string Amount);

/// <summary>
/// Bir "zaten girilmiş" kayıt için varsayılandan farklı seçim. <c>Kind</c>:
/// <c>income</c>, <c>pos-settlement</c>, <c>counterparty-payment</c>,
/// <c>obligation-settlement</c>, <c>counterparty-charge</c>, <c>obligation</c>.
/// </summary>
public sealed record DayCloseRecordOverrideRequest(string Kind, Guid Id, bool Included);

/// <summary>
/// Satışı da tahsilatı da nakit tutarına dahil edilen bir grubun (kişinin ya
/// da faturanın) <b>ortak tutarı</b>: iki kayıtta da görünen ama girilen
/// tutarda bir kez yer alan para. Sıfır "ayrı ayrı sayıldı" demektir.
/// </summary>
public sealed record DayCloseOverlapRequest(Guid GroupId, string Amount);

/// <summary>
/// Gün sonu (ADR 0019 T1–T2); önizleme ve kayıt aynı gövdeyi alır.
/// </summary>
/// <remarks>
/// Nakit ve POS satırlarından her biri yalnız kendi tutarı yazıldıysa kayıt
/// üretir; toplam hiçbir zaman kayıt üretmez ve eksik tarafı hesaplamaz,
/// yalnız farkı gösterir. <c>ClientRequestId</c> kaydı idempotent yapar ve
/// yalnız kayıtta zorunludur. <c>RecordOverrides</c> kullanıcının değiştirdiği
/// işaretleri ve <c>requiresAnswer</c> taşıyan her kaydın cevabını taşır; adı
/// geçmeyen kayıt varsayılanıyla işlenir, cevap isteyen kayıt ise cevapsız
/// sayılır ve nakit tutarı yazıldıysa kayıt reddedilir. <c>Overlaps</c>
/// önizlemenin <c>overlapGroups</c> ile sorduğu her grubun ortak tutarını
/// taşır. <c>RangeStart</c> ve <c>ZNumber</c> Z raporu okumasından gelir; elle
/// girişte boştur.
/// </remarks>
public sealed record DayCloseRequest(
    string Date,
    string? CashAmount = null,
    IReadOnlyList<DayClosePosAmountRequest>? PosAmounts = null,
    string? TotalAmount = null,
    Guid? CashAccountId = null,
    Guid? CashCategoryId = null,
    IReadOnlyList<DayCloseRecordOverrideRequest>? RecordOverrides = null,
    bool IsAdditional = false,
    string? RangeStart = null,
    int? ZNumber = null,
    Guid? ClientRequestId = null,
    IReadOnlyList<DayCloseOverlapRequest>? Overlaps = null);

public sealed record DayCloseExistingRecordResponse(
    string Kind,
    Guid Id,
    string Side,
    string Date,
    string Amount,
    string Title,
    Guid? PosDefinitionId,
    string AccountName,
    bool IncludedByDefault,
    // Boş: cevaplanmadı (yalnız <c>RequiresAnswer</c> taşıyan kayıtta).
    bool? Included,
    // Kart tarafında bir alacağın kartla tahsili; başlık kişinin adıdır.
    bool IsCardCollection = false,
    // Vadeli satış ve nakit tahsilat: hazır cevapla gelmez.
    bool RequiresAnswer = false,
    // Satışı ile tahsilatı aynı parayı gösterebilecek kayıtların grubu: cari
    // kayıtta kişi, alacak faturasında fatura.
    Guid? GroupId = null,
    string? GroupName = null);

/// <summary>
/// Ortak tutarı sorulan (ya da saklanmış) bir grup. <c>OverlapAmount</c> boşsa
/// cevaplanmamıştır; <c>DeductedAmount</c> = satışlar + tahsilatlar − ortak.
/// </summary>
/// <remarks>
/// <c>Kind</c>: <c>counterparty</c> (kişi) ya da <c>obligation</c> (fatura).
/// Üç cevabın sonucu hazır gelir: "ayrı ayrı" <c>SeparateAmount</c>, "biri
/// öbürünün içinde" <c>InsideAmount</c>, "bir kısmı" yazılan tutarla
/// <c>DeductedAmount</c> kadar düşer. <c>LargerSide</c> (<c>sales</c> ya da
/// <c>collections</c>; eşitse <c>sales</c>) hangisinin öbürünün içinde
/// sayılabileceğini söyler.
/// </remarks>
public sealed record DayCloseOverlapGroupResponse(
    Guid GroupId,
    string Name,
    string SalesAmount,
    string CollectionsAmount,
    string MaximumOverlap,
    string? OverlapAmount,
    string? DeductedAmount,
    string Kind,
    string SeparateAmount,
    string InsideAmount,
    string LargerSide);

/// <summary>
/// Nakit tutarından düşülenin dökümü; dört tutarın toplamı eksi
/// <c>SharedAmount</c>, nakit satırının <c>DeductedAmount</c> değeridir.
/// </summary>
public sealed record DayCloseCashDeductionsResponse(
    string SalesAmount,
    string CollectionsAmount,
    string CreditSalesAmount,
    string InvoicesAmount,
    string SharedAmount);

public sealed record DayCloseCashLineResponse(
    bool Stated,
    string EnteredAmount,
    string DeductedAmount,
    string AmountToWrite,
    Guid? AccountId,
    string? AccountName,
    Guid? CategoryId,
    string? CategoryName,
    DayCloseCashDeductionsResponse Deductions);

public sealed record DayClosePosLineResponse(
    Guid PosDefinitionId,
    string Name,
    bool IsDefault,
    string AccountName,
    bool Stated,
    string EnteredAmount,
    string DeductedAmount,
    string AmountToWrite,
    string CommissionAmount,
    string NetAmount,
    string ExpectedTransferDate);

public sealed record DayCloseSummaryResponse(
    Guid Id,
    string ClosedOn,
    string? RangeStart,
    int? ZNumber,
    bool IsAdditional);

/// <summary>
/// Panelin önizlemesi. <c>BlockerCode</c> doluysa bu girdiyle kayıt o kodla
/// reddedilir; panel bunu ilgili alanın yanında söyler.
/// </summary>
public sealed record DayClosePreviewResponse(
    string Date,
    string? RangeStart,
    string Currency,
    IReadOnlyList<DayCloseSummaryResponse> ClosedBy,
    DayCloseCashLineResponse Cash,
    IReadOnlyList<DayClosePosLineResponse> PosLines,
    string? TotalEntered,
    string TotalComputed,
    string? TotalDifference,
    IReadOnlyList<DayCloseExistingRecordResponse> ExistingRecords,
    string? BlockerCode,
    // Satışı da tahsilatı da dahil edilen gruplar; ortak tutarları sorulur.
    IReadOnlyList<DayCloseOverlapGroupResponse> OverlapGroups);

public sealed record DayCloseIncomeResponse(
    Guid TransactionId,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    string Amount,
    string Date,
    string Scope,
    bool IsCancelled);

public sealed record DayCloseResponse(
    Guid Id,
    string ClosedOn,
    string? RangeStart,
    int? ZNumber,
    bool IsAdditional,
    bool IsCancelled,
    DateTimeOffset? CancelledAtUtc,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<DayCloseIncomeResponse> Incomes,
    IReadOnlyList<PosSettlementResponse> Settlements,
    string CashAmount,
    string CardGrossAmount,
    string CommissionAmount,
    string Currency,
    // Gün sonunun saydığı (tek tek girilmiş ve tutardan düşülmüş) kayıtlar;
    // geri alınmış gün sonunda boştur.
    IReadOnlyList<DayCloseExistingRecordResponse> CountedRecords,
    // Nakit tutarından düşülen: sayılan nakit kayıtlar − ortak tutarlar.
    string CountedCashAmount,
    string CountedCardAmount,
    IReadOnlyList<DayCloseOverlapGroupResponse> Overlaps);

public sealed record DayCloseListResponse(IReadOnlyList<DayCloseResponse> Items);

/// <summary>
/// Bir günün bütünü: ana ve ek gün sonları (ilk giren üstte), dışarıda kalan
/// tek tek girilmiş kayıtlar ve günün toplamı (yazılan + sayılan).
/// </summary>
public sealed record DayCloseDayResponse(
    string Date,
    bool IsClosed,
    IReadOnlyList<DayCloseResponse> Closes,
    IReadOnlyList<DayCloseExistingRecordResponse> OutsideRecords,
    string CashTotal,
    string CardTotal,
    string Currency);

public static class DayCloseEndpoints
{
    internal static readonly Dictionary<DayCloseRecordKind, string> RecordKindValues = new()
    {
        [DayCloseRecordKind.Income] = "income",
        [DayCloseRecordKind.PosSettlement] = "pos-settlement",
        [DayCloseRecordKind.CounterpartyPayment] = "counterparty-payment",
        [DayCloseRecordKind.ObligationSettlement] = "obligation-settlement",
        [DayCloseRecordKind.CounterpartyCharge] = "counterparty-charge",
        [DayCloseRecordKind.Obligation] = "obligation"
    };

    public static IEndpointRouteBuilder MapDayCloseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/day-closes")
            .WithTags("DayCloses")
            .RequireAuthorization();

        // Gövde taşıdığı için POST; hiçbir şey yazmaz.
        group.MapPost("/preview", PreviewAsync)
            .WithName("PreviewDayClose")
            .Produces<DayClosePreviewResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateAsync)
            .WithName("CreateDayClose")
            .Produces<DayCloseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("/", ListAsync)
            .WithName("ListDayCloses")
            .Produces<DayCloseListResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/day", GetDayAsync)
            .WithName("GetDayCloseDay")
            .Produces<DayCloseDayResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapGet("/{id:guid}", GetAsync)
            .WithName("GetDayClose")
            .Produces<DayCloseResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // Silme yerine geri alma: gün sonu ve kayıtları kalır, kayıtlar iptal
        // olur ve gün yeniden açılır.
        group.MapDelete("/{id:guid}", RevertAsync)
            .WithName("RevertDayClose")
            .Produces<DayCloseResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> PreviewAsync(
        DayCloseRequest request,
        PreviewDayCloseUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParse(request, httpContext, out var input, out var problem))
        {
            return problem!;
        }

        var result = await useCase.ExecuteAsync(input!, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var preview = result.Value;
        return Results.Ok(new DayClosePreviewResponse(
            FinanceContract.Date(preview.Date),
            preview.RangeStart is DateOnly rangeStart ? FinanceContract.Date(rangeStart) : null,
            preview.Currency.ToString(),
            preview.ClosedBy.Select(ToResponse).ToArray(),
            new DayCloseCashLineResponse(
                preview.Cash.Stated,
                FinanceContract.Money(preview.Cash.EnteredAmount),
                FinanceContract.Money(preview.Cash.DeductedAmount),
                FinanceContract.Money(preview.Cash.AmountToWrite),
                preview.Cash.AccountId,
                preview.Cash.AccountName,
                preview.Cash.CategoryId,
                preview.Cash.CategoryName,
                new DayCloseCashDeductionsResponse(
                    FinanceContract.Money(preview.Cash.Deductions.SalesAmount),
                    FinanceContract.Money(preview.Cash.Deductions.CollectionsAmount),
                    FinanceContract.Money(preview.Cash.Deductions.CreditSalesAmount),
                    FinanceContract.Money(preview.Cash.Deductions.InvoicesAmount),
                    FinanceContract.Money(preview.Cash.Deductions.SharedAmount))),
            preview.PosLines
                .Select(line => new DayClosePosLineResponse(
                    line.PosDefinitionId,
                    line.Name,
                    line.IsDefault,
                    line.AccountName,
                    line.Stated,
                    FinanceContract.Money(line.EnteredAmount),
                    FinanceContract.Money(line.DeductedAmount),
                    FinanceContract.Money(line.AmountToWrite),
                    FinanceContract.Money(line.CommissionAmount),
                    FinanceContract.Money(line.NetAmount),
                    FinanceContract.Date(line.ExpectedTransferDate)))
                .ToArray(),
            FinanceContract.OptionalMoney(preview.TotalEntered),
            FinanceContract.Money(preview.TotalComputed),
            FinanceContract.OptionalMoney(preview.TotalDifference),
            preview.ExistingRecords.Select(ToResponse).ToArray(),
            preview.Blocker?.Code,
            preview.OverlapGroups.Select(ToResponse).ToArray()));
    }

    private static async Task<IResult> CreateAsync(
        DayCloseRequest request,
        CreateDayCloseUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!TryParse(request, httpContext, out var input, out var problem))
        {
            return problem!;
        }

        var result = await useCase.ExecuteAsync(
            new CreateDayCloseCommand(request.ClientRequestId ?? Guid.Empty, input!),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var response = ToResponse(result.Value);
        return Results.Created($"/api/v1/day-closes/{response.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        string? from,
        string? to,
        ListDayClosesUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(from, out var fromDate) ||
            !FinanceContract.TryParseDate(to, out var toDate))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "From and to must use the yyyy-MM-dd format.",
                "day_closes.invalid_range");
        }

        var result = await useCase.ExecuteAsync(fromDate, toDate, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new DayCloseListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> GetDayAsync(
        string? date,
        GetDayCloseDayUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(date, out var day))
        {
            return ApiProblemResults.Validation(
                httpContext, "Date must use the yyyy-MM-dd format.", "day_closes.invalid_date");
        }

        var result = await useCase.ExecuteAsync(day, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblemResult(httpContext);
        }

        var value = result.Value;
        return Results.Ok(new DayCloseDayResponse(
            FinanceContract.Date(value.Date),
            value.Closes.Count > 0,
            value.Closes.Select(ToResponse).ToArray(),
            value.OutsideRecords.Select(ToResponse).ToArray(),
            FinanceContract.Money(value.CashTotal),
            FinanceContract.Money(value.CardTotal),
            value.Currency.ToString()));
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetDayCloseUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RevertAsync(
        Guid id,
        RevertDayCloseUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static bool TryParse(
        DayCloseRequest request,
        HttpContext httpContext,
        out DayCloseInput? input,
        out IResult? problem)
    {
        input = null;
        problem = null;
        if (!FinanceContract.TryParseDate(request.Date, out var date))
        {
            problem = ApiProblemResults.Validation(
                httpContext, "Date must use the yyyy-MM-dd format.", "day_closes.invalid_date");
            return false;
        }

        DateOnly? rangeStart = null;
        if (!string.IsNullOrWhiteSpace(request.RangeStart))
        {
            if (!FinanceContract.TryParseDate(request.RangeStart, out var parsedStart))
            {
                problem = ApiProblemResults.Validation(
                    httpContext,
                    "Range start must use the yyyy-MM-dd format.",
                    "day_closes.invalid_date");
                return false;
            }

            rangeStart = parsedStart;
        }

        if (!FinanceContract.TryParseOptionalAmount(request.CashAmount, out var cashAmount) ||
            !FinanceContract.TryParseOptionalAmount(request.TotalAmount, out var totalAmount))
        {
            problem = InvalidAmount(httpContext);
            return false;
        }

        var posAmounts = new List<DayClosePosAmount>();
        foreach (var item in request.PosAmounts ?? [])
        {
            if (!FinanceContract.TryParseAmount(item.Amount, out var amount))
            {
                problem = InvalidAmount(httpContext);
                return false;
            }

            posAmounts.Add(new DayClosePosAmount(item.PosDefinitionId, amount));
        }

        var overrides = new List<DayCloseRecordOverride>();
        foreach (var item in request.RecordOverrides ?? [])
        {
            var kind = RecordKindValues.FirstOrDefault(pair => pair.Value == item.Kind);
            if (kind.Value is null)
            {
                problem = ApiProblemResults.Validation(
                    httpContext,
                    "Record kind is not supported.",
                    "day_closes.invalid_record_kind");
                return false;
            }

            overrides.Add(new DayCloseRecordOverride(kind.Key, item.Id, item.Included));
        }

        var overlaps = new List<DayCloseOverlap>();
        foreach (var item in request.Overlaps ?? [])
        {
            if (!FinanceContract.TryParseAmount(item.Amount, out var amount))
            {
                problem = InvalidAmount(httpContext);
                return false;
            }

            overlaps.Add(new DayCloseOverlap(item.GroupId, amount));
        }

        input = new DayCloseInput(
            date,
            cashAmount,
            posAmounts,
            totalAmount,
            request.CashAccountId,
            request.CashCategoryId,
            overrides,
            request.IsAdditional,
            rangeStart,
            request.ZNumber,
            overlaps);
        return true;
    }

    private static IResult InvalidAmount(HttpContext httpContext) =>
        ApiProblemResults.Validation(
            httpContext,
            "Amounts must be decimal strings with at most four decimals.",
            "day_closes.invalid_amount");

    private static DayCloseExistingRecordResponse ToResponse(
        DayCloseExistingRecordDto record) => new(
        RecordKindValues[record.Kind],
        record.Id,
        record.Side == DayCloseSide.Cash ? "cash" : "card",
        FinanceContract.Date(record.Date),
        FinanceContract.Money(record.Amount),
        record.Title,
        record.PosDefinitionId,
        record.AccountName,
        record.IncludedByDefault,
        record.Included,
        record.IsCardCollection,
        record.RequiresAnswer,
        record.GroupId,
        record.GroupName);

    private static DayCloseOverlapGroupResponse ToResponse(DayCloseOverlapGroupDto group) => new(
        group.GroupId,
        group.Name,
        FinanceContract.Money(group.SalesAmount),
        FinanceContract.Money(group.CollectionsAmount),
        FinanceContract.Money(group.MaximumOverlap),
        FinanceContract.OptionalMoney(group.OverlapAmount),
        FinanceContract.OptionalMoney(group.DeductedAmount),
        group.Kind == DayCloseGroupKind.Obligation ? "obligation" : "counterparty",
        FinanceContract.Money(group.SeparateAmount),
        FinanceContract.Money(group.InsideAmount),
        group.LargerSide == DayCloseLargerSide.Collections ? "collections" : "sales");

    private static DayCloseSummaryResponse ToResponse(DayCloseSummaryDto summary) => new(
        summary.Id,
        FinanceContract.Date(summary.ClosedOn),
        summary.RangeStart is DateOnly rangeStart ? FinanceContract.Date(rangeStart) : null,
        summary.ZNumber,
        summary.IsAdditional);

    private static DayCloseResponse ToResponse(DayCloseDto dayClose) => new(
        dayClose.Id,
        FinanceContract.Date(dayClose.ClosedOn),
        dayClose.RangeStart is DateOnly rangeStart ? FinanceContract.Date(rangeStart) : null,
        dayClose.ZNumber,
        dayClose.IsAdditional,
        dayClose.IsCancelled,
        dayClose.CancelledAtUtc,
        dayClose.CreatedAtUtc,
        dayClose.Incomes
            .Select(income => new DayCloseIncomeResponse(
                income.TransactionId,
                income.AccountId,
                income.AccountName,
                income.CategoryId,
                income.CategoryName,
                FinanceContract.Money(income.Amount),
                FinanceContract.Date(income.Date),
                FinanceContract.ScopeValue(income.Scope),
                income.IsCancelled))
            .ToArray(),
        dayClose.Settlements.Select(PosSettlementEndpoints.ToResponse).ToArray(),
        FinanceContract.Money(dayClose.CashAmount),
        FinanceContract.Money(dayClose.CardGrossAmount),
        FinanceContract.Money(dayClose.CommissionAmount),
        dayClose.Currency.ToString(),
        dayClose.CountedRecords.Select(ToResponse).ToArray(),
        FinanceContract.Money(dayClose.CountedCashAmount),
        FinanceContract.Money(dayClose.CountedCardAmount),
        dayClose.Overlaps.Select(ToResponse).ToArray());
}
