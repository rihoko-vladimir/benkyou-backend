using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Api.Models.Requests;

public class ResetPasswordConfirmationRequest
{
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}