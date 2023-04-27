
var builder = WebApplication.CreateBuilder(args);

//add data context
builder.Services.AddDbContext<RemoteMachineDB>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteRemotMachines"));
});

//add repository service
builder.Services.AddScoped<IRemoteMachineRepository, RemoteMachineRepository>();

//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//example usage DI
//using (var serviceScope = app.Services.CreateScope())
//{
//    var services = serviceScope.ServiceProvider;
//    var myDependency = services.GetRequiredService<IRemoteMachineRepository>();
//    var rm = myDependency.GetRemoteMachinesAsync().Result;
//}

//while development ensure db scheme created while app started
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var dbService = scope.ServiceProvider.GetRequiredService<RemoteMachineDB>();
    dbService.Database.EnsureCreated();
}

#region Remote Machines
//Get all remote machines
app.MapGet("/remoteMachines", async (IRemoteMachineRepository repo) => await repo.GetRemoteMachinesAsync())
    //.CacheOutput()
    .Produces<List<RemoteMachine>>(StatusCodes.Status200OK)
    .WithTags("GET");

//Get specific remote machine by id
app.MapGet("/remoteMachines/{id}", async (int id, IRemoteMachineRepository repo) =>
    await repo.GetRemoteMachineAsync(id) is RemoteMachine remoteMachine ? Results.Ok(remoteMachine) : Results.NotFound())
    .WithTags("GET");

//Add Remote Machine
app.MapPost("/remoteMachines", async ([FromBody] RemoteMachine remoteMachine, IRemoteMachineRepository repo) =>
{
    await repo.InsertRemoteMachineAsync(remoteMachine);
    await repo.SaveAsync();
    return Results.Created($"/remoteMachines{remoteMachine.Id}", remoteMachine);
})
    .WithTags("POST");

//Delete specific Remote Machine
app.MapDelete("/remoteMachines{id}", async (int id, IRemoteMachineRepository repo) =>
{
    await repo.DeleteRemoteMachineAsync(id);
    await repo.SaveAsync();
    return Results.NoContent();
})
    .WithTags("DELETE");

//Update specific Remote Machine, id shall be same
app.MapPut("/remoteMachines", async ([FromBody] RemoteMachine remoteMachine, IRemoteMachineRepository repo) =>
{
    await repo.UpdateRemoteMachineAsync(remoteMachine);
    await repo.SaveAsync();
    return Results.NoContent();
})
    .WithTags("PUT");
#endregion

app.Run();
app.UseHttpsRedirection();