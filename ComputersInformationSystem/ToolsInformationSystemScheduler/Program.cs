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

Timer updateTimer = null;
Timer discoverTimer = null;

#region Post
app.MapPost("/startScheduling", async () =>
{
    var configurations = await RequestsWarpper.GetConfiguration();
    var configuration = configurations.FirstOrDefault();
    var discoverFrequencyMinutes = configuration.DiscoverFrequencyMinutes;
    var updateFrequencyMinutes = configuration.UpdateFrequencyMinutes;

    updateTimer = new Timer(UpdateMachines, null, 1000, updateFrequencyMinutes * 60 * 1000);
    discoverTimer = new Timer(DiscoverMachines, null, 1000, discoverFrequencyMinutes * 60 * 1000);

    return Results.Created("Scheduling Started", "Scheduling started");
})
    .WithTags("POST");

void DiscoverMachines(object? state)
{
    Task.Run(async () =>
    {
        IList<string>? aliveRemoteMachines = await RequestsWarpper.GetAliveMachines();
        var existingCRUDRemoteMachines = await RequestsWarpper.GetExistingCRUDRemoteMachines();

        foreach (var remoteConfiguredIpMachine in aliveRemoteMachines)
        {
            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                var phoenixVersion = await RequestsWarpper.GetPhoenixVersion(remoteConfiguredIpMachine);
                if (phoenixVersion == null)
                    return;

                var fwVersion = await RequestsWarpper.GetFWVersion(remoteConfiguredIpMachine);
                if (fwVersion == null)
                    return;

                if (existingCRUDRemoteMachines.Find(r => r.IPAddress == remoteConfiguredIpMachine) == null)
                {
                    var newRemoteMachine = new RemoteMachine
                    {
                        IPAddress = remoteConfiguredIpMachine,
                        FWVersion = fwVersion,
                        PhoenixVersion = phoenixVersion,
                        LastUpdate = DateTime.Now
                    };

                    await RequestsWarpper.AddRemoteMachine(newRemoteMachine);
                }
            });
        }
    });
}

void UpdateMachines(object? state)
{
    Task.Run(async () =>
    {
        var existingCRUDRemoteMachines = await RequestsWarpper.GetExistingCRUDRemoteMachines();

        foreach (var existingCRUDRemoteMachine in existingCRUDRemoteMachines)
        {
            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                var remoteMachine = existingCRUDRemoteMachines.Find(r => r.IPAddress == existingCRUDRemoteMachine.IPAddress);
                if (remoteMachine == null)
                    return;

                var phoenixVersion = await RequestsWarpper.GetPhoenixVersion(existingCRUDRemoteMachine.IPAddress);
                if (phoenixVersion == null)
                    return;

                var fwVersion = await RequestsWarpper.GetFWVersion(existingCRUDRemoteMachine.IPAddress);
                if (fwVersion == null)
                    return;

                remoteMachine.FWVersion = fwVersion;
                remoteMachine.PhoenixVersion = phoenixVersion;
                remoteMachine.LastUpdate = DateTime.Now;

                await RequestsWarpper.ChangeRemoteMachine(remoteMachine);
            });
        }
    });
}

app.MapPost("/stopScheduling", () =>
{
    discoverTimer?.Change(Timeout.Infinite, Timeout.Infinite);
    updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);

    discoverTimer?.Dispose();
    updateTimer?.Dispose();

    return Results.Created("Scheduling stopped", "Scheduling stopped");
})
    .WithTags("POST");

#endregion

app.Run();
app.UseHttpsRedirection();