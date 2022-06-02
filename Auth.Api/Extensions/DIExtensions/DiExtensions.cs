using Auth.Api.Extensions.ConfigurationExtensions;
using Auth.Api.Extensions.JWTExtensions;
using Auth.Api.Generators;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.DbContext;
using Auth.Api.Services;
using Azure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ext = Auth.Api.Extensions.EnvironmentExtensions;
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
        services.AddSingleton(configuration.GetJwtConfiguration());
        services.AddTransient<ISenderService, SenderService>();
        services.AddApiVersioning();
        services.AddScoped<IUserService, UserService>();

        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString") ?? "",
                builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
        services.AddConfiguredMassTransit(configuration);
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(configuration); });
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationContext>("Users database", tags: new List<string> {"Database"})
            .AddAzureKeyVault(uri, new DefaultAzureCredential(), _ => { }, "Azure Key vault",
                HealthStatus.Unhealthy, new List<string> {"Azure Key Vault"});
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var massConfig = configuration.GetMassTransitConfiguration();

        services.AddMassTransit(configurator =>
        {
            if (ext.IsDevelopment() || ext.IsLocal())
                configurator.UsingRabbitMq((_, factoryConfigurator) =>
                {
                    ConfigurationExtensionsApp.ConfigureRabbitMq(factoryConfigurator, massConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((_, factoryConfigurator) =>
                {
                    ConfigurationExtensionsApp.ConfigureAzureServiceBus(factoryConfigurator, massConfig);
                });
        });
    }
}