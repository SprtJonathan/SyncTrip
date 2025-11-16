# Guide d'Implémentation SyncTrip

Ce document fournit un guide étape par étape pour implémenter l'application SyncTrip avec des exemples de code concrets utilisant **.NET 10**.

## Table des matières

1. [Configuration de l'environnement](#configuration-de-lenvironnement)
2. [Structure du projet MAUI](#structure-du-projet-maui)
3. [Implémentation Backend](#implémentation-backend)
4. [Implémentation Frontend](#implémentation-frontend)
5. [Intégration SignalR](#intégration-signalr)
6. [Gestion GPS et Cartes](#gestion-gps-et-cartes)
7. [Authentification Passwordless](#authentification-passwordless)
8. [Tests](#tests)

---

## Configuration de l'environnement

### Étape 1 : Installer les outils

```bash
# Vérifier .NET SDK (.NET 10 requis)
dotnet --version  # Doit être 10.0+

# Installer workload MAUI pour .NET 10
dotnet workload install maui

# Vérifier les workloads installés
dotnet workload list
```

### Étape 2 : Créer docker-compose.yml

Créer `docker-compose.yml` à la racine du projet :

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16
    container_name: synctrip-postgres
    environment:
      POSTGRES_DB: synctrip
      POSTGRES_USER: synctrip_user
      POSTGRES_PASSWORD: dev_password_123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U synctrip_user"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: synctrip-redis
    ports:
      - "6379:6379"
    command: redis-server --appendonly yes
    volumes:
      - redis_data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  pgadmin:
    image: dpage/pgadmin4
    container_name: synctrip-pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@synctrip.local
      PGADMIN_DEFAULT_PASSWORD: admin123
    ports:
      - "5050:80"
    depends_on:
      - postgres

volumes:
  postgres_data:
  redis_data:
```

Lancer :
```bash
docker-compose up -d
```

---

## Structure du projet MAUI

### Étape 1 : Créer la structure de dossiers

```bash
# Depuis le répertoire SyncTrip/
mkdir -p Core/Entities Core/Enums Core/Interfaces Core/Constants
mkdir -p Services/Api Services/SignalR Services/Location Services/Database Services/Authentication Services/Cache
mkdir -p ViewModels/Base ViewModels/Auth ViewModels/Convoy ViewModels/Trip ViewModels/Map ViewModels/History
mkdir -p Views/Auth Views/Convoy Views/Trip Views/Map Views/History
mkdir -p Models/DTOs Models/Requests Models/Responses
mkdir -p Helpers Controls Converters Behaviors
```

### Étape 2 : Installer les packages NuGet

Modifier `SyncTrip.csproj` :

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net10.0-android;net10.0-ios</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>SyncTrip</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <!-- Android -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>

    <!-- iOS -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">14.2</SupportedOSPlatformVersion>
  </PropertyGroup>

  <ItemGroup>
    <!-- MVVM -->
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.0" />

    <!-- SignalR Client -->
    <PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.0" />

    <!-- Cartes OpenStreetMap -->
    <PackageReference Include="Mapsui.Maui" Version="5.0.0" />

    <!-- SQLite local -->
    <PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
    <PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.10" />

    <!-- HTTP et JSON -->
    <PackageReference Include="Microsoft.Extensions.Http" Version="10.0.0" />
    <PackageReference Include="System.Text.Json" Version="10.0.0" />

    <!-- Logging -->
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
  </ItemGroup>

</Project>
```

Puis :
```bash
dotnet restore
```

---

## Implémentation Backend

### Étape 1 : Créer le projet API

```bash
# Depuis la racine du repository
dotnet new webapi -n SyncTrip.Api --framework net10.0
cd SyncTrip.Api
```

### Étape 2 : Installer les packages NuGet Backend

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
dotnet add package Microsoft.AspNetCore.SignalR --version 1.1.0
dotnet add package StackExchange.Redis --version 2.8.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.0
dotnet add package AutoMapper --version 13.0.1
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add package Swashbuckle.AspNetCore --version 7.0.0
dotnet add package Serilog.AspNetCore --version 8.0.0
```

### Étape 3 : Créer les enums

`Core/Enums/Enums.cs` :

```csharp
namespace SyncTrip.Api.Core.Enums;

public enum ConvoyStatus
{
    Active = 0,
    Archived = 1
}

public enum TripStatus
{
    Planned = 0,
    Active = 1,
    Paused = 2,
    Finished = 3,
    Cancelled = 4
}

public enum ParticipantRole
{
    Member = 0,
    Leader = 1
}

[Flags]
public enum ParticipantPermissions
{
    None = 0,
    CanAddWaypoints = 1 << 0,      // 1
    CanSendMessages = 1 << 1,       // 2
    CanModifyRoute = 1 << 2,        // 4
    CanSeeAllPositions = 1 << 3,    // 8

    Default = CanAddWaypoints | CanSendMessages | CanSeeAllPositions  // 11
}

public enum ParticipantStatus
{
    Active = 0,
    Paused = 1,
    Problem = 2,
    Arrived = 3,
    Offline = 4
}

public enum VehicleType
{
    Car = 0,
    Motorcycle = 1,
    Truck = 2,
    Van = 3
}

public enum WaypointType
{
    Destination = 0,
    PlannedStop = 1,
    QuickStop = 2,
    Alert = 3
}

public enum WaypointCategory
{
    Fuel = 0,
    Restaurant = 1,
    Rest = 2,
    Other = 3,
    Danger = 4
}

public enum RoutePreference
{
    Fastest = 0,
    Scenic = 1,
    Economical = 2,
    Shortest = 3
}
```

### Étape 4 : Créer les entités principales

`Core/Entities/User.cs` :

```csharp
using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Core.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public bool PhoneVerified { get; set; } = false;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Convoy> CreatedConvoys { get; set; } = new List<Convoy>();
    public ICollection<ConvoyParticipant> ConvoyParticipations { get; set; } = new List<ConvoyParticipant>();
    public ICollection<MagicLinkToken> MagicLinkTokens { get; set; } = new List<MagicLinkToken>();
}
```

`Core/Entities/Convoy.cs` :

```csharp
using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

public class Convoy
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(6)]
    public string Code { get; set; } = string.Empty;  // Alphanumérique

    [MaxLength(500)]
    public string? InviteLink { get; set; }

    [Required]
    public Guid CreatorId { get; set; }

    public ConvoyStatus Status { get; set; } = ConvoyStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ArchivedAt { get; set; }

    public int MaxParticipants { get; set; } = 10;

    public ParticipantPermissions DefaultMemberPermissions { get; set; } = ParticipantPermissions.Default;

    // Navigation properties
    public User Creator { get; set; } = null!;
    public ICollection<ConvoyParticipant> Participants { get; set; } = new List<ConvoyParticipant>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();

    // Helper
    public Trip? ActiveTrip => Trips.FirstOrDefault(t => t.Status == TripStatus.Active);
}
```

`Core/Entities/Trip.cs` :

```csharp
using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

public class Trip
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ConvoyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid DestinationWaypointId { get; set; }  // OBLIGATOIRE

    public RoutePreference RoutePreference { get; set; } = RoutePreference.Fastest;

    public TripStatus Status { get; set; } = TripStatus.Planned;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PlannedDepartureDate { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    // Statistiques (calculées à la fin)
    public double? TotalDistanceKm { get; set; }
    public double? AverageSpeedKmh { get; set; }
    public int? StopCount { get; set; }
    public int? TotalPauseMinutes { get; set; }
    public string? RouteGeoJson { get; set; }

    // Navigation properties
    public Convoy Convoy { get; set; } = null!;
    public Waypoint Destination { get; set; } = null!;
    public ICollection<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
    public ICollection<LocationHistory> LocationHistory { get; set; } = new List<LocationHistory>();
}
```

`Core/Entities/ConvoyParticipant.cs` :

```csharp
using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

public class ConvoyParticipant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ConvoyId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public ParticipantRole Role { get; set; } = ParticipantRole.Member;

    public ParticipantPermissions Permissions { get; set; } = ParticipantPermissions.Default;

    public VehicleType VehicleType { get; set; } = VehicleType.Car;

    [MaxLength(100)]
    public string? VehicleName { get; set; }

    [MaxLength(7)]
    public string Color { get; set; } = "#3F51B5";

    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LeftAt { get; set; }

    public bool IsBanned { get; set; } = false;

    // Navigation properties
    public Convoy Convoy { get; set; } = null!;
    public User User { get; set; } = null!;
}
```

`Core/Entities/Waypoint.cs` :

```csharp
using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Core.Entities;

public class Waypoint
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid TripId { get; set; }  // Lié au TRIP

    [Required]
    public Guid CreatedById { get; set; }

    public WaypointType Type { get; set; } = WaypointType.QuickStop;

    public WaypointCategory Category { get; set; } = WaypointCategory.Other;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ScheduledAt { get; set; }

    public bool IsReached { get; set; } = false;

    public int Order { get; set; } = 0;  // 0 pour destination

    // Navigation properties
    public Trip Trip { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}
```

`Core/Entities/MagicLinkToken.cs` :

```csharp
using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Core.Entities;

public class MagicLinkToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid? UserId { get; set; }  // Nullable pour premier login

    [Required]
    [MaxLength(100)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}
```

### Étape 5 : Créer le DbContext

`Infrastructure/Data/ApplicationDbContext.cs` :

```csharp
using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Convoy> Convoys => Set<Convoy>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<ConvoyParticipant> ConvoyParticipants => Set<ConvoyParticipant>();
    public DbSet<Waypoint> Waypoints => Set<Waypoint>();
    public DbSet<LocationHistory> LocationHistories => Set<LocationHistory>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MagicLinkToken> MagicLinkTokens => Set<MagicLinkToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Convoy
        modelBuilder.Entity<Convoy>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Creator)
                .WithMany(u => u.CreatedConvoys)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Trip
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasOne(e => e.Convoy)
                .WithMany(c => c.Trips)
                .HasForeignKey(e => e.ConvoyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Destination)
                .WithMany()
                .HasForeignKey(e => e.DestinationWaypointId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ConvoyParticipant
        modelBuilder.Entity<ConvoyParticipant>(entity =>
        {
            entity.HasIndex(e => new { e.ConvoyId, e.UserId });

            entity.HasOne(e => e.Convoy)
                .WithMany(c => c.Participants)
                .HasForeignKey(e => e.ConvoyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.ConvoyParticipations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Waypoint
        modelBuilder.Entity<Waypoint>(entity =>
        {
            entity.HasOne(e => e.Trip)
                .WithMany(t => t.Waypoints)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MagicLinkToken
        modelBuilder.Entity<MagicLinkToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.MagicLinkTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

### Étape 6 : Générateur de code convoy

`Infrastructure/CodeGenerator/ConvoyCodeGenerator.cs` :

```csharp
using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Infrastructure.CodeGenerator;

public class ConvoyCodeGenerator
{
    private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int CODE_LENGTH = 6;
    private readonly ApplicationDbContext _context;

    public ConvoyCodeGenerator(ApplicationDbContext context)
    {
        _context = context;
    }

    public static string GenerateCode()
    {
        var random = new Random();
        return new string(Enumerable.Range(0, CODE_LENGTH)
            .Select(_ => CHARS[random.Next(CHARS.Length)])
            .ToArray());
    }

    public async Task<string> GenerateUniqueCodeAsync()
    {
        string code;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            code = GenerateCode();
            attempts++;

            if (attempts >= maxAttempts)
                throw new InvalidOperationException("Failed to generate unique convoy code");

        } while (await _context.Convoys.AnyAsync(c => c.Code == code));

        return code;
    }
}

// Exemples générés : aB3xK9, Zp7mQ2, kL9nR4, Tr5pYx
// 62^6 = 56,800,235,584 possibilités
```

### Étape 7 : Configurer Program.cs

`Program.cs` :

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SyncTrip.Api.Infrastructure.Data;
using SyncTrip.Api.Infrastructure.CodeGenerator;
using SyncTrip.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Database PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
    ));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "SyncTrip:";
});

// Code Generator
builder.Services.AddScoped<ConvoyCodeGenerator>();

// Authentication JWT
var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Support JWT dans SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost", "https://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// SignalR
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ConvoyHub>("/hubs/convoy");

app.Run();
```

`appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=synctrip;Username=synctrip_user;Password=dev_password_123",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "SuperSecretKeyForDevelopmentOnly_MustBe32CharsMinimum_ChangeInProduction",
    "Issuer": "SyncTripApi",
    "Audience": "SyncTripApp",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Étape 8 : Créer les migrations

```bash
# Installer l'outil EF
dotnet tool install --global dotnet-ef

# Créer la migration initiale
dotnet ef migrations add InitialCreate

# Appliquer la migration
dotnet ef database update
```

---

## Implémentation Frontend

### Étape 1 : Créer les entités Core (Frontend)

`Core/Entities/Convoy.cs` :

```csharp
using SyncTrip.Core.Enums;

namespace SyncTrip.Core.Entities;

public class Convoy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;  // Code alphanumérique
    public string? InviteLink { get; set; }
    public ConvoyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ConvoyParticipant> Participants { get; set; } = new();
    public List<Trip> Trips { get; set; } = new();
    public Trip? ActiveTrip => Trips.FirstOrDefault(t => t.Status == TripStatus.Active);
}
```

`Core/Entities/Trip.cs` :

```csharp
using SyncTrip.Core.Enums;

namespace SyncTrip.Core.Entities;

public class Trip
{
    public Guid Id { get; set; }
    public Guid ConvoyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public RoutePreference RoutePreference { get; set; }
    public TripStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }

    public Waypoint Destination { get; set; } = null!;
    public List<Waypoint> Waypoints { get; set; } = new();
}
```

### Étape 2 : Configurer MauiProgram.cs

`MauiProgram.cs` :

```csharp
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Services.Api;
using SyncTrip.Services.Location;
using SyncTrip.Services.SignalR;
using SyncTrip.ViewModels;

namespace SyncTrip;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // HTTP Client
        builder.Services.AddSingleton(sp =>
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7001")
            };
            return httpClient;
        });

        // Services
        builder.Services.AddSingleton<IApiClient, ApiClient>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<ISignalRService, SignalRService>();

        // ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ConvoyMapViewModel>();
        builder.Services.AddTransient<AuthViewModel>();

        // Pages
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

---

## Intégration SignalR

### Backend : ConvoyHub

`Hubs/ConvoyHub.cs` :

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SyncTrip.Api.Hubs;

public class LocationUpdateDto
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Accuracy { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public DateTime Timestamp { get; set; }
}

[Authorize]
public class ConvoyHub : Hub
{
    private readonly ILogger<ConvoyHub> _logger;

    public ConvoyHub(ILogger<ConvoyHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinTrip(string tripId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tripId);
        _logger.LogInformation("User {UserId} joined trip {TripId}",
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, tripId);
    }

    public async Task LeaveTrip(string tripId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, tripId);
        _logger.LogInformation("User {UserId} left trip {TripId}",
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, tripId);
    }

    public async Task SendLocationUpdate(LocationUpdateDto locationUpdate)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        locationUpdate.UserId = Guid.Parse(userId!);
        locationUpdate.Timestamp = DateTime.UtcNow;

        // Broadcast à tous les participants du trip
        await Clients.Group(locationUpdate.TripId.ToString())
            .SendAsync("LocationUpdate", locationUpdate);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
```

### Frontend : SignalRService

`Services/SignalR/SignalRService.cs` :

```csharp
using Microsoft.AspNetCore.SignalR.Client;
using SyncTrip.Core.Interfaces;
using SyncTrip.Models.DTOs;

namespace SyncTrip.Services.SignalR;

public class SignalRService : ISignalRService
{
    private HubConnection? _hubConnection;

    public event EventHandler<LocationUpdateDto>? LocationUpdateReceived;
    public event EventHandler<WaypointAddedDto>? WaypointAddedReceived;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(string token)
    {
        if (_hubConnection != null)
            await DisconnectAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/hubs/convoy", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10) })
            .Build();

        // Subscribe to events
        _hubConnection.On<LocationUpdateDto>("LocationUpdate", (update) =>
        {
            LocationUpdateReceived?.Invoke(this, update);
        });

        _hubConnection.On<WaypointAddedDto>("WaypointAdded", (waypoint) =>
        {
            WaypointAddedReceived?.Invoke(this, waypoint);
        });

        await _hubConnection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async Task JoinTripAsync(Guid tripId)
    {
        if (_hubConnection != null && IsConnected)
        {
            await _hubConnection.InvokeAsync("JoinTrip", tripId.ToString());
        }
    }

    public async Task LeaveTripAsync(Guid tripId)
    {
        if (_hubConnection != null && IsConnected)
        {
            await _hubConnection.InvokeAsync("LeaveTrip", tripId.ToString());
        }
    }

    public async Task SendLocationUpdateAsync(LocationUpdateDto location)
    {
        if (_hubConnection != null && IsConnected)
        {
            await _hubConnection.InvokeAsync("SendLocationUpdate", location);
        }
    }
}
```

---

## Authentification Passwordless

### Backend : AuthService

`Application/Services/AuthService.cs` :

```csharp
using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Infrastructure.Data;

namespace SyncTrip.Api.Application.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AuthService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<string> RequestMagicLinkAsync(string email)
    {
        // Vérifier si user existe
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Générer token unique
        var token = Guid.NewGuid().ToString();
        var magicLink = new MagicLinkToken
        {
            UserId = user?.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };

        _context.MagicLinkTokens.Add(magicLink);
        await _context.SaveChangesAsync();

        // Envoyer email
        var link = $"https://synctrip.app/auth/verify?token={token}";
        await _emailService.SendMagicLinkAsync(email, link);

        return token;
    }

    public async Task<(bool success, User? user, bool needsProfile)> VerifyMagicLinkAsync(string token)
    {
        var magicLink = await _context.MagicLinkTokens
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Token == token);

        if (magicLink == null || magicLink.IsUsed || magicLink.ExpiresAt < DateTime.UtcNow)
            return (false, null, false);

        // Marquer comme utilisé
        magicLink.IsUsed = true;
        await _context.SaveChangesAsync();

        var needsProfile = magicLink.User == null;

        return (true, magicLink.User, needsProfile);
    }
}
```

---

## Prochaines étapes

1. **Implémenter controllers API** (ConvoysController, TripsController, AuthController)
2. **Créer ViewModels MAUI** avec MVVM Toolkit
3. **Intégrer Mapsui** pour les cartes
4. **Implémenter SQLite local** pour cache offline
5. **Ajouter tests unitaires** (xUnit)

Consultez `DOCUMENTATION.md` pour l'architecture complète et tous les diagrammes.

---

**Version** : 2.0
**Date** : 2025-11-16
**Framework** : .NET 10
