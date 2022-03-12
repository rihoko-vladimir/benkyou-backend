using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateSetDescriptionRequest
{
    [Required] [JsonPropertyName("setId")] public string SetId { get; set; }

    [Required]
    [JsonPropertyName("newDescription")]
    public string NewName { get; set; }
}