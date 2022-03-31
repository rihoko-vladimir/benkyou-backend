using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserInfoRequest
{
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;

    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    
    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }

    [JsonPropertyName("about")] public string? About { get; set; } = null!;

    [JsonPropertyName("avatar")] public string? Avatar { get; set; } = null!;
}