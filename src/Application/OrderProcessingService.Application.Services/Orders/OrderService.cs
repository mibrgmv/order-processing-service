using OrderProcessingService.Application.Abstractions.Persistence;
using OrderProcessingService.Application.Abstractions.Persistence.Queries;
using OrderProcessingService.Application.Contracts.Orders;
using OrderProcessingService.Application.Contracts.Orders.Events;
using OrderProcessingService.Application.Contracts.Orders.Operations;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Infrastructure.Events;
using System.Transactions;

namespace OrderProcessingService.Application.Services.Orders;

internal sealed class OrderService : IOrderService
{
    private readonly IPersistenceContext _context;
    private readonly IEventPublisher _eventPublisher;

    public OrderService(IPersistenceContext context, IEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task CreateAsync(CreateOrder.Request request, CancellationToken cancellationToken)
    {
        var order = new Order(request.OrderId, OrderState.Created, request.CreatedAt, request.CreatedAt);
        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
    }

    public async Task<StartOrderProcessing.Result> StartProcessingAsync(
        StartOrderProcessing.Request request,
        CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderProcessing.Result.OrderNotFound();

        order = order with
        {
            State = OrderState.PendingApproval,
            UpdatedAt = request.StartedAt,
        };

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);

        return new StartOrderProcessing.Result.Success();
    }

    public async Task<ApproveOrder.Result> ApproveAsync(ApproveOrder.Request request, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new ApproveOrder.Result.OrderNotFound();

        if (order.State is not OrderState.PendingApproval)
            return new ApproveOrder.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsApproved ? OrderState.Approved : OrderState.Cancelled,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var evt = new OrderApprovalResultReceivedEvent(
            request.OrderId,
            request.IsApproved,
            request.CreatedBy,
            order.UpdatedAt);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        transaction.Complete();

        return new ApproveOrder.Result.Success();
    }

    public async Task<StartOrderPacking.Result> StartPackingAsync(StartOrderPacking.Request request, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderPacking.Result.OrderNotFound();

        if (order.State is not OrderState.Approved)
            return new StartOrderPacking.Result.InvalidState(order.State);

        order = order with
        {
            State = OrderState.Packing,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var evt = new OrderPackingStartedEvent(order.Id, request.PackingBy, order.UpdatedAt);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        transaction.Complete();

        return new StartOrderPacking.Result.Success();
    }

    public async Task<FinishOrderPacking.Result> FinishPackingAsync(FinishOrderPacking.Request request, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new FinishOrderPacking.Result.OrderNotFound();

        if (order.State is not OrderState.Packing)
            return new FinishOrderPacking.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsSuccessful ? OrderState.Packed : OrderState.Cancelled,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var evt = new OrderPackingFinishedEvent(
            order.Id,
            order.UpdatedAt,
            request.IsSuccessful,
            request.FailureReason);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        transaction.Complete();

        return new FinishOrderPacking.Result.Success();
    }

    public async Task<StartOrderDelivery.Result> StartDeliveryAsync(StartOrderDelivery.Request request, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new StartOrderDelivery.Result.OrderNotFound();

        if (order.State is not OrderState.Packed)
            return new StartOrderDelivery.Result.InvalidState(order.State);

        order = order with
        {
            State = OrderState.InDelivery,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var evt = new OrderDeliveryStartedEvent(
            order.Id,
            request.DeliveredBy,
            order.UpdatedAt);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        transaction.Complete();

        return new StartOrderDelivery.Result.Success();
    }

    public async Task<FinishOrderDelivery.Result> FinishDeliveryAsync(FinishOrderDelivery.Request request, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([request.OrderId], default, 1);

        Order? order = await _context.Orders
            .QueryAsync(orderQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
            return new FinishOrderDelivery.Result.OrderNotFound();

        if (order.State is not OrderState.InDelivery)
            return new FinishOrderDelivery.Result.InvalidState(order.State);

        order = order with
        {
            State = request.IsSuccessful ? OrderState.Delivered : OrderState.Cancelled,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var evt = new OrderDeliveryFinishedEvent(
            order.Id,
            order.UpdatedAt,
            request.IsSuccessful,
            request.FailureReason);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _context.Orders.AddOrUpdateAsync([order], cancellationToken);
        await _eventPublisher.PublishAsync(evt, cancellationToken);

        transaction.Complete();

        return new FinishOrderDelivery.Result.Success();
    }
}