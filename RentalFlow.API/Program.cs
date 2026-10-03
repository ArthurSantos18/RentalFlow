var builder = WebApplication.CreateBuilder(args);

builder.AddRentalFlowLogging();

builder.Services.AddRentalFlowOpenApi();
builder.Services.AddMediator();
builder.Services.AddValidators();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExceptionHandling();
builder.Services.AddRentalFlowHealthChecks(builder.Configuration);
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