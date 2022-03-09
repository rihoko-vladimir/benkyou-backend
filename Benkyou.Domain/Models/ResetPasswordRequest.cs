using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class ResetPasswordRequest
{
    [Required] [JsonPropertyName("email")] public string EmailAddress { get; set; }
}