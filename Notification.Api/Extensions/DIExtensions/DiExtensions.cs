using MassTransit;
using Notification.Api.Consumers;
using Notification.Api.Extensions.ConfigurationExtensions;
using ConfigurationExtensionsApp = Notification.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;
using Notification.Api.Generators;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Models;
using Notification.Api.Services;

namespace Notification.Api.Extensions.DIExtensions;

public static class DiExtensions
{
    public static void AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailTemplateGenerator, EmailTemplateGenerator>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var massConfig = configuration.GetMassTransitConfiguration();
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<SendEmailCodeConsumer>();
            configurator.AddConsumer<SendPasswordResetConsumer>();
            switch (massConfig.Type)
            {
                case MassTransitType.RabbitMq:
                    configurator.UsingRabbitMq((context, factoryConfigurator) =>
                    {
                        ConfigurationExtensionsApp.ConfigureRabbitMq(context, factoryConfigurator, massConfig);
                    });
                    break;
                case MassTransitType.AzureServiceBus:
                    configurator.UsingAzureServiceBus((context, factoryConfigurator) =>
                    {
                        ConfigurationExtensionsApp.ConfigureAzureServiceBus(context, factoryConfigurator, massConfig);
                    });
                    break;
            }
        });
    }
}