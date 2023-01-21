using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sets.Api.Extensions.ConfigurationExtensions;

namespace Sets.Api.Extensions.JWTExtensions;

public static class JwtOptionsExtensions
{
    public static void ConfigureJwtBearer(this JwtBearerOptions options, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetJwtConfiguration();

        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.AccessSecret));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        options.TokenValidationParameters = validationParameters;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access"];

                return Task.CompletedTask;
            }
        };
    }
}