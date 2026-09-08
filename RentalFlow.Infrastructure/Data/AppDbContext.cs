using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Infrastructure.Data.Configurations;

namespace RentalFlow.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<ApplicantEntity> Applicants { get; set; }
    public DbSet<PropertyEntity> Properties { get; set; }
    public DbSet<OperatorEntity> Operators { get; set; }
    public DbSet<RentalApplicationEntity> RentalApplications { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicantConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
        modelBuilder.ApplyConfiguration(new OperatorConfiguration());
        modelBuilder.ApplyConfiguration(new RentalApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
    }
}
