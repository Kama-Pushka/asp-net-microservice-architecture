using System.Net;
using CoreLib.HttpLogic.Entities;
using CoreLib.HttpLogic.Services.Interfaces;
using IdentityConnectionLib.ConnectionServices.Dtos;
using IdentityConnectionLib.ConnectionServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityConnectionLib.ConnectionServices;

public class IdentityConnectionService : IIdentityConnectionService
{
    private readonly string _identityServiceUrl = "http://localhost:5156";
    private readonly IHttpRequestService _httpRequestService;

    public IdentityConnectionService(IServiceProvider serviceProvider) // TODO IConfiguration configuration
    {
        _httpRequestService = serviceProvider.GetRequiredService<IHttpRequestService>();
    }

    public async Task<CheckUserExistsIdentityServiceResponse> CheckUserExistAsync(CheckUserExistsIdentityServiceRequest user)
    {
        var request = new HttpRequestData
        {
            Uri = new Uri($"{_identityServiceUrl}/api/User/{user.UserId}"),
            Method = HttpMethod.Get,
            ContentType = ContentType.ApplicationJson
        };

        try
        {
            var response = await _httpRequestService.SendRequestAsync<UserResponse>(request);
            return new CheckUserExistsIdentityServiceResponse(response.IsSuccessStatusCode && response.Body is not null);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new CheckUserExistsIdentityServiceResponse(false);
        }
    }
}

public record UserResponse // TODO а это не дублирование из IdentityService??
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}