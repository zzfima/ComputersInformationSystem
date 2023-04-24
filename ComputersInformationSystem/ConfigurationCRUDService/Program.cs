
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ConfigurationDB>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConfigurations"));
});

//add repository service
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//while development ensure db scheme created while app started
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var dbService = scope.ServiceProvider.GetRequiredService<ConfigurationDB>();
    dbService.Database.EnsureCreated();
}

#region Configuration
//Get all configurations
app.MapGet("/configurations", async (IConfigurationRepository repo) => await repo.GetConfigurationsAsync())
    .WithTags("GET");

//Update specific Configuration
app.MapPut("/configurations", async ([FromBody] Configuration configuration, IConfigurationRepository repo) =>
{
    await repo.UpdateConfigurationAsync(configuration);
    await repo.SaveAsync();
    return Results.NoContent();
})
    .WithTags("PUT");

//Add Configuration
app.MapPost("/configurations", async ([FromBody] Configuration configuration, IConfigurationRepository repo) =>
{
    await repo.InsertConfigurationAsync(configuration);
    await repo.SaveAsync();
    return Results.Created($"/configurations{configuration.Id}", configuration);
})
    .WithTags("POST");
#endregion

app.Run();
app.UseHttpsRedirection();