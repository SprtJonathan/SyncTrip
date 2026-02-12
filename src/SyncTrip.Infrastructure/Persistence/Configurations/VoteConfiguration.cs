using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SyncTrip.Core.Entities;

namespace SyncTrip.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Vote.
/// </summary>
public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.ToTable("Votes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.StopProposalId)
            .IsRequired();

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.IsYes)
            .IsRequired();

        builder.Property(v => v.VotedAt)
            .IsRequired();

        // Relation many-to-one avec StopProposal
        builder.HasOne(v => v.StopProposal)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.StopProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation many-to-one avec User
        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index unique : un seul vote par utilisateur par proposition
        builder.HasIndex(v => new { v.StopProposalId, v.UserId })
            .IsUnique();
    }
}
