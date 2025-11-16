using AutoMapper;
using SyncTrip.Api.Application.DTOs.Convoys;
using SyncTrip.Api.Core.Entities;
using SyncTrip.Api.Core.Enums;
using SyncTrip.Api.Core.Interfaces;

namespace SyncTrip.Api.Infrastructure.Services;

/// <summary>
/// Service de gestion des convois
/// </summary>
public class ConvoyService : IConvoyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConvoyCodeGenerator _codeGenerator;
    private readonly ILogger<ConvoyService> _logger;

    public ConvoyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConvoyCodeGenerator codeGenerator,
        ILogger<ConvoyService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _codeGenerator = codeGenerator;
        _logger = logger;
    }

    public async Task<ConvoyDto> CreateConvoyAsync(Guid userId, CreateConvoyRequest request, CancellationToken cancellationToken = default)
    {
        // Vérifier que l'utilisateur existe
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("Utilisateur non trouvé");
        }

        // Générer un code unique
        var code = await _codeGenerator.GenerateUniqueCodeAsync();

        // Créer le convoi
        var convoy = _mapper.Map<Convoy>(request);
        convoy.Code = code;
        convoy.Status = ConvoyStatus.Active;

        await _unitOfWork.Convoys.AddAsync(convoy, cancellationToken);

        // Ajouter le créateur comme Owner
        var participant = new ConvoyParticipant
        {
            ConvoyId = convoy.Id,
            UserId = userId,
            Role = ConvoyRole.Owner,
            VehicleName = request.VehicleName,
            IsActive = true
        };

        await _unitOfWork.ConvoyParticipants.AddAsync(participant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Convoi créé: {Code} par {UserId}", code, userId);

        return await GetConvoyByIdAsync(convoy.Id, cancellationToken) ?? throw new InvalidOperationException("Erreur lors de la création du convoi");
    }

    public async Task<ConvoyDto> JoinConvoyAsync(Guid userId, JoinConvoyRequest request, CancellationToken cancellationToken = default)
    {
        // Récupérer le convoi
        var convoy = await _unitOfWork.Convoys.GetByCodeAsync(request.Code, cancellationToken);
        if (convoy == null)
        {
            throw new InvalidOperationException("Convoi non trouvé");
        }

        if (convoy.Status != ConvoyStatus.Active)
        {
            throw new InvalidOperationException("Ce convoi n'est plus actif");
        }

        // Vérifier si l'utilisateur est déjà membre
        var isAlreadyMember = await _unitOfWork.ConvoyParticipants.IsUserInConvoyAsync(userId, convoy.Id, cancellationToken);
        if (isAlreadyMember)
        {
            throw new InvalidOperationException("Vous êtes déjà membre de ce convoi");
        }

        // Ajouter le participant
        var participant = new ConvoyParticipant
        {
            ConvoyId = convoy.Id,
            UserId = userId,
            Role = ConvoyRole.Member,
            VehicleName = request.VehicleName,
            IsActive = true
        };

        await _unitOfWork.ConvoyParticipants.AddAsync(participant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Utilisateur {UserId} a rejoint le convoi {Code}", userId, request.Code);

        return await GetConvoyByIdAsync(convoy.Id, cancellationToken) ?? throw new InvalidOperationException("Erreur lors de la récupération du convoi");
    }

    public async Task LeaveConvoyAsync(Guid userId, Guid convoyId, CancellationToken cancellationToken = default)
    {
        // Récupérer la participation
        var participation = await _unitOfWork.ConvoyParticipants.GetUserParticipationAsync(userId, convoyId, cancellationToken);
        if (participation == null)
        {
            throw new InvalidOperationException("Vous n'êtes pas membre de ce convoi");
        }

        // Marquer comme inactif
        participation.IsActive = false;
        participation.LeftAt = DateTime.UtcNow;
        _unitOfWork.ConvoyParticipants.Update(participation);

        // Vérifier si c'était le dernier participant
        var remainingParticipants = await _unitOfWork.ConvoyParticipants.CountActiveParticipantsAsync(convoyId, cancellationToken);
        if (remainingParticipants == 0)
        {
            // Archiver le convoi
            var convoy = await _unitOfWork.Convoys.GetByIdAsync(convoyId, cancellationToken);
            if (convoy != null)
            {
                convoy.Status = ConvoyStatus.Archived;
                convoy.ArchivedAt = DateTime.UtcNow;
                _unitOfWork.Convoys.Update(convoy);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Utilisateur {UserId} a quitté le convoi {ConvoyId}", userId, convoyId);
    }

    public async Task<ConvoyDto?> GetConvoyByIdAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        var convoy = await _unitOfWork.Convoys.GetByIdAsync(convoyId, cancellationToken);
        if (convoy == null)
        {
            return null;
        }

        // Charger les participants
        var participants = await _unitOfWork.ConvoyParticipants.GetConvoyParticipantsAsync(convoyId, cancellationToken);
        convoy.Participants = participants.ToList();

        return _mapper.Map<ConvoyDto>(convoy);
    }

    public async Task<ConvoyDto?> GetConvoyByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var convoy = await _unitOfWork.Convoys.GetByCodeAsync(code, cancellationToken);
        return convoy != null ? _mapper.Map<ConvoyDto>(convoy) : null;
    }

    public async Task<IEnumerable<ConvoyDto>> GetUserConvoysAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var convoys = await _unitOfWork.Convoys.GetUserActiveConvoysAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<ConvoyDto>>(convoys);
    }

    public async Task<ConvoyDto> UpdateConvoyAsync(Guid convoyId, UpdateConvoyRequest request, CancellationToken cancellationToken = default)
    {
        var convoy = await _unitOfWork.Convoys.GetByIdAsync(convoyId, cancellationToken);
        if (convoy == null)
        {
            throw new InvalidOperationException("Convoi non trouvé");
        }

        _mapper.Map(request, convoy);
        _unitOfWork.Convoys.Update(convoy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Convoi {ConvoyId} mis à jour", convoyId);

        return await GetConvoyByIdAsync(convoyId, cancellationToken) ?? throw new InvalidOperationException("Erreur lors de la mise à jour");
    }

    public async Task DeleteConvoyAsync(Guid convoyId, CancellationToken cancellationToken = default)
    {
        var convoy = await _unitOfWork.Convoys.GetByIdAsync(convoyId, cancellationToken);
        if (convoy == null)
        {
            throw new InvalidOperationException("Convoi non trouvé");
        }

        convoy.Status = ConvoyStatus.Deleted;
        convoy.DeletedAt = DateTime.UtcNow;
        _unitOfWork.Convoys.Update(convoy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Convoi {ConvoyId} supprimé", convoyId);
    }
}
