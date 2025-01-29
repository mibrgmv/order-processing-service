using OrderProcessingService.Infrastructure.Events;

namespace OrderProcessingService.Application.Contracts.Orders.Events;

public record OrderPackingStartedEvent(
    long OrderId,
    string PackingBy,
    DateTimeOffset StartedAt) : IEvent;