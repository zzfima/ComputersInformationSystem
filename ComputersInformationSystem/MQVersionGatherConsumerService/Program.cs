using Utilities;

var configurations = await RESTAPIWrapper.GetConfigurationAsync();
var _configuration = configurations.FirstOrDefault();
if (_configuration == null)
    throw new Exception("Configuration not found");

var rabbitMQRecieveService = new MQVersionGatherConsumer();
rabbitMQRecieveService.Initialize(_configuration);

Console.ReadLine();