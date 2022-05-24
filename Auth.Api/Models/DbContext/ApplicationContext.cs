using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Models;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; }
}