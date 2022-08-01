using MassTransit;
using Shared.Models.Models.Configurations;
using Shared.Models.QueueNames;
using Statistics.Api.Consumers;

namespace Statistics.Api.Extensions;

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
        ConfigureEndpoints(factoryConfigurator, context);

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
        ConfigureEndpoints(factoryConfigurator, context);

        factoryConfigurator.Host(busConfig.AzureServiceBusConnectionString);
    }

    private static void ConfigureEndpoints(
        this IBusFactoryConfigurator factoryConfigurator,
        IBusRegistrationContext context)
    {
        factoryConfigurator.ReceiveEndpoint(QueueNames.RegistrationTimeQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<UserRegisterMessageConsumer>(context); });
        factoryConfigurator.ReceiveEndpoint(QueueNames.FinishSetLearningQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<FinishLearningConsumer>(context); });
    }
}