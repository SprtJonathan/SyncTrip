using SyncTrip.Api.Application.DTOs.Waypoints;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Trips;

/// <summary>
/// DTO Trip
/// </summary>
public class TripDto
{
    public Guid Id { get; set; }
    public Guid ConvoyId { get; set; }
    public string? Name { get; set; }
    public string Destination { get; set; } = string.Empty;
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }
    public RoutePreference RoutePreference { get; set; }
    public TripStatus Status { get; set; }
    public DateTime? PlannedDepartureTime { get; set; }
    public DateTime? ActualDepartureTime { get; set; }
    public DateTime? PlannedArrivalTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<WaypointDto> Waypoints { get; set; } = new();
}
