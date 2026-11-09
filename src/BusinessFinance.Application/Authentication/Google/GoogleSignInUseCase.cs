using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication.LoginUser;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.Google;

/// <param name="Username">
/// Yalnız yeni hesabı açan ikinci istekte ("Bir adım kaldı" → <c>Devam et</c>)
/// gelir. Boşsa ve hesap yoksa hesap açılmaz, öneri döner.
/// </param>
public sealed record GoogleSignInCommand(
    string IdToken,
    string? Username,
    bool HasBusiness);

/// <summary>Ya oturum açılmıştır ya da yeni hesap için kullanıcı adı gerekir.</summary>
public sealed record GoogleSignInResult(
    LoginUserResponse? Session,
    GoogleSignupRequired? SignupRequired);

/// <param name="SuggestedUsername">
/// E-postadan türetilmiş, şu an boş olan ad; uygun bir ad bulunamazsa null.
/// </param>
public sealed record GoogleSignupRequired(string Email, string? SuggestedUsername);

/// <summary>
/// Google ile giriş ve kayıt (Aşama 06.4; kararlar 9 Kasım 2026, devir notu
/// §12). Belirteç her istekte sunucuda doğrulanır; istemcinin söylediği
/// e-postaya güvenilmez.
/// </summary>
/// <remarks>
/// <para>
/// <b>Hesap <c>Devam et</c>'te açılır</b> (D1): Google hesabı tanınmıyorsa ilk
/// istek hiçbir şey yazmaz, kullanıcı adı önerisiyle döner. Vazgeçen kişi iz
/// bırakmaz.
/// </para>
/// <para>
/// <b>Aynı e-postalı hesap Google'a bağlanır</b> (D4, G2): Google e-postanın
/// sahibini doğruladıysa hesap sormadan bağlanır. Hesabın e-postası hiç
/// doğrulanmamışsa hesabı başkası açmış olabilir: parola kaldırılır ve açık
/// oturumlar kapanır, hesap e-postanın gerçek sahibine geçer (Firebase'in
/// "güvenilir sağlayıcı" davranışı). Aynı kişiyse parolasını <c>Parolamı
/// unuttum</c> ile yeniden koyar. Doğrulanmış hesapta parola durur. Google
/// e-postayı doğrulamadıysa bağlanmaz.
/// </para>
/// </remarks>
public sealed class GoogleSignInUseCase(
    IGoogleIdTokenValidator tokenValidator,
    IExternalAccountService externalAccounts,
    IIdentityAccountService identityAccounts,
    IUserProfileRepository userProfiles,
    ISecurityTokenService securityTokenService,
    IRefreshSessionRepository refreshSessions,
    TimeProvider timeProvider)
{
    public const string Provider = "Google";

    /// <summary>Öneri bulunamazsa vazgeçmeden önce denenecek sayı.</summary>
    private const int MaxSuggestionAttempts = 50;

    public async Task<ApplicationResult<GoogleSignInResult>> ExecuteAsync(
        GoogleSignInCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var google = string.IsNullOrWhiteSpace(command.IdToken)
            ? null
            : await tokenValidator.ValidateAsync(command.IdToken, cancellationToken);
        if (google is null)
        {
            return Failure(AuthenticationErrors.GoogleInvalidToken);
        }

        var linked = await externalAccounts.FindByLoginAsync(
            Provider,
            google.Subject,
            cancellationToken);
        if (linked is not null)
        {
            return linked.IsActive
                ? await SignInAsync(linked, cancellationToken)
                : Failure(AuthenticationErrors.InvalidCredentials);
        }

        var existing = await externalAccounts.FindByEmailAsync(google.Email, cancellationToken);
        if (existing is not null)
        {
            return await LinkAsync(existing, google, cancellationToken);
        }

        if (command.Username is null)
        {
            return ApplicationResult<GoogleSignInResult>.Success(new GoogleSignInResult(
                null,
                new GoogleSignupRequired(
                    google.Email,
                    await SuggestUsernameAsync(google.Email, cancellationToken))));
        }

        return await CreateAsync(command, google, cancellationToken);
    }

    private async Task<ApplicationResult<GoogleSignInResult>> LinkAsync(
        ExternalAccount existing,
        GoogleIdentity google,
        CancellationToken cancellationToken)
    {
        if (!existing.IsActive)
        {
            return Failure(AuthenticationErrors.InvalidCredentials);
        }

        if (!google.EmailVerified)
        {
            return Failure(AuthenticationErrors.GoogleEmailUnverified);
        }

        var takeOver = !existing.EmailConfirmed;
        await externalAccounts.LinkAsync(
            existing.UserId,
            Provider,
            google.Subject,
            removePassword: takeOver,
            cancellationToken);

        if (takeOver)
        {
            await refreshSessions.RevokeAllActiveForUserAsync(
                existing.UserId,
                timeProvider.GetUtcNow(),
                cancellationToken);
        }

        return await SignInAsync(existing, cancellationToken);
    }

    private async Task<ApplicationResult<GoogleSignInResult>> CreateAsync(
        GoogleSignInCommand command,
        GoogleIdentity google,
        CancellationToken cancellationToken)
    {
        var username = Usernames.Normalize(command.Username!);
        if (!Usernames.IsValid(username))
        {
            return Failure(AuthenticationErrors.InvalidUsername);
        }

        var registration = await externalAccounts.CreateAsync(
            google.Email,
            google.EmailVerified,
            username,
            Provider,
            google.Subject,
            cancellationToken);

        if (registration is not { Status: IdentityRegistrationStatus.Succeeded, UserId: Guid userId })
        {
            return Failure(registration.Status switch
            {
                IdentityRegistrationStatus.DuplicateUsername => AuthenticationErrors.UsernameTaken,
                IdentityRegistrationStatus.DuplicateEmail => AuthenticationErrors.RegistrationConflict,
                _ => AuthenticationErrors.InvalidRegistration
            });
        }

        // Kayıtla aynı gerekçe: profil hemen yazılır, varsayılan kategori seti
        // ilk okumada bu satıra bakar.
        await userProfiles.AddAsync(new UserProfile(userId, command.HasBusiness), cancellationToken);

        var session = await SessionIssuance.IssueAsync(
            new AuthenticatedIdentity(userId, registration.Email ?? google.Email),
            securityTokenService,
            refreshSessions,
            cancellationToken);
        return ApplicationResult<GoogleSignInResult>.Success(new GoogleSignInResult(session, null));
    }

    private async Task<ApplicationResult<GoogleSignInResult>> SignInAsync(
        ExternalAccount account,
        CancellationToken cancellationToken)
    {
        var identity = await identityAccounts.FindActiveByIdAsync(account.UserId, cancellationToken);
        if (identity is null)
        {
            return Failure(AuthenticationErrors.InvalidCredentials);
        }

        var session = await SessionIssuance.IssueAsync(
            identity,
            securityTokenService,
            refreshSessions,
            cancellationToken);
        return ApplicationResult<GoogleSignInResult>.Success(new GoogleSignInResult(session, null));
    }

    private async Task<string?> SuggestUsernameAsync(string email, CancellationToken cancellationToken)
    {
        var suggestion = Usernames.SuggestFromEmail(email);
        for (var attempt = 1; attempt <= MaxSuggestionAttempts; attempt++)
        {
            var candidate = attempt == 1 ? suggestion : Usernames.WithNumber(suggestion, attempt);
            if (!await identityAccounts.IsUsernameTakenAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        return null;
    }

    private static ApplicationResult<GoogleSignInResult> Failure(ApplicationError error) =>
        ApplicationResult<GoogleSignInResult>.Failure(error);
}
