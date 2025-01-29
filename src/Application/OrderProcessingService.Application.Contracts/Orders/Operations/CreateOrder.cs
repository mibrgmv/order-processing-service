namespace OrderProcessingService.Application.Contracts.Orders.Operations;

public static class CreateOrder
{
    public readonly record struct Request(long OrderId, DateTimeOffset CreatedAt);
}