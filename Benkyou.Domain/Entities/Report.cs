using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Benkyou.Domain.Entities;

public class Report
{
    [Key] public Guid Id { get; set; }
    [MaxLength(90)] public string Description { get; set; } = null!;

    [Column("set_id")] public Guid SetId { get; set; }

    public Set Set { get; set; } = null!;

    [Column("reporter_id")] public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}