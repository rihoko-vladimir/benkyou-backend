using Benkyou.Domain.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benkyou.Domain.Extensions;

public static class JwtPropertiesExtension
{
    public static JwtProperties AddJwtProperties(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var audience = configuration.GetSection("JWT")["Audience"] ??
                       throw new InvalidDataException("Audience was not found");
        var issuer = configuration.GetSection("JWT")["Issuer"] ??
                     throw new InvalidDataException("Issuer was not found");
        var accessSecret = configuration.GetSection("JWT")["AccessSecret"] ??
                           throw new InvalidDataException("Access secret was not found");
        var refreshSecret = configuration.GetSection("JWT")["RefreshSecret"] ??
                            throw new InvalidDataException("Refresh secret was not found");
        var accessExpiration = configuration.GetSection("JWT")["AccessTokenExpirationTimeMinutes"] ??
                               throw new InvalidDataException("Access expiration was not found");
        var refreshExpiration = configuration.GetSection("JWT")["RefreshTokenExpirationTimeMinutes"] ??
                                throw new InvalidDataException("Refresh expiration was not found");
        var jwtProperties = new JwtProperties
        {
            Audience = audience,
            Issuer = issuer,
            AccessSecret = accessSecret,
            RefreshSecret = refreshSecret,
            AccessTokenExpirationTime = int.Parse(accessExpiration),
            RefreshTokenExpirationTime = int.Parse(refreshExpiration)
        };
        serviceCollection.AddSingleton(jwtProperties);
        return jwtProperties;
    }
}