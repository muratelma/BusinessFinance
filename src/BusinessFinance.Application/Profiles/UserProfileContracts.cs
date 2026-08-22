using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Profiles;

public sealed record UserProfileDto(bool HasBusiness);

public sealed record SetUserProfileCommand(bool HasBusiness);

public interface IUserProfileRepository
{
    Task<UserProfile?> FindAsync(Guid userId, bool track, CancellationToken cancellationToken);
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken);
    Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken);
}

public static class UserProfileErrors
{
    public static readonly ApplicationError AuthenticationRequired = new(
        "authentication.required",
        "An authenticated user is required.",
        ApplicationErrorType.Unauthorized);
}

public sealed class GetUserProfileUseCase(
    ICurrentUser currentUser,
    IUserProfileRepository repository)
{
    public async Task<ApplicationResult<UserProfileDto>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<UserProfileDto>.Failure(
                UserProfileErrors.AuthenticationRequired);
        }

        var profile = await repository.FindAsync(userId, track: false, cancellationToken);

        // Profili olmayan kullanıcı "işletmesi yok" sayılır. Bu bir varsayım
        // değil, sorunun sorulmadığı hâlin doğru cevabı: kapsam boyutu gizli
        // kalır ve hiçbir kayıt yanlış etiketlenmez.
        return ApplicationResult<UserProfileDto>.Success(
            new UserProfileDto(profile?.HasBusiness ?? false));
    }
}

public sealed class SetUserProfileUseCase(
    ICurrentUser currentUser,
    IUserProfileRepository repository)
{
    public async Task<ApplicationResult<UserProfileDto>> ExecuteAsync(
        SetUserProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (currentUser.UserId is not Guid userId || userId == Guid.Empty)
        {
            return ApplicationResult<UserProfileDto>.Failure(
                UserProfileErrors.AuthenticationRequired);
        }

        var profile = await repository.FindAsync(userId, track: true, cancellationToken);
        if (profile is null)
        {
            await repository.AddAsync(
                new UserProfile(userId, command.HasBusiness),
                cancellationToken);
        }
        else
        {
            profile.SetHasBusiness(command.HasBusiness);
            await repository.UpdateAsync(profile, cancellationToken);
        }

        return ApplicationResult<UserProfileDto>.Success(
            new UserProfileDto(command.HasBusiness));
    }
}
