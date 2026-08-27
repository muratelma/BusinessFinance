using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Tokens;
using BusinessFinance.Application.UserAccount;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Verification;

/// <summary>
/// Parolasını unutan kullanıcının kendi başına sıfırlaması — birinci adım.
/// </summary>
/// <remarks>
/// <b>Cevap her zaman aynıdır.</b> Kayıtlı olmayan bir adres, kayıtlı bir adres,
/// pasif bir hesap ve az önce kod istemiş bir adres — dördü de aynı başarıya
/// döner. Farklı cevap vermek, bir adresin bu üründe hesabı olduğunu söyleyen
/// bir sorgu hâline getirirdi.
/// </remarks>
public sealed class RequestPasswordResetUseCase(
    IIdentityAccountService identityAccountService,
    IVerificationCodeService codeService,
    IVerificationCodeRepository repository,
    IVerificationEmailSender emailSender,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<RequestPasswordResetResponse>> ExecuteAsync(
        RequestPasswordResetCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = command.Email?.Trim() ?? string.Empty;
        if (email.Length == 0)
        {
            return Success();
        }

        var userId = await identityAccountService.FindActiveUserIdByEmailAsync(
            email,
            cancellationToken);

        if (userId is not Guid ownerId)
        {
            return Success();
        }

        var utcNow = timeProvider.GetUtcNow();
        var latest = await repository.FindLatestAsync(
            ownerId,
            VerificationPurpose.PasswordReset,
            cancellationToken);

        if (latest is not null &&
            latest.IsUsable(utcNow) &&
            utcNow - latest.CreatedAtUtc < VerificationPolicy.ResendInterval)
        {
            // Sessizce geçilir: "biraz bekleyin" demek, adresin kayıtlı
            // olduğunu söylemek olurdu.
            return Success();
        }

        var account = await identityAccountService.FindAccountAsync(ownerId, cancellationToken);
        if (account is null)
        {
            return Success();
        }

        await VerificationCodeIssuer.IssueAsync(
            ownerId,
            account.Email,
            VerificationPurpose.PasswordReset,
            codeService,
            repository,
            emailSender,
            utcNow,
            cancellationToken);

        return Success();
    }

    private static ApplicationResult<RequestPasswordResetResponse> Success() =>
        ApplicationResult<RequestPasswordResetResponse>.Success(
            new RequestPasswordResetResponse());
}

/// <summary>
/// Sıfırlamanın ikinci adımı: kod ve yeni parola.
/// </summary>
/// <remarks>
/// Başarılı sıfırlama <b>bütün oturumları kapatır</b>. Parolayı unutmuş
/// olmanın en olası sebeplerinden biri hesabın başkasının elinde olmasıdır;
/// sıfırlama o eli boşaltmıyorsa işe yaramaz.
/// </remarks>
public sealed class ResetPasswordUseCase(
    IIdentityAccountService identityAccountService,
    IVerificationCodeService codeService,
    IVerificationCodeRepository repository,
    IRefreshSessionRepository refreshSessionRepository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ResetPasswordResponse>> ExecuteAsync(
        ResetPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = command.Email?.Trim() ?? string.Empty;
        if (email.Length == 0)
        {
            return InvalidCode();
        }

        var userId = await identityAccountService.FindActiveUserIdByEmailAsync(
            email,
            cancellationToken);

        if (userId is not Guid ownerId)
        {
            return InvalidCode();
        }

        var utcNow = timeProvider.GetUtcNow();
        var matched = await VerificationCodeIssuer.TryConsumeAsync(
            ownerId,
            VerificationPurpose.PasswordReset,
            command.Code,
            codeService,
            repository,
            utcNow,
            cancellationToken);

        if (!matched)
        {
            return InvalidCode();
        }

        var status = await identityAccountService.SetPasswordAsync(
            ownerId,
            command.NewPassword ?? string.Empty,
            cancellationToken);

        if (status == PasswordChangeStatus.PasswordPolicy)
        {
            // Kod tüketildi ve geri alınmaz: politikaya uymayan bir parola
            // denemesi, kodu bir saldırgana bırakmanın bahanesi olmamalı.
            // Kullanıcı yeni kod ister; maliyeti bir e-postadır.
            return ApplicationResult<ResetPasswordResponse>.Failure(
                AuthenticationErrors.PasswordPolicy);
        }

        if (status != PasswordChangeStatus.Succeeded)
        {
            return InvalidCode();
        }

        await refreshSessionRepository.RevokeAllActiveForUserAsync(
            ownerId,
            utcNow,
            cancellationToken);

        return ApplicationResult<ResetPasswordResponse>.Success(new ResetPasswordResponse());
    }

    private static ApplicationResult<ResetPasswordResponse> InvalidCode() =>
        ApplicationResult<ResetPasswordResponse>.Failure(
            VerificationErrors.InvalidResetCode);
}
