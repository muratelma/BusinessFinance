using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Pos;

/// <summary>POS tanımının tam hâli; oluşturma ve düzenleme aynı alanları taşır.</summary>
public sealed record SavePosDefinitionCommand(
    string Name,
    Guid AccountId,
    Guid SalesCategoryId,
    decimal CommissionRate,
    Guid? CommissionCategoryId,
    int TransferDays,
    bool BusinessDaysOnly);

public sealed record PosDefinitionDto(
    Guid Id,
    string Name,
    Guid AccountId,
    string AccountName,
    Guid SalesCategoryId,
    string SalesCategoryName,
    Guid? CommissionCategoryId,
    string? CommissionCategoryName,
    decimal CommissionRate,
    int TransferDays,
    bool BusinessDaysOnly,
    bool IsActive,
    bool IsDefault = false);

/// <summary>
/// Tanımla yazılacak tahsilatın önizlemesi: komisyon, hesaba geçecek net ve
/// beklenen gün. Hiçbir şey yazmaz; istemci bu sayıları kendisi hesaplamaz.
/// </summary>
public sealed record PosSettlementPreviewDto(
    decimal GrossAmount,
    decimal CommissionAmount,
    decimal NetAmount,
    CurrencyCode Currency,
    DateOnly SettlementDate,
    DateOnly ExpectedTransferDate);

public interface IPosDefinitionRepository
{
    Task AddAsync(PosDefinition definition, CancellationToken cancellationToken);

    /// <summary>Kullanıcının başka bir varsayılan POS'u var mı.</summary>
    Task<bool> HasDefaultAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Kullanıcının [exceptDefinitionId] dışındaki varsayılan POS'larını
    /// izlenen (tracked) olarak döner; işaretleri aynı kayıtta kaldırılır.
    /// </summary>
    Task<IReadOnlyList<PosDefinition>> FindOtherDefaultsAsync(
        Guid userId,
        Guid exceptDefinitionId,
        CancellationToken cancellationToken);

    /// <summary>Kullanıcının tanımları, adlarıyla; önce varsayılan, sonra aktifler, ada göre.</summary>
    Task<IReadOnlyList<PosDefinitionDto>> ListAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<PosDefinition?> FindOwnedByIdAsync(
        Guid definitionId,
        Guid userId,
        bool track,
        CancellationToken cancellationToken);

    /// <summary>İptal edilmiş olanlar dahil, tanıma bağlı tahsilat var mı.</summary>
    Task<bool> HasSettlementsAsync(
        Guid definitionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task RemoveAsync(PosDefinition definition, CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken);
}

public static class PosDefinitionErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError AccountUnavailable = new(
        "pos_definitions.account_unavailable",
        "An active owned bank account is required.",
        ApplicationErrorType.NotFound);

    public static readonly ApplicationError SalesCategoryUnavailable = new(
        "pos_definitions.sales_category_unavailable",
        "An active owned income category is required for sales.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError CommissionCategoryUnavailable = new(
        "pos_definitions.commission_category_unavailable",
        "An active owned expense category is required for the commission.",
        ApplicationErrorType.Validation);

    /// <summary>
    /// Tahsilatı olan tanım silinmez, pasife alınır (boş hesap silme kuralının
    /// aynısı): bağ kopsaydı geçmiş tahsilatın hangi POS'tan geldiği kaybolurdu.
    /// </summary>
    public static readonly ApplicationError HasSettlements = new(
        "pos_definitions.has_settlements",
        "A pos definition with settlements cannot be deleted; deactivate it instead.",
        ApplicationErrorType.Conflict);

    /// <summary>Pasif tanımla yeni tahsilat yazılmaz.</summary>
    public static readonly ApplicationError Inactive = new(
        "pos_definitions.inactive",
        "An inactive pos definition cannot be used for a new settlement.",
        ApplicationErrorType.Conflict);

    public static ApplicationError NotFound(Guid id) => new(
        "pos_definitions.not_found",
        $"Pos definition '{id}' was not found.",
        ApplicationErrorType.NotFound);

    public static ApplicationError Conflict(string message) => new(
        "pos_definitions.conflict",
        message,
        ApplicationErrorType.Conflict);

    public static ApplicationError Validation(string message) => new(
        "pos_definitions.validation",
        message,
        ApplicationErrorType.Validation);
}
