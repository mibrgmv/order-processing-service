using OrderProcessingService.Application.Abstractions.Persistence.Repositories;

namespace OrderProcessingService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IOrderRepository OrderRepository { get; }
}