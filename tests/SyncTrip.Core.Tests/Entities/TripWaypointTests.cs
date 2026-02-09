using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité TripWaypoint.
/// </summary>
public class TripWaypointTests
{
    private readonly Guid _validTripId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateWaypoint()
    {
        // Act
        var waypoint = TripWaypoint.Create(_validTripId, 0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);

        // Assert
        waypoint.Should().NotBeNull();
        waypoint.Id.Should().NotBe(Guid.Empty);
        waypoint.TripId.Should().Be(_validTripId);
        waypoint.OrderIndex.Should().Be(0);
        waypoint.Latitude.Should().Be(48.8566);
        waypoint.Longitude.Should().Be(2.3522);
        waypoint.Name.Should().Be("Paris");
        waypoint.Type.Should().Be(WaypointType.Start);
        waypoint.AddedByUserId.Should().Be(_validUserId);
    }

    [Fact]
    public void Create_WithEmptyTripId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(Guid.Empty, 0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*voyage*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 48.8566, 2.3522, "Paris", WaypointType.Start, Guid.Empty);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*utilisateur*");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 48.8566, 2.3522, "", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*nom*");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 48.8566, 2.3522, "   ", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*nom*");
    }

    [Fact]
    public void Create_WithLatitudeTooHigh_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 91.0, 2.3522, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*latitude*");
    }

    [Fact]
    public void Create_WithLatitudeTooLow_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, -91.0, 2.3522, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*latitude*");
    }

    [Fact]
    public void Create_WithLongitudeTooHigh_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 48.8566, 181.0, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*longitude*");
    }

    [Fact]
    public void Create_WithLongitudeTooLow_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => TripWaypoint.Create(_validTripId, 0, 48.8566, -181.0, "Paris", WaypointType.Start, _validUserId);
        act.Should().Throw<DomainException>()
            .WithMessage("*longitude*");
    }

    [Theory]
    [InlineData(-90.0, -180.0)]
    [InlineData(90.0, 180.0)]
    [InlineData(0.0, 0.0)]
    public void Create_WithBoundaryValues_ShouldSucceed(double lat, double lon)
    {
        // Act
        var waypoint = TripWaypoint.Create(_validTripId, 0, lat, lon, "Test", WaypointType.Stopover, _validUserId);

        // Assert
        waypoint.Latitude.Should().Be(lat);
        waypoint.Longitude.Should().Be(lon);
    }

    #endregion

    #region UpdateOrder

    [Fact]
    public void UpdateOrder_ShouldSetNewOrderIndex()
    {
        // Arrange
        var waypoint = TripWaypoint.Create(_validTripId, 0, 48.8566, 2.3522, "Paris", WaypointType.Start, _validUserId);

        // Act
        waypoint.UpdateOrder(5);

        // Assert
        waypoint.OrderIndex.Should().Be(5);
    }

    #endregion
}
