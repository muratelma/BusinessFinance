using BusinessFinance.Domain;

namespace BusinessFinance.Application.Accounts.UpdateAccount;

/// <summary>
/// Hesabın tam güncel hâli. <see cref="DefaultScope"/> yetkilidir: boş
/// gönderilmesi "bu hesap artık kapsam belirlemiyor" demektir, "dokunma"
/// değil — aynı <see cref="Name"/> ve <see cref="IsActive"/> gibi.
/// </summary>
public sealed record UpdateAccountCommand(
    Guid AccountId,
    string Name,
    bool IsActive,
    TransactionScope? DefaultScope);
