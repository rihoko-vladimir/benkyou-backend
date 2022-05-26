using MassTransit;
using Messages.Contracts;
using Notification.Api.Consumers;
using Notification.Api.Generators;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Services;

namespace Notification.Api.Extensions.DIExtensions;

public static class DiExtensions
{
    public static void AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailTemplateGenerator, EmailTemplateGenerator>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<SendEmailCodeConsumer>();
            configurator.AddConsumer<SendPasswordResetConsumer>();
            configurator.UsingRabbitMq((context, factoryConfigurator) =>
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
                factoryConfigurator.Host("rabbitmq", "/", hostConfigurator =>
                {
                    hostConfigurator.Username("guest");
                    hostConfigurator.Password("guest");
                });
            });
        });
    }
}