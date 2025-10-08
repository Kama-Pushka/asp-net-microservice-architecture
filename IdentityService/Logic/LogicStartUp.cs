using Logic;
using Logic.Interfaces;

public static class LogicStartUp
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        // Регистрация сервисов
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<IUserRoleManager, UserRoleManager>();

        return services;
    }
}