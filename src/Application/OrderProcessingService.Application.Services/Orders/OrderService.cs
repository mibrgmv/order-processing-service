using OrderProcessingService.Application.Contracts.Orders;
using OrderProcessingService.Application.Contracts.Orders.Operations;

namespace OrderProcessingService.Application.Services.Orders;

internal class OrderService : IOrderService
{
    public Task CreateAsync(CreateOrder.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<StartOrderProcessing.Result> StartProcessingAsync(StartOrderProcessing.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApproveOrder.Result> ApproveAsync(ApproveOrder.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<StartOrderPacking.Result> StartPackingAsync(StartOrderPacking.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FinishOrderPacking.Result> FinishPackingAsync(FinishOrderPacking.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<StartOrderDelivery.Result> StartDeliveryAsync(StartOrderDelivery.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FinishOrderDelivery.Result> FinishDeliveryAsync(FinishOrderDelivery.Request request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}