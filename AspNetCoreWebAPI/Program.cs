using AspNetCoreWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

//// 1. Add services to the container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� ���� ����������� �������������
// �������, ������ ������������� � ������ � ��� ����������� ��� ����������
// ���������� ���� ������������ � ������� "a" � ������� "a" (����������)
// �����, ��� �������� �� � ������� AddSingleton, � �� AddTransient ��� AddScoped
// (���� �� �� � ������ ������ ��������� � �� ���������,
// ��� ��� �� ���������� ������ � ����� ����������� � ������������,
// � �� �� ������ ������)
builder.Services.AddSingleton<UserRepository>();

// ���������������� �������������� - �������������� � ������� ��� (��������������� ASP.NET Core)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // ��� ������ "Cookies"
    .AddCookie(options =>
    {
        // ��� ���� ��������� �� ����� ��������, ���� ����� �������� � ������ �����
        options.Cookie.SameSite = SameSiteMode.None;
        // ���� ����� HttpOnly - �� ������ ����� �������� ����� JavaScript � ��������
        // ��� ���� ����� ���������
        options.Cookie.HttpOnly = true;
        // ��� �� ���
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // ���������� �������� �������� �� /Account/Login (����������� �������� � ���)
        // ��� ���� ����� ��� ��������� �������������� ��� ����� ����������� �� /Account/Login,
        // � ��� ��� ����������� �� ����� ���� ���, �� ���������� �� ����� ��� 404 not found.
        // � � ���� ������ ��� ��������� �������������� ������ ���������� ��� 401 Not Authorized
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// ����� �������� ��� �������� � �������� - CORS
// CORS �����, ��� ��� ����� ����� ��������� �������� �� ����
builder.Services.AddCors(conf =>
{
    conf.AddPolicy("forFrontend", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
        // ��������� �������� ������� ������ �� ���������
        string whereIsFrontend = "http://localhost:3000";
        policy.WithOrigins(whereIsFrontend);
        // ��� �� ����� ��������� �������� ������� �������� (�� �������� ��� �����������)
        //policy.SetIsOriginAllowed(origin => true);
    });
});

//// 2. Build the WebApplication
var app = builder.Build();

//// 3. Configure the HTTP request pipeline.

// ������������� ������������������ ����� �������� CORS
app.UseCors("forFrontend");

// Swagger - ��� ������� ���������� ����������, ��� ���� � API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ��������������
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
