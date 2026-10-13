namespace RentalFlow.API;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.AddRentalFlowLogging();

        builder.Services.AddRentalFlowOpenApi();
        builder.Services.AddMediator();
        builder.Services.AddValidators();
        builder.Services.AddJwtAuthentication(configuration);
        builder.Services.AddApiServices();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(configuration);
        builder.Services.AddExceptionHandling();
        builder.Services.AddRentalFlowHealthChecks(configuration);
        builder.Services.AddRentalFlowRateLimiting();

        var app = builder.Build();

        await app.SeedDatabaseAsync();

        app.UseCorrelationIdMiddleware();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseRentalFlowOpenApi();
        app.UseRentalFlowHealthChecks();
        app.UseAuthentication();
        app.UseRequestLogContext();
        app.UseAuthorization();
        app.UseRentalFlowRateLimiting();
        app.MapControllers();

        app.RunWithLogging();
    }
}