using Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class SqliteDbContext : DbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
    {
    }

    public DbSet<UserDal> Users { get; set; }
    public DbSet<RoleDal> Roles { get; set; }
    public DbSet<UserRoleDal> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка сущностей
        modelBuilder.Entity<UserDal>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            // entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<RoleDal>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired();
        });

        modelBuilder.Entity<UserRoleDal>(entity =>
        {
            entity.HasKey(ur => ur.Id);
            entity.HasOne<UserDal>().WithMany().HasForeignKey(ur => ur.UserId);
            entity.HasOne<RoleDal>().WithMany().HasForeignKey(ur => ur.RoleId);
        });
    }
}