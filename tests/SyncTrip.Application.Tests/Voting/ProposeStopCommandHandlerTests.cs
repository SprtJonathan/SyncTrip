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
/// Tests unitaires pour le handler ProposeStopCommand.
/// </summary>
public class ProposeStopCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IStopProposalRepository> _proposalRepositoryMock;
    private readonly Mock<ITripNotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<ProposeStopCommandHandler>> _loggerMock;
    private readonly ProposeStopCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public ProposeStopCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _proposalRepositoryMock = new Mock<IStopProposalRepository>();
        _notificationServiceMock = new Mock<ITripNotificationService>();
        _loggerMock = new Mock<ILogger<ProposeStopCommandHandler>>();

        _handler = new ProposeStopCommandHandler(
            _tripRepositoryMock.Object,
            _proposalRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object
        );
    }

    private (Convoy convoy, Trip trip) CreateConvoyWithActiveTrip()
    {
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var trip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);
        return (convoy, trip);
    }

    private void SetupTripRepository(Trip trip, Convoy convoy)
    {
        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .Returns<Guid, CancellationToken>((id, ct) =>
            {
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
                return Task.FromResult<Trip?>(trip);
            });
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateProposalAndReturnId()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        SetupTripRepository(trip, convoy);

        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StopProposal?)null);

        _proposalRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _proposalRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAutoVoteYesForProposer()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        SetupTripRepository(trip, convoy);

        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StopProposal?)null);

        StopProposal? savedProposal = null;
        _proposalRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()))
            .Callback<StopProposal, CancellationToken>((p, ct) => savedProposal = p)
            .Returns(Task.CompletedTask);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        savedProposal.Should().NotBeNull();
        savedProposal!.Votes.Should().HaveCount(1);
        savedProposal.Votes.First().UserId.Should().Be(_validLeaderId);
        savedProposal.Votes.First().IsYes.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldNotifyStopProposed()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        SetupTripRepository(trip, convoy);

        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StopProposal?)null);

        _proposalRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<StopProposal>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyStopProposedAsync(trip.Id, It.IsAny<StopProposalDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentTrip_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        var command = new ProposeStopCommand
        {
            TripId = Guid.NewGuid(),
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithFinishedTrip_ShouldThrowDomainException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        trip.Finish();
        SetupTripRepository(trip, convoy);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*voyage terminé*");
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        SetupTripRepository(trip, convoy);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = Guid.NewGuid(), // Non-member
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithExistingPendingProposal_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        SetupTripRepository(trip, convoy);

        var existingProposal = StopProposal.Create(trip.Id, _validLeaderId, StopType.Break, 45.0, 3.0, "Pause");
        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProposal);

        var command = new ProposeStopCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            StopType = StopType.Fuel,
            Latitude = 48.8566,
            Longitude = 2.3522,
            LocationName = "Station Total"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*déjà en cours*");
    }

    #endregion
}
