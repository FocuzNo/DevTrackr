using IdentityService.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.DisplayName).HasMaxLength(100);
            builder.Property(x => x.PasswordHash).HasMaxLength(500);
        });
    }
}
