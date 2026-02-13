using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

public class StopTypeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int stopType)
        {
            return stopType switch
            {
                1 => "Carburant",
                2 => "Pause",
                3 => "Repas",
                4 => "Photo",
                _ => "Inconnu"
            };
        }
        return "Inconnu";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
