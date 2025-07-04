using Microsoft.EntityFrameworkCore; // ��� ������ � Entity Framework Core
using WebApi.Data;                 // ��� ������ AppDbContext
using WebApi.Intarface;           // ��� ����� ����������� ��������
// using WebApi.Services;          // ��������, ���� � ��� ���� �������, ������� ����� ���������������� �����.

var builder = WebApplication.CreateBuilder(args); // ������ ������ ��� ��������� ����������

// --- ���������� �������� � ��������� Dependency Injection ---

// 1. ��������� ��������� ������������ API
builder.Services.AddControllers();

// 2. ������������� � ��������� ��������� Swagger/OpenAPI
// Swagger ����� ������� ��� ���������������� ������ API � �������������� ������������.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. ��������� DbContext (����������� � ���� ������)
// ���������, ��� ����� ������������ SQL Server � ������ ����� ������ �����������.
// ���������, ��� "OrderManagementConnection" (��� ��� �� � �������) ������������ � appsettings.json
// In your WebApi project's Program.cs

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���� � ��� ���� ������ ������� (��������, �����������, ������-������), ��������������� �� �����.
// ������:
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// builder.Services.AddScoped<IOrderService, OrderService>();


// --- ���������� ���������� ---
var app = builder.Build(); // ������ ��������� ������ ���-����������

// --- ��������� HTTP-��������� �������� (Middleware) ---

// 1. � ������ ���������� ���������� Swagger UI
// ��� ��������� ��� ������������� � ����������� ��� API ����� ���-���������.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. ��������������� HTTP-�������� �� HTTPS
app.UseHttpsRedirection();

// 3. ��������� middleware ��� ����������� (���� ������ ������������)
// ���� �� ���������� ��������������/����������� (������, JWT � �.�.), ���� middleware ���������.
// ���������, ��� �� ��������� �������������� ����� ����, ��������, ����� AddAuthentication().AddJwtBearer().
app.UseAuthorization();

// 4. ���������� ����������� �� ��������
// ��� ������������ �������� HTTP-������� � ���������������� �������� � ����� ������������.
app.MapControllers();

// --- ������ ���������� ---
app.Run(); // ��������� ���� ASP.NET Core Web API