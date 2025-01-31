namespace OrderProcessingService.Infrastructure.Events;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    ValueTask HandleAsync(TEvent evt, CancellationToken cancellationToken);
}