using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Convertit un rôle de convoi (int) en texte lisible en français.
/// </summary>
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
