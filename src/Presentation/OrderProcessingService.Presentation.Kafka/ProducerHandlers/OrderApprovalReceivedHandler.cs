using Google.Protobuf.WellKnownTypes;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Infrastructure.Events;
using OrderProcessingService.Infrastructure.Kafka.Producer;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ProducerHandlers;

internal class OrderApprovalReceivedHandler : IEventHandler<OrderApprovalResultReceivedEvent>
{
    private readonly IKafkaProducer<OrderProcessingKey, OrderProcessingValue> _producer;

    public OrderApprovalReceivedHandler(IKafkaProducer<OrderProcessingKey, OrderProcessingValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(OrderApprovalResultReceivedEvent evt, CancellationToken cancellationToken)
    {
        var key = new OrderProcessingKey { OrderId = evt.OrderId };

        var value = new OrderProcessingValue
        {
            ApprovalReceived = new OrderProcessingValue.Types.OrderApprovalReceived
            {
                OrderId = evt.OrderId,
                IsApproved = evt.IsApproved,
                CreatedBy = evt.CreatedBy,
                CreatedAt = evt.CreatedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<OrderProcessingKey, OrderProcessingValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}