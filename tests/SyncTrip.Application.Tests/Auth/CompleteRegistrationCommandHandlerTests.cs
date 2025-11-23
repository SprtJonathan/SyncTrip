using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Application.Auth.Commands;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Exceptions;
using SyncTrip.Core.Interfaces;
using Xunit;

namespace SyncTrip.Application.Tests.Auth;

/// <summary>
/// Tests unitaires pour le handler CompleteRegistrationCommand.
/// </summary>
public class CompleteRegistrationCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ILogger<CompleteRegistrationCommandHandler>> _loggerMock;
    private readonly CompleteRegistrationCommandHandler _handler;

    public CompleteRegistrationCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ILogger<CompleteRegistrationCommandHandler>>();

        _handler = new CompleteRegistrationCommandHandler(
            _userRepositoryMock.Object,
            _authServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUserAndReturnJwt()
    {
        // Arrange
        var command = new CompleteRegistrationCommand
        {
            Email = "test@example.com",
            Username = "TestUser",
            FirstName = "John",
            LastName = "Doe",
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var expectedJwt = "fake-jwt-token";
        _authServiceMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns(expectedJwt);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expectedJwt);

        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync(command.Email.ToLowerInvariant().Trim(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u =>
                u.Email == command.Email.ToLowerInvariant().Trim() &&
                u.Username == command.Username.Trim() &&
                u.FirstName == command.FirstName &&
                u.LastName == command.LastName &&
                u.BirthDate == command.BirthDate
            ), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _authServiceMock.Verify(
            x => x.GenerateJwtToken(It.IsAny<User>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithAge14_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CompleteRegistrationCommand
        {
            Email = "test@example.com",
            Username = "TestUser",
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-14))
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*14 ans*");
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CompleteRegistrationCommand
        {
            Email = "existing@example.com",
            Username = "TestUser",
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
        };

        var existingUser = User.Create(
            "existing@example.com",
            "ExistingUser",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
        );

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*existe déjà*");

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_ShouldNormalizeEmail()
    {
        // Arrange
        var command = new CompleteRegistrationCommand
        {
            Email = "  TEST@EXAMPLE.COM  ",
            Username = "TestUser",
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var expectedJwt = "fake-jwt-token";
        _authServiceMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns(expectedJwt);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()),
            Times.Once
        );

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => u.Email == "test@example.com"), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldTrimUsername()
    {
        // Arrange
        var command = new CompleteRegistrationCommand
        {
            Email = "test@example.com",
            Username = "  TestUser  ",
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var expectedJwt = "fake-jwt-token";
        _authServiceMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns(expectedJwt);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => u.Username == "TestUser"), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
