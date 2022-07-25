using Azure.Identity;
using Dapper;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Users.Api.Common.Factories;
using Users.Api.Common.HealthChecks;
using Users.Api.Common.MapperProfiles;
using Users.Api.Common.TypeHandlers;
using Users.Api.Common.Validators;
using Users.Api.Configurations;
using Users.Api.Consumers;
using Users.Api.Extensions.ConfigurationExtensions;
using Users.Api.Extensions.JWTExtensions;
using Users.Api.Interfaces.Factories;
using Users.Api.Interfaces.Repositories;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Configurations;
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
        services.AddSingleton(new DbHealthCheck(configuration));
        services.AddSingleton<IAccessTokenService, AccessTokenService>();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddTransient<ISenderService, SenderService>();
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IUserInformationService, UserInformationService>();

        services.AddConfiguredMassTransit(configuration);

        services.AddAutoMapper(expression => { expression.AddProfile<AutoMappingProfile>(); });

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

        var vaultUri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var blobUri = configuration.GetConnectionString("AzureStorageBlobConnectionString");
        var containerName = configuration.GetSection(AzureBlobConfiguration.Key).GetValue<string>("ContainerName");
        
        services.AddHealthChecks()
            .AddCheck<DbHealthCheck>("User information database",
                tags: new List<string> { "Database" })
            .AddAzureKeyVault(vaultUri,
                new DefaultAzureCredential(),
                _ => { },
                "Azure Key vault",
                HealthStatus.Unhealthy,
                new List<string> { "Azure Key Vault" })
            .AddAzureBlobStorage(
                blobUri,
                containerName,
                name: "Storage Blob",
                failureStatus: HealthStatus.Unhealthy,
                tags: new List<string> { "Storage Blob" });

        SqlMapper.AddTypeHandler(new TrimmedStringHandler());

        services.AddValidatorsFromAssemblyContaining<UserInfoValidator>();

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options => { options.WaitUntilStarted = true; });
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