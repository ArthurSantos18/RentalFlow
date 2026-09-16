namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class PropertyConfiguration : BaseConfiguration<PropertyEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PropertyEntity> builder)
    {
        builder.ToTable("Properties");

        builder.ComplexProperty(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(200)
                .IsRequired();
                
            address.Property(a => a.Number)
                .HasMaxLength(20)
                .IsRequired();
                
            address.Property(a => a.Complement)
                .HasMaxLength(200);
                
            address.Property(a => a.Neighborhood)
                .HasMaxLength(100)
                .IsRequired();
                
            address.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();
                
            address.Property(a => a.State)
                .HasMaxLength(2)
                .IsRequired();
                
            address.Property(a => a.ZipCode)
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

        builder.HasIndex(p => p.IsAvailable);
    }
}