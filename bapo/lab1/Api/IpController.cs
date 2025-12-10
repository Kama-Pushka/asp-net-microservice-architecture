using lab1.Logic;
using Microsoft.AspNetCore.Mvc;

namespace lab1.Api;

[Route("api/[controller]")]
[ApiController]
public class IpController : ControllerBase
{
    private readonly IIpService _service;

    public IpController(IIpService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyIp()
    {
        var type = Environment.GetEnvironmentVariable("TYPE") ?? "ipapi";
        
        try
        {
            var myIp = await _service.GetCurrentIpAddress(type);
            return Ok(myIp);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}