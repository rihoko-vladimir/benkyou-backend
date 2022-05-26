using Auth.Api.Generators;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Services;
using MassTransit;

namespace Auth.Api.DIExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IEmailCodeGenerator, EmailCodeGenerator>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IAccessTokenService, AccessTokenService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<IResetTokenService, ResetTokenService>();
        services.AddTransient<ISenderService, SenderService>();
        services.AddScoped<IUserService, UserService>();
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((_, factoryConfigurator) =>
            {
                factoryConfigurator.Host("rabbitmq", "/", hostConfigurator =>
                {
                    hostConfigurator.Username("guest");
                    hostConfigurator.Password("guest");
                });
            });
        });
    }
}