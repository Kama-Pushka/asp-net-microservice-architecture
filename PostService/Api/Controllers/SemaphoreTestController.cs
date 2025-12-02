using DistributedLock;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SemaphoreController : ControllerBase
{
    private readonly ZookeeperDistributedSemaphore _semaphore;

    public SemaphoreController(ZookeeperDistributedSemaphore semaphore)
    {
        _semaphore = semaphore;
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestAsync()
    {
        try
        {
            using (var handle = await _semaphore.AcquireAsync(TimeSpan.FromSeconds(5)))
            {
                if (handle == null)
                    return StatusCode(503, "Resource unavailable");
                
                await Task.Delay(2000);
                
                return Ok("Semaphore test completed successfully");
            }
        }
        catch (TimeoutException ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
}