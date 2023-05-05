var configurations = await ConfigurationWarpper.GetConfiguration();
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
}

Timer? _updateTimer = null;
Timer? _discoverTimer = null;

#region Post
_app.MapPost("/startScheduling", () =>
{
    //if already started - return error
    if (_updateTimer != null || _discoverTimer != null)
        return Results.BadRequest("Scheduling ALREADY started");

    var discoverFrequencyMinutes = _configuration.DiscoverFrequencyMinutes;
    var updateFrequencyMinutes = _configuration.UpdateFrequencyMinutes;

    _updateTimer = new Timer(UpdateMachines, null, 1000, updateFrequencyMinutes * 60 * 1000);
    _discoverTimer = new Timer(DiscoverMachines, null, 1000, discoverFrequencyMinutes * 60 * 1000);

    return Results.Created("Scheduling Started", "Scheduling started");
})
    .WithTags("POST");

_app.MapPost("/stopScheduling", () =>
{
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

_app.Run(_configuration?.ToolsInformationSystemSchedulerServiceURL);
_app.UseHttpsRedirection();

void DiscoverMachines(object? state)
{
    Task.Run(async () =>
    {
        IList<string>? aliveRemoteMachines = await GetAliveMachines();
        var existingCRUDRemoteMachines = await GetExistingCRUDRemoteMachines();
        var machinesToRemove = new List<RemoteMachine>();

        //treat death machines
        foreach (var existingCRUDRemoteMachine in existingCRUDRemoteMachines)
        {
            if (!aliveRemoteMachines.Any(m => m == existingCRUDRemoteMachine.IPAddress))
            {
                machinesToRemove.Add(existingCRUDRemoteMachine);
            }
        }

        foreach (var machineToRemove in machinesToRemove)
        {
            existingCRUDRemoteMachines.Remove(machineToRemove);
            await RemoveRemoteMachineFromDB(machineToRemove);
        }

        foreach (var aliveRemoteMachine in aliveRemoteMachines)
        {
            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                var existingRemoteMachine = existingCRUDRemoteMachines.Find(r => r.IPAddress == aliveRemoteMachine);

                var phoenixVersion = await GetPhoenixVersion(aliveRemoteMachine);
                if (phoenixVersion == null)
                {
                    //Machine in Tools Information but have not Phoenix installed - remove it
                    if (existingRemoteMachine != null)
                        await RemoveRemoteMachineFromDB(existingRemoteMachine);
                    return;
                }

                var fwVersion = await GetFWVersion(aliveRemoteMachine);
                if (fwVersion == null)
                {
                    //Machine in Tools Information but have not FW installed - remove it
                    if (existingRemoteMachine != null)
                        await RemoveRemoteMachineFromDB(existingRemoteMachine);
                    return;
                }

                //New machine. Add to Tools Information
                if (existingRemoteMachine == null)
                {
                    var newRemoteMachine = new RemoteMachine
                    {
                        IPAddress = aliveRemoteMachine,
                        FWVersion = fwVersion,
                        PhoenixVersion = phoenixVersion,
                        LastUpdate = DateTime.Now
                    };

                    await PublishRemoteMachine(newRemoteMachine);
                    await AddRemoteMachineToDB(newRemoteMachine);
                }
            });
        }
    });
}

void UpdateMachines(object? state)
{
    Task.Run(async () =>
    {
        var existingCRUDRemoteMachines = await GetExistingCRUDRemoteMachines();

        foreach (var existingCRUDRemoteMachine in existingCRUDRemoteMachines)
        {
            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                var phoenixVersion = await GetPhoenixVersion(existingCRUDRemoteMachine.IPAddress);
                if (phoenixVersion == null)
                    return;

                var fwVersion = await GetFWVersion(existingCRUDRemoteMachine.IPAddress);
                if (fwVersion == null)
                    return;

                existingCRUDRemoteMachine.FWVersion = fwVersion;
                existingCRUDRemoteMachine.PhoenixVersion = phoenixVersion;
                existingCRUDRemoteMachine.LastUpdate = DateTime.Now;

                await ChangeRemoteMachineOnDB(existingCRUDRemoteMachine);
                await PublishRemoteMachine(existingCRUDRemoteMachine);
            });
        }
    });
}

async Task<List<RemoteMachine>> GetExistingCRUDRemoteMachines()
{
    IHttpResponseWrapper<List<RemoteMachine>> toolsInformationCRUDServiceRead = new HttpResponseWrapper<List<RemoteMachine>>();
    return await toolsInformationCRUDServiceRead.Get($"{_configuration.ToolsInformationCRUDServiceURL}remoteMachines");
}

async Task<string> AddRemoteMachineToDB(RemoteMachine newRemoteMachine)
{
    IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
    return await toolsInformationCRUDServiceWrite.Post($"{_configuration.ToolsInformationCRUDServiceURL}remoteMachine", newRemoteMachine);
}

async Task<string> RemoveRemoteMachineFromDB(RemoteMachine remoteMachineToDelete)
{
    IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
    return await toolsInformationCRUDServiceWrite.Delete($"{_configuration.ToolsInformationCRUDServiceURL}remoteMachine/{remoteMachineToDelete.Id}");
}

async Task<string> ChangeRemoteMachineOnDB(RemoteMachine newRemoteMachine)
{
    IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
    return await toolsInformationCRUDServiceWrite.Put($"{_configuration.ToolsInformationCRUDServiceURL}remoteMachine", newRemoteMachine);
}

async Task<string> GetPhoenixVersion(string remoteConfiguredIpMachine)
{
    IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
    var phoenixVersionRestApi = $"{_configuration.RemoteMachineVersionInfoServiceURL}remoteMachinePhoenixVersion/{remoteConfiguredIpMachine}";
    return await remoteMachineVersionInfoService.Get(phoenixVersionRestApi);
}

async Task<string> GetFWVersion(string remoteConfiguredIpMachine)
{
    IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
    var phoenixVersionRestApi = $"{_configuration.RemoteMachineVersionInfoServiceURL}remoteMachineFWVersion/{remoteConfiguredIpMachine}";
    return await remoteMachineVersionInfoService.Get(phoenixVersionRestApi);
}

async Task<IList<string>> GetAliveMachines()
{
    IHttpResponseWrapper<IList<string>> httpResponseAliveRemoteMachines = new HttpResponseWrapper<IList<string>>();
    return await httpResponseAliveRemoteMachines.Get($"{_configuration.DiscoverAliveRemoteMachinesServiceURL}remoteMachines/{_configuration.FromIPAddress}/{_configuration.ToIPAddress}");
}

async Task<string> PublishRemoteMachine(RemoteMachine remoteMachineToPush)
{
    IHttpResponseWrapper<string> messageQueueService = new HttpResponseWrapper<string>();
    return await messageQueueService.Post($"{_configuration.MessageQueueServiceURL}PushRemoteMachineIP/{remoteMachineToPush.IPAddress}", remoteMachineToPush.IPAddress);
}