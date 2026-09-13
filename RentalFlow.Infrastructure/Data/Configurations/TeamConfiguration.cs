using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities;

namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class TeamConfiguration : BaseConfiguration<TeamEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TeamEntity> builder)
    {
        builder.ToTable("Teams");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.HasMany(t => t.Operators)
            .WithOne(o => o.Team)
            .HasForeignKey(o => o.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}