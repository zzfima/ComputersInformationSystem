
using Utilities;

var configurations = await ConfigurationWarpper.GetConfiguration();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

//add data context
_builder.Services.AddDbContext<RemoteMachineDB>(options =>
{
    options.UseSqlite(_builder.Configuration.GetConnectionString("SqliteRemotMachines"));
});

//add repository service
_builder.Services.AddScoped<IRemoteMachineRepository, RemoteMachineRepository>();

//add swagger
_builder.Services.AddEndpointsApiExplorer();
_builder.Services.AddSwaggerGen();

var _app = _builder.Build();

//example usage DI
//using (var serviceScope = app.Services.CreateScope())
//{
//    var services = serviceScope.ServiceProvider;
//    var myDependency = services.GetRequiredService<IRemoteMachineRepository>();
//    var rm = myDependency.GetRemoteMachinesAsync().Result;
//}

//while development ensure db scheme created while app started
if (_app.Environment.IsDevelopment())
{
    _app.UseSwagger();
    _app.UseSwaggerUI();
    using var scope = _app.Services.CreateScope();
    var dbService = scope.ServiceProvider.GetRequiredService<RemoteMachineDB>();
    dbService.Database.EnsureCreated();
}

#region REST API
//Get all remote machines
_app.MapGet("/remoteMachines", async (IRemoteMachineRepository repo) => await repo.GetRemoteMachinesAsync())
    //.CacheOutput()
    .Produces<List<RemoteMachine>>(StatusCodes.Status200OK)
    .WithTags("GET");

//Get specific remote machine by id
_app.MapGet("/remoteMachine/{id}", async (int id, IRemoteMachineRepository repo) =>
{
    try
    {
        RemoteMachine? remoteMachine = await repo.GetRemoteMachineAsync(id);
        return Results.Ok(remoteMachine);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");

//Add Remote Machine
_app.MapPost("/remoteMachine", async ([FromBody] RemoteMachine remoteMachine, IRemoteMachineRepository repo) =>
{
    await repo.InsertRemoteMachineAsync(remoteMachine);
    await repo.SaveAsync();
    return Results.Created($"/remoteMachine ID: {remoteMachine.Id} success POST", remoteMachine);
})
    .WithTags("POST");

//Delete specific Remote Machine
_app.MapDelete("/remoteMachine/{id}", async (int id, IRemoteMachineRepository repo) =>
{
    await repo.DeleteRemoteMachineAsync(id);
    await repo.SaveAsync();
    return Results.Accepted($"remoteMachine ID: {id} success DELETE", id);
})
    .WithTags("DELETE");

//Update specific Remote Machine, id shall be same
_app.MapPut("/remoteMachine", async ([FromBody] RemoteMachine remoteMachine, IRemoteMachineRepository repo) =>
{
    await repo.UpdateRemoteMachineAsync(remoteMachine);
    await repo.SaveAsync();
    return Results.Created($"/remoteMachine ID: {remoteMachine.Id} success PUT", remoteMachine);
})
    .WithTags("PUT");
#endregion

_app.Run(_configuration?.ToolsInformationCRUDServiceURL);
_app.UseHttpsRedirection();