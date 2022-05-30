using Azure.Security.KeyVault.Secrets;
using MassTransit;
using Messages.Contracts;
using Notification.Api.Consumers;
using Notification.Api.Models;

namespace Notification.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static EmailConfiguration GetEmailConfiguration(this IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("SmtpConfiguration");
        var server = configurationSection.GetValue<string>("Server");
        var serverPort = configurationSection.GetValue<int>("ServerPort");
        var login = configurationSection.GetValue<string>("Login");
        var password = configurationSection.GetValue<string>("Password");
        return new EmailConfiguration(server, serverPort, login, password);
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
                var host = configurationSection.GetValue<string>("host");
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

    public static void ConfigureRabbitMq(IBusRegistrationContext context,
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);
        factoryConfigurator.Host(massConfig.Host, massConfig.VirtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(massConfig.UserName);
            hostConfigurator.Password(massConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(IBusRegistrationContext context,
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);
        factoryConfigurator.Host(massConfig.ConnectionString);
    }

    private static void ConfigureEndpoints(this IBusFactoryConfigurator factoryConfigurator,
        IBusRegistrationContext context)
    {
        factoryConfigurator.ReceiveEndpoint(QueueNames.EmailConfirmationQueue,
            endpointConfigurator =>
            {
                endpointConfigurator.ConfigureConsumer<SendEmailCodeConsumer>(context);
            });
        factoryConfigurator.ReceiveEndpoint(QueueNames.PasswordResetQueue,
            endpointConfigurator =>
            {
                endpointConfigurator.ConfigureConsumer<SendPasswordResetConsumer>(context);
            });
    }
}