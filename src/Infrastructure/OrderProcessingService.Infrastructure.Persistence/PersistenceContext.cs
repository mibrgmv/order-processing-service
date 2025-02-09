using OrderProcessingService.Application.Abstractions.Persistence;
using OrderProcessingService.Application.Abstractions.Persistence.Repositories;

namespace OrderProcessingService.Infrastructure.Persistence;

internal sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IOrderRepository orderRepository)
    {
        OrderRepository = orderRepository;
    }

    public IOrderRepository OrderRepository { get; }
}