using Azure.Identity;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notification.Api.Consumers;
using Notification.Api.Extensions.ConfigurationExtensions;
using Notification.Api.Generators;
using Notification.Api.HealthChecks;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Services;
using ConfigurationExtensionsApp = Notification.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;
using ext = Notification.Api.Extensions.EnvironmentExtensions;

namespace Notification.Api.Extensions.DIExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfiguration = configuration.GetEmailConfiguration();
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));

        services.AddHealthChecks()
            .AddCheck("SmtpCheck", new PingHealthCheck(emailConfiguration.Server), tags: new List<string> {"Email"})
            .AddAzureKeyVault(uri, new DefaultAzureCredential(), options => { }, "Azure Key vault",
                HealthStatus.Unhealthy, new List<string> {"Azure Key Vault"});

        services.AddEmailSender();
        services.AddSingleton(emailConfiguration);
        services.AddConfiguredMassTransit(configuration);
    }

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
                    ConfigurationExtensionsApp.ConfigureRabbitMq(context, factoryConfigurator, massConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((context, factoryConfigurator) =>
                {
                    ConfigurationExtensionsApp.ConfigureAzureServiceBus(context, factoryConfigurator, massConfig);
                });
        });
    }
}