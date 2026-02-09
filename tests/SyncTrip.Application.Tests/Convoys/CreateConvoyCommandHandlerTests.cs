using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Convoys.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Convoys;

/// <summary>
/// Tests unitaires pour le handler CreateConvoyCommand.
/// </summary>
public class CreateConvoyCommandHandlerTests
{
    private readonly Mock<IConvoyRepository> _convoyRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ILogger<CreateConvoyCommandHandler>> _loggerMock;
    private readonly CreateConvoyCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    public CreateConvoyCommandHandlerTests()
    {
        _convoyRepositoryMock = new Mock<IConvoyRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<CreateConvoyCommandHandler>>();

        _handler = new CreateConvoyCommandHandler(
            _convoyRepositoryMock.Object,
            _userRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    private User CreateValidUser() =>
        User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

    private Vehicle CreateValidVehicle() =>
        Vehicle.Create(_validUserId, 1, "Clio", Core.Enums.VehicleType.Car);

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateConvoyAndReturnId()
    {
        // Arrange
        var command = new CreateConvoyCommand
        {
            UserId = _validUserId,
            VehicleId = _validVehicleId,
            IsPrivate = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateValidUser());

        var vehicle = CreateValidVehicle();
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_validVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _convoyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Convoy>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _convoyRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Convoy>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithPrivateConvoy_ShouldCreatePrivateConvoy()
    {
        // Arrange
        var command = new CreateConvoyCommand
        {
            UserId = _validUserId,
            VehicleId = _validVehicleId,
            IsPrivate = true
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateValidUser());

        var vehicle = CreateValidVehicle();
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_validVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _convoyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Convoy>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _convoyRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Convoy>(c => c.IsPrivate), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new CreateConvoyCommand
        {
            UserId = _validUserId,
            VehicleId = _validVehicleId,
            IsPrivate = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNonExistentVehicle_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new CreateConvoyCommand
        {
            UserId = _validUserId,
            VehicleId = _validVehicleId,
            IsPrivate = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateValidUser());

        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_validVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithVehicleNotOwnedByUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var command = new CreateConvoyCommand
        {
            UserId = _validUserId,
            VehicleId = _validVehicleId,
            IsPrivate = false
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(_validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateValidUser());

        // Véhicule appartient à un autre utilisateur
        var otherVehicle = Vehicle.Create(otherUserId, 1, "Golf", Core.Enums.VehicleType.Car);
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_validVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherVehicle);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    #endregion
}
