using Dal;
using Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

public static class DalStartUp
{
    public static IServiceCollection AddDalServices(this IServiceCollection services)
    {
        // Подключение к SQLite
        services.AddDbContext<SqliteDbContext>(options =>
            options.UseSqlite("Data Source=IdentityService.db"));

        // Регистрация репозиториев
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        return services;
    }
}