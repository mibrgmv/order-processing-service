using Microsoft.Extensions.Logging;
using OrderProcessingService.Application.Contracts.Orders;
using OrderProcessingService.Application.Contracts.Orders.Operations;
using OrderProcessingService.Infrastructure.Kafka.Consumer;
using OrderProcessingService.Infrastructure.Kafka.Consumer.Models;
using Orders.Kafka.Contracts;

namespace OrderProcessingService.Presentation.Kafka.ConsumerHandlers;

internal class OrderCreationConsumerHandler : IKafkaConsumerHandler<OrderCreationKey, OrderCreationValue>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderCreationConsumerHandler> _logger;

    public OrderCreationConsumerHandler(IOrderService orderService, ILogger<OrderCreationConsumerHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        KafkaConsumerMessage<OrderCreationKey, OrderCreationValue> message,
        CancellationToken cancellationToken)
    {
        if (message.Value.EventCase is OrderCreationValue.EventOneofCase.OrderCreated)
        {
            var request = new CreateOrder.Request(
                message.Value.OrderCreated.OrderId,
                message.Value.OrderCreated.CreatedAt.ToDateTimeOffset());

            await _orderService.CreateAsync(request, cancellationToken);
        }
        else if (message.Value.EventCase is OrderCreationValue.EventOneofCase.OrderProcessingStarted)
        {
            var request = new StartOrderProcessing.Request(
                message.Value.OrderProcessingStarted.OrderId,
                message.Value.OrderProcessingStarted.StartedAt.ToDateTimeOffset());

            StartOrderProcessing.Result result = await _orderService
                .StartProcessingAsync(request, cancellationToken);
        }
        else
        {
            _logger.LogError(
                "Invalid event case received = {EventCase} for order = {OrderId}",
                message.Value.EventCase,
                message.Key.OrderId);
        }
    }
}