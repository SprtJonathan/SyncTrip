using Microsoft.EntityFrameworkCore;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Infrastructure.Data;

/// <summary>
/// DbContext principal de l'application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Convoy> Convoys => Set<Convoy>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<ConvoyParticipant> ConvoyParticipants => Set<ConvoyParticipant>();
    public DbSet<Waypoint> Waypoints => Set<Waypoint>();
    public DbSet<MagicLinkToken> MagicLinkTokens => Set<MagicLinkToken>();
    public DbSet<LocationHistory> LocationHistories => Set<LocationHistory>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== USER CONFIGURATION =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            entity.HasIndex(e => e.PhoneNumber)
                .HasDatabaseName("IX_Users_PhoneNumber");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            // Conversion enum vers string pour meilleure lisibilité en base
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(100);
        });

        // ===== CONVOY CONFIGURATION =====
        modelBuilder.Entity<Convoy>(entity =>
        {
            entity.ToTable("Convoys");

            entity.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_Convoys_Code");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Convoys_Status");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Convoys_CreatedAt");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(6)
                .IsFixedLength();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Filtre global : exclure les convois supprimés par défaut
            entity.HasQueryFilter(c => c.Status != ConvoyStatus.Deleted);
        });

        // ===== TRIP CONFIGURATION =====
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("Trips");

            entity.HasIndex(e => e.ConvoyId)
                .HasDatabaseName("IX_Trips_ConvoyId");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Trips_Status");

            entity.HasIndex(e => new { e.ConvoyId, e.Status })
                .HasDatabaseName("IX_Trips_ConvoyId_Status");

            entity.Property(e => e.Destination)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.RoutePreference)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relation avec Convoy
            entity.HasOne(t => t.Convoy)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.ConvoyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== CONVOY PARTICIPANT CONFIGURATION =====
        modelBuilder.Entity<ConvoyParticipant>(entity =>
        {
            entity.ToTable("ConvoyParticipants");

            // Index unique : un user ne peut être qu'une fois dans un convoy (actif)
            entity.HasIndex(e => new { e.ConvoyId, e.UserId, e.IsActive })
                .IsUnique()
                .HasFilter("\"IsActive\" = true")
                .HasDatabaseName("IX_ConvoyParticipants_Unique_Active");

            entity.HasIndex(e => e.ConvoyId)
                .HasDatabaseName("IX_ConvoyParticipants_ConvoyId");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_ConvoyParticipants_UserId");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_ConvoyParticipants_IsActive");

            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relations
            entity.HasOne(cp => cp.Convoy)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConvoyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cp => cp.User)
                .WithMany(u => u.ConvoyParticipations)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== WAYPOINT CONFIGURATION =====
        modelBuilder.Entity<Waypoint>(entity =>
        {
            entity.ToTable("Waypoints");

            entity.HasIndex(e => e.TripId)
                .HasDatabaseName("IX_Waypoints_TripId");

            entity.HasIndex(e => new { e.TripId, e.Order })
                .HasDatabaseName("IX_Waypoints_TripId_Order");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Relation avec Trip
            entity.HasOne(w => w.Trip)
                .WithMany(t => t.Waypoints)
                .HasForeignKey(w => w.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== MAGIC LINK TOKEN CONFIGURATION =====
        modelBuilder.Entity<MagicLinkToken>(entity =>
        {
            entity.ToTable("MagicLinkTokens");

            entity.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("IX_MagicLinkTokens_Token");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_MagicLinkTokens_UserId");

            entity.HasIndex(e => new { e.ExpiresAt, e.IsUsed })
                .HasDatabaseName("IX_MagicLinkTokens_ExpiresAt_IsUsed");

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(100);

            // Relation avec User
            entity.HasOne(mlt => mlt.User)
                .WithMany(u => u.MagicLinkTokens)
                .HasForeignKey(mlt => mlt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== LOCATION HISTORY CONFIGURATION =====
        modelBuilder.Entity<LocationHistory>(entity =>
        {
            entity.ToTable("LocationHistories");

            entity.HasIndex(e => e.TripId)
                .HasDatabaseName("IX_LocationHistories_TripId");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_LocationHistories_UserId");

            entity.HasIndex(e => new { e.TripId, e.UserId, e.Timestamp })
                .HasDatabaseName("IX_LocationHistories_TripId_UserId_Timestamp");

            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_LocationHistories_Timestamp");

            // Relations
            entity.HasOne(lh => lh.Trip)
                .WithMany(t => t.LocationHistory)
                .HasForeignKey(lh => lh.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(lh => lh.User)
                .WithMany(u => u.LocationHistory)
                .HasForeignKey(lh => lh.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== MESSAGE CONFIGURATION =====
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");

            entity.HasIndex(e => e.ConvoyId)
                .HasDatabaseName("IX_Messages_ConvoyId");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Messages_UserId");

            entity.HasIndex(e => new { e.ConvoyId, e.SentAt })
                .HasDatabaseName("IX_Messages_ConvoyId_SentAt");

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_Messages_IsDeleted");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relations
            entity.HasOne(m => m.Convoy)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConvoyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Si user supprimé, message reste mais UserId = null

            // Filtre global : exclure les messages supprimés par défaut
            entity.HasQueryFilter(m => !m.IsDeleted);
        });
    }
}
