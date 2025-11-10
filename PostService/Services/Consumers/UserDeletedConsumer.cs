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
        try // TODO а как откатывать транзакцию, если упал этот сервис?
        {
            var userId = context.Message.UserId;
            await _manager.DeletePostsByUserId(userId);
            await context.Publish<PostDeleted>(new { UserId = userId }); // TODO а должен ли исходный контроллер дождаться ответа от последнего контроллера?
            
            // _logger.LogInformation("Получена команда на удаление ачивок для пользователя {UserId}", userId);
            //
            // // --- Здесь была бы логика удаления из БД ---
            // _logger.LogInformation("Ачивки для пользователя {UserId} успешно удалены", userId);

            // Сообщаем оркестратору, что этот шаг выполнен
        }
        catch(Exception ex)
        {
            // todo
        }
    }
}