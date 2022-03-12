using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Benkyou.Domain.Entities;

namespace Benkyou.Domain.Models.Requests;

public class CreateSetRequest
{
    [Required] [JsonPropertyName("name")] public string SetName { get; set; }

    [Required]
    [JsonPropertyName("description")]
    public string SetDescription { get; set; }

    [Required]
    [JsonPropertyName("kanjiList")]
    public ICollection<Kanji> KanjiList { get; set; }
}