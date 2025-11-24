using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Users.Queries;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Users;

/// <summary>
/// Tests unitaires pour le handler GetUserProfileQuery.
/// </summary>
public class GetUserProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<GetUserProfileQueryHandler>> _loggerMock;
    private readonly GetUserProfileQueryHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();

    public GetUserProfileQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<GetUserProfileQueryHandler>>();

        _handler = new GetUserProfileQueryHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithValidUserId_ShouldReturnUserProfile()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate, "John", "Doe");
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Username.Should().Be(user.Username);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.BirthDate.Should().Be(user.BirthDate);
        result.Age.Should().Be(user.CalculateAge());
        result.LicenseTypes.Should().BeEmpty();
        result.CreatedAt.Should().BeCloseTo(user.CreatedAt, TimeSpan.FromSeconds(1));

        _userRepositoryMock.Verify(
            x => x.GetByIdWithLicensesAsync(query.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithUserWithoutOptionalFields_ShouldReturnProfileWithNulls()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));
        var user = User.Create("test@example.com", "TestUser", birthDate);
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.AvatarUrl.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithUserWithAvatar_ShouldReturnAvatarUrl()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        var user = User.Create("test@example.com", "TestUser", birthDate);
        user.UpdateProfile("TestUser", "John", "Doe", "https://example.com/avatar.jpg");
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.AvatarUrl.Should().Be("https://example.com/avatar.jpg");
    }

    [Fact]
    public async Task Handle_ShouldCalculateAgeCorrectly()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-22).AddDays(-1));
        var user = User.Create("test@example.com", "TestUser", birthDate);
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Age.Should().Be(22);
    }

    #endregion

    #region Handle - With Licenses

    [Fact]
    public async Task Handle_WithUserWithOneLicense_ShouldReturnLicenseInList()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        // Use reflection to add license to private collection
        var licensesProperty = typeof(User).GetProperty("Licenses");
        var licenses = (ICollection<UserLicense>)licensesProperty!.GetValue(user)!;
        licenses.Add(UserLicense.Create(user.Id, LicenseType.B));

        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.LicenseTypes.Should().HaveCount(1);
        result.LicenseTypes.Should().Contain((int)LicenseType.B);
    }

    [Fact]
    public async Task Handle_WithUserWithMultipleLicenses_ShouldReturnAllLicenses()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        // Use reflection to add licenses to private collection
        var licensesProperty = typeof(User).GetProperty("Licenses");
        var licenses = (ICollection<UserLicense>)licensesProperty!.GetValue(user)!;
        licenses.Add(UserLicense.Create(user.Id, LicenseType.B));
        licenses.Add(UserLicense.Create(user.Id, LicenseType.A));
        licenses.Add(UserLicense.Create(user.Id, LicenseType.BE));

        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.LicenseTypes.Should().HaveCount(3);
        result.LicenseTypes.Should().Contain((int)LicenseType.B);
        result.LicenseTypes.Should().Contain((int)LicenseType.A);
        result.LicenseTypes.Should().Contain((int)LicenseType.BE);
    }

    [Fact]
    public async Task Handle_WithUserWithNoLicenses_ShouldReturnEmptyLicenseList()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18));
        var user = User.Create("test@example.com", "TestUser", birthDate);
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.LicenseTypes.Should().NotBeNull();
        result.LicenseTypes.Should().BeEmpty();
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
    public async Task Handle_WithDifferentLicenseTypes_ShouldReturnCorrectLicenseType(LicenseType licenseType)
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        // Use reflection to add license to private collection
        var licensesProperty = typeof(User).GetProperty("Licenses");
        var licenses = (ICollection<UserLicense>)licensesProperty!.GetValue(user)!;
        licenses.Add(UserLicense.Create(user.Id, licenseType));

        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.LicenseTypes.Should().Contain((int)licenseType);
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var query = new GetUserProfileQuery(Guid.NewGuid());

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Utilisateur*introuvable*");

        _userRepositoryMock.Verify(
            x => x.GetByIdWithLicensesAsync(query.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithEmptyUserId_ShouldCallRepositoryWithEmptyGuid()
    {
        // Arrange
        var query = new GetUserProfileQuery(Guid.Empty);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();

        _userRepositoryMock.Verify(
            x => x.GetByIdWithLicensesAsync(Guid.Empty, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Repository Calls

    [Fact]
    public async Task Handle_ShouldUseGetByIdWithLicensesAsync()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        var user = User.Create("test@example.com", "TestUser", birthDate);
        var query = new GetUserProfileQuery(_validUserId);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByIdWithLicensesAsync(query.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion
}
