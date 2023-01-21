using System.Text.Json.Serialization;

namespace Auth.Api.Models.Requests;

public class RegistrationRequest
{
    [JsonPropertyName("userName")] public string UserName { get; init; }

    [JsonPropertyName("email")] public string Email { get; init; }

    [JsonPropertyName("firstName")] public string FirstName { get; init; }

    [JsonPropertyName("lastName")] public string LastName { get; init; }

    [JsonPropertyName("password")] public string Password { get; init; }

    [JsonPropertyName("isTermsAccepted")] public bool IsTermsAccepted { get; init; }
}