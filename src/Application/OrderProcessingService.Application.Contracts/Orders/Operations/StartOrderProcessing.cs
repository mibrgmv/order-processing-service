namespace OrderProcessingService.Application.Contracts.Orders.Operations;

public static class StartOrderProcessing
{
    public readonly record struct Request(long OrderId, DateTimeOffset StartedAt);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record OrderNotFound : Result;
    }
}