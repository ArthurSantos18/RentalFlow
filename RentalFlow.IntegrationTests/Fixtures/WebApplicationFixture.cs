namespace RentalFlow.IntegrationTests.Fixtures;

public sealed class WebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IConfiguration? _configuration;
    public string AdminEmail => GetConfig()["Seed:AdminEmail"] ?? throw new InvalidOperationException("Seed:AdminEmail not configured in appsettings.Testing.json");
    public string AdminPassword => GetConfig()["Seed:AdminPassword"] ?? throw new InvalidOperationException("Seed:AdminPassword not configured in appsettings.Testing.json");

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Testing@Passw0rd")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbDescriptor is not null)
            {
                services.Remove(dbDescriptor);
            }

            var interceptorDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AuditSaveChangesInterceptor));

            if (interceptorDescriptor is not null)
            {
                services.Remove(interceptorDescriptor);
            }

            services.AddScoped<AuditSaveChangesInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            services.RemoveAll<IConfigureOptions<RateLimiterOptions>>();

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                    RateLimitPartition.GetNoLimiter("test"));

                options.AddPolicy("login", _ => RateLimitPartition.GetNoLimiter("login-test"));
                options.AddPolicy("refresh", _ => RateLimitPartition.GetNoLimiter("refresh-test"));
                options.AddPolicy("authenticated", _ => RateLimitPartition.GetNoLimiter("authenticated-test"));
            });
        });
    }

    private IConfiguration GetConfig()
    {
        _configuration ??= Services.GetRequiredService<IConfiguration>();
        return _configuration;
    }
}