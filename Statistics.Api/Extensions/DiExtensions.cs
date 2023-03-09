using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Statistics.Api.Common.MapperProfiles;
using Statistics.Api.Configurations;
using Statistics.Api.Consumers;
using Statistics.Api.Extensions.JWTExtensions;
using Statistics.Api.Interfaces.Repositories;
using Statistics.Api.Interfaces.Services;
using Statistics.Api.Models.Configurations;
using Statistics.Api.Repositories;
using Statistics.Api.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Statistics.Api.Extensions;

using ext = EnvironmentExtensions;
using ConfigurationExtensionsApp = ConfigurationExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        services.AddTransient<IStatisticsRepository, StatisticsRepository>();
        services.AddTransient<IStatisticsService, StatisticsService>();
        services.AddSingleton<IAccessTokenService, AccessTokenService>();

        var mongoConfig = new MongoConfig
        {
            ConnectionString = configuration.GetConnectionString("StatisticsMongoDbConnectionString") ?? throw new InvalidOperationException()
        };
        services.AddSingleton(mongoConfig);

        services.AddHealthChecks()
            .AddMongoHealthCheck(mongoConfig.ConnectionString, "Statistics database");

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

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(configuration); });

        services.AddConfiguredMassTransit(configuration);

        services.AddAutoMapper(expression => { expression.AddProfile<AppProfile>(); });
    }

    public static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<UserRegisterMessageConsumer>();
            configurator.AddConsumer<FinishLearningConsumer>();

            if (ext.IsDevelopment() || ext.IsLocal())

                configurator.UsingRabbitMq((context, factoryConfigurator) =>
                {
                    var rabbitConfig = configuration.GetRabbitMqConfiguration();
                    ConfigurationExtensionsApp.ConfigureRabbitMq(context, factoryConfigurator, rabbitConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((context, factoryConfigurator) =>
                {
                    var busConfig = configuration.GetServiceBusConfiguration();
                    ConfigurationExtensionsApp.ConfigureAzureServiceBus(context, factoryConfigurator, busConfig);
                });
        });
    }
}