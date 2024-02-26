using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Service;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// builder.Services.AddHostedService<DiscordBotService>();
builder.Services.AddSingleton<SSEManagerContext>();
builder.Services.AddDbContext<WebAPIDbContext>(options => options.UseSqlite( builder.Configuration.GetConnectionString("SQLite") ));
builder.Services.AddControllers();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/";
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
