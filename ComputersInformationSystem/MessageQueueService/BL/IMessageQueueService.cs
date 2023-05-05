namespace MessageQueueService.BL
{
    public interface IMessageQueueService : IDisposable
    {
        void Initialize(Configuration configuration);
        void Publish(string message);
    }
}