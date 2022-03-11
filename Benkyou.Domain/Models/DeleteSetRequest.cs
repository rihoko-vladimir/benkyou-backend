using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class DeleteSetRequest
{
    [JsonPropertyName("setId")] [Required] public string SetId { get; set; }
}