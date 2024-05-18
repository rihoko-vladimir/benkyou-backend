using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sets.Api.Models.Entities;

public class Kanji
{
    [Key] public Guid Id { get; init; }

    [MaxLength(1)]
    [MinLength(1)]
    [Required]
    [JsonPropertyName("kanji")]
    public string KanjiChar { get; init; }

    [Required]
    [JsonPropertyName("kunyoumiReadings")]
    public ICollection<Kunyomi> KunyomiReadings { get; init; }

    [Required]
    [JsonPropertyName("onyoumiReadings")]
    public ICollection<Onyomi> OnyomiReadings { get; init; }

    public Guid SetId { get; init; }

    public Set Set { get; init; }
}