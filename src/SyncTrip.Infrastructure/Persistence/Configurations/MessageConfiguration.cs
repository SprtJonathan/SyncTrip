using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Message.
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ConvoyId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.SentAt)
            .IsRequired();

        // Relation many-to-one avec Convoy
        builder.HasOne(m => m.Convoy)
            .WithMany()
            .HasForeignKey(m => m.ConvoyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation many-to-one avec User (expéditeur)
        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index composite pour la pagination par convoi et date
        builder.HasIndex(m => new { m.ConvoyId, m.SentAt })
            .IsDescending(false, true);
    }
}
