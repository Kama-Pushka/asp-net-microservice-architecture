using Services.Interfaces;

namespace Services;

public static class ServicesStartUp
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        // Регистрация сервисов
        services.AddScoped<IPostManager, PostManager>();

        return services;
    }
}