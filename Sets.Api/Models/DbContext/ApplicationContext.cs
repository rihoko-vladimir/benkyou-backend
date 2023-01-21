using Microsoft.EntityFrameworkCore;
using Sets.Api.Models.Entities;

namespace Sets.Api.Models.DbContext;

public class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Set> Sets { get; set; }
    public DbSet<Kanji> Kanji { get; set; }
    public DbSet<Kunyomi> Kunyomis { get; set; }
    public DbSet<Onyomi> Onyomis { get; set; }
}