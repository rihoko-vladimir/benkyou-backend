using MassTransit;
using Sets.Api.Consumers;
using Shared.Models.Models.Configurations;
using Shared.Models.QueueNames;

namespace Sets.Api.Extensions.ConfigurationExtensions;

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
        AzureServiceBusConfiguration busConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);
        factoryConfigurator.Host(busConfig.AzureServiceBusConnectionString);
    }
    
    private static void ConfigureEndpoints(this IBusFactoryConfigurator factoryConfigurator,
        IBusRegistrationContext context)
    {
        factoryConfigurator.ReceiveEndpoint(QueueNames.AccountVisibilityChangeQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<UpdateAccountVisibilityConsumer>(context); });
    }
}