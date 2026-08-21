using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Authentication;

public static class AuthenticationErrors
{
    public static readonly ApplicationError InvalidCredentials = new(
        "authentication.invalid_credentials",
        "Email or password is invalid.",
        ApplicationErrorType.Unauthorized);

    public static readonly ApplicationError RegistrationConflict = new(
        "authentication.registration_conflict",
        "Registration could not be completed.",
        ApplicationErrorType.Conflict);

    public static readonly ApplicationError PasswordPolicy = new(
        "authentication.password_policy",
        "Password does not meet the required policy.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidRegistration = new(
        "authentication.invalid_registration",
        "Registration information is invalid.",
        ApplicationErrorType.Validation);

    public static readonly ApplicationError InvalidRefreshToken = new(
        "authentication.invalid_refresh_token",
        "Refresh token is invalid or expired.",
        ApplicationErrorType.Unauthorized);
}
