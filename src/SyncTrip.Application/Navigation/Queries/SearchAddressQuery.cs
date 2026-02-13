using MediatR;
using SyncTrip.Shared.DTOs.Navigation;

namespace SyncTrip.Application.Navigation.Queries;

public record SearchAddressQuery : IRequest<IList<AddressResultDto>>
{
    public string Query { get; init; } = string.Empty;
    public int Limit { get; init; } = 5;
}
