using System.Text.Json.Serialization;

namespace Auth.Api.Models.Requests;

public class ConfirmEmailRequest
{
    [JsonPropertyName("emailCode")] public string EmailCode { get; init; } = null!;

    [JsonPropertyName("userId")] public Guid UserId { get; init; }
}