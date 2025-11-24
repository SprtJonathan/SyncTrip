using MediatR;
using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.Application.Brands.Queries;

/// <summary>
/// Query pour récupérer toutes les marques de véhicules disponibles.
/// </summary>
public record GetBrandsQuery : IRequest<IList<BrandDto>>
{
}
