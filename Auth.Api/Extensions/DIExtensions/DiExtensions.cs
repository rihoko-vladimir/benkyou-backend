using Auth.Api.Extensions.ConfigurationExtensions;
using Auth.Api.Generators;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using ConfigurationExtensionsApp = Auth.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;
using Auth.Api.Models.Configuration;
using Auth.Api.Services;
using Azure.Security.KeyVault.Secrets;
using MassTransit;

namespace Auth.Api.Extensions.DIExtensions;

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

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration, SecretClient secretClient)
    {
        var massConfig = configuration.GetMassTransitConfiguration(secretClient);
        services.AddMassTransit(configurator =>
        {
            switch (massConfig.Type)
            {
                case MassTransitType.RabbitMq:
                    configurator.UsingRabbitMq((_, factoryConfigurator) =>
                    {
                        ConfigurationExtensionsApp.ConfigureRabbitMq(factoryConfigurator, massConfig);
                    });
                    break;
                case MassTransitType.AzureServiceBus:
                    configurator.UsingAzureServiceBus((_, factoryConfigurator) =>
                    {
                        ConfigurationExtensionsApp.ConfigureAzureServiceBus(factoryConfigurator, massConfig);
                    });
                    break;
            }
        });
        
    }
}