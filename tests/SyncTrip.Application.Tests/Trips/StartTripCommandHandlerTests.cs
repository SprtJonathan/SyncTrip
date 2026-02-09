using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Trips.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Trips;
using Xunit;

namespace SyncTrip.Application.Tests.Trips;

/// <summary>
/// Tests unitaires pour le handler StartTripCommand.
/// </summary>
public class StartTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IConvoyRepository> _convoyRepositoryMock;
    private readonly Mock<ILogger<StartTripCommandHandler>> _loggerMock;
    private readonly StartTripCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public StartTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _convoyRepositoryMock = new Mock<IConvoyRepository>();
        _loggerMock = new Mock<ILogger<StartTripCommandHandler>>();

        _handler = new StartTripCommandHandler(
            _tripRepositoryMock.Object,
            _convoyRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    private Convoy CreateValidConvoy()
    {
        return Convoy.Create(_validLeaderId, _validVehicleId, false);
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateTripAndReturnId()
    {
        // Arrange
        var convoy = CreateValidConvoy();
        var command = new StartTripCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Status = TripStatus.Recording,
            RouteProfile = RouteProfile.Fast
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _tripRepositoryMock
            .Setup(x => x.GetActiveByConvoyIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        _tripRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _tripRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithWaypoints_ShouldCreateTripWithWaypoints()
    {
        // Arrange
        var convoy = CreateValidConvoy();
        var command = new StartTripCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Status = TripStatus.Recording,
            RouteProfile = RouteProfile.Fast,
            Waypoints = new List<CreateWaypointRequest>
            {
                new() { OrderIndex = 0, Latitude = 48.8566, Longitude = 2.3522, Name = "Paris", Type = (int)WaypointType.Start },
                new() { OrderIndex = 1, Latitude = 43.2965, Longitude = 5.3698, Name = "Marseille", Type = (int)WaypointType.Destination }
            }
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _tripRepositoryMock
            .Setup(x => x.GetActiveByConvoyIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        _tripRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _tripRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Trip>(t => t.Waypoints.Count == 2), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentConvoy_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new StartTripCommand
        {
            ConvoyId = Guid.NewGuid(),
            UserId = _validLeaderId,
            Status = TripStatus.Recording,
            RouteProfile = RouteProfile.Fast
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ConvoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonLeader_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = CreateValidConvoy();
        var nonLeaderId = Guid.NewGuid();
        convoy.AddMember(nonLeaderId, Guid.NewGuid());

        var command = new StartTripCommand
        {
            ConvoyId = convoy.Id,
            UserId = nonLeaderId,
            Status = TripStatus.Recording,
            RouteProfile = RouteProfile.Fast
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*leader*");
    }

    [Fact]
    public async Task Handle_WithActiveTripExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var convoy = CreateValidConvoy();
        var activeTrip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);

        var command = new StartTripCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Status = TripStatus.Recording,
            RouteProfile = RouteProfile.Fast
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _tripRepositoryMock
            .Setup(x => x.GetActiveByConvoyIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTrip);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*déjà en cours*");
    }

    #endregion
}
