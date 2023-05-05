using Utilities;

var configurations = await ConfigurationWarpper.GetConfiguration();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

//IoC
_builder.Services.AddScoped<IIpIterator, IpIterator>();
_builder.Services.AddScoped<IIpLiveFilter, IpLiveIpFilter>();

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
//Get remote machines manually
_app.MapGet("/remoteMachines/{ipFrom}/{ipTo}", async (string ipFrom, string ipTo, IIpIterator ipIterator, IIpLiveFilter ipLiveFilter) =>
{
    return await GetLiveIPs(ipIterator, ipLiveFilter, IPAddress.Parse(ipFrom), IPAddress.Parse(ipTo));
})
    .WithTags("GET");
#endregion

_app.Run(_configuration?.DiscoverAliveRemoteMachinesServiceURL);
_app.UseHttpsRedirection();

async Task<IResult> GetLiveIPs(IIpIterator ipIterator, IIpLiveFilter ipLiveFilter, IPAddress fromIP, IPAddress toIP)
{
    var IPs = ipIterator.CreateIpList(from: fromIP, to: toIP);
    var liveIPs = await ipLiveFilter.FilterLiveOnly(IPs);
    var liveIPsAsStrings = liveIPs.Select(ip => ip.ToString()).ToArray();
    return liveIPsAsStrings.Length > 0 ? Results.Ok(liveIPsAsStrings) : Results.NotFound();
}