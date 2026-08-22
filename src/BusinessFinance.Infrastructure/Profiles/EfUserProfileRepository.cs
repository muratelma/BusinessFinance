using Microsoft.EntityFrameworkCore;
using BusinessFinance.Application.Profiles;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Persistence;

namespace BusinessFinance.Infrastructure.Profiles;

internal sealed class EfUserProfileRepository(BusinessFinanceDbContext dbContext)
    : IUserProfileRepository
{
    public async Task<UserProfile?> FindAsync(
        Guid userId,
        bool track,
        CancellationToken cancellationToken)
    {
        var query = track ? dbContext.UserProfiles : dbContext.UserProfiles.AsNoTracking();
        return await query.SingleOrDefaultAsync(
            profile => profile.UserId == userId,
            cancellationToken);
    }

    public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        await dbContext.UserProfiles.AddAsync(profile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        dbContext.UserProfiles.Update(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
