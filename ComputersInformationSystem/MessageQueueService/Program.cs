using MessageQueueService.BL;
using Utilities;

var configurations = await ConfigurationWarpper.GetConfiguration();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

_builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>(m =>
{
    var mqService = new RabbitMQService();
    mqService?.Initialize(_configuration);
    return mqService;
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
    using var scope = _app.Services.CreateScope();
}

//Add Configuration
_app.MapPost("/PushRemoteMachineIP/{ip}", async (string ip, IMessageQueueService mqService) =>
{
    await Task.Run(() =>
    {
        mqService.Publish(ip);
    });
    return Results.Created($"Pushed to MessageQueue IP: {ip} success POST", ip);
})
    .WithTags("POST");


_app.Run(_configuration?.MessageQueueServiceURL);
_app.UseHttpsRedirection();