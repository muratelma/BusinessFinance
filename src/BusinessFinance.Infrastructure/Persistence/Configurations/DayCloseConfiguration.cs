using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessFinance.Domain;
using BusinessFinance.Infrastructure.Identity;

namespace BusinessFinance.Infrastructure.Persistence.Configurations;

internal sealed class DayCloseConfiguration : IEntityTypeConfiguration<DayClose>
{
    public void Configure(EntityTypeBuilder<DayClose> builder)
    {
        // Bu tablo tutar taşımaz (ADR 0019 İ3): yalnız kimlik, gün ve Z no.
        // Para, gün sonunun ürettiği gelir ve POS tahsilatı kayıtlarındadır.
        builder.ToTable("DayCloses", table =>
        {
            table.HasCheckConstraint(
                "CK_DayCloses_Range", "[RangeStart] IS NULL OR [RangeStart] < [ClosedOn]");
            table.HasCheckConstraint(
                "CK_DayCloses_ZNumber", "[ZNumber] IS NULL OR [ZNumber] > 0");
        });

        builder.HasKey(dayClose => dayClose.Id);
        builder.HasAlternateKey(dayClose => new { dayClose.UserId, dayClose.Id });
        builder.Property(dayClose => dayClose.ClosedOn).HasColumnType("date");
        builder.Property(dayClose => dayClose.RangeStart).HasColumnType("date");
        builder.Property(dayClose => dayClose.IsAdditional).IsRequired();
        builder.Property(dayClose => dayClose.CreatedAtUtc).HasColumnType("datetimeoffset");
        builder.Property(dayClose => dayClose.CancelledAtUtc).HasColumnType("datetimeoffset");
        builder.Property(dayClose => dayClose.IsCancelled).IsRequired();
        builder.Ignore(dayClose => dayClose.FirstDay);

        // Gün başına tek gün sonu: ikincisi yalnız açıkça "ek" olarak yazılır.
        // Aynı anda gelen iki istekten biri bu indekse çarpar.
        builder.HasIndex(dayClose => new { dayClose.UserId, dayClose.ClosedOn })
            .IsUnique()
            .HasFilter("[IsCancelled] = 0 AND [IsAdditional] = 0")
            .HasDatabaseName("UX_DayCloses_UserId_ClosedOn");
        // Aynı Z raporu iki kez girilemez.
        builder.HasIndex(dayClose => new { dayClose.UserId, dayClose.ZNumber })
            .IsUnique()
            .HasFilter("[ZNumber] IS NOT NULL AND [IsCancelled] = 0")
            .HasDatabaseName("UX_DayCloses_UserId_ZNumber");

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(dayClose => dayClose.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
