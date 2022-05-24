using Auth.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Models.DbContext;

public class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; }
}