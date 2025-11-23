using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité User.
/// </summary>
public class UserTests
{
    [Fact]
    public void Create_WithAge14_ShouldThrowDomainException()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-14));

        // Act & Assert
        var act = () => User.Create("test@email.com", "TestUser", birthDate);
        act.Should().Throw<DomainException>()
            .WithMessage("*14 ans*");
    }

    [Fact]
    public void Create_WithAge15_ShouldSucceed()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-15).AddDays(-1));

        // Act
        var user = User.Create("test@email.com", "TestUser", birthDate);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be("test@email.com");
        user.Username.Should().Be("TestUser");
        user.BirthDate.Should().Be(birthDate);
        user.IsActive.Should().BeTrue();
        user.CalculateAge().Should().Be(15);
    }

    [Theory]
    [InlineData(-10)] // 10 ans
    [InlineData(-14)] // 14 ans
    [InlineData(-13)] // 13 ans
    [InlineData(-5)]  // 5 ans
    public void Create_WithAgeUnderOrEqual14_ShouldThrowDomainException(int yearsToSubtract)
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(yearsToSubtract));

        // Act & Assert
        var act = () => User.Create("test@email.com", "TestUser", birthDate);
        act.Should().Throw<DomainException>()
            .WithMessage("*14 ans*");
    }

    [Theory]
    [InlineData(-15)] // 15 ans
    [InlineData(-16)] // 16 ans
    [InlineData(-20)] // 20 ans
    [InlineData(-50)] // 50 ans
    public void Create_WithAgeOver14_ShouldSucceed(int yearsToSubtract)
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(yearsToSubtract).AddDays(-1));

        // Act
        var user = User.Create("test@email.com", "TestUser", birthDate);

        // Assert
        user.Should().NotBeNull();
        user.CalculateAge().Should().BeGreaterThan(14);
    }

    [Fact]
    public void Create_WithOptionalFields_ShouldSetThemCorrectly()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = User.Create("test@email.com", "TestUser", birthDate, firstName, lastName);

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
    }

    [Fact]
    public void SetBirthDate_WithAge14_ShouldThrowDomainException()
    {
        // Arrange
        var user = User.Create("test@email.com", "TestUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-14));

        // Act & Assert
        var act = () => user.SetBirthDate(newBirthDate);
        act.Should().Throw<DomainException>()
            .WithMessage("*14 ans*");
    }

    [Fact]
    public void SetBirthDate_WithValidAge_ShouldUpdateBirthDate()
    {
        // Arrange
        var user = User.Create("test@email.com", "TestUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        var newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));

        // Act
        user.SetBirthDate(newBirthDate);

        // Assert
        user.BirthDate.Should().Be(newBirthDate);
        user.CalculateAge().Should().Be(25);
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateFields()
    {
        // Arrange
        var user = User.Create("test@email.com", "TestUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

        // Act
        user.UpdateProfile("NewUsername", "Jane", "Smith", "https://avatar.url");

        // Assert
        user.Username.Should().Be("NewUsername");
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.AvatarUrl.Should().Be("https://avatar.url");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndSetDeactivationDate()
    {
        // Arrange
        var user = User.Create("test@email.com", "TestUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.DeactivationDate.Should().NotBeNull();
        user.DeactivationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reactivate_ShouldSetIsActiveToTrueAndClearDeactivationDate()
    {
        // Arrange
        var user = User.Create("test@email.com", "TestUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));
        user.Deactivate();

        // Act
        user.Reactivate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.DeactivationDate.Should().BeNull();
    }

    [Fact]
    public void CalculateAge_ShouldReturnCorrectAge()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30).AddDays(-1));
        var user = User.Create("test@email.com", "TestUser", birthDate);

        // Act
        var age = user.CalculateAge();

        // Assert
        age.Should().Be(30);
    }
}
