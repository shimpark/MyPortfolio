using System.Data;
using Npgsql;
using MediatR;
using MyPortfolio.Infrastructure;
using MyPortfolio.Infrastructure.Repositories;
using MyPortfolio.Infrastructure.Services;
using MyPortfolio.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// ✅ Render가 주입하는 PORT 환경변수 읽기 (없으면 8080)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// DB Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// SQL Cache Service (Singleton)
builder.Services.AddSingleton<ISqlCacheService, SqlCacheService>();

// MediatR 등록
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosHandler).Assembly));

// Scrutor DI Setup
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(TodoRepository).Assembly, typeof(GetTodosHandler).Assembly)
    // Repositories -> Scoped
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    // Handlers -> Scoped
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Handler")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();