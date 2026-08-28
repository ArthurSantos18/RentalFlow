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

        builder.ComplexProperty(p => p.Address, address =>
        {
            address.Property(p => p.Street)
                .HasMaxLength(200)
                .IsRequired();

            address.Property(p => p.Number)
                .HasMaxLength(20)
                .IsRequired();

            address.Property(p => p.Complement)
                .HasMaxLength(200);

            address.Property(p => p.Neighborhood)
                .HasMaxLength(100)
                .IsRequired();

            address.Property(p => p.City)
                .HasMaxLength(100)
                .IsRequired();

            address.Property(p => p.State)
                .HasMaxLength(2)
                .IsRequired();

            address.Property(p => p.ZipCode)
                .HasMaxLength(8)
                .IsRequired();
        });

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
