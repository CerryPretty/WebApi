using Microsoft.EntityFrameworkCore; // Для работы с Entity Framework Core
using WebApi.Data;                 // Для вашего AppDbContext
using WebApi.Intarface;           // Для ваших интерфейсов сервисов
// using WebApi.Services;          // Добавьте, если у вас есть сервисы, которые нужно зарегистрировать здесь.

var builder = WebApplication.CreateBuilder(args); // Создаём билдер для настройки приложения

// --- Добавление сервисов в контейнер Dependency Injection ---

// 1. Добавляем поддержку контроллеров API
builder.Services.AddControllers();

// 2. Конфигурируем и добавляем поддержку Swagger/OpenAPI
// Swagger очень полезен для документирования вашего API и интерактивного тестирования.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Добавляем DbContext (подключение к базе данных)
// Указываем, что будем использовать SQL Server и откуда взять строку подключения.
// Убедитесь, что "OrderManagementConnection" (или как вы её назвали) присутствует в appsettings.json
// In your WebApi project's Program.cs

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Если у вас есть другие сервисы (например, репозитории, бизнес-логика), зарегистрируйте их здесь.
// Пример:
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// builder.Services.AddScoped<IOrderService, OrderService>();


// --- Построение приложения ---
var app = builder.Build(); // Создаём экземпляр вашего веб-приложения

// --- Настройка HTTP-конвейера запросов (Middleware) ---

// 1. В режиме разработки используем Swagger UI
// Это позволяет вам просматривать и тестировать ваш API через веб-интерфейс.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Перенаправление HTTP-запросов на HTTPS
app.UseHttpsRedirection();

// 3. Добавляем middleware для авторизации (если будете использовать)
// Если вы планируете аутентификацию/авторизацию (токены, JWT и т.д.), этот middleware необходим.
// Убедитесь, что вы настроили аутентификацию перед этим, например, через AddAuthentication().AddJwtBearer().
app.UseAuthorization();

// 4. Отображаем контроллеры на маршруты
// Это сопоставляет входящие HTTP-запросы с соответствующими методами в ваших контроллерах.
app.MapControllers();

// --- Запуск приложения ---
app.Run(); // Запускаем ваше ASP.NET Core Web API