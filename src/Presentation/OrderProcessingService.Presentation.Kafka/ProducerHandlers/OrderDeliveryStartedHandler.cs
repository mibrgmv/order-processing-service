using Google.Protobuf.WellKnownTypes;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Infrastructure.Events;
using OrderProcessingService.Infrastructure.Kafka.Producer;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ProducerHandlers;

internal class OrderDeliveryStartedHandler : IEventHandler<OrderDeliveryStartedEvent>
{
    private readonly IKafkaProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderDeliveryStartedHandler(IKafkaProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderDeliveryStartedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            DeliveryStarted = new OrderProcessingValue.Types.OrderDeliveryStarted
            {
                OrderId = evt.OrderId,
                DeliveredBy = evt.DeliveredBy,
                StartedAt = evt.StartedBy.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}