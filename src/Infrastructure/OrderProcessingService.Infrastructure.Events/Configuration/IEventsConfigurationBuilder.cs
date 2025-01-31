namespace OrderProcessingService.Infrastructure.Events.Configuration;

public interface IEventsConfigurationBuilder
{
    IEventsConfigurationBuilder AddHandlersFromAssemblyContaining<T>();
}