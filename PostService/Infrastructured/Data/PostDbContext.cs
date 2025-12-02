using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructured.Data;

public class PostDbContext : DbContext
{
    public PostDbContext(DbContextOptions<PostDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка сущностей
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired();
            entity.Property(p => p.Content).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.UpdatedAt);
            entity.OwnsOne(
                e => e.UserInfo,
                ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.Property(u => u.Id).HasColumnName("UserInfo_UserId").IsRequired();
                    ownedNavigationBuilder.Property(u => u.Name).HasColumnName("UserInfo_Name").IsRequired();
                });
        });
        
    }
}