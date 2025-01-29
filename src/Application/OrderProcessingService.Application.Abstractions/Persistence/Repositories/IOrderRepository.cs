using OrderProcessingService.Application.Abstractions.Persistence.Queries;
using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstractions.Persistence.Repositories;

public interface IOrderRepository
{
    IAsyncEnumerable<Order> QueryAsync(OrderQuery query, CancellationToken cancellationToken);

    Task AddOrUpdateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken);
}