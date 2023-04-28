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
                    LastCheck = DateTime.Now
                };

                await RequestsWarpper.PutRemoteMachine(newRemoteMachine);
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