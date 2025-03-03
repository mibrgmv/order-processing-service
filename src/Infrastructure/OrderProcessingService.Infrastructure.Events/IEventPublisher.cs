namespace OrderProcessingService.Infrastructure.Events;

public interface IEventPublisher
{
    ValueTask PublishAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent;
}