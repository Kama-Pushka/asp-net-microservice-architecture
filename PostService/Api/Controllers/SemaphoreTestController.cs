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

    // блокировка получена - блокировка снята
    [HttpGet("one-acquire-one-lock")]
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
    
    // спс ImAbobaBoy за тест
    // 2 таски получат блокировку, 5 не получат
    [HttpGet("many-acquire-one-lock")]
    public async Task<IActionResult> Test2Async()
    {
        var tasks = Enumerable.Range(1, 7).Select(async i =>
        {
            await using (var handle = await _semaphore.TryAcquireAsync()) // TODO отлавливать случаи, когда отвалился сервис, который захватил блокировку
            {
                if (handle != null)
                {
                    Console.WriteLine($"Таска {i} получила блокировку");
                    await Task.Delay(2000);
                }
                else
                {
                    Console.WriteLine($"Таска {i} не смогла получить блокировку");
                }
            }
        });

        await Task.WhenAll(tasks);

        return Ok("Semaphore test completed successfully");
    }
}