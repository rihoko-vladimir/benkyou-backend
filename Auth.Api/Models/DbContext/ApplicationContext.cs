using Auth.Api.Models.Entities;
using Fido2NetLib.Development;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Models.DbContext;

public class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredCredential>()
            .HasKey(credential => credential.AaGuid);
    }

    public DbSet<UserCredential> UserCredentials { get; init; }

    public DbSet<Token> Tokens { get; init; }

    public DbSet<StoredCredential> FidoCredentials { get; init; }
}