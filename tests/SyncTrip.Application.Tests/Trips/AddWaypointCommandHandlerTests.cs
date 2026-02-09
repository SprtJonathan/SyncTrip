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
/// Tests unitaires pour le handler AddWaypointCommand.
/// </summary>
public class AddWaypointCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<AddWaypointCommandHandler>> _loggerMock;
    private readonly AddWaypointCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public AddWaypointCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<AddWaypointCommandHandler>>();

        _handler = new AddWaypointCommandHandler(
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
    public async Task Handle_WithValidData_ShouldAddWaypointAndReturnId()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        var memberId = Guid.NewGuid();
        convoy.AddMember(memberId, Guid.NewGuid());

        var command = new AddWaypointCommand
        {
            TripId = trip.Id,
            UserId = memberId,
            OrderIndex = 0,
            Latitude = 48.8566,
            Longitude = 2.3522,
            Name = "Paris",
            Type = WaypointType.Start
        };

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
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _tripRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentTrip_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new AddWaypointCommand
        {
            TripId = Guid.NewGuid(),
            UserId = _validLeaderId,
            OrderIndex = 0,
            Latitude = 48.8566,
            Longitude = 2.3522,
            Name = "Paris",
            Type = WaypointType.Start
        };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        var nonMemberId = Guid.NewGuid();

        var command = new AddWaypointCommand
        {
            TripId = trip.Id,
            UserId = nonMemberId,
            OrderIndex = 0,
            Latitude = 48.8566,
            Longitude = 2.3522,
            Name = "Paris",
            Type = WaypointType.Start
        };

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .Returns<Guid, CancellationToken>((id, ct) =>
            {
                typeof(Trip).GetProperty("Convoy")!.SetValue(trip, convoy);
                return Task.FromResult<Trip?>(trip);
            });

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithFinishedTrip_ShouldThrowDomainException()
    {
        // Arrange
        var (convoy, trip) = CreateConvoyWithActiveTrip();
        trip.Finish();

        var command = new AddWaypointCommand
        {
            TripId = trip.Id,
            UserId = _validLeaderId,
            OrderIndex = 0,
            Latitude = 48.8566,
            Longitude = 2.3522,
            Name = "Paris",
            Type = WaypointType.Start
        };

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
            .WithMessage("*voyage terminé*");
    }

    #endregion
}
