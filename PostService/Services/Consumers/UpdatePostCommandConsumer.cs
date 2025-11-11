using CoreLib.Contract;
using MassTransit;
using Services.Interfaces;

namespace Services.Consumers;

public class UpdatePostCommandConsumer: IConsumer<UpdatePostCommand>
{
    private readonly ILogger<UpdatePostCommandConsumer> _logger;
    private readonly IPostManager _manager;

    public UpdatePostCommandConsumer(ILogger<UpdatePostCommandConsumer> logger,  IPostManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<UpdatePostCommand> context)
    {
        try
        {
            _logger.LogInformation("Получена команда: UpdatePostCommand для пользователя {userId}", context.Message.UserId);
            
            //throw new Exception("check exception");
            
            var userId = context.Message.UserId;
            var userName = context.Message.NewUsername;
            await _manager.UpdatePostsByUserId(userId, userName);
            _logger.LogInformation("Все посты для пользователя {userId} успешно обновлены", userId);

            await context.Publish(new PostUpdated(userId));
        }
        catch(Exception ex)
        {
            var userId = context.Message.UserId;
            await context.Publish(new UpdatePostFailed(userId));
        }
    }
}