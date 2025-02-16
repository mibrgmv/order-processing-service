using OrderProcessingService.Infrastructure.Kafka.Consumer.Models;

namespace OrderProcessingService.Infrastructure.Kafka.Consumer;

public interface IKafkaConsumerHandler<TKey, TValue>
{
    ValueTask HandleAsync(KafkaConsumerMessage<TKey, TValue> message, CancellationToken cancellationToken);
}