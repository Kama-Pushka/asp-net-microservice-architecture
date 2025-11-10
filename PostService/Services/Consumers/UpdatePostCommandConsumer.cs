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
        try // TODO а как откатывать транзакцию, если упал этот сервис?
        {
            var userId = context.Message.UserId;
            await _manager.UpdatePostsByUserId(userId);
            await context.Publish(new PostUpdated(userId)); // TODO а должен ли исходный контроллер дождаться ответа от последнего контроллера?
            
            // _logger.LogInformation("Получена команда на удаление ачивок для пользователя {UserId}", userId);
            //
            // // --- Здесь была бы логика удаления из БД ---
            // _logger.LogInformation("Ачивки для пользователя {UserId} успешно удалены", userId);

            // Сообщаем оркестратору, что этот шаг выполнен
        }
        catch(Exception ex)
        {
            var userId = context.Message.UserId;
            await context.Publish(new UpdatePostFailed(userId)); // TODO а должен ли исходный контроллер дождаться ответа от последнего контроллера?
            // todo
        }
    }
}