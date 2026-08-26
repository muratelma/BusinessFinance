using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.DataPortability;
using BusinessFinance.Application.Reports;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Taxes;

public static class AccountantPackageErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError InvalidPeriod = new(
        "accountant_package.invalid_period",
        "Year or month is outside the supported range.",
        ApplicationErrorType.Validation);
}

/// <summary>
/// Ay sonu muhasebeci paketini kurar.
/// </summary>
/// <remarks>
/// Toplamlar aynı ayın <b>işletme</b> raporundan okunur; paket ikinci bir
/// hesaplama yolu açmaz. Satırlar o toplamın dökümüdür.
/// </remarks>
public sealed class GetAccountantPackageUseCase(
    ICurrentUser currentUser,
    IFinancialReportRepository reportRepository,
    IAccountantPackageRepository packageRepository)
{
    public async Task<ApplicationResult<AccountantPackageDto>> ExecuteAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<AccountantPackageDto>.Failure(
                AccountantPackageErrors.AuthenticationRequired);
        }

        if (year is < MonthlyBudget.MinimumYear or > MonthlyBudget.MaximumYear ||
            month is < 1 or > 12)
        {
            return ApplicationResult<AccountantPackageDto>.Failure(
                AccountantPackageErrors.InvalidPeriod);
        }

        // Kapsam burada **sabittir**: paket muhasebeciye gider ve şahsi kayıt
        // ona ait değildir. Filtreyi çağırana bırakmak, paketin şahsi kayıt
        // taşıyabileceği bir yol açardı.
        var report = await reportRepository.GetMonthlyAsync(
            userId, year, month, TransactionScope.Business, cancellationToken);
        var lines = await packageRepository.ListBusinessLinesAsync(
            userId, year, month, cancellationToken);
        var attachments = await packageRepository.ListAttachmentsAsync(
            userId,
            [.. lines.Where(line => line.AttachmentCount > 0).Select(line => line.SourceId)],
            cancellationToken);

        return ApplicationResult<AccountantPackageDto>.Success(new AccountantPackageDto(
            year,
            month,
            report.Currency,
            report.TotalIncome,
            report.TotalExpense,
            report.Net,
            lines.Where(line => line.Type == TransactionType.Income)
                .Sum(line => line.VatAmount ?? 0m),
            lines.Where(line => line.Type == TransactionType.Expense)
                .Sum(line => line.VatAmount ?? 0m),
            lines.Count(line => line.VatAmount is null),
            lines.Where(line =>
                    line.Type == TransactionType.Expense && line.IsTaxDeductible == false)
                .Sum(line => line.Amount),
            lines.Count(line =>
                line.Type == TransactionType.Expense && line.IsTaxDeductible == false),
            lines.Count(line =>
                line.Type == TransactionType.Expense && line.IsTaxDeductible is null),
            lines,
            attachments));
    }
}

/// <summary>
/// Paketi tek dosya olarak üretir.
/// </summary>
/// <remarks>
/// Dosya, ekranın gördüğü paketin aynısından yazılır: ikinci bir okuma ve
/// ikinci bir toplam yoktur. Dosya kullanıcının kendi cihazından paylaşılır;
/// sunucu üçüncü kişiye hiçbir şey göndermez.
/// </remarks>
public sealed class DownloadAccountantPackageUseCase(
    ICurrentUser currentUser,
    GetAccountantPackageUseCase package,
    IAccountantPackageRepository repository)
{
    public async Task<ApplicationResult<PortableFile>> ExecuteAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<PortableFile>.Failure(
                AccountantPackageErrors.AuthenticationRequired);
        }

        var result = await package.ExecuteAsync(year, month, cancellationToken);
        if (!result.IsSuccess)
        {
            return ApplicationResult<PortableFile>.Failure(result.Error);
        }

        return ApplicationResult<PortableFile>.Success(
            await repository.BuildPackageAsync(userId, result.Value, cancellationToken));
    }
}
