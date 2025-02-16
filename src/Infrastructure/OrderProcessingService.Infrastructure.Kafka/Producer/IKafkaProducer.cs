using Confluent.Kafka;
using OrderProcessingService.Infrastructure.Kafka.Producer.Models;

namespace OrderProcessingService.Infrastructure.Kafka.Producer;

public interface IKafkaProducer<TKey, TValue>
{
    Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        KafkaProducerMessage<TKey, TValue> kafkaProducerMessage,
        CancellationToken cancellationToken);
}