using CoreLib.Contract;
using Logic.Dtos;
using Logic.Interfaces;
using MassTransit;

namespace Logic.Consumers;

public class PostDeleteFailedConsumer : IConsumer<PostDeleteFailed>
{
    private readonly ILogger<DeleteUserConsumer> _logger;
    private readonly IUserManager _manager;

    public PostDeleteFailedConsumer(ILogger<DeleteUserConsumer> logger,  IUserManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<PostDeleteFailed> context)
    {
        try
        {
            _logger.LogInformation("Получено событие: PostDeleteFailed для пользователя {userId}", context.Message.UserInfo.UserId);
            var userInfo = context.Message.UserInfo;
            await _manager.AddUserAsync(new UserLogic
            {
                Id = userInfo.UserId, 
                Username = userInfo.Username, 
                Email = userInfo.Email
            });
            _logger.LogInformation("Пользователь {userId} успешно восстановлен", context.Message.UserInfo.UserId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}