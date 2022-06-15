using Microsoft.EntityFrameworkCore;
using Sets.Api.Models.Entities;

namespace Sets.Api.Models.DbContext;
using DbContext = Microsoft.EntityFrameworkCore.DbContext; 

public class ApplicationContext : DbContext
{
    public DbSet<Set> Sets { get; set; }
    public DbSet<Kanji> Kanji { get; set; }
    public DbSet<Kunyomi> Kunyomis { get; set; }
    public DbSet<Onyomi> Onyomis { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
}