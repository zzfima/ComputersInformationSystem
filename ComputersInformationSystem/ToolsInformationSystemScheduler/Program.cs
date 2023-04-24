using Utilities;

var builder = WebApplication.CreateBuilder(args);

//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//while development ensure db scheme created while app started
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


#region Post
app.MapPost("/startScheduling", async () =>
{
    IList<string>? aliveRemoteMachines = await GetAliveMachines();
    var existingCRUDRemoteMachines = await GetExistingCRUDRemoteMachines();

    foreach (var remoteConfiguredIpMachine in aliveRemoteMachines)
    {
        ThreadPool.QueueUserWorkItem(async (a) =>
        {
            var phoenixVersion = await GetPhoenixVersion(remoteConfiguredIpMachine);
            if (phoenixVersion == null)
                return;

            var fwVersion = await GetFWVersion(remoteConfiguredIpMachine);
            if (fwVersion == null)
                return;

            if (existingCRUDRemoteMachines.Find(r => r.IPAddress == remoteConfiguredIpMachine) == null)
            {
                var newRemoteMachine = new RemoteMachine
                {
                    IPAddress = remoteConfiguredIpMachine,
                    FWVersion = fwVersion,
                    PhoenixVersion = phoenixVersion,
                    LastCheck = DateTime.Now
                };

                await PutRemoteMachine(newRemoteMachine);
            }
        });
    }

    return Results.Created("Scheduling Started", "Scheduling started");
})
    .WithTags("POST");



app.MapPost("/stopScheduling", () =>
{
    return Results.Created("Scheduling stopped", "Scheduling stopped");
})
    .WithTags("POST");

#endregion

app.Run();
app.UseHttpsRedirection();

static async Task<Configuration> GetConfiguration()
{
    IHttpResponseWrapper<IList<Configuration>> httpResponseConfiguration = new HttpResponseWrapper<IList<Configuration>>();
    var configs = await httpResponseConfiguration.GetResponse("http://localhost:5223/configurations");
    return configs.FirstOrDefault();
}

static async Task<IList<string>> GetAliveMachines()
{
    var configuration = await GetConfiguration();
    IHttpResponseWrapper<IList<string>> httpResponseAliveRemoteMachines = new HttpResponseWrapper<IList<string>>();
    return await httpResponseAliveRemoteMachines.GetResponse($"http://localhost:5084/remoteMachines/{configuration.FromIPAddress}/{configuration.ToIPAddress}");
}

static async Task<string> GetPhoenixVersion(string remoteConfiguredIpMachine)
{
    IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
    var phoenixVersionRestApi = $"http://localhost:5218/remoteMachinePhoenixVersion/{remoteConfiguredIpMachine}";
    return await remoteMachineVersionInfoService.GetResponse(phoenixVersionRestApi);
}

static async Task<string> GetFWVersion(string remoteConfiguredIpMachine)
{
    IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
    var phoenixVersionRestApi = $"http://localhost:5218/remoteMachineFWVersion/{remoteConfiguredIpMachine}";
    return await remoteMachineVersionInfoService.GetResponse(phoenixVersionRestApi);
}

static async Task<List<RemoteMachine>> GetExistingCRUDRemoteMachines()
{
    IHttpResponseWrapper<List<RemoteMachine>> toolsInformationCRUDServiceRead = new HttpResponseWrapper<List<RemoteMachine>>();
    return await toolsInformationCRUDServiceRead.GetResponse("http://localhost:5271/remoteMachines");
}


static async Task<string> PutRemoteMachine(RemoteMachine newRemoteMachine)
{
    IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
    return await toolsInformationCRUDServiceWrite.Post("http://localhost:5271/remoteMachines", newRemoteMachine);
}