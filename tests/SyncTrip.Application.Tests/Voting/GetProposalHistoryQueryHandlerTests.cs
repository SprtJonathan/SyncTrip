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
/// Tests unitaires pour le handler GetProposalHistoryQuery.
/// </summary>
public class GetProposalHistoryQueryHandlerTests
{
    private readonly Mock<IStopProposalRepository> _proposalRepositoryMock;
    private readonly Mock<ILogger<GetProposalHistoryQueryHandler>> _loggerMock;
    private readonly GetProposalHistoryQueryHandler _handler;

    public GetProposalHistoryQueryHandlerTests()
    {
        _proposalRepositoryMock = new Mock<IStopProposalRepository>();
        _loggerMock = new Mock<ILogger<GetProposalHistoryQueryHandler>>();

        _handler = new GetProposalHistoryQueryHandler(
            _proposalRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithProposals_ShouldReturnList()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var proposal1 = StopProposal.Create(tripId, Guid.NewGuid(), StopType.Fuel, 48.8566, 2.3522, "Station Total");
        var proposal2 = StopProposal.Create(tripId, Guid.NewGuid(), StopType.Break, 45.0, 3.0, "Aire de repos");

        _proposalRepositoryMock
            .Setup(x => x.GetByTripIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StopProposal> { proposal1, proposal2 });

        var query = new GetProposalHistoryQuery(tripId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].TripId.Should().Be(tripId);
        result[1].TripId.Should().Be(tripId);
    }

    [Fact]
    public async Task Handle_WithNoProposals_ShouldReturnEmptyList()
    {
        // Arrange
        var tripId = Guid.NewGuid();

        _proposalRepositoryMock
            .Setup(x => x.GetByTripIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StopProposal>());

        var query = new GetProposalHistoryQuery(tripId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
