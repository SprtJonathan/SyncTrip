using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Convertit un enum VehicleType en texte lisible en français.
/// </summary>
public class VehicleTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int vehicleType)
            return "Inconnu";

        return vehicleType switch
        {
            1 => "Voiture",
            2 => "Moto",
            3 => "Camion",
            4 => "Van",
            5 => "Camping-car",
            _ => "Inconnu"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
