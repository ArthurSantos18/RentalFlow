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
app.MapControllers();

app.RunWithLogging();