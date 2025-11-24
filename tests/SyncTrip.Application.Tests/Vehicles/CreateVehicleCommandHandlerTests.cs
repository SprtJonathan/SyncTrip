using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Vehicles.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Vehicles;

/// <summary>
/// Tests unitaires pour le handler CreateVehicleCommand.
/// </summary>
public class CreateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IBrandRepository> _brandRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<CreateVehicleCommandHandler>> _loggerMock;
    private readonly CreateVehicleCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();
    private const int ValidBrandId = 1;

    public CreateVehicleCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _brandRepositoryMock = new Mock<IBrandRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<CreateVehicleCommandHandler>>();

        _handler = new CreateVehicleCommandHandler(
            _vehicleRepositoryMock.Object,
            _brandRepositoryMock.Object,
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidMinimalData_ShouldCreateVehicleAndReturnId()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = ValidBrandId,
            Model = "Clio",
            Type = VehicleType.Car
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var brand = Brand.Create(ValidBrandId, "Renault", "https://example.com/logo.png");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _brandRepositoryMock.Verify(
            x => x.GetByIdAsync(command.BrandId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Vehicle>(v =>
                v.UserId == command.UserId &&
                v.BrandId == command.BrandId &&
                v.Model == command.Model &&
                v.Type == command.Type &&
                v.Color == null &&
                v.Year == null
            ), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithAllFields_ShouldCreateVehicleWithAllData()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = ValidBrandId,
            Model = "CBR 600",
            Type = VehicleType.Motorcycle,
            Color = "Rouge",
            Year = 2020
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)));
        var brand = Brand.Create(ValidBrandId, "Honda", "https://example.com/honda.png");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Vehicle>(v =>
                v.UserId == command.UserId &&
                v.BrandId == command.BrandId &&
                v.Model == command.Model &&
                v.Type == command.Type &&
                v.Color == command.Color &&
                v.Year == command.Year
            ), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Theory]
    [InlineData(VehicleType.Car)]
    [InlineData(VehicleType.Motorcycle)]
    [InlineData(VehicleType.Truck)]
    [InlineData(VehicleType.Van)]
    [InlineData(VehicleType.Motorhome)]
    public async Task Handle_WithDifferentVehicleTypes_ShouldSucceed(VehicleType vehicleType)
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = ValidBrandId,
            Model = "TestModel",
            Type = vehicleType
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var brand = Brand.Create(ValidBrandId, "TestBrand", "https://example.com/logo.png");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        _vehicleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Vehicle>(v => v.Type == vehicleType), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Validation Failures

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = Guid.NewGuid(),
            BrandId = ValidBrandId,
            Model = "Clio",
            Type = VehicleType.Car
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Utilisateur*introuvable*");

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _brandRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithNonExistentBrand_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = 999,
            Model = "Clio",
            Type = VehicleType.Car
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Marque*introuvable*");

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _brandRepositoryMock.Verify(
            x => x.GetByIdAsync(command.BrandId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithInvalidYear_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = ValidBrandId,
            Model = "Clio",
            Type = VehicleType.Car,
            Year = 1899 // Invalid year
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var brand = Brand.Create(ValidBrandId, "Renault", "https://example.com/logo.png");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*année*");

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithEmptyModel_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = ValidBrandId,
            Model = "   ", // Empty model
            Type = VehicleType.Car
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var brand = Brand.Create(ValidBrandId, "Renault", "https://example.com/logo.png");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*modèle*");

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    #endregion

    #region Handle - Execution Flow

    [Fact]
    public async Task Handle_ShouldCheckUserExistenceBeforeBrand()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = Guid.NewGuid(),
            BrandId = ValidBrandId,
            Model = "Clio",
            Type = VehicleType.Car
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();

        // Verify brand check was never called (user check failed first)
        _brandRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_ShouldVerifyValidationsBeforeAddingVehicle()
    {
        // Arrange
        var command = new CreateVehicleCommand
        {
            UserId = _validUserId,
            BrandId = 999,
            Model = "Clio",
            Type = VehicleType.Car
        };

        var user = User.Create("test@example.com", "TestUser", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _brandRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();

        // Verify vehicle was never added (brand validation failed)
        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    #endregion
}
