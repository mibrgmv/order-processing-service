using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessingService.Infrastructure.Events.Configuration;

internal class EventsConfigurationBuilder : IEventsConfigurationBuilder
{
    private readonly IServiceCollection _collection;

    public EventsConfigurationBuilder(IServiceCollection collection)
    {
        _collection = collection;
    }

    public IEventsConfigurationBuilder AddHandlersFromAssemblyContaining<T>()
    {
        using ServiceCollectionAssemblyScanner scanner = _collection.UseAssemblyScanner(typeof(T));

        scanner
            .EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAsTypesConstructedFrom(typeof(IEventHandler<>))
            .WithScopedLifetime()
            .AreNotAbstractClasses()
            .AreNotInterfaces()
            .AreBasedOnTypesConstructedFrom(typeof(IEventHandler<>));

        return this;
    }
}