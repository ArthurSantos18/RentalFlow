namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .IsRequired();

        builder.Property(x => x.FieldName)
            .HasMaxLength(100);

        builder.Property(x => x.OldValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.EntityName, x.EntityId });

        builder.HasIndex(x => x.CreatedAt);
    }
}