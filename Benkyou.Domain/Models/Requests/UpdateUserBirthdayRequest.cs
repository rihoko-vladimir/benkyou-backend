using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserBirthdayRequest
{
    [JsonPropertyName("birthday")]
    [Required]
    public string Birthday { get; set; }
}