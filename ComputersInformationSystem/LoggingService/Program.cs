using LoggingService.BL;
using Utilities;

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

_builder.Services.AddScoped<ILogWriter, ElasticLogWriter>(m =>
{
    return new ElasticLogWriter(_configuration);
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

#region Post
_app.MapPost("/log/{sender}/{message}", async (ILogWriter logWriter, string sender, string message) =>
{
    await Task.Run(() =>
    {
        logWriter.Write(new Entities.LogRecord() { Sender = sender, Message = message });
        return Results.Created("Log Posted", "Log Posted");
    });

})
    .WithTags("POST");
#endregion

_app.Run(_configuration?.LoggingServiceURL);
_app.UseHttpsRedirection();
