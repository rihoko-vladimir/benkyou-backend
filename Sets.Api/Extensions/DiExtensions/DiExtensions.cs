using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sets.Api.Common.MappingProfiles;
using Sets.Api.Common.Validators;
using Sets.Api.Extensions.ConfigurationExtensions;
using Sets.Api.Extensions.JWTExtensions;
using Sets.Api.Models.DbContext;
using ext = Sets.Api.Extensions.EnvironmentExtensions;
using ConfigurationExtensionsApp = Sets.Api.Extensions.ConfigurationExtensions.ConfigurationExtensions;

namespace Sets.Api.Extensions.DiExtensions;

public static class DiExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(builder =>
        {
            builder.UseSqlServer(configuration.GetConnectionString("SetsSqlServerConnectionString"));
        });

        services.AddAutoMapper(expression =>
        {
            expression.AddProfile<ApplicationProfile>();
        });
        
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.ReportApiVersions = true;
        });
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(configuration); });
        
        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        
        services.AddConfiguredMassTransit(configuration);
        
        services.AddValidatorsFromAssemblyContaining<KanjiValidator>();
        services.AddValidatorsFromAssemblyContaining<SetValidator>();
        services.AddValidatorsFromAssemblyContaining<OnyomiValidator>();
        services.AddValidatorsFromAssemblyContaining<KunyomiValidator>();
    }

    private static void AddConfiguredMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
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