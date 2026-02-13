using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Navigation.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Navigation;

public class CalculateTripRouteCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IRoutingService> _routingServiceMock;
    private readonly CalculateTripRouteCommandHandler _handler;

    public CalculateTripRouteCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _routingServiceMock = new Mock<IRoutingService>();
        _handler = new CalculateTripRouteCommandHandler(
            _tripRepositoryMock.Object,
            _routingServiceMock.Object,
            new Mock<ILogger<CalculateTripRouteCommandHandler>>().Object);
    }

    private static Trip CreateTripWithWaypoints(Guid userId)
    {
        var convoy = Convoy.Create(userId, Guid.NewGuid(), false);

        var trip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);

        // Set convoy via reflection for IsMember check
        var convoyProp = typeof(Trip).GetProperty("Convoy");
        convoyProp!.SetValue(trip, convoy);

        trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, userId);
        trip.AddWaypoint(1, 45.7640, 4.8357, "Lyon", WaypointType.Destination, userId);

        return trip;
    }

    [Fact]
    public async Task Handle_WithValidTrip_ShouldCalculateAndPersistRoute()
    {
        var userId = Guid.NewGuid();
        var trip = CreateTripWithWaypoints(userId);

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _routingServiceMock
            .Setup(x => x.CalculateRouteAsync(It.IsAny<IList<(double, double)>>(), RouteProfile.Fast, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteResult("{\"type\":\"LineString\"}", 50000, 3600, new List<RouteStep>()));

        var result = await _handler.Handle(
            new CalculateTripRouteCommand { TripId = trip.Id, UserId = userId },
            CancellationToken.None);

        result.DistanceMeters.Should().Be(50000);
        trip.RouteGeometry.Should().NotBeNull();
        _tripRepositoryMock.Verify(x => x.UpdateAsync(trip, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTrip_ShouldThrowKeyNotFoundException()
    {
        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        var act = async () => await _handler.Handle(
            new CalculateTripRouteCommand { TripId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var trip = CreateTripWithWaypoints(userId);

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var act = async () => await _handler.Handle(
            new CalculateTripRouteCommand { TripId = trip.Id, UserId = Guid.NewGuid() },
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithLessThan2Waypoints_ShouldThrowInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var convoy = Convoy.Create(userId, Guid.NewGuid(), false);

        var trip = Trip.Create(convoy.Id, TripStatus.Recording, RouteProfile.Fast);
        var convoyProp = typeof(Trip).GetProperty("Convoy");
        convoyProp!.SetValue(trip, convoy);

        trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, userId);

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var act = async () => await _handler.Handle(
            new CalculateTripRouteCommand { TripId = trip.Id, UserId = userId },
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*2 waypoints*");
    }

    [Fact]
    public async Task Handle_WithFinishedTrip_ShouldThrowDomainException()
    {
        var userId = Guid.NewGuid();
        var trip = CreateTripWithWaypoints(userId);
        trip.Finish();

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _routingServiceMock
            .Setup(x => x.CalculateRouteAsync(It.IsAny<IList<(double, double)>>(), It.IsAny<RouteProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteResult("{}", 100, 60, new List<RouteStep>()));

        var act = async () => await _handler.Handle(
            new CalculateTripRouteCommand { TripId = trip.Id, UserId = userId },
            CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*terminé*");
    }
}
