using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Infrastructure.Events.Configuration;

namespace OrderProcessingService.Infrastructure.Events;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformEvents(
        this IServiceCollection collection,
        Func<IEventsConfigurationBuilder, IEventsConfigurationBuilder> configuration)
    {
        var configurationBuilder = new EventsConfigurationBuilder(collection);
        configuration.Invoke(configurationBuilder);

        collection.AddScoped<IEventPublisher, EventPublisher>();

        return collection;
    }
}