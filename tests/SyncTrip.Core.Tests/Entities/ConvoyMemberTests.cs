using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité ConvoyMember.
/// </summary>
public class ConvoyMemberTests
{
    private readonly Guid _validConvoyId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateMember()
    {
        // Act
        var member = ConvoyMember.Create(_validConvoyId, _validUserId, _validVehicleId, ConvoyRole.Member);

        // Assert
        member.Should().NotBeNull();
        member.ConvoyId.Should().Be(_validConvoyId);
        member.UserId.Should().Be(_validUserId);
        member.VehicleId.Should().Be(_validVehicleId);
        member.Role.Should().Be(ConvoyRole.Member);
        member.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_AsLeader_ShouldHaveLeaderRole()
    {
        // Act
        var member = ConvoyMember.Create(_validConvoyId, _validUserId, _validVehicleId, ConvoyRole.Leader);

        // Assert
        member.Role.Should().Be(ConvoyRole.Leader);
    }

    [Fact]
    public void Create_WithEmptyConvoyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => ConvoyMember.Create(Guid.Empty, _validUserId, _validVehicleId, ConvoyRole.Member);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*convoi*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => ConvoyMember.Create(_validConvoyId, Guid.Empty, _validVehicleId, ConvoyRole.Member);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*utilisateur*");
    }

    [Fact]
    public void Create_WithEmptyVehicleId_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => ConvoyMember.Create(_validConvoyId, _validUserId, Guid.Empty, ConvoyRole.Member);
        act.Should().Throw<DomainException>()
            .WithMessage("*véhicule*");
    }

    #endregion

    #region PromoteToLeader / DemoteToMember

    [Fact]
    public void PromoteToLeader_ShouldSetRoleToLeader()
    {
        // Arrange
        var member = ConvoyMember.Create(_validConvoyId, _validUserId, _validVehicleId, ConvoyRole.Member);

        // Act
        member.PromoteToLeader();

        // Assert
        member.Role.Should().Be(ConvoyRole.Leader);
    }

    [Fact]
    public void DemoteToMember_ShouldSetRoleToMember()
    {
        // Arrange
        var member = ConvoyMember.Create(_validConvoyId, _validUserId, _validVehicleId, ConvoyRole.Leader);

        // Act
        member.DemoteToMember();

        // Assert
        member.Role.Should().Be(ConvoyRole.Member);
    }

    #endregion
}
