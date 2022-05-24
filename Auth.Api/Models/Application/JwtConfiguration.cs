namespace Auth.Api.Models.Application;

public record JwtConfiguration(string Audience, string Issuer, string AccessSecret, string RefreshSecret, string ResetSecret,
    int AccessExpiresIn, int RefreshExpiresIn, int ResetExpiresIn);