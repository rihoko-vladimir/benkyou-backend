using Auth.Api.Models.Application;
using Auth.Api.Models.Configuration;
using MassTransit;
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

    public static MassTransitConfiguration GetMassTransitConfiguration(this IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(MassTransitConfiguration.Key);

        if (ext.IsDevelopment() || ext.IsLocal())
        {
            var massConfig = new MassTransitConfiguration();
            configurationSection.Bind(massConfig);

            return massConfig;
        }

        if (!ext.IsProduction()) throw new ConfigurationException("Configuration is incorrect");
        return new MassTransitConfiguration
        {
            AzureServiceBusConnectionString = configurationSection.GetValue<string>("AzureServiceBusConnectionString")
        };
    }

    public static void ConfigureRabbitMq(
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.Host(massConfig.Host, massConfig.VirtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(massConfig.UserName);
            hostConfigurator.Password(massConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.Host(massConfig.AzureServiceBusConnectionString);
    }
}