using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

public class TripRouteTests
{
    [Fact]
    public void UpdateRoute_WithValidData_ShouldSetProperties()
    {
        var trip = Trip.Create(Guid.NewGuid(), TripStatus.Recording, RouteProfile.Fast);

        trip.UpdateRoute("{\"type\":\"LineString\"}", 15000, 900);

        trip.RouteGeometry.Should().Be("{\"type\":\"LineString\"}");
        trip.RouteDistanceMeters.Should().Be(15000);
        trip.RouteDurationSeconds.Should().Be(900);
    }

    [Fact]
    public void UpdateRoute_WhenFinished_ShouldThrowDomainException()
    {
        var trip = Trip.Create(Guid.NewGuid(), TripStatus.Recording, RouteProfile.Fast);
        trip.Finish();

        var act = () => trip.UpdateRoute("{}", 100, 60);

        act.Should().Throw<DomainException>()
            .WithMessage("*terminé*");
    }

    [Fact]
    public void ClearRoute_ShouldSetPropertiesToNull()
    {
        var trip = Trip.Create(Guid.NewGuid(), TripStatus.Recording, RouteProfile.Fast);
        trip.UpdateRoute("{\"type\":\"LineString\"}", 15000, 900);

        trip.ClearRoute();

        trip.RouteGeometry.Should().BeNull();
        trip.RouteDistanceMeters.Should().BeNull();
        trip.RouteDurationSeconds.Should().BeNull();
    }
}
