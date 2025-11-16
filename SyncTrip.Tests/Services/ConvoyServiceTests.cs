using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SyncTrip.Api.Application.DTOs.Convoys;
using SyncTrip.Api.Application.Mappings;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;
using SyncTrip.Api.Infrastructure.Services;

namespace SyncTrip.Tests.Services;

/// <summary>
/// Tests unitaires pour ConvoyService
/// Focus sur: création de convoi, jointure, archivage automatique, codes uniques
/// </summary>
public class ConvoyServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<IConvoyCodeGenerator> _codeGeneratorMock;
    private readonly Mock<ILogger<ConvoyService>> _loggerMock;
    private readonly ConvoyService _sut;

    public ConvoyServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _codeGeneratorMock = new Mock<IConvoyCodeGenerator>();
        _loggerMock = new Mock<ILogger<ConvoyService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new ConvoyService(
            _unitOfWorkMock.Object,
            _mapper,
            _codeGeneratorMock.Object,
            _loggerMock.Object);
    }

    #region CreateConvoyAsync Tests

    [Fact]
    public async Task CreateConvoyAsync_WhenUserNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateConvoyRequest
        {
            Name = "Voyage en Pologne",
            VehicleName = "Peugeot 308"
        };

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _sut.CreateConvoyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Utilisateur non trouvé");
    }

    [Fact]
    public async Task CreateConvoyAsync_WhenValid_ShouldCreateConvoyWithUniqueCode()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        var request = new CreateConvoyRequest
        {
            Name = "Voyage en Pologne",
            VehicleName = "Peugeot 308"
        };

        var generatedCode = "ABC123";

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _codeGeneratorMock.Setup(x => x.GenerateUniqueCodeAsync())
            .ReturnsAsync(generatedCode);

        _unitOfWorkMock.Setup(x => x.Convoys.AddAsync(It.IsAny<Convoy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy c, CancellationToken ct) => c);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.AddAsync(It.IsAny<ConvoyParticipant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConvoyParticipant p, CancellationToken ct) => p);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Mock GetConvoyByIdAsync pour le retour
        var convoy = new Convoy
        {
            Id = Guid.NewGuid(),
            Code = generatedCode,
            Name = request.Name,
            Status = ConvoyStatus.Active,
            Participants = new List<ConvoyParticipant>
            {
                new()
                {
                    UserId = userId,
                    User = user,
                    Role = ConvoyRole.Owner,
                    VehicleName = request.VehicleName,
                    IsActive = true
                }
            }
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetConvoyParticipantsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy.Participants);

        // Act
        var result = await _sut.CreateConvoyAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(generatedCode);
        result.Name.Should().Be("Voyage en Pologne");
        result.Status.Should().Be(ConvoyStatus.Active);

        // Vérifier qu'un convoi a été créé
        _unitOfWorkMock.Verify(x => x.Convoys.AddAsync(It.Is<Convoy>(c =>
            c.Code == generatedCode &&
            c.Name == request.Name &&
            c.Status == ConvoyStatus.Active
        ), It.IsAny<CancellationToken>()), Times.Once);

        // Vérifier que le créateur a été ajouté comme Owner
        _unitOfWorkMock.Verify(x => x.ConvoyParticipants.AddAsync(It.Is<ConvoyParticipant>(p =>
            p.UserId == userId &&
            p.Role == ConvoyRole.Owner &&
            p.VehicleName == request.VehicleName &&
            p.IsActive == true
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region JoinConvoyAsync Tests

    [Fact]
    public async Task JoinConvoyAsync_WhenConvoyNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new JoinConvoyRequest
        {
            Code = "INVALID",
            VehicleName = "Tesla Model 3"
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        // Act
        var act = async () => await _sut.JoinConvoyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Convoi non trouvé");
    }

    [Fact]
    public async Task JoinConvoyAsync_WhenConvoyNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new JoinConvoyRequest
        {
            Code = "ABC123",
            VehicleName = "Tesla Model 3"
        };

        var convoy = new Convoy
        {
            Id = Guid.NewGuid(),
            Code = "ABC123",
            Name = "Test Convoy",
            Status = ConvoyStatus.Archived // Convoi archivé
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        // Act
        var act = async () => await _sut.JoinConvoyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ce convoi n'est plus actif");
    }

    [Fact]
    public async Task JoinConvoyAsync_WhenUserAlreadyMember_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new JoinConvoyRequest
        {
            Code = "ABC123",
            VehicleName = "Tesla Model 3"
        };

        var convoy = new Convoy
        {
            Id = Guid.NewGuid(),
            Code = "ABC123",
            Name = "Test Convoy",
            Status = ConvoyStatus.Active
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.IsUserInConvoyAsync(userId, convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Déjà membre

        // Act
        var act = async () => await _sut.JoinConvoyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Vous êtes déjà membre de ce convoi");
    }

    [Fact]
    public async Task JoinConvoyAsync_WhenValid_ShouldAddUserAsMember()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();
        var request = new JoinConvoyRequest
        {
            Code = "ABC123",
            VehicleName = "Tesla Model 3"
        };

        var convoy = new Convoy
        {
            Id = convoyId,
            Code = "ABC123",
            Name = "Test Convoy",
            Status = ConvoyStatus.Active,
            Participants = new List<ConvoyParticipant>()
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.IsUserInConvoyAsync(userId, convoy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.AddAsync(It.IsAny<ConvoyParticipant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConvoyParticipant p, CancellationToken ct) => p);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetConvoyParticipantsAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy.Participants);

        // Act
        var result = await _sut.JoinConvoyAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("ABC123");

        // Vérifier que le participant a été ajouté comme Member
        _unitOfWorkMock.Verify(x => x.ConvoyParticipants.AddAsync(It.Is<ConvoyParticipant>(p =>
            p.UserId == userId &&
            p.ConvoyId == convoyId &&
            p.Role == ConvoyRole.Member &&
            p.VehicleName == request.VehicleName &&
            p.IsActive == true
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LeaveConvoyAsync Tests

    [Fact]
    public async Task LeaveConvoyAsync_WhenUserNotMember_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetUserParticipationAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConvoyParticipant?)null);

        // Act
        var act = async () => await _sut.LeaveConvoyAsync(userId, convoyId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Vous n'êtes pas membre de ce convoi");
    }

    [Fact]
    public async Task LeaveConvoyAsync_WhenLastParticipant_ShouldArchiveConvoy()
    {
        // Arrange - RÈGLE: Auto-archivage si le convoi devient vide
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();

        var participation = new ConvoyParticipant
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ConvoyId = convoyId,
            Role = ConvoyRole.Owner,
            IsActive = true
        };

        var convoy = new Convoy
        {
            Id = convoyId,
            Code = "ABC123",
            Name = "Test Convoy",
            Status = ConvoyStatus.Active,
            ArchivedAt = null
        };

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetUserParticipationAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participation);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.CountActiveParticipantsAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // Plus aucun participant actif après le départ

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        await _sut.LeaveConvoyAsync(userId, convoyId);

        // Assert
        participation.IsActive.Should().BeFalse();
        participation.LeftAt.Should().NotBeNull();
        participation.LeftAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        convoy.Status.Should().Be(ConvoyStatus.Archived);
        convoy.ArchivedAt.Should().NotBeNull();
        convoy.ArchivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _unitOfWorkMock.Verify(x => x.ConvoyParticipants.Update(participation), Times.Once);
        _unitOfWorkMock.Verify(x => x.Convoys.Update(convoy), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LeaveConvoyAsync_WhenOtherParticipantsRemain_ShouldNotArchiveConvoy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var convoyId = Guid.NewGuid();

        var participation = new ConvoyParticipant
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ConvoyId = convoyId,
            Role = ConvoyRole.Member,
            IsActive = true
        };

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetUserParticipationAsync(userId, convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participation);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.CountActiveParticipantsAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2); // Il reste 2 participants actifs

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.LeaveConvoyAsync(userId, convoyId);

        // Assert
        participation.IsActive.Should().BeFalse();

        // Le convoi NE DOIT PAS être archivé
        _unitOfWorkMock.Verify(x => x.Convoys.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Convoys.Update(It.IsAny<Convoy>()), Times.Never);
    }

    #endregion

    #region DeleteConvoyAsync Tests

    [Fact]
    public async Task DeleteConvoyAsync_WhenConvoyNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var convoyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        // Act
        var act = async () => await _sut.DeleteConvoyAsync(convoyId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Convoi non trouvé");
    }

    [Fact]
    public async Task DeleteConvoyAsync_WhenValid_ShouldSoftDeleteConvoy()
    {
        // Arrange - SOFT DELETE
        var convoyId = Guid.NewGuid();
        var convoy = new Convoy
        {
            Id = convoyId,
            Code = "ABC123",
            Name = "Test Convoy",
            Status = ConvoyStatus.Active,
            DeletedAt = null
        };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.DeleteConvoyAsync(convoyId);

        // Assert
        convoy.Status.Should().Be(ConvoyStatus.Deleted);
        convoy.DeletedAt.Should().NotBeNull();
        convoy.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _unitOfWorkMock.Verify(x => x.Convoys.Update(convoy), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Vérifier que le convoi n'a PAS été physiquement supprimé
        _unitOfWorkMock.Verify(x => x.Convoys.Remove(It.IsAny<Convoy>()), Times.Never);
    }

    #endregion

    #region UpdateConvoyAsync Tests

    [Fact]
    public async Task UpdateConvoyAsync_WhenConvoyNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var convoyId = Guid.NewGuid();
        var request = new UpdateConvoyRequest { Name = "Nouveau nom" };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Convoy?)null);

        // Act
        var act = async () => await _sut.UpdateConvoyAsync(convoyId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Convoi non trouvé");
    }

    [Fact]
    public async Task UpdateConvoyAsync_WhenValid_ShouldUpdateConvoy()
    {
        // Arrange
        var convoyId = Guid.NewGuid();
        var convoy = new Convoy
        {
            Id = convoyId,
            Code = "ABC123",
            Name = "Ancien nom",
            Status = ConvoyStatus.Active,
            Participants = new List<ConvoyParticipant>()
        };

        var request = new UpdateConvoyRequest { Name = "Nouveau nom" };

        _unitOfWorkMock.Setup(x => x.Convoys.GetByIdAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(x => x.ConvoyParticipants.GetConvoyParticipantsAsync(convoyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(convoy.Participants);

        // Act
        var result = await _sut.UpdateConvoyAsync(convoyId, request);

        // Assert
        convoy.Name.Should().Be("Nouveau nom");

        _unitOfWorkMock.Verify(x => x.Convoys.Update(convoy), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
