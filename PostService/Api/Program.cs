using Infrastructured;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации для DAL и Logic
builder.Services.AddInfrastructuredServices();
builder.Services.AddLogicServices();

// Регистрация контроллеров
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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

app.Run();