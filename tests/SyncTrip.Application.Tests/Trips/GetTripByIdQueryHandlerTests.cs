using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Trips.Queries;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Trips;

/// <summary>
/// Tests unitaires pour le handler GetTripByIdQuery.
/// </summary>
public class GetTripByIdQueryHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<GetTripByIdQueryHandler>> _loggerMock;
    private readonly GetTripByIdQueryHandler _handler;

    public GetTripByIdQueryHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<GetTripByIdQueryHandler>>();

        _handler = new GetTripByIdQueryHandler(
            _tripRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingTrip_ShouldReturnTripDetails()
    {
        // Arrange
        var convoyId = Guid.NewGuid();
        var trip = Trip.Create(convoyId, TripStatus.Recording, RouteProfile.Fast);
        var query = new GetTripByIdQuery(trip.Id);

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(trip.Id);
        result.ConvoyId.Should().Be(convoyId);
        result.Status.Should().Be((int)TripStatus.Recording);
        result.RouteProfile.Should().Be((int)RouteProfile.Fast);
    }

    [Fact]
    public async Task Handle_WithNonExistentTrip_ShouldReturnNull()
    {
        // Arrange
        var query = new GetTripByIdQuery(Guid.NewGuid());

        _tripRepositoryMock
            .Setup(x => x.GetByIdAsync(query.TripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
