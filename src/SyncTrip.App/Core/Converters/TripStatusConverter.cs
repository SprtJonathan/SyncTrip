using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

public class TripStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int status)
            return "Inconnu";

        return status switch
        {
            1 => "Enregistrement",
            2 => "Surveillance",
            3 => "Termine",
            _ => "Inconnu"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
