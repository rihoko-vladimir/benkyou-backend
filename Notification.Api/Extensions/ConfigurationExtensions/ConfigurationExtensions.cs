using MassTransit;
using Notification.Api.Consumers;
using Notification.Api.Models;
using Shared.Models;
using ext = Notification.Api.Extensions.EnvironmentExtensions;

namespace Notification.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static EmailConfiguration GetEmailConfiguration(this IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("SmtpConfiguration");
        var emailConfig = new EmailConfiguration();

        configurationSection.Bind(emailConfig);
        return emailConfig;
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
        IBusRegistrationContext context,
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.Host(massConfig.Host, massConfig.VirtualHost, hostConfigurator =>
        {
            factoryConfigurator.ConfigureEndpoints(context);

            hostConfigurator.Username(massConfig.UserName);
            hostConfigurator.Password(massConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(
        IBusRegistrationContext context,
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.ConfigureEndpoints(context);

        factoryConfigurator.Host(massConfig.AzureServiceBusConnectionString);
    }

    private static void ConfigureEndpoints(this IBusFactoryConfigurator factoryConfigurator,
        IBusRegistrationContext context)
    {
        factoryConfigurator.ReceiveEndpoint(QueueNames.EmailConfirmationQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<SendEmailCodeConsumer>(context); });
        factoryConfigurator.ReceiveEndpoint(QueueNames.PasswordResetQueue,
            endpointConfigurator => { endpointConfigurator.ConfigureConsumer<SendPasswordResetConsumer>(context); });
    }
}