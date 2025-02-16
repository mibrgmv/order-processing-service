namespace OrderProcessingService.Infrastructure.Kafka.Consumer.Inbox;

public record InboxMessage<TKey, TValue>(
    long MessageId,
    TKey MessageKey,
    TValue MessageValue,
    DateTime CreatedAt,
    DateTime? ProcessedAt);
