using CoreLib.Contract;
using Logic.Interfaces;
using MassTransit;

namespace Logic.Consumers;

public class DeleteUserConsumer: IConsumer<DeleteUserCommand>
{
    private readonly ILogger<DeleteUserConsumer> _logger;
    private readonly IUserManager _manager;

    public DeleteUserConsumer(ILogger<DeleteUserConsumer> logger,  IUserManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<DeleteUserCommand> context)
    {
        try
        {
            var userId = context.Message.UserId;
            _logger.LogInformation("Получена команда: DeleteUserCommand для пользователя {userId}", userId);
            
            //throw new Exception("check exception");
            
            var user = _manager.GetUserByIdAsync(userId);
            
            await _manager.DeleteUserAsync(userId);
            _logger.LogInformation("Пользователь {UserId} успешно удален", userId);
            await context.Publish(new UserDeleted(user.Result.Id, user.Result.Username, user.Result.Email));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}