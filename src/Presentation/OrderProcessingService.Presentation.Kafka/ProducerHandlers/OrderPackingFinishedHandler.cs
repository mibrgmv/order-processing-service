using Google.Protobuf.WellKnownTypes;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Infrastructure.Events;
using OrderProcessingService.Infrastructure.Kafka.Producer;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ProducerHandlers;

internal class OrderPackingFinishedHandler : IEventHandler<OrderPackingFinishedEvent>
{
    private readonly IKafkaProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderPackingFinishedHandler(IKafkaProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderPackingFinishedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            PackingFinished = new OrderProcessingValue.Types.OrderPackingFinished
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