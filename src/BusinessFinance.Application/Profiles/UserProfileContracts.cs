using BusinessFinance.Application.Abstractions.Authentication;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Counterparties;
using BusinessFinance.Domain;

namespace BusinessFinance.Application.Profiles;

/// <param name="HasBusiness">Kaydolurken sorulan sorunun cevabı.</param>
/// <param name="HasCounterpartyLedger">
/// Kullanıcının cari hareketi var. Cevap "yok" olsa da istemci <c>Cari
/// hesap</c> kapısını bu durumda gösterir: gizleme bir ön ayardır, kilit
/// değildir (ADR 0020).
/// </param>
public sealed record UserProfileDto(bool HasBusiness, bool HasCounterpartyLedger = false);

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
    IUserProfileRepository repository,
    ICounterpartyRepository counterpartyRepository)
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
            new UserProfileDto(
                profile?.HasBusiness ?? false,
                await counterpartyRepository.HasLedgerEntriesAsync(userId, cancellationToken)));
    }
}

public sealed class SetUserProfileUseCase(
    ICurrentUser currentUser,
    IUserProfileRepository repository,
    ICounterpartyRepository counterpartyRepository)
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
            new UserProfileDto(
                command.HasBusiness,
                await counterpartyRepository.HasLedgerEntriesAsync(userId, cancellationToken)));
    }
}
