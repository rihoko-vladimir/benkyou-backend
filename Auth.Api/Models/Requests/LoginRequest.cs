using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Api.Models.Requests;

public class LoginRequest
{
    [Required] 
    [JsonPropertyName("login")]
    public string Email { get; set; }

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}