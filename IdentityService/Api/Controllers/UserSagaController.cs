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
    
    // public async Task RestoreUserAsync(Guid userId)
    // {
    //     // Восстанавливаем пользователя
    //     await _bus.Publish<IRestoreUserCommand>(new { UserId = userId });
    // }
    
    [HttpPut]
    public async Task UpdateUserAsync(Guid userId, string newUsername) // TODO dto
    {
        // Обновляем имя пользователя
        await _bus.Publish(new UserUpdateRequested(userId, newUsername));
    }
    
    // public async Task RevertUserUpdateAsync(Guid userId, string oldUsername)
    // {
    //     // Откатываем обновление имени пользователя
    //     await _bus.Publish<IRevertUserUpdateCommand>(new { UserId = userId, OldUsername = oldUsername });
    // }
}