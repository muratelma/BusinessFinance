using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Application.Verification;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Authentication.RegisterUser;

public sealed class RegisterUserUseCase
{
    private readonly IIdentityAccountService _identityAccountService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IVerificationCodeService _codeService;
    private readonly IVerificationCodeRepository _codeRepository;
    private readonly IVerificationEmailSender _emailSender;
    private readonly TimeProvider _timeProvider;

    public RegisterUserUseCase(
        IIdentityAccountService identityAccountService,
        IUserProfileRepository userProfileRepository,
        IVerificationCodeService codeService,
        IVerificationCodeRepository codeRepository,
        IVerificationEmailSender emailSender,
        TimeProvider timeProvider)
    {
        _identityAccountService = identityAccountService;
        _userProfileRepository = userProfileRepository;
        _codeService = codeService;
        _codeRepository = codeRepository;
        _emailSender = emailSender;
        _timeProvider = timeProvider;
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

            // Doğrulama kodu kayıtla birlikte gider ama kaydı **rehin almaz**:
            // posta servisi ulaşılamazsa bile hesap açılmış olur ve kullanıcı
            // giriş yapabilir. Doğrulama uygulama içinden yeniden istenebilir.
            if (registration.Email is not null)
            {
                await VerificationCodeIssuer.IssueAsync(
                    registeredUserId,
                    registration.Email,
                    VerificationPurpose.EmailConfirmation,
                    _codeService,
                    _codeRepository,
                    _emailSender,
                    _timeProvider.GetUtcNow(),
                    cancellationToken);
            }
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
