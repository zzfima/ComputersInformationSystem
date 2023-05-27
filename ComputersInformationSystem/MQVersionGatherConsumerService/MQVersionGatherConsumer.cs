using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemoteMachineLiveIPService;
using System.Net;
using System.Text;
using Utilities;
using VersionRetrieverService;

public class MQVersionGatherConsumer
{
    const string ServiceName = "MQ Version Gather Consumer Service";
    private Configuration? _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _isDisposed;
    IVersionRetriver? _phoenixVersionRetriever;
    IVersionRetriver? _firmwareVersionRetriever;
    IRemoteMachineLiveIP? _remoteMachineLiveIP;

    public void Initialize(Configuration configuration)
    {
        _configuration = configuration;
        Task.Run(async () => await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "Initialize"));
        _phoenixVersionRetriever = new VersionRetriver(_configuration.InstalledVersions.Where(n => n.Name == "Phoenix").FirstOrDefault().Path);
        _firmwareVersionRetriever = new VersionRetriver(_configuration.InstalledVersions.Where(n => n.Name == "FW").FirstOrDefault().Path);
        _remoteMachineLiveIP = new RemoteMachineLiveIP();

        var factory = new ConnectionFactory { HostName = _configuration.MQHostName };
        factory.DispatchConsumersAsync = true;
        factory.UserName = _configuration.MQUserName;
        factory.Password = _configuration.MQPassword;
        factory.DispatchConsumersAsync = true;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _configuration?.MQVersionGatherRoutingKey,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += MessageQueueuEventDelivered;

        _channel.BasicConsume(queue: _configuration?.MQVersionGatherRoutingKey,
            autoAck: true,
           consumer: consumer);
    }

    private async Task MessageQueueuEventDelivered(object model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        string ip = Encoding.UTF8.GetString(body);
        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"[x] Received {ip} at {DateTime.Now.ToLongTimeString()}");

        var existingMachine = await RESTAPIWrapper.GetExistingDBRemoteMachineByIPAsync(_configuration, ip);

        //if no phoenix installed and written in DB: remove it from DB and return
        var phoenixVersion = await GetPhoenixVersion(ip);
        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"phoenixVersion is null {phoenixVersion == null}");
        if (phoenixVersion == null)
        {
            if (existingMachine != null && _configuration.IsToDeleteDeathRemoteMachine)
                await RESTAPIWrapper.RemoveRemoteMachineFromDBAsync(_configuration, existingMachine);
            return;
        }

        //if no FW installed and written in DB: remove it from DB and return
        var fwVersion = await GetFWVersion(ip);
        await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, $"fwVersion is null {fwVersion == null}");
        if (fwVersion == null)
        {
            if (existingMachine != null && _configuration.IsToDeleteDeathRemoteMachine)
                await RESTAPIWrapper.RemoveRemoteMachineFromDBAsync(_configuration, existingMachine);
            return;
        }

        var hostName = await _remoteMachineLiveIP.GetHostName(ip);

        //If not written in DB - add, else update
        if (existingMachine == null)
        {
            await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "existingMachine is null");
            var newMachine = new RemoteMachine()
            {
                IPAddress = ip,
                FWVersion = fwVersion,
                PhoenixVersion = phoenixVersion,
                HostName = hostName,
                LastUpdate = DateTime.Now,
            };
            await RESTAPIWrapper.AddNewRemoteMachineToSqliteDBAsync(_configuration, newMachine);
            await RESTAPIWrapper.AddNewRemoteMachineToNeo4jDBAsync(_configuration, newMachine);
        }
        else
        {
            await RESTAPIWrapper.SentLogAsync(_configuration, ServiceName, "existingMachine is not null");
            existingMachine.IPAddress = ip;
            existingMachine.FWVersion = fwVersion;
            existingMachine.PhoenixVersion = phoenixVersion;
            existingMachine.HostName = hostName;
            existingMachine.LastUpdate = DateTime.Now;
            await RESTAPIWrapper.ChangeRemoteMachineOnSqliteDBAsync(_configuration, existingMachine);
            await RESTAPIWrapper.AddNewRemoteMachineToNeo4jDBAsync(_configuration, existingMachine);
        }
    }

    async Task<string> GetPhoenixVersion(string remoteConfiguredIpMachine) =>
        await _phoenixVersionRetriever.GetVersionByIp(IPAddress.Parse(remoteConfiguredIpMachine), _configuration?.UserName, _configuration?.Password);


    async Task<string> GetFWVersion(string remoteConfiguredIpMachine) =>
        await _firmwareVersionRetriever.GetVersionByIp(IPAddress.Parse(remoteConfiguredIpMachine), _configuration?.UserName, _configuration?.Password);

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

        Console.WriteLine("RabbitMQService Disposed");
    }

    ~MQVersionGatherConsumer() => Dispose(false);
    #endregion
}
