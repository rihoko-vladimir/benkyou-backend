using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Users.Api.Configurations;
using Users.Api.Consumers;
using Users.Api.Extensions.ConfigurationExtensions;
using Users.Api.Interfaces.Repositories;
using Users.Api.Interfaces.Services;
using Users.Api.MapperProfiles;
using Users.Api.Repositories;
using Users.Api.Services;
using ext = Users.Api.Extensions.EnvironmentExtensions;
using massExt = Users.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;


namespace Users.Api.Extensions.DiExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        services.AddSingleton(configuration.GetJwtConfiguration());
        services.AddSingleton(configuration.GetBlobConfiguration());
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IUserInformationService, UserInformationService>();
        services.AddConfiguredMassTransit(configuration);
        
        services.AddAutoMapper(expression =>
        {
            expression.AddProfile<AutoMappingProfile>();
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
    }

    private static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<RegisterUserMessageConsumer>();
            if (ext.IsDevelopment() || ext.IsLocal())
                configurator.UsingRabbitMq((context, factoryConfigurator) =>
                {
                    var rabbitConfig = configuration.GetRabbitMqConfiguration();
                    massExt.ConfigureRabbitMq(context, factoryConfigurator, rabbitConfig);
                });

            if (ext.IsProduction())
                configurator.UsingAzureServiceBus((context, factoryConfigurator) =>
                {
                    var busConfig = configuration.GetServiceBusConfiguration();
                    massExt.ConfigureAzureServiceBus(context, factoryConfigurator, busConfig);
                });
        });
    }
}