namespace SyncTrip.Api.Application.DTOs.Locations;

/// <summary>
/// DTO Position GPS
/// </summary>
public class LocationDto
{
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Altitude { get; set; }
    public double? Speed { get; set; }
    public double? Heading { get; set; }
    public double? Accuracy { get; set; }
    public DateTime Timestamp { get; set; }
}
