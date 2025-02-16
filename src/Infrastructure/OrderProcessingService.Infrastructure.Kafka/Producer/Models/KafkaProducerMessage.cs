namespace OrderProcessingService.Infrastructure.Kafka.Producer.Models;

public record KafkaProducerMessage<TKey, TValue>(TKey Key, TValue Value);