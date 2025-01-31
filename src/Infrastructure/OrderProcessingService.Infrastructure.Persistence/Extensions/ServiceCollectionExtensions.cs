using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using OrderProcessingService.Application.Abstractions.Persistence;
using OrderProcessingService.Application.Abstractions.Persistence.Repositories;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Infrastructure.Persistence.Repositories;

namespace OrderProcessingService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection collection)
    {
        collection.AddOptions<PersistenceOptions>().BindConfiguration(nameof(PersistenceOptions));

        collection.AddScoped<NpgsqlDataSource>(sp =>
        {
            PersistenceOptions options = sp.GetRequiredService<IOptionsSnapshot<PersistenceOptions>>().Value;

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(options.ConnectionString);

            dataSourceBuilder.MapEnum<OrderState>(pgName: "order_state");

            return dataSourceBuilder.Build();
        });

        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(sp =>
                {
                    PersistenceOptions options = sp.GetRequiredService<IOptionsSnapshot<PersistenceOptions>>().Value;
                    return options.ConnectionString;
                })
                .ScanIn(typeof(IAssemblyMarker).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        collection.AddScoped<IPersistenceContext, PersistenceContext>();
        collection.AddScoped<IOrderRepository, OrderRepository>();

        return collection;
    }
}