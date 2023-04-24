using Utilities;

var builder = WebApplication.CreateBuilder(args);

//IoC
builder.Services.AddScoped<IIpIterator, IpIterator>();
builder.Services.AddScoped<IIpLiveFilter, IpLiveIpFilter>();

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
//Get remote machines manually
app.MapGet("/remoteMachines/{ipFrom}/{ipTo}", async (string ipFrom, string ipTo, IIpIterator ipIterator, IIpLiveFilter ipLiveFilter) =>
{
    return await GetLiveIPs(ipIterator, ipLiveFilter, IPAddress.Parse(ipFrom), IPAddress.Parse(ipTo));
})
    .WithTags("GET");

//Get remote machines from configuration
app.MapGet("/remoteConfiguredIpMachines", async (IIpIterator ipIterator, IIpLiveFilter ipLiveFilter) =>
{
    var fromIP = IPAddress.Parse(config?.FromIPAddress ?? "127.0.0.1");
    var toIP = IPAddress.Parse(config?.ToIPAddress ?? "127.0.0.1");
    return await GetLiveIPs(ipIterator, ipLiveFilter, fromIP, toIP);
})
    .WithTags("GET");
#endregion


app.Run();

//add HTTPS
app.UseHttpsRedirection();

static async Task<IResult> GetLiveIPs(IIpIterator ipIterator, IIpLiveFilter ipLiveFilter, IPAddress fromIP, IPAddress toIP)
{
    var IPs = ipIterator.CreateIpList(from: fromIP, to: toIP);
    var liveIPs = await ipLiveFilter.FilterLiveOnly(IPs);
    var liveIPsAsStrings = liveIPs.Select(ip => ip.ToString()).ToArray();
    return liveIPsAsStrings.Length > 0 ? Results.Ok(liveIPsAsStrings) : Results.NotFound();
}