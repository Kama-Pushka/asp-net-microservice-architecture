using CoreLib.Contract;
using Logic.Interfaces;
using MassTransit;

namespace Logic.Consumers;

public class RestoreUserConsumer : IConsumer<RestoreUserCommand> // TODO
{
    private readonly ILogger<DeleteUserConsumer> _logger;
    private readonly IUserManager _manager;

    public RestoreUserConsumer(ILogger<DeleteUserConsumer> logger,  IUserManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<RestoreUserCommand> context)
    {
        try // TODO а как откатывать транзакцию, если упал этот сервис?
        {
            // TODO

        }
        catch(Exception ex)
        {
        }
    }
}