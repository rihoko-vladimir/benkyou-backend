using MassTransit;
using Shared.Models.Models.Configurations;
using ext = Auth.Api.Extensions.EnvironmentExtensions;

namespace Auth.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static JwtConfiguration GetJwtConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtConfiguration.Key);
        var jwtConfiguration = new JwtConfiguration();
        section.Bind(jwtConfiguration);

        return jwtConfiguration;
    }

    public static RabbitMqConfiguration GetRabbitMqConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMqConfiguration.Key);
        var rabbitConfiguration = new RabbitMqConfiguration();
        section.Bind(rabbitConfiguration);

        return rabbitConfiguration;
    }

    public static AzureServiceBusConfiguration GetServiceBusConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection(AzureServiceBusConfiguration.Key);
        var serviceBusConfiguration = new AzureServiceBusConfiguration();
        section.Bind(serviceBusConfiguration);

        return serviceBusConfiguration;
    }

    public static void ConfigureRabbitMq(
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        RabbitMqConfiguration rabbitConfig)
    {
        factoryConfigurator.Host(rabbitConfig.Host, rabbitConfig.VirtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(rabbitConfig.UserName);
            hostConfigurator.Password(rabbitConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        AzureServiceBusConfiguration busConfig)
    {
        factoryConfigurator.Host(busConfig.AzureServiceBusConnectionString);
    }
}