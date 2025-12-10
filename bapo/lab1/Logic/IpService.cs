using System.Text.Json;
using lab1.Dal;

namespace lab1.Logic;

public interface IIpService
{
    Task<string> GetCurrentIpAddress(string apiType);
}

public class IpService : IIpService
{
    private readonly HttpClient _client;
    private readonly IIpRepository _repository;

    public IpService(HttpClient client, IIpRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    public async Task<string> GetCurrentIpAddress(string apiType)
    {
        try
        {
            string endpointUrl;
            string fieldName;

            switch (apiType.ToLower())
            {
                case "ipapi":
                    endpointUrl = "http://ip-api.com/json/";
                    fieldName = "query";
                    break;
                case "jsonip":
                    endpointUrl = "https://jsonip.com/";
                    fieldName = "ip";
                    break;
                default:
                    throw new ArgumentException("Неверный тип API.");
            }

            var response = await _client.GetAsync(endpointUrl);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            var document = await JsonDocument.ParseAsync(stream);
            var rootElement = document.RootElement;
            var ipAddress = rootElement.GetProperty(fieldName).ToString();

            await SaveIpToDb(ipAddress);

            return $"{{\"myIP\": \"{ipAddress}\"}}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при получении IP-адреса: {ex.Message}");
        }
    }

    private async Task SaveIpToDb(string ipAddress)
    {
        var record = new IpDal
        {
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.Add(record);
    }
}