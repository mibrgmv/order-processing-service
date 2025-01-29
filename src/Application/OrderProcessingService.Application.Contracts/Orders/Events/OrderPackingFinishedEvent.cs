using OrderProcessingService.Infrastructure.Events;

namespace OrderProcessingService.Application.Contracts.Orders.Events;

public record OrderPackingFinishedEvent(
    long OrderId,
    DateTimeOffset FinishedAt,
    bool IsSuccessful,
    string? FailureReason) : IEvent;