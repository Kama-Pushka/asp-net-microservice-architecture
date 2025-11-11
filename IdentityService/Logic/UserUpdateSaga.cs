using CoreLib.Contract;
using MassTransit;

namespace Logic;

public class UserUpdateSaga : MassTransitStateMachine<UserUpdateSagaState>
{
    public State Processing { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<UserUpdateRequested> UpdateUserCommand { get; private set; }
    public Event<UserUpdated> UserUpdated { get; private set; }
    public Event<UpdateUserFailed> UpdateUserFailed { get; private set; }
    public Event<PostUpdated> PostUpdated { get; private set; }
    public Event<UpdatePostFailed> UpdatePostFailed { get; private set; }
    
    public UserUpdateSaga()
    {
        InstanceState(x => x.CurrentState);
        Event(() => UpdateUserCommand, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => UserUpdated, x => x.CorrelateById(context => context.Message.UserId)); // todo
        Event(() => UpdateUserFailed, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => PostUpdated, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => UpdatePostFailed, x => x.CorrelateById(context => context.Message.UserId));
        
        Initially(
            When(UpdateUserCommand)
                .Then(context =>
                {
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.NewUsername = context.Message.NewUsername;
                })
                .TransitionTo(Processing)
                .Send(new Uri("queue:update-user"),context => new UpdateUserCommand(context.Saga.UserId, context.Saga.NewUsername))
        );

        During(Processing,
            When(UserUpdated)
                .Then(context =>
                {
                    context.Saga.OldUsername = context.Message.OldUsername;
                })
                .Send(new Uri("queue:update-post"),context => new UpdatePostCommand(context.Saga.UserId, context.Saga.NewUsername)),
            
            When(UpdateUserFailed)
                .Then(_ =>
                {
                    Console.WriteLine("Получено событие: UserUpdateFailed");
                })
                .TransitionTo(Failed)
                .Finalize(),
            
            When(PostUpdated)
                .TransitionTo(Completed)
                .Finalize(), // TODO вынести окончание саги в During(Completed)?
            
            When(UpdatePostFailed)
                .Then(_ =>
                {
                    Console.WriteLine("Получено событие: UpdatePostFailed");
                })
                .Send(new Uri("queue:restore-user"),context => new RevertUserUpdateCommand(context.Saga.UserId, context.Saga.OldUsername))
                .TransitionTo(Failed)
                .Finalize()
        );
    }
}

public class UserUpdateSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid UserId { get; set; }
    public string NewUsername { get; set; }
    public string OldUsername { get; set; }
}