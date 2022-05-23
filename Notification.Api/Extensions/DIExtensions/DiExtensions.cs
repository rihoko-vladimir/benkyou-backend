using Notification.Api.Generators;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Services;

namespace Notification.Api.Extensions.DIExtensions;

public static class DiExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailTemplateGenerator, EmailTemplateGenerator>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        return services;
    }
}