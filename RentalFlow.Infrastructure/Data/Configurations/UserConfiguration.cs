using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities;

namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : BaseConfiguration<UserEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.MustChangePassword)
            .IsRequired();

        builder.Property(u => u.OperatorId)
            .IsRequired();

        builder.HasOne(u => u.Operator)
            .WithOne(o => o.User)
            .HasForeignKey<UserEntity>(u => u.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasIndex(u => u.OperatorId)
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
