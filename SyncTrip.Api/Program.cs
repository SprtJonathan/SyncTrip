using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using SyncTrip.Api.API.Hubs;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Data;
using SyncTrip.Api.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== SERILOG CONFIGURATION =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ===== DATABASE CONFIGURATION =====
var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        Log.Information("Utilisation de SQLite pour le développement/tests");
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        Log.Information("Utilisation de PostgreSQL");
    }
});

// ===== REDIS CONFIGURATION =====
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = ConfigurationOptions.Parse(redisConnectionString);
        return ConnectionMultiplexer.Connect(configuration);
    });
}
else
{
    Log.Warning("Redis connection string not configured. Caching will be disabled.");
}

// ===== JWT AUTHENTICATION =====
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var issuer = jwtSettings["Issuer"] ?? "SyncTripApi";
var audience = jwtSettings["Audience"] ?? "SyncTripApp";

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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    // Configuration pour SignalR avec JWT
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // Si la requête est pour un hub SignalR
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ===== SIGNALR CONFIGURATION =====
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 102400; // 100 KB
});

// ===== CORS CONFIGURATION =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // En développement : autoriser tout avec credentials (pour SignalR)
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // En production : spécifier les origines exactes
            policy.WithOrigins("https://yourdomain.com", "https://app.yourdomain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// ===== AUTOMAPPER =====
builder.Services.AddAutoMapper(typeof(Program));

// ===== DEPENDENCY INJECTION =====
// Unit of Work & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IConvoyCodeGenerator, ConvoyCodeGenerator>();
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConvoyService, ConvoyService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ILocationService, LocationService>();

// ===== CONTROLLERS =====
builder.Services.AddControllers();

// ===== SWAGGER / OPENAPI =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== BUILD APPLICATION =====
var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SyncTrip API v1");
        options.RoutePrefix = string.Empty; // Swagger à la racine
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowMobileApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hubs
app.MapHub<LocationHub>("/hubs/location");
app.MapHub<ChatHub>("/hubs/chat");

// ===== DATABASE INITIALIZATION =====
// Créer la base de données automatiquement en mode développement (SQLite)
if (app.Environment.IsDevelopment() && useSqlite)
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            Log.Information("Base de données SQLite créée/vérifiée avec succès");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erreur lors de la création de la base de données SQLite");
        }
    }
}

// ===== RUN APPLICATION =====
try
{
    Log.Information("Démarrage de SyncTrip API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application a échoué au démarrage");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
