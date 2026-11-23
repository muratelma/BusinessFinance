using System.Net.Http.Json;
using BusinessFinance.Api.Features.Debts;
using BusinessFinance.Api.Features.Pos;

namespace BusinessFinance.Api.Tests.Features.Debts;

/// <summary>
/// Tek ödemeli kayıt (fatura, tek seferlik alacak) için test düzeneği.
/// </summary>
/// <remarks>
/// Eski <c>/api/v1/obligations</c> uçları ADR 0022 Faz 8'de kalktı; aynı kayıt
/// <c>POST /api/v1/debts</c> (<c>planKind: single</c>) ile açılır, ödemesi
/// <c>/payments</c> ile yazılır, iptali <c>DELETE /api/v1/debts/{id}</c>'dir.
/// Testler kaydı eski alanlarla tarif eder; çeviri burada, tek yerde.
/// </remarks>
internal sealed record SingleDebtInput(
    string Direction,
    string Amount,
    string Currency,
    Guid CategoryId,
    string IssueDate,
    string DueDate,
    string? Scope = null,
    Guid? CounterpartyId = null,
    string? Description = null);

/// <summary>Kaydın tamamının kapanışı: hesap ya da kartla tahsil (POS).</summary>
internal sealed record SingleDebtSettlement(
    Guid? AccountId,
    string SettlementDate,
    CardCollectionRequest? Card = null);

internal static class SingleDebts
{
    public static CreateDebtRequest Request(SingleDebtInput input) => new(
        CounterpartyName: null,
        Direction: input.Direction,
        Scope: input.Scope,
        Principal: input.Amount,
        TotalRepayment: input.Amount,
        AnnualInterestRate: null,
        Currency: input.Currency,
        SourceType: input.Direction == "payable" ? "expense" : "income",
        OpeningAccountId: null,
        CategoryId: input.CategoryId,
        StartDate: input.IssueDate,
        FirstDueDate: input.DueDate,
        InstallmentCount: 1,
        // Kişisiz kayıt açıklama ister; eski yükümlülükte zorunlu değildi.
        Description: input.Description ?? (input.CounterpartyId is null ? "Fatura" : null),
        AsOfDate: input.IssueDate,
        PlanKind: "single",
        CounterpartyId: input.CounterpartyId);

    public static Task<HttpResponseMessage> PostSingleDebtAsync(
        this HttpClient client, SingleDebtInput input) =>
        client.PostAsJsonAsync("/api/v1/debts", Request(input));

    /// <summary>Açılır ve kaydın kendisini döner; açılamazsa testi düşürür.</summary>
    public static async Task<DebtResponse> CreateSingleDebtAsync(
        this HttpClient client, SingleDebtInput input)
    {
        using var response = await client.PostSingleDebtAsync(input);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Tek ödemeli kayıt açılamadı: {(int)response.StatusCode} " +
                await response.Content.ReadAsStringAsync());
        }

        return (await response.Content.ReadFromJsonAsync<DebtResponse>())!;
    }

    /// <summary>Kalanın tamamını o günle kapatır (eski "kapat" düğmesi).</summary>
    public static async Task<HttpResponseMessage> SettleSingleDebtAsync(
        this HttpClient client, Guid debtId, SingleDebtSettlement settlement)
    {
        var debt = await client.GetFromJsonAsync<DebtResponse>(
            $"/api/v1/debts/{debtId}?asOfDate={settlement.SettlementDate}");
        return await client.PostAsJsonAsync(
            $"/api/v1/debts/{debtId}/payments",
            new RecordDebtPaymentRequest(
                debt!.RemainingAmount,
                settlement.SettlementDate,
                settlement.SettlementDate,
                settlement.AccountId,
                Card: settlement.Card));
    }

    public static Task<HttpResponseMessage> CancelSingleDebtAsync(
        this HttpClient client, Guid debtId, string asOfDate) =>
        client.DeleteAsync($"/api/v1/debts/{debtId}?asOfDate={asOfDate}");
}
