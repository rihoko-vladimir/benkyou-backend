using Microsoft.EntityFrameworkCore;
using Sets.Api.Models.Entities;

namespace Sets.Api.Models.DbContext;

public class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Set> Sets { get; init; }
    public DbSet<Kanji> Kanji { get; init; }
    public DbSet<Kunyomi> Kunyomis { get; init; }
    public DbSet<Onyomi> Onyomis { get; init; }
}