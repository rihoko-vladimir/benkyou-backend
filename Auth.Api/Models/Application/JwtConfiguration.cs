namespace Auth.Api.Models.Application;

public record JwtConfiguration(string Audience, string Issuer, string AccessSecret, string RefreshSecret,
    int AccessExpiresIn, int RefreshExpiresIn);