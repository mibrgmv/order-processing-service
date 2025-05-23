using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Presentation.Grpc.Controllers;

namespace OrderProcessingService.Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection collection)
    {
        collection.AddGrpc();
        collection.AddGrpcReflection();

        return collection;
    }

    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<OrderController>();
            endpoints.MapGrpcReflectionService();
        });

        return builder;
    }
}