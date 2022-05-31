using MassTransit;
using Notification.Api.Consumers;
using Notification.Api.Extensions.ConfigurationExtensions;
using Notification.Api.Generators;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Services;
using ConfigurationExtensionsApp = Notification.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;
using ext = Notification.Api.Extensions.EnvironmentExtensions;

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
            if (ext.IsDevelopment() || ext.IsLocal())
                configurator.UsingRabbitMq((context, factoryConfigurator) =>
                {
                    ConfigurationExtensionsApp.ConfigureRabbitMq(factoryConfigurator, massConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((context, factoryConfigurator) =>
                {
                    ConfigurationExtensionsApp.ConfigureAzureServiceBus(factoryConfigurator, massConfig);
                });
        });
    }
}