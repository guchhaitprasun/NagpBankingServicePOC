namespace AccountService.MessageBroker
{
    public interface IQueuePublisher<T>
    {
        Task PublishMessageAsync(T message);
    }
}
