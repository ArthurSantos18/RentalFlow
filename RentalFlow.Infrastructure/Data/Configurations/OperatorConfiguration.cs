namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class OperatorConfiguration : BaseConfiguration<OperatorEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OperatorEntity> builder)
    {
        builder.ToTable("Operators");

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.TeamId)
            .IsRequired();

        builder.HasOne(o => o.Team)
            .WithMany(t => t.Operators)
            .HasForeignKey(o => o.TeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.User)
            .WithOne(u => u.Operator)
            .HasForeignKey<UserEntity>(u => u.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.TeamId);
    }
}