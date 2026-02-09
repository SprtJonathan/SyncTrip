using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Trip.
/// </summary>
public class TripTests
{
    private readonly Guid _validConvoyId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateTrip()
    {
        // Act
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Assert
        trip.Should().NotBeNull();
        trip.Id.Should().NotBe(Guid.Empty);
        trip.ConvoyId.Should().Be(_validConvoyId);
        trip.Status.Should().Be(TripStatus.Recording);
        trip.RouteProfile.Should().Be(RouteProfile.Fast);
        trip.Waypoints.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithRecordingStatus_ShouldSucceed()
    {
        // Act
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Assert
        trip.Status.Should().Be(TripStatus.Recording);
    }

    [Fact]
    public void Create_WithMonitorOnlyStatus_ShouldSucceed()
    {
        // Act
        var trip = Trip.Create(_validConvoyId, TripStatus.MonitorOnly, RouteProfile.Scenic);

        // Assert
        trip.Status.Should().Be(TripStatus.MonitorOnly);
    }

    [Fact]
    public void Create_WithFinishedStatus_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => Trip.Create(_validConvoyId, TripStatus.Finished, RouteProfile.Fast);
        act.Should().Throw<DomainException>()
            .WithMessage("*Terminé*");
    }

    [Fact]
    public void Create_WithEmptyConvoyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Trip.Create(Guid.Empty, TripStatus.Recording, RouteProfile.Fast);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*convoi*");
    }

    [Fact]
    public void Create_ShouldSetStartTime()
    {
        // Act
        var before = DateTime.UtcNow;
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);
        var after = DateTime.UtcNow;

        // Assert
        trip.StartTime.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_ShouldSetEndTimeToNull()
    {
        // Act
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Assert
        trip.EndTime.Should().BeNull();
    }

    #endregion

    #region Finish

    [Fact]
    public void Finish_ShouldSetStatusToFinishedAndSetEndTime()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Act
        var before = DateTime.UtcNow;
        trip.Finish();
        var after = DateTime.UtcNow;

        // Assert
        trip.Status.Should().Be(TripStatus.Finished);
        trip.EndTime.Should().NotBeNull();
        trip.EndTime!.Value.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Finish_AlreadyFinished_ShouldThrowDomainException()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);
        trip.Finish();

        // Act & Assert
        var act = () => trip.Finish();
        act.Should().Throw<DomainException>()
            .WithMessage("*déjà terminé*");
    }

    #endregion

    #region AddWaypoint

    [Fact]
    public void AddWaypoint_WithValidData_ShouldAddWaypoint()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Act
        var waypoint = trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);

        // Assert
        trip.Waypoints.Should().HaveCount(1);
        waypoint.Name.Should().Be("Paris");
        waypoint.TripId.Should().Be(trip.Id);
    }

    [Fact]
    public void AddWaypoint_OnFinishedTrip_ShouldThrowDomainException()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);
        trip.Finish();

        // Act & Assert
        var act = () => trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*voyage terminé*");
    }

    #endregion

    #region RemoveWaypoint

    [Fact]
    public void RemoveWaypoint_WithValidWaypoint_ShouldRemove()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);
        var waypoint = trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);

        // Act
        trip.RemoveWaypoint(waypoint.Id);

        // Assert
        trip.Waypoints.Should().BeEmpty();
    }

    [Fact]
    public void RemoveWaypoint_NonExistent_ShouldThrowDomainException()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);

        // Act & Assert
        var act = () => trip.RemoveWaypoint(Guid.NewGuid());
        act.Should().Throw<DomainException>()
            .WithMessage("*n'existe pas*");
    }

    [Fact]
    public void RemoveWaypoint_OnFinishedTrip_ShouldThrowDomainException()
    {
        // Arrange
        var trip = Trip.Create(_validConvoyId, TripStatus.Recording, RouteProfile.Fast);
        var waypoint = trip.AddWaypoint(0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);
        trip.Finish();

        // Act & Assert
        var act = () => trip.RemoveWaypoint(waypoint.Id);
        act.Should().Throw<DomainException>()
            .WithMessage("*voyage terminé*");
    }

    #endregion
}
