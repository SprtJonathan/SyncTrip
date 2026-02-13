using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

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
