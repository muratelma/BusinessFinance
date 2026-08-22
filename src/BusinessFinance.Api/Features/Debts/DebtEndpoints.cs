using System.Globalization;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Contracts;
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
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateDebtRequest request, CreateDebtUseCase useCase, HttpContext context, CancellationToken cancellationToken)
    {
        if (!TryMoney(request.Principal, out var principal) ||
            !TryOptionalMoney(request.TotalRepayment, out var total) ||
            !TryOptionalRate(request.AnnualInterestRate, out var rate) ||
            !FinanceContract.TryParseDate(request.StartDate, out var start) ||
            !FinanceContract.TryParseDate(request.FirstDueDate, out var firstDue) ||
            !FinanceContract.TryParseDate(request.AsOfDate, out var asOf) ||
            !TryDirection(request.Direction, out var direction) ||
            !TrySourceType(request.SourceType, out var sourceType) ||
            !FinanceContract.TryParseScope(request.Scope, out var scope) ||
            !string.Equals(request.Currency, "TRY", StringComparison.OrdinalIgnoreCase))
            return ApiProblemResults.Validation(context, "Debt fields are invalid.", "debt.invalid_contract");
        var result = await useCase.ExecuteAsync(new CreateDebtCommand(
            request.CounterpartyName, direction, scope, principal, total, rate, CurrencyCode.TRY,
            sourceType, request.OpeningAccountId, request.CategoryId,
            start, firstDue, request.InstallmentCount, request.Description), asOf, cancellationToken);
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
        debt.Id, debt.CounterpartyName, debt.Direction == DebtDirection.Payable ? "payable" : "receivable",
        FinanceContract.ScopeValue(debt.Scope),
        FinanceContract.Money(debt.Principal), FinanceContract.Money(debt.TotalRepayment),
        FinanceContract.Money(debt.RemainingAmount), debt.Currency.ToString(),
        debt.AnnualInterestRate.ToString("0.0000", CultureInfo.InvariantCulture),
        FinanceContract.Money(debt.TotalInterest), SourceTypeName(debt.SourceType),
        debt.OpeningAccountId, debt.CategoryId,
        FinanceContract.Date(debt.StartDate), FinanceContract.Date(debt.FirstDueDate),
        debt.InstallmentCount, debt.Description, debt.IsClosed,
        debt.Installments.Select(x => new DebtInstallmentResponse(
            x.Id, x.Sequence, FinanceContract.Money(x.Amount), x.Currency.ToString(),
            FinanceContract.Date(x.DueDate), x.Status, x.PaymentAccountId,
            x.PaymentDate is DateOnly date ? FinanceContract.Date(date) : null, x.PaidAtUtc,
            FinanceContract.Money(x.PrincipalPortion),
            FinanceContract.Money(x.InterestPortion))).ToArray());

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
