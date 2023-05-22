using RabbitMQ.Client;
using System.Text;

namespace MessageQueuePublishService.BL
{
    public class RabbitMQProducer : IMessageQueueProducer
    {
        private IConnection? _connection;
        private IModel? _channel;
        private bool _isDisposed;
        private string? _routingKey;

        public void Initialize(string hostName, string userName, string password, string routingKey)
        {
            _routingKey = routingKey;
            var factory = new ConnectionFactory { HostName = hostName };
            factory.UserName = userName;
            factory.Password = password;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: routingKey,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: _routingKey,
                                 basicProperties: null,
                                 body: body);
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

            Console.WriteLine("RabbitMQService Disposed");
        }

        ~RabbitMQProducer() => Dispose(false);
        #endregion
    }
}
