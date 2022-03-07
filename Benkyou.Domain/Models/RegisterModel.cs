using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class RegisterModel
{
    [JsonPropertyName("login")] public string Login { get; set; } = null!;
    [JsonPropertyName("email")] public string Email { get; set; } = null!;
    [JsonPropertyName("password")] public string Password { get; set; } = null!;
    [JsonPropertyName("isTermsAccepted")] public bool IsTermsAccepted { get; set; }
}