using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class ModifySetRequest
{
    [Required] [JsonPropertyName("setId")] public string SetId { get; set; }

    [Required] [JsonPropertyName("name")] public string SetName { get; set; }

    [Required]
    [JsonPropertyName("description")]
    public string SetDescription { get; set; }

    [Required]
    [JsonPropertyName("kanjiList")]
    public ICollection<KanjiRequest> KanjiList { get; set; }
}