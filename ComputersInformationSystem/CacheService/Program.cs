using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Utilities;

const string ServiceName = "Cache Service";

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

//add data context
_builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = _configuration.RedisServerHostName;
});

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

//Get IP
_app.MapGet("/ip/{address}", async (IDistributedCache distributedCache, string address) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapGet({address})");
    var isAliveBinary = await distributedCache.GetAsync(address);
    if (isAliveBinary == null)
        return Results.NotFound();
    var isAlive = bool.Parse(Encoding.UTF8.GetString(isAliveBinary));
    return Results.Ok(new IP() { Address = address, IsAlive = isAlive });
})
    .WithTags("GET");

_app.MapPost("/ip/{address}/{isAlive}", async (IDistributedCache distributedCache, string address, bool isAlive) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapPost(/{address}, {isAlive})");
    await InsertIntoCache(distributedCache, address, isAlive);
    return Results.Ok($"Key/value {address}/{isAlive} post");
})
    .WithTags("POST");
#endregion

await WarmCache();

await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Service start");
_app.Run(_configuration?.CacheServiceURL);
_app.UseHttpsRedirection();

async Task WarmCache()
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Cache warming...");
    var ips = await RESTAPIWrapper.GetIPsFromDBAsync(_configuration);
    IDistributedCache? distributedCache = null;

    using (var serviceScope = _app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        distributedCache = services.GetRequiredService<IDistributedCache>();
    }

    if (ips != null)
    {
        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"Add {ips.Count} items");
        foreach (var ip in ips)
            await InsertIntoCache(distributedCache, ip.Address, ip.IsAlive);
    }
}

async Task InsertIntoCache(IDistributedCache distributedCache, string address, bool isAlive)
{
    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
    var dtOffset = DateTimeOffset.UtcNow;
    options.AbsoluteExpiration = dtOffset.AddMinutes(_configuration.CacheServiceTTLAbsoluteExpirationMinutes);
    await distributedCache.SetAsync(address, Encoding.UTF8.GetBytes(isAlive.ToString()), options);
}