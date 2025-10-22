using Domain.Interfaces;
using IdentityConnectionLib.ConnectionServices;
using IdentityConnectionLib.ConnectionServices.Interfaces;
using Infrastructured.Connections;
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
        
        services.AddScoped<IStorePost, PostRepository>();
        services.AddScoped<ICheckUser, CheckUser>();
        services.AddScoped<IIdentityConnectionService, IdentityConnectionService>();

        return services;
    }
}