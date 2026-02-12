using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité StopProposal.
/// </summary>
public class StopProposalTests
{
    private readonly Guid _validTripId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateProposal()
    {
        // Act
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 48.8566, 2.3522, "Station Total");

        // Assert
        proposal.Should().NotBeNull();
        proposal.Id.Should().NotBe(Guid.Empty);
        proposal.TripId.Should().Be(_validTripId);
        proposal.ProposedByUserId.Should().Be(_validUserId);
        proposal.StopType.Should().Be(StopType.Fuel);
        proposal.Latitude.Should().Be(48.8566);
        proposal.Longitude.Should().Be(2.3522);
        proposal.LocationName.Should().Be("Station Total");
        proposal.Status.Should().Be(ProposalStatus.Pending);
        proposal.Votes.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldSetCreatedAtAndExpiresAt()
    {
        // Act
        var before = DateTime.UtcNow;
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Break, 45.0, 3.0, "Aire de repos");
        var after = DateTime.UtcNow;

        // Assert
        proposal.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        proposal.ExpiresAt.Should().BeCloseTo(proposal.CreatedAt.AddSeconds(30), TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Create_ShouldSetResolvedAtToNull()
    {
        // Act
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Food, 45.0, 3.0, "Restaurant");

        // Assert
        proposal.ResolvedAt.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldSetCreatedWaypointIdToNull()
    {
        // Act
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Photo, 45.0, 3.0, "Point de vue");

        // Assert
        proposal.CreatedWaypointId.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyTripId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(Guid.Empty, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*voyage*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(_validTripId, Guid.Empty, StopType.Fuel, 45.0, 3.0, "Station");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*utilisateur*");
    }

    [Fact]
    public void Create_WithEmptyLocationName_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "");
        act.Should().Throw<DomainException>()
            .WithMessage("*nom du lieu*");
    }

    [Fact]
    public void Create_WithWhitespaceLocationName_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "   ");
        act.Should().Throw<DomainException>()
            .WithMessage("*nom du lieu*");
    }

    [Fact]
    public void Create_WithInvalidLatitude_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 91.0, 3.0, "Station");
        act.Should().Throw<DomainException>()
            .WithMessage("*latitude*");
    }

    [Fact]
    public void Create_WithInvalidLongitude_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 181.0, "Station");
        act.Should().Throw<DomainException>()
            .WithMessage("*longitude*");
    }

    [Theory]
    [InlineData(StopType.Fuel)]
    [InlineData(StopType.Break)]
    [InlineData(StopType.Food)]
    [InlineData(StopType.Photo)]
    public void Create_WithAllStopTypes_ShouldSucceed(StopType stopType)
    {
        // Act
        var proposal = StopProposal.Create(_validTripId, _validUserId, stopType, 45.0, 3.0, "Lieu");

        // Assert
        proposal.StopType.Should().Be(stopType);
    }

    #endregion

    #region CastVote

    [Fact]
    public void CastVote_WithValidData_ShouldAddVote()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        var voterId = Guid.NewGuid();

        // Act
        proposal.CastVote(voterId, true);

        // Assert
        proposal.Votes.Should().HaveCount(1);
        proposal.Votes.First().UserId.Should().Be(voterId);
        proposal.Votes.First().IsYes.Should().BeTrue();
    }

    [Fact]
    public void CastVote_WithNoVote_ShouldAddNoVote()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        var voterId = Guid.NewGuid();

        // Act
        proposal.CastVote(voterId, false);

        // Assert
        proposal.Votes.Should().HaveCount(1);
        proposal.Votes.First().IsYes.Should().BeFalse();
    }

    [Fact]
    public void CastVote_MultipleVoters_ShouldAddAllVotes()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");

        // Act
        proposal.CastVote(Guid.NewGuid(), true);
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), true);

        // Assert
        proposal.Votes.Should().HaveCount(3);
    }

    [Fact]
    public void CastVote_SameUserTwice_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        var voterId = Guid.NewGuid();
        proposal.CastVote(voterId, true);

        // Act & Assert
        var act = () => proposal.CastVote(voterId, false);
        act.Should().Throw<DomainException>()
            .WithMessage("*déjà voté*");
    }

    [Fact]
    public void CastVote_OnResolvedProposal_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.Resolve(3); // Résout sans votes → accepté par silence

        // Act & Assert
        var act = () => proposal.CastVote(Guid.NewGuid(), true);
        act.Should().Throw<DomainException>()
            .WithMessage("*plus en attente*");
    }

    #endregion

    #region Resolve - Règle du silence

    [Fact]
    public void Resolve_WithNoVotes_ShouldAcceptBySilence()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");

        // Act
        proposal.Resolve(3);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Accepted);
        proposal.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public void Resolve_WithAllYesVotes_ShouldAccept()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), true);
        proposal.CastVote(Guid.NewGuid(), true);
        proposal.CastVote(Guid.NewGuid(), true);

        // Act
        proposal.Resolve(3);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Accepted);
    }

    [Fact]
    public void Resolve_WithMajorityNo_ShouldReject()
    {
        // Arrange (3 membres, 2 NON = majorité absolue)
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), true);

        // Act
        proposal.Resolve(3);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Rejected);
    }

    [Fact]
    public void Resolve_WithExactHalfNo_ShouldAcceptBySilence()
    {
        // Arrange (4 membres, 2 NON = exactement la moitié, pas majorité absolue)
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), true);
        proposal.CastVote(Guid.NewGuid(), true);

        // Act
        proposal.Resolve(4);

        // Assert — 2 NON sur 4 membres = 2 > 2.0 est faux → Accepted
        proposal.Status.Should().Be(ProposalStatus.Accepted);
    }

    [Fact]
    public void Resolve_WithMajorityNoOn4Members_ShouldReject()
    {
        // Arrange (4 membres, 3 NON = majorité absolue)
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.CastVote(Guid.NewGuid(), true);

        // Act
        proposal.Resolve(4);

        // Assert — 3 NON sur 4 membres = 3 > 2.0 → Rejected
        proposal.Status.Should().Be(ProposalStatus.Rejected);
    }

    [Fact]
    public void Resolve_WithOneMemberOneYes_ShouldAccept()
    {
        // Arrange (1 membre, 1 OUI)
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), true);

        // Act
        proposal.Resolve(1);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Accepted);
    }

    [Fact]
    public void Resolve_WithOneMemberOneNo_ShouldReject()
    {
        // Arrange (1 membre, 1 NON → 1 > 0.5 = true)
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), false);

        // Act
        proposal.Resolve(1);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Rejected);
    }

    [Fact]
    public void Resolve_ShouldSetResolvedAt()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");

        // Act
        var before = DateTime.UtcNow;
        proposal.Resolve(3);
        var after = DateTime.UtcNow;

        // Assert
        proposal.ResolvedAt.Should().NotBeNull();
        proposal.ResolvedAt!.Value.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Resolve_AlreadyResolved_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.Resolve(3);

        // Act & Assert
        var act = () => proposal.Resolve(3);
        act.Should().Throw<DomainException>()
            .WithMessage("*déjà été résolue*");
    }

    #endregion

    #region AllMembersVoted

    [Fact]
    public void AllMembersVoted_WithAllVotes_ShouldReturnTrue()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), true);
        proposal.CastVote(Guid.NewGuid(), false);

        // Act & Assert
        proposal.AllMembersVoted(2).Should().BeTrue();
    }

    [Fact]
    public void AllMembersVoted_WithMissingVotes_ShouldReturnFalse()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), true);

        // Act & Assert
        proposal.AllMembersVoted(3).Should().BeFalse();
    }

    [Fact]
    public void AllMembersVoted_WithNoVotes_ShouldReturnFalse()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");

        // Act & Assert
        proposal.AllMembersVoted(3).Should().BeFalse();
    }

    #endregion

    #region SetCreatedWaypoint

    [Fact]
    public void SetCreatedWaypoint_OnAcceptedProposal_ShouldSetWaypointId()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.Resolve(3); // Accepted by silence
        var waypointId = Guid.NewGuid();

        // Act
        proposal.SetCreatedWaypoint(waypointId);

        // Assert
        proposal.CreatedWaypointId.Should().Be(waypointId);
    }

    [Fact]
    public void SetCreatedWaypoint_OnPendingProposal_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        var waypointId = Guid.NewGuid();

        // Act & Assert
        var act = () => proposal.SetCreatedWaypoint(waypointId);
        act.Should().Throw<DomainException>()
            .WithMessage("*acceptée*");
    }

    [Fact]
    public void SetCreatedWaypoint_OnRejectedProposal_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.CastVote(Guid.NewGuid(), false);
        proposal.Resolve(1); // Rejected (1 NO on 1 member)
        var waypointId = Guid.NewGuid();

        // Act & Assert
        var act = () => proposal.SetCreatedWaypoint(waypointId);
        act.Should().Throw<DomainException>()
            .WithMessage("*acceptée*");
    }

    [Fact]
    public void SetCreatedWaypoint_WithEmptyWaypointId_ShouldThrowArgumentException()
    {
        // Arrange
        var proposal = StopProposal.Create(_validTripId, _validUserId, StopType.Fuel, 45.0, 3.0, "Station");
        proposal.Resolve(3); // Accepted

        // Act & Assert
        var act = () => proposal.SetCreatedWaypoint(Guid.Empty);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*waypoint*");
    }

    #endregion
}
