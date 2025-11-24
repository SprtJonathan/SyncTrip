using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité UserLicense.
/// </summary>
public class UserLicenseTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private const LicenseType ValidLicenseType = LicenseType.B;

    #region Create - Success Cases

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var userLicense = UserLicense.Create(_validUserId, ValidLicenseType);

        // Assert
        userLicense.Should().NotBeNull();
        userLicense.UserId.Should().Be(_validUserId);
        userLicense.LicenseType.Should().Be(ValidLicenseType);
        userLicense.AddedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(LicenseType.AM)]
    [InlineData(LicenseType.A1)]
    [InlineData(LicenseType.A2)]
    [InlineData(LicenseType.A)]
    [InlineData(LicenseType.B)]
    [InlineData(LicenseType.BE)]
    [InlineData(LicenseType.C)]
    [InlineData(LicenseType.CE)]
    [InlineData(LicenseType.D)]
    [InlineData(LicenseType.DE)]
    public void Create_WithAllLicenseTypes_ShouldSucceed(LicenseType licenseType)
    {
        // Arrange & Act
        var userLicense = UserLicense.Create(_validUserId, licenseType);

        // Assert
        userLicense.LicenseType.Should().Be(licenseType);
    }

    [Fact]
    public void Create_WithDifferentUserIds_ShouldSucceed()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var userLicense1 = UserLicense.Create(userId1, LicenseType.B);
        var userLicense2 = UserLicense.Create(userId2, LicenseType.A);

        // Assert
        userLicense1.UserId.Should().Be(userId1);
        userLicense2.UserId.Should().Be(userId2);
        userLicense1.UserId.Should().NotBe(userLicense2.UserId);
    }

    [Fact]
    public void Create_ShouldSetAddedAtToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var userLicense = UserLicense.Create(_validUserId, ValidLicenseType);

        // Assert
        var afterCreation = DateTime.UtcNow;
        userLicense.AddedAt.Should().BeOnOrAfter(beforeCreation);
        userLicense.AddedAt.Should().BeOnOrBefore(afterCreation);
    }

    #endregion

    #region Create - Validation Failures

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => UserLicense.Create(Guid.Empty, ValidLicenseType);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*identifiant utilisateur*")
            .WithParameterName("userId");
    }

    #endregion

    #region Multiple Licenses Per User

    [Fact]
    public void Create_SameUserWithMultipleLicenseTypes_ShouldAllowDifferentLicenses()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var license1 = UserLicense.Create(userId, LicenseType.B);
        var license2 = UserLicense.Create(userId, LicenseType.A);
        var license3 = UserLicense.Create(userId, LicenseType.BE);

        // Assert
        license1.UserId.Should().Be(userId);
        license2.UserId.Should().Be(userId);
        license3.UserId.Should().Be(userId);
        license1.LicenseType.Should().Be(LicenseType.B);
        license2.LicenseType.Should().Be(LicenseType.A);
        license3.LicenseType.Should().Be(LicenseType.BE);
    }

    #endregion

    #region Enum Value Validation

    [Fact]
    public void Create_WithValidEnumValue_ShouldSucceed()
    {
        // Arrange
        var licenseType = (LicenseType)5; // LicenseType.B

        // Act
        var userLicense = UserLicense.Create(_validUserId, licenseType);

        // Assert
        userLicense.LicenseType.Should().Be(LicenseType.B);
    }

    #endregion
}
