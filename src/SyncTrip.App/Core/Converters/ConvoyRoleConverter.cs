using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

public class ConvoyRoleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int role)
            return "Inconnu";

        return role switch
        {
            2 => "Leader",
            _ => "Membre"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
