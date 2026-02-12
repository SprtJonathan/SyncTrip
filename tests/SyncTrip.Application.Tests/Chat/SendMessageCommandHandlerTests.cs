using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Chat.Commands;
using SyncTrip.Application.Chat.Services;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Chat;
using Xunit;

namespace SyncTrip.Application.Tests.Chat;

/// <summary>
/// Tests unitaires pour le handler SendMessageCommand.
/// </summary>
public class SendMessageCommandHandlerTests
{
    private readonly Mock<IConvoyRepository> _convoyRepositoryMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConvoyNotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<SendMessageCommandHandler>> _loggerMock;
    private readonly SendMessageCommandHandler _handler;
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public SendMessageCommandHandlerTests()
    {
        _convoyRepositoryMock = new Mock<IConvoyRepository>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationServiceMock = new Mock<IConvoyNotificationService>();
        _loggerMock = new Mock<ILogger<SendMessageCommandHandler>>();

        _handler = new SendMessageCommandHandler(
            _convoyRepositoryMock.Object,
            _messageRepositoryMock.Object,
            _userRepositoryMock.Object,
            _notificationServiceMock.Object,
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

    private void SetupUserRepository(Guid userId, string username, string? avatarUrl = null)
    {
        var user = User.Create($"{username}@test.com", username, new DateOnly(1990, 1, 1));
        typeof(User).GetProperty("Id")!.SetValue(user, userId);
        typeof(User).GetProperty("AvatarUrl")!.SetValue(user, avatarUrl);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateMessageAndReturnId()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);
        SetupUserRepository(_validLeaderId, "leader");

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new SendMessageCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Content = "Salut tout le monde !"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _messageRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotifyNewMessage()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);
        SetupUserRepository(_validLeaderId, "leader");

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new SendMessageCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Content = "Test notification"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyNewMessageAsync(convoy.Id, It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapDtoWithSenderInfo()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);
        SetupUserRepository(_validLeaderId, "testuser", "https://avatar.url/test.png");

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MessageDto? sentDto = null;
        _notificationServiceMock
            .Setup(x => x.NotifyNewMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, MessageDto, CancellationToken>((_, dto, _) => sentDto = dto)
            .Returns(Task.CompletedTask);

        var command = new SendMessageCommand
        {
            ConvoyId = convoy.Id,
            UserId = _validLeaderId,
            Content = "Hello"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sentDto.Should().NotBeNull();
        sentDto!.SenderUsername.Should().Be("testuser");
        sentDto.SenderAvatarUrl.Should().Be("https://avatar.url/test.png");
        sentDto.Content.Should().Be("Hello");
        sentDto.ConvoyId.Should().Be(convoy.Id);
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

        var command = new SendMessageCommand
        {
            ConvoyId = Guid.NewGuid(),
            UserId = _validLeaderId,
            Content = "Test"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonMember_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var convoy = CreateConvoy();
        SetupConvoyRepository(convoy);

        var command = new SendMessageCommand
        {
            ConvoyId = convoy.Id,
            UserId = Guid.NewGuid(), // Non-member
            Content = "Test"
        };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    #endregion
}
