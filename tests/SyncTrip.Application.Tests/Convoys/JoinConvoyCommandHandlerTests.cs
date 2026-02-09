using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Convoys.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Convoys;

/// <summary>
/// Tests unitaires pour le handler JoinConvoyCommand.
/// </summary>
public class JoinConvoyCommandHandlerTests
{
    private readonly Mock<IConvoyRepository> _convoyRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ILogger<JoinConvoyCommandHandler>> _loggerMock;
    private readonly JoinConvoyCommandHandler _handler;
    private readonly Guid _leaderId = Guid.NewGuid();
    private readonly Guid _leaderVehicleId = Guid.NewGuid();
    private readonly Guid _memberId = Guid.NewGuid();
    private readonly Guid _memberVehicleId = Guid.NewGuid();

    public JoinConvoyCommandHandlerTests()
    {
        _convoyRepositoryMock = new Mock<IConvoyRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<JoinConvoyCommandHandler>>();

        _handler = new JoinConvoyCommandHandler(
            _convoyRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldJoinConvoy()
    {
        // Arrange
        var convoy = Convoy.Create(_leaderId, _leaderVehicleId, false);
        var command = new JoinConvoyCommand
        {
            JoinCode = convoy.JoinCode,
            UserId = _memberId,
            VehicleId = _memberVehicleId
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByJoinCodeAsync(convoy.JoinCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        var vehicle = Vehicle.Create(_memberId, 1, "Golf", Core.Enums.VehicleType.Car);
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_memberVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _convoyRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Convoy>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        convoy.Members.Should().HaveCount(2);
        _convoyRepositoryMock.Verify(
            x => x.UpdateAsync(convoy, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithInvalidCode_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new JoinConvoyCommand
        {
            JoinCode = "XXXXXX",
            UserId = _memberId,
            VehicleId = _memberVehicleId
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByJoinCodeAsync("XXXXXX", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyMember_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_leaderId, _leaderVehicleId, false);
        var command = new JoinConvoyCommand
        {
            JoinCode = convoy.JoinCode,
            UserId = _leaderId, // Déjà leader/membre
            VehicleId = _leaderVehicleId
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByJoinCodeAsync(convoy.JoinCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        var vehicle = Vehicle.Create(_leaderId, 1, "Clio", Core.Enums.VehicleType.Car);
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_leaderVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*déjà membre*");
    }

    [Fact]
    public async Task Handle_VehicleNotOwned_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var convoy = Convoy.Create(_leaderId, _leaderVehicleId, false);
        var command = new JoinConvoyCommand
        {
            JoinCode = convoy.JoinCode,
            UserId = _memberId,
            VehicleId = _memberVehicleId
        };

        _convoyRepositoryMock
            .Setup(x => x.GetByJoinCodeAsync(convoy.JoinCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        // Véhicule appartient à un autre
        var otherVehicle = Vehicle.Create(Guid.NewGuid(), 1, "Golf", Core.Enums.VehicleType.Car);
        _vehicleRepositoryMock
            .Setup(x => x.GetByIdAsync(_memberVehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherVehicle);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
