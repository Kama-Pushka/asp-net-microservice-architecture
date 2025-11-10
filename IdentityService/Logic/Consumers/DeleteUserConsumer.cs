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
        try // TODO а как откатывать транзакцию, если упал этот сервис?
        {
            var userId = context.Message.UserId;
            // await _manager.DeleteUserAsync(userId);
            await context.Publish(new UserDeleted(userId));
            
            // _logger.LogInformation("Получена команда на удаление ачивок для пользователя {UserId}", userId);
            //
            // // --- Здесь была бы логика удаления из БД ---
            // _logger.LogInformation("Ачивки для пользователя {UserId} успешно удалены", userId);

            // Сообщаем оркестратору, что этот шаг выполнен
        }
        catch(Exception ex)
        {
        }
    }
}