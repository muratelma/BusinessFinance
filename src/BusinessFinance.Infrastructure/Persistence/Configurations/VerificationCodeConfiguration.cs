using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("VerificationCodes", table =>
        {
            table.HasCheckConstraint(
                "CK_VerificationCodes_Expiry",
                "[ExpiresAtUtc] > [CreatedAtUtc]");
            table.HasCheckConstraint(
                "CK_VerificationCodes_Consumption",
                "[ConsumedAtUtc] IS NULL OR [ConsumedAtUtc] >= [CreatedAtUtc]");
            table.HasCheckConstraint(
                "CK_VerificationCodes_FailedAttempts",
                "[FailedAttemptCount] >= 0");
            table.HasCheckConstraint(
                "CK_VerificationCodes_Purpose",
                "[Purpose] IN (1, 2)");
        });

        builder.HasKey(code => code.Id);
        builder.Property(code => code.Purpose).HasConversion<int>();
        builder.Property(code => code.CodeHash)
            .IsUnicode(false)
            .HasMaxLength(VerificationCode.CodeHashLength)
            .IsRequired();
        builder.Property(code => code.CreatedAtUtc).HasPrecision(0);
        builder.Property(code => code.ExpiresAtUtc).HasPrecision(0);
        builder.Property(code => code.ConsumedAtUtc).HasPrecision(0);

        // Kod her zaman "bu kullanıcının, bu amaç için ürettiği en sonuncusu"
        // olarak aranır; tarih sırası indeksin parçası.
        builder.HasIndex(code => new
        {
            code.UserId,
            code.Purpose,
            code.CreatedAtUtc
        }).HasDatabaseName("IX_VerificationCodes_UserId_Purpose_CreatedAtUtc");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(code => code.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
