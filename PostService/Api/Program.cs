using CoreLib.HttpLogic;
using CoreLib.TraceIdLogic;
using Infrastructured;
using Serilog;
using Services;

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