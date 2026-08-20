using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities.Operator;

namespace RentalFlow.Infrastructure.Data.Configurations;

public class OperatorConfiguration : IEntityTypeConfiguration<OperatorEntity>
{
    public void Configure(EntityTypeBuilder<OperatorEntity> builder)
    {
        builder.ToTable("Operators");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Role)
            .HasMaxLength(50);

        builder.HasMany(o => o.Applications)
            .WithOne(ra => ra.Operator)
            .HasForeignKey(ra => ra.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}