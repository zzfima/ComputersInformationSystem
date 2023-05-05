using Utilities;

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

#region REST API
//Get specific remote machine Phoenix Version by ip
_app.MapGet("/remoteMachinePhoenixVersion/{ip}", async (string ip) =>
{
    IVersionRetriver phnx = new VersionRetriver(_configuration.PhoenixFileName);
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), _configuration?.UserName, _configuration?.Password);
        return Results.Ok(ver);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");

//Get specific remote machine FW Version by ip
_app.MapGet("/remoteMachineFWVersion/{ip}", async (string ip) =>
{
    IVersionRetriver phnx = new VersionRetriver(_configuration.FWFileName);
    try
    {
        var ver = await phnx.GetVersionByIp(IPAddress.Parse(ip), _configuration.UserName, _configuration.Password);
        return Results.Ok(ver);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
    .WithTags("GET");
#endregion

_app.Run(_configuration?.RemoteMachineVersionInfoServiceURL);
_app.UseHttpsRedirection();