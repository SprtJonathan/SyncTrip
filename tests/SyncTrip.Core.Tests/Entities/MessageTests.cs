using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Exceptions;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Message.
/// </summary>
public class MessageTests
{
    private readonly Guid _validConvoyId = Guid.NewGuid();
    private readonly Guid _validSenderId = Guid.NewGuid();

    #region Create - Success

    [Fact]
    public void Create_WithValidData_ShouldCreateMessage()
    {
        // Act
        var message = Message.Create(_validConvoyId, _validSenderId, "Bonjour tout le monde !");

        // Assert
        message.Should().NotBeNull();
        message.Id.Should().NotBe(Guid.Empty);
        message.ConvoyId.Should().Be(_validConvoyId);
        message.SenderId.Should().Be(_validSenderId);
        message.Content.Should().Be("Bonjour tout le monde !");
        message.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region Create - Validation Errors

    [Fact]
    public void Create_WithEmptyConvoyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        FluentActions.Invoking(() => Message.Create(Guid.Empty, _validSenderId, "Hello"))
            .Should().Throw<ArgumentException>()
            .WithParameterName("convoyId");
    }

    [Fact]
    public void Create_WithEmptySenderId_ShouldThrowArgumentException()
    {
        // Act & Assert
        FluentActions.Invoking(() => Message.Create(_validConvoyId, Guid.Empty, "Hello"))
            .Should().Throw<ArgumentException>()
            .WithParameterName("senderId");
    }

    [Fact]
    public void Create_WithEmptyContent_ShouldThrowDomainException()
    {
        // Act & Assert
        FluentActions.Invoking(() => Message.Create(_validConvoyId, _validSenderId, ""))
            .Should().Throw<DomainException>()
            .WithMessage("*vide*");
    }

    [Fact]
    public void Create_WithWhitespaceContent_ShouldThrowDomainException()
    {
        // Act & Assert
        FluentActions.Invoking(() => Message.Create(_validConvoyId, _validSenderId, "   "))
            .Should().Throw<DomainException>()
            .WithMessage("*vide*");
    }

    [Fact]
    public void Create_WithContentExceeding500Chars_ShouldThrowDomainException()
    {
        // Arrange
        var longContent = new string('A', 501);

        // Act & Assert
        FluentActions.Invoking(() => Message.Create(_validConvoyId, _validSenderId, longContent))
            .Should().Throw<DomainException>()
            .WithMessage("*500*");
    }

    [Fact]
    public void Create_WithContentExactly500Chars_ShouldSucceed()
    {
        // Arrange
        var content = new string('A', 500);

        // Act
        var message = Message.Create(_validConvoyId, _validSenderId, content);

        // Assert
        message.Content.Should().HaveLength(500);
    }

    #endregion
}
