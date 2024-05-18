using System.ComponentModel.DataAnnotations;

namespace Sets.Api.Models.Entities;

public class Kunyomi
{
    [Key] public Guid Id { get; init; }

    [MaxLength(10)] public string Reading { get; init; }

    public Guid KanjiId { get; init; }

    public Kanji? Kanji { get; init; }
}