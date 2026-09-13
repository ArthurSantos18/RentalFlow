using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalFlow.Domain.Entities;

namespace RentalFlow.Infrastructure.Data.Configurations;

public sealed class RentalApplicationConfiguration : BaseConfiguration<RentalApplicationEntity>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RentalApplicationEntity> builder)
    {
        builder.ToTable("RentalApplications");

        builder.Property(ra => ra.ProposalNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ra => ra.FinancedAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ra => ra.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ra => ra.Installments)
            .IsRequired();

        builder.Property(ra => ra.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ra => ra.ContractDate)
            .IsRequired();

        builder.HasOne(ra => ra.Applicant)
            .WithMany(a => a.Applications)
            .HasForeignKey(ra => ra.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ra => ra.Property)
            .WithMany(p => p.Applications)
            .HasForeignKey(ra => ra.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ra => ra.Operator)
            .WithMany(o => o.Applications)
            .HasForeignKey(ra => ra.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ra => ra.ApplicantId);
        builder.HasIndex(ra => ra.PropertyId);
        builder.HasIndex(ra => ra.OperatorId);
        builder.HasIndex(ra => ra.ProposalNumber).IsUnique();
    }
}