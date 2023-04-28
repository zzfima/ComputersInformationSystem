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

#region Get
//Get specific remote machine Phoenix Version by ip
app.MapGet("/remoteMachinePhoenixVersion/{ip}", async (string ip) =>
{
    var configurations = await RequestsWarpper.GetConfiguration();
    var configuration = configurations.FirstOrDefault();

    IVersionRetriver phnx = new VersionRetriver(configuration?.PhoenixFileName ?? "");
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), configuration.UserName, configuration.Password);
        return Results.Ok(ver);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");

//Get specific remote machine FW Version by ip
app.MapGet("/remoteMachineFWVersion/{ip}", async (string ip) =>
{
    var configurations = await RequestsWarpper.GetConfiguration();
    var configuration = configurations.FirstOrDefault();

    IVersionRetriver phnx = new VersionRetriver(configuration?.FWFileName ?? "");
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), configuration.UserName, configuration.Password);
        return Results.Ok(ver);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");
#endregion

app.Run();
app.UseHttpsRedirection();