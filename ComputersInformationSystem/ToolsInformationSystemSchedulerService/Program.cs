using RemoteMachineLiveIPService;
using System.Net;

const string ServiceName = "Tools Information System Scheduler Service";

Configuration _configuration = await GetConfiguratuion();

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
}

Timer? _updateTimer = null;
Timer? _discoverTimer = null;
IRemoteMachineLiveIP _remoteMachineLiveIP = new RemoteMachineLiveIP();

#region Post
_app.MapPost("/startScheduling", () =>
{
    Task.Run(async () => await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "MapPost(startScheduling)"));
    //if already started - return error
    if (_updateTimer != null || _discoverTimer != null)
        return Results.BadRequest("Scheduling ALREADY started");

    _configuration = GetConfiguratuion().Result;
    var discoverFrequencyMinutes = _configuration.DiscoverFrequencyMinutes;
    var updateFrequencyMinutes = _configuration.UpdateFrequencyMinutes;

    //currently want only discover
    _updateTimer = new Timer(UpdateMachines, null, 1000, updateFrequencyMinutes * 60 * 1000);
    _discoverTimer = new Timer(DiscoverMachines, null, 1000, discoverFrequencyMinutes * 60 * 1000);

    return Results.Created("Scheduling Started", "Scheduling started");
})
    .WithTags("POST");

_app.MapPost("/stopScheduling", () =>
{
    Task.Run(async () => await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "MapPost(stopScheduling)"));
    //if already stopped - return error
    if (_updateTimer == null || _discoverTimer == null)
        return Results.BadRequest("Scheduling ALREADY stopped");

    _discoverTimer?.Change(Timeout.Infinite, Timeout.Infinite);
    _updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);

    _discoverTimer?.Dispose();
    _updateTimer?.Dispose();

    _discoverTimer = null;
    _updateTimer = null;

    return Results.Created("Scheduling stopped", "Scheduling stopped");
})
    .WithTags("POST");
#endregion

await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Service start");

Console.WriteLine($"Run service on {_configuration?.ToolsInformationSystemSchedulerServiceURL}");

_app.UseHttpsRedirection();
_app.Run();


void DiscoverMachines(object? state)
{
    Task.Run(async () =>
    {
        IIpIterator ipIterator = new IpIterator();
        var ipList = ipIterator.CreateIpList(IPAddress.Parse(_configuration.FromIPAddress), IPAddress.Parse(_configuration.ToIPAddress));

        foreach (var aliveIP in ipList)
        {
            await RESTAPIWrapper.PublishIPToIPGatherAsync(_configuration, aliveIP.ToString());
        }
    });
}

void UpdateMachines(object? state)
{
    Task.Run(async () =>
    {
        var existingDBRemoteMachines = await RESTAPIWrapper.GetExistingDBRemoteMachinesAsync(_configuration);

        foreach (var existingCRUDRemoteMachine in existingDBRemoteMachines)
        {
            await RESTAPIWrapper.PublishIPToIPGatherAsync(_configuration, existingCRUDRemoteMachine.IPAddress);
        }
    });
}

async Task<Configuration> GetConfiguratuion()
{
    var configurations = await RESTAPIWrapper.GetConfigurationAsync();
    var configuration = configurations.FirstOrDefault();
    if (configuration == null)
        throw new Exception("Configuration not found");
    return configuration;
}
