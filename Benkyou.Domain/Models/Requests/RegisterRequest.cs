using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class RegisterModel
{
    [JsonPropertyName("login")] public string Login { get; set; } = null!;
    [JsonPropertyName("email")] public string Email { get; set; } = null!;
    [JsonPropertyName("firstName")] public string FirstName { get; set; } = null!;
    [JsonPropertyName("lastName")] public string LastName { get; set; } = null!;
    [JsonPropertyName("password")] public string Password { get; set; } = null!;
    [JsonPropertyName("isTermsAccepted")] public bool IsTermsAccepted { get; set; }
}