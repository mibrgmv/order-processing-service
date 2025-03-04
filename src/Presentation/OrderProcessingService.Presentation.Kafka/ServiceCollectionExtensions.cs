using Itmo.Dev.Platform.Events;

namespace OrderProcessingService.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IEventsConfigurationBuilder AddPresentationKafkaEventHandlers(
        this IEventsConfigurationBuilder builder)
    {
        return builder.AddHandlersFromAssemblyContaining<IAssemblyMarker>();
    }
}