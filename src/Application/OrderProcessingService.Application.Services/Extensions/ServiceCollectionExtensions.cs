using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Application.Contracts.Orders;
using OrderProcessingService.Application.Services.Orders;

namespace OrderProcessingService.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderService, OrderService>();

        return collection;
    }
}