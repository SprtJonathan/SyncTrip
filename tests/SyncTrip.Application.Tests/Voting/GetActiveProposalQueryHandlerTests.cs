using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Voting.Queries;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Voting;

/// <summary>
/// Tests unitaires pour le handler GetActiveProposalQuery.
/// </summary>
public class GetActiveProposalQueryHandlerTests
{
    private readonly Mock<IStopProposalRepository> _proposalRepositoryMock;
    private readonly Mock<ILogger<GetActiveProposalQueryHandler>> _loggerMock;
    private readonly GetActiveProposalQueryHandler _handler;

    public GetActiveProposalQueryHandlerTests()
    {
        _proposalRepositoryMock = new Mock<IStopProposalRepository>();
        _loggerMock = new Mock<ILogger<GetActiveProposalQueryHandler>>();

        _handler = new GetActiveProposalQueryHandler(
            _proposalRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithActiveProposal_ShouldReturnDto()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var proposal = StopProposal.Create(tripId, Guid.NewGuid(), StopType.Fuel, 48.8566, 2.3522, "Station Total");

        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposal);

        var query = new GetActiveProposalQuery(tripId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.TripId.Should().Be(tripId);
        result.Status.Should().Be((int)ProposalStatus.Pending);
        result.LocationName.Should().Be("Station Total");
    }

    [Fact]
    public async Task Handle_WithNoActiveProposal_ShouldReturnNull()
    {
        // Arrange
        var tripId = Guid.NewGuid();

        _proposalRepositoryMock
            .Setup(x => x.GetPendingByTripIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StopProposal?)null);

        var query = new GetActiveProposalQuery(tripId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
