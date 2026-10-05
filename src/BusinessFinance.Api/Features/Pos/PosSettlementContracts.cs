using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Pos;

/// <summary>
/// <c>PosDefinitionId</c> verilirse boş bırakılan alanlar tanımdan dolar
/// (ADR 0019 T4): hesap, satış kategorisi, komisyon (tanımın oranıyla),
/// komisyon kategorisi ve beklenen gün; açıkça gönderilen alan tanımı ezer.
/// Tanım verilmezse hesap, kategori ve beklenen gün zorunludur.
/// </summary>
public sealed record CreatePosSettlementRequest(
    Guid? AccountId,
    Guid? CategoryId,
    string GrossAmount,
    string Currency,
    string SettlementDate,
    string? ExpectedTransferDate = null,
    string? CommissionAmount = null,
    string? CommissionRate = null,
    Guid? CommissionCategoryId = null,
    string? Scope = null,
    string? Description = null,
    Guid? PosDefinitionId = null);

public sealed record PosSettlementResponse(
    Guid Id,
    Guid AccountId,
    string AccountName,
    Guid CategoryId,
    string CategoryName,
    Guid? CommissionCategoryId,
    string? CommissionCategoryName,
    string GrossAmount,
    string CommissionAmount,
    string NetAmount,
    string CommissionRate,
    string Currency,
    string Scope,
    string SettlementDate,
    string ExpectedTransferDate,
    string? TransferredOn,
    string? Description,
    bool IsInTransit,
    bool IsCancelled,
    bool IsLate,
    Guid? PosDefinitionId,
    string? PosDefinitionName,
    // Parayı hesaba geçiren yatış; para yoldaysa boştur.
    Guid? PosDepositId,
    // Tahsilatı üreten gün sonu; tek tek girilende boştur. Doluysa tahsilat
    // tek başına iptal edilemez, gün sonu geri alınır.
    Guid? DayCloseId = null,
    // Tek tek girilmiş tahsilatı sayan gün sonu; sayılmamışsa boştur. Doluysa
    // tahsilat tek başına iptal edilemez, önce gün sonu geri alınır.
    Guid? CountedInDayCloseId = null);

public sealed record PosSettlementListResponse(
    IReadOnlyList<PosSettlementResponse> Items,
    string MoneyInTransit,
    int InTransitCount);
