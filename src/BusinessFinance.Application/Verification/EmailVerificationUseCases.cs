using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Verification;

/// <summary>
/// Kullanıcının kendi e-postasına doğrulama kodu göndertmesi.
/// </summary>
/// <remarks>
/// Doğrulanmamış hesap <b>kilitli değildir</b>: kullanıcı uygulamayı olduğu gibi
/// kullanır, yalnız uygulama içinde kalıcı bir uyarı taşır. Kilitlemek, dış
/// posta servisi çöktüğünde ürünü kullanılamaz hâle getirirdi.
/// </remarks>
public sealed class SendEmailVerificationUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IVerificationCodeService codeService,
    IVerificationCodeRepository repository,
    IVerificationEmailSender emailSender,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<SendEmailVerificationResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<SendEmailVerificationResponse>.Failure(
                VerificationErrors.AuthenticationRequired);
        }

        var account = await identityAccountService.FindAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return ApplicationResult<SendEmailVerificationResponse>.Failure(
                VerificationErrors.AuthenticationRequired);
        }

        if (account.EmailConfirmed)
        {
            // Zaten doğrulanmış hesaba kod göndermek gereksiz posta ve gereksiz
            // bir "yanlış yaptınız" cevabı olurdu.
            return ApplicationResult<SendEmailVerificationResponse>.Success(
                new SendEmailVerificationResponse(AlreadyConfirmed: true, CodeSent: false));
        }

        var utcNow = timeProvider.GetUtcNow();
        var latest = await repository.FindLatestAsync(
            userId,
            VerificationPurpose.EmailConfirmation,
            cancellationToken);

        if (latest is not null &&
            latest.IsUsable(utcNow) &&
            utcNow - latest.CreatedAtUtc < VerificationPolicy.ResendInterval)
        {
            return ApplicationResult<SendEmailVerificationResponse>.Failure(
                VerificationErrors.CodeRequestedTooSoon);
        }

        var sent = await VerificationCodeIssuer.IssueAsync(
            userId,
            account.Email,
            VerificationPurpose.EmailConfirmation,
            codeService,
            repository,
            emailSender,
            utcNow,
            cancellationToken);

        return ApplicationResult<SendEmailVerificationResponse>.Success(
            new SendEmailVerificationResponse(AlreadyConfirmed: false, CodeSent: sent));
    }
}

public sealed class ConfirmEmailUseCase(
    ICurrentUser currentUser,
    IIdentityAccountService identityAccountService,
    IVerificationCodeService codeService,
    IVerificationCodeRepository repository,
    TimeProvider timeProvider)
{
    public async Task<ApplicationResult<ConfirmEmailResponse>> ExecuteAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<ConfirmEmailResponse>.Failure(
                VerificationErrors.AuthenticationRequired);
        }

        var matched = await VerificationCodeIssuer.TryConsumeAsync(
            userId,
            VerificationPurpose.EmailConfirmation,
            command.Code,
            codeService,
            repository,
            timeProvider.GetUtcNow(),
            cancellationToken);

        if (!matched)
        {
            return ApplicationResult<ConfirmEmailResponse>.Failure(
                VerificationErrors.InvalidVerificationCode);
        }

        await identityAccountService.MarkEmailConfirmedAsync(userId, cancellationToken);
        return ApplicationResult<ConfirmEmailResponse>.Success(new ConfirmEmailResponse());
    }
}

/// <summary>
/// Kod üretme ve tüketme adımlarının iki akışta da aynı kalması için tek yer.
/// </summary>
internal static class VerificationCodeIssuer
{
    public static async Task<bool> IssueAsync(
        Guid userId,
        string email,
        VerificationPurpose purpose,
        IVerificationCodeService codeService,
        IVerificationCodeRepository repository,
        IVerificationEmailSender emailSender,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken)
    {
        // Yeni kod eskisini öldürür: aynı anda iki geçerli kod, birini gören
        // saldırgana ikinci bir şans verirdi.
        await repository.ConsumeAllAsync(userId, purpose, utcNow, cancellationToken);

        var code = codeService.CreateCode();
        var expiresAtUtc = utcNow.Add(VerificationPolicy.CodeLifetime);
        await repository.AddAsync(
            new VerificationCode(
                Guid.NewGuid(),
                userId,
                purpose,
                codeService.HashCode(code),
                utcNow,
                expiresAtUtc),
            cancellationToken);

        var status = await emailSender.SendCodeAsync(
            email,
            purpose,
            code,
            expiresAtUtc,
            cancellationToken);

        return status == EmailDeliveryStatus.Sent;
    }

    /// <summary>
    /// Kodu doğrular ve doğruysa tüketir. Yanlış deneme sayılır: beşinci
    /// yanlıştan sonra kod ölür ve kullanıcı yenisini istemek zorunda kalır.
    /// </summary>
    public static async Task<bool> TryConsumeAsync(
        Guid userId,
        VerificationPurpose purpose,
        string? suppliedCode,
        IVerificationCodeService codeService,
        IVerificationCodeRepository repository,
        DateTimeOffset utcNow,
        CancellationToken cancellationToken)
    {
        var code = await repository.FindLatestAsync(userId, purpose, cancellationToken);
        if (code is null || !code.IsUsable(utcNow))
        {
            return false;
        }

        var normalized = suppliedCode?.Trim() ?? string.Empty;
        if (normalized.Length == 0 ||
            !string.Equals(
                code.CodeHash,
                codeService.HashCode(normalized),
                StringComparison.Ordinal))
        {
            code.RegisterFailedAttempt();
            await repository.UpdateAsync(code, cancellationToken);
            return false;
        }

        code.Consume(utcNow);
        await repository.UpdateAsync(code, cancellationToken);
        return true;
    }
}
