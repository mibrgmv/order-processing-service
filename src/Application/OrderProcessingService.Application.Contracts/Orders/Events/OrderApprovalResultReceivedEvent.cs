using Itmo.Dev.Platform.Events;

namespace OrderProcessingService.Application.Contracts.Orders.Events;

public record OrderApprovalResultReceivedEvent(
    long OrderId,
    bool IsApproved,
    string CreatedBy,
    DateTimeOffset CreatedAt) : IEvent;