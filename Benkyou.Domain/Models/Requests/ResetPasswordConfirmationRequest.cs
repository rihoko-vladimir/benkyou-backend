using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class ResetPasswordConfirmationRequest
{
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}