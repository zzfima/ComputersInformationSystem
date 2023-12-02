var _builder = WebApplication.CreateBuilder(args);

_builder.Services.AddDbContext<ConfigurationDB>(options =>
{
    options.UseSqlite(_builder.Configuration.GetConnectionString("SqliteConfigurations"));
});

//add repository service
_builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

//add swagger
_builder.Services.AddEndpointsApiExplorer();
_builder.Services.AddSwaggerGen();

var _app = _builder.Build();

//while development ensure db scheme created while app started
if (_app.Environment.IsDevelopment())
{
    _app.UseSwagger();
    _app.UseSwaggerUI();
}

using var scope = _app.Services.CreateScope();
var dbService = scope.ServiceProvider.GetRequiredService<ConfigurationDB>();
dbService.Database.EnsureCreated();

#region REST API
//Get all configurations
_app.MapGet("/configurations", async (IConfigurationRepository repo) => await repo.GetConfigurationsAsync())
    .WithTags("GET");

//Update specific Configuration
_app.MapPut("/configuration", async ([FromBody] Configuration configuration, IConfigurationRepository repo) =>
{
    await repo.UpdateConfigurationAsync(configuration);
    await repo.SaveAsync();
    return Results.Accepted($"configuration ID: {configuration.Id} success PUT", configuration);
})
    .WithTags("PUT");

//Add Configuration
_app.MapPost("/configuration", async ([FromBody] Configuration configuration, IConfigurationRepository repo) =>
{
    await repo.InsertConfigurationAsync(configuration);
    await repo.SaveAsync();
    return Results.Created($"configuration ID: {configuration.Id} success POST", configuration);
})
    .WithTags("POST");

//Delete specific configuration
_app.MapDelete("/configuration/{id}", async (int id, IConfigurationRepository repo) =>
{
    await repo.DeleteConfigurationAsync(id);
    await repo.SaveAsync();
    return Results.Accepted($"configuration ID: {id} success DELETE", id);
})
    .WithTags("DELETE");
#endregion

string configurationSqliteCRUDServiceURL = string.Empty;
using (var serviceScope = _app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var repo = services.GetRequiredService<IConfigurationRepository>();
    var configurations = await repo.GetConfigurationsAsync();
    if (configurations.Count != 0)
        configurationSqliteCRUDServiceURL = configurations[0].ConfigurationSqliteCRUDServiceURL;
    else
        configurationSqliteCRUDServiceURL = "http://localhost:5200/";
}

_app.UseHttpsRedirection();
_app.Run();
