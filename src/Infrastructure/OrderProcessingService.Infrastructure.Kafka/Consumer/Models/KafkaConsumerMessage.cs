namespace OrderProcessingService.Infrastructure.Kafka.Consumer.Models;

public record KafkaConsumerMessage<TKey, TValue>(TKey Key, TValue Value);
