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
_app.MapPost("/startScheduling", async () =>
{
    //if already started - return error
    if (_updateTimer != null || _discoverTimer != null)
        return Results.BadRequest("Scheduling ALREADY started");

    var configurations = await RequestsWarpper.GetConfiguration();
    var configuration = configurations.FirstOrDefault();
    var discoverFrequencyMinutes = configuration.DiscoverFrequencyMinutes;
    var updateFrequencyMinutes = configuration.UpdateFrequencyMinutes;

    _updateTimer = new Timer(UpdateMachines, null, 1000, updateFrequencyMinutes * 60 * 1000);
    _discoverTimer = new Timer(DiscoverMachines, null, 1000, discoverFrequencyMinutes * 60 * 1000);

    return Results.Created("Scheduling Started", "Scheduling started");
})
    .WithTags("POST");

void DiscoverMachines(object? state)
{
    Task.Run(async () =>
    {
        IList<string>? aliveRemoteMachines = await RequestsWarpper.GetAliveMachines();
        var existingCRUDRemoteMachines = await RequestsWarpper.GetExistingCRUDRemoteMachines();

        foreach (var aliveRemoteMachine in aliveRemoteMachines)
        {
            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                var existingRemoteMachine = existingCRUDRemoteMachines.Find(r => r.IPAddress == aliveRemoteMachine);

                var phoenixVersion = await RequestsWarpper.GetPhoenixVersion(aliveRemoteMachine);
                if (phoenixVersion == null)
                {
                    //If Existing machine - Remove from to Tools Information
                    if (existingRemoteMachine != null)
                        await RequestsWarpper.RemoveRemoteMachine(existingRemoteMachine);
                    return;
                }

                var fwVersion = await RequestsWarpper.GetFWVersion(aliveRemoteMachine);
                if (fwVersion == null)
                {
                    //If Existing machine - Remove from to Tools Information
                    if (existingRemoteMachine != null)
                        await RequestsWarpper.RemoveRemoteMachine(existingRemoteMachine);
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
                var phoenixVersion = await RequestsWarpper.GetPhoenixVersion(existingCRUDRemoteMachine.IPAddress);
                if (phoenixVersion == null)
                    return;

                var fwVersion = await RequestsWarpper.GetFWVersion(existingCRUDRemoteMachine.IPAddress);
                if (fwVersion == null)
                    return;

                existingCRUDRemoteMachine.FWVersion = fwVersion;
                existingCRUDRemoteMachine.PhoenixVersion = phoenixVersion;
                existingCRUDRemoteMachine.LastUpdate = DateTime.Now;

                await RequestsWarpper.ChangeRemoteMachine(existingCRUDRemoteMachine);
            });
        }
    });
}

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

_app.Run();
_app.UseHttpsRedirection();