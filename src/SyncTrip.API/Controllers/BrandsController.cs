using MediatR;
using Microsoft.AspNetCore.Mvc;
using SyncTrip.Application.Brands.Queries;
using SyncTrip.Shared.DTOs.Brands;

namespace SyncTrip.API.Controllers;

/// <summary>
/// Contrôleur pour la gestion des marques de véhicules.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BrandsController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur marques.
    /// </summary>
    public BrandsController(IMediator mediator, ILogger<BrandsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Récupère toutes les marques de véhicules disponibles.
    /// </summary>
    /// <returns>Liste de toutes les marques.</returns>
    /// <remarks>
    /// Cet endpoint est public car la liste des marques est utilisée
    /// lors de l'ajout d'un véhicule et ne contient aucune donnée sensible.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<IList<BrandDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBrands()
    {
        _logger.LogInformation("Récupération de toutes les marques");

        var query = new GetBrandsQuery();
        var brands = await _mediator.Send(query);

        return Ok(brands);
    }
}
