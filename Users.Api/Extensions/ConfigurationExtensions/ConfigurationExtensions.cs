using MassTransit;
using Shared.Models.Models.Configurations;
using Shared.Models.QueueNames;
using Users.Api.Consumers;
using Users.Api.Models.Configurations;
using ext = Users.Api.Extensions.EnvironmentExtensions;

namespace Users.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static JwtConfiguration GetJwtConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtConfiguration.Key);
        var jwtConfiguration = new JwtConfiguration();
        section.Bind(jwtConfiguration);

        return jwtConfiguration;
    }

    public static AzureBlobConfiguration GetBlobConfiguration(this IConfiguration configuration)
    {
        var section = configuration.GetSection(AzureBlobConfiguration.Key);
        var blobConfiguration = new AzureBlobConfiguration();
        section.Bind(blobConfiguration);
        blobConfiguration.ConnectionString = configuration.GetConnectionString("AzureStorageBlobConnectionString");

        return blobConfiguration;
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
        IBusRegistrationContext context,
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        RabbitMqConfiguration rabbitConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);

        factoryConfigurator.Host(rabbitConfig.Host, rabbitConfig.VirtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(rabbitConfig.UserName);
            hostConfigurator.Password(rabbitConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(
        IBusRegistrationContext context,
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        AzureServiceBusConfiguration azureConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);

        factoryConfigurator.Host(azureConfig.AzureServiceBusConnectionString);
    }

    private static void ConfigureEndpoints(
        this IBusFactoryConfigurator factoryConfigurator,
        IBusRegistrationContext context)
    {
        factoryConfigurator.ReceiveEndpoint(QueueNames.RegistrationQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<RegisterUserMessageConsumer>(context); });
    }
}