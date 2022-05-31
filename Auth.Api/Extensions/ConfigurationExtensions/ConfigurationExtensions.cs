using Auth.Api.Models.Application;
using Auth.Api.Models.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
        if (ext.IsDevelopment() || ext.IsLocal())
        {
            var configurationSection = configuration.GetSection(MassTransitConfiguration.Key);
            var massConfig = new MassTransitConfiguration();
            configurationSection.Bind(massConfig);
            return massConfig;
        }

        if (!ext.IsProduction()) throw new ConfigurationException("Configuration is incorrect");
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var secretClient = new SecretClient(uri, new DefaultAzureCredential());
        var connectionString = secretClient.GetSecret("AzureServiceBusConnectionString").Value.Value;
        return new MassTransitConfiguration
        {
            ConnectionString = connectionString
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
        factoryConfigurator.Host(massConfig.ConnectionString);
    }
}