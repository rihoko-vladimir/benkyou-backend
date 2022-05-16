using Benkyou.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Benkyou.Domain.Database;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Set> Sets { get; set; } = null!;

    public DbSet<Kanji> KanjiList { get; set; } = null!;

    public DbSet<Kunyomi> KunyomiList { get; set; } = null!;

    public DbSet<Onyomi> OnyomiList { get; set; } = null!;

    public new DbSet<User> Users { get; set; } = null!;

    public DbSet<Report> Reports { get; set; } = null!;

    public DbSet<UserStatistic> UserStatistics { get; set; } = null!;
}