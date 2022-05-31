using Auth.Api.Extensions.ConfigurationExtensions;
using Auth.Api.Generators;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.Configuration;
using Auth.Api.Models.DbContext;
using Auth.Api.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ConfigurationExtensionsApp = Auth.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;

namespace Auth.Api.Extensions.DIExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEmailCodeGenerator, EmailCodeGenerator>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IAccessTokenService, AccessTokenService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<IResetTokenService, ResetTokenService>();
        services.AddTransient<ISenderService, SenderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString") ?? "",
                builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var massConfig = configuration.GetMassTransitConfiguration();
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