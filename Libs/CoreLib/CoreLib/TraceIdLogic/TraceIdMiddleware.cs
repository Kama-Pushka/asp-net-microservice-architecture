using CoreLib.TraceLogic.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CoreLib.TraceIdLogic;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;
    
    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, ITraceReader traceReader, ITraceWriter traceWriter)
    {
        const string traceHeaderName = "TraceId";

        if (context.Request.Headers.TryGetValue(traceHeaderName, out var traceId) 
            && !string.IsNullOrWhiteSpace(traceId))
        {
            traceReader.WriteValue(traceId);
        }
        else
        {
            traceReader.WriteValue(null);
            context.Request.Headers[traceHeaderName] = traceWriter.GetValue();
        }

        await _next(context);
    }
}