using RabbitMQ.Client;
using System.Text;

namespace MessageQueueService.BL
{
    public class RabbitMQService : IMessageQueueService
    {
        private Configuration? _configuration;
        private IConnection? _connection;
        private IModel? _channel;
        private bool _isDisposed;

        public void Initialize(Configuration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory { HostName = _configuration.MessageQueueHostName };
            factory.UserName = _configuration.MessageQueueUserName;
            factory.Password = _configuration.MessageQueuePassword;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _configuration?.MessageQueueRoutingKey,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: _configuration?.MessageQueueRoutingKey,
                                 basicProperties: null,
                                 body: body);
        }

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

        ~RabbitMQService() => Dispose(false);
    }
}
