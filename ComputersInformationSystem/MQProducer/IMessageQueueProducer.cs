namespace MessageQueuePublishService.BL
{
    public interface IMessageQueueProducer : IDisposable
    {
        void Initialize(string hostName, string userName, string password, string routingKey);
        void Publish(string message);
    }
}