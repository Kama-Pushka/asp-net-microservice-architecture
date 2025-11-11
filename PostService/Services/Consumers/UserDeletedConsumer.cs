using CoreLib.Contract;
using MassTransit;
using Services.Interfaces;

namespace Services.Consumers;

public class UserDeletedConsumer: IConsumer<UserDeleted>
{
    private readonly ILogger<UserDeletedConsumer> _logger;
    private readonly IPostManager _manager;

    public UserDeletedConsumer(ILogger<UserDeletedConsumer> logger,  IPostManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        try
        {
            _logger.LogInformation("Получено событие: UserDeleted для пользователя {userId}", context.Message.UserId);
            
            //throw new Exception("check exception"); // для проверки exception кейсов
            
            var userId = context.Message.UserId;
            await _manager.DeletePostsByUserId(userId);
            _logger.LogInformation("Все посты для пользователя {userId} успешно удалены", userId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await context.Publish(new PostDeleteFailed(context.Message)); // TODO через заголовки из rabbitmq доставать сервисы, где транзакции завершились и откатывать
        }
    }
}