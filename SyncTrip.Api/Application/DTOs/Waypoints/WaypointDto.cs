namespace SyncTrip.Api.Application.DTOs.Waypoints;

/// <summary>
/// DTO Waypoint
/// </summary>
public class WaypointDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Order { get; set; }
    public bool IsReached { get; set; }
    public DateTime? ReachedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
