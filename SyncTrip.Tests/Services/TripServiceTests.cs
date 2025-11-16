using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Api.Application.DTOs.Trips;
using SyncTrip.Api.Application.DTOs.Waypoints;
using SyncTrip.Api.Application.Mappings;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Services;

namespace SyncTrip.Tests.Services;

/// <summary>
/// Tests unitaires pour TripService
/// Focus sur la règle critique: UN SEUL TRIP ACTIF PAR CONVOI
/// </summary>
public class TripServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TripService>> _loggerMock;
    private readonly TripService _sut; // System Under Test

    public TripServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TripService>>();

        // Configuration AutoMapper avec le vrai profil
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new TripService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    #region CreateTripAsync Tests

    [Fact]
    public async Task CreateTripAsync_WhenUserIsNotMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();
        var request = new CreateTripRequest
        {
            ConvoyId = convoyId,
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522
        };

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.IsUserInConvoyAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _sut.CreateTripAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Vous devez être membre du convoi pour créer un trip");
    }

    [Fact]
    public async Task CreateTripAsync_WhenConvoyHasActiveTrip_ShouldThrowInvalidOperationException()
    {
        // Arrange - RÈGLE CRITIQUE: UN SEUL TRIP ACTIF
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();
        var request = new CreateTripRequest
        {
            ConvoyId = convoyId,
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522
        };

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.IsUserInConvoyAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(x => x.Trips.HasActiveTripAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Un trip actif existe déjà

        // Act
        var act = async () => await _sut.CreateTripAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ce convoi a déjà un trip actif. Terminez-le avant d'en créer un nouveau.");
    }

    [Fact]
    public async Task CreateTripAsync_WhenValid_ShouldCreateTripWithPlannedStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();
        var request = new CreateTripRequest
        {
            ConvoyId = convoyId,
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522,
            RoutePreference = RoutePreference.Fastest,
            PlannedDepartureTime = DateTime.UtcNow.AddDays(1)
        };

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.IsUserInConvoyAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(x => x.Trips.HasActiveTripAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Pas de trip actif

        _unitOfWorkMock.Setup(x => x.Trips.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip trip, CancellationToken ct) => trip);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.CreateTripAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.ConvoyId.Should().Be(convoyId);
        result.Destination.Should().Be("Paris");
        result.Status.Should().Be(TripStatus.Planned);

        _unitOfWorkMock.Verify(x => x.Trips.AddAsync(It.Is<Trip>(t =>
            t.ConvoyId == convoyId &&
            t.Status == TripStatus.Planned &&
            t.Destination == "Paris"
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateTripStatusAsync Tests

    [Fact]
    public async Task UpdateTripStatusAsync_WhenTripNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var request = new UpdateTripStatusRequest { Status = TripStatus.InProgress };

        _unitOfWorkMock.Setup(x => x.Trips.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act
        var act = async () => await _sut.UpdateTripStatusAsync(tripId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Trip non trouvé");
    }

    [Fact]
    public async Task UpdateTripStatusAsync_FromPlannedToInProgress_ShouldSetActualDepartureTime()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var trip = new Trip
        {
            Id = tripId,
            ConvoyId = Guid.NewGuid(),
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522,
            Status = TripStatus.Planned,
            ActualDepartureTime = null
        };

        var request = new UpdateTripStatusRequest { Status = TripStatus.InProgress };

        _unitOfWorkMock.Setup(x => x.Trips.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateTripStatusAsync(tripId, request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TripStatus.InProgress);

        trip.ActualDepartureTime.Should().NotBeNull();
        trip.ActualDepartureTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _unitOfWorkMock.Verify(x => x.Trips.Update(trip), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTripStatusAsync_ToCompleted_ShouldSetActualArrivalTime()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var trip = new Trip
        {
            Id = tripId,
            ConvoyId = Guid.NewGuid(),
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522,
            Status = TripStatus.InProgress,
            ActualDepartureTime = DateTime.UtcNow.AddHours(-2),
            ActualArrivalTime = null
        };

        var request = new UpdateTripStatusRequest { Status = TripStatus.Completed };

        _unitOfWorkMock.Setup(x => x.Trips.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateTripStatusAsync(tripId, request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TripStatus.Completed);

        trip.ActualArrivalTime.Should().NotBeNull();
        trip.ActualArrivalTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _unitOfWorkMock.Verify(x => x.Trips.Update(trip), Times.Once);
    }

    #endregion

    #region AddWaypointAsync Tests

    [Fact]
    public async Task AddWaypointAsync_WhenTripNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var request = new CreateWaypointRequest
        {
            Name = "Aire de repos A1",
            Latitude = 50.0,
            Longitude = 3.0
        };

        _unitOfWorkMock.Setup(x => x.Trips.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act
        var act = async () => await _sut.AddWaypointAsync(tripId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Trip non trouvé");
    }

    [Fact]
    public async Task AddWaypointAsync_WhenValid_ShouldAddWaypointWithCorrectOrder()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var trip = new Trip
        {
            Id = tripId,
            ConvoyId = Guid.NewGuid(),
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522,
            Status = TripStatus.Planned
        };

        var existingWaypoints = new List<Waypoint>
        {
            new() { Id = Guid.NewGuid(), TripId = tripId, Order = 0, Name = "Waypoint 1" },
            new() { Id = Guid.NewGuid(), TripId = tripId, Order = 1, Name = "Waypoint 2" }
        };

        var request = new CreateWaypointRequest
        {
            Name = "Waypoint 3",
            Latitude = 50.0,
            Longitude = 3.0
        };

        _unitOfWorkMock.Setup(x => x.Trips.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _unitOfWorkMock.Setup(x => x.Waypoints.GetTripWaypointsAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingWaypoints);

        _unitOfWorkMock.Setup(x => x.Waypoints.AddAsync(It.IsAny<Waypoint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Waypoint w, CancellationToken ct) => w);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.AddWaypointAsync(tripId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Waypoint 3");
        result.Order.Should().Be(2); // Devrait être après les 2 waypoints existants

        _unitOfWorkMock.Verify(x => x.Waypoints.AddAsync(It.Is<Waypoint>(w =>
            w.TripId == tripId &&
            w.Order == 2 &&
            w.Name == "Waypoint 3"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region MarkWaypointReachedAsync Tests

    [Fact]
    public async Task MarkWaypointReachedAsync_WhenWaypointNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var waypointId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Waypoints.GetByIdAsync(waypointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Waypoint?)null);

        // Act
        var act = async () => await _sut.MarkWaypointReachedAsync(waypointId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Waypoint non trouvé");
    }

    [Fact]
    public async Task MarkWaypointReachedAsync_WhenAlreadyReached_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var waypointId = Guid.NewGuid();
        var waypoint = new Waypoint
        {
            Id = waypointId,
            TripId = Guid.NewGuid(),
            Name = "Waypoint 1",
            Latitude = 50.0,
            Longitude = 3.0,
            Order = 0,
            IsReached = true, // Déjà atteint
            ReachedAt = DateTime.UtcNow.AddMinutes(-30)
        };

        _unitOfWorkMock.Setup(x => x.Waypoints.GetByIdAsync(waypointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waypoint);

        // Act
        var act = async () => await _sut.MarkWaypointReachedAsync(waypointId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ce waypoint a déjà été atteint");
    }

    [Fact]
    public async Task MarkWaypointReachedAsync_WhenValid_ShouldMarkAsReachedWithTimestamp()
    {
        // Arrange
        var waypointId = Guid.NewGuid();
        var waypoint = new Waypoint
        {
            Id = waypointId,
            TripId = Guid.NewGuid(),
            Name = "Waypoint 1",
            Latitude = 50.0,
            Longitude = 3.0,
            Order = 0,
            IsReached = false,
            ReachedAt = null
        };

        _unitOfWorkMock.Setup(x => x.Waypoints.GetByIdAsync(waypointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waypoint);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.MarkWaypointReachedAsync(waypointId);

        // Assert
        waypoint.IsReached.Should().BeTrue();
        waypoint.ReachedAt.Should().NotBeNull();
        waypoint.ReachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _unitOfWorkMock.Verify(x => x.Waypoints.Update(waypoint), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetTripByIdAsync Tests

    [Fact]
    public async Task GetTripByIdAsync_WhenTripExists_ShouldReturnTripDto()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var trip = new Trip
        {
            Id = tripId,
            ConvoyId = Guid.NewGuid(),
            Destination = "Paris",
            DestinationLatitude = 48.8566,
            DestinationLongitude = 2.3522,
            Status = TripStatus.Planned
        };

        _unitOfWorkMock.Setup(x => x.Trips.GetTripWithWaypointsAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        // Act
        var result = await _sut.GetTripByIdAsync(tripId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tripId);
        result.Destination.Should().Be("Paris");
    }

    [Fact]
    public async Task GetTripByIdAsync_WhenTripNotFound_ShouldReturnNull()
    {
        // Arrange
        var tripId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Trips.GetTripWithWaypointsAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act
        var result = await _sut.GetTripByIdAsync(tripId);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
