using Microsoft.AspNetCore.Identity;
using BusinessFinance.Application.Authentication;
using BusinessFinance.Application.UserAccount;

namespace BusinessFinance.Infrastructure.Identity;

public sealed class IdentityAccountService : IIdentityAccountService
{
    private const string DummyPassword = "Not-A-Real-Password-123!";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TimeProvider _timeProvider;
    private readonly ApplicationUser _dummyUser;
    private readonly string _dummyPasswordHash;

    public IdentityAccountService(
        UserManager<ApplicationUser> userManager,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
        _dummyUser = new ApplicationUser(
            Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            "dummy@invalid.local",
            DateTimeOffset.UnixEpoch);
        _dummyPasswordHash = _userManager.PasswordHasher.HashPassword(
            _dummyUser,
            DummyPassword);
    }

    public async Task<IdentityRegistrationResult> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            return new IdentityRegistrationResult(
                IdentityRegistrationStatus.InvalidRegistration,
                null,
                null);
        }

        var user = new ApplicationUser(
            Guid.NewGuid(),
            email,
            _timeProvider.GetUtcNow());
        var result = await _userManager.CreateAsync(user, password);

        cancellationToken.ThrowIfCancellationRequested();

        if (result.Succeeded)
        {
            return new IdentityRegistrationResult(
                IdentityRegistrationStatus.Succeeded,
                user.Id,
                user.Email);
        }

        var errorCodes = result.Errors
            .Select(error => error.Code)
            .ToArray();

        if (errorCodes.Contains("DuplicateEmail", StringComparer.Ordinal) ||
            errorCodes.Contains("DuplicateUserName", StringComparer.Ordinal))
        {
            return new IdentityRegistrationResult(
                IdentityRegistrationStatus.DuplicateEmail,
                null,
                null);
        }

        if (errorCodes.Any(code => code.StartsWith("Password", StringComparison.Ordinal)))
        {
            return new IdentityRegistrationResult(
                IdentityRegistrationStatus.InvalidPassword,
                null,
                null);
        }

        return new IdentityRegistrationResult(
            IdentityRegistrationStatus.InvalidRegistration,
            null,
            null);
    }

    public async Task<AuthenticatedIdentity?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var suppliedPassword = password ?? string.Empty;
        ApplicationUser? user = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            user = await _userManager.FindByEmailAsync(email.Trim());
        }

        if (user is null)
        {
            _userManager.PasswordHasher.VerifyHashedPassword(
                _dummyUser,
                _dummyPasswordHash,
                suppliedPassword);
            return null;
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(
            user,
            suppliedPassword);

        cancellationToken.ThrowIfCancellationRequested();

        if (!passwordIsValid || !user.IsActive || user.Email is null)
        {
            return null;
        }

        return new AuthenticatedIdentity(user.Id, user.Email);
    }

    public async Task<UserAccountProfile?> FindAccountAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await FindActiveUserAsync(userId, cancellationToken);

        return user is { Email: not null }
            ? new UserAccountProfile(
                user.Id,
                user.Email,
                user.EmailConfirmed,
                user.CreatedAtUtc)
            : null;
    }

    public async Task<bool> VerifyPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await FindActiveUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password ?? string.Empty);
        cancellationToken.ThrowIfCancellationRequested();
        return isValid;
    }

    public async Task<PasswordChangeStatus> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await FindActiveUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return PasswordChangeStatus.UserNotFound;
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            currentPassword ?? string.Empty,
            newPassword ?? string.Empty);

        cancellationToken.ThrowIfCancellationRequested();

        if (result.Succeeded)
        {
            return PasswordChangeStatus.Succeeded;
        }

        var errorCodes = result.Errors
            .Select(error => error.Code)
            .ToArray();

        if (errorCodes.Contains("PasswordMismatch", StringComparer.Ordinal))
        {
            return PasswordChangeStatus.InvalidCurrentPassword;
        }

        return errorCodes.Any(code => code.StartsWith("Password", StringComparison.Ordinal))
            ? PasswordChangeStatus.PasswordPolicy
            : PasswordChangeStatus.InvalidCurrentPassword;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        cancellationToken.ThrowIfCancellationRequested();

        if (user is null)
        {
            return;
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "The identity record could not be deleted.");
        }
    }

    private async Task<ApplicationUser?> FindActiveUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (userId == Guid.Empty)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        cancellationToken.ThrowIfCancellationRequested();

        return user is { IsActive: true } ? user : null;
    }

    public async Task<AuthenticatedIdentity?> FindActiveByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (userId == Guid.Empty)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        cancellationToken.ThrowIfCancellationRequested();

        return user is { IsActive: true, Email: not null }
            ? new AuthenticatedIdentity(user.Id, user.Email)
            : null;
    }
}
