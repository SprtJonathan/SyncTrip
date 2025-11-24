using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.Application.Brands.Queries;

/// <summary>
/// Handler pour récupérer toutes les marques de véhicules.
/// </summary>
public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, IList<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly ILogger<GetBrandsQueryHandler> _logger;

    public GetBrandsQueryHandler(
        IBrandRepository brandRepository,
        ILogger<GetBrandsQueryHandler> logger)
    {
        _brandRepository = brandRepository;
        _logger = logger;
    }

    /// <summary>
    /// Récupère toutes les marques disponibles.
    /// </summary>
    /// <param name="request">Query vide.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste de toutes les marques.</returns>
    public async Task<IList<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Récupération de toutes les marques");

        var brands = await _brandRepository.GetAllAsync(cancellationToken);

        return brands.Select(b => new BrandDto
        {
            Id = b.Id,
            Name = b.Name,
            LogoUrl = b.LogoUrl
        }).ToList();
    }
}
