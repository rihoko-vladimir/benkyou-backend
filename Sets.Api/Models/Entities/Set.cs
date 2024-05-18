using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sets.Api.Models.Entities;

public class Set
{
    [Key] public Guid Id { get; init; }

    [MaxLength(15)]
    [MinLength(3)]
    [Required]
    public string Name { get; set; }

    [MaxLength(90)] public string Description { get; set; }

    [Column("author_id")] public Guid UserId { get; set; }

    public bool IsPublic { get; set; }

    public ICollection<Kanji> KanjiList { get; set; }
}