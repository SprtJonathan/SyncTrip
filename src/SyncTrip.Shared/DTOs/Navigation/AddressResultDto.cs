namespace SyncTrip.Shared.DTOs.Navigation;

public class AddressResultDto
{
    public string DisplayName { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Type { get; init; } = string.Empty;
}
