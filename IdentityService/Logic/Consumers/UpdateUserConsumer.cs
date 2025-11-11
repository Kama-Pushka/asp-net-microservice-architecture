using CoreLib.Contract;
using Logic.Dtos;
using Logic.Interfaces;
using MassTransit;

namespace Logic.Consumers;

public class UpdateUserConsumer : IConsumer<UpdateUserCommand>
{
    private readonly ILogger<UpdateUserConsumer> _logger;
    private readonly IUserManager _manager;

    public UpdateUserConsumer(ILogger<UpdateUserConsumer> logger,  IUserManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<UpdateUserCommand> context)
    {
        try
        {
            var userId = context.Message.UserId;
            _logger.LogInformation("Получена команда: UpdateUserCommand для пользователя {userId}", userId);
            
            //throw new Exception("check exception");
            
            var user = await _manager.GetUserByIdAsync(userId);
            var newUsername = context.Message.NewUsername;
            var oldUsername = user.Username;
            
            var userLogic = new UserLogic { Id = userId, Username = newUsername };
            await _manager.UpdateUserAsync(userLogic);
            _logger.LogInformation("Информация о пользователе {UserId} успешно обновлена", userId);
            await context.Publish(new UserUpdated(userId, oldUsername));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await context.Publish(new UpdateUserFailed(context.Message.UserId));
        }
    }
}