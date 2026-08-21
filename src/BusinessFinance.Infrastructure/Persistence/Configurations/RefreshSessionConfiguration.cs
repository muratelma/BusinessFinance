using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("RefreshSessions", table =>
        {
            table.HasCheckConstraint(
                "CK_RefreshSessions_Expiry",
                "[ExpiresAtUtc] > [CreatedAtUtc]");
            table.HasCheckConstraint(
                "CK_RefreshSessions_Revocation",
                "[RevokedAtUtc] IS NULL OR [RevokedAtUtc] >= [CreatedAtUtc]");
            table.HasCheckConstraint(
                "CK_RefreshSessions_ReuseDetection",
                "[ReuseDetectedAtUtc] IS NULL OR [ReuseDetectedAtUtc] >= [CreatedAtUtc]");
        });

        builder.HasKey(session => session.Id);
        builder.Property(session => session.TokenHash)
            .IsUnicode(false)
            .HasMaxLength(RefreshSession.MaximumTokenHashLength)
            .IsRequired();
        builder.Property(session => session.CreatedAtUtc).HasPrecision(0);
        builder.Property(session => session.ExpiresAtUtc).HasPrecision(0);
        builder.Property(session => session.RevokedAtUtc).HasPrecision(0);
        builder.Property(session => session.ReuseDetectedAtUtc).HasPrecision(0);

        builder.HasIndex(session => session.TokenHash)
            .IsUnique()
            .HasDatabaseName("UX_RefreshSessions_TokenHash");
        builder.HasIndex(session => new { session.UserId, session.RevokedAtUtc })
            .HasDatabaseName("IX_RefreshSessions_UserId_RevokedAtUtc");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<RefreshSession>()
            .WithMany()
            .HasForeignKey(session => session.ReplacedBySessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
