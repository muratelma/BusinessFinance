using Microsoft.AspNetCore.Identity;

namespace BusinessFinance.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public bool IsActive { get; private set; }

    private ApplicationUser()
    {
    }

    public ApplicationUser(
        Guid id,
        string email,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Created-at time must use the UTC offset.",
                nameof(createdAtUtc));
        }

        Id = id;
        Email = email.Trim();
        UserName = Email;
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
