using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class LoginModel
{
    [Required] [JsonPropertyName("login")] public string Email { get; set; }

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}