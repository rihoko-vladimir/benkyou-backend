using Auth.Api.Models.Application;
using Auth.Api.Models.Configuration;
using Azure.Security.KeyVault.Secrets;
using MassTransit;

namespace Auth.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static JwtConfiguration GetJwtConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection("JWTConfiguration");
        var audience = section.GetValue<string>("Audience");
        var issuer = section.GetValue<string>("Issuer");
        var accessSecret = section.GetValue<string>("AccessSecret");
        var refreshSecret = section.GetValue<string>("RefreshSecret");
        var resetSecret = section.GetValue<string>("ResetSecret");
        var accessExpiresIn = section.GetValue<int>("AccessTokenExpirationTimeMinutes");
        var refreshExpiresIn = section.GetValue<int>("RefreshTokenExpirationTimeMinutes");
        var resetExpiresIn = section.GetValue<int>("ResetTokenExpirationTimeMinutes");
        return new JwtConfiguration(audience, issuer, accessSecret, refreshSecret, resetSecret, accessExpiresIn,
            refreshExpiresIn, resetExpiresIn);
    }

    public static MassTransitConfiguration GetMassTransitConfiguration(this IConfiguration configuration, SecretClient secretClient)
    {
        var configurationSection = configuration.GetSection("MassTransitConfiguration");
        var stringType = configurationSection.GetValue<string>("BusType");
        var type = stringType is not (MassTransitType.RabbitMq or MassTransitType.AzureServiceBus)
            ? "Unknown"
            : stringType;
        switch (type)
        {
            case MassTransitType.RabbitMq:
            {
                var host = configurationSection.GetValue<string>("Host");
                var virtualHost = configurationSection.GetValue<string>("VirtualHost");
                var userName = configurationSection.GetValue<string>("UserName");
                var password = configurationSection.GetValue<string>("Password");
                return new MassTransitConfiguration(type, host, virtualHost, userName, password);
            }
            case MassTransitType.AzureServiceBus:
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    var connectionString = configurationSection.GetValue<string>("AzureConnection");
                    return new MassTransitConfiguration(type, ConnectionString: connectionString);
                }
                else
                {
                    //var connectionStringName = configurationSection.GetValue<string>("AzureConnectionName");
                    var connectionString = secretClient.GetSecret("AzureConnectionString").Value.Value;
                    return new MassTransitConfiguration(type, ConnectionString: connectionString);
                }
        }

        return new MassTransitConfiguration(type);
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
        factoryConfigurator.Host(massConfig.Host);
    }
}