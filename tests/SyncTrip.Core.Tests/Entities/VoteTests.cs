using FluentAssertions;
using SyncTrip.Core.Entities;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Vote.
/// </summary>
public class VoteTests
{
    private readonly Guid _validProposalId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateVote()
    {
        // Act
        var vote = Vote.Create(_validProposalId, _validUserId, true);

        // Assert
        vote.Should().NotBeNull();
        vote.Id.Should().NotBe(Guid.Empty);
        vote.StopProposalId.Should().Be(_validProposalId);
        vote.UserId.Should().Be(_validUserId);
        vote.IsYes.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNoVote_ShouldSetIsYesFalse()
    {
        // Act
        var vote = Vote.Create(_validProposalId, _validUserId, false);

        // Assert
        vote.IsYes.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldSetVotedAt()
    {
        // Act
        var before = DateTime.UtcNow;
        var vote = Vote.Create(_validProposalId, _validUserId, true);
        var after = DateTime.UtcNow;

        // Assert
        vote.VotedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_WithEmptyProposalId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Vote.Create(Guid.Empty, _validUserId, true);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*proposition*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Vote.Create(_validProposalId, Guid.Empty, true);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*utilisateur*");
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Act
        var vote1 = Vote.Create(_validProposalId, _validUserId, true);
        var vote2 = Vote.Create(_validProposalId, Guid.NewGuid(), false);

        // Assert
        vote1.Id.Should().NotBe(vote2.Id);
    }

    #endregion
}
