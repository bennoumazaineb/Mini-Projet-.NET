using FluentValidation.AspNetCore;
using InterventionService.Data;
using InterventionService.Helpers;
using InterventionService.Middleware;
using InterventionService.Repositories;
using InterventionService.Repositories.Interfaces;
using InterventionService.Services;
using InterventionService.Services.Interfaces;
using InterventionService.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog pour le logging structuré
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/intervention-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateInterventionValidator>());

// Configuration de la base de données
builder.Services.AddDbContext<InterventionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.MigrationsAssembly("InterventionService")));

// Configuration de l'authentification JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Configuration de l'autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResponsableSAVOnly", policy =>
        policy.RequireRole("ResponsableSAV"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("TechnicienOrAbove", policy =>
        policy.RequireRole("Technicien", "ResponsableSAV", "Admin"));
});

// Enregistrement des services
builder.Services.AddScoped<IInterventionRepository, InterventionRepository>();
builder.Services.AddScoped<IInterventionService, InterventionService.Services.InterventionService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuration Swagger avec support JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Intervention Service API",
        Version = "v1",
        Description = "Microservice pour la gestion des interventions techniques SAV",
        Contact = new OpenApiContact
        {
            Name = "Équipe SAV",
            Email = "sav@entreprise.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 🔥 LOG LES INFORMATIONS DE DÉMARRAGE
var urls = app.Urls;
Log.Information("=== DÉMARRAGE INTERVENTION SERVICE ===");
Log.Information("URLs d'écoute: {Urls}", urls);
Log.Information("Environnement: {Environment}", app.Environment.EnvironmentName);

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    Log.Information("Mode développement activé");

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intervention Service API v1");
        c.RoutePrefix = "swagger"; // Swagger accessible à /swagger
        Log.Information("Swagger UI disponible à: /swagger");
    });

    // Appliquer les migrations et seeds en développement
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<InterventionDbContext>();
        try
        {
            dbContext.Database.Migrate();
            SeedData.Initialize(scope.ServiceProvider);
            Log.Information("Base de données migrée et initialisée");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erreur lors de la migration de la base de données");
        }
    }
}

app.UseRouting();

// 🔥 AFFICHER LES ROUTES DISPONIBLES AU DÉMARRAGE
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // Log toutes les routes découvertes
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var endpointSources = endpoints.DataSources;

    foreach (var source in endpointSources)
    {
        foreach (var endpoint in source.Endpoints)
        {
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                var method = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.FirstOrDefault() ?? "ANY";
                var pattern = routeEndpoint.RoutePattern.RawText ?? "N/A";
                logger.LogInformation("Route enregistrée: {Method} {Pattern}", method, pattern);
            }
        }
    }
});

// Page d'accueil avec liste des endpoints
app.MapGet("/", () =>
{
    var endpoints = app.Services.GetServices<EndpointDataSource>();
    var sb = new System.Text.StringBuilder();

    sb.AppendLine("<!DOCTYPE html>");
    sb.AppendLine("<html>");
    sb.AppendLine("<head>");
    sb.AppendLine("    <title>Intervention Service API</title>");
    sb.AppendLine("    <style>");
    sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 40px; }");
    sb.AppendLine("        h1 { color: #333; }");
    sb.AppendLine("        ul { list-style-type: none; padding: 0; }");
    sb.AppendLine("        li { padding: 8px 0; border-bottom: 1px solid #eee; }");
    sb.AppendLine("        .method { display: inline-block; width: 80px; font-weight: bold; }");
    sb.AppendLine("        .get { color: green; }");
    sb.AppendLine("        .post { color: orange; }");
    sb.AppendLine("        .put { color: blue; }");
    sb.AppendLine("        .delete { color: red; }");
    sb.AppendLine("    </style>");
    sb.AppendLine("</head>");
    sb.AppendLine("<body>");
    sb.AppendLine("    <h1>🔧 Intervention Service API</h1>");
    sb.AppendLine("    <p>Microservice de gestion des interventions techniques SAV</p>");
    sb.AppendLine("    <h2>📋 Endpoints disponibles:</h2>");
    sb.AppendLine("    <ul>");

    foreach (var endpointSource in endpoints)
    {
        foreach (var endpoint in endpointSource.Endpoints)
        {
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                var method = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.FirstOrDefault() ?? "GET";
                var pattern = routeEndpoint.RoutePattern.RawText ?? "";
                var methodClass = method.ToLower();

                sb.AppendLine($"        <li>");
                sb.AppendLine($"            <span class='method {methodClass}'>{method}</span>");
                sb.AppendLine($"            <code>{pattern}</code>");
                sb.AppendLine($"        </li>");
            }
        }
    }

    sb.AppendLine("    </ul>");
    sb.AppendLine("    <h2>🔗 Liens utiles:</h2>");
    sb.AppendLine("    <ul>");
    sb.AppendLine("        <li><a href='/swagger'>📚 Swagger UI (Documentation API)</a></li>");
    sb.AppendLine("        <li><a href='/health'>❤️ Health Check</a></li>");
    sb.AppendLine("    </ul>");
    sb.AppendLine("    <p><em>Service démarré le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "</em></p>");
    sb.AppendLine("</body>");
    sb.AppendLine("</html>");

    return Results.Content(sb.ToString(), "text/html");
}).WithName("Home").WithDisplayName("Page d'accueil");

app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint amélioré
app.MapGet("/health", () =>
{
    var healthInfo = new
    {
        Status = "Healthy",
        Service = "Intervention Service",
        Timestamp = DateTime.UtcNow,
        Environment = app.Environment.EnvironmentName,
        Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
    };

    Log.Information("Health check appelé: {Status}", healthInfo.Status);
    return Results.Ok(healthInfo);
}).WithName("HealthCheck").WithDisplayName("Health Check");

Log.Information("=== INTERVENTION SERVICE PRÊT ===");
Log.Information("URLs d'accès:");
Log.Information("  - Page d'accueil: http://localhost:5262/");
Log.Information("  - Swagger UI: http://localhost:5262/swagger");
Log.Information("  - Health Check: http://localhost:5262/health");

app.Run();