using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Chat.Queries;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Chat;

/// <summary>
/// Tests unitaires pour le handler GetConvoyMessagesQuery.
/// </summary>
public class GetConvoyMessagesQueryHandlerTests
{
    private readonly Mock<IConvoyRepository> _convoyRepositoryMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<ILogger<GetConvoyMessagesQueryHandler>> _loggerMock;
    private readonly GetConvoyMessagesQueryHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public GetConvoyMessagesQueryHandlerTests()
    {
        _convoyRepositoryMock = new Mock<IConvoyRepository>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _loggerMock = new Mock<ILogger<GetConvoyMessagesQueryHandler>>();

        _handler = new GetConvoyMessagesQueryHandler(
            _convoyRepositoryMock.Object,
            _messageRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    private Convoy CreateConvoy()
    {
        return Convoy.Create(_validLeaderId, _validVehicleId, false);
    }

    private void SetupConvoyRepository(Convoy convoy)
    {
        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);
    }

    private Message CreateMessage(Guid convoyId, Guid senderId, string content)
    {
        var message = Message.Create(convoyId, senderId, content);
        // Set Sender via reflection for mapping test
        var sender = User.Create($"user@test.com", "testuser", new DateOnly(1990, 1, 1));
        typeof(User).GetProperty("Id")!.SetValue(sender, senderId);
        typeof(Message).GetProperty("Sender")!.SetValue(message, sender);
        return message;
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnMessages()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);

        var messages = new List<Message>
        {
            CreateMessage(convoy.Id, _validLeaderId, "Message 1"),
            CreateMessage(convoy.Id, _validLeaderId, "Message 2")
        };

        _messageRepositoryMock
            .Setup(x => x.GetByConvoyIdAsync(convoy.Id, 50, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var query = new GetConvoyMessagesQuery
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            PageSize = 50
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Content.Should().Be("Message 1");
        result[1].Content.Should().Be("Message 2");
    }

    [Fact]
    public async Task Handle_WithNoMessages_ShouldReturnEmptyList()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);

        _messageRepositoryMock
            .Setup(x => x.GetByConvoyIdAsync(convoy.Id, 50, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Message>());

        var query = new GetConvoyMessagesQuery
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            PageSize = 50
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentConvoy_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _convoyRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        var query = new GetConvoyMessagesQuery
        {
            ConvoyId = Guid.NewGuid(),
            UserId = _validLeaderId,
            PageSize = 50
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);

        var query = new GetConvoyMessagesQuery
        {
            ConvoyId = convoy.Id,
            UserId = Guid.NewGuid(), // Non-member
            PageSize = 50
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    #endregion
}
