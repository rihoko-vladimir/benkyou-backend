using Auth.Api.Models.Application;
using Auth.Api.Models.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MassTransit;
using ext = Auth.Api.Extensions.EnvironmentExtensions;

namespace Auth.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static JwtConfiguration GetJwtConfiguration(this IConfiguration configuration)
    {
        if (ext.IsDevelopment() || ext.IsLocal())
        {
            var section = configuration.GetSection("JWTConfiguration");
            var audience = section.GetValue<string>("Audience");
            var issuer = section.GetValue<string>("Issuer");
            var accessSecret = section.GetValue<string>("AccessSecret");
            var refreshSecret = section.GetValue<string>("RefreshSecret");
            var resetSecret = section.GetValue<string>("ResetSecret");
            var accessExpiresIn = section.GetValue<int>("AccessTokenExpirationTimeMinutes");
            var refreshExpiresIn = section.GetValue<int>("RefreshTokenExpirationTimeMinutes");
            var resetExpiresIn = section.GetValue<int>("ResetTokenExpirationTimeMinutes");
            return new JwtConfiguration(audience, issuer, accessSecret, refreshSecret, resetSecret, accessExpiresIn,
                refreshExpiresIn, resetExpiresIn);
        }

        if (!ext.IsProduction()) throw new ConfigurationException("Incorrect environment provided");
        {
            var section = configuration.GetSection("JWTAzureKeyVaultKeys");
            var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
            var secretClient = new SecretClient(uri, new DefaultAzureCredential());
            var audienceKey = section.GetValue<string>("AudienceKey");
            var issuerKey = section.GetValue<string>("IssuerKey");
            var accessSecretKey = section.GetValue<string>("AccessSecretKey");
            var refreshSecretKey = section.GetValue<string>("RefreshSecretKey");
            var resetSecretKey = section.GetValue<string>("ResetSecretKey");
            var accessExpirationKey = section.GetValue<string>("AccessTokenExpirationTimeMinutesKey");
            var refreshExpirationKey = section.GetValue<string>("RefreshTokenExpirationTimeMinutesKey");
            var resetExpirationKey = section.GetValue<string>("ResetTokenExpirationTimeMinutesKey");
            var audience = secretClient.GetSecret(audienceKey).Value.Value;
            var issuer = secretClient.GetSecret(issuerKey).Value.Value;
            var accessSecret = secretClient.GetSecret(accessSecretKey).Value.Value;
            var refreshSecret = secretClient.GetSecret(refreshSecretKey).Value.Value;
            var resetSecret = secretClient.GetSecret(resetSecretKey).Value.Value;
            var accessExpiresIn = secretClient.GetSecret(accessExpirationKey).Value.Value;
            var refreshExpiresIn = secretClient.GetSecret(refreshExpirationKey).Value.Value;
            var resetExpiresIn = secretClient.GetSecret(resetExpirationKey).Value.Value;
            return new JwtConfiguration(audience,
                issuer,
                accessSecret,
                refreshSecret,
                resetSecret,
                int.Parse(accessExpiresIn),
                int.Parse(refreshExpiresIn),
                int.Parse(resetExpiresIn));
        }
    }

    public static MassTransitConfiguration GetMassTransitConfiguration(this IConfiguration configuration)
    {
        if (ext.IsDevelopment() || ext.IsLocal())
        {
            var configurationSection = configuration.GetSection("MassTransitConfiguration");
            var host = configurationSection.GetValue<string>("Host");
            var virtualHost = configurationSection.GetValue<string>("VirtualHost");
            var userName = configurationSection.GetValue<string>("UserName");
            var password = configurationSection.GetValue<string>("Password");
            return new MassTransitConfiguration(MassTransitType.RabbitMq, host, virtualHost, userName, password);
        }

        if (!ext.IsProduction()) throw new ConfigurationException("Configuration is incorrect");
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var secretClient = new SecretClient(uri, new DefaultAzureCredential());
        var connectionString = secretClient.GetSecret("AzureServiceBusConnectionString").Value.Value;
        return new MassTransitConfiguration(MassTransitType.AzureServiceBus, ConnectionString: connectionString);

    }

    public static void ConfigureRabbitMq(
        IRabbitMqBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.Host(massConfig.Host, massConfig.VirtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(massConfig.UserName);
            hostConfigurator.Password(massConfig.Password);
        });
    }

    public static void ConfigureAzureServiceBus(
        IServiceBusBusFactoryConfigurator factoryConfigurator,
        MassTransitConfiguration massConfig)
    {
        factoryConfigurator.Host(massConfig.ConnectionString);
    }
}