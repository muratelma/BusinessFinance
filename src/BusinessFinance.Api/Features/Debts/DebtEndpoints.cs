using System.Globalization;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Features.Pos;
using BusinessFinance.Api.Features.Transactions;
using BusinessFinance.Application.Debts;
using BusinessFinance.Domain;

namespace BusinessFinance.Api.Features.Debts;

public static class DebtEndpoints
{
    public static IEndpointRouteBuilder MapDebtEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/debts").WithTags("Debts").RequireAuthorization();
        group.MapPost("/", CreateAsync).Produces<DebtResponse>(201)
            .ProducesProblem(400).ProducesProblem(401);
        group.MapGet("/", ListAsync).Produces<DebtListResponse>()
            .ProducesProblem(400).ProducesProblem(401);
        group.MapPost("/{debtId:guid}/opening", RecordOpeningAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);
        group.MapPost("/{debtId:guid}/installments/{sequence:int}/pay", PayAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);

        // Dalga 2 (ADR 0021): borcun kendisi ve kapanışları, her planda.
        group.MapGet("/{debtId:guid}", GetAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401).ProducesProblem(404);
        group.MapPost("/{debtId:guid}/payments", RecordPaymentAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);
        group.MapDelete("/{debtId:guid}/payments/{paymentId:guid}", ReversePaymentAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);
        group.MapPost("/{debtId:guid}/discounts", ApplyDiscountAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);
        group.MapPut("/{debtId:guid}/plan", ChangePlanAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);
        group.MapDelete("/{debtId:guid}", CancelAsync)
            .Produces<DebtResponse>().ProducesProblem(400).ProducesProblem(401)
            .ProducesProblem(404).ProducesProblem(409);

        var book = endpoints.MapGroup("/api/v1/debt-book").WithTags("Debts").RequireAuthorization();
        book.MapGet("/", GetBookAsync).Produces<DebtBookResponse>().ProducesProblem(400).ProducesProblem(401);
        book.MapGet("/closed", ListClosedAsync).Produces<DebtBookClosedResponse>().ProducesProblem(401);
        return endpoints;
    }

    private static async Task<IResult> GetBookAsync(
        string asOfDate, GetDebtBookUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "asOfDate must use yyyy-MM-dd.", "debt.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(asOf, cancellationToken);
        if (!result.IsSuccess) return result.Error.ToProblemResult(context);
        var value = result.Value;
        return Results.Ok(new DebtBookResponse(
            FinanceContract.Money(value.PayableTotal), FinanceContract.Money(value.ReceivableTotal),
            value.Items.Select(item => new DebtBookItemResponse(
                item.Kind, item.Id, DirectionName(item.Direction), SectionName(item.Section), item.Title,
                item.CounterpartyId, item.CounterpartyName,
                item.PlanKind is DebtPlanKind planKind ? PlanKindName(planKind) : null,
                FinanceContract.Money(item.Remaining), FinanceContract.Money(item.Overdue),
                item.NextDueDate is DateOnly next ? FinanceContract.Date(next) : null,
                FinanceContract.Money(item.NextDueAmount), item.HasClosings, item.IsCarriedOver,
                item.StartDate is DateOnly start ? FinanceContract.Date(start) : null)).ToArray(),
            value.People.Select(person => new DebtBookPersonResponse(
                person.CounterpartyId, person.Name,
                FinanceContract.Money(person.OwedToYou), FinanceContract.Money(person.OwedByYou),
                FinanceContract.Money(person.LedgerOwedToYou), FinanceContract.Money(person.LedgerOwedByYou),
                person.OpenDebtCount)).ToArray(),
            value.ClosedPayableCount, value.ClosedReceivableCount));
    }

    private static async Task<IResult> ListClosedAsync(
        ListClosedDebtsUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new DebtBookClosedResponse(result.Value.Select(item => new DebtBookClosedItemResponse(
                item.DebtId, DirectionName(item.Direction), item.Title, item.CounterpartyId, item.CounterpartyName,
                PlanKindName(item.PlanKind), FinanceContract.Money(item.Amount),
                FinanceContract.Date(item.ClosedOn))).ToArray()))
            : result.Error.ToProblemResult(context);
    }

    private static string DirectionName(DebtDirection direction) =>
        direction == DebtDirection.Payable ? "payable" : "receivable";

    private static string SectionName(DebtBookSection section) => section switch
    {
        DebtBookSection.Overdue => "overdue",
        DebtBookSection.Upcoming => "upcoming",
        _ => "undated"
    };

    private static async Task<IResult> GetAsync(
        Guid debtId, string asOfDate, GetDebtUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "asOfDate must use yyyy-MM-dd.", "debt.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(debtId, asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> RecordPaymentAsync(
        Guid debtId, RecordDebtPaymentRequest request, RecordDebtPaymentUseCase useCase,
        HttpContext context, CancellationToken cancellationToken)
    {
        if (!TryMoney(request.Amount, out var amount) ||
            !FinanceContract.TryParseDate(request.PaymentDate, out var paymentDate) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "Debt payment fields are invalid.", "debt.invalid_contract");
        if (!CardCollectionRequests.TryParse(request.Card, context, out var card, out var cardError))
            return cardError!;
        var result = await useCase.ExecuteAsync(new RecordDebtPaymentCommand(
            debtId, request.ClientRequestId, amount, paymentDate,
            request.AccountId, request.CreditCardId, card), asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ReversePaymentAsync(
        Guid debtId, Guid paymentId, string asOfDate, ReverseDebtPaymentUseCase useCase,
        HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "asOfDate must use yyyy-MM-dd.", "debt.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(debtId, paymentId, asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ApplyDiscountAsync(
        Guid debtId, ApplyDebtDiscountRequest request, ApplyDebtDiscountUseCase useCase,
        HttpContext context, CancellationToken cancellationToken)
    {
        if (!TryMoney(request.Amount, out var amount) ||
            !FinanceContract.TryParseDate(request.DiscountDate, out var discountDate) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "Debt discount fields are invalid.", "debt.invalid_contract");
        var result = await useCase.ExecuteAsync(
            new ApplyDebtDiscountCommand(debtId, request.ClientRequestId, amount, discountDate),
            asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ChangePlanAsync(
        Guid debtId, ChangeDebtPlanRequest request, ChangeDebtPlanUseCase useCase,
        HttpContext context, CancellationToken cancellationToken)
    {
        if (!TryPlanKind(request.PlanKind, out var planKind) ||
            !TryOptionalDate(request.FirstDueDate, out var firstDue) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "Debt plan fields are invalid.", "debt.invalid_contract");
        var result = await useCase.ExecuteAsync(
            new ChangeDebtPlanCommand(debtId, planKind, firstDue, request.InstallmentCount),
            asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> CancelAsync(
        Guid debtId, string asOfDate, CancelDebtUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "asOfDate must use yyyy-MM-dd.", "debt.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(debtId, asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static bool TryPlanKind(string? value, out DebtPlanKind planKind)
    {
        planKind = DebtPlanKind.Installments;
        switch (value?.ToLowerInvariant())
        {
            case null or "installments":
                return true;
            case "single":
                planKind = DebtPlanKind.Single;
                return true;
            case "none":
                planKind = DebtPlanKind.None;
                return true;
            default:
                return false;
        }
    }

    private static string PlanKindName(DebtPlanKind planKind) => planKind switch
    {
        DebtPlanKind.Single => "single",
        DebtPlanKind.None => "none",
        _ => "installments"
    };

    private static bool TryOptionalDate(string? value, out DateOnly? date)
    {
        date = null;
        if (value is null) return true;
        if (!FinanceContract.TryParseDate(value, out var parsed)) return false;
        date = parsed;
        return true;
    }

    private static async Task<IResult> CreateAsync(
        CreateDebtRequest request, CreateDebtUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        decimal upfrontAmount = 0m;
        if (!TryMoney(request.Principal, out var principal) ||
            !TryOptionalMoney(request.TotalRepayment, out var total) ||
            !TryOptionalRate(request.AnnualInterestRate, out var rate) ||
            !FinanceContract.TryParseDate(request.StartDate, out var start) ||
            !TryOptionalDate(request.FirstDueDate, out var firstDue) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf) ||
            !TryDirection(request.Direction, out var direction) ||
            !TrySourceType(request.SourceType, out var sourceType) ||
            !TryPlanKind(request.PlanKind, out var planKind) ||
            !FinanceContract.TryParseOptionalScope(request.Scope, out var scope) ||
            (request.UpfrontPayment is not null && !TryMoney(request.UpfrontPayment.Amount, out upfrontAmount)) ||
            !string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(context, "Debt fields are invalid.", "debt.invalid_contract");
        var result = await useCase.ExecuteAsync(new CreateDebtCommand(
            request.CounterpartyName, direction, scope, principal, total, rate, CurrencyCode.TRY,
            sourceType, request.OpeningAccountId, request.CategoryId,
            start, firstDue, request.InstallmentCount, request.Description,
            planKind, request.CounterpartyId, request.IsCarriedOver,
            request.UpfrontPayment is null
                ? null
                : new DebtUpfrontPayment(upfrontAmount, request.UpfrontPayment.AccountId, request.UpfrontPayment.CreditCardId),
            request.ClientRequestId), asOf, cancellationToken);
        return result.IsSuccess
            ? Results.Created($"/api/v1/debts/{result.Value.Id}", ToResponse(result.Value))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> RecordOpeningAsync(
        Guid debtId, RecordDebtOpeningRequest request, RecordDebtOpeningUseCase useCase,
        HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.AsOfDate, out var asOf) ||
            !TrySourceType(request.SourceType, out var sourceType))
            return ApiProblemResults.Validation(context, "Debt opening fields are invalid.", "debt.invalid_contract");
        var result = await useCase.ExecuteAsync(
            new RecordDebtOpeningCommand(debtId, sourceType, request.OpeningAccountId, request.CategoryId),
            asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> ListAsync(
        string asOfDate, ListDebtsUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(asOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "asOfDate must use yyyy-MM-dd.", "debt.invalid_as_of_date");
        var result = await useCase.ExecuteAsync(asOf, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new DebtListResponse(result.Value.Select(ToResponse).ToArray()))
            : result.Error.ToProblemResult(context);
    }

    private static async Task<IResult> PayAsync(
        Guid debtId, int sequence, PayDebtInstallmentRequest request,
        PayDebtInstallmentUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!FinanceContract.TryParseDate(request.PaymentDate, out var paymentDate) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf))
            return ApiProblemResults.Validation(context, "Dates must use yyyy-MM-dd.", "debt.invalid_date");
        var result = await useCase.ExecuteAsync(
            new PayDebtInstallmentCommand(debtId, sequence, request.AccountId, paymentDate),
            asOf, cancellationToken);
        return result.IsSuccess ? Results.Ok(ToResponse(result.Value)) : result.Error.ToProblemResult(context);
    }

    internal static DebtResponse ToResponse(DebtDto debt) => new(
        debt.Id, debt.CounterpartyId, debt.CounterpartyName,
        debt.Direction == DebtDirection.Payable ? "payable" : "receivable",
        FinanceContract.ScopeValue(debt.Scope),
        FinanceContract.Money(debt.Principal), FinanceContract.Money(debt.TotalRepayment),
        FinanceContract.Money(debt.RemainingAmount), debt.Currency.ToString(),
        debt.AnnualInterestRate.ToString("0.0000", CultureInfo.InvariantCulture),
        FinanceContract.Money(debt.TotalInterest), SourceTypeName(debt.SourceType),
        debt.OpeningAccountId, debt.CategoryId,
        FinanceContract.Date(debt.StartDate),
        debt.FirstDueDate is DateOnly firstDue ? FinanceContract.Date(firstDue) : null,
        debt.InstallmentCount, debt.Description, debt.IsClosed,
        debt.Installments.Select(x => new DebtInstallmentResponse(
            x.Id, x.Sequence, FinanceContract.Money(x.Amount), x.Currency.ToString(),
            x.DueDate is DateOnly due ? FinanceContract.Date(due) : null, x.Status, x.PaymentAccountId,
            x.PaymentDate is DateOnly date ? FinanceContract.Date(date) : null, x.PaidAtUtc,
            FinanceContract.Money(x.PrincipalPortion),
            FinanceContract.Money(x.InterestPortion),
            FinanceContract.Money(x.PaidAmount),
            FinanceContract.Money(x.OpenAmount))).ToArray(),
        PlanKindName(debt.PlanKind), debt.IsCarriedOver, debt.IsCancelled,
        (debt.Closings ?? []).Select(x => new DebtClosingResponse(
            x.Id, x.Kind == DebtPaymentKind.Discount ? "discount" : "payment",
            FinanceContract.Money(x.Amount), FinanceContract.Date(x.Date), x.RecordedAtUtc,
            x.AccountId, x.CreditCardId, x.PosSettlementId, FinanceContract.Money(x.InterestAmount),
            x.IsCancelled, x.CancelledAtUtc)).ToArray());

    private static bool TryMoney(string value, out decimal amount) =>
        FinanceContract.TryParseAmount(value, out amount) && amount > 0m;

    // Toplam ve oran birbirinin alternatifi: biri yoksa diğerinden hesaplanır.
    // Yokluk geçerlidir, geçersiz bir değer değildir.
    private static bool TryOptionalMoney(string? value, out decimal? amount)
    {
        amount = null;
        if (value is null) return true;
        if (!TryMoney(value, out var parsed)) return false;
        amount = parsed;
        return true;
    }

    private static bool TryOptionalRate(string? value, out decimal? rate)
    {
        rate = null;
        if (value is null) return true;
        if (!decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed) ||
            parsed is < 0m or > 1000m || decimal.Round(parsed, 4) != parsed)
            return false;
        rate = parsed;
        return true;
    }

    // `unrecorded` dışarıdan gönderilemez: o durum yalnız migration ve eski
    // sürüm yedeklerin ürettiği bir geçmiş kalıntısıdır, yeni borcun seçeneği
    // değildir.
    private static bool TrySourceType(string value, out DebtSourceType sourceType)
    {
        sourceType = DebtSourceType.Cash;
        if (string.Equals(value, "cash", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(value, "expense", StringComparison.OrdinalIgnoreCase))
        {
            sourceType = DebtSourceType.Expense;
            return true;
        }

        if (!string.Equals(value, "income", StringComparison.OrdinalIgnoreCase)) return false;
        sourceType = DebtSourceType.Income;
        return true;
    }

    private static string SourceTypeName(DebtSourceType sourceType) => sourceType switch
    {
        DebtSourceType.Cash => "cash",
        DebtSourceType.Expense => "expense",
        DebtSourceType.Income => "income",
        _ => "unrecorded"
    };
    private static bool TryDirection(string value, out DebtDirection direction)
    {
        direction = string.Equals(value, "payable", StringComparison.OrdinalIgnoreCase)
            ? DebtDirection.Payable
            : DebtDirection.Receivable;
        return string.Equals(value, "payable", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "receivable", StringComparison.OrdinalIgnoreCase);
    }
}
