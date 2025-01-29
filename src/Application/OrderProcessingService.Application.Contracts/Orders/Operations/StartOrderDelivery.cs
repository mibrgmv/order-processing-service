using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Contracts.Orders.Operations;

public static class StartOrderDelivery
{
    public readonly record struct Request(long OrderId, string DeliveredBy);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record OrderNotFound : Result;

        public sealed record InvalidState(OrderState State) : Result;
    }
}