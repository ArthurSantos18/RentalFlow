using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities;

namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserTokenEntity>
{
    public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.RefreshToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.RevokedAt)
            .IsRequired(false);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.RefreshToken)
            .IsUnique();

        builder.HasIndex(t => t.UserId);
    }
}
