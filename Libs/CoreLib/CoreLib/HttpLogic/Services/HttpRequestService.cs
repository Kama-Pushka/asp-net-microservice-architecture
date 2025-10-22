using System.Net.Http.Json;
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
        HttpConnectionData connectionData)
    {
        if (requestData.Uri is null)
            throw new ArgumentNullException(nameof(requestData.Uri), "Request Uri is required");

        var client = _httpConnectionService.CreateHttpClient(connectionData);

        var httpRequestMessage = PrepareRequestMessage(requestData);

        var responseMessage = await _httpConnectionService.SendRequestAsync(
            httpRequestMessage,
            client,
            connectionData.CancellationToken,
            connectionData.CompletionOption);

        var body = responseMessage.Content is null
            ? default
            : await responseMessage.Content.ReadFromJsonAsync<TResponse>(connectionData.CancellationToken);

        return new HttpResponse<TResponse>
        {
            StatusCode = responseMessage.StatusCode,
            Headers = responseMessage.Headers,
            ContentHeaders = responseMessage.Content?.Headers,
            Body = body
        };
    }

    /// <summary>
    /// Формирует HttpRequestMessage для запроса
    /// </summary>
    private HttpRequestMessage PrepareRequestMessage(HttpRequestData requestData)
    {
        var uri = AddQueryParametersInUri(requestData.Uri, requestData.QueryParameterList);

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = requestData.Method,
            RequestUri = uri // requestData.Uri
        };

        foreach (var header in requestData.HeaderDictionary)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        foreach (var traceWriter in _traceWriterList)
        {
            var value = traceWriter.GetValue();
            if (!string.IsNullOrWhiteSpace(value))
                httpRequestMessage.Headers.TryAddWithoutValidation(traceWriter.Name, value);
        }

        if (requestData.Body != null && HttpMethodAllowsBody(requestData.Method))
        {
            httpRequestMessage.Content = PrepareContent(requestData.Body, requestData.ContentType);
        }
        
        return httpRequestMessage;
    }

    /// <summary>
    /// Добавляет параметры запроса в Uri
    /// </summary>
    private static Uri AddQueryParametersInUri(Uri rawUri, ICollection<KeyValuePair<string, string>> queryParameters)
    {
        var uri = new StringBuilder();

        uri.Append(rawUri.ToString());
        uri.Append('?');
        foreach (var pair in queryParameters)
        {
            uri.Append(pair.Key);
            uri.Append('=');
            uri.Append(pair.Value);
            uri.Append('&');
        }
        uri.Remove(uri.Length-1, 1);

        return new Uri(uri.ToString());
    }

    /// <summary>
    /// Проверяет, разрешено ли HTTP-методу иметь тело запроса.
    /// </summary>
    private static bool HttpMethodAllowsBody(HttpMethod method) =>
        method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch || method == HttpMethod.Delete;

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