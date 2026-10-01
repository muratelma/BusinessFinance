using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", table =>
        {
            table.HasCheckConstraint("CK_Categories_Type", "[Type] IN (1, 2)");
            table.HasCheckConstraint(
                "CK_Categories_DefaultScope",
                "[DefaultScope] IS NULL OR [DefaultScope] IN (1, 2)");

            // Vergi bir nakit çıkışıdır; yalnız gider kategorisi işaretlenir.
            table.HasCheckConstraint("CK_Categories_IsTax", "[IsTax] = 0 OR [Type] = 2");
        });

        builder.HasKey(category => category.Id);
        builder.HasAlternateKey(category => new { category.UserId, category.Id });

        builder.Property(category => category.Name)
            .HasMaxLength(Category.MaximumNameLength)
            .IsRequired();
        builder.Property(category => category.Type).HasConversion<byte>().HasColumnType("tinyint");
        builder.Property(category => category.DefaultScope).HasConversion<byte?>().HasColumnType("tinyint");
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.IsTax).IsRequired();

        builder.HasIndex(category => new { category.UserId, category.Type, category.Name })
            .IsUnique()
            .HasDatabaseName("UX_Categories_UserId_Type_Name");
        builder.HasIndex(category => new { category.UserId, category.IsActive, category.Type, category.Name })
            .HasDatabaseName("IX_Categories_UserId_IsActive_Type_Name");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(category => category.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
