using System.ComponentModel.DataAnnotations;

namespace Sets.Api.Models.Entities;

public class Onyomi
{
    [Key] public Guid Id { get; set; }

    [MaxLength(10)] public string Reading { get; set; }

    public Guid KanjiId { get; set; }

    public Kanji? Kanji { get; set; }
}