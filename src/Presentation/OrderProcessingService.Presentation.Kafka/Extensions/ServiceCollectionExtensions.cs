using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Infrastructure.Kafka.Extensions;
using OrderProcessingService.Presentation.Kafka.ConsumerHandlers;
using OrderProcessingService.Presentation.Kafka.Repositories;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        const string configurationKey = "Kafka:Configuration";
        const string producerKey = "Kafka:Producers:OrderCreation";
        const string consumerKey = "Kafka:Consumers:OrderProcessing";

        serviceCollection.AddKafka(builder => builder
            .WithOptions(configuration.GetSection(configurationKey))
            .AddProducer(selector => selector
                .WithKey<OrderProcessingKey>()
                .WithValue<OrderProcessingValue>()
                .WithConfiguration(configuration.GetSection(producerKey))
                .SerializeKeyWithProtobuf()
                .SerializeValueWithProtobuf()
                .WithOutbox<OrderProcessingOutboxRepository>())
            .AddConsumer(selector => selector
                .WithKey<OrderCreationKey>()
                .WithValue<OrderCreationValue>()
                .WithConfiguration(configuration.GetSection(consumerKey))
                .DeserializeKeyWithProtobuf()
                .DeserializeValueWithProtobuf()
                .WithInbox<OrderCreationInboxRepository>()
                .WithInboxHandler<OrderCreationConsumerHandler>()));

        return serviceCollection;
    }
}