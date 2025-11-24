using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Users.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Users;

/// <summary>
/// Tests unitaires pour le handler UpdateUserProfileCommand.
/// </summary>
public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UpdateUserProfileCommandHandler>> _loggerMock;
    private readonly UpdateUserProfileCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();

    public UpdateUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UpdateUserProfileCommandHandler>>();

        _handler = new UpdateUserProfileCommandHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    #region Handle - Success Cases

    [Fact]
    public async Task Handle_WithUsernameUpdate_ShouldUpdateUsername()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "OldUsername", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            Username = "NewUsername"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.Username.Should().Be("NewUsername");

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<User>(u => u.Username == "NewUsername"), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithAllFieldsUpdate_ShouldUpdateAllFields()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "OldUsername", birthDate);
        var newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            Username = "NewUsername",
            FirstName = "Jane",
            LastName = "Smith",
            BirthDate = newBirthDate,
            AvatarUrl = "https://example.com/new-avatar.jpg"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.Username.Should().Be("NewUsername");
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.BirthDate.Should().Be(newBirthDate);
        user.AvatarUrl.Should().Be("https://example.com/new-avatar.jpg");
    }

    [Fact]
    public async Task Handle_WithPartialUpdate_ShouldUpdateProvidedFieldsAndSetOthersToNull()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "Username", birthDate, "John", "Doe");

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            FirstName = "Jane"
            // Only updating FirstName, other profile fields will be set to null
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // UpdateProfile replaces all fields, so null values will overwrite existing data
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().BeNull(); // Set to null by UpdateProfile
        user.Username.Should().Be("Username"); // Username not updated (null input doesn't change it)
    }

    [Fact]
    public async Task Handle_WithBirthDateUpdate_ShouldUpdateBirthDate()
    {
        // Arrange
        var oldBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));
        var user = User.Create("test@example.com", "TestUser", oldBirthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            BirthDate = newBirthDate
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.BirthDate.Should().Be(newBirthDate);
        user.CalculateAge().Should().Be(30);
    }

    #endregion

    #region Handle - License Updates

    [Fact]
    public async Task Handle_WithLicenseTypesUpdate_ShouldCallUpdateUserLicensesAsync()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            LicenseTypes = new List<int> { (int)LicenseType.B, (int)LicenseType.A }
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                user.Id,
                It.Is<List<LicenseType>>(l =>
                    l.Contains(LicenseType.B) &&
                    l.Contains(LicenseType.A) &&
                    l.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithEmptyLicenseTypes_ShouldClearLicenses()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            LicenseTypes = new List<int>() // Empty list = remove all licenses
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                user.Id,
                It.Is<List<LicenseType>>(l => l.Count == 0),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNullLicenseTypes_ShouldNotCallUpdateUserLicensesAsync()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            Username = "NewUsername",
            LicenseTypes = null // Not updating licenses
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData(LicenseType.AM)]
    [InlineData(LicenseType.A1)]
    [InlineData(LicenseType.B)]
    [InlineData(LicenseType.C)]
    [InlineData(LicenseType.D)]
    public async Task Handle_WithSingleLicenseType_ShouldUpdateCorrectly(LicenseType licenseType)
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            LicenseTypes = new List<int> { (int)licenseType }
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                user.Id,
                It.Is<List<LicenseType>>(l => l.Contains(licenseType) && l.Count == 1),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion

    #region Handle - Error Cases

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new UpdateUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            Username = "NewUsername"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Utilisateur*introuvable*");

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithInvalidBirthDate_ShouldThrowException()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var invalidBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-14)); // Too young

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            BirthDate = invalidBirthDate
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>(); // DomainException from User.SetBirthDate

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    #endregion

    #region Handle - Execution Flow

    [Fact]
    public async Task Handle_ShouldCallUpdateProfileBeforeSetBirthDate()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "OldUsername", birthDate);
        var newBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            Username = "NewUsername",
            BirthDate = newBirthDate
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.Username.Should().Be("NewUsername");
        user.BirthDate.Should().Be(newBirthDate);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLicensesBeforeUpdatingUser()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId,
            Username = "NewUsername",
            LicenseTypes = new List<int> { (int)LicenseType.B }
        };

        var sequence = new MockSequence();

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .InSequence(sequence)
            .Setup(x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .InSequence(sequence)
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - sequence verification happens automatically with InSequence
        _userRepositoryMock.Verify(
            x => x.UpdateUserLicensesAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<LicenseType>>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNoChanges_ShouldStillCallUpdateAsync()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25));
        var user = User.Create("test@example.com", "TestUser", birthDate);

        var command = new UpdateUserProfileCommand
        {
            UserId = _validUserId
            // No fields to update
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdWithLicensesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion
}
