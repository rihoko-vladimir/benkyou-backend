using System.ComponentModel.DataAnnotations;

namespace Benkyou.Domain.Entities;

public class Kanji
{
    [Key] public Guid Id { get; set; }

    [MaxLength(1)]
    [MinLength(1)]
    [Required]
    public string KanjiChar { get; set; } = null!;

    public ICollection<Kunyomi> KunyomiReadings { get; set; } = null!;

    public ICollection<Onyomi> OnyomiReadings { get; set; } = null!;

    public Guid CardId { get; set; }

    public Card Card { get; set; } = null!;
}