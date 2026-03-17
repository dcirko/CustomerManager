using System.Diagnostics;
using CustomerManager.Controllers;
using CustomerManager.Data;
using CustomerManager.Middleware;
using CustomerManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomerManagerDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("CustomerManager")));

builder.Services.AddControllers();
builder.Services.AddValidation();
builder.Services.AddScoped<CustomerManagerService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.Endpoints();

var stopwatch = Stopwatch.StartNew();

await app.MigrateDb();

stopwatch.Stop();
Console.WriteLine($"Seeding took: {stopwatch.ElapsedMilliseconds} ms");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAngular");
app.MapControllers();

app.Run();


