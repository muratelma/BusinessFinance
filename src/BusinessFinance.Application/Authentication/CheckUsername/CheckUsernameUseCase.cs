using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Authentication.CheckUsername;

/// <summary>
/// Kayıt formunda kullanıcı adının alınıp alınmadığını söyler: kişi alandan
/// çıkınca ve <c>Hesap oluştur</c>'a basınca sorulur, ikinci adıma geçmeden
/// "Bu kullanıcı adı alınmış" görür (kullanıcı kararı, 8 Kasım 2026).
/// </summary>
/// <remarks>
/// Anonimdir ve oran sınırlıdır. Bir kullanıcı adının var olduğu dışarıdan
/// öğrenilebilir; kullanıcı adı gizli bilgi değildir ve e-posta sorulmaz.
/// Cevap kesin değildir: kayıt anında aynı ad alınmışsa kayıt
/// <c>authentication.username_taken</c> ile reddedilir.
/// </remarks>
public sealed class CheckUsernameUseCase
{
    private readonly IIdentityAccountService _identityAccountService;

    public CheckUsernameUseCase(IIdentityAccountService identityAccountService)
    {
        _identityAccountService = identityAccountService;
    }

    public async Task<ApplicationResult<UsernameAvailability>> ExecuteAsync(
        string? username,
        CancellationToken cancellationToken = default)
    {
        var normalized = Usernames.Normalize(username ?? string.Empty);
        if (!Usernames.IsValid(normalized))
        {
            return ApplicationResult<UsernameAvailability>.Failure(
                AuthenticationErrors.InvalidUsername);
        }

        var taken = await _identityAccountService.IsUsernameTakenAsync(
            normalized,
            cancellationToken);

        return ApplicationResult<UsernameAvailability>.Success(
            new UsernameAvailability(normalized, !taken));
    }
}

public sealed record UsernameAvailability(string Username, bool Available);
