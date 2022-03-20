using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserInfoRequest
{
    [Required]
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = null!;

    [Required]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;

    [Required]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    [Required]
    [JsonPropertyName("birthday")]
    public DateTime? Birthday { get; set; }

    [Required] [JsonPropertyName("about")] public string? About { get; set; } = null!;

    // [Required]
    // [JsonPropertyName("currentPassword")]
    // public string CurrentPassword { get; set; } = null!;
    //
    // [Required]
    // [JsonPropertyName("newPassword")]
    // public string NewPassword { get; set; } = null!;
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; } = null!;
}