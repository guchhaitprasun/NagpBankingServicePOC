namespace AccountService.MessageBroker
{
    public interface IQueuePublisher<T>
    {
        Task PublishMessageAsync(T message);
        Task PublishAccountStatementPdfRequestAsync(T message);
    }
}
