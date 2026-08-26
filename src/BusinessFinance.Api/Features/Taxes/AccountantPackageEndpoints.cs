using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Taxes;

namespace BusinessFinance.Api.Features.Taxes;

public sealed record AccountantPackageLineResponse(
    string Source,
    Guid SourceId,
    string Date,
    string Type,
    string Amount,
    string? CategoryName,
    string? CounterpartyName,
    string? Description,
    string? VatRate,
    string? VatAmount,
    bool? IsTaxDeductible,
    int AttachmentCount);

public sealed record AccountantPackageAttachmentResponse(
    Guid Id,
    Guid TransactionId,
    string FileName,
    string ContentType,
    long SizeBytes,
    bool IsIncluded);

/// <summary>
/// Ay sonu muhasebeci paketinin önizlemesi.
/// </summary>
/// <remarks>
/// Toplamlar aynı ayın <b>işletme</b> raporundan gelir; kapsam parametresi
/// yoktur çünkü paket muhasebeciye gider ve şahsi kayıt ona ait değildir.
/// </remarks>
public sealed record AccountantPackageResponse(
    int Year,
    int Month,
    string Scope,
    string Currency,
    string TotalIncome,
    string TotalExpense,
    string Net,
    string VatOnIncome,
    string VatOnExpense,
    int LinesWithoutVat,
    string NonDeductibleExpense,
    int NonDeductibleCount,
    int DeductibilityUnansweredCount,
    IReadOnlyList<AccountantPackageLineResponse> Lines,
    IReadOnlyList<AccountantPackageAttachmentResponse> Attachments);

public static class AccountantPackageEndpoints
{
    public static IEndpointRouteBuilder MapAccountantPackageEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/accountant-package", GetAsync)
            .WithTags("TaxCalendar")
            .WithName("GetAccountantPackage")
            .RequireAuthorization()
            .Produces<AccountantPackageResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        endpoints.MapGet("/api/v1/exports/accountant-package.zip", DownloadAsync)
            .WithTags("Data Portability")
            .WithName("DownloadAccountantPackage")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK, contentType: "application/zip")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> GetAsync(
        int year,
        int month,
        GetAccountantPackageUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(year, month, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> DownloadAsync(
        int year,
        int month,
        DownloadAccountantPackageUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(year, month, cancellationToken);
        return result.IsSuccess
            ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.Error.ToProblemResult(httpContext);
    }

    private static AccountantPackageResponse ToResponse(AccountantPackageDto package) => new(
        package.Year,
        package.Month,
        "business",
        package.Currency.ToString(),
        FinanceContract.Money(package.TotalIncome),
        FinanceContract.Money(package.TotalExpense),
        FinanceContract.Money(package.Net),
        FinanceContract.Money(package.VatOnIncome),
        FinanceContract.Money(package.VatOnExpense),
        package.LinesWithoutVat,
        FinanceContract.Money(package.NonDeductibleExpense),
        package.NonDeductibleCount,
        package.DeductibilityUnansweredCount,
        [.. package.Lines.Select(line => new AccountantPackageLineResponse(
            line.Source,
            line.SourceId,
            FinanceContract.Date(line.Date),
            FinanceContract.TransactionTypeValue(line.Type),
            FinanceContract.Money(line.Amount),
            line.CategoryName,
            line.CounterpartyName,
            line.Description,
            FinanceContract.OptionalMoney(line.VatRate),
            FinanceContract.OptionalMoney(line.VatAmount),
            line.IsTaxDeductible,
            line.AttachmentCount))],
        [.. package.Attachments.Select(attachment => new AccountantPackageAttachmentResponse(
            attachment.Id,
            attachment.TransactionId,
            attachment.FileName,
            attachment.ContentType,
            attachment.SizeBytes,
            attachment.IsIncluded))]);
}
