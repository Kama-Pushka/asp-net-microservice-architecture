using CoreLib.Contract;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/saga/user")]
public class UserSagaController : ControllerBase
{
    private readonly IBus _bus;

    public UserSagaController(IBus bus)
    {
        _bus = bus;
    }

    [HttpDelete("{id}")]
    public async Task DeleteUserAsync(Guid id) // TODO dto
    {
        await _bus.Publish(new DeleteUserCommand(id));
    }
    
    [HttpPut]
    public async Task UpdateUserAsync(Guid userId, string newUsername) // TODO dto
    {
        // Обновляем имя пользователя
        await _bus.Publish(new UserUpdateRequested(userId, newUsername));
    }
}