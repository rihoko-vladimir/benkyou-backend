using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class LoginModel
{
    [Required] [JsonPropertyName("login")] public string Login { get; set; }

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}