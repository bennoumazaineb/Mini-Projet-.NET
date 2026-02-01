using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SecureAPI_JMT.Models;
using SecureAPI_JWT.Helpers;
using SecureAPI_JWT.Models;
using SecureAPI_JWT.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------- LOGGING ----------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// ---------------- DB CONTEXT ----------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------- IDENTITY ----------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ---------------- JWT CONFIG ----------------
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<JWT>(sp =>
    sp.GetRequiredService<IOptions<JWT>>().Value);

// ---------------- SERVICES ----------------
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// ---------------- CONTROLLERS ----------------
builder.Services.AddControllers();

// ---------------- CORS (🔥 OBLIGATOIRE) ----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy
            .WithOrigins("https://localhost:7052") // Frontend Blazor
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ---------------- SWAGGER ----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
            new List<string>()
        }
    });
});

// ---------------- AUTH JWT ----------------
var jwtConfig = builder.Configuration.GetSection("JWT").Get<JWT>();

if (jwtConfig != null && !string.IsNullOrEmpty(jwtConfig.Key))
{
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
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig.Key)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });
}

var app = builder.Build();

// ---------------- AUTO CREATE ROLES ----------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "ResponsableSAV", "Client" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// ---------------- MIDDLEWARE ORDER (🔥 CRITIQUE) ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorApp"); // 🔥 DOIT ÊTRE AVANT AUTH

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
