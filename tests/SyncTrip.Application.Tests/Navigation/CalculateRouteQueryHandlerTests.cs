using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Navigation.Queries;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Navigation;
using Xunit;

namespace SyncTrip.Application.Tests.Navigation;

public class CalculateRouteQueryHandlerTests
{
    private readonly Mock<IRoutingService> _routingServiceMock;
    private readonly CalculateRouteQueryHandler _handler;

    public CalculateRouteQueryHandlerTests()
    {
        _routingServiceMock = new Mock<IRoutingService>();
        _handler = new CalculateRouteQueryHandler(
            _routingServiceMock.Object,
            new Mock<ILogger<CalculateRouteQueryHandler>>().Object);
    }

    [Fact]
    public async Task Handle_WithValidWaypoints_ShouldReturnRouteResult()
    {
        _routingServiceMock
            .Setup(x => x.CalculateRouteAsync(It.IsAny<IList<(double, double)>>(), RouteProfile.Fast, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteResult("{\"type\":\"LineString\"}", 50000, 3600, new List<RouteStep>()));

        var result = await _handler.Handle(new CalculateRouteQuery
        {
            RouteProfile = RouteProfile.Fast,
            Waypoints = new List<WaypointCoordinate>
            {
                new() { Latitude = 48.8566, Longitude = 2.3522 },
                new() { Latitude = 45.7640, Longitude = 4.8357 }
            }
        }, CancellationToken.None);

        result.DistanceMeters.Should().Be(50000);
        result.DurationSeconds.Should().Be(3600);
        result.GeometryGeoJson.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WithScenicProfile_ShouldPassScenicToService()
    {
        _routingServiceMock
            .Setup(x => x.CalculateRouteAsync(It.IsAny<IList<(double, double)>>(), RouteProfile.Scenic, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteResult("{}", 60000, 4200, new List<RouteStep>()));

        await _handler.Handle(new CalculateRouteQuery
        {
            RouteProfile = RouteProfile.Scenic,
            Waypoints = new List<WaypointCoordinate>
            {
                new() { Latitude = 48.8566, Longitude = 2.3522 },
                new() { Latitude = 45.7640, Longitude = 4.8357 }
            }
        }, CancellationToken.None);

        _routingServiceMock.Verify(x => x.CalculateRouteAsync(
            It.IsAny<IList<(double, double)>>(), RouteProfile.Scenic, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapStepsCorrectly()
    {
        var steps = new List<RouteStep>
        {
            new("turn-right", 500, 30, "Rue de la Paix")
        };

        _routingServiceMock
            .Setup(x => x.CalculateRouteAsync(It.IsAny<IList<(double, double)>>(), It.IsAny<RouteProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteResult("{}", 500, 30, steps));

        var result = await _handler.Handle(new CalculateRouteQuery
        {
            RouteProfile = RouteProfile.Fast,
            Waypoints = new List<WaypointCoordinate>
            {
                new() { Latitude = 48.0, Longitude = 2.0 },
                new() { Latitude = 49.0, Longitude = 3.0 }
            }
        }, CancellationToken.None);

        result.Steps.Should().HaveCount(1);
        result.Steps[0].Instruction.Should().Be("turn-right");
        result.Steps[0].Name.Should().Be("Rue de la Paix");
    }
}
