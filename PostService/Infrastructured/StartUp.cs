using Domain.Interfaces;
using Infrastructured.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructured;

public static class InfrastructuredStartUp
{
    public static IServiceCollection AddInfrastructuredServices(this IServiceCollection services)
    {
        // Подключение к SQLite
        services.AddDbContext<PostDbContext>(options =>
            options.UseSqlite("Data Source=PostService.db"));

        // Регистрация репозиториев
        services.AddScoped<IStorePost, PostRepository>();

        return services;
    }
}