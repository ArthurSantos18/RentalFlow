using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities.Team;

namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class TeamConfiguration : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        builder.ToTable("Teams");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.HasMany(t => t.Operators)
            .WithOne(o => o.Team)
            .HasForeignKey(o => o.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
