using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Infrastructure.Kafka.Producer;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ProducerHandlers;

public class OrderDeliveryFinishedHandler : IEventHandler<OrderDeliveryFinishedEvent>
{
    private readonly IKafkaProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderDeliveryFinishedHandler(IKafkaProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderDeliveryFinishedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            DeliveryFinished = new OrderProcessingValue.Types.OrderDeliveryFinished
            {
                OrderId = evt.OrderId,
                FinishedAt = evt.FinishedAt.ToTimestamp(),
                IsFinishedSuccessfully = evt.IsSuccessful,
                FailureReason = evt.FailureReason,
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}