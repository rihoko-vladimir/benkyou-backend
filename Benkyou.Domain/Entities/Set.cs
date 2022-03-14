using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Benkyou.Domain.Entities;

public class Set
{
    [Key] public Guid Id { get; set; }

    [MaxLength(15)]
    [MinLength(3)]
    [Required]
    public string Name { get; set; } = null!;

    [MaxLength(90)] public string Description { get; set; } = null!;

    [Column("author_id")] public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Kanji> KanjiList { get; set; } = null!;
}