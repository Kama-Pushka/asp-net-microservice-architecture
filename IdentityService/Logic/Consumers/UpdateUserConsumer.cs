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
        try // TODO а как откатывать транзакцию, если упал этот сервис?
        {
            var userLogic = new UserLogic { Id = context.Message.UserId, Username = context.Message.NewUsername, Email = "test"}; // TODO а где имейл
            await _manager.UpdateUserAsync(userLogic);
            await context.Publish(new UserUpdated(context.Message.UserId, context.Message.NewUsername));
            
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