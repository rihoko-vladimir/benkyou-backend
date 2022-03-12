using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class VerifyEmailCodeRequest
{
    [JsonPropertyName("emailCode")] public string EmailCode { get; set; } = null!;

    [JsonPropertyName("userId")] public Guid UserId { get; set; }
}