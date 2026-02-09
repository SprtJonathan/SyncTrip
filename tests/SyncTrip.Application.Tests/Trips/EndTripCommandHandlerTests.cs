using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Trips.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Trips;

/// <summary>
/// Tests unitaires pour le handler EndTripCommand.
/// </summary>
public class EndTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<EndTripCommandHandler>> _loggerMock;
    private readonly EndTripCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public EndTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<EndTripCommandHandler>>();

        _handler = new EndTripCommandHandler(
            _tripRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    private (Convoy convoy, Trip trip) CreateConvoyWithActiveTrip()
    {
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var trip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);
        return (convoy, trip);
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldEndTrip()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        var command = new EndTripCommand { TripId = trip.Id, UserId = _validLeaderId };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip)
            .Callback(() =>
            {
                // Simuler la navigation property chargée par EF
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
            });

        // Configurer le setup pour retourner trip avec Convoy
        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .Returns<Guid, CancellationToken>((id, ct) =>
            {
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
                return Task.FromResult<Trip?>(trip);
            });

        _tripRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _tripRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Trip>(t => t.Status == TripStatus.Finished), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentTrip_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new EndTripCommand { TripId = Guid.NewGuid(), UserId = _validLeaderId };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonLeader_ShouldThrowDomainException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        var nonLeaderId = Guid.NewGuid();
        convoy.AddMember(nonLeaderId, Guid.NewGuid());

        var command = new EndTripCommand { TripId = trip.Id, UserId = nonLeaderId };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .Returns<Guid, CancellationToken>((id, ct) =>
            {
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
                return Task.FromResult<Trip?>(trip);
            });

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*leader*");
    }

    [Fact]
    public async Task Handle_WithAlreadyFinishedTrip_ShouldThrowDomainException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        trip.Finish();

        var command = new EndTripCommand { TripId = trip.Id, UserId = _validLeaderId };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .Returns<Guid, CancellationToken>((id, ct) =>
            {
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
                return Task.FromResult<Trip?>(trip);
            });

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*déjà terminé*");
    }

    #endregion
}
