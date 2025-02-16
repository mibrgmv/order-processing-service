using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderProcessingService.Infrastructure.Kafka.Configuration;

internal sealed class KafkaConfigurationBuilder : IKafkaConfigurationOptionsSelector, IKafkaConfigurationBuilder
{
    public KafkaConfigurationBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public IKafkaConfigurationBuilder WithOptions(IConfiguration configuration)
    {
        Services.AddOptions<KafkaConfigurationOptions>().Bind(configuration);
        return this;
    }
}