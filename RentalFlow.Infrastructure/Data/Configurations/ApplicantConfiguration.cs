namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class ApplicantConfiguration : BaseConfiguration<ApplicantEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ApplicantEntity> builder)
    {
        builder.ToTable("Applicants");

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(a => a.Cpf)
            .IsUnique();

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Phone)
            .HasMaxLength(15);

        builder.Property(a => a.MonthlyIncome)
            .HasColumnType("decimal(18,2)").IsRequired();

        builder.HasMany(a => a.Applications)
            .WithOne(ra => ra.Applicant)
            .HasForeignKey(ra => ra.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
