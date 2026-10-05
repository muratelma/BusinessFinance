using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Pos;

namespace BusinessFinance.Api.Features.Pos;

/// <summary>
/// Cari tahsilatın ya da tek seferlik alacağın kapanışının kartla (POS)
/// alındığını söyleyen istek parçası (ADR 0019 T5).
/// </summary>
/// <remarks>
/// POS tahsilatıyla aynı kural: <c>posDefinitionId</c> verilirse boş alanlar
/// POS'tan dolar (hesap, komisyon oranı ve kategorisi, beklenen gün); açıkça
/// gönderilen alan onu ezer. POS verilmezse hesap ve beklenen gün zorunludur.
/// Komisyon ya tutar ya oran olarak gelir, ikisi birden değil.
/// </remarks>
public sealed record CardCollectionRequest(
    Guid? PosDefinitionId = null,
    Guid? AccountId = null,
    string? CommissionAmount = null,
    string? CommissionRate = null,
    Guid? CommissionCategoryId = null,
    string? ExpectedTransferDate = null);

internal static class CardCollectionRequests
{
    /// <summary>
    /// İsteği komuta çevirir; biçim hatasında <paramref name="error"/> doludur.
    /// Kart bloğu yoksa sonuç boştur ve hata yoktur.
    /// </summary>
    public static bool TryParse(
        CardCollectionRequest? request,
        HttpContext httpContext,
        out CardCollectionInput? input,
        out IResult? error)
    {
        input = null;
        error = null;
        if (request is null)
        {
            return true;
        }

        decimal? commissionAmount = null;
        if (request.CommissionAmount is not null)
        {
            if (!FinanceContract.TryParseAmount(request.CommissionAmount, out var parsed))
            {
                error = ApiProblemResults.Validation(
                    httpContext,
                    "Commission amount must be a decimal string with at most four decimals.",
                    "pos_settlements.invalid_commission_amount");
                return false;
            }

            commissionAmount = parsed;
        }

        decimal? commissionRate = null;
        if (request.CommissionRate is not null)
        {
            if (!FinanceContract.TryParseAmount(request.CommissionRate, out var parsed))
            {
                error = ApiProblemResults.Validation(
                    httpContext,
                    "Commission rate must be a decimal string with at most four decimals.",
                    "pos_settlements.invalid_commission_rate");
                return false;
            }

            commissionRate = parsed;
        }

        DateOnly? expectedTransferDate = null;
        if (request.ExpectedTransferDate is not null)
        {
            if (!FinanceContract.TryParseDate(request.ExpectedTransferDate, out var parsed))
            {
                error = ApiProblemResults.Validation(
                    httpContext,
                    "Expected transfer date must use the yyyy-MM-dd format.",
                    "pos_settlements.invalid_expected_transfer_date");
                return false;
            }

            expectedTransferDate = parsed;
        }

        input = new CardCollectionInput(
            request.PosDefinitionId,
            request.AccountId,
            commissionAmount,
            commissionRate,
            request.CommissionCategoryId,
            expectedTransferDate);
        return true;
    }
}
