namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class WebApplicationExtension
{
    public static void RunWithLogging(this WebApplication app)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Log.Fatal(args.ExceptionObject as Exception, "Unhandled exception on AppDomain.");
            Log.CloseAndFlush();
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Log.Error(args.Exception, "Unobserved task exception.");
        };

        app.Lifetime.ApplicationStarted.Register(() =>
            Log.Information("RentalFlow API started successfully."));

        app.Lifetime.ApplicationStopping.Register(() =>
            Log.Information("RentalFlow API is shutting down..."));

        app.Lifetime.ApplicationStopped.Register(() =>
        {
            Log.Information("RentalFlow API stopped.");
            Log.CloseAndFlush();
        });

        try
        {
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "RentalFlow API terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}