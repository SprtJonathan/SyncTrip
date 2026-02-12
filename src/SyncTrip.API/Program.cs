using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SyncTrip.API.Hubs;
using SyncTrip.Application.Auth.Validators;
using SyncTrip.Infrastructure;
using SyncTrip.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// OpenAPI + Scalar (remplacement de Swagger pour .NET 10)
builder.Services.AddOpenApi();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173") // Ajuster selon les besoins
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Support JWT via query string pour SignalR
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

builder.Services.AddAuthorization();

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Limite globale par IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Policy spécifique pour les endpoints d'authentification (plus restrictif)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(10);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Comportement en cas de dépassement
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Message = "Trop de requêtes. Veuillez réessayer dans quelques instants.",
            RetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? (double?)retryAfter.TotalSeconds
                : (double?)null
        }, cancellationToken: token);
    };
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(SyncTrip.Application.Auth.Commands.SendMagicLinkCommand).Assembly);
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CompleteRegistrationValidator>();

// Add SignalR
builder.Services.AddSignalR();

// Add Trip Notification Service (SignalR abstraction for Application layer)
builder.Services.AddScoped<SyncTrip.Application.Voting.Services.ITripNotificationService, SyncTrip.API.Services.TripNotificationService>();

// Add Infrastructure services (includes DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

// Exception Handling Middleware - IMPORTANT: Doit être placé avant tout autre middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            // Log l'erreur avec tous les détails (pour le monitoring)
            logger.LogError(exceptionHandlerPathFeature?.Error,
                "Unhandled exception occurred. Path: {Path}",
                exceptionHandlerPathFeature?.Path);

            // Retourner une réponse générique au client (ne pas exposer les détails)
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Une erreur interne est survenue. Veuillez réessayer ultérieurement.",
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            });
        });
    });

    // HSTS pour sécurité en production
    app.UseHsts();
}

// Apply migrations and seed data automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Rate Limiting Middleware
app.UseRateLimiter();

app.MapControllers();

// Map SignalR Hub
app.MapHub<TripHub>("/hubs/trip");

app.Run();
