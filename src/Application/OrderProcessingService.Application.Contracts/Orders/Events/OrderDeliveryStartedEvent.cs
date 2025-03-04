using Itmo.Dev.Platform.Events;

namespace OrderProcessingService.Application.Contracts.Orders.Events;

public record OrderDeliveryStartedEvent(
    long OrderId,
    string DeliveredBy,
    DateTimeOffset StartedBy) : IEvent;