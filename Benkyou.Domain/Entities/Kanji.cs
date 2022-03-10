using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Entities;

public class Kanji
{
    [Key] public Guid Id { get; set; }

    [MaxLength(1)]
    [MinLength(1)]
    [Required]
    [JsonPropertyName("kanji")]
    public string KanjiChar { get; set; } = null!;

    [Required]
    [JsonPropertyName("kunyoumiReadings")]
    public ICollection<Kunyomi> KunyomiReadings { get; set; } = null!;

    [Required]
    [JsonPropertyName("onyoumiReadings")]
    public ICollection<Onyomi> OnyomiReadings { get; set; } = null!;

    public Guid CardId { get; set; }

    public Card? Card { get; set; } = null!;
}