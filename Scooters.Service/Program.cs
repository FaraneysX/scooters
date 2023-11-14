﻿using Scooters.Service.IoC;
using Scooters.Service.Settings;

// Создание объекта конфигурации на основе файла appsettings.json.
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var settings = ScootersSettingsReader.Read(configuration);
var builder = WebApplication.CreateBuilder();

// Регистрация контроллеров для обработки HTTP-запросов.
builder.Services.AddControllers();

SerilogConfigurator.ConfigureService(builder);
SwaggerConfigurator.ConfigureServices(builder.Services);
DbContextConfigurator.ConfigureService(builder.Services, settings);

// Построение объекта приложения.
var app = builder.Build();

SerilogConfigurator.ConfigureApplication(app);
SwaggerConfigurator.ConfigureApplication(app);
DbContextConfigurator.ConfigureApplication(app);

// Включение промежуточных слоев для перенаправления HTTP-запросов
// с HTTPS, аутентификации и обработки контроллеров.
app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();

app.Run();