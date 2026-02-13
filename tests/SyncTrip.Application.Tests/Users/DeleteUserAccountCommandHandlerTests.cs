using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Users.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Users;

public class DeleteUserAccountCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<DeleteUserAccountCommandHandler>> _loggerMock;
    private readonly DeleteUserAccountCommandHandler _handler;

    public DeleteUserAccountCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<DeleteUserAccountCommandHandler>>();

        _handler = new DeleteUserAccountCommandHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingUser_ShouldDeleteSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new DeleteUserAccountCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new DeleteUserAccountCommand(userId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*introuvable*");

        _userRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenDeleteThrows_ShouldPropagateException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erreur de suppression"));

        var command = new DeleteUserAccountCommand(userId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erreur de suppression");
    }
}
