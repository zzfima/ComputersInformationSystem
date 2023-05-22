using MessageQueuePublishService.BL;
using Utilities;

const string ServiceName = "MQ Version Gather Producer Service";

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var _builder = WebApplication.CreateBuilder(args);

_builder.Services.AddSingleton<IMessageQueueProducer, RabbitMQProducer>(m =>
{
    var mqService = new RabbitMQProducer();
    mqService?.Initialize(_configuration.MQHostName, _configuration.MQUserName, _configuration.MQPassword, _configuration.MQVersionGatherRoutingKey);
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
_app.MapPost("/Publish/{ip}", async (string ip, IMessageQueueProducer mqService) =>
{
    await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"MapPost(Publish {ip})");
    await Task.Run(() =>
    {
        mqService.Publish(ip);
    });
    return Results.Created($"Published to MessageQueue IP: {ip} success POST", ip);
})
    .WithTags("POST");

await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Service start");
_app.Run(_configuration?.MQVersionGatherProducerServiceURL);
_app.UseHttpsRedirection();