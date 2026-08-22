using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.RegisterUser;

public sealed class RegisterUserUseCase
{
    private readonly IIdentityAccountService _identityAccountService;
    private readonly IUserProfileRepository _userProfileRepository;

    public RegisterUserUseCase(
        IIdentityAccountService identityAccountService,
        IUserProfileRepository userProfileRepository)
    {
        _identityAccountService = identityAccountService;
        _userProfileRepository = userProfileRepository;
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

        // Profil kayıttan hemen sonra yazılır: varsayılan kategori seti ilk
        // kategori okumasında kuruluyor ve o an hangi setin geleceğini bu
        // satır belirliyor. Sonraya bırakmak, kişisel setle başlayan bir
        // esnafa işletme kalemlerini bir daha hiç veremezdi — set yalnız hiç
        // kategorisi olmayan kullanıcıya bir kez uygulanır.
        if (registration.Status == IdentityRegistrationStatus.Succeeded &&
            registration.UserId is Guid registeredUserId)
        {
            await _userProfileRepository.AddAsync(
                new UserProfile(registeredUserId, command.HasBusiness),
                cancellationToken);
        }

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
