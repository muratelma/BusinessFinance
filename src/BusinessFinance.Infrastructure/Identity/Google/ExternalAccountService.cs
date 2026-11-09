using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace BusinessFinance.Infrastructure.Identity.Google;

/// <summary>
/// Google girişini Identity'nin <c>AspNetUserLogins</c> tablosuna bağlar
/// (InitialCreate'ten beri var; şema değişmedi).
/// </summary>
public sealed class ExternalAccountService(
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider) : IExternalAccountService
{
    public async Task<ExternalAccount?> FindByLoginAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ToAccount(await userManager.FindByLoginAsync(provider, providerKey));
    }

    public async Task<ExternalAccount?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return string.IsNullOrWhiteSpace(email)
            ? null
            : ToAccount(await userManager.FindByEmailAsync(email.Trim()));
    }

    public async Task LinkAsync(
        Guid userId,
        string provider,
        string providerKey,
        bool removePassword,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException("The account to link was not found.");

        if (removePassword && await userManager.HasPasswordAsync(user))
        {
            Ensure(await userManager.RemovePasswordAsync(user), "remove the password");
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            Ensure(await userManager.UpdateAsync(user), "confirm the email");
        }

        Ensure(
            await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider)),
            "link the Google account");
    }

    public async Task<IdentityRegistrationResult> CreateAsync(
        string email,
        bool emailConfirmed,
        string username,
        string provider,
        string providerKey,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = new ApplicationUser(Guid.NewGuid(), email, timeProvider.GetUtcNow(), username)
        {
            EmailConfirmed = emailConfirmed
        };

        var created = await userManager.CreateAsync(user);
        if (!created.Succeeded)
        {
            var codes = created.Errors.Select(error => error.Code).ToArray();
            var status = codes.Contains("DuplicateEmail", StringComparer.Ordinal)
                ? IdentityRegistrationStatus.DuplicateEmail
                : codes.Contains("DuplicateUserName", StringComparer.Ordinal)
                    ? IdentityRegistrationStatus.DuplicateUsername
                    : IdentityRegistrationStatus.InvalidRegistration;
            return new IdentityRegistrationResult(status, null, null);
        }

        // Bağ yazılamazsa hesap parolasız ve bağsız kalır; aynı kişi yeniden
        // denediğinde e-postayla bulunur ve bağlanır (e-postası doğrulanmış).
        Ensure(
            await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider)),
            "link the Google account");

        return new IdentityRegistrationResult(IdentityRegistrationStatus.Succeeded, user.Id, user.Email);
    }

    private static ExternalAccount? ToAccount(ApplicationUser? user) =>
        user is { Email: not null }
            ? new ExternalAccount(user.Id, user.Email, user.IsActive, user.EmailConfirmed)
            : null;

    private static void Ensure(IdentityResult result, string action)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Could not {action}.");
        }
    }
}
