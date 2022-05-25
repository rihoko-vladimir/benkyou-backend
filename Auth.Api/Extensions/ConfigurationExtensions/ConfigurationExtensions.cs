using Auth.Api.Models.Application;

namespace Auth.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static JwtConfiguration GetJwtConfiguration(this IConfiguration configuration)
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
}