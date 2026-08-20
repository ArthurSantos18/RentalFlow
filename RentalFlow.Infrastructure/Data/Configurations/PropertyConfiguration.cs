using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities.Property;

namespace RentalFlow.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<PropertyEntity>
{
    public void Configure(EntityTypeBuilder<PropertyEntity> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.RentPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Bedrooms)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.HasMany(p => p.Applications)
            .WithOne(ra => ra.Property)
            .HasForeignKey(ra => ra.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
