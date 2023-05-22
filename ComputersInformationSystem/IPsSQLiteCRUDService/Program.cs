using Utilities;

const string ServiceName = "IPs SQLite CRUD Service";

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

//add data context
_builder.Services.AddDbContext<IPDB>(options =>
{
    options.UseSqlite(_builder.Configuration.GetConnectionString("SqliteIPs"));
});

//add repository service
_builder.Services.AddScoped<IIPRepository, IPRepository>();

//add swagger
_builder.Services.AddEndpointsApiExplorer();
_builder.Services.AddSwaggerGen();

var _app = _builder.Build();

//while development ensure db scheme created while app started
if (_app.Environment.IsDevelopment())
{
    _app.UseSwagger();
    _app.UseSwaggerUI();
    using var scope = _app.Services.CreateScope();
    var dbService = scope.ServiceProvider.GetRequiredService<IPDB>();
    dbService.Database.EnsureCreated();
}

#region REST API
//Get all IPs
_app.MapGet("/IPs", async (IIPRepository repo) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "MapGet(IPs)");
    return Results.Ok(await repo.GetIPsAsync());
})
    .Produces<List<IP>>(StatusCodes.Status200OK)
    .WithTags("GET");

//Get specific remote machine by address
_app.MapGet("/IP/{address}", async (int address, IIPRepository repo) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapGet({address})");
    try
    {
        IP? remoteMachine = await repo.GetIPAsync(address);
        return Results.Ok(remoteMachine);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");

//Add Remote Machine
_app.MapPost("/IP", async ([FromBody] IP ip, IIPRepository repo) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapPost({ip})");
    await repo.InsertIPAsync(ip);
    await repo.SaveAsync();
    return Results.Created($"IP ID: {ip.Id} success POST", ip);
})
    .WithTags("POST");

//Delete specific Remote Machine
_app.MapDelete("/IP/{id}", async (int id, IIPRepository repo) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapDelete(IP {id})");
    await repo.DeleteIPAsync(id);
    await repo.SaveAsync();
    return Results.Accepted($"IP ID: {id} success DELETE", id);
})
    .WithTags("DELETE");

//Update specific Remote Machine, id shall be same
_app.MapPut("/IP", async ([FromBody] IP ip, IIPRepository repo) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "MapPut(IP)");
    await repo.UpdateIPAsync(ip);
    await repo.SaveAsync();
    return Results.Created($"IP ID: {ip.Id} success PUT", ip);
})
    .WithTags("PUT");
#endregion

await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Service start");
_app.Run(_configuration?.IPsSqliteCRUDServiceURL);
_app.UseHttpsRedirection();