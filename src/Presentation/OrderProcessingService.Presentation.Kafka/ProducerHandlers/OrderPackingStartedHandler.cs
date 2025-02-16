using Google.Protobuf.WellKnownTypes;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Infrastructure.Events;
using OrderProcessingService.Infrastructure.Kafka.Producer;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ProducerHandlers;

internal class OrderPackingStartedHandler : IEventHandler<OrderPackingStartedEvent>
{
    private readonly IKafkaProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderPackingStartedHandler(IKafkaProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderPackingStartedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            PackingStarted = new OrderProcessingValue.Types.OrderPackingStarted
            {
                OrderId = evt.OrderId,
                PackingBy = evt.PackingBy,
                StartedAt = evt.StartedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}