using CoreLib.Contract;
using Logic.Dtos;
using Logic.Interfaces;
using MassTransit;

namespace Logic.Consumers;

public class RevertUserUpdateConsumer : IConsumer<RevertUserUpdateCommand>
{
    private readonly ILogger<DeleteUserConsumer> _logger;
    private readonly IUserManager _manager;

    public RevertUserUpdateConsumer(ILogger<DeleteUserConsumer> logger,  IUserManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<RevertUserUpdateCommand> context)
    {
        try
        {
            _logger.LogInformation("Получена команда: RevertUserUpdateCommand для пользователя {userId}", context.Message.UserId);
            var userLogic = new UserLogic { Id = context.Message.UserId, Username = context.Message.OldUsername };
            await _manager.UpdateUserAsync(userLogic);
            _logger.LogInformation("Откат изменений информации {userId} успешно завершен", context.Message.UserId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}