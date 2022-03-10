using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class UpdateSetKanjiListRequest
{
    [Required]
    [JsonPropertyName("setId")]
    public string SetId { get; set; }
    [Required]
    [JsonPropertyName("newKanjiList")]
    public ICollection<KanjiRequest> NewKanjiList { get; set; }
}