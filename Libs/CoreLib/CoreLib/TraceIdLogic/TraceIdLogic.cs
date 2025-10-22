using CoreLib.TraceLogic.Interfaces;
using Serilog.Context;

namespace CoreLib.TraceIdLogic;

internal class TraceIdAccessor : ITraceReader, ITraceWriter, ITraceIdAccessor
{
    public string Name => "TraceId";

    private string _value;
    
    public string GetValue()
    {
        return _value;
    }

    public void WriteValue(string value)
    {
        // на случай если это первый в цепочке сервис и до этого не было traceId
        if (string.IsNullOrWhiteSpace(value))
        {
            value = Guid.NewGuid().ToString();
        }
        
        _value = value;
        LogContext.PushProperty("TraceId", value);
    }
}