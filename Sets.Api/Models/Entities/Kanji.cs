using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sets.Api.Models.Entities;

public class Kanji
{
    [Key] 
    public Guid Id { get; set; }

    [MaxLength(1)]
    [MinLength(1)]
    [Required]
    [JsonPropertyName("kanji")]
    public string KanjiChar { get; set; }

    [Required]
    [JsonPropertyName("kunyoumiReadings")]
    public ICollection<Kunyomi> KunyomiReadings { get; set; }

    [Required]
    [JsonPropertyName("onyoumiReadings")]
    public ICollection<Onyomi> OnyomiReadings { get; set; }

    public Guid SetId { get; set; }

    public Set Set { get; set; }
}