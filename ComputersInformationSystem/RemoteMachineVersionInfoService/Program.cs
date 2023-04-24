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

//get configuration
IHttpResponseWrapper<IList<Configuration>> httpResponseConfiguration = new HttpResponseWrapper<IList<Configuration>>();
var configs = await httpResponseConfiguration.GetResponse("http://localhost:5223/configurations");
var config = configs.FirstOrDefault();

#region Get
//Get specific remote machine Phoenix Version by ip
app.MapGet("/remoteMachinePhoenixVersion/{ip}", async (string ip) =>
{
    IVersionRetriver phnx = new VersionRetriver(config?.PhoenixFileName ?? "");
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), config.UserName, config.Password);
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
    IVersionRetriver phnx = new VersionRetriver(config?.FWFileName ?? "");
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), config.UserName, config.Password);
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