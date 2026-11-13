namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class LoggingExtension
{
    public static WebApplicationBuilder AddRentalFlowLogging(this WebApplicationBuilder builder)
    {
        const string consoleTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] [CorrelationId:{CorrelationId}] [User:{UserId}] [Role:{Role}] {Message:lj}{NewLine}{Exception}";

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "RentalFlow.API")
            .WriteTo.Async(wt => wt.Console(outputTemplate: consoleTemplate));

        if (builder.Environment.IsDevelopment())
        {
            var logPath = Path.Combine(
                builder.Environment.ContentRootPath,
                "logs",
                "rentalflow-.log");

            const string fileTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [CorrelationId:{CorrelationId}] [User:{UserId}] [Role:{Role}] {Message:lj}{NewLine}{Exception}";

            loggerConfig.WriteTo.Async(wt => wt.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: fileTemplate));
        }

        Log.Logger = loggerConfig.CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}