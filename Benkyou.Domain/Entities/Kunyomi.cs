using System.ComponentModel.DataAnnotations;

namespace Benkyou.Domain.Entities;

public class Kunyomi
{
    [Key] public Guid Id { get; set; }

    [MaxLength(10)] public string Reading { get; set; } = null!;

    public Guid KanjiId { get; set; }

    public Kanji? Kanji { get; set; } = null!;
}