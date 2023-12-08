using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Utilities;

const string ServiceName = "Remote Machines Neo4j CRUD Service";

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

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
}

var _driver = GraphDatabase.Driver($"bolt://{_configuration.Neo4jHostName}", AuthTokens.Basic(_configuration.Neo4jUserName, _configuration.Neo4jPassword));

#region REST API
_app.MapPost("/remoteMachine", async ([FromBody] RemoteMachine remoteMachine) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "MapPost(remoteMachine)");
    var session = _driver.AsyncSession();

    var phoenixVersion = remoteMachine.PhoenixVersion;
    var firmwareVersion = remoteMachine.FWVersion;
    var ipAddress = remoteMachine.IPAddress;
    var hostName = remoteMachine.HostName;

    //check if need to create. If no exists - create
    var needCreatePhoenixEntity = true;
    var needCreateFirmwareEntity = true;
    var needCreateRemoteMachineEntity = true;

    await session.ExecuteReadAsync(
        async tx =>
        {
            var reader = await tx.RunAsync("MATCH (n:Phoenix) WHERE n.version = $phoenixVersion RETURN n", new { phoenixVersion });
            while (await reader.FetchAsync())
            {
                needCreatePhoenixEntity = false;
                break;
            }

            reader = await tx.RunAsync("MATCH (n:Firmware) WHERE n.version = $firmwareVersion RETURN n", new { firmwareVersion });
            while (await reader.FetchAsync())
            {
                needCreateFirmwareEntity = false;
                break;
            }

            reader = await tx.RunAsync("MATCH (n:RemoteMachine) WHERE n.ipAddress = $ipAddress RETURN n", new { ipAddress });
            while (await reader.FetchAsync())
            {
                needCreateRemoteMachineEntity = false;
                break;
            }
        });

    //create entities in neo4j
    if (needCreatePhoenixEntity)
        await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (n:Phoenix {version: $phoenixVersion})", new { phoenixVersion }));
    if (needCreateFirmwareEntity)
        await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (n:Firmware {version: $firmwareVersion})", new { firmwareVersion }));
    if (needCreateRemoteMachineEntity)
        await session.ExecuteWriteAsync(tx => tx.RunAsync("CREATE (n:RemoteMachine {ipAddress: $ipAddress, hostName: $hostName})", new { ipAddress, hostName }));

    //connect Phoenix to RemoteMachine
    if (needCreatePhoenixEntity || needCreateRemoteMachineEntity)
        await session.ExecuteWriteAsync(tx => tx.RunAsync(
            "MATCH (rm:RemoteMachine), (p:Phoenix) " +
            "WHERE rm.ipAddress = $ipAddress AND p.version = $phoenixVersion " +
            "CREATE (rm)-[r:Installed {ipAddress: rm.ipAddress + '<->' + p.version}]->(p) RETURN type(r), r.version"
            , new { phoenixVersion, ipAddress }));

    //connect Firmware to RemoteMachine
    if (needCreateFirmwareEntity || needCreateRemoteMachineEntity)
        await session.ExecuteWriteAsync(tx => tx.RunAsync(
            "MATCH (rm:RemoteMachine), (fw:Firmware) " +
            "WHERE rm.ipAddress = $ipAddress AND fw.version = $firmwareVersion " +
            "CREATE (rm)-[r:Installed {ipAddress: rm.ipAddress + '<->' + fw.version}]->(fw) RETURN type(r), r.version"
            , new { firmwareVersion, ipAddress }));


    return Results.Created($"Pushed to Neo4j IP Address: {ipAddress}, Host Name: {hostName}, Phoenix Version: {phoenixVersion}, Firmware Version: {firmwareVersion}", ipAddress);
})
    .WithTags("POST");
#endregion

await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Service start");

Console.WriteLine($"Run service on {_configuration?.RemoteMachinesNeo4jCRUDServiceURL}");

_app.UseHttpsRedirection();
_app.Run();
