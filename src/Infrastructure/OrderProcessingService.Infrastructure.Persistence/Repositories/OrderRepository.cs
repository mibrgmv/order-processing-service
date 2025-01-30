using Npgsql;
using OrderProcessingService.Application.Abstractions.Persistence.Queries;
using OrderProcessingService.Application.Abstractions.Persistence.Repositories;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Infrastructure.Persistence.Extensions;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace OrderProcessingService.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<Order> QueryAsync(
        OrderQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        select  order_id,
                order_state, 
                order_created_at, 
                order_updated_at
        from orders
        where 
            (cardinality(:ids) = 0 or order_id = any (:ids))
            and order_id > :cursor
        limit :page_size;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("ids", query.OrderIds)
            .AddParameter("cursor", query.Cursor)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Order(
                Id: reader.GetInt64("order_id"),
                State: await reader.GetFieldValueAsync<OrderState>("order_state", cancellationToken),
                CreatedAt: await reader.GetFieldValueAsync<DateTimeOffset>("order_created_at", cancellationToken),
                UpdatedAt: await reader.GetFieldValueAsync<DateTimeOffset>("order_updated_at", cancellationToken));
        }
    }

    public async Task AddOrUpdateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into orders (order_id, order_state, order_created_at, order_updated_at)
        select order_id,
               order_state,
               order_created_at,
               order_updated_at
        from unnest(:ids, :states, :created_at, :updated_at)
        on conflict on constraint orders_pkey
        do update 
        set order_state = excluded.order_state,
            order_created_at = excluded.order_created_at,
            order_updated_at = excluded.order_updated_at;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection)
            .AddParameter("ids", orders.Select(x => x.Id))
            .AddParameter("states", orders.Select(x => x.State))
            .AddParameter("created_at", orders.Select(x => x.CreatedAt))
            .AddParameter("updated_at", orders.Select(x => x.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}