using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; } = null!;
}