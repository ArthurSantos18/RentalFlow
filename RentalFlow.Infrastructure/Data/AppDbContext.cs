using Microsoft.EntityFrameworkCore;
using RentalFlow.Domain.Entities;

namespace RentalFlow.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<ApplicantEntity> Applicants { get; set; }
    public DbSet<PropertyEntity> Properties { get; set; }
    public DbSet<OperatorEntity> Operators { get; set; }
    public DbSet<RentalApplicationEntity> RentalApplications { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserTokenEntity> UserTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
