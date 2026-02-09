using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Convoy.
/// </summary>
public class ConvoyTests
{
    private readonly Guid _validLeaderId = Guid.NewGuid();
    private readonly Guid _validVehicleId = Guid.NewGuid();

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateConvoy()
    {
        // Act
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Assert
        convoy.Should().NotBeNull();
        convoy.Id.Should().NotBe(Guid.Empty);
        convoy.JoinCode.Should().HaveLength(6);
        convoy.LeaderUserId.Should().Be(_validLeaderId);
        convoy.IsPrivate.Should().BeFalse();
        convoy.Members.Should().HaveCount(1);
    }

    [Fact]
    public void Create_ShouldSetLeaderAsMember()
    {
        // Act
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, true);

        // Assert
        var leader = convoy.Members.First();
        leader.UserId.Should().Be(_validLeaderId);
        leader.VehicleId.Should().Be(_validVehicleId);
        leader.Role.Should().Be(ConvoyRole.Leader);
    }

    [Fact]
    public void Create_WithPrivateTrue_ShouldBePrivate()
    {
        // Act
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, true);

        // Assert
        convoy.IsPrivate.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyLeaderId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Convoy.Create(Guid.Empty, _validVehicleId, false);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*leader*");
    }

    [Fact]
    public void Create_WithEmptyVehicleId_ShouldThrowDomainException()
    {
        // Act & Assert
        var act = () => Convoy.Create(_validLeaderId, Guid.Empty, false);
        act.Should().Throw<DomainException>()
            .WithMessage("*véhicule*");
    }

    [Fact]
    public void Create_ShouldGenerateUniqueJoinCodes()
    {
        // Act
        var convoy1 = Convoy.Create(Guid.NewGuid(), Guid.NewGuid(), false);
        var convoy2 = Convoy.Create(Guid.NewGuid(), Guid.NewGuid(), false);

        // Assert - Les codes devraient être différents (probabilité très élevée)
        convoy1.JoinCode.Should().NotBe(convoy2.JoinCode);
    }

    [Fact]
    public void Create_JoinCode_ShouldContainOnlyValidCharacters()
    {
        // Act
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Assert - Pas de caractères ambigus (0, O, 1, I, L)
        convoy.JoinCode.Should().MatchRegex("^[ABCDEFGHJKMNPQRSTUVWXYZ23456789]{6}$");
    }

    #endregion

    #region AddMember

    [Fact]
    public void AddMember_WithValidData_ShouldAddMember()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var newUserId = Guid.NewGuid();
        var newVehicleId = Guid.NewGuid();

        // Act
        var member = convoy.AddMember(newUserId, newVehicleId);

        // Assert
        convoy.Members.Should().HaveCount(2);
        member.UserId.Should().Be(newUserId);
        member.VehicleId.Should().Be(newVehicleId);
        member.Role.Should().Be(ConvoyRole.Member);
    }

    [Fact]
    public void AddMember_AlreadyMember_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert - Le leader est déjà membre
        var act = () => convoy.AddMember(_validLeaderId, Guid.NewGuid());
        act.Should().Throw<DomainException>()
            .WithMessage("*déjà membre*");
    }

    #endregion

    #region RemoveMember

    [Fact]
    public void RemoveMember_WithValidMember_ShouldRemove()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var memberId = Guid.NewGuid();
        convoy.AddMember(memberId, Guid.NewGuid());

        // Act
        convoy.RemoveMember(memberId);

        // Assert
        convoy.Members.Should().HaveCount(1);
        convoy.IsMember(memberId).Should().BeFalse();
    }

    [Fact]
    public void RemoveMember_LeaderCantLeave_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        var act = () => convoy.RemoveMember(_validLeaderId);
        act.Should().Throw<DomainException>()
            .WithMessage("*leader*");
    }

    [Fact]
    public void RemoveMember_NotMember_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        var act = () => convoy.RemoveMember(Guid.NewGuid());
        act.Should().Throw<DomainException>()
            .WithMessage("*pas membre*");
    }

    #endregion

    #region KickMember

    [Fact]
    public void KickMember_AsLeader_ShouldRemoveMember()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var memberId = Guid.NewGuid();
        convoy.AddMember(memberId, Guid.NewGuid());

        // Act
        convoy.KickMember(_validLeaderId, memberId);

        // Assert
        convoy.Members.Should().HaveCount(1);
        convoy.IsMember(memberId).Should().BeFalse();
    }

    [Fact]
    public void KickMember_AsNonLeader_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var memberId = Guid.NewGuid();
        convoy.AddMember(memberId, Guid.NewGuid());

        // Act & Assert
        var act = () => convoy.KickMember(memberId, _validLeaderId);
        act.Should().Throw<DomainException>()
            .WithMessage("*leader*");
    }

    [Fact]
    public void KickMember_LeaderKicksSelf_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        var act = () => convoy.KickMember(_validLeaderId, _validLeaderId);
        act.Should().Throw<DomainException>()
            .WithMessage("*s'exclure*");
    }

    #endregion

    #region TransferLeadership

    [Fact]
    public void TransferLeadership_ToValidMember_ShouldTransfer()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var newLeaderId = Guid.NewGuid();
        convoy.AddMember(newLeaderId, Guid.NewGuid());

        // Act
        convoy.TransferLeadership(_validLeaderId, newLeaderId);

        // Assert
        convoy.LeaderUserId.Should().Be(newLeaderId);
        convoy.Members.First(m => m.UserId == newLeaderId).Role.Should().Be(ConvoyRole.Leader);
        convoy.Members.First(m => m.UserId == _validLeaderId).Role.Should().Be(ConvoyRole.Member);
    }

    [Fact]
    public void TransferLeadership_ToSelf_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        var act = () => convoy.TransferLeadership(_validLeaderId, _validLeaderId);
        act.Should().Throw<DomainException>()
            .WithMessage("*déjà le leader*");
    }

    [Fact]
    public void TransferLeadership_ToNonMember_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        var act = () => convoy.TransferLeadership(_validLeaderId, Guid.NewGuid());
        act.Should().Throw<DomainException>()
            .WithMessage("*membre du convoi*");
    }

    [Fact]
    public void TransferLeadership_AsNonLeader_ShouldThrowDomainException()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);
        var memberId = Guid.NewGuid();
        convoy.AddMember(memberId, Guid.NewGuid());

        // Act & Assert
        var act = () => convoy.TransferLeadership(memberId, Guid.NewGuid());
        act.Should().Throw<DomainException>()
            .WithMessage("*leader*");
    }

    #endregion

    #region IsMember

    [Fact]
    public void IsMember_WithLeader_ShouldReturnTrue()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        convoy.IsMember(_validLeaderId).Should().BeTrue();
    }

    [Fact]
    public void IsMember_WithNonMember_ShouldReturnFalse()
    {
        // Arrange
        var convoy = Convoy.Create(_validLeaderId, _validVehicleId, false);

        // Act & Assert
        convoy.IsMember(Guid.NewGuid()).Should().BeFalse();
    }

    #endregion
}
