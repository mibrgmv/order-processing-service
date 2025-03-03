using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessingService.Infrastructure.Events;

internal sealed class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _provider;

    public EventPublisher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async ValueTask PublishAsync<T>(T evt, CancellationToken cancellationToken) where T : IEvent
    {
        IEnumerable<IEventHandler<T>> handlers = _provider.GetRequiredService<IEnumerable<IEventHandler<T>>>();

        foreach (IEventHandler<T> handler in handlers)
        {
            await handler.HandleAsync(evt, cancellationToken);
        }
    }
}