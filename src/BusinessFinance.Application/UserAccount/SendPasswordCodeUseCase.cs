using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.UserAccount;

public sealed record SendPasswordCodeResponse(bool CodeSent);

/// <summary>
/// "Parola belirle" için oturumdaki kullanıcının kendi e-postasına kod
/// gönderir (9 Kasım 2026). Kod sıfırlamanın kodudur ve parola değiştirme
/// ucunda mevcut parolanın yerine geçer.
/// </summary>
/// <remarks>
/// Anonim sıfırlama isteğinden farkı: o her adrese aynı cevabı verir
/// (hesabın varlığını söylememek için), bu ise oturumdaki kullanıcının kendi
/// adresi için postanın gerçekten gidip gitmediğini söyler. Gitmediyse ekran
/// "gönderdik" diyemez.
/// </remarks>
public sealed class SendPasswordCodeUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IVerificationCodeService codeService,
    IVerificationCodeRepository repository,
    IVerificationEmailSender emailSender,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<SendPasswordCodeResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<SendPasswordCodeResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var account = await identityAccountService.FindAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return ApplicationResult<SendPasswordCodeResponse>.Failure(
                UserAccountErrors.AuthenticationRequired);
        }

        var utcNow = timeProvider.GetUtcNow();
        var latest = await repository.FindLatestAsync(
            userId,
            VerificationPurpose.PasswordReset,
            cancellationToken);

        if (latest is not null &&
            latest.IsUsable(utcNow) &&
            utcNow - latest.CreatedAtUtc < VerificationPolicy.ResendInterval)
        {
            return ApplicationResult<SendPasswordCodeResponse>.Failure(
                VerificationErrors.CodeRequestedTooSoon);
        }

        var sent = await VerificationCodeIssuer.IssueAsync(
            userId,
            account.Email,
            VerificationPurpose.PasswordReset,
            codeService,
            repository,
            emailSender,
            utcNow,
            cancellationToken);

        return ApplicationResult<SendPasswordCodeResponse>.Success(
            new SendPasswordCodeResponse(sent));
    }
}
