using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Convertit une valeur nullable en booléen (true si non null).
/// Utilisé pour IsVisible sur des int?.
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
