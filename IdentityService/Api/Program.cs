using CoreLib.HttpLogic;
using CoreLib.TraceIdLogic;
using Logic;
using Logic.Consumers;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации для DAL и Logic
builder.Services.AddDalServices();
builder.Services.AddLogicServices();
builder.Services.AddHttpRequestService();
builder.Services.AddTraceId();

// Регистрация контроллеров
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Properties}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    // State Machine + хранилище состояний в памяти
    x.AddSagaStateMachine<UserUpdateSaga, UserUpdateSagaState>()
        .InMemoryRepository();
    
    x.AddConsumer<DeleteUserConsumer>();
    x.AddConsumer<PostDeleteFailedConsumer>();

    x.AddConsumer<UpdateUserConsumer>();
    x.AddConsumer<RevertUserUpdateConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        //Здесь указаны стандартные настройки для подключения к RabbitMq
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("update-user", e =>
        {
            e.ConfigureConsumer<UpdateUserConsumer>(context);
        });
        cfg.ReceiveEndpoint("restore-user", e =>
        {
            e.ConfigureConsumer<RevertUserUpdateConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseTraceId();

app.Run();