using BusinessFinance.Api.Contracts;

namespace BusinessFinance.Api.Features.Accounts;

public sealed record CreateAccountRequest(
    string Name,
    string Type,
    string Currency,
    string OpeningBalance = "0",
    string? DefaultScope = null);

/// <summary>
/// Hesabın tam güncel hâli. <see cref="DefaultScope"/> boş gönderilirse
/// varsayılan kapsam kaldırılır; alan yetkilidir, "dokunma" anlamına gelmez.
/// </summary>
public sealed record UpdateAccountRequest(
    string Name,
    bool IsActive,
    string? DefaultScope = null);

public sealed record AccountResponse(
    Guid Id,
    string Name,
    string Type,
    string Currency,
    bool IsActive,
    string OpeningBalance,
    string Balance,
    string? DefaultScope);

public sealed record AccountListResponse(
    IReadOnlyList<AccountResponse> Items,
    PaginationMetadata Pagination);
