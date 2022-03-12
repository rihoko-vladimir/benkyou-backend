using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserLastNameRequest
{
    [JsonPropertyName("lastName")]
    [Required]
    public string LastName { get; set; }
}