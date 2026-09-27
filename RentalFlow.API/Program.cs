var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRentalFlowOpenApi();
builder.Services.AddMediator();
builder.Services.AddValidators();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

await app.SeedDatabaseAsync();

app.UseRentalFlowOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
