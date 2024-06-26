using Auth.Api.Configurations;
using Auth.Api.Extensions.ConfigurationExtensions;
using Auth.Api.Extensions.JWTExtensions;
using Auth.Api.Generators;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Repositories;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.DbContext;
using Auth.Api.Repositories;
using Auth.Api.Services;
using Auth.Api.Validators;
using Azure.Identity;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        services.AddTransient<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        services.AddScoped<IUserService, UserService>();

        services.AddConfiguredMassTransit(configuration);

        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("AuthSqlServerConnectionString") ?? "",
                builder =>
                {
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    builder.EnableRetryOnFailure(40);
                });
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(configuration); });

        services.AddFido2(options =>
        {
            options.ServerDomain = configuration["webauthn:domain"];
            options.ServerName = "Benkyou Auth";
            options.Origins = configuration.GetSection("webauthn:origins").Get<HashSet<string>>();
            options.TimestampDriftTolerance = configuration.GetValue<int>("webauthn:timestampDriftTolerance");
        });

        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri")!);

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationContext>("Users database",
                tags: new List<string>
                {
                    "Database"
                });
        if (EnvironmentExtensions.IsDevelopment())
            services.AddHealthChecks().AddAzureKeyVault(uri,
                new DefaultAzureCredential(),
                _ => { },
                "Azure Key vault",
                HealthStatus.Unhealthy,
                new List<string>
                {
                    "Azure Key Vault"
                });

        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddValidatorsFromAssemblyContaining<RegistrationDataValidator>();

        services.AddValidatorsFromAssemblyContaining<ResetPasswordDataValidator>();
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            if (ext.IsDevelopment() || ext.IsLocal())
                configurator.UsingRabbitMq((_, factoryConfigurator) =>
                {
                    var rabbitConfig = configuration.GetRabbitMqConfiguration();
                    ConfigurationExtensionsApp.ConfigureRabbitMq(factoryConfigurator, rabbitConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((_, factoryConfigurator) =>
                {
                    var busConfig = configuration.GetServiceBusConfiguration();
                    ConfigurationExtensionsApp.ConfigureAzureServiceBus(factoryConfigurator, busConfig);
                });
        });
    }
}