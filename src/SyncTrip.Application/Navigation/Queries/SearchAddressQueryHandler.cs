using MediatR;
using Microsoft.Extensions.Logging;
using SyncTrip.Core.Interfaces;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Queries;

public class SearchAddressQueryHandler : IRequestHandler<SearchAddressQuery, IList<AddressResultDto>>
{
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<SearchAddressQueryHandler> _logger;

    public SearchAddressQueryHandler(IGeocodingService geocodingService, ILogger<SearchAddressQueryHandler> logger)
    {
        _geocodingService = geocodingService;
        _logger = logger;
    }

    public async Task<IList<AddressResultDto>> Handle(SearchAddressQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recherche d'adresse : {Query}", request.Query);

        var results = await _geocodingService.SearchAsync(request.Query, request.Limit, cancellationToken);

        return results.Select(r => new AddressResultDto
        {
            DisplayName = r.DisplayName,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Type = r.Type
        }).ToList();
    }
}
