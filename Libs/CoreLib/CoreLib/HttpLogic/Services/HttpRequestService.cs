using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoreLib.HttpLogic.Entities;
using CoreLib.HttpLogic.Services.Interfaces;
using CoreLib.TraceLogic.Interfaces;
using ContentType = CoreLib.HttpLogic.Entities.ContentType;

namespace CoreLib.HttpLogic.Services;

/// <inheritdoc />
internal class HttpRequestService : IHttpRequestService
{
    private readonly IHttpConnectionService _httpConnectionService;
    private readonly IEnumerable<ITraceWriter> _traceWriterList;

    ///
    public HttpRequestService(
        IHttpConnectionService httpConnectionService,
        IEnumerable<ITraceWriter> traceWriterList)
    {
        _httpConnectionService = httpConnectionService;
        _traceWriterList = traceWriterList;
    }

    /// <inheritdoc />
    public async Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(HttpRequestData requestData,
        HttpConnectionData connectionData) // TODO
    {
        if (requestData.Uri is null)
        {
            throw new ArgumentNullException(nameof(requestData.Uri), "Request Uri is required");
        }

        var client = _httpConnectionService.CreateHttpClient(connectionData);

        using var httpRequestMessage = new HttpRequestMessage
        {
            Method = requestData.Method,
            RequestUri = requestData.Uri
        };

        foreach (var traceWriter in _traceWriterList)
        {
            var value = traceWriter.GetValue();
            if (!string.IsNullOrWhiteSpace(value))
                httpRequestMessage.Headers.TryAddWithoutValidation(traceWriter.Name, value);
        }

        foreach (var kv in requestData.HeaderDictionary)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        }

        if (requestData.Body != null && HttpMethodAllowsBody(requestData.Method))
        {
            httpRequestMessage.Content = PrepareContent(requestData.Body, requestData.ContentType);
        }

        using var responseMessage = await _httpConnectionService.SendRequestAsync(
            httpRequestMessage,
            client,
            connectionData.CancellationToken,
            connectionData.CompletionOption);

        var responseBytes = responseMessage.Content is null
            ? Array.Empty<byte>()
            : await responseMessage.Content.ReadAsByteArrayAsync(connectionData.CancellationToken);

        TResponse? body = default;
        if (typeof(TResponse) == typeof(string))
        {
            body = (TResponse)(object)Encoding.UTF8.GetString(responseBytes);
        }
        else if (typeof(TResponse) == typeof(byte[]))
        {
            body = (TResponse)(object)responseBytes;
        }
        else if (responseBytes.Length > 0)
        {
            body = JsonSerializer.Deserialize<TResponse>(responseBytes, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        return new HttpResponse<TResponse>
        {
            StatusCode = responseMessage.StatusCode,
            Headers = responseMessage.Headers,
            ContentHeaders = responseMessage.Content?.Headers,
            Body = body
        };
    }
    
    /// <summary>
    /// Проверяет, разрешено ли HTTP-методу иметь тело запроса.
    /// </summary>
    private static bool HttpMethodAllowsBody(HttpMethod method) =>
        method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch ||
        method.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase); // TODO

    /// <summary>
    /// Преобразует объект запроса в HttpContent в зависимости от типа содержимого.
    /// </summary>
    private static HttpContent PrepareContent(object body, ContentType contentType) // TODO
    {
        return contentType switch
        {
            ContentType.ApplicationJson => new StringContent(
                body is string sJson ? sJson : JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }),
                Encoding.UTF8,
                MediaTypeNames.Application.Json),

            ContentType.XWwwFormUrlEncoded => body is IEnumerable<KeyValuePair<string, string>> kv
                ? new FormUrlEncodedContent(kv)
                : throw new ArgumentException("Body must be IEnumerable<KeyValuePair<string,string>>"),

            ContentType.ApplicationXml => body is string sXml
                ? new StringContent(sXml, Encoding.UTF8, MediaTypeNames.Application.Xml)
                : throw new ArgumentException("Body must be XML string"),

            ContentType.TextXml => body is string sTextXml
                ? new StringContent(sTextXml, Encoding.UTF8, MediaTypeNames.Text.Xml)
                : throw new ArgumentException("Body must be XML string"),

            ContentType.Binary => body is byte[] bytes
                ? new ByteArrayContent(bytes)
                : throw new ArgumentException("Body must be byte[]"),

            _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null)
        };
    }
}