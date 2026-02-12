using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Voting.Commands;
using SyncTrip.Application.Voting.Services;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Voting;
using Xunit;

namespace SyncTrip.Application.Tests.Voting;

/// <summary>
/// Tests unitaires pour le handler CastVoteCommand.
/// </summary>
public class CastVoteCommandHandlerTests
{
    private readonly Mock<IStopProposalRepository> _proposalRepositoryMock;
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ITripNotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<CastVoteCommandHandler>> _loggerMock;
    private readonly CastVoteCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public CastVoteCommandHandlerTests()
    {
        _proposalRepositoryMock = new Mock<IStopProposalRepository>();
        _tripRepositoryMock = new Mock<ITripRepository>();
        _notificationServiceMock = new Mock<ITripNotificationService>();
        _loggerMock = new Mock<ILogger<CastVoteCommandHandler>>();

        _handler = new CastVoteCommandHandler(
            _proposalRepositoryMock.Object,
            _tripRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object
        );
    }

    private (Convoy convoy, Trip trip, StopProposal proposal) CreateConvoyWithTripAndProposal()
    {
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var trip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);

        // Wire up relationships
        typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);

        var proposal = StopProposal.Create(trip.Id, _validLeaderId, StopType.Fuel, 48.8566, 2.3522, "Station Total");
        proposal.CastVote(_validLeaderId, true); // Auto-vote du proposeur

        // Wire up relationships
        typeof(StopProposal).GetProperty("Trip")!.SetValue(proposal, trip);

        return (convoy, trip, proposal);
    }

    private void SetupProposalRepository(StopProposal proposal)
    {
        _proposalRepositoryMock
            .Setup(x => x.GetByIdAsync(proposal.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposal);

        _proposalRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithYesVote_ShouldCastVote()
    {
        // Arrange
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        var voterId = Guid.NewGuid();
        convoy.AddMember(voterId, Guid.NewGuid());
        SetupProposalRepository(proposal);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = voterId,
            IsYes = true
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        proposal.Votes.Should().HaveCount(2); // Auto-vote + new vote
        _proposalRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WithNoVote_ShouldCastVote()
    {
        // Arrange
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        var voterId = Guid.NewGuid();
        convoy.AddMember(voterId, Guid.NewGuid());
        SetupProposalRepository(proposal);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = voterId,
            IsYes = false
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        proposal.Votes.Should().Contain(v => v.UserId == voterId && !v.IsYes);
    }

    [Fact]
    public async Task Handle_ShouldNotifyVoteUpdate()
    {
        // Arrange
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        var voterId = Guid.NewGuid();
        convoy.AddMember(voterId, Guid.NewGuid());
        // Add a third member so we don't trigger early resolution
        convoy.AddMember(Guid.NewGuid(), Guid.NewGuid());
        SetupProposalRepository(proposal);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = voterId,
            IsYes = true
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyVoteUpdateAsync(
                proposal.TripId, proposal.Id, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAllMembersVoted_ShouldResolveEarly()
    {
        // Arrange (2 members: leader + voter)
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        var voterId = Guid.NewGuid();
        convoy.AddMember(voterId, Guid.NewGuid());
        SetupProposalRepository(proposal);

        _tripRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = voterId,
            IsYes = true
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — All 2 members voted, should resolve
        proposal.Status.Should().NotBe(ProposalStatus.Pending);
        _notificationServiceMock.Verify(
            x => x.NotifyProposalResolvedAsync(
                proposal.TripId, It.IsAny<StopProposalDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccepted_ShouldCreateWaypoint()
    {
        // Arrange (2 members, both vote yes → accepted)
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        var voterId = Guid.NewGuid();
        convoy.AddMember(voterId, Guid.NewGuid());
        SetupProposalRepository(proposal);

        _tripRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = voterId,
            IsYes = true
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Accepted);
        proposal.CreatedWaypointId.Should().NotBeNull();
        trip.Waypoints.Should().HaveCount(1);
        _tripRepositoryMock.Verify(
            x => x.UpdateAsync(trip, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentProposal_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _proposalRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StopProposal?)null);

        var command = new CastVoteCommand
        {
            ProposalId = Guid.NewGuid(),
            UserId = _validLeaderId,
            IsYes = true
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        SetupProposalRepository(proposal);

        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = Guid.NewGuid(), // Non-member
            IsYes = true
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithAlreadyVotedUser_ShouldThrowDomainException()
    {
        // Arrange
        var (convoy, trip, proposal) = CreateConvoyWithTripAndProposal();
        SetupProposalRepository(proposal);

        // Leader already voted (auto-vote)
        var command = new CastVoteCommand
        {
            ProposalId = proposal.Id,
            UserId = _validLeaderId,
            IsYes = false
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*déjà voté*");
    }

    #endregion
}
