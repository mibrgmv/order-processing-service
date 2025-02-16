namespace OrderProcessingService.Infrastructure.Kafka.Producer.Outbox;

public sealed class OutboxOptions
{
    public int BatchSize { get; set; }

    public TimeSpan PollingDelay { get; set; }

    public int RetryCount { get; set; }
}