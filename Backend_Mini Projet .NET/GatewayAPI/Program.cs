using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Charger ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// CORS pour Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7052")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowBlazorApp");

// Log simple (debug)
app.Use(async (ctx, next) =>
{
    Console.WriteLine($"[GATEWAY] {ctx.Request.Method} {ctx.Request.Path}");
    await next();
});

// ⚠️ Ocelot DOIT être le DERNIER
await app.UseOcelot();

app.Run();
