using BusinessFinance.Application.Abstractions.Results;

namespace BusinessFinance.Application.Authentication.RegisterUser;

public sealed class RegisterUserUseCase
{
    private readonly IIdentityAccountService _identityAccountService;

    public RegisterUserUseCase(IIdentityAccountService identityAccountService)
    {
        _identityAccountService = identityAccountService;
    }

    public async Task<ApplicationResult<RegisterUserResponse>> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var registration = await _identityAccountService.RegisterAsync(
            command.Email,
            command.Password,
            cancellationToken);

        return registration.Status switch
        {
            IdentityRegistrationStatus.Succeeded
                when registration.UserId is Guid userId && registration.Email is not null
                => ApplicationResult<RegisterUserResponse>.Success(
                    new RegisterUserResponse(userId, registration.Email)),
            IdentityRegistrationStatus.DuplicateEmail
                => ApplicationResult<RegisterUserResponse>.Failure(
                    AuthenticationErrors.RegistrationConflict),
            IdentityRegistrationStatus.InvalidPassword
                => ApplicationResult<RegisterUserResponse>.Failure(
                    AuthenticationErrors.PasswordPolicy),
            _ => ApplicationResult<RegisterUserResponse>.Failure(
                AuthenticationErrors.InvalidRegistration)
        };
    }
}
