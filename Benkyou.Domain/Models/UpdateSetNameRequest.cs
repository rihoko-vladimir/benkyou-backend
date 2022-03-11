using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class UpdateSetNameRequest
{
    [Required] [JsonPropertyName("setId")] public string SetId { get; set; }

    [Required]
    [JsonPropertyName("newName")]
    public string NewName { get; set; }
}