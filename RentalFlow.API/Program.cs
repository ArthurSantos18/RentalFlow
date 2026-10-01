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

var app = builder.Build();

await app.SeedDatabaseAsync();

app.UseRequestIdMiddleware();
app.UseExceptionHandler();
app.UseRentalFlowOpenApi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRequestLogContext();
app.UseAuthorization();
app.MapControllers();

app.RunWithLogging();