using CoreLib.HttpLogic;
using CoreLib.TraceIdLogic;
using Infrastructured;
using MassTransit;
using Serilog;
using Services;
using Services.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации для DAL и Logic
builder.Services.AddInfrastructuredServices();
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
    // По умолчанию MassTransit использует Pascal case.
    // Так как в большинстве случае в эндпоинтах будет использоваться Kebab case,
    // укажем на это явно.
    x.SetKebabCaseEndpointNameFormatter();
    
    x.AddConsumer<UserDeletedConsumer>();
    x.AddConsumer<UpdatePostCommandConsumer>(); // todo ??

    // Меняем InMemory на RabbitMq и указываем данные для подключение к серверу.
    // В остальном подключение похоже на InMemory, так реализация его в MassTransit
    // намеренно реплицирует поведение именно RabbitMq
    x.UsingRabbitMq((context, cfg) =>
    {
        // Здесь указаны стандартные настройки для подключения к RabbitMq
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("update-post", e =>
        {
            e.ConfigureConsumer<UpdatePostCommandConsumer>(context);
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