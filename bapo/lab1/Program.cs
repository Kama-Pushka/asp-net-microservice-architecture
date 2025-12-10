using lab1.Dal;
using lab1.Logic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации для DAL и Logic
builder.Services.AddScoped<IIpRepository, IpRepository>();
builder.Services.AddScoped<IIpService, IpService>();

var databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER")?.Trim().ToLowerInvariant() ?? "sqlite";
if (databaseProvider == "postgres")
{
    builder.Services.AddDbContext<IpContext>(options =>
        options.UseNpgsql(builder.Configuration["CONNECTION_STRING"])
    );
    Console.WriteLine(builder.Configuration["CONNECTION_STRING"]);
}
else
{
    builder.Services.AddDbContext<IpContext>(options =>
        options.UseSqlite($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "..", "db", "IpService.db")}")
    );
}

// Регистрация контроллеров
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // ?

app.UseAuthorization(); // ??

app.MapControllers();

app.Run();