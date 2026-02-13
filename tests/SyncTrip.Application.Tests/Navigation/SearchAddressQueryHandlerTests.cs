using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Navigation.Queries;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Navigation;

public class SearchAddressQueryHandlerTests
{
    private readonly Mock<IGeocodingService> _geocodingServiceMock;
    private readonly SearchAddressQueryHandler _handler;

    public SearchAddressQueryHandlerTests()
    {
        _geocodingServiceMock = new Mock<IGeocodingService>();
        _handler = new SearchAddressQueryHandler(
            _geocodingServiceMock.Object,
            new Mock<ILogger<SearchAddressQueryHandler>>().Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnResults()
    {
        _geocodingServiceMock
            .Setup(x => x.SearchAsync("Lyon", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GeocodingResult>
            {
                new("Lyon, France", 45.7640, 4.8357, "city", 0.9)
            });

        var result = await _handler.Handle(
            new SearchAddressQuery { Query = "Lyon", Limit = 5 }, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].DisplayName.Should().Be("Lyon, France");
        result[0].Latitude.Should().Be(45.7640);
        result[0].Longitude.Should().Be(4.8357);
    }

    [Fact]
    public async Task Handle_WithEmptyResults_ShouldReturnEmptyList()
    {
        _geocodingServiceMock
            .Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GeocodingResult>());

        var result = await _handler.Handle(
            new SearchAddressQuery { Query = "xyznonexistent" }, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldPassLimitToService()
    {
        _geocodingServiceMock
            .Setup(x => x.SearchAsync("Paris", 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GeocodingResult>());

        await _handler.Handle(
            new SearchAddressQuery { Query = "Paris", Limit = 3 }, CancellationToken.None);

        _geocodingServiceMock.Verify(x => x.SearchAsync("Paris", 3, It.IsAny<CancellationToken>()), Times.Once);
    }
}
