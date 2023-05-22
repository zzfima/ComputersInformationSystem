using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemoteMachineLiveIPService;
using System.Text;
using Utilities;

public class MQAliveIPGatherGatherConsumer
{
    const string ServiceName = "MQ Alive IP Gather Consumer Service";
    private Configuration? _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _isDisposed;
    IRemoteMachineLiveIP? _remoteMachineLiveIP;

    public void Initialize(Configuration configuration)
    {
        _configuration = configuration;
        Task.Run(async () => await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Initialize"));
        _remoteMachineLiveIP = new RemoteMachineLiveIP();

        var factory = new ConnectionFactory { HostName = _configuration.MQHostName };
        factory.DispatchConsumersAsync = true;
        factory.UserName = _configuration.MQUserName;
        factory.Password = _configuration.MQPassword;
        factory.DispatchConsumersAsync = true;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _configuration?.MQAliveIPGatherRoutingKey,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += MessageQueueEventDelivered;

        _channel.BasicConsume(
            queue: _configuration?.MQAliveIPGatherRoutingKey,
            autoAck: true,
            consumer: consumer);
    }

    private async Task MessageQueueEventDelivered(object model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        string ip = Encoding.UTF8.GetString(body);
        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"[x] Received {ip} at {DateTime.Now.ToLongTimeString()}");

        /*
         * Algorithm
         * Chech in cache
         * If cache missing
         *   go to service to check ip alive
         *      ip alive send to db and cache
         *      use the value
         * else use cache value
        */

        bool isAlive = false;
        var getValueFromCache = await RESTAPIWrapper.GetIPFromCacheAsync(_configuration, ip);
        if (getValueFromCache == null)
        {
            await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Cache missing");
            isAlive = await _remoteMachineLiveIP.IsIPAlive(ip);
        }
        else
        {
            isAlive = getValueFromCache.IsAlive;
        }

        var newIP = new IP() { Address = ip, IsAlive = isAlive };
        await RESTAPIWrapper.SentIPToCacheAsync(_configuration, newIP);
        await RESTAPIWrapper.SentIPToDBAsync(_configuration, newIP);

        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"isAlive: {isAlive}");

        //if IP is not alive and written in DB: remove it from DB and return
        if (!isAlive)
        {
            var existingMachine = await RESTAPIWrapper.GetExistingDBRemoteMachineByIPAsync(_configuration, ip);
            if (existingMachine != null && _configuration.IsToDeleteDeathRemoteMachine)
                await RESTAPIWrapper.RemoveRemoteMachineFromDBAsync(_configuration, existingMachine);
        }
        else
            await RESTAPIWrapper.PublishIPToMQVersionGatherProducerServiceAsync(_configuration, ip);


    }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposed)
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _channel?.Dispose();
        _channel = null;
        _connection?.Dispose();
        _connection = null;

        Console.WriteLine("RabbitMQ AliveIPGatherGatherConsumer Disposed");
    }

    ~MQAliveIPGatherGatherConsumer() => Dispose(false);
    #endregion
}
