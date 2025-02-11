using OrderProcessingService.Infrastructure.Events.Configuration;

namespace OrderProcessingService.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IEventsConfigurationBuilder AddPresentationKafkaEventHandlers(
        this IEventsConfigurationBuilder builder)
    {
        return builder.AddHandlersFromAssemblyContaining<IAssemblyMarker>();
    }
}