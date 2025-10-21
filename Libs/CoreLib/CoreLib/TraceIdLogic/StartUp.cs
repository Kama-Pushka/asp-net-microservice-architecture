using CoreLib.TraceLogic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreLib.TraceIdLogic;

public static class StartUpTraceId
{
    public static IServiceCollection TryAddTraceId(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TraceIdAccessor>();
        serviceCollection
            .TryAddScoped<ITraceWriter>(provider => ServiceProviderServiceExtensions.GetRequiredService<TraceIdAccessor>(provider));
        serviceCollection
            .TryAddScoped<ITraceReader>(provider => provider.GetRequiredService<TraceIdAccessor>());
        serviceCollection
            .TryAddScoped<ITraceIdAccessor>(provider => provider.GetRequiredService<TraceIdAccessor>());

        return serviceCollection;
    }
    
    public static IApplicationBuilder UseTraceId(this IApplicationBuilder app)
    {
        app.UseMiddleware<TraceIdMiddleware>();
        return app;
    }
}