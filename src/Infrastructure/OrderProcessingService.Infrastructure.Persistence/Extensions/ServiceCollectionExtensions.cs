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
    public static IServiceCollection AddPersistencePostgres(this IServiceCollection collection)
    {
        const string configurationKey = "Postgres";

        collection.AddOptions<PersistenceOptions>().BindConfiguration(configurationKey);

        collection.AddScoped<NpgsqlDataSource>(sp =>
        {
            PersistenceOptions options = sp.GetRequiredService<IOptionsSnapshot<PersistenceOptions>>().Value;

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(options.ConnectionString);

            dataSourceBuilder.MapEnum<OrderState>(pgName: "order_state");

            return dataSourceBuilder.Build();
        });

        collection.AddScoped<IPersistenceContext, PersistenceContext>();
        collection.AddScoped<IOrderRepository, OrderRepository>();

        return collection;
    }

    public static IServiceCollection AddPersistenceMigrations(this IServiceCollection collection)
    {
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

        collection.AddHostedService<MigrationRunnerBackgroundService>();

        return collection;
    }
}