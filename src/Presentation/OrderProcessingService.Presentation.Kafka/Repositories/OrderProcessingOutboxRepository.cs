using Google.Protobuf;
using Npgsql;
using OrderProcessingService.Infrastructure.Kafka.Producer.Outbox;
using OrderProcessingService.Presentation.Kafka.Extensions;
using Orders.Kafka.Contracts;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace OrderProcessingService.Presentation.Kafka.Repositories;

public class OrderProcessingOutboxRepository : IOutboxRepository<OrderProcessingKey, OrderProcessingValue>
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderProcessingOutboxRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddOrUpdateAsync(OutboxMessage<OrderProcessingKey, OrderProcessingValue> message, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into outbox(message_type, message_key, message_value, message_created_at, message_processed_at)
        values (:type, :key, :value, :created_at, :processed_at)
        on conflict (message_key, message_value)
        do update
        set message_processed_at = excluded.message_processed_at
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("id", message.MessageId)
            .AddParameter("type", message.MessageType)
            .AddParameter("key", message.MessageKey.ToByteArray())
            .AddParameter("value", message.MessageValue.ToByteArray())
            .AddParameter("created_at", message.CreatedAt)
            .AddParameter("processed_at", message.ProcessedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<OutboxMessage<OrderProcessingKey, OrderProcessingValue>> GetPendingMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken,
        int? batchSize = null)
    {
        const string sql = """
        select message_id, 
               message_type,
               message_key,
               message_value,
               message_created_at
        from outbox
        where (message_processed_at is null)
        order by message_created_at
        limit :batch_size
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        command.AddParameter("batch_size", batchSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            byte[] keyArr = (byte[])reader["message_key"];
            var deserializedKey = new OrderProcessingKey();
            deserializedKey.MergeFrom(keyArr);

            byte[] valueArr = (byte[])reader["message_value"];
            var deserializedValue = new OrderProcessingValue();
            deserializedValue.MergeFrom(valueArr);

            yield return new OutboxMessage<OrderProcessingKey, OrderProcessingValue>(
                MessageId: reader.GetInt64("message_id"),
                MessageType: reader.GetString("message_type"),
                MessageKey: deserializedKey,
                MessageValue: deserializedValue,
                CreatedAt: reader.GetDateTime("message_created_at"),
                ProcessedAt: null);
        }
    }
}